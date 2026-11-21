namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1364/OE.510196 </summary>
static class SUcs {
    static readonly Mat XyzToLms = new(.4002, .7075, -.0807, -.228, 1.15, .0612, 0, 0, .9184);

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

    static readonly Mat LmsToXyz = XyzToLms.Inverse(), IabToLms = LmsToIab.Inverse();

    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 0))).C2,
        ToXyz);

    public static Tri FromXyz(in Tri xyz) {
        var (l, m, s) = XyzToLms * xyz;
        var (i, a, b) = LmsToIab * new Tri(F(l), F(m), F(s));
        var (r, h) = Polar.AbToCh(a, b);
        var c = Log(1 + .0447 * r) / .0252;
        return new(i, c, h);
    }

    public static Tri ToXyz(in Tri ich) {
        var (i, c, h) = ich;
        var r = (Exp(.0252 * c) - 1) / .0447;
        var (a, b) = Polar.ChToAb(r, h);
        var (l, m, s) = IabToLms * new Tri(i, a, b);
        return LmsToXyz * new Tri(InvF(l), InvF(m), InvF(s));
    }

    static f64 F(f64 v) => CopySign(Pow(Abs(v), .43), v);
    static f64 InvF(f64 v) => CopySign(Pow(Abs(v), 1 / .43), v);
}
