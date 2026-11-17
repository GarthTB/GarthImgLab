namespace GarthImgLab.Colour;

using static Xyz;

/// <summary> CIE 015:2018 </summary>
readonly record struct Xyz(f64 X, f64 Y, f64 Z) {
    public static Xyz FromXy(f64 x, f64 y) => new(x / y, 1, (1 - x - y) / y);
}

/// <summary> ISO/CIE 11664-2:2022 </summary>
static class D65 {
    /// <summary> ISO/CIE 11664-1:2019 <br/> ISO/CIE 11664-3:2019 </summary>
    public const f64 X = .95047055865428297, Y = 1, Z = 1.08882873639588466;

    /// <summary> ISO/CIE 11664-5:2024 </summary>
    public const f64 Up = .197839974380297418, Vp = .4683363817033085536;
}

/// <summary> ITU-R BT.709-6 </summary>
static class Bt709 {
    public static readonly Xyz R = FromXy(.64, .33), G = FromXy(.3, .6), B = FromXy(.15, .06);
}

/// <summary> ITU-R BT.2408-9 </summary>
static class Bt2408 {
    public const f64 SdrWhiteLum = 203;
}
