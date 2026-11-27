namespace GarthImgLab.Colour;

using static CubeRoot;
using static Polar;

/// <summary> ISO/CIE 11664-4:2019 </summary>
static class CieLChab {
    public static readonly Lazy<MaxCLut> MaxCLut = new(static () => new(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(0, 0, 1))).C2,
        ToXyz));

    public static Tri FromXyz(Tri xyz) {
        var (l, a, b) = CieLab.FromXyz(xyz);
        var (c, h) = FromCart(a, b);
        return new(l, c, h);
    }

    public static Tri ToXyz(Tri lch) {
        var (l, c, h) = lch;
        var (a, b) = ToCart(c, h);
        return CieLab.ToXyz(new(l, a, b));
    }
}

/// <summary> ISO/CIE 11664-5:2024 </summary>
static class CieLChuv {
    public static readonly Lazy<MaxCLut> MaxCLut = new(static () => new(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 0))).C2,
        ToXyz));

    public static Tri FromXyz(Tri xyz) {
        var (l, u, v) = CieLuv.FromXyz(xyz);
        var (c, h) = FromCart(u, v);
        return new(l, c, h);
    }

    public static Tri ToXyz(Tri lch) {
        var (l, c, h) = lch;
        var (u, v) = ToCart(c, h);
        return CieLuv.ToXyz(new(l, u, v));
    }
}

/// <summary> ISO/CIE 11664-4:2019 </summary>
file static class CieLab {
    public static Tri FromXyz(Tri xyz) {
        var (x, y, z) = xyz;
        var fx = F(x / D65.X);
        var fy = F(y / D65.Y);
        var fz = F(z / D65.Z);

        var l = 116 * fy - 16;
        var a = 500 * (fx - fy);
        var b = 200 * (fy - fz);
        return new(l, a, b);
    }

    public static Tri ToXyz(Tri lab) {
        var (l, a, b) = lab;
        var fy = (l + 16) / 116;
        var fx = fy + a / 500;
        var fz = fy - b / 200;

        var x = D65.X * InvF(fx);
        var y = D65.Y * InvF(fy);
        var z = D65.Z * InvF(fz);
        return new(x, y, z);
    }
}

/// <summary> ISO/CIE 11664-5:2024 </summary>
file static class CieLuv {
    public static Tri FromXyz(Tri xyz) {
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

    public static Tri ToXyz(Tri luv) {
        var (l, u, v) = luv;
        if (l == 0) return new(0, 0, 0);

        var y = D65.Y * InvF((l + 16) / 116);
        var up = u / (13 * l) + D65.Up;
        var vp = v / (13 * l) + D65.Vp;

        var t = y / (4 * vp);
        var x = 9 * up * t;
        var z = (12 - 3 * up - 20 * vp) * t;
        return new(x, y, z);
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
