namespace GarthImgLab.Colour;

/// <summary> https://bottosson.github.io/posts/colorpicker/ </summary>
readonly record struct OkLrCh(f64 Lr, f64 C, f64 H) {
    const f64 K1 = .206, K2 = .03, K3 = (1 + K1) / (1 + K2);

    public static OkLrCh FromXyz(Xyz xyz) {
        var (l, c, h) = OkLCh.FromXyz(xyz);
        var d = K3 * l - K1;
        var lr = .5 * (d + Math.Sqrt(d * d + 4 * K2 * K3 * l));
        return new(lr, c, h);
    }

    public Xyz ToXyz() {
        var l = Lr * (Lr + K1) / (K3 * (Lr + K2));
        return new OkLCh(l, C, H).ToXyz();
    }
}
