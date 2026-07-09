namespace GarthImgLab.Contexts;

using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using Models;

public sealed class WorkspaceCtx: ObservableObject, IWorkspaceCtx {
    private const int ThumbSize = 1024 * 1024, DebounceMs = 150;
    private MagickImage? _bef, _aft;
    private Bitmap? _befBmp, _aftBmp;
    private CancellationTokenSource _befCts = new(), _aftCts = new();
    private bool _on;

    public IImage? DisplayImg =>
        _on
            ? _aftBmp
            : _befBmp;

    public void Clear() {
        CancelAndGetNewCt(ref _befCts);
        CancelAndGetNewCt(ref _aftCts);
        DisposeCurrent();
        OnPropertyChanged(nameof(DisplayImg));
    }

    public void Toggle(bool on) {
        if (_on == on) return;
        _on = on;
        OnPropertyChanged(nameof(DisplayImg));
    }

    public async Task LoadBefAsync(string path) {
        var ct = CancelAndGetNewCt(ref _befCts);
        CancelAndGetNewCt(ref _aftCts);
        MagickImage bef = new();
        try {
            await bef.ReadAsync(path, ct);
            await Task.Run(() => bef.ToThumb(ThumbSize, ct), ct);
            var bmp = await ToBmpAsync(bef, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                if (ct.IsCancellationRequested) {
                    bef.Dispose();
                    bmp.Dispose();
                    return;
                }
                DisposeCurrent();
                _bef = bef;
                _befBmp = bmp;
                if (!_on) OnPropertyChanged(nameof(DisplayImg));
            });
        } catch (Exception ex) {
            bef.Dispose();
            if (ex is not OperationCanceledException) Clear();
        }
    }

    public async Task UpdateAftAsync(IReadOnlyList<IFx> fxs) {
        var ct = CancelAndGetNewCt(ref _aftCts);
        if (_bef is null || fxs.Count == 0) return;
        MagickImage? aft = null;
        try {
            await Task.Delay(DebounceMs, ct);
            aft = await Task.Run(
                () => {
                    var img = (MagickImage)_bef.CloneArea(_bef.Width, _bef.Height);
                    try {
                        foreach (var fx in fxs) fx.Apply(img, ct);
                    } catch {
                        img.Dispose();
                        throw;
                    }
                    return img;
                },
                ct);
            var bmp = await ToBmpAsync(aft, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                if (ct.IsCancellationRequested) {
                    aft.Dispose();
                    bmp.Dispose();
                    return;
                }
                _aft?.Dispose();
                _aftBmp?.Dispose();
                _aft = aft;
                _aftBmp = bmp;
                if (_on) OnPropertyChanged(nameof(DisplayImg));
            });
        } catch { aft?.Dispose(); }
    }

    private static CancellationToken CancelAndGetNewCt(ref CancellationTokenSource cts) {
        cts.Cancel();
        cts = new();
        return cts.Token;
    }

    private void DisposeCurrent() {
        _bef?.Dispose();
        _aft?.Dispose();
        _bef = _aft = null;
        _befBmp?.Dispose();
        _aftBmp?.Dispose();
        _befBmp = _aftBmp = null;
    }

    private static async Task<Bitmap> ToBmpAsync(MagickImage img, CancellationToken ct) {
        using MemoryStream ms = new();
        await img.WriteAsync(ms, MagickFormat.Bmp, ct);
        ms.Seek(0, SeekOrigin.Begin);
        return new(ms);
    }
}
