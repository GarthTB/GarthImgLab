namespace GarthImgLab.Core;

using CS = Wacton.Unicolour.ColourSpace;
using Uc = Wacton.Unicolour.Unicolour;

internal static class Sat {
    private const double LchabMax = 133.80761432012983,
        LchuvMax = 179.04142708939614,
        HsluvMax = 100.05922027251566,
        HpluvMax = 1784.328864093446,
        JzczhzMax = .19027906590136512,
        OklchMax = .32249096477516476,
        OkhsvMax = 1.0119794128679827,
        OkhslMax = 1.0146240005026508,
        HctMax = 113.35620829574427;

    public static readonly Dictionary<string, Func<double, Func<RGB, RGB>>> Adjustors = new() {
        ["HSB/HSV"] = static gain => rgb => {
            var (h, s, b) = rgb.ToUc().Hsb.Tuple;
            return CS.Hsb.ToRgb(h, Gain(s, gain), b);
        },
        ["HSL"] = static gain => rgb => {
            var (h, s, l) = rgb.ToUc().Hsl.Tuple;
            return CS.Hsl.ToRgb(h, Gain(s, gain), l);
        },
        ["HSI"] = static gain => rgb => {
            var (h, s, i) = rgb.ToUc().Hsi.Tuple;
            return CS.Hsi.ToRgb(h, Gain(s, gain), i);
        },
        ["CIELChab"] = static gain => rgb => {
            var (l, c, h) = rgb.ToUc().Lchab.Tuple;
            return CS.Lchab.ToRgb(l, Gain(c / LchabMax, gain) * LchabMax, h);
        },
        ["CIELChuv"] = static gain => rgb => {
            var (l, c, h) = rgb.ToUc().Lchuv.Tuple;
            return CS.Lchuv.ToRgb(l, Gain(c / LchuvMax, gain) * LchuvMax, h);
        },
        ["HSLuv"] = static gain => rgb => {
            var (h, s, l) = rgb.ToUc().Hsluv.Tuple;
            return CS.Hsluv.ToRgb(h, Gain(s / HsluvMax, gain) * HsluvMax, l);
        },
        ["HPLuv"] = static gain => rgb => {
            var (h, s, l) = rgb.ToUc().Hpluv.Tuple;
            return CS.Hpluv.ToRgb(h, Gain(s / HpluvMax, gain) * HpluvMax, l);
        },
        ["TSL"] = static gain => rgb => {
            var (t, s, l) = rgb.ToUc().Tsl.Tuple;
            return CS.Tsl.ToRgb(t, Gain(s, gain), l);
        },
        ["JzCzhz"] = static gain => rgb => {
            var (j, c, h) = rgb.ToUc().Jzczhz.Tuple;
            return CS.Jzczhz.ToRgb(j, Gain(c / JzczhzMax, gain) * JzczhzMax, h);
        },
        ["OKLCh"] = static gain => rgb => {
            var (l, c, h) = rgb.ToUc().Oklch.Tuple;
            return CS.Oklch.ToRgb(l, Gain(c / OklchMax, gain) * OklchMax, h);
        },
        ["OKHSV"] = static gain => rgb => {
            var (h, s, v) = rgb.ToUc().Okhsv.Tuple;
            return CS.Okhsv.ToRgb(h, Gain(s / OkhsvMax, gain) * OkhsvMax, v);
        },
        ["OKHSL"] = static gain => rgb => {
            var (h, s, l) = rgb.ToUc().Okhsl.Tuple;
            return CS.Okhsl.ToRgb(h, Gain(s / OkhslMax, gain) * OkhslMax, l);
        },
        ["OKLrCh"] = static gain => rgb => {
            var (l, c, h) = rgb.ToUc().Oklrch.Tuple;
            return CS.Oklrch.ToRgb(l, Gain(c / OklchMax, gain) * OklchMax, h);
        },
        ["HCT"] = static gain => rgb => {
            var (h, c, t) = rgb.ToUc().Hct.Tuple;
            return CS.Hct.ToRgb(h, Gain(c / HctMax, gain) * HctMax, t);
        }
    };

    private static Uc ToUc(this RGB rgb) => new(CS.Rgb, rgb);

    private static double Gain(double sat, double gain) =>
        gain > 0
            ? Math.Pow(sat, 1 - gain)
            : sat * (1 + gain);

    private static RGB ToRgb(this CS cs, double f, double s, double t) =>
        new Uc(cs, f, s, t).Rgb.ConstrainedTuple;
}
