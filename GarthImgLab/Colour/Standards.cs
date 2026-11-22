namespace GarthImgLab.Colour;

/// <summary> ISO/CIE 11664-1:2019 <br/> ISO/CIE 11664-2:2022 </summary>
static class D65 {
    /// <summary> ISO/CIE 11664-3:2019 </summary>
    public const f64 X = .95047055865428297, Y = 1, Z = 1.08882873639588466;

    /// <summary> ISO/CIE 11664-5:2024 </summary>
    public const f64 Up = .197839974380297418, Vp = .4683363817033085536;

    public static readonly Tri Xyz = new(X, Y, Z);
}

/// <summary> ITU-R BT.709-6 </summary>
static class Bt709 {
    const f64 Rx = .64, Ry = .33, Gx = .3, Gy = .6, Bx = .15, By = .06;

    public static readonly Tri RXyz = FromXy(Rx, Ry), GXyz = FromXy(Gx, Gy), BXyz = FromXy(Bx, By);

    /// <summary> CIE 015:2018 </summary>
    static Tri FromXy(f64 x, f64 y) => new(x / y, 1, (1 - x - y) / y);
}

/// <summary> ITU-R BT.2408-9 </summary>
static class Bt2408 {
    public const f64 SdrWhiteLum = 203;
}
