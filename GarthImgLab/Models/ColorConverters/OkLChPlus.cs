namespace GarthImgLab.Models.ColorConverters;

using static Math;

public readonly struct OkLChPlus: IColorSpace<OkLChPlus> {
    private const double A = .73, N = .87, S = .34;

    private static readonly double[] Cusps = CuspLut.Build<OkLChPlus>(1, FromLinearSRgb(1, 0, 1).C);
    public static double GetCusp(double l, double h) => CuspLut.Sample(Cusps, 1, l, h);

    public static (double L, double C, double H) FromLinearSRgb(double r, double g, double b) {
        var (l, c, h) = OkLCh.FromLinearSRgb(r, g, b);
        l = Pow(l, A);
        var c1 = Pow(c, N);
        c = c1 / (c1 + Pow(S, N));
        return (l, c, h);
    }

    public static (double R, double G, double B) ToLinearSRgb(double l, double c, double h) {
        l = Pow(l, 1 / A);
        c = Pow(c * Pow(S, N) / (1 - c), 1 / N);
        return OkLCh.ToLinearSRgb(l, c, h);
    }
}
