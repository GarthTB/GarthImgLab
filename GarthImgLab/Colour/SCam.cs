namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1364/OE.510196 </summary>
readonly record struct SCam(f64 Ia, f64 C, f64 H) {
    const f64 SurroundC = .52, YbOverYw = .2;
    static readonly f64 Cz = SurroundC * (1.48 + Sqrt(YbOverYw));

    public static SCam FromXyz(Xyz xyz) {
        var (i, a, b) = SUcs.FromXyz(xyz);
        var (r, h) = Polar.AbToCh(a, b);
        var c = Log(1 + .0447 * r) / .0252;
        var ia = 100 * Pow(i / 100, Cz);
        return new(ia, c, h);
    }

    public Xyz ToXyz() {
        var i = 100 * Pow(Ia / 100, 1 / Cz);
        var r = (Exp(.0252 * C) - 1) / .0447;
        var (a, b) = Polar.ChToAb(r, H);
        return new SUcs(i, a, b).ToXyz();
    }
}

/// <summary> DOI: 10.1364/OE.510196 </summary>
file readonly record struct SUcs(f64 I, f64 A, f64 B) {
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
        return new(i, a, b);
    }

    public Xyz ToXyz() {
        var (lp, mp, sp) = IabToLms.Mul3X1(I, A, B);
        var (x, y, z) = LmsToXyz.Mul3X1(InvF(lp), InvF(mp), InvF(sp));
        return new(x, y, z);
    }

    static f64 F(f64 v) => CopySign(Pow(Abs(v), .43), v);
    static f64 InvF(f64 v) => CopySign(Pow(Abs(v), 1 / .43), v);
}
