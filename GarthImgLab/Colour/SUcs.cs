namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1364/OE.510196 </summary>
readonly record struct SUcs(f64 I, f64 C, f64 H) {
    static readonly Mat3X3 XyzToLms = new(.4002, .7075, -.0807, -.228, 1.15, .0612, 0, 0, .9184);

    static readonly Mat3X3 LmsToIab = new(
        200 / 3.05,
        100 / 3.05,
        5 / 3.05,
        430,
        -470,
        40,
        49,
        49,
        -98);

    static readonly Mat3X3 LmsToXyz = XyzToLms.Inverse(), IabToLms = LmsToIab.Inverse();

    public static SUcs FromXyz(Xyz xyz) {
        var (l, m, s) = XyzToLms.Mul3X1(xyz.X, xyz.Y, xyz.Z);
        var (i, a, b) = LmsToIab.Mul3X1(F(l), F(m), F(s));
        var (r, h) = Polar.AbToCh(a, b);
        var c = Log(1 + .0447 * r) / .0252;
        return new(i, c, h);
    }

    public Xyz ToXyz() {
        var r = (Exp(.0252 * C) - 1) / .0447;
        var (a, b) = Polar.ChToAb(r, H);
        var (l, m, s) = IabToLms.Mul3X1(I, a, b);
        var (x, y, z) = LmsToXyz.Mul3X1(InvF(l), InvF(m), InvF(s));
        return new(x, y, z);
    }

    static f64 F(f64 v) => CopySign(Pow(Abs(v), .43), v);
    static f64 InvF(f64 v) => CopySign(Pow(Abs(v), 1 / .43), v);
}
