namespace GarthImgLab.Colour;

using static Math;

/// <summary> arXiv:2606.05255 </summary>
readonly record struct OkLChPlus(f64 L, f64 C, f64 H) {
    const f64 Alpha = .73, N = .87, Sigma = .34;
    static readonly f64 SigmaN = Pow(Sigma, N);

    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(new SRgb(1, 1, 1).ToXyz()).L,
        FromXyz(new SRgb(1, 0, 1).ToXyz()).C,
        static (l, c, h) => new OkLChPlus(l, c, h).ToXyz());

    public static OkLChPlus FromXyz(Xyz xyz) {
        var (l, c, h) = OkLCh.FromXyz(xyz);
        var lp = Pow(l, Alpha);
        var cn = Pow(c, N);
        var cp = cn / (cn + SigmaN);
        return new(lp, cp, h);
    }

    public Xyz ToXyz() {
        var l = Pow(L, 1 / Alpha);
        var c = Pow(SigmaN * C / (1 - C), 1 / N);
        return new OkLCh(l, c, H).ToXyz();
    }
}
