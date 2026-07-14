namespace GarthImgLab.Models;

using System.Diagnostics.CodeAnalysis;
using ColorConverters;
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
    private readonly double _cGain, _rGain, _gGain, _bGain;
    private readonly Action<nint>? _proc;

    public unsafe Saturator(SM mode, double cGain, double rGain, double gGain, double bGain) {
        _proc = cGain switch {
            > 0 => mode switch {
                SM.HSV => Boost<Hsv>,
                SM.HSL => Boost<Hsl>,
                SM.CIELCh => Boost<CieLCh>,
                SM.JzCzhz => Boost<JzCzhz>,
                SM.OkLCh => Boost<OkLCh>,
                SM.OkLrCh => Boost<OkLrCh>,
                _ => throw new Never()
            },
            < 0 => mode switch {
                SM.HSV => Reduce<Hsv>,
                SM.HSL => Reduce<Hsl>,
                SM.CIELCh => Reduce<CieLCh>,
                SM.JzCzhz => Reduce<JzCzhz>,
                SM.OkLCh => Reduce<OkLCh>,
                SM.OkLrCh => Reduce<OkLrCh>,
                _ => throw new Never()
            },
            _ => p => {
                var rgb = (ushort*)p;
                var r = rgb[0] / Max16;
                var g = rgb[1] / Max16;
                var b = rgb[2] / Max16;
                RgbGain(ref r, ref g, ref b);
                RgbClamp(ref r, ref g, ref b);
                rgb[0] = (ushort)(r * Max16 + .5);
                rgb[1] = (ushort)(g * Max16 + .5);
                rgb[2] = (ushort)(b * Max16 + .5);
            }
        };
        _cGain = cGain;
        _rGain = rGain;
        _gGain = gGain;
        _bGain = bGain;
    }

    public void Apply(Img img, CT ct) {
        if (_proc is {}) img.MapPixel(_proc, ct);
    }

    private unsafe void Boost<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = rgb[0] / Max16;
        var g = rgb[1] / Max16;
        var b = rgb[2] / Max16;

        RgbGain(ref r, ref g, ref b);

        var min = Math.Min(r, Math.Min(g, b));
        var max = Math.Max(r, Math.Max(g, b));
        var d = 2 * Math.Min(min, 1 - max);
        var mask = Math.Pow(d, .2);

        var (l, c, h) = T.FromSRgb(r, g, b);

        var maxC = T.GetCusp(l, h);
        var x = c / maxC;
        var s = _cGain * mask;
        var y = x / (1 - s + s * x);
        c = y * maxC;

        (r, g, b) = T.ToSRgb(l, c, h);

        RgbClamp(ref r, ref g, ref b);

        rgb[0] = (ushort)(r * Max16 + .5);
        rgb[1] = (ushort)(g * Max16 + .5);
        rgb[2] = (ushort)(b * Max16 + .5);
    }

    private unsafe void Reduce<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = rgb[0] / Max16;
        var g = rgb[1] / Max16;
        var b = rgb[2] / Max16;

        RgbGain(ref r, ref g, ref b);

        var (l, c, h) = T.FromSRgb(r, g, b);
        c *= 1 + _cGain;
        (r, g, b) = T.ToSRgb(l, c, h);

        RgbClamp(ref r, ref g, ref b);

        rgb[0] = (ushort)(r * Max16 + .5);
        rgb[1] = (ushort)(g * Max16 + .5);
        rgb[2] = (ushort)(b * Max16 + .5);
    }

    private void RgbGain(ref double r, ref double g, ref double b) {
        if (Math.Abs(_rGain - 1) < 1e-4
         && Math.Abs(_gGain - 1) < 1e-4
         && Math.Abs(_bGain - 1) < 1e-4)
            return;
        r = SRgb.LinearToSRgb(SRgb.SRgbToLinear(r) * _rGain);
        g = SRgb.LinearToSRgb(SRgb.SRgbToLinear(g) * _gGain);
        b = SRgb.LinearToSRgb(SRgb.SRgbToLinear(b) * _bGain);
    }

    private static void RgbClamp(ref double r, ref double g, ref double b) {
        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        var max = Math.Max(r, Math.Max(g, b));
        if (max <= 1) return;
        r /= max;
        g /= max;
        b /= max;
    }
}
