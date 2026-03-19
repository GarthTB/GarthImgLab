// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace DevTools;

using BenchmarkDotNet.Attributes;
using ImageMagick;
using ImageMagick.Drawing;

/* 结果：NotNegate最优
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8037/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

| Method    | Color | Ratio | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|---------- |------ |------ |---------:|---------:|---------:|------:|----------:|------------:|
| CopyAlpha | #4820 | 0     | 35.74 ms | 0.171 ms | 0.160 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #4820 | 0     | 26.00 ms | 0.050 ms | 0.039 ms |  0.73 |   7.96 KB |        0.59 |
| WriteMask | #4820 | 0     | 31.67 ms | 0.166 ms | 0.147 ms |  0.89 |  13.34 KB |        1.00 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #4820 | 0.25  | 61.07 ms | 0.316 ms | 0.295 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #4820 | 0.25  | 51.82 ms | 0.485 ms | 0.453 ms |  0.85 |   7.96 KB |        0.59 |
| WriteMask | #4820 | 0.25  | 59.52 ms | 0.435 ms | 0.407 ms |  0.97 |  13.34 KB |        1.00 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #4828 | 0     | 36.33 ms | 0.281 ms | 0.249 ms |  1.00 |   13.5 KB |        1.00 |
| NotNegate | #4828 | 0     | 26.29 ms | 0.133 ms | 0.125 ms |  0.72 |   8.06 KB |        0.60 |
| WriteMask | #4828 | 0     | 31.38 ms | 0.193 ms | 0.180 ms |  0.86 |  13.34 KB |        0.99 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #4828 | 0.25  | 61.24 ms | 0.559 ms | 0.495 ms |  1.00 |   13.5 KB |        1.00 |
| NotNegate | #4828 | 0.25  | 51.93 ms | 0.215 ms | 0.201 ms |  0.85 |   8.06 KB |        0.60 |
| WriteMask | #4828 | 0.25  | 59.64 ms | 0.226 ms | 0.211 ms |  0.97 |  13.34 KB |        0.99 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #482F | 0     | 35.63 ms | 0.156 ms | 0.146 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #482F | 0     | 31.99 ms | 0.275 ms | 0.258 ms |  0.90 |   8.27 KB |        0.62 |
| WriteMask | #482F | 0     | 36.39 ms | 1.390 ms | 4.100 ms |  1.02 |  13.23 KB |        0.99 |
|           |       |       |          |          |          |       |           |             |
| CopyAlpha | #482F | 0.25  | 65.01 ms | 1.137 ms | 2.271 ms |  1.00 |   13.4 KB |        1.00 |
| NotNegate | #482F | 0.25  | 62.75 ms | 1.245 ms | 1.278 ms |  0.97 |   8.27 KB |        0.62 |
| WriteMask | #482F | 0.25  | 60.20 ms | 1.115 ms | 1.095 ms |  0.93 |  13.23 KB |        0.99 | */

[MemoryDiagnoser]
public class RoundCornerBench {
    private readonly MagickImage _img = new("F:/Photos/Test/lena_std.tif");
    [Params("#482F", "#4828", "#4820")] public string Color = "";
    [Params(0, .25)] public double Ratio = 0;

    [Benchmark(Baseline = true)]
    public string CopyAlpha() {
        using var img = _img.Clone();
        uint w = img.Width, h = img.Height;
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
        uint w = img.Width, h = img.Height;
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
        uint w = img.Width, h = img.Height;
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
