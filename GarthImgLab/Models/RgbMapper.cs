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

public sealed class RgbMapper: IFx {
    private const double Max16 = 65535;
    private readonly double _cGain;
    private readonly double[] _gainParams, _mixParams;
    private readonly Action<nint> _proc;

    public unsafe RgbMapper(SM mode, double cGain, double[] gainParams, double[] mixParams) {
        _cGain = cGain;
        _gainParams = gainParams;
        _mixParams = mixParams;
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
                var r = SRgb.SRgbToLinear(rgb[0] / Max16);
                var g = SRgb.SRgbToLinear(rgb[1] / Max16);
                var b = SRgb.SRgbToLinear(rgb[2] / Max16);
                GainAndMix(ref r, ref g, ref b);
                rgb[0] = (ushort)(SRgb.LinearToSRgb(r) * Max16 + .5);
                rgb[1] = (ushort)(SRgb.LinearToSRgb(g) * Max16 + .5);
                rgb[2] = (ushort)(SRgb.LinearToSRgb(b) * Max16 + .5);
            }
        };
    }

    public void Apply(Img img, CT ct) => img.MapPx(_proc, ct);

    private void GainAndMix(ref double r, ref double g, ref double b) {
        var r0 = r * _gainParams[0];
        var g0 = g * _gainParams[1];
        var b0 = b * _gainParams[2];
        r = r0 * _mixParams[0] + g0 * _mixParams[1] + b0 * _mixParams[2];
        g = r0 * _mixParams[3] + g0 * _mixParams[4] + b0 * _mixParams[5];
        b = r0 * _mixParams[6] + g0 * _mixParams[7] + b0 * _mixParams[8];
    }

    private unsafe void Boost<T>(nint p) where T: struct, IColorSpace<T> {
        var rgb = (ushort*)p;

        var r = SRgb.SRgbToLinear(rgb[0] / Max16);
        var g = SRgb.SRgbToLinear(rgb[1] / Max16);
        var b = SRgb.SRgbToLinear(rgb[2] / Max16);

        GainAndMix(ref r, ref g, ref b);

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

        var r = SRgb.SRgbToLinear(rgb[0] / Max16);
        var g = SRgb.SRgbToLinear(rgb[1] / Max16);
        var b = SRgb.SRgbToLinear(rgb[2] / Max16);

        GainAndMix(ref r, ref g, ref b);

        var (l, c, h) = T.FromLinearSRgb(r, g, b);
        c *= 1 + _cGain;
        (r, g, b) = T.ToLinearSRgb(l, c, h);

        rgb[0] = (ushort)(SRgb.LinearToSRgb(r) * Max16 + .5);
        rgb[1] = (ushort)(SRgb.LinearToSRgb(g) * Max16 + .5);
        rgb[2] = (ushort)(SRgb.LinearToSRgb(b) * Max16 + .5);
    }
}
