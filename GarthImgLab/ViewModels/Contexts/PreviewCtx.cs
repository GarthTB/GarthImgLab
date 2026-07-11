namespace GarthImgLab.ViewModels.Contexts;

using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using Models;

public sealed class PreviewCtx: ObservableObject, IPreviewCtx {
    private const int ThumbSize = 1024 * 1024, DebounceMs = 150;
    private bool _active, _enabled;
    private Img? _bef, _aft;
    private Bitmap? _befBmp, _aftBmp;
    private CTS _befCts = new(), _aftCts = new();

    public IImage? DisplayImg =>
        _enabled
            ? _aftBmp
            : _befBmp;

    public void Clear() {
        CancelAndGetNewCt(ref _befCts);
        DisposeImg(ref _bef, ref _befBmp);
        CancelAndGetNewCt(ref _aftCts);
        DisposeImg(ref _aft, ref _aftBmp);
        OnPropertyChanged(nameof(DisplayImg));
    }

    public void SetActive(bool active) {
        if (_active == active) return;
        _active = active;
        if (!active) Clear();
    }

    public void SetEnabled(bool enabled) {
        if (_enabled == enabled) return;
        _enabled = enabled;
        OnPropertyChanged(nameof(DisplayImg));
    }

    public async Task LoadBefAsync(string path) {
        if (!_active) return;
        var ct = CancelAndGetNewCt(ref _befCts);
        CancelAndGetNewCt(ref _aftCts);
        Img? bef = null;
        Bitmap? bmp = null;
        try {
            bef = new();
            await bef.ReadAsync(path, ct);
            await Task.Run(() => bef.ToThumb(ThumbSize, ct), ct);
            bmp = await ToBmpAsync(bef, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                ct.ThrowIfCancellationRequested();
                DisposeImg(ref _bef, ref _befBmp);
                DisposeImg(ref _aft, ref _aftBmp);
                _bef = bef;
                _befBmp = bmp;
                OnPropertyChanged(nameof(DisplayImg));
            });
        } catch (Exception ex) {
            bef?.Dispose();
            bmp?.Dispose();
            if (ex is not OCEx) Clear();
        }
    }

    public async Task UpdateAftAsync(IReadOnlyList<IFx> fxs) {
        if (!_active) return;
        var ct = CancelAndGetNewCt(ref _aftCts);
        if (_bef is null) return;
        Img? aft = null;
        Bitmap? bmp = null;
        try {
            await Task.Delay(DebounceMs, ct);
            aft = await Task.Run(
                () => {
                    var img = (Img)_bef.CloneArea(_bef.Width, _bef.Height);
                    try {
                        foreach (var fx in fxs) fx.Apply(img, ct);
                        return img;
                    } catch {
                        img.Dispose();
                        throw;
                    }
                },
                ct);
            bmp = await ToBmpAsync(aft, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                ct.ThrowIfCancellationRequested();
                DisposeImg(ref _aft, ref _aftBmp);
                _aft = aft;
                _aftBmp = bmp;
                if (_enabled) OnPropertyChanged(nameof(DisplayImg));
            });
        } catch {
            aft?.Dispose();
            bmp?.Dispose();
        }
    }

    private static CT CancelAndGetNewCt(ref CTS cts) {
        cts.Cancel();
        cts.Dispose();
        return (cts = new()).Token;
    }

    private static void DisposeImg(ref Img? img, ref Bitmap? bmp) {
        img?.Dispose();
        img = null;
        bmp?.Dispose();
        bmp = null;
    }

    private static async Task<Bitmap> ToBmpAsync(Img img, CT ct) {
        using MemoryStream ms = new();
        await img.WriteAsync(ms, MagickFormat.Bmp, ct);
        ms.Seek(0, SeekOrigin.Begin);
        return new(ms);
    }
}
