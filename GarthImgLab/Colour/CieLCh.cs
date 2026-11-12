namespace GarthImgLab.Colour;

readonly record struct CieLCh(double L, double C, double H) {
    public static CieLCh FromXyz(Xyz xyz) {
        var (l, a, b) = CieLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(l, c, h);
    }

    public static Xyz ToXyz(CieLCh lch) {
        var (l, c, h) = lch;
        var (a, b) = Polar.ChToAb(c, h);
        return CieLab.ToXyz(new(l, a, b));
    }
}

file readonly record struct CieLab(double L, double A, double B) {
    const double Epsilon = 216d / 24389, Kappa = 24389d / 27;

    public static CieLab FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        var (wx, wy, wz) = Xyz.D65;

        var fx = F(x / wx);
        var fy = F(y / wy);
        var fz = F(z / wz);

        var l = 116 * fy - 16;
        var a = 500 * (fx - fy);
        var b = 200 * (fy - fz);
        return new(l, a, b);
    }

    public static Xyz ToXyz(CieLab lab) {
        var (l, a, b) = lab;
        var (wx, wy, wz) = Xyz.D65;

        var fy = (l + 16) / 116;
        var fx = fy + a / 500;
        var fz = fy - b / 200;

        var x = wx * InvF(fx);
        var y = wy * InvF(fy);
        var z = wz * InvF(fz);
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
