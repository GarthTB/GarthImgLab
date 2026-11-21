namespace GarthImgLab.Colour;

/// <summary> CIE 015:2018 </summary>
static class Xyz {
    public static Tri FromXy(f64 x, f64 y) => new(x / y, 1, (1 - x - y) / y);
}
