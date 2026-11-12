namespace GarthImgLab.Colour;

readonly record struct JzCzhz(double J, double C, double H) {
    public static JzCzhz FromXyz(Xyz xyz) {
        var (j, a, b) = Jzazbz.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(j, c, h);
    }

    public static Xyz ToXyz(JzCzhz jch) {
        var (j, c, h) = jch;
        var (a, b) = Polar.ChToAb(c, h);
        return Jzazbz.ToXyz(new(j, a, b));
    }
}
