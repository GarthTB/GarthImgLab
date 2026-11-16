namespace GarthImgLab.Colour;

using static Xyz;

/// <summary> CIE 015:2018 </summary>
readonly record struct Xyz(f64 X, f64 Y, f64 Z) {
    public static Xyz FromXy(f64 x, f64 y) => new(x / y, 1, (1 - x - y) / y);
}

/// <summary> ITU-R BT.709-6 </summary>
static class Bt709 {
    const f64 Rx = .64, Ry = .33, Gx = .3, Gy = .6, Bx = .15, By = .06;
    public static readonly Xyz R = FromXy(Rx, Ry), G = FromXy(Gx, Gy), B = FromXy(Bx, By);
}

/// <summary> ISO/CIE 11664-2:2022 </summary>
static class D65 {
    /// <summary> ISO/CIE 11664-1:2019 <br/> ISO/CIE 11664-3:2019 </summary>
    public const f64 X = .95047055865428297, Y = 1, Z = 1.08882873639588466;

    /// <summary> ISO/CIE 11664-5:2024 </summary>
    public const f64 Up = .197839974380297418, Vp = .4683363817033085536;
}

/// <summary> CIE 248:2022 </summary>
static class Cam {
    public const f64 DimC = .59, DimNc = .9, DimF = .9, YbOverYw = .2;

    public static readonly Mat3X3 M16 = new(
        .401288,
        .650173,
        -.051461,
        -.250268,
        1.204414,
        .045854,
        -.002079,
        .048952,
        .953127);
}
