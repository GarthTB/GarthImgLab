namespace GarthImgLab.Colour;

using static D65;

/// <summary> ISO/CIE 11664-5:2024 </summary>
readonly record struct CieLChuv(f64 L, f64 C, f64 H) {
    public static CieLChuv FromXyz(Xyz xyz) {
        var (l, u, v) = CieLuv.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(u, v);
        return new(l, c, h);
    }

    public Xyz ToXyz() {
        var (u, v) = Polar.ChToAb(C, H);
        return new CieLuv(L, u, v).ToXyz();
    }
}

/// <summary> ISO/CIE 11664-5:2024 </summary>
file readonly record struct CieLuv(f64 L, f64 U, f64 V) {
    const f64 Epsilon = 216d / 24389, Kappa = 24389d / 27;
    static readonly f64 Un = 4 * W.X / Den(W.X, W.Y, W.Z);
    static readonly f64 Vn = 9 * W.Y / Den(W.X, W.Y, W.Z);

    public static CieLuv FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        if (y == 0) return new(0, 0, 0);

        var den = Den(x, y, z);
        var up = 4 * x / den;
        var vp = 9 * y / den;

        var l = 116 * F(y / W.Y) - 16;
        var u = 13 * l * (up - Un);
        var v = 13 * l * (vp - Vn);
        return new(l, u, v);
    }

    public Xyz ToXyz() {
        if (L == 0) return new(0, 0, 0);

        var up = U / (13 * L) + Un;
        var vp = V / (13 * L) + Vn;

        var y = W.Y * InvF((L + 16) / 116);
        var x = 9 * y * up / (4 * vp);
        var z = y * (12 - 3 * up - 20 * vp) / (4 * vp);
        return new(x, y, z);
    }

    static f64 Den(f64 x, f64 y, f64 z) => x + 15 * y + 3 * z;

    static f64 F(f64 t) =>
        t > Epsilon
            ? Math.Cbrt(t)
            : (Kappa * t + 16) / 116;

    static f64 InvF(f64 t) {
        var t3 = t * t * t;
        return t3 > Epsilon
            ? t3
            : (116 * t - 16) / Kappa;
    }
}
