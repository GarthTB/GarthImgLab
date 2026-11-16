namespace GarthImgLab.Colour;

using static Xyz;

/// <summary> BT.709-6 </summary>
static class Bt709 {
    const f64 Rx = .64, Ry = .33, Gx = .3, Gy = .6, Bx = .15, By = .06;
    public static readonly Xyz R = FromXy(Rx, Ry), G = FromXy(Gx, Gy), B = FromXy(Bx, By);
}

/// <summary> CIE 015:2018 </summary>
static class D65 {
    const f64 X = .31272, Y = .32903;
    public static readonly Xyz W = FromXy(X, Y);
}

/// <summary> CIE 015:2018 </summary>
readonly record struct Xyz(f64 X, f64 Y, f64 Z) {
    public static Xyz FromXy(f64 x, f64 y) => new(x / y, 1, (1 - x - y) / y);
}
