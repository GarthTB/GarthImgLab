namespace GarthImgLab.Models.ColorConverters;

using static Math;

public static class SRgb {
    private const double A = 1.055, S = 12.92, Th = 0.04045;

    public static double SRgbToLinear(double v) {
        var abs = Abs(v);
        var linear = abs <= Th
            ? abs / S
            : Pow((abs + A - 1) / A, 2.4);
        return CopySign(linear, v);
    }

    public static double LinearToSRgb(double v) {
        var abs = Abs(v);
        var gamma = abs >= Th / S
            ? A * Pow(abs, 1 / 2.4) - (A - 1)
            : S * abs;
        return CopySign(gamma, v);
    }
}
