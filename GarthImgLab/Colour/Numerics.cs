namespace GarthImgLab.Colour;

using static Math;

readonly record struct Tri(f64 C1, f64 C2, f64 C3) {
    public f64 Min => Min(C1, Min(C2, C3));
    public f64 Max => Max(C1, Max(C2, C3));
}

readonly record struct Mat(
    f64 M11,
    f64 M12,
    f64 M13,
    f64 M21,
    f64 M22,
    f64 M23,
    f64 M31,
    f64 M32,
    f64 M33) {
    public static Tri operator *(in Mat m, in Tri v) =>
        new(
            m.M11 * v.C1 + m.M12 * v.C2 + m.M13 * v.C3,
            m.M21 * v.C1 + m.M22 * v.C2 + m.M23 * v.C3,
            m.M31 * v.C1 + m.M32 * v.C2 + m.M33 * v.C3);

    public Mat Inverse() {
        f64 c11 = M22 * M33 - M23 * M32,
            c12 = M23 * M31 - M21 * M33,
            c13 = M21 * M32 - M22 * M31,
            c21 = M32 * M13 - M33 * M12,
            c22 = M33 * M11 - M31 * M13,
            c23 = M31 * M12 - M32 * M11,
            c31 = M12 * M23 - M13 * M22,
            c32 = M13 * M21 - M11 * M23,
            c33 = M11 * M22 - M12 * M21,
            det = M11 * c11 + M12 * c12 + M13 * c13;
        return new(
            c11 / det,
            c21 / det,
            c31 / det,
            c12 / det,
            c22 / det,
            c32 / det,
            c13 / det,
            c23 / det,
            c33 / det);
    }
}
