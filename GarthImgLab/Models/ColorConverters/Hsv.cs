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

        double r, g, b;
        if (h < 60) {
            r = c;
            g = x;
            b = 0;
        } else if (h < 120) {
            r = x;
            g = c;
            b = 0;
        } else if (h < 180) {
            r = 0;
            g = c;
            b = x;
        } else if (h < 240) {
            r = 0;
            g = x;
            b = c;
        } else if (h < 300) {
            r = x;
            g = 0;
            b = c;
        } else {
            r = c;
            g = 0;
            b = x;
        }

        return (r + m, g + m, b + m);
    }
}
