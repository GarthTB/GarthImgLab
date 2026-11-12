namespace GarthImgLab.Colour;

using static Hexagon;
using static Math;

readonly record struct Hsv(double H, double S, double V) {
    public static Hsv FromSrgb(SRgb rgb) {
        var (max, _, c) = MaxMinC(rgb);
        if (c == 0) return new(0, 0, max);
        return new(Hue(rgb, max, c), c / max, max);
    }

    public static SRgb ToSrgb(Hsv hsv) {
        var (h, s, max) = hsv;
        var c = max * s;
        var min = max - c;
        return Reconstruct(h, c, min);
    }
}

readonly record struct Hsl(double H, double S, double L) {
    public static Hsl FromSrgb(SRgb rgb) {
        var (max, min, c) = MaxMinC(rgb);
        var l = (max + min) / 2;
        if (c == 0) return new(0, 0, l);
        return new(Hue(rgb, max, c), c / (1 - Abs(2 * l - 1)), l);
    }

    public static SRgb ToSrgb(Hsl hsl) {
        var (h, s, l) = hsl;
        var c = (1 - Abs(2 * l - 1)) * s;
        var min = l - c / 2;
        return Reconstruct(h, c, min);
    }
}

file static class Hexagon {
    public static (double Max, double Min, double C) MaxMinC(SRgb rgb) {
        var (r, g, b) = rgb;
        var max = Max(r, Max(g, b));
        var min = Min(r, Min(g, b));
        return (max, min, max - min);
    }

    // ReSharper disable CompareOfFloatsByEqualityOperator
    public static double Hue(SRgb rgb, double max, double c) {
        var (r, g, b) = rgb;
        var h = max switch {
            _ when max == r => 60 * ((g - b) / c),
            _ when max == g => 60 * ((b - r) / c + 2),
            _ => 60 * ((r - g) / c + 4)
        };
        return Polar.NormHue(h);
    }

    public static SRgb Reconstruct(double h, double c, double min) {
        h /= 60;
        var x = c * (1 - Abs(h % 2 - 1));
        var (r, g, b) = h switch {
            < 1 => (c, x, 0d),
            < 2 => (x, c, 0),
            < 3 => (0, c, x),
            < 4 => (0, x, c),
            < 5 => (x, 0, c),
            _ => (c, 0, x)
        };
        return new(r + min, g + min, b + min);
    }
}
