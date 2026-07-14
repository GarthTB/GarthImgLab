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
    private readonly Action<nint> _proc;

    public unsafe Saturator(SM mode, double cGain, double rGain, double gGain, double bGain) {
        _cGain = cGain;
        _rGain = rGain;
        _gGain = gGain;
        _bGain = bGain;
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
                var r = SRgb.SRgbToLinear(rgb[0] / Max16) * _rGain;
                var g = SRgb.SRgbToLinear(rgb[1] / Max16) * _gGain;
                var b = SRgb.SRgbToLinear(rgb[2] / Max16) * _bGain;
                rgb[0] = (ushort)(SRgb.LinearToSRgb(r) * Max16 + .5);
                rgb[1] = (ushort)(SRgb.LinearToSRgb(g) * Max16 + .5);
                rgb[2] = (ushort)(SRgb.LinearToSRgb(b) * Max16 + .5);
            }
        };
    }

    public void Apply(Img img, CT ct) => img.MapPixel(_proc, ct);

    private unsafe void Boost<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = SRgb.SRgbToLinear(rgb[0] / Max16) * _rGain;
        var g = SRgb.SRgbToLinear(rgb[1] / Max16) * _gGain;
        var b = SRgb.SRgbToLinear(rgb[2] / Max16) * _bGain;

        var (l, c, h) = T.FromLinearSRgb(r, g, b);

        var min = Math.Min(r, Math.Min(g, b));
        var max = Math.Max(r, Math.Max(g, b));
        var d = 2 * Math.Min(min, 1 - max);
        var mask = Math.Pow(d, .2);

        var maxC = T.GetCusp(l, h);
        var x = c / maxC;
        var s = _cGain * mask;
        var y = x / (1 - s + s * x);
        c = y * maxC;

        (r, g, b) = T.ToLinearSRgb(l, c, h);

        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        max = Math.Max(r, Math.Max(g, b));
        if (max > 1) {
            r /= max;
            g /= max;
            b /= max;
        }

        rgb[0] = (ushort)(SRgb.LinearToSRgb(r) * Max16 + .5);
        rgb[1] = (ushort)(SRgb.LinearToSRgb(g) * Max16 + .5);
        rgb[2] = (ushort)(SRgb.LinearToSRgb(b) * Max16 + .5);
    }

    private unsafe void Reduce<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = SRgb.SRgbToLinear(rgb[0] / Max16) * _rGain;
        var g = SRgb.SRgbToLinear(rgb[1] / Max16) * _gGain;
        var b = SRgb.SRgbToLinear(rgb[2] / Max16) * _bGain;

        var (l, c, h) = T.FromLinearSRgb(r, g, b);
        c *= 1 + _cGain;
        (r, g, b) = T.ToLinearSRgb(l, c, h);

        rgb[0] = (ushort)(SRgb.LinearToSRgb(r) * Max16 + .5);
        rgb[1] = (ushort)(SRgb.LinearToSRgb(g) * Max16 + .5);
        rgb[2] = (ushort)(SRgb.LinearToSRgb(b) * Max16 + .5);
    }
}
