namespace GarthImgLab.ViewModels;

using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using Models;

public sealed class Workspace: ObservableObject, IWorkspace {
    private const int ThumbSize = 1024 * 1024, DebounceMs = 150;
    private bool _active, _enabled;
    private MagickImage? _bef, _aft;
    private Bitmap? _befBmp, _aftBmp;
    private CancellationTokenSource _befCts = new(), _aftCts = new();

    public IImage? DisplayImg =>
        _enabled
            ? _aftBmp
            : _befBmp;

    public void Clear() {
        CancelAndGetNewCt(ref _befCts);
        DisposeBef();
        CancelAndGetNewCt(ref _aftCts);
        DisposeAft();
        OnPropertyChanged(nameof(DisplayImg));
    }

    public void SetPreviewActive(bool active) {
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
        MagickImage? bef = null;
        Bitmap? bmp = null;
        try {
            bef = new();
            await bef.ReadAsync(path, ct);
            await Task.Run(() => bef.ToThumb(ThumbSize, ct), ct);
            bmp = await ToBmpAsync(bef, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                ct.ThrowIfCancellationRequested();
                DisposeBef();
                DisposeAft();
                _bef = bef;
                _befBmp = bmp;
                OnPropertyChanged(nameof(DisplayImg));
            });
        } catch (Exception ex) {
            bef?.Dispose();
            bmp?.Dispose();
            if (ex is not OperationCanceledException) Clear();
        }
    }

    public async Task UpdateAftAsync(IReadOnlyList<IFx>? fxs) {
        if (!_active) return;
        var ct = CancelAndGetNewCt(ref _aftCts);
        if (_bef is null) return;
        if (fxs is null) {
            await Dispatcher.UIThread.InvokeAsync(() => {
                DisposeAft();
                OnPropertyChanged(nameof(DisplayImg));
            });
            return;
        }
        MagickImage? aft = null;
        Bitmap? bmp = null;
        try {
            await Task.Delay(DebounceMs, ct);
            aft = await Task.Run(
                () => {
                    var img = (MagickImage)_bef.CloneArea(_bef.Width, _bef.Height);
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
                DisposeAft();
                if (_enabled) OnPropertyChanged(nameof(DisplayImg));
            });
        } catch {
            aft?.Dispose();
            bmp?.Dispose();
        }
    }

    private static CancellationToken CancelAndGetNewCt(ref CancellationTokenSource cts) {
        cts.Cancel();
        cts.Dispose();
        return (cts = new()).Token;
    }

    private void DisposeBef() {
        _bef?.Dispose();
        _bef = null;
        _befBmp?.Dispose();
        _befBmp = null;
    }

    private void DisposeAft() {
        _aft?.Dispose();
        _aft = null;
        _aftBmp?.Dispose();
        _aftBmp = null;
    }

    private static async Task<Bitmap> ToBmpAsync(MagickImage img, CancellationToken ct) {
        using MemoryStream ms = new();
        await img.WriteAsync(ms, MagickFormat.Bmp, ct);
        ms.Seek(0, SeekOrigin.Begin);
        return new(ms);
    }
}
