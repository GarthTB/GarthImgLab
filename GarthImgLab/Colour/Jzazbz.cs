namespace GarthImgLab.Colour;

using static Math;

readonly record struct Jzazbz(double Jz, double Az, double Bz) {
    const double B = 1.15, G = .66, D = -.56, D0 = 1.6295499532821566e-11;

    const double C1 = 3424d / 4096,
        C2 = 2413d / 128,
        C3 = 2392d / 128,
        N = 2610d / 16384,
        P = 1.7 * 2523 / 32;

    const double D1 = 4.674703637808;
    const double Al = .6479375 / D1, Bl = .271354 / D1;
    const double Am = -.6479375 / D1, Bm = -.271354 / D1;
    const double As = -.4488615 / D1, Bs = -3.795354 / D1;

    const double D2 = .382008180505132590012;
    const double Lx = .7350702396151 / D2, Mx = -.3838388831201 / D2, Sx = .0143831443472 / D2;
    const double Ly = .13382386888836 / D2, My = .277521759065028 / D2, Sy = -.024977384443776 / D2;
    const double Lz = -.0347561780808 / D2, Mz = -.1194647652552 / D2, Sz = .58170928341828 / D2;

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

    public Xyz ToXyz() {
        var i = (Jz + D0) / (1 + D - D * (Jz + D0));

        var l = i + Al * Az + Bl * Bz;
        var m = i + Am * Az + Bm * Bz;
        var s = i + As * Az + Bs * Bz;

        l = InvPq(l);
        m = InvPq(m);
        s = InvPq(s);

        var xp = Lx * l + Mx * m + Sx * s;
        var yp = Ly * l + My * m + Sy * s;
        var z = Lz * l + Mz * m + Sz * s;

        var x = (xp + (B - 1) * z) / B;
        var y = (yp + (G - 1) * x) / G;
        return new(x / 100, y / 100, z / 100);
    }

    static double Pq(double v) {
        var vn = Pow(v / 10000, N);
        return Pow((C1 + C2 * vn) / (1 + C3 * vn), P);
    }

    static double InvPq(double v) {
        var vp = Pow(v, 1 / P);
        return 10000 * Pow(Max(vp - C1, 0) / (C2 - C3 * vp), 1 / N);
    }
}
