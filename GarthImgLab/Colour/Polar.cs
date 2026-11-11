namespace GarthImgLab.Colour;

using static Math;

static class Polar {
    public static (double C, double H) AbToCh(double a, double b) {
        var c = double.Hypot(a, b);
        if (c == 0) return (0, 0);

        var h = double.RadiansToDegrees(Atan2(b, a));
        return (c, NormHue(h));
    }

    public static (double A, double B) ChToAb(double c, double h) {
        if (c == 0) return (0, 0);

        var (sin, cos) = SinCos(double.DegreesToRadians(h));
        return (c * cos, c * sin);
    }

    public static double NormHue(double h) {
        h %= 360;
        while (h < 0) h += 360;
        return h;
    }
}
