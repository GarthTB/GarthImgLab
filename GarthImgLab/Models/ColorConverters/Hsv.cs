// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GarthImgLab.Models.ColorConverters;

using static Math;

public readonly struct Hsv: IColorSpace<Hsv> {
    public static double GetCusp(double l, double h) => 1;

    public static (double L, double C, double H) FromSRgb(double r, double g, double b) {
        var max = Max(r, Max(g, b));
        var min = Min(r, Min(g, b));
        var c = max - min;

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

        var s = max == 0
            ? 0
            : c / max;
        return (max, s, h);
    }

    public static (double R, double G, double B) ToSRgb(double v, double s, double h) {
        h %= 360;
        if (h < 0) h += 360;
        var c = v * s;
        var x = c * (1 - Abs(h / 60 % 2 - 1));
        var m = v - c;

        var (r, g, b) = h switch {
            < 60 => (c, x, 0d),
            < 120 => (x, c, 0),
            < 180 => (0, c, x),
            < 240 => (0, x, c),
            < 300 => (x, 0, c),
            _ => (c, 0, x)
        };

        return (r + m, g + m, b + m);
    }
}
