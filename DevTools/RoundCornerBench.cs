// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace DevTools;

using BenchmarkDotNet.Attributes;
using ImageMagick;
using ImageMagick.Drawing;

/* 结果：NotNegate最优
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7922/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.200-preview.0.26103.119
  [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3

| Method    | Color | Ratio | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|---------- |------ |------ |---------:|---------:|---------:|------:|----------:|------------:|
| CopyAlpha | #4820 | 0     | 36.61 ms | 0.104 ms | 0.092 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #4820 | 0     | 26.15 ms | 0.088 ms | 0.078 ms |  0.71 |   7.96 KB |        0.59 |
| WriteMask | #4820 | 0     | 31.24 ms | 0.153 ms | 0.143 ms |  0.85 |  13.34 KB |        1.00 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #4820 | 0.25  | 61.99 ms | 0.386 ms | 0.322 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #4820 | 0.25  | 51.97 ms | 0.238 ms | 0.211 ms |  0.84 |   7.96 KB |        0.59 |
| WriteMask | #4820 | 0.25  | 59.56 ms | 0.552 ms | 0.516 ms |  0.96 |  13.34 KB |        1.00 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #4828 | 0     | 36.60 ms | 0.341 ms | 0.319 ms |  1.00 |   13.5 KB |        1.00 |
| NotNegate | #4828 | 0     | 26.39 ms | 0.141 ms | 0.132 ms |  0.72 |   8.06 KB |        0.60 |
| WriteMask | #4828 | 0     | 31.27 ms | 0.153 ms | 0.143 ms |  0.85 |  13.34 KB |        0.99 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #4828 | 0.25  | 62.00 ms | 0.270 ms | 0.252 ms |  1.00 |   13.5 KB |        1.00 |
| NotNegate | #4828 | 0.25  | 52.02 ms | 0.348 ms | 0.325 ms |  0.84 |   8.06 KB |        0.60 |
| WriteMask | #4828 | 0.25  | 59.63 ms | 0.388 ms | 0.363 ms |  0.96 |  13.34 KB |        0.99 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #482F | 0     | 36.16 ms | 0.264 ms | 0.247 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #482F | 0     | 31.03 ms | 0.177 ms | 0.166 ms |  0.86 |   8.25 KB |        0.62 |
| WriteMask | #482F | 0     | 29.78 ms | 0.131 ms | 0.116 ms |  0.82 |  13.23 KB |        0.99 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #482F | 0.25  | 61.49 ms | 0.340 ms | 0.318 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #482F | 0.25  | 56.75 ms | 0.211 ms | 0.176 ms |  0.92 |   8.25 KB |        0.62 |
| WriteMask | #482F | 0.25  | 57.98 ms | 0.375 ms | 0.351 ms |  0.94 |  13.23 KB |        0.99 | */

[MemoryDiagnoser]
public class RoundCornerBench
{
    private readonly MagickImage _img = new("F:/Photos/Test/lena_std.tif");
    [Params("#482F", "#4828", "#4820")] public string Color = "";
    [Params(0, .25)] public double Ratio = 0;

    [Benchmark(Baseline = true)]
    public string CopyAlpha() {
        using var img = _img.Clone();
        var (w, h) = (img.Width, img.Height);
        var r = Ratio * Math.Min(w, h);
        MagickColor color = new(Color);

        using MagickImage mask = new(new MagickColor(color) { A = (ushort)~color.A }, w, h);
        new Drawables().FillColor(MagickColors.Black)
            .RoundRectangle(0, 0, w, h, r, r)
            .Draw(mask); // 内不透外反A
        mask.Negate(Channels.Alpha); // 内透外原A
        using MagickImage corners = new(color, w, h);
        corners.Composite(mask, CompositeOperator.CopyAlpha);
        img.Composite(corners, CompositeOperator.Over);

        return img.Signature;
    }

    [Benchmark]
    public string NotNegate() {
        using var img = _img.Clone();
        var (w, h) = (img.Width, img.Height);
        var r = Ratio * Math.Min(w, h);
        MagickColor color = new(Color);

        var notA = (ushort)~color.A;
        using MagickImage mask = new(new MagickColor(color) { A = notA }, w, h);
        new Drawables().FillColor(new MagickColor(color) { A = 65535 })
            .RoundRectangle(0, 0, w, h, r, r)
            .Draw(mask); // 内不透外反A
        mask.Negate(Channels.Alpha); // 内透外原A
        if (notA == 0) mask.Colorize(color, new(100)); // 若A为0，RGB会被置0，需重新上色
        img.Composite(mask, CompositeOperator.Over);

        return img.Signature;
    }

    [Benchmark]
    public string WriteMask() {
        using var img = _img.Clone();
        var (w, h) = (img.Width, img.Height);
        var r = Ratio * Math.Min(w, h);
        MagickColor color = new(Color);

        using MagickImage mask = new(MagickColors.Black, w, h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, w, h, r, r).Draw(mask);
        img.SetWriteMask(mask);
        try { // Drawables会忽略mask，只能Composite
            using MagickImage colorLayer = new(color, w, h);
            img.Composite(colorLayer, CompositeOperator.Over);
        } finally { img.RemoveWriteMask(); } // 恢复状态

        return img.Signature;
    }
}
