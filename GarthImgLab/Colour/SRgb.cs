namespace GarthImgLab.Colour;

using static Math;

/// <summary> IEC 61966-2-1:1999 </summary>
static class SRgb {
    public static bool Contains(in Tri xyz) =>
        LinSRgb.FromXyz(xyz) is (>= 0 and <= 1, >= 0 and <= 1, >= 0 and <= 1);

    public static Tri FromXyz(in Tri xyz) => LinSRgb.FromXyz(xyz).Apply(EncodeClamp);
    public static Tri ToXyz(in Tri rgb) => LinSRgb.ToXyz(rgb.Apply(ClampDecode));

    static f64 EncodeClamp(f64 v) {
        var nl = v <= .0031308
            ? v * 12.92
            : 1.055 * Pow(v, 1 / 2.4) - .055;
        return Clamp(nl, 0, 1);
    }

    static f64 ClampDecode(f64 v) {
        var l = Clamp(v, 0, 1);
        return l <= .04045
            ? l / 12.92
            : Pow((l + .055) / 1.055, 2.4);
    }
}

/// <summary> IEC 61966-2-1:1999 </summary>
file static class LinSRgb {
    static readonly Mat LinRgbToXyz = CreateMat(), XyzToLinRgb = LinRgbToXyz.Inverse();

    public static Tri FromXyz(in Tri xyz) => XyzToLinRgb * xyz;
    public static Tri ToXyz(in Tri rgb) => LinRgbToXyz * rgb;

    static Mat CreateMat() {
        var (rx, ry, rz) = Bt709.RXyz;
        var (gx, gy, gz) = Bt709.GXyz;
        var (bx, by, bz) = Bt709.BXyz;
        Mat p = new(rx, gx, bx, ry, gy, by, rz, gz, bz);
        return p.ScaleCols(p.Inverse() * D65.Xyz);
    }
}
