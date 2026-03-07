// ReSharper disable UnusedParameterInPartialMethod

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
    partial void OnBefChanged(BitmapSource? value) => SetPreview();
    partial void OnAftChanged(BitmapSource? value) => SetPreview();
    partial void OnVisibleChanged(bool value) => SetPreview();

    partial void OnCurFXTabVMChanged(FXTabVM? oldValue, FXTabVM? newValue) {
        oldValue?.PropertyChanged -= OnFxPropChanged;
        newValue?.PropertyChanged += OnFxPropChanged;
        _ = GenAftAsync();
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
        CurFXTabVM?.PropertyChanged -= OnFxPropChanged;
        Thumb?.Dispose();
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
                            MImg src = new();
                            await src.ReadAsync(path, ct);
                            src.ToThumb(1 << 18);
                            ct.ThrowIfCancellationRequested();
                            return src;
                        },
                        ct)
                    : null;
            });

    private Task GenBefAsync() =>
        Dialog.RunOrShowExAsync(
            "生成原图预览",
            async () => {
                var ct = await RenewBef();
                Bef = Thumb is {}
                    ? await Task.Run(
                        () => {
                            var bef = Thumb.ToBitmapSource();
                            bef.Freeze();
                            ct.ThrowIfCancellationRequested();
                            return bef;
                        },
                        ct)
                    : null;
            });

    private Task GenAftAsync() =>
        Dialog.RunOrShowExAsync(
            "生成效果预览",
            async () => {
                var ct = await RenewAft();
                await Task.Delay(128, ct); // 防抖
                Aft = Visible && Thumb is {} t && CurFXTabVM is {} vm
                    ? await Task.Run(
                        () => {
                            using var clone = (MImg)t.CloneArea(t.Width, t.Height);
                            ct.ThrowIfCancellationRequested();
                            vm.Apply(clone, ct);
                            ct.ThrowIfCancellationRequested();
                            var aft = clone.ToBitmapSource();
                            aft.Freeze();
                            ct.ThrowIfCancellationRequested();
                            return aft;
                        },
                        ct)
                    : null;
            });

    private void SetPreview() =>
        Preview = Visible && CurFXTabVM?.Enabled is {} enabled
            ? enabled
                ? Aft
                : Bef
            : null;

    #endregion 渲染方法
}
