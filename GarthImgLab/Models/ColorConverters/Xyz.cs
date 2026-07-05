namespace GarthImgLab.Models.ColorConverters;

using static SRgb;

public static class Xyz {
    public static (double X, double Y, double Z) FromSRgb(double r, double g, double b) {
        var lr = SRgbToLinear(r);
        var lg = SRgbToLinear(g);
        var lb = SRgbToLinear(b);

        var x = 0.4123907992659595 * lr + .35758433938387796 * lg + 0.1804807884018343 * lb;
        var y = .21263900587151036 * lr + 0.7151686787677559 * lg + .07219231536073371 * lb;
        var z = .01933081871559185 * lr + .11919477979462599 * lg + 0.9505321522496606 * lb;

        return (x, y, z);
    }

    public static (double R, double G, double B) ToSRgb(double x, double y, double z) {
        var lr = +3.2409699419045213 * x - 1.5373831775700935 * y - 0.4986107602930033 * z;
        var lg = -0.9692436362808798 * x + 1.8759675015077206 * y + .04155505740717561 * z;
        var lb = 0.05563007969699361 * x - .20397695888897657 * y + 1.0569715142428786 * z;

        return (LinearToSRgb(lr), LinearToSRgb(lg), LinearToSRgb(lb));
    }
}
