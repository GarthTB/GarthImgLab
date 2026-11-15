namespace GarthImgLab.Colour;

readonly record struct Matrix3X3(
    f64 M11,
    f64 M12,
    f64 M13,
    f64 M21,
    f64 M22,
    f64 M23,
    f64 M31,
    f64 M32,
    f64 M33) {
    public (f64 X, f64 Y, f64 Z) Multiply(f64 x, f64 y, f64 z) =>
        (M11 * x + M12 * y + M13 * z, M21 * x + M22 * y + M23 * z, M31 * x + M32 * y + M33 * z);

    public Matrix3X3 Inverse() {
        var c11 = M22 * M33 - M23 * M32;
        var c12 = M23 * M31 - M21 * M33;
        var c13 = M21 * M32 - M22 * M31;
        var invDet = 1 / (M11 * c11 + M12 * c12 + M13 * c13);
        return new(
            c11 * invDet,
            (M32 * M13 - M33 * M12) * invDet,
            (M12 * M23 - M13 * M22) * invDet,
            c12 * invDet,
            (M33 * M11 - M31 * M13) * invDet,
            (M13 * M21 - M11 * M23) * invDet,
            c13 * invDet,
            (M31 * M12 - M32 * M11) * invDet,
            (M11 * M22 - M12 * M21) * invDet);
    }
}
