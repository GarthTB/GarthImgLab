namespace GarthImgLab.Colour;

readonly record struct OkLCh(double L, double C, double H) {
    public static OkLCh FromXyz(Xyz xyz) {
        var (l, a, b) = OkLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(l, c, h);
    }

    public Xyz ToXyz() {
        var (a, b) = Polar.ChToAb(C, H);
        return new OkLab(L, a, b).ToXyz();
    }
}
