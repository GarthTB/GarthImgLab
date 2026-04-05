// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.VMs;

using System.ComponentModel;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using TabVMs;
using CTS = CancellationTokenSource;

internal sealed partial class PreviewVM: ObservableObject, IDisposable {
    #region 属性和响应

    public FXTabVM? CurFXTabVM {
        get;
        set {
            var old = field;
            if (!SetProperty(ref field, value)) return;
            old?.PropertyChanged -= OnFxPropChanged;
            value?.PropertyChanged += OnFxPropChanged;
            _ = GenAftAsync();
        }
    }

    private MImg? Thumb {
        get;
        set {
            var old = field;
            if (!SetProperty(ref field, value)) return;
            old?.Dispose();
            _ = GenBefAsync();
            _ = GenAftAsync();
        }
    }

    [ObservableProperty] public partial BitmapSource? Preview { get; private set; }
    [ObservableProperty] private partial BitmapSource? Bef { get; set; }
    partial void OnBefChanged(BitmapSource? value) => SetPreview();
    [ObservableProperty] private partial BitmapSource? Aft { get; set; }
    partial void OnAftChanged(BitmapSource? value) => SetPreview();
    [ObservableProperty] public partial bool Visible { get; set; }
    partial void OnVisibleChanged(bool value) => SetPreview();

    private void OnFxPropChanged(object? s, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(FXTabVM.Enabled))
            SetPreview();
        else
            _ = GenAftAsync();
    }

    #endregion 属性和响应

    #region 打断和销毁

    private CTS _srcCts = new(), _befCts = new(), _aftCts = new();

    public void CancelAll() {
        _ = RenewSrc();
        _ = RenewBef();
        _ = RenewAft();
    }

    private async Task<CTS> RenewSrc() {
        await _srcCts.CancelAsync();
        _srcCts.Dispose();
        return _srcCts = new();
    }

    private async Task<CTS> RenewBef() {
        await _befCts.CancelAsync();
        _befCts.Dispose();
        return _befCts = new();
    }

    private async Task<CTS> RenewAft() {
        await _aftCts.CancelAsync();
        _aftCts.Dispose();
        return _aftCts = new();
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

    public async Task SetSrcAsync(string? path) {
        try {
            var ct = (await RenewSrc()).Token;
            Thumb = path is {}
                ? await Task.Run(
                    async () => {
                        MImg src = new();
                        await src.ReadAsync(path, ct);
                        ct.ThrowIfCancellationRequested();
                        src.ToThumb(1 << 18);
                        ct.ThrowIfCancellationRequested();
                        return src;
                    },
                    ct)
                : null;
        } catch (OCE) {} catch (Exception ex) { Show($"加载预览源时：\n{ex}", "异常", OK, Error); }
    }

    private async Task GenBefAsync() {
        try {
            var ct = (await RenewBef()).Token;
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
        } catch (OCE) {} catch (Exception ex) { Show($"预览原图时：\n{ex}", "异常", OK, Error); }
    }

    private async Task GenAftAsync() {
        try {
            var ct = (await RenewAft()).Token;
            await Task.Delay(150, ct); // 防抖
            Aft = Visible && Thumb is {} t && CurFXTabVM is {} fx
                ? await Task.Run(
                    () => {
                        using var clone = (MImg)t.CloneArea(t.Width, t.Height);
                        ct.ThrowIfCancellationRequested();
                        fx.Apply(clone, ct);
                        ct.ThrowIfCancellationRequested();
                        var aft = clone.ToBitmapSource();
                        aft.Freeze();
                        ct.ThrowIfCancellationRequested();
                        return aft;
                    },
                    ct)
                : null;
        } catch (Exception ex) {
            if (ex is not (OCE or { InnerException: OCE })) Show($"预览效果时：\n{ex}", "异常", OK, Error);
        }
    }

    private void SetPreview() =>
        Preview = Visible && CurFXTabVM is {} fx
            ? fx.Enabled
                ? Aft
                : Bef
            : null;

    #endregion 渲染方法
}
