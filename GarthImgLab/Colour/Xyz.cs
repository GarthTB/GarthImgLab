namespace GarthImgLab.Colour;

readonly record struct Xyz(double X, double Y, double Z) {
    public static readonly Xyz D65 = new(31272d / 32903, 1, 35825d / 32903);
}
