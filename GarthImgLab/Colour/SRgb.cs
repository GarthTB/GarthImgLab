namespace GarthImgLab.Colour;

using static Math;

readonly record struct SRgb(double R, double G, double B) {
    static readonly Matrix3X3 XyzToLinRgb = new(
        12831d / 3959,
        -329d / 214,
        -1974d / 3959,
        -851781d / 878810,
        1648619d / 878810,
        36519d / 878810,
        705d / 12673,
        -2585d / 12673,
        705d / 667);

    static readonly Matrix3X3 LinRgbToXyz = new(
        506752d / 1228815,
        87881d / 245763,
        12673d / 70218,
        87098d / 409605,
        175762d / 245763,
        12673d / 175545,
        7918d / 409605,
        87881d / 737289,
        1001167d / 1053270);

    public static SRgb FromXyz(Xyz xyz) {
        var (lr, lg, lb) = XyzToLinRgb.Multiply(xyz.X, xyz.Y, xyz.Z);
        return new(ToNonLin(lr), ToNonLin(lg), ToNonLin(lb));
    }

    public Xyz ToXyz() {
        var (x, y, z) = LinRgbToXyz.Multiply(ToLin(R), ToLin(G), ToLin(B));
        return new(x, y, z);
    }

    static double ToNonLin(double v) =>
        v <= .0031308
            ? v * 12.92
            : 1.055 * Pow(v, 1 / 2.4) - .055;

    static double ToLin(double v) =>
        v <= .04045
            ? v / 12.92
            : Pow((v + .055) / 1.055, 2.4);
}
