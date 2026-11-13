namespace GarthImgLab.Colour;

readonly record struct Matrix3X3(
    double M11,
    double M12,
    double M13,
    double M21,
    double M22,
    double M23,
    double M31,
    double M32,
    double M33) {
    public (double X, double Y, double Z) Multiply(double x, double y, double z) =>
        (M11 * x + M12 * y + M13 * z, M21 * x + M22 * y + M23 * z, M31 * x + M32 * y + M33 * z);

    public Matrix3X3 Inverse() {
        var c11 = M22 * M33 - M23 * M32;
        var c12 = M23 * M31 - M21 * M33;
        var c13 = M21 * M32 - M22 * M31;

        var det = M11 * c11 + M12 * c12 + M13 * c13;
        if (Math.Abs(det) < 1e-12) throw new InvalidOperationException("矩阵不可逆");

        var invDet = 1 / det;
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
