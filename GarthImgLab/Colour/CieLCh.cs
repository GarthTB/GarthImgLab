namespace GarthImgLab.Colour;

using static Xyz;

readonly record struct CieLCh(double L, double C, double H) {
    public static CieLCh FromXyz(Xyz xyz) {
        var (l, a, b) = CieLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(l, c, h);
    }

    public Xyz ToXyz() {
        var (a, b) = Polar.ChToAb(C, H);
        return new CieLab(L, a, b).ToXyz();
    }
}

file readonly record struct CieLab(double L, double A, double B) {
    const double Epsilon = 216d / 24389, Kappa = 24389d / 27;

    public static CieLab FromXyz(Xyz xyz) {
        var fx = F(xyz.X / D65.X);
        var fy = F(xyz.Y / D65.Y);
        var fz = F(xyz.Z / D65.Z);

        var l = 116 * fy - 16;
        var a = 500 * (fx - fy);
        var b = 200 * (fy - fz);
        return new(l, a, b);
    }

    public Xyz ToXyz() {
        var fy = (L + 16) / 116;
        var fx = fy + A / 500;
        var fz = fy - B / 200;

        var x = D65.X * InvF(fx);
        var y = D65.Y * InvF(fy);
        var z = D65.Z * InvF(fz);
        return new(x, y, z);
    }

    static double F(double t) =>
        t > Epsilon
            ? Math.Cbrt(t)
            : (Kappa * t + 16) / 116;

    static double InvF(double t) {
        var t3 = t * t * t;
        return t3 > Epsilon
            ? t3
            : (116 * t - 16) / Kappa;
    }
}
