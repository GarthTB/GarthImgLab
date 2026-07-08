namespace GarthImgLab.Models;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ColorConverters;
using ImageMagick;
using SM = SaturateMode;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum SaturateMode: byte {
    HSV,
    HSL,
    CIELCh,
    JzCzhz,
    OkLCh,
    OkLrCh
}

public sealed class Saturator: IFx {
    private const double Max16 = 65535;
    private readonly Action<nint>? _proc;
    private readonly double _strength;

    public Saturator(SM mode, double strength) {
        _strength = strength;
        _proc = strength switch {
            > 0 => mode switch {
                SM.HSV => Boost<Hsv>,
                SM.HSL => Boost<Hsl>,
                SM.CIELCh => Boost<CieLCh>,
                SM.JzCzhz => Boost<JzCzhz>,
                SM.OkLCh => Boost<OkLCh>,
                SM.OkLrCh => Boost<OkLrCh>,
                _ => throw new UnreachableException()
            },
            < 0 => mode switch {
                SM.HSV => Reduce<Hsv>,
                SM.HSL => Reduce<Hsl>,
                SM.CIELCh => Reduce<CieLCh>,
                SM.JzCzhz => Reduce<JzCzhz>,
                SM.OkLCh => Reduce<OkLCh>,
                SM.OkLrCh => Reduce<OkLrCh>,
                _ => throw new UnreachableException()
            },
            _ => null
        };
    }

    public void Apply(MagickImage img, CancellationToken ct) {
        if (_proc is {}) img.MapPixel(_proc, ct);
    }

    private unsafe void Boost<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = rgb[0] / Max16;
        var g = rgb[1] / Max16;
        var b = rgb[2] / Max16;

        var min = Math.Min(r, Math.Min(g, b));
        var max = Math.Max(r, Math.Max(g, b));
        var mask = 2 * Math.Min(min, 1 - max);

        var (l, c, h) = T.FromSRgb(r, g, b);

        var x = c / T.MaxSat;
        var s = _strength * mask;
        var y = x / (1 - s + s * x);
        c = y * T.MaxSat;

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

    private unsafe void Reduce<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = rgb[0] / Max16;
        var g = rgb[1] / Max16;
        var b = rgb[2] / Max16;

        var (l, c, h) = T.FromSRgb(r, g, b);
        c *= 1 + _strength;
        (r, g, b) = T.ToSRgb(l, c, h);

        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        var max = Math.Max(r, Math.Max(g, b));
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
