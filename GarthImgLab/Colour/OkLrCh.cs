namespace GarthImgLab.Colour;

readonly record struct OkLrCh(double L, double C, double H) {
    const double K1 = .206, K2 = .03, K3 = (1 + K1) / (1 + K2);

    public static OkLrCh FromXyz(Xyz xyz) {
        var (l, a, b) = OkLab.FromXyz(xyz);

        var d = K3 * l - K1;
        var lr = .5 * (d + Math.Sqrt(d * d + 4 * K2 * K3 * l));

        var (c, h) = Polar.AbToCh(a, b);
        return new(lr, c, h);
    }

    public static Xyz ToXyz(OkLrCh lch) {
        var (lr, c, h) = lch;

        var l = lr * (lr + K1) / K3 / (lr + K2);

        var (a, b) = Polar.ChToAb(c, h);
        return OkLab.ToXyz(new(l, a, b));
    }
}
