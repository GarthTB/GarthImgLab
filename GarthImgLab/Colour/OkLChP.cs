namespace GarthImgLab.Colour;

using static Math;

/// <summary> arXiv:2606.05255 </summary>
static class OkLChP {
    const f64 Alpha = .73, N = .87, Sigma = .34;
    static readonly f64 SigmaN = Pow(Sigma, N);

    public static readonly Lazy<MaxCLut> MaxCLut = new(static () => new(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 1))).C2,
        ToXyz));

    public static Tri FromXyz(Tri xyz) {
        var (l, c, h) = OkLCh.FromXyz(xyz);
        var lp = Pow(l, Alpha);
        var cn = Pow(c, N);
        var cp = cn / (cn + SigmaN);
        return new(lp, cp, h);
    }

    public static Tri ToXyz(Tri lch) {
        var (lp, cp, h) = lch;
        var l = Pow(lp, 1 / Alpha);
        var c = Pow(SigmaN * cp / (1 - cp), 1 / N);
        return OkLCh.ToXyz(new(l, c, h));
    }
}
