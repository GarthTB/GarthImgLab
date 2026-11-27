namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1364/OE.510196 </summary>
static class SUcs {
    static readonly Mat UnadaptedXyzToLms = new(
        .4002,
        .7075,
        -.0807,
        -.228,
        1.15,
        .0612,
        0,
        0,
        .9184);

    static readonly Mat LmsToIab = new(
        200 / 3.05,
        100 / 3.05,
        5 / 3.05,
        430,
        -470,
        40,
        49,
        49,
        -98);

    static readonly Mat XyzToLms
            = UnadaptedXyzToLms.ScaleRows((UnadaptedXyzToLms * D65.Xyz).Apply(static c => 1 / c)),
        LmsToXyz = XyzToLms.Inverse(),
        IabToLms = LmsToIab.Inverse();

    public static readonly Lazy<MaxCLut> MaxCLut = new(static () => new(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 0))).C2,
        ToXyz));

    public static Tri FromXyz(Tri xyz) {
        var (i, a, b) = LmsToIab * (XyzToLms * xyz).Apply(F);
        var (r, h) = Polar.FromCart(a, b);
        var c = Log(1 + .0447 * r) / .0252;
        return new(i, c, h);
    }

    public static Tri ToXyz(Tri ich) {
        var (i, c, h) = ich;
        var r = (Exp(.0252 * c) - 1) / .0447;
        var (a, b) = Polar.ToCart(r, h);
        return LmsToXyz * (IabToLms * new Tri(i, a, b)).Apply(InvF);
    }

    static f64 F(f64 v) => CopySign(Pow(Abs(v), .43), v);
    static f64 InvF(f64 v) => CopySign(Pow(Abs(v), 1 / .43), v);
}
