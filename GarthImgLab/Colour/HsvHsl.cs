namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1145/965139.807361 </summary>
readonly record struct Hsv(f64 H, f64 S, f64 V) {
    public static Hsv FromSrgb(SRgb rgb) {
        var (min, max) = rgb.MinMax;
        var c = max - min;
        if (c == 0) return new(0, 0, max);

        var h = Hue.FromRgb(rgb, max, c);
        return new(h, c / max, max);
    }

    public SRgb ToSrgb() {
        var c = V * S;
        return Hue.ToRgb(H, c, V - c);
    }
}

/// <summary> DOI: 10.1145/800248.807362 </summary>
readonly record struct Hsl(f64 H, f64 S, f64 L) {
    public static Hsl FromSrgb(SRgb rgb) {
        var (min, max) = rgb.MinMax;
        var c = max - min;
        var l = .5 * (max + min);
        if (c == 0) return new(0, 0, l);

        var h = Hue.FromRgb(rgb, max, c);
        var s = c / (1 - Abs(2 * l - 1));
        return new(h, s, l);
    }

    public SRgb ToSrgb() {
        var c = (1 - Abs(2 * L - 1)) * S;
        return Hue.ToRgb(H, c, L - .5 * c);
    }
}

file static class Hue {
    // ReSharper disable CompareOfFloatsByEqualityOperator
    public static f64 FromRgb(SRgb rgb, f64 max, f64 c) {
        var (r, g, b) = rgb;
        var h = max switch {
            _ when max == r => 60 * ((g - b) / c),
            _ when max == g => 60 * ((b - r) / c + 2),
            _ => 60 * ((r - g) / c + 4)
        };
        return Polar.NormHue(h);
    }

    public static SRgb ToRgb(f64 h, f64 c, f64 min) {
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
