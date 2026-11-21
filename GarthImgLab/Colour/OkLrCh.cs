namespace GarthImgLab.Colour;

/// <summary> https://bottosson.github.io/posts/colorpicker/ </summary>
static class OkLrCh {
    const f64 K1 = .206, K2 = .03, K3 = (1 + K1) / (1 + K2);

    public static readonly MaxCLut MaxC = MaxCLut.Create(
        FromXyz(SRgb.ToXyz(new(1, 1, 1))).C1,
        FromXyz(SRgb.ToXyz(new(1, 0, 1))).C2,
        ToXyz);

    public static Tri FromXyz(in Tri xyz) {
        var (l, c, h) = OkLCh.FromXyz(xyz);
        var d = K3 * l - K1;
        var lr = .5 * (d + Math.Sqrt(d * d + 4 * K2 * K3 * l));
        return new(lr, c, h);
    }

    public static Tri ToXyz(in Tri lch) {
        var (lr, c, h) = lch;
        var l = lr * (lr + K1) / (K3 * (lr + K2));
        return OkLCh.ToXyz(new(l, c, h));
    }
}
