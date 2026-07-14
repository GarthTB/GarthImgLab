namespace GarthImgLab.Models.ColorConverters;

public interface IColorSpace<T> where T: IColorSpace<T> {
    static abstract double GetCusp(double l, double h);
    static abstract (double L, double C, double H) FromLinearSRgb(double r, double g, double b);
    static abstract (double R, double G, double B) ToLinearSRgb(double l, double c, double h);
}
