namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using CS = Wacton.Unicolour.ColourSpace;
using Uc = Wacton.Unicolour.Unicolour;

internal sealed partial class SatTabVM: FXTabVM {
    private const double LchabMax = 133.80761432012983,
        LchuvMax = 179.04142708939614,
        HsluvMax = 100.05922027251566,
        HpluvMax = 1784.328864093446,
        JzczhzMax = .19027906590136512,
        OklchMax = .32249096477516476,
        OkhsvMax = 1.0119794128679827,
        OkhslMax = 1.0146240005026508,
        HctMax = 113.35620829574427;

    private static readonly
        Dictionary<string, Func<double, Func<double, double, double, (double, double, double)>>>
        Adjustors = new() {
            ["HSB/HSV"] = static gain => (r, g, b) => {
                var (h, s, v) = ToUc(r, g, b).Hsb;
                return ToRgb(CS.Hsb, h, Adjust(s, gain), v);
            },
            ["HSL"] = static gain => (r, g, b) => {
                var (h, s, l) = ToUc(r, g, b).Hsl;
                return ToRgb(CS.Hsl, h, Adjust(s, gain), l);
            },
            ["HSI"] = static gain => (r, g, b) => {
                var (h, s, i) = ToUc(r, g, b).Hsi;
                return ToRgb(CS.Hsi, h, Adjust(s, gain), i);
            },
            ["CIELChab"] = static gain => (r, g, b) => {
                var (l, c, h) = ToUc(r, g, b).Lchab;
                return ToRgb(CS.Lchab, l, Adjust(c / LchabMax, gain) * LchabMax, h);
            },
            ["CIELChuv"] = static gain => (r, g, b) => {
                var (l, c, h) = ToUc(r, g, b).Lchuv;
                return ToRgb(CS.Lchuv, l, Adjust(c / LchuvMax, gain) * LchuvMax, h);
            },
            ["HSLuv"] = static gain => (r, g, b) => {
                var (h, s, l) = ToUc(r, g, b).Hsluv;
                return ToRgb(CS.Hsluv, h, Adjust(s / HsluvMax, gain) * HsluvMax, l);
            },
            ["HPLuv"] = static gain => (r, g, b) => {
                var (h, s, l) = ToUc(r, g, b).Hpluv;
                return ToRgb(CS.Hpluv, h, Adjust(s / HpluvMax, gain) * HpluvMax, l);
            },
            ["TSL"] = static gain => (r, g, b) => {
                var (t, s, l) = ToUc(r, g, b).Tsl;
                return ToRgb(CS.Tsl, t, Adjust(s, gain), l);
            },
            ["JzCzhz"] = static gain => (r, g, b) => {
                var (j, c, h) = ToUc(r, g, b).Jzczhz;
                return ToRgb(CS.Jzczhz, j, Adjust(c / JzczhzMax, gain) * JzczhzMax, h);
            },
            ["OKLCh"] = static gain => (r, g, b) => {
                var (l, c, h) = ToUc(r, g, b).Oklch;
                return ToRgb(CS.Oklch, l, Adjust(c / OklchMax, gain) * OklchMax, h);
            },
            ["OKHSV"] = static gain => (r, g, b) => {
                var (h, s, v) = ToUc(r, g, b).Okhsv;
                return ToRgb(CS.Okhsv, h, Adjust(s / OkhsvMax, gain) * OkhsvMax, v);
            },
            ["OKHSL"] = static gain => (r, g, b) => {
                var (h, s, l) = ToUc(r, g, b).Okhsl;
                return ToRgb(CS.Okhsl, h, Adjust(s / OkhslMax, gain) * OkhslMax, l);
            },
            ["OKLrCh"] = static gain => (r, g, b) => {
                var (l, c, h) = ToUc(r, g, b).Oklrch;
                return ToRgb(CS.Oklrch, l, Adjust(c / OklchMax, gain) * OklchMax, h);
            },
            ["HCT"] = static gain => (r, g, b) => {
                var (h, c, t) = ToUc(r, g, b).Hct;
                return ToRgb(CS.Hct, h, Adjust(c / HctMax, gain) * HctMax, t);
            }
        };

    public static IReadOnlyCollection<string> Strats => Adjustors.Keys;
    [ObservableProperty] public partial string SelStrat { get; set; } = "";
    [ObservableProperty] public partial bool AntiClip { get; set; } = true;
    public double Gain { get; set => SetProperty(ref field, Math.Clamp(value, -1, 1)); }

    public override void Apply(MImg img, CT ct) {
        if (Enabled && Gain != 0) img.MapRgb(Adjustors[SelStrat](Gain), AntiClip, ct);
    }

    private static Uc ToUc(double r, double g, double b) => new(CS.Rgb, r, g, b);

    private static double Adjust(double sat, double gain) =>
        gain > 0
            ? Math.Pow(sat, 1 - gain)
            : sat * (1 + gain);

    private static (double, double, double) ToRgb(CS cs, double f, double s, double t) =>
        new Uc(cs, f, s, t).Rgb.Clipped.Tuple;
}
