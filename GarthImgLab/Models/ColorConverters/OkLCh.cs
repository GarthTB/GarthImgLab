namespace GarthImgLab.Models.ColorConverters;

using static Math;
using static SRgb;

public readonly struct OkLCh: IColorSpace<OkLCh> {
    public static double MaxSat => .32249098837223966; // (1,0,1)

    public static (double L, double C, double H) FromSRgb(double r, double g, double b) {
        var lr = SRgbToLinear(r);
        var lg = SRgbToLinear(g);
        var lb = SRgbToLinear(b);

        var l = 0.41222147014600025 * lr + .53633253814027138 * lg + .05144599335177303 * lb;
        var m = 0.21190349584501836 * lr + .68069955068061472 * lg + .10739695354033429 * lb;
        var s = .088302459229775782 * lr + .28171883918779955 * lg + .62997870167877923 * lb;

        var lc = Cbrt(l);
        var mc = Cbrt(m);
        var sc = Cbrt(s);

        var okL = 0.21045426830931405 * lc + .79361777470230532 * mc - .0040720430116192871 * sc;
        var okA = 1.97799853243116860 * lc - 2.4285922420485799 * mc + 0.450593709617411130 * sc;
        var okB = .025904042465547741 * lc + .78277171245752986 * mc - 0.808675754923077640 * sc;

        var c = Sqrt(okA * okA + okB * okB);
        var h = Atan2(okB, okA) * (180 / PI);
        if (h < 0) h += 360;

        return (okL, c, h);
    }

    public static (double R, double G, double B) ToSRgb(double okL, double c, double h) {
        var hRad = h * (PI / 180);
        var okA = c * Cos(hRad);
        var okB = c * Sin(hRad);

        var lc = okL + .39633777737617493 * okA + 0.21580375730991361 * okB;
        var mc = okL - 0.1055613458156586 * okA - 0.06385417282581329 * okB;
        var sc = okL - 0.0894841775298119 * okA - 1.29148554801940920 * okB;

        var l = lc * lc * lc;
        var m = mc * mc * mc;
        var s = sc * sc * sc;

        var lr = +4.07674162959401800 * l - 3.3077115392580612 * m + .23096990318210434 * s;
        var lg = -1.26843797134654460 * l + 2.6097573492876887 * m - .34131937600265722 * s;
        var lb = -.004196076249935685 * l - 0.7034186179359363 * m + 1.7076146940746117 * s;

        return (LinearToSRgb(lr), LinearToSRgb(lg), LinearToSRgb(lb));
    }
}
