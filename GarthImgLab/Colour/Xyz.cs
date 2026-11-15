namespace GarthImgLab.Colour;

readonly record struct Xyz(f64 X, f64 Y, f64 Z) {
    public static readonly Xyz D65 = new(31272d / 32903, 1, 35825d / 32903);
}
