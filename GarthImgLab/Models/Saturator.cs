namespace GarthImgLab.Models;

using Colour;
using static Math;

sealed class Saturator(string space, f64 amount, bool hard, bool mask): IEffect {
    public static readonly string[] ColourSpaces = [
        "HSV", "HSL", "CIELCh(ab)", "CIELCh(uv)", "JzCzhz", "OkLCh", "OkLCh+", "sUCS"
    ];

    readonly Func<f64, f64> _gain = (amount, hard) switch {
        (< 0, true) => sat => sat * (1 + amount),
        (< 0, false) or (> 0, true) => sat => Pow(sat, Pow(2 * E, -amount)),
        (> 0, false) => sat => {
            var t = 1 - sat;
            var tp = Pow(t, 2.8);
            return sat + amount * 2 * t * (1 - tp) / (2 - tp);
        },
        _ => throw new Never()
    };

    public void Apply(Img img, CT ct) => throw new NotImplementedException();

    static f64 GetMask(in Tri rgb) {
        var min = rgb.Min;
        var max = rgb.Max;
        var m = Min(min, 1 - max);
        return 2 * Sqrt(m * (1 - m));
    }
}
