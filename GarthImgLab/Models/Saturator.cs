namespace GarthImgLab.Models;

using System.Collections.Immutable;
using Colour;
using static Math;

sealed class Saturator(string spaceName, f64 amount, bool hard, bool mask): IEffect {
    public static readonly ImmutableArray<string> SpaceNames = [
        "HSV", "HSL", "CIELCh(ab)", "CIELCh(uv)", "JzCzhz", "OkLCh", "OkLCh+", "sUCS"
    ];

    readonly Func<f64, f64>? _adjustSat = (amount, hard) switch {
        (< 0, true) => sat => sat + amount * sat,
        (< 0, _) or (> 0, true) => sat => Pow(sat, Pow(2 * E, -amount)),
        (> 0, _) => sat => {
            var t = 1 - sat;
            var tp = Pow(t, 2.8526749696370415);
            return sat + amount * 2 * t * (1 - tp) / (2 - tp);
        },
        _ => null // 0 增益
    };

    readonly Func<Tri, Tri> _fromRgb = spaceName switch {
        "HSV" => Hsv.FromSrgb,
        "HSL" => Hsl.FromSrgb,
        "CIELCh(ab)" => static rgb => CieLChab.FromXyz(SRgb.ToXyz(rgb)),
        "CIELCh(uv)" => static rgb => CieLChuv.FromXyz(SRgb.ToXyz(rgb)),
        "JzCzhz" => static rgb => JzCzhz.FromXyz(SRgb.ToXyz(rgb)),
        "OkLCh" => static rgb => OkLCh.FromXyz(SRgb.ToXyz(rgb)),
        "OkLCh+" => static rgb => OkLChP.FromXyz(SRgb.ToXyz(rgb)),
        "sUCS" => static rgb => SUcs.FromXyz(SRgb.ToXyz(rgb)),
        _ => throw new Never()
    };

    readonly MaxCLut? _maxCLut = spaceName switch {
        "HSV" or "HSL" => null,
        "CIELCh(ab)" => CieLChab.MaxCLut.Value,
        "CIELCh(uv)" => CieLChuv.MaxCLut.Value,
        "JzCzhz" => JzCzhz.MaxCLut.Value,
        "OkLCh" => OkLCh.MaxCLut.Value,
        "OkLCh+" => OkLChP.MaxCLut.Value,
        "sUCS" => SUcs.MaxCLut.Value,
        _ => throw new Never()
    };

    readonly Func<Tri, Tri> _toRgb = spaceName switch {
        "HSV" => Hsv.ToSrgb,
        "HSL" => Hsl.ToSrgb,
        "CIELCh(ab)" => static lch => SRgb.FromXyz(CieLChab.ToXyz(lch)),
        "CIELCh(uv)" => static lch => SRgb.FromXyz(CieLChuv.ToXyz(lch)),
        "JzCzhz" => static jch => SRgb.FromXyz(JzCzhz.ToXyz(jch)),
        "OkLCh" => static lch => SRgb.FromXyz(OkLCh.ToXyz(lch)),
        "OkLCh+" => static lch => SRgb.FromXyz(OkLChP.ToXyz(lch)),
        "sUCS" => static lch => SRgb.FromXyz(SUcs.ToXyz(lch)),
        _ => throw new Never()
    };

    public void Apply(Img img, CT ct) {
        if (_adjustSat is null) return;
        img.Apply(
            rgb => {
                if (rgb is (0, 0, 0) or (1, 1, 1)) return rgb;
                var (c1, sat0, c3) = _fromRgb(rgb);

                f64 sat1;
                if (_maxCLut is null)
                    sat1 = _adjustSat(sat0);
                else {
                    var den = Max(sat0, _maxCLut[c1, c3]);
                    sat1 = _adjustSat(sat0 / den) * den;
                }

                if (mask) sat1 = sat0 + GetMask(rgb) * (sat1 - sat0);
                return _toRgb(new(c1, sat1, c3));
            },
            ct);
    }

    static f64 GetMask(Tri rgb) {
        var d = Min(rgb.Min, 1 - rgb.Max);
        return Sqrt(2 * Sqrt(d * (1 - d)));
    }
}
