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

file readonly record struct OkLab(double L, double A, double B) {
    static readonly Matrix3X3 XyzToLms = new(
        .819022437996703,
        .3619062600528904,
        -.1288737815209879,
        .0329836539323885,
        .9292868615863434,
        .0361446663506424,
        .0481771893596242,
        .2642395317527308,
        .6335478284694309);

    static readonly Matrix3X3 LmsToLab = new(
        .210454268309314,
        .7936177747023054,
        -.0040720430116193,
        1.9779985324311684,
        -2.4285922420485799,
        .450593709617411,
        .0259040424655478,
        .7827717124575296,
        -.8086757549230774);

    static readonly Matrix3X3 LmsToXyz = XyzToLms.Inverse(), LabToLms = LmsToLab.Inverse();

    public static OkLab FromXyz(Xyz xyz) {
        var (l, m, s) = XyzToLms.Multiply(xyz.X, xyz.Y, xyz.Z);
        var (okl, a, b) = LmsToLab.Multiply(Math.Cbrt(l), Math.Cbrt(m), Math.Cbrt(s));
        return new(okl, a, b);
    }

    public Xyz ToXyz() {
        var (l, m, s) = LabToLms.Multiply(L, A, B);
        var (x, y, z) = LmsToXyz.Multiply(l * l * l, m * m * m, s * s * s);
        return new(x, y, z);
    }
}
