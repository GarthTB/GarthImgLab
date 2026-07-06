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

public static class SatBooster {
    private const double Max16 = 65535;

    public static void Boost(
        MagickImage img,
        SatBoostMode mode,
        double strength,
        CancellationToken ct) {
        Action<nint> proc = mode switch {
            SatBoostMode.HSV => p => Proc<Hsv>(p, strength),
            SatBoostMode.HSL => p => Proc<Hsl>(p, strength),
            SatBoostMode.CIELCh => p => Proc<CieLCh>(p, strength),
            SatBoostMode.JzCzhz => p => Proc<JzCzhz>(p, strength),
            SatBoostMode.OkLCh => p => Proc<OkLCh>(p, strength),
            SatBoostMode.OkLrCh => p => Proc<OkLrCh>(p, strength),
            _ => throw new UnreachableException()
        };
        img.MapPixel(proc, ct);
    }

    private static unsafe void Proc<T>(nint p, double strength) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = rgb[0] / Max16;
        var g = rgb[1] / Max16;
        var b = rgb[2] / Max16;

        var min = Math.Min(r, Math.Min(g, b));
        var max = Math.Max(r, Math.Max(g, b));
        var t = Math.Max(0, 2 * Math.Min(min, 1 - max));
        var mask = t * t * (3 - 2 * t);

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
