namespace GarthImgLab.Models;

using ImageMagick;

public interface IFx {
    Task Apply(MagickImage img, CancellationToken ct);
}
