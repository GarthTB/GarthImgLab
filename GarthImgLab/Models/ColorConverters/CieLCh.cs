namespace GarthImgLab.Models.ColorConverters;

using static Math;

public readonly struct CieLCh: IColorSpace<CieLCh> {
    private const double Xn = .9504559270516717, Yn = 1, Zn = 1.0890577507598784, D = 6d / 29;

    private static readonly double[] Cusps = CuspLut.Build<CieLCh>(100, FromSRgb(0, 0, 1).C);
    public static double GetCusp(double l, double h) => CuspLut.Sample(Cusps, 100, l, h);

    private static double F(double t) =>
        t > D * D * D
            ? Cbrt(t)
            : t / (3 * D * D) + 4d / 29;

    private static double InvF(double t) =>
        t > D
            ? t * t * t
            : 3 * D * D * (t - 4d / 29);

    public static (double L, double C, double H) FromSRgb(double r, double g, double b) {
        var (x, y, z) = Xyz.FromSRgb(r, g, b);

        var fx = F(x / Xn);
        var fy = F(y / Yn);
        var fz = F(z / Zn);

        var cieL = 116 * fy - 16;
        var cieA = 500 * (fx - fy);
        var cieB = 200 * (fy - fz);

        var c = Sqrt(cieA * cieA + cieB * cieB);
        var h = Atan2(cieB, cieA) * (180 / PI);
        if (h < 0) h += 360;

        return (cieL, c, h);
    }

    public static (double R, double G, double B) ToSRgb(double l, double c, double h) {
        var hRad = h * (PI / 180);
        var cieA = c * Cos(hRad);
        var cieB = c * Sin(hRad);

        var fy = (l + 16) / 116;
        var fx = cieA / 500 + fy;
        var fz = fy - cieB / 200;

        var x = Xn * InvF(fx);
        var y = Yn * InvF(fy);
        var z = Zn * InvF(fz);

        return Xyz.ToSRgb(x, y, z);
    }
}
