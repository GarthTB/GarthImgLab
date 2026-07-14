// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GarthImgLab.Models.ColorConverters;

using static Math;

public readonly struct Hsl: IColorSpace<Hsl> {
    public static double GetCusp(double l, double h) => 1;

    public static (double L, double C, double H) FromLinearSRgb(double r, double g, double b) {
        r = SRgb.LinearToSRgb(r);
        g = SRgb.LinearToSRgb(g);
        b = SRgb.LinearToSRgb(b);

        var max = Max(r, Max(g, b));
        var min = Min(r, Min(g, b));
        var c = max - min;
        var l = .5 * (max + min);

        var h = 0d;
        if (c != 0) {
            if (max == r)
                h = 60 * ((g - b) / c % 6);
            else if (max == g)
                h = 60 * ((b - r) / c + 2);
            else
                h = 60 * ((r - g) / c + 4);
            if (h < 0) h += 360;
        }

        if (c > 0) c /= 1 - Abs(2 * l - 1);
        return (l, c, h);
    }

    public static (double R, double G, double B) ToLinearSRgb(double l, double s, double h) {
        h %= 360;
        if (h < 0) h += 360;
        var c = (1 - Abs(2 * l - 1)) * s;

        var x = c * (1 - Abs(h / 60 % 2 - 1));
        var m = l - .5 * c;

        var (r, g, b) = h switch {
            < 60 => (c, x, 0d),
            < 120 => (x, c, 0),
            < 180 => (0, c, x),
            < 240 => (0, x, c),
            < 300 => (x, 0, c),
            _ => (c, 0, x)
        };

        r = SRgb.SRgbToLinear(r + m);
        g = SRgb.SRgbToLinear(g + m);
        b = SRgb.SRgbToLinear(b + m);

        return (r, g, b);
    }
}
