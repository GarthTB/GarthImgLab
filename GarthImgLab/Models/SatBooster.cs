namespace GarthImgLab.Models;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ColorConverters;
using ImageMagick;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum SatBoostMode: byte {
    HSV,
    HSL,
    CIELCh,
    JzCzhz,
    OkLCh,
    OkLrCh
}

public sealed class SatBooster(SatBoostMode mode, double strength): IFx {
    private const double Max16 = 65535;

    public void Apply(MagickImage img, CancellationToken ct) {
        Action<nint> proc = mode switch {
            SatBoostMode.HSV => Proc<Hsv>,
            SatBoostMode.HSL => Proc<Hsl>,
            SatBoostMode.CIELCh => Proc<CieLCh>,
            SatBoostMode.JzCzhz => Proc<JzCzhz>,
            SatBoostMode.OkLCh => Proc<OkLCh>,
            SatBoostMode.OkLrCh => Proc<OkLrCh>,
            _ => throw new UnreachableException()
        };
        img.MapPixel(proc, ct);
    }

    private unsafe void Proc<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = rgb[0] / Max16;
        var g = rgb[1] / Max16;
        var b = rgb[2] / Max16;

        var min = Math.Min(r, Math.Min(g, b));
        var max = Math.Max(r, Math.Max(g, b));
        var x = Math.Min(min, 1 - max);
        var mask = 4 * x * (1 - x);
        if (mask < 0) mask = 0;

        var (l, c, h) = T.FromSRgb(r, g, b);
        c += (T.MaxSat - c) * strength * mask;
        (r, g, b) = T.ToSRgb(l, c, h);

        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        max = Math.Max(r, Math.Max(g, b));
        if (max > 1) {
            r /= max;
            g /= max;
            b /= max;
        }

        rgb[0] = (ushort)(r * Max16 + .5);
        rgb[1] = (ushort)(g * Max16 + .5);
        rgb[2] = (ushort)(b * Max16 + .5);
    }
}
