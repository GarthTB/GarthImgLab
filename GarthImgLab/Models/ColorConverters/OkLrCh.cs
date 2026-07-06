namespace GarthImgLab.Models.ColorConverters;

public readonly struct OkLrCh: IColorSpace<OkLrCh> {
    public static double MaxSat => .32249098837223966; // (1,0,1)
    private const double K1 = .206, K2 = .03, K3 = (1 + K1) / (1 + K2);

    public static (double L, double C, double H) FromSRgb(double r, double g, double b) {
        var (l, c, h) = OkLCh.FromSRgb(r, g, b);
        var term = K3 * l - K1;
        var root = Math.Sqrt(term * term + 4 * K2 * K3 * l);
        var toe = term >= 0
            ? .5 * (term + root)
            : 2 * K2 * K3 * l / (root - term);
        return (toe, c, h);
    }

    public static (double R, double G, double B) ToSRgb(double l, double c, double h) =>
        OkLCh.ToSRgb((l * l + K1 * l) / (K3 * (l + K2)), c, h);
}
