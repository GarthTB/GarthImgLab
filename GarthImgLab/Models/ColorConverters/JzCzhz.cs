namespace GarthImgLab.Models.ColorConverters;

using static Math;
using static PQ;

public readonly struct JzCzhz: IColorSpace<JzCzhz> {
    public static double MaxSat { get; } = FromSRgb(0, 0, 1).C;
    private const double B = 1.15, G = .66, D = -.56, D0 = 1.6295499532821566e-11;

    public static (double L, double C, double H) FromSRgb(double r, double g, double b) {
        var (x, y, z) = Xyz.FromSRgb(r, g, b);

        var xp = B * x - (B - 1) * z;
        var yp = G * y - (G - 1) * x;

        var l = .41478972 * xp + 0.579999 * yp + 0.014648 * z;
        var m = -.2015100 * xp + 1.120649 * yp + .0531008 * z;
        var s = -.0166008 * xp + 0.264800 * yp + .6684799 * z;

        var lp = InvEotf(l);
        var mp = InvEotf(m);
        var sp = InvEotf(s);

        var iz = .5 * lp + .5 * mp;
        var az = 3.52400 * lp - 4.066708 * mp + 0.542708 * sp;
        var bz = .199076 * lp + 1.096799 * mp - 1.295875 * sp;

        var j = (1 + D) * iz / (1 + D * iz) - D0;
        var c = Sqrt(az * az + bz * bz);
        var h = Atan2(bz, az) * (180 / PI);
        if (h < 0) h += 360;

        return (j, c, h);
    }

    public static (double R, double G, double B) ToSRgb(double j, double c, double h) {
        var hzRad = h * (PI / 180);
        var iz = (j + D0) / (1 + D - D * (j + D0));
        var az = c * Cos(hzRad);
        var bz = c * Sin(hzRad);

        var lp = iz + 0.1386050432715393 * az + .058047316156118876 * bz;
        var mp = iz - 0.1386050432715393 * az - .058047316156118876 * bz;
        var sp = iz - .09601924202631895 * az - 0.81189189605603900 * bz;

        var l = Eotf(lp);
        var m = Eotf(mp);
        var s = Eotf(sp);

        var xp = 1.92422643578760670 * l - 1.0047923125953655 * m + .03765140403061801 * s;
        var yp = 0.35031676209499907 * l + 0.7264811939316552 * m - .06538442294808502 * s;
        var zp = -.09098281098284758 * l - 0.3127282905230740 * m + 1.5227665613052606 * s;

        var x = (xp + (B - 1) * zp) / B;
        var y = (yp + (G - 1) * x) / G;

        return Xyz.ToSRgb(x, y, zp);
    }
}

file static class PQ {
    private const double C1 = 3424d / 4096, C2 = 2413d / 128, C3 = 2392d / 128;
    private const double N = 2610d / 16384, P = 134.034375;
    private const double WhiteL = 203;

    public static double Eotf(double ep) {
        var absEp = Abs(ep);
        var rootP = Pow(absEp, 1 / P);
        var top = Max(rootP - C1, 0);
        var btm = C2 - C3 * rootP;
        var y = Pow(top / btm, 1 / N);
        return CopySign(10000 * y / WhiteL, ep);
    }

    public static double InvEotf(double f) {
        var y = f * WhiteL / 10000;
        var absY = Abs(y);
        var powN = Pow(absY, N);
        var top = C1 + C2 * powN;
        var btm = 1 + C3 * powN;
        return CopySign(Pow(top / btm, P), y);
    }
}
