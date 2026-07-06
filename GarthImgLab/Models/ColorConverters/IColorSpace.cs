namespace GarthImgLab.Models.ColorConverters;

public interface IColorSpace<T> where T: IColorSpace<T> {
    static abstract double MaxSat { get; }
    static abstract (double L, double C, double H) FromSRgb(double r, double g, double b);
    static abstract (double R, double G, double B) ToSRgb(double l, double c, double h);
}
