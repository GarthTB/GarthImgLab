namespace GarthImgLab.Colour;

using static Math;

static class Polar {
    public static (f64 C, f64 H) AbToCh(f64 a, f64 b) {
        var c = f64.Hypot(a, b);
        if (c == 0) return (0, 0);

        var h = f64.RadiansToDegrees(Atan2(b, a));
        return (c, NormHue(h));
    }

    public static (f64 A, f64 B) ChToAb(f64 c, f64 h) {
        if (c == 0) return (0, 0);

        var (sin, cos) = SinCos(f64.DegreesToRadians(h));
        return (c * cos, c * sin);
    }

    public static f64 NormHue(f64 h) {
        h %= 360;
        if (h < 0) h += 360;
        return h;
    }
}
