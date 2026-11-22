namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1364/OE.25.015131 </summary>
static class JzCzhz {
    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(0, 0, 1))).C2,
        ToXyz);

    public static Tri FromXyz(in Tri xyz) {
        var (j, a, b) = Jzazbz.FromXyz(xyz);
        var (c, h) = Polar.FromCart(a, b);
        return new(j, c, h);
    }

    public static Tri ToXyz(in Tri jch) {
        var (j, c, h) = jch;
        var (a, b) = Polar.ToCart(c, h);
        return Jzazbz.ToXyz(new(j, a, b));
    }
}

/// <summary> DOI: 10.1364/OE.25.015131 </summary>
file static class Jzazbz {
    const f64 B = 1.15, G = .66, D = -.56, D0 = 1.6295499532821566e-11;

    const f64 C1 = 3424d / 4096,
        C2 = 2413d / 128,
        C3 = 2392d / 128,
        N = 2610d / 16384,
        P = 1.7 * 2523 / 32;

    static readonly Mat UnadaptedXyzToLms = new(
        .41478972,
        .579999,
        .014648,
        -.20151,
        1.120649,
        .0531008,
        -.0166008,
        .2648,
        .6684799);

    static readonly Mat LmsToIab = new(
        .5,
        .5,
        0,
        3.524,
        -4.066708,
        .542708,
        .199076,
        1.096799,
        -1.295875);

    static readonly Mat XyzToLms = AdaptMat(),
        LmsToXyz = XyzToLms.Inverse(),
        IabToLms = LmsToIab.Inverse();

    public static Tri FromXyz(in Tri xyz) {
        var (x, y, z) = xyz;

        var xp = B * x - (B - 1) * z;
        var yp = G * y - (G - 1) * x;

        var (i, a, b) = LmsToIab * (XyzToLms * new Tri(xp, yp, z)).Apply(Pq);

        var j = (1 + D) * i / (1 + D * i) - D0;
        return new(j, a, b);
    }

    public static Tri ToXyz(in Tri jab) {
        var (j, a, b) = jab;

        var i = (j + D0) / (1 + D - D * (j + D0));

        var (xp, yp, z) = LmsToXyz * (IabToLms * new Tri(i, a, b)).Apply(InvPq);

        var x = (xp + (B - 1) * z) / B;
        var y = (yp + (G - 1) * x) / G;
        return new(x, y, z);
    }

    static f64 Pq(f64 v) {
        var vn = Pow(v / 10000, N);
        return Pow((C1 + C2 * vn) / (1 + C3 * vn), P);
    }

    static f64 InvPq(f64 v) {
        var vp = Pow(v, 1 / P);
        return 10000 * Pow(Max(vp - C1, 0) / (C2 - C3 * vp), 1 / N);
    }

    static Mat AdaptMat() {
        const f64 x = B * D65.X - (B - 1) * D65.Z;
        const f64 y = G * D65.Y - (G - 1) * D65.X;
        var lms = UnadaptedXyzToLms * new Tri(x, y, D65.Z);
        var k = Bt2408.SdrWhiteLum * (lms.C1 + lms.C2 + lms.C3) / 3;
        return UnadaptedXyzToLms.ScaleRows(lms.Apply(c => k / c));
    }
}
