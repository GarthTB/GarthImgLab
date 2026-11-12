namespace GarthImgLab.Colour;

using static Math;

readonly record struct Jzazbz(double Jz, double Az, double Bz) {
    const double B = 1.15, G = .66, D = -.56, D0 = 1.6295499532821566e-11;

    const double C1 = 3424d / 4096,
        C2 = 2413d / 128,
        C3 = 2392d / 128,
        N = 2610d / 16384,
        P = 1.7 * 2523 / 32;

    public static Jzazbz FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        x *= 100;
        y *= 100;
        z *= 100;

        var xp = B * x - (B - 1) * z;
        var yp = G * y - (G - 1) * x;

        var l = .41478972 * xp + 0.579999 * yp + .0146480 * z;
        var m = -.2015100 * xp + 1.120649 * yp + .0531008 * z;
        var s = -.0166008 * xp + 0.264800 * yp + .6684799 * z;

        l = Pq(l);
        m = Pq(m);
        s = Pq(s);

        var i = .5 * l + .5 * m;
        var a = 3.52400 * l - 4.066708 * m + 0.542708 * s;
        var b = .199076 * l + 1.096799 * m - 1.295875 * s;

        var j = (1 + D) * i / (1 + D * i) - D0;
        return new(j, a, b);
    }

    public static Xyz ToXyz(Jzazbz jab) {
        var (j, a, b) = jab;
        var i = (j + D0) / (1 + D - D * (j + D0));

        var l = i + .13860504327153930 * a + .05804731615611888 * b;
        var m = i - .13860504327153930 * a - .05804731615611888 * b;
        var s = i - .09601924202631895 * a - .81189189605603897 * b;

        l = InvPq(l);
        m = InvPq(m);
        s = InvPq(s);

        var xp = 1.9242264357876080 * l - 1.0047923125953662 * m + .03765140403061803 * s;
        var yp = .35031676209499931 * l + .72648119393165574 * m - .06538442294808507 * s;
        var z = -.09098281098284764 * l - .31272829052307421 * m + 1.5227665613052615 * s;

        var x = (xp + (B - 1) * z) / B;
        var y = (yp - (G - 1) * x) / G;
        return new(x / 100, y / 100, z / 100);
    }

    static double Pq(double v) {
        var vn = Pow(v / 10000, N);
        return Pow((C1 + C2 * vn) / (1 + C3 * vn), P);
    }

    static double InvPq(double v) {
        var vp = Pow(v, 1 / P);
        var num = Max(vp - C1, 0);
        var den = C2 - C3 * vp;
        return 10000 * Pow(num / den, 1 / N);
    }
}
