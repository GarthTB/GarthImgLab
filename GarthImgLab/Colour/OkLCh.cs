namespace GarthImgLab.Colour;

/// <summary> https://bottosson.github.io/posts/oklab/ </summary>
readonly record struct OkLCh(f64 L, f64 C, f64 H) {
    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(new SRgb(1, 1, 1).ToXyz()).L,
        FromXyz(new SRgb(1, 0, 1).ToXyz()).C,
        static (l, c, h) => new OkLCh(l, c, h).ToXyz());

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

/// <summary> https://bottosson.github.io/posts/oklab/ </summary>
file readonly record struct OkLab(f64 L, f64 A, f64 B) {
    /// <summary> https://www.w3.org/TR/css-color-4/ </summary>
    static readonly Mat3X3 XyzToLms = new(
        .819022437996703,
        .3619062600528904,
        -.1288737815209879,
        .0329836539323885,
        .9292868615863434,
        .0361446663506424,
        .0481771893596242,
        .2642395317527308,
        .6335478284694309);

    /// <summary> https://www.w3.org/TR/css-color-4/ </summary>
    static readonly Mat3X3 LmsToLab = new(
        .210454268309314,
        .7936177747023054,
        -.0040720430116193,
        1.9779985324311684,
        -2.4285922420485799,
        .450593709617411,
        .0259040424655478,
        .7827717124575296,
        -.8086757549230774);

    static readonly Mat3X3 LmsToXyz = XyzToLms.Inverse(), LabToLms = LmsToLab.Inverse();

    public static OkLab FromXyz(Xyz xyz) {
        var (l, m, s) = XyzToLms.Mul3X1(xyz.X, xyz.Y, xyz.Z);
        var (okl, a, b) = LmsToLab.Mul3X1(Math.Cbrt(l), Math.Cbrt(m), Math.Cbrt(s));
        return new(okl, a, b);
    }

    public Xyz ToXyz() {
        var (l, m, s) = LabToLms.Mul3X1(L, A, B);
        var (x, y, z) = LmsToXyz.Mul3X1(l * l * l, m * m * m, s * s * s);
        return new(x, y, z);
    }
}
