namespace GarthImgLab.Colour;

using static Math;

readonly record struct SRgb(double R, double G, double B) {
    public static SRgb FromXyz(Xyz xyz) {
        var (x, y, z) = xyz;
        var lr = 3.2409699419045213 * x - 1.5373831775700935 * y - .49861076029300330 * z;
        var lg = -.9692436362808798 * x + 1.8759675015077206 * y + .04155505740717561 * z;
        var lb = 0.0556300796969936 * x - .20397695888897657 * y + 1.0569715142428786 * z;
        return new(ToNonLinear(lr), ToNonLinear(lg), ToNonLinear(lb));
    }

    public static Xyz ToXyz(SRgb rgb) {
        var (r, g, b) = rgb;
        double lr = ToLinear(r), lg = ToLinear(g), lb = ToLinear(b);
        return new(
            .41239079926595950 * lr + .35758433938387796 * lg + .18048078840183430 * lb,
            .21263900587151036 * lr + .71516867876775590 * lg + .07219231536073371 * lb,
            .01933081871559185 * lr + .11919477979462598 * lg + .95053215224966060 * lb);
    }

    static double ToNonLinear(double v) =>
        v <= .0031308
            ? v * 12.92
            : 1.055 * Pow(v, 1 / 2.4) - .055;

    static double ToLinear(double v) =>
        v <= .04045
            ? v / 12.92
            : Pow((v + .055) / 1.055, 2.4);
}
