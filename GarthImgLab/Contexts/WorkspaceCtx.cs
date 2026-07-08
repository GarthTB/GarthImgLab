namespace GarthImgLab.Contexts;

using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using Models;

public sealed partial class WorkspaceCtx: ObservableObject, IWorkspaceCtx {
    private const int ThumbSize = 1024 * 1024, DebounceMs = 150;
    private MagickImage? _bef, _aft;
    private Bitmap? _befBmp, _aftBmp;
    private CancellationTokenSource _cts = new();

    [ObservableProperty, NotifyPropertyChangedFor(nameof(DisplayImg))]
    public partial bool AftActive { get; set; }

    public IImage? DisplayImg =>
        AftActive
            ? _aftBmp
            : _befBmp;

    public async Task LoadBefAsync(string path) {
        var ct = CancelAndGetNewCt();
        try {
            MagickImage bef = new();
            await bef.ReadAsync(path, ct);
            await Task.Run(() => bef.ToThumb(ThumbSize, ct), ct);
            var bmp = await ToBmpAsync(bef, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                DisposeCurrent();
                _bef = bef;
                _befBmp = bmp;
                if (!AftActive) OnPropertyChanged(nameof(DisplayImg));
            });
        } catch (OperationCanceledException) {} catch { Clear(); }
    }

    public async Task UpdateAftAsync(IReadOnlyList<IFx> fxs) {
        var ct = CancelAndGetNewCt();
        try {
            await Task.Delay(DebounceMs, ct);
            if (_bef is null) return;
            var aft = await Task.Run(
                () => {
                    var img = (MagickImage)_bef.CloneArea(_bef.Width, _bef.Height);
                    foreach (var fx in fxs) fx.Apply(img, ct);
                    return img;
                },
                ct);
            var bmp = await ToBmpAsync(aft, ct);
            await Dispatcher.UIThread.InvokeAsync(() => {
                _aft?.Dispose();
                _aftBmp?.Dispose();
                _aft = aft;
                _aftBmp = bmp;
                if (AftActive) OnPropertyChanged(nameof(DisplayImg));
            });
        } catch (OperationCanceledException) {}
    }

    public void Clear() {
        CancelAndGetNewCt();
        DisposeCurrent();
        OnPropertyChanged(nameof(DisplayImg));
    }

    private CancellationToken CancelAndGetNewCt() {
        _cts.Cancel();
        _cts.Dispose();
        return (_cts = new()).Token;
    }

    private void DisposeCurrent() {
        _bef?.Dispose();
        _aft?.Dispose();
        _befBmp?.Dispose();
        _aftBmp?.Dispose();
        _bef = _aft = null;
        _befBmp = _aftBmp = null;
    }

    private static async Task<Bitmap> ToBmpAsync(MagickImage img, CancellationToken ct) {
        using MemoryStream ms = new();
        await img.WriteAsync(ms, MagickFormat.Bmp, ct);
        ms.Seek(0, SeekOrigin.Begin);
        return new(ms);
    }
}
