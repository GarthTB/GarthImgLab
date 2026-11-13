namespace GarthImgLab.Colour;

readonly record struct OkLrCh(double L, double C, double H) {
    const double K1 = .206, K2 = .03, K3 = (1 + K1) / (1 + K2);

    public static OkLrCh FromXyz(Xyz xyz) {
        var (l, c, h) = OkLCh.FromXyz(xyz);

        var d = K3 * l - K1;
        var lr = .5 * (d + Math.Sqrt(d * d + 4 * K2 * K3 * l));

        return new(lr, c, h);
    }

    public Xyz ToXyz() {
        var l = L * (L + K1) / K3 / (L + K2);
        return new OkLCh(l, C, H).ToXyz();
    }
}
