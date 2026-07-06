namespace GarthImgLab.Models;

using ImageMagick;

public sealed class Annotator(
    IEnumerable<string> tagInfoUnions,
    string separator,
    double bRatio,
    double tRatio,
    string font,
    MagickColor c,
    string? iconPath,
    double margin): IDisposable, IFx {
    private readonly MagickImage? _icon = iconPath is {}
        ? new(iconPath)
        : null;

    public void Dispose() => _icon?.Dispose();
    public void Apply(MagickImage img, CancellationToken ct) => throw new NotImplementedException();
}
