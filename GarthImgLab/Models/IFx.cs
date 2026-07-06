namespace GarthImgLab.Models;

using ImageMagick;

public interface IFx {
    void Apply(MagickImage img, CancellationToken ct);
}
