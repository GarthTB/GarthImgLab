namespace GarthImgLab.Colour;

/// <summary> IEC 61966-2-1:1999 </summary>
static class SRgb {
    public static bool Contains(Tri xyz) =>
        LinSRgb.FromXyz(xyz) is (>= 0 and <= 1, >= 0 and <= 1, >= 0 and <= 1);

    public static Tri FromXyz(Tri xyz) => LinSRgb.FromXyz(xyz).Apply(EncodeClamp);
    public static Tri ToXyz(Tri rgb) => LinSRgb.ToXyz(rgb.Apply(Decode));

    static f64 EncodeClamp(f64 v) {
        var nl = v <= .003130668442500634
            ? v * 12.92
            : 1.055 * Math.Pow(v, 1 / 2.4) - .055;
        return Math.Clamp(nl, 0, 1);
    }

    static f64 Decode(f64 v) =>
        v <= .040448236277108192
            ? v / 12.92
            : Math.Pow((v + .055) / 1.055, 2.4);
}

/// <summary> IEC 61966-2-1:1999 </summary>
file static class LinSRgb {
    static readonly Mat LinRgbToXyz = CreateMat(), XyzToLinRgb = LinRgbToXyz.Inverse();

    public static Tri FromXyz(Tri xyz) => XyzToLinRgb * xyz;
    public static Tri ToXyz(Tri rgb) => LinRgbToXyz * rgb;

    static Mat CreateMat() {
        var (rx, ry, rz) = Bt709.RXyz;
        var (gx, gy, gz) = Bt709.GXyz;
        var (bx, by, bz) = Bt709.BXyz;
        Mat p = new(rx, gx, bx, ry, gy, by, rz, gz, bz);
        return p.ScaleCols(p.Inverse() * D65.Xyz);
    }
}
