namespace GarthImgLab.Colour;

using static Math;

readonly record struct OkLChPlus(double L, double C, double H) {
    const double Alpha = .73, N = .87, Sigma = .34;
    static readonly double SigmaN = Pow(Sigma, N);

    public static OkLChPlus FromXyz(Xyz xyz) {
        var (l, a, b) = OkLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);

        var lp = Pow(l, Alpha);
        var cn = Pow(c, N);
        var cp = cn / (cn + SigmaN);

        return new(lp, cp, h);
    }

    public Xyz ToXyz() {
        var l = Pow(L, 1 / Alpha);
        var c = Pow(SigmaN * C / (1 - C), 1 / N);

        var (a, b) = Polar.ChToAb(c, H);
        return new OkLab(l, a, b).ToXyz();
    }
}
