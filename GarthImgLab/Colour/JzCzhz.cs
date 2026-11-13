namespace GarthImgLab.Colour;

readonly record struct JzCzhz(double J, double C, double H) {
    public static JzCzhz FromXyz(Xyz xyz) {
        var (j, a, b) = Jzazbz.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(j, c, h);
    }

    public Xyz ToXyz() {
        var (a, b) = Polar.ChToAb(C, H);
        return new Jzazbz(J, a, b).ToXyz();
    }
}
