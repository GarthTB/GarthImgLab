namespace GarthImgLab.Colour;

readonly record struct Xyz(double X, double Y, double Z) {
    public static Xyz D65 => new(.95047, 1, 1.08883);
}
