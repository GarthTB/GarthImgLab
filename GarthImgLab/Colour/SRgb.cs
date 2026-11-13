namespace GarthImgLab.Colour;

using static Math;

readonly record struct SRgb(double R, double G, double B) {
    const double Xr = 12831d / 3959, Yr = -329d / 214, Zr = -1974d / 3959;
    const double Xg = -851781d / 878810, Yg = 1648619d / 878810, Zg = 36519d / 878810;
    const double Xb = 705d / 12673, Yb = -2585d / 12673, Zb = 705d / 667;

    const double Rx = 506752d / 1228815, Gx = 87881d / 245763, Bx = 12673d / 70218;
    const double Ry = 87098d / 409605, Gy = 175762d / 245763, By = 12673d / 175545;
    const double Rz = 7918d / 409605, Gz = 87881d / 737289, Bz = 1001167d / 1053270;

    public static SRgb FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        var lr = Xr * x + Yr * y + Zr * z;
        var lg = Xg * x + Yg * y + Zg * z;
        var lb = Xb * x + Yb * y + Zb * z;
        return new(ToNonLinear(lr), ToNonLinear(lg), ToNonLinear(lb));
    }

    public Xyz ToXyz() {
        double lr = ToLinear(R), lg = ToLinear(G), lb = ToLinear(B);
        return new(
            Rx * lr + Gx * lg + Bx * lb,
            Ry * lr + Gy * lg + By * lb,
            Rz * lr + Gz * lg + Bz * lb);
    }

    static double ToNonLinear(double v) =>
        v <= .0031308
            ? v * 12.92
            : 1.055 * Pow(v, 1 / 2.4) - .055;

    static double ToLinear(double v) =>
        v <= .04045
            ? v / 12.92
            : Pow((v + .055) / 1.055, 2.4);
}
