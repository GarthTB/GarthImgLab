namespace GarthImgLab.Colour;

using static Math;

/// <summary> IEC 61966-2-1:1999 </summary>
static class SRgb {
    public static bool Contains(in Tri xyz) =>
        LinSRgb.FromXyz(xyz) is (>= 0 and <= 1, >= 0 and <= 1, >= 0 and <= 1);

    public static Tri FromXyz(in Tri xyz) {
        var (lr, lg, lb) = LinSRgb.FromXyz(xyz);
        return new(Clamp01(ToNonLin(lr)), Clamp01(ToNonLin(lg)), Clamp01(ToNonLin(lb)));
    }

    public static Tri ToXyz(in Tri rgb) {
        var (r, g, b) = rgb;
        return LinSRgb.ToXyz(new(Clamp01(ToLin(r)), Clamp01(ToLin(g)), Clamp01(ToLin(b))));
    }

    static f64 Clamp01(f64 v) => Clamp(v, 0, 1);

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
file static class LinSRgb {
    static readonly Mat LinRgbToXyz = CreateLinRgbToXyz(), XyzToLinRgb = LinRgbToXyz.Inverse();

    public static Tri FromXyz(in Tri xyz) => XyzToLinRgb * xyz;
    public static Tri ToXyz(in Tri rgb) => LinRgbToXyz * rgb;

    static Mat CreateLinRgbToXyz() {
        var (rx, ry, rz) = Xyz.FromXy(Bt709.Rx, Bt709.Ry);
        var (gx, gy, gz) = Xyz.FromXy(Bt709.Gx, Bt709.Gy);
        var (bx, by, bz) = Xyz.FromXy(Bt709.Bx, Bt709.By);
        var (sx, sy, sz) = new Mat(rx, gx, bx, ry, gy, by, rz, gz, bz).Inverse()
                         * new Tri(D65.X, D65.Y, D65.Z);
        return new(sx * rx, sy * gx, sz * bx, sx * ry, sy * gy, sz * by, sx * rz, sy * gz, sz * bz);
    }
}
