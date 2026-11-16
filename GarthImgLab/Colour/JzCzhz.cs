namespace GarthImgLab.Colour;

using static Math;

/// <summary> DOI: 10.1364/OE.25.015131 </summary>
readonly record struct JzCzhz(f64 J, f64 C, f64 H) {
    public static JzCzhz FromXyz(Xyz xyz) {
        var (j, a, b) = Jzazbz.FromXyz(xyz);
        var (c, h) = Polar.AbToCh(a, b);
        return new(j, c, h);
    }

    public Xyz ToXyz() {
        var (a, b) = Polar.ChToAb(C, H);
        return new Jzazbz(J, a, b).ToXyz();
    }
}

/// <summary> DOI: 10.1364/OE.25.015131 </summary>
file readonly record struct Jzazbz(f64 Jz, f64 Az, f64 Bz) {
    /// <summary> ITU-R BT.2408-9 </summary>
    const f64 L = 203;

    const f64 B = 1.15, G = .66, D = -.56, D0 = 1.6295499532821566e-11;

    const f64 C1 = 3424d / 4096,
        C2 = 2413d / 128,
        C3 = 2392d / 128,
        N = 2610d / 16384,
        P = 1.7 * 2523 / 32;

    static readonly Mat3X3 XyzToLms = new(
        .41478972,
        .579999,
        .014648,
        -.20151,
        1.120649,
        .0531008,
        -.0166008,
        .2648,
        .6684799);

    static readonly Mat3X3 LmsToIab = new(
        .5,
        .5,
        0,
        3.524,
        -4.066708,
        .542708,
        .199076,
        1.096799,
        -1.295875);

    static readonly Mat3X3 LmsToXyz = XyzToLms.Inverse(), IabToLms = LmsToIab.Inverse();

    public static Jzazbz FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        x *= L;
        y *= L;
        z *= L;

        var xp = B * x - (B - 1) * z;
        var yp = G * y - (G - 1) * x;

        var (l, m, s) = XyzToLms.Mul3X1(xp, yp, z);
        var (i, a, b) = LmsToIab.Mul3X1(Pq(l), Pq(m), Pq(s));

        var j = (1 + D) * i / (1 + D * i) - D0;
        return new(j, a, b);
    }

    public Xyz ToXyz() {
        var i = (Jz + D0) / (1 + D - D * (Jz + D0));

        var (l, m, s) = IabToLms.Mul3X1(i, Az, Bz);
        var (xp, yp, z) = LmsToXyz.Mul3X1(InvPq(l), InvPq(m), InvPq(s));

        var x = (xp + (B - 1) * z) / B;
        var y = (yp + (G - 1) * x) / G;

        return new(x / L, y / L, z / L);
    }

    static f64 Pq(f64 v) {
        var vn = Pow(v / 10000, N);
        return Pow((C1 + C2 * vn) / (1 + C3 * vn), P);
    }

    static f64 InvPq(f64 v) {
        var vp = Pow(v, 1 / P);
        return 10000 * Pow(Max(vp - C1, 0) / (C2 - C3 * vp), 1 / N);
    }
}
