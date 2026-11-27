namespace Tests.Colour;

using GarthImgLab.Colour;
using Xunit;
using static Math;
using static Xunit.Assert;

public sealed class RoundTrip {
    const double Tol = 1e-11; // sRGB 绝对容差
    const int Samples = 65536; // 随机点数；另有结构点
    static readonly Tri[] Points = [..GenPoints()];

    [Fact] public static void ViaHsv() => Direct(Hsv.FromSrgb, Hsv.ToSrgb);
    [Fact] public static void ViaHsl() => Direct(Hsl.FromSrgb, Hsl.ToSrgb);
    [Fact] public static void ViaCieLChab() => ViaXyz(CieLChab.FromXyz, CieLChab.ToXyz);
    [Fact] public static void ViaCieLChuv() => ViaXyz(CieLChuv.FromXyz, CieLChuv.ToXyz);
    [Fact] public static void ViaJzCzhz() => ViaXyz(JzCzhz.FromXyz, JzCzhz.ToXyz);
    [Fact] public static void ViaOkLCh() => ViaXyz(OkLCh.FromXyz, OkLCh.ToXyz);
    [Fact] public static void ViaOkLChP() => ViaXyz(OkLChP.FromXyz, OkLChP.ToXyz);
    [Fact] public static void ViaSUcs() => ViaXyz(SUcs.FromXyz, SUcs.ToXyz);

    static void Direct(Func<Tri, Tri> to, Func<Tri, Tri> from) => Check(p => from(to(p)));

    static void ViaXyz(Func<Tri, Tri> to, Func<Tri, Tri> from) =>
        Check(p => SRgb.FromXyz(from(to(SRgb.ToXyz(p)))));

    static void Check(Func<Tri, Tri> rt) {
        var bad = 0;
        var worst = 0d;
        Tri src = default, dst = default;
        foreach (var p in Points) {
            var q = rt(p);
            Tri diff = new(p.C1 - q.C1, p.C2 - q.C2, p.C3 - q.C3);
            var max = diff.Apply(Abs).Max;
            if (!(max <= worst)) (worst, src, dst) = (max, p, q); // !(<=) 包括 NaN
            if (!(max <= Tol)) bad++;
        }
        True(bad == 0, $"{bad}/{Points.Length} 点超差；Tol={Tol:G3}，最大偏差 {worst:R}，输入 {src}，输出 {dst}");
    }

    static IEnumerable<Tri> GenPoints() {
        double[] v = [
            0,
            1d / 65535,
            1d / 255,
            .5,
            1 - 1d / 255,
            1 - 1d / 65535,
            1,
            ..Edge(.040448236277108192), // sRGB 拐点
            ..Edge(1.055 * Pow(216d / 24389, 1 / 2.4) - .055) // CIE F/InvF 拐点
        ];
        foreach (var r in v)
        foreach (var g in v)
        foreach (var b in v)
            yield return new(r, g, b);

        Random rand = new(42);
        for (var i = 0; i < Samples; i++)
            yield return new(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
    }

    static double[] Edge(double v) => [BitDecrement(v), v, BitIncrement(v)];
}
