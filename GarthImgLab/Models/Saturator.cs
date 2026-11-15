namespace GarthImgLab.Models;

using static Math;

sealed class Saturator: IEffect {
    readonly Func<f64, f64>? _f;

    public Saturator(f64 amount, bool hard) =>
        _f = (amount, hard) switch {
            (< 0, true) => sat => sat * amount,
            (< 0, false) or (> 0, true) => sat => Pow(sat, Pow(2 * E, -amount)),
            (> 0, false) => sat => {
                var t = 1 - sat;
                var tp = Pow(t, 2.8);
                return sat + amount * 2 * t * (1 - tp) / (2 - tp);
            },
            _ => null
        };

    public void Apply(Img img, CT ct) => throw new NotImplementedException();
}
