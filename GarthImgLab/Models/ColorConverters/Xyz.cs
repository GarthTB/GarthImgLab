namespace GarthImgLab.Models.ColorConverters;

public static class Xyz {
    public static (double X, double Y, double Z) FromLinearSRgb(double r, double g, double b) {
        var x = 0.4123907992659595 * r + .35758433938387796 * g + 0.1804807884018343 * b;
        var y = .21263900587151036 * r + 0.7151686787677559 * g + .07219231536073371 * b;
        var z = .01933081871559185 * r + .11919477979462599 * g + 0.9505321522496606 * b;
        return (x, y, z);
    }

    public static (double R, double G, double B) ToLinearSRgb(double x, double y, double z) {
        var r = +3.2409699419045213 * x - 1.5373831775700935 * y - 0.4986107602930033 * z;
        var g = -0.9692436362808798 * x + 1.8759675015077206 * y + .04155505740717561 * z;
        var b = 0.05563007969699361 * x - .20397695888897657 * y + 1.0569715142428786 * z;
        return (r, g, b);
    }
}
