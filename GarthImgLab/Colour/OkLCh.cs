namespace GarthImgLab.Colour;

/// <summary> https://bottosson.github.io/posts/oklab/ </summary>
static class OkLCh {
    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 1))).C2,
        ToXyz);

    public static Tri FromXyz(in Tri xyz) {
        var (l, a, b) = OkLab.FromXyz(xyz);
        var (c, h) = Polar.FromCart(a, b);
        return new(l, c, h);
    }

    public static Tri ToXyz(in Tri lch) {
        var (l, c, h) = lch;
        var (a, b) = Polar.ToCart(c, h);
        return OkLab.ToXyz(new(l, a, b));
    }
}

/// <summary> https://bottosson.github.io/posts/oklab/ </summary>
file static class OkLab {
    static readonly Mat UnadaptedXyzToLms = new(
        .8189330101,
        .3618667424,
        -.1288597137,
        .0329845436,
        .9293118715,
        .0361456387,
        .0482003018,
        .2643662691,
        .633851707);

    static readonly Mat LabToLms = new(
        1,
        .3963377774,
        .2158037573,
        1,
        -.1055613458,
        -.0638541728,
        1,
        -.0894841775,
        -1.2914855480);

    static readonly Mat XyzToLms
            = UnadaptedXyzToLms.ScaleRows((UnadaptedXyzToLms * D65.Xyz).Apply(static c => 1 / c)),
        LmsToXyz = XyzToLms.Inverse(),
        LmsToLab = LabToLms.Inverse();

    public static Tri FromXyz(in Tri xyz) => LmsToLab * (XyzToLms * xyz).Apply(Math.Cbrt);
    public static Tri ToXyz(in Tri lab) => LmsToXyz * (LabToLms * lab).Apply(static c => c * c * c);
}
