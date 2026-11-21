namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1145/965139.807361 </summary>
static class Hsv {
    public static Tri FromSrgb(in Tri rgb) {
        var max = rgb.Max;
        var c = max - rgb.Min;
        if (c == 0) return new(0, 0, max);
        return new(Hue.FromRgb(rgb, max, c), c / max, max);
    }

    public static Tri ToSrgb(in Tri hsv) {
        var (h, s, v) = hsv;
        var c = v * s;
        return Hue.ToRgb(h, c, v - c);
    }
}

/// <summary> DOI: 10.1145/800248.807362 </summary>
static class Hsl {
    public static Tri FromSrgb(in Tri rgb) {
        var min = rgb.Min;
        var max = rgb.Max;
        var c = max - min;
        var l = .5 * (max + min);
        if (c == 0) return new(0, 0, l);

        var h = Hue.FromRgb(rgb, max, c);
        var s = c / (1 - Abs(2 * l - 1));
        return new(h, s, l);
    }

    public static Tri ToSrgb(in Tri hsl) {
        var (h, s, l) = hsl;
        var c = (1 - Abs(2 * l - 1)) * s;
        return Hue.ToRgb(h, c, l - .5 * c);
    }
}

file static class Hue {
    // ReSharper disable CompareOfFloatsByEqualityOperator
    public static f64 FromRgb(in Tri rgb, f64 max, f64 c) {
        var (r, g, b) = rgb;
        var h = max switch {
            _ when max == r => 60 * ((g - b) / c),
            _ when max == g => 60 * ((b - r) / c + 2),
            _ => 60 * ((r - g) / c + 4)
        };
        return Polar.NormHue(h);
    }

    public static Tri ToRgb(f64 h, f64 c, f64 min) {
        var seg = h / 60;
        var x = c * (1 - Abs(seg % 2 - 1));
        var (r, g, b) = seg switch {
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
