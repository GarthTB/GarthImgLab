namespace GarthImgLab.Colour;

readonly record struct OkLCh(double L, double C, double H) {
    public static OkLCh FromXyz(Xyz xyz) {
        var (l, a, b) = OkLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(l, c, h);
    }

    public static Xyz ToXyz(OkLCh lch) {
        var (l, c, h) = lch;
        var (a, b) = Polar.ChToAb(c, h);
        return OkLab.ToXyz(new(l, a, b));
    }
}
