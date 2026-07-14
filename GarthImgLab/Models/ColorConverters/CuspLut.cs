namespace GarthImgLab.Models.ColorConverters;

public static class CuspLut {
    private const int Size = 256;
    private const double HStep = 360d / (Size - 1);

    public static double[] Build<T>(double maxL, double maxC) where T: IColorSpace<T> {
        var cusps = new double[Size * Size];
        var lStep = maxL / (Size - 1);
        for (var y = 0; y < Size; y++) {
            var l = y * lStep;
            for (var x = 0; x < Size; x++) cusps[y * Size + x] = FindCusp<T>(l, x * HStep, maxC);
        }
        return cusps;
    }

    private static double FindCusp<T>(double l, double h, double maxC) where T: IColorSpace<T> {
        const double loLim = -1e-4, hiLim = 1.0001;
        double lo = 0, hi = maxC;
        var (r, g, b) = T.ToLinearSRgb(l, hi, h);
        if (r is > loLim and < hiLim && g is > loLim and < hiLim && b is > loLim and < hiLim)
            return hi;
        for (var i = 0; i < 20; i++) {
            var mid = .5 * (lo + hi);
            (r, g, b) = T.ToLinearSRgb(l, mid, h);
            if (r is > loLim and < hiLim && g is > loLim and < hiLim && b is > loLim and < hiLim)
                lo = mid;
            else
                hi = mid;
        }
        return lo;
    }

    public static double Sample(double[] lut, double maxL, double l, double h) {
        double u = h / 360 * (Size - 1), v = l / maxL * (Size - 1);
        int x0 = u switch { < 0 => 0, > Size - 1 => Size - 1, _ => (int)u },
            y0 = v switch { < 0 => 0, > Size - 1 => Size - 1, _ => (int)v },
            x1 = (x0 + 1) % Size,
            y1 = y0 + 1 == Size
                ? Size - 1
                : y0 + 1;
        double fx = u - x0, fy = v - y0;
        double c00 = lut[y0 * Size + x0], c10 = lut[y0 * Size + x1], c0 = c00 + (c10 - c00) * fx;
        double c01 = lut[y1 * Size + x0], c11 = lut[y1 * Size + x1], c1 = c01 + (c11 - c01) * fx;
        return c0 + (c1 - c0) * fy;
    }
}
