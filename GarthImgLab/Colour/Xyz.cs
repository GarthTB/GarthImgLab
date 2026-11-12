namespace GarthImgLab.Colour;

readonly record struct Xyz(double X, double Y, double Z) {
    const double D65X = 31272d / 32903, D65Z = 35825d / 32903;
    public static Xyz D65 => new(D65X, 1, D65Z);
}
