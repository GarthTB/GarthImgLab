namespace GarthImgLab.Colour;

using static Math;

static class Polar {
    const f64 DegToRad = PI / 180, RadToDeg = 180 / PI;

    public static (f64 R, f64 H) FromCart(f64 x, f64 y) {
        var r = Sqrt(x * x + y * y);
        if (r == 0) return (0, 0);

        var h = NormHue(Atan2(y, x) * RadToDeg);
        return (r, h);
    }

    public static (f64 X, f64 Y) ToCart(f64 r, f64 h) {
        if (r == 0) return (0, 0);

        var (sin, cos) = SinCos(h * DegToRad);
        return (r * cos, r * sin);
    }

    // ReSharper disable CompareOfFloatsByEqualityOperator
    public static f64 NormHue(f64 h) {
        h %= 360;
        if (h < 0) h += 360; // 360 出现
        if (h == 360) return 0; // 360 -> 0
        return h;
    }
}
