namespace GarthImgLab.Colour;

using static CubeRoot;

/// <summary> ISO/CIE 11664-4:2019 </summary>
readonly record struct CieLChab(f64 L, f64 C, f64 H) {
    public static CieLChab FromXyz(Xyz xyz) {
        var (l, a, b) = CieLab.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(l, c, h);
    }

    public Xyz ToXyz() {
        var (a, b) = Polar.ChToAb(C, H);
        return new CieLab(L, a, b).ToXyz();
    }
}

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

/// <summary> ISO/CIE 11664-4:2019 </summary>
file readonly record struct CieLab(f64 L, f64 A, f64 B) {
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
}

/// <summary> ISO/CIE 11664-5:2024 </summary>
file readonly record struct CieLuv(f64 L, f64 U, f64 V) {
    public static CieLuv FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        if (y == 0) return new(0, 0, 0);

        var den = x + 15 * y + 3 * z;
        var up = 4 * x / den;
        var vp = 9 * y / den;

        var l = 116 * F(y / D65.Y) - 16;
        var u = 13 * l * (up - D65.Up);
        var v = 13 * l * (vp - D65.Vp);
        return new(l, u, v);
    }

    public Xyz ToXyz() {
        if (L == 0) return new(0, 0, 0);

        var y = D65.Y * InvF((L + 16) / 116);
        var up = U / (13 * L) + D65.Up;
        var vp = V / (13 * L) + D65.Vp;

        var den = 6 * up - 16 * vp + 12;
        var xp = 9 * up / den;
        var yp = 4 * vp / den;

        return new(xp * y / yp, y, (1 - xp - yp) * y / yp);
    }
}

/// <summary> CIE 015:2018 </summary>
file static class CubeRoot {
    const f64 Eps = 216d / 24389,
        Delta = 6d / 29,
        Slope = 841d / 108,
        Bias = 4d / 29,
        InvSlope = 108d / 841;

    public static f64 F(f64 t) {
        if (t > Eps) return Math.Cbrt(t);
        return Slope * t + Bias;
    }

    public static f64 InvF(f64 f) {
        if (f > Delta) return f * f * f;
        return (f - Bias) * InvSlope;
    }
}
