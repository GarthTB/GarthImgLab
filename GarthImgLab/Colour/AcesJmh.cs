namespace GarthImgLab.Colour;

using static Math;
using static Xyz;

readonly record struct AcesJmh(f64 J, f64 M, f64 H) {
    public static AcesJmh FromXyz(Xyz xyz) {
        var (j, a, b) = AcesJab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(j, AcesJab.SurroundNc * c, h);
    }

    public Xyz ToXyz() {
        var (a, b) = Polar.ChToAb(M / AcesJab.SurroundNc, H);
        return new AcesJab(J, a, b).ToXyz();
    }
}

file readonly record struct AcesJab(f64 J, f64 A, f64 B) {
    public const f64 SurroundNc = .9;

    const f64 YbOverYw = .2;
    static readonly f64 Cz = SurroundNc * (1.48 + Sqrt(YbOverYw)), Aw = 61d / 20 * F(1);

    static readonly Matrix3X3 XyzToLms = new(
        .401288,
        .650173,
        -.051461,
        -.250268,
        1.204414,
        .045854,
        -.002079,
        .048952,
        .953127);

    static readonly Matrix3X3 LmsToXyz = XyzToLms.Inverse();
    static readonly (f64 L, f64 M, f64 S) D65Lms = XyzToLms.Multiply(D65.X, D65.Y, D65.Z);

    public static AcesJab FromXyz(Xyz xyz) {
        var (l, m, s) = XyzToLms.Multiply(xyz.X, xyz.Y, xyz.Z);

        var ra = F(l / D65Lms.L);
        var ga = F(m / D65Lms.M);
        var ba = F(s / D65Lms.S);

        var j = 100 * Pow((2 * ra + ga + ba / 20) / Aw, Cz);
        var a = ra - 12 * ga / 11 + ba / 11;
        var b = (ra + ga - 2 * ba) / 9;
        return new(j, a, b);
    }

    public Xyz ToXyz() {
        var aw = Aw * Pow(J / 100, 1 / Cz);
        var d = 11d / 23 * (9 * B - A);
        var ga = (aw - 2 * A - 29d / 220 * d) * 220 / 671;
        var ra = A + ga + d / 11;
        var ba = ga - d;

        var l = InvF(ra) * D65Lms.L;
        var m = InvF(ga) * D65Lms.M;
        var s = InvF(ba) * D65Lms.S;

        var (x, y, z) = LmsToXyz.Multiply(l, m, s);
        return new(x, y, z);
    }

    static f64 F(f64 v) {
        var xp = Pow(Abs(v), .42);
        return CopySign(400 * xp / (xp + 27.13), v);
    }

    static f64 InvF(f64 v) {
        var av = Abs(v);
        var xp = av * 27.13 / (400 - av);
        return CopySign(Pow(xp, 1 / .42), v);
    }
}
