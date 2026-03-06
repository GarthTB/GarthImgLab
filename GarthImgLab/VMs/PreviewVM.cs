namespace GarthImgLab.VMs;

using System.ComponentModel;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Core;
using ImageMagick;
using TabVMs;

internal sealed partial class PreviewVM: ObservableObject, IDisposable
{
    #region 属性和响应

    [ObservableProperty] private BitmapSource? _bef, _aft, _preview;
    [ObservableProperty] private FXTabVM? _curFXTabVM;
    [ObservableProperty] private MImg? _thumb;
    [ObservableProperty] private bool _visible;
    private bool Active => CurFXTabVM is {} && Visible;
    partial void OnBefChanged(BitmapSource? value) => SetPreview();
    partial void OnAftChanged(BitmapSource? value) => SetPreview();
    partial void OnVisibleChanged(bool value) => SetPreview();

    partial void OnCurFXTabVMChanged(FXTabVM? oldValue, FXTabVM? newValue) {
        oldValue?.PropertyChanged -= OnFxPropChanged;
        newValue?.PropertyChanged += OnFxPropChanged;
        if (Active) _ = GenAftAsync();
    }

    private void OnFxPropChanged(object? s, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(FXTabVM.Enabled))
            SetPreview();
        else
            _ = GenAftAsync();
    }

    partial void OnThumbChanged(MImg? oldValue, MImg? newValue) {
        oldValue?.Dispose();
        _ = (GenBefAsync(), GenAftAsync());
    }

    #endregion 属性和响应

    #region 打断和销毁

    private CTS _srcCts = new(), _befCts = new(), _aftCts = new();
    public void CancelAll() => _ = (RenewSrc(), RenewBef(), RenewAft());

    private async Task<CT> RenewSrc() {
        await _srcCts.CancelAsync();
        _srcCts.Dispose();
        return (_srcCts = new()).Token;
    }

    private async Task<CT> RenewBef() {
        await _befCts.CancelAsync();
        _befCts.Dispose();
        return (_befCts = new()).Token;
    }

    private async Task<CT> RenewAft() {
        await _aftCts.CancelAsync();
        _aftCts.Dispose();
        return (_aftCts = new()).Token;
    }

    public void Dispose() {
        _srcCts.Cancel();
        _befCts.Cancel();
        _aftCts.Cancel();
        _srcCts.Dispose();
        _befCts.Dispose();
        _aftCts.Dispose();
        Thumb?.Dispose();
        CurFXTabVM?.PropertyChanged -= OnFxPropChanged;
    }

    #endregion 打断和销毁

    #region 渲染方法

    public Task SetSrcAsync(string? path) =>
        Dialog.RunOrShowExAsync(
            "加载预览源",
            async () => {
                var ct = await RenewSrc();
                Thumb = path is {}
                    ? await Task.Run(
                        async () => {
                            MImg img = new();
                            await img.ReadAsync(path, ct);
                            img.ToThumb(1 << 18);
                            return img;
                        },
                        ct)
                    : null;
            });

    private Task GenBefAsync() =>
        Dialog.RunOrShowExAsync(
            "生成原图预览",
            async () => {
                Bef = Thumb is {}
                    ? await Task.Run(
                        () => {
                            var bef = Thumb.ToBitmapSource();
                            bef.Freeze();
                            return bef;
                        },
                        await RenewBef())
                    : null;
            });

    private Task GenAftAsync() =>
        Dialog.RunOrShowExAsync(
            "生成效果预览",
            async () => {
                var ct = await RenewAft();
                if (Active && CurFXTabVM is {} vm)
                    Aft = Thumb is {}
                        ? await Task.Run(
                            () => {
                                using MImg clone = new(Thumb);
                                ct.ThrowIfCancellationRequested();
                                vm.Apply(clone, ct);
                                ct.ThrowIfCancellationRequested();
                                var aft = clone.ToBitmapSource();
                                aft.Freeze();
                                return aft;
                            },
                            ct)
                        : null;
            });

    private void SetPreview() =>
        Preview = Active
            ? CurFXTabVM!.Enabled
                ? Aft
                : Bef
            : null;

    #endregion 渲染方法
}
