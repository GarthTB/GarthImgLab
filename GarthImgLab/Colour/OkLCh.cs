namespace GarthImgLab.Colour;

/// <summary> https://bottosson.github.io/posts/oklab/ </summary>
static class OkLCh {
    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 1))).C2,
        ToXyz);

    public static Tri FromXyz(in Tri xyz) {
        var (l, a, b) = OkLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(l, c, h);
    }

    public static Tri ToXyz(in Tri lch) {
        var (l, c, h) = lch;
        var (a, b) = Polar.ChToAb(c, h);
        return OkLab.ToXyz(new(l, a, b));
    }
}

/// <summary> https://bottosson.github.io/posts/oklab/ </summary>
file static class OkLab {
    /// <summary> https://www.w3.org/TR/css-color-4/ </summary>
    static readonly Mat XyzToLms = new(
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
    static readonly Mat LmsToLab = new(
        .210454268309314,
        .7936177747023054,
        -.0040720430116193,
        1.9779985324311684,
        -2.4285922420485799,
        .450593709617411,
        .0259040424655478,
        .7827717124575296,
        -.8086757549230774);

    static readonly Mat LmsToXyz = XyzToLms.Inverse(), LabToLms = LmsToLab.Inverse();

    public static Tri FromXyz(in Tri xyz) {
        var (l, m, s) = XyzToLms * xyz;
        return LmsToLab * new Tri(Math.Cbrt(l), Math.Cbrt(m), Math.Cbrt(s));
    }

    public static Tri ToXyz(in Tri lab) {
        var (l, m, s) = LabToLms * lab;
        return LmsToXyz * new Tri(l * l * l, m * m * m, s * s * s);
    }
}
