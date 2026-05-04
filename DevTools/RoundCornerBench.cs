// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
// ReSharper disable UnusedMember.Local

namespace DevTools;

using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using ImageMagick;
using ImageMagick.Drawing;
using static ImageMagick.CompositeOperator;
using static ImageMagick.MagickColors;
using MImg = ImageMagick.MagickImage;
using MColor = ImageMagick.MagickColor;

/* 结果：DirectCorners最优
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.300-preview.0.26177.108
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

| Method              | Color | Ratio | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------- |------ |------ |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| TmpCorners          | #4820 | 0     | 36.33 ms | 0.355 ms | 0.332 ms |  1.00 |    0.01 |   13.4 KB |        1.00 |
| DirectCorners       | #4820 | 0     | 26.80 ms | 0.126 ms | 0.118 ms |  0.74 |    0.01 |   7.96 KB |        0.59 |
| WriteMask_Composite | #4820 | 0     | 31.68 ms | 0.182 ms | 0.162 ms |  0.87 |    0.01 |  13.34 KB |        1.00 |
|                     |       |       |          |          |          |       |         |           |             |
| TmpCorners          | #4820 | 0.3   | 63.71 ms | 0.391 ms | 0.366 ms |  1.00 |    0.01 |   13.4 KB |        1.00 |
| DirectCorners       | #4820 | 0.3   | 54.28 ms | 0.343 ms | 0.304 ms |  0.85 |    0.01 |   7.96 KB |        0.59 |
| WriteMask_Composite | #4820 | 0.3   | 61.80 ms | 0.278 ms | 0.260 ms |  0.97 |    0.01 |  13.34 KB |        1.00 |
|                     |       |       |          |          |          |       |         |           |             |
| TmpCorners          | #4828 | 0     | 36.45 ms | 0.217 ms | 0.203 ms |  1.00 |    0.01 |   13.5 KB |        1.00 |
| DirectCorners       | #4828 | 0     | 26.94 ms | 0.111 ms | 0.104 ms |  0.74 |    0.00 |   8.06 KB |        0.60 |
| WriteMask_Composite | #4828 | 0     | 31.66 ms | 0.140 ms | 0.124 ms |  0.87 |    0.01 |  13.34 KB |        0.99 |
|                     |       |       |          |          |          |       |         |           |             |
| TmpCorners          | #4828 | 0.3   | 63.86 ms | 0.393 ms | 0.349 ms |  1.00 |    0.01 |   13.5 KB |        1.00 |
| DirectCorners       | #4828 | 0.3   | 54.37 ms | 0.275 ms | 0.257 ms |  0.85 |    0.01 |   8.06 KB |        0.60 |
| WriteMask_Composite | #4828 | 0.3   | 61.84 ms | 0.310 ms | 0.290 ms |  0.97 |    0.01 |  13.34 KB |        0.99 |
|                     |       |       |          |          |          |       |         |           |             |
| TmpCorners          | #482F | 0     | 36.38 ms | 0.373 ms | 0.349 ms |  1.00 |    0.01 |   13.4 KB |        1.00 |
| DirectCorners       | #482F | 0     | 32.51 ms | 0.176 ms | 0.165 ms |  0.89 |    0.01 |   8.27 KB |        0.62 |
| WriteMask_Composite | #482F | 0     | 30.40 ms | 0.237 ms | 0.222 ms |  0.84 |    0.01 |  13.23 KB |        0.99 |
|                     |       |       |          |          |          |       |         |           |             |
| TmpCorners          | #482F | 0.3   | 64.43 ms | 0.298 ms | 0.264 ms |  1.00 |    0.01 |   13.4 KB |        1.00 |
| DirectCorners       | #482F | 0.3   | 62.45 ms | 1.214 ms | 1.247 ms |  0.97 |    0.02 |   8.27 KB |        0.62 |
| WriteMask_Composite | #482F | 0.3   | 60.68 ms | 0.317 ms | 0.297 ms |  0.94 |    0.01 |  13.23 KB |        0.99 | */

[MemoryDiagnoser]
public class RoundCornerBench {
    private readonly MImg _img = new("F:/Photos/Test/lena_std.tif");
    [Params("#482F", "#4828", "#4820")] public string Color = "#4828";
    [Params(0, .3)] public double Ratio = .3;

    [Benchmark(Baseline = true)]
    public string TmpCorners() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg tmp = new(new MColor(0, 0, 0, (ushort)~color.A), w, h);
        new Drawables().FillColor(Black).RoundRectangle(0, 0, w, h, r, r).Draw(tmp); // 内不透外反A
        tmp.Negate(Channels.Alpha); // 内透外原A
        using MImg corners = new(color, w, h);
        corners.Composite(tmp, CopyAlpha);
        img.Composite(corners, Over);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string DirectCorners() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        var notA = (ushort)~color.A;
        using MImg corners = new(new MColor(color.R, color.G, color.B, notA), w, h);
        new Drawables().FillColor(new MColor(color.R, color.G, color.B))
            .RoundRectangle(0, 0, w, h, r, r)
            .Draw(corners); // 内不透外反A
        corners.Negate(Channels.Alpha); // 内透外原A
        if (notA == 0) corners.Colorize(color, new(100)); // 若A为0，RGB会被置0，需重新上色
        img.Composite(corners, Over);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string WriteMask_Composite() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg mask = new(Black, w, h);
        new Drawables().FillColor(White).RoundRectangle(0, 0, w, h, r, r).Draw(mask);
        img.SetWriteMask(mask);
        try {
            using MImg tmp = new(color, w, h);
            img.Composite(tmp, Over);
        } finally { img.RemoveWriteMask(); }

        // Write(img);
        return img.Signature;
    }

    /* [Benchmark] Drawables绕过蒙版，效果错误
    public string WriteMask_Draw() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg mask = new(Black, w, h);
        new Drawables().FillColor(White).RoundRectangle(0, 0, w, h, r, r).Draw(mask);
        img.SetWriteMask(mask);
        try { new Drawables().FillColor(color).Rectangle(0, 0, w, h).Draw(img); } finally {
            img.RemoveWriteMask();
        }

        // Write(img);
        return img.Signature;
    } */

    private IMagickImage<ushort> Prepare(out uint w, out uint h, out double r, out MColor c) {
        (w, h) = (_img.Width, _img.Height);
        r = Ratio * Math.Min(w, h);
        c = new(Color);
        return _img.Clone();
    }

    private void Write(IMagickImage<ushort> img, [CallerMemberName] string name = "") {
        _img.Write($"F:/Photos/Test/lena_std_{name}_Bef.tif");
        img.Write($"F:/Photos/Test/lena_std_{name}_Aft.tif");
    }
}
