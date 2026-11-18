namespace GarthImgLab.Colour;

using static Math;

/// <summary> IEC 61966-2-1:1999 </summary>
readonly record struct SRgb(f64 R, f64 G, f64 B) {
    public (f64 Min, f64 Max) MinMax => (Min(R, Min(G, B)), Max(R, Max(G, B)));

    public static bool IsValid(Xyz xyz) =>
        LinSRgb.FromXyz(xyz) is (>= 0 and <= 1, >= 0 and <= 1, >= 0 and <= 1);

    public static SRgb FromXyz(Xyz xyz) {
        var (lr, lg, lb) = LinSRgb.FromXyz(xyz);
        return new(Clamp(ToNonLin(lr), 0, 1), Clamp(ToNonLin(lg), 0, 1), Clamp(ToNonLin(lb), 0, 1));
    }

    public Xyz ToXyz() =>
        new LinSRgb(Clamp(ToLin(R), 0, 1), Clamp(ToLin(G), 0, 1), Clamp(ToLin(B), 0, 1)).ToXyz();

    static f64 ToNonLin(f64 v) =>
        v <= .0031308
            ? v * 12.92
            : 1.055 * Pow(v, 1 / 2.4) - .055;

    static f64 ToLin(f64 v) =>
        v <= .04045
            ? v / 12.92
            : Pow((v + .055) / 1.055, 2.4);
}

/// <summary> IEC 61966-2-1:1999 </summary>
file readonly record struct LinSRgb(f64 R, f64 G, f64 B) {
    static readonly Mat3X3 LinRgbToXyz = CreateLinRgbToXyz(), XyzToLinRgb = LinRgbToXyz.Inverse();

    static Mat3X3 CreateLinRgbToXyz() {
        var (rx, ry, rz) = Bt709.R;
        var (gx, gy, gz) = Bt709.G;
        var (bx, by, bz) = Bt709.B;
        Mat3X3 p = new(rx, gx, bx, ry, gy, by, rz, gz, bz);
        var (sx, sy, sz) = p.Inverse().Mul3X1(D65.X, D65.Y, D65.Z);
        return p.ScaleCols(sx, sy, sz);
    }

    public static LinSRgb FromXyz(Xyz xyz) {
        var (r, g, b) = XyzToLinRgb.Mul3X1(xyz.X, xyz.Y, xyz.Z);
        return new(r, g, b);
    }

    public Xyz ToXyz() {
        var (x, y, z) = LinRgbToXyz.Mul3X1(R, G, B);
        return new(x, y, z);
    }
}
