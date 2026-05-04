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
using Ch = ImageMagick.Channels;
using MColor = ImageMagick.MagickColor;
using MImg = ImageMagick.MagickImage;

/* 结果：CompositeCornersOpt最优
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.300-preview.0.26177.108
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

| Method                 | Color | Ratio | Mean        | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------------- |------ |------ |------------:|----------:|----------:|------:|--------:|----------:|------------:|
| WriteMask_Composite    | #D820 | 0     |    31.35 ms |  0.072 ms |  0.064 ms |  1.00 |    0.00 |  13.34 KB |        1.00 |
| WriteMask_Texture_1    | #D820 | 0     | 1,157.41 ms |  5.339 ms |  4.733 ms | 36.92 |    0.16 |  13.27 KB |        1.00 |
| WriteMask_Texture_32   | #D820 | 0     |    35.46 ms |  0.172 ms |  0.153 ms |  1.13 |    0.01 |   13.2 KB |        0.99 |
| WriteMask_Texture_Size | #D820 | 0     |    31.23 ms |  0.151 ms |  0.141 ms |  1.00 |    0.00 |  13.27 KB |        1.00 |
| CompositeCorners       | #D820 | 0     |    35.93 ms |  0.307 ms |  0.287 ms |  1.15 |    0.01 |   13.4 KB |        1.00 |
| CompositeCornersOpt    | #D820 | 0     |    26.39 ms |  0.134 ms |  0.126 ms |  0.84 |    0.00 |   7.96 KB |        0.60 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D820 | 0.2   |    54.25 ms |  0.387 ms |  0.343 ms |  1.00 |    0.01 |  13.34 KB |        1.00 |
| WriteMask_Texture_1    | #D820 | 0.2   | 2,535.94 ms |  8.443 ms |  7.050 ms | 46.75 |    0.31 |  13.27 KB |        1.00 |
| WriteMask_Texture_32   | #D820 | 0.2   |    99.90 ms |  0.779 ms |  0.729 ms |  1.84 |    0.02 |   13.2 KB |        0.99 |
| WriteMask_Texture_Size | #D820 | 0.2   |    54.30 ms |  0.447 ms |  0.418 ms |  1.00 |    0.01 |  13.27 KB |        1.00 |
| CompositeCorners       | #D820 | 0.2   |    55.91 ms |  0.426 ms |  0.378 ms |  1.03 |    0.01 |   13.4 KB |        1.00 |
| CompositeCornersOpt    | #D820 | 0.2   |    46.55 ms |  0.222 ms |  0.208 ms |  0.86 |    0.01 |   7.96 KB |        0.60 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D820 | 0.5   |    72.73 ms |  0.544 ms |  0.483 ms |  1.00 |    0.01 |  13.34 KB |        1.00 |
| WriteMask_Texture_1    | #D820 | 0.5   | 2,302.34 ms |  8.450 ms |  7.056 ms | 31.66 |    0.22 |  13.27 KB |        1.00 |
| WriteMask_Texture_32   | #D820 | 0.5   |   111.79 ms |  0.662 ms |  0.586 ms |  1.54 |    0.01 |   13.2 KB |        0.99 |
| WriteMask_Texture_Size | #D820 | 0.5   |    72.89 ms |  0.457 ms |  0.427 ms |  1.00 |    0.01 |  13.27 KB |        1.00 |
| CompositeCorners       | #D820 | 0.5   |    75.16 ms |  0.382 ms |  0.357 ms |  1.03 |    0.01 |   13.4 KB |        1.00 |
| CompositeCornersOpt    | #D820 | 0.5   |    65.61 ms |  0.426 ms |  0.377 ms |  0.90 |    0.01 |   7.96 KB |        0.60 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D828 | 0     |    31.37 ms |  0.082 ms |  0.073 ms |  1.00 |    0.00 |  13.34 KB |        1.00 |
| WriteMask_Texture_1    | #D828 | 0     | 1,203.01 ms | 23.943 ms | 26.612 ms | 38.34 |    0.83 |  13.27 KB |        1.00 |
| WriteMask_Texture_32   | #D828 | 0     |    35.63 ms |  0.137 ms |  0.121 ms |  1.14 |    0.00 |   13.2 KB |        0.99 |
| WriteMask_Texture_Size | #D828 | 0     |    31.45 ms |  0.243 ms |  0.227 ms |  1.00 |    0.01 |  13.27 KB |        1.00 |
| CompositeCorners       | #D828 | 0     |    36.19 ms |  0.155 ms |  0.137 ms |  1.15 |    0.00 |   13.5 KB |        1.01 |
| CompositeCornersOpt    | #D828 | 0     |    26.77 ms |  0.095 ms |  0.074 ms |  0.85 |    0.00 |   8.06 KB |        0.60 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D828 | 0.2   |    54.45 ms |  0.238 ms |  0.223 ms |  1.00 |    0.01 |  13.34 KB |        1.00 |
| WriteMask_Texture_1    | #D828 | 0.2   | 2,577.84 ms | 13.739 ms | 12.179 ms | 47.35 |    0.29 |  13.27 KB |        1.00 |
| WriteMask_Texture_32   | #D828 | 0.2   |    99.81 ms |  0.500 ms |  0.444 ms |  1.83 |    0.01 |   13.2 KB |        0.99 |
| WriteMask_Texture_Size | #D828 | 0.2   |    54.30 ms |  0.351 ms |  0.329 ms |  1.00 |    0.01 |  13.27 KB |        1.00 |
| CompositeCorners       | #D828 | 0.2   |    55.96 ms |  0.301 ms |  0.252 ms |  1.03 |    0.01 |   13.5 KB |        1.01 |
| CompositeCornersOpt    | #D828 | 0.2   |    46.74 ms |  0.387 ms |  0.362 ms |  0.86 |    0.01 |   8.06 KB |        0.60 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D828 | 0.5   |    73.07 ms |  0.787 ms |  0.736 ms |  1.00 |    0.01 |  13.34 KB |        1.00 |
| WriteMask_Texture_1    | #D828 | 0.5   | 2,300.42 ms | 10.975 ms | 10.266 ms | 31.48 |    0.33 |  13.27 KB |        1.00 |
| WriteMask_Texture_32   | #D828 | 0.5   |   111.56 ms |  0.418 ms |  0.349 ms |  1.53 |    0.02 |   13.2 KB |        0.99 |
| WriteMask_Texture_Size | #D828 | 0.5   |    73.15 ms |  0.583 ms |  0.545 ms |  1.00 |    0.01 |  13.27 KB |        1.00 |
| CompositeCorners       | #D828 | 0.5   |    75.36 ms |  0.343 ms |  0.320 ms |  1.03 |    0.01 |   13.5 KB |        1.01 |
| CompositeCornersOpt    | #D828 | 0.5   |    66.02 ms |  0.275 ms |  0.244 ms |  0.90 |    0.01 |   8.06 KB |        0.60 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D82F | 0     |    29.81 ms |  0.135 ms |  0.106 ms |  1.00 |    0.00 |  13.23 KB |        1.00 |
| WriteMask_Texture_1    | #D82F | 0     |    25.06 ms |  0.133 ms |  0.125 ms |  0.84 |    0.00 |   13.1 KB |        0.99 |
| WriteMask_Texture_32   | #D82F | 0     |    24.96 ms |  0.121 ms |  0.108 ms |  0.84 |    0.00 |   13.1 KB |        0.99 |
| WriteMask_Texture_Size | #D82F | 0     |    26.24 ms |  0.172 ms |  0.161 ms |  0.88 |    0.01 |  13.17 KB |        1.00 |
| CompositeCorners       | #D82F | 0     |    35.73 ms |  0.181 ms |  0.160 ms |  1.20 |    0.01 |   13.4 KB |        1.01 |
| CompositeCornersOpt    | #D82F | 0     |    32.04 ms |  0.239 ms |  0.223 ms |  1.07 |    0.01 |   8.27 KB |        0.63 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D82F | 0.2   |    52.76 ms |  0.321 ms |  0.300 ms |  1.00 |    0.01 |  13.23 KB |        1.00 |
| WriteMask_Texture_1    | #D82F | 0.2   |    47.77 ms |  0.267 ms |  0.223 ms |  0.91 |    0.01 |   13.1 KB |        0.99 |
| WriteMask_Texture_32   | #D82F | 0.2   |    47.71 ms |  0.282 ms |  0.250 ms |  0.90 |    0.01 |   13.1 KB |        0.99 |
| WriteMask_Texture_Size | #D82F | 0.2   |    49.20 ms |  0.239 ms |  0.212 ms |  0.93 |    0.01 |  13.17 KB |        1.00 |
| CompositeCorners       | #D82F | 0.2   |    56.04 ms |  0.458 ms |  0.406 ms |  1.06 |    0.01 |   13.4 KB |        1.01 |
| CompositeCornersOpt    | #D82F | 0.2   |    52.22 ms |  0.383 ms |  0.359 ms |  0.99 |    0.01 |   8.27 KB |        0.63 |
|                        |       |       |             |           |           |       |         |           |             |
| WriteMask_Composite    | #D82F | 0.5   |    71.22 ms |  0.399 ms |  0.374 ms |  1.00 |    0.01 |  13.23 KB |        1.00 |
| WriteMask_Texture_1    | #D82F | 0.5   |    66.58 ms |  0.350 ms |  0.310 ms |  0.93 |    0.01 |   13.1 KB |        0.99 |
| WriteMask_Texture_32   | #D82F | 0.5   |    66.29 ms |  0.327 ms |  0.306 ms |  0.93 |    0.01 |   13.1 KB |        0.99 |
| WriteMask_Texture_Size | #D82F | 0.5   |    67.74 ms |  0.388 ms |  0.363 ms |  0.95 |    0.01 |  13.17 KB |        1.00 |
| CompositeCorners       | #D82F | 0.5   |    75.23 ms |  0.432 ms |  0.404 ms |  1.06 |    0.01 |   13.4 KB |        1.01 |
| CompositeCornersOpt    | #D82F | 0.5   |    71.37 ms |  0.372 ms |  0.329 ms |  1.00 |    0.01 |   8.27 KB |        0.63 | */

[MemoryDiagnoser]
public class RoundCornerBench {
    private readonly MImg _img = new("F:/Photos/Test/lena_std.tif");
    [Params("#D82F", "#D828", "#D820")] public string Color = "#D828";
    [Params(0, .2, .5)] public double Ratio = .2;

    [Benchmark(Baseline = true)]
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

    /* [Benchmark] Drawables会绕过蒙版，效果错误
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

    [Benchmark]
    public string WriteMask_Texture_1() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg mask = new(Black, w, h);
        new Drawables().FillColor(White).RoundRectangle(0, 0, w, h, r, r).Draw(mask);
        img.SetWriteMask(mask);
        try {
            using MImg tmp = new(color, 1, 1);
            img.Texture(tmp);
        } finally { img.RemoveWriteMask(); }

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string WriteMask_Texture_32() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg mask = new(Black, w, h);
        new Drawables().FillColor(White).RoundRectangle(0, 0, w, h, r, r).Draw(mask);
        img.SetWriteMask(mask);
        try {
            using MImg tmp = new(color, 32, 32);
            img.Texture(tmp);
        } finally { img.RemoveWriteMask(); }

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string WriteMask_Texture_Size() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg mask = new(Black, w, h);
        new Drawables().FillColor(White).RoundRectangle(0, 0, w, h, r, r).Draw(mask);
        img.SetWriteMask(mask);
        try {
            using MImg tmp = new(color, w, h);
            img.Texture(tmp);
        } finally { img.RemoveWriteMask(); }

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string CompositeCorners() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        using MImg a = new(new MColor { A = (ushort)~color.A }, w, h);
        new Drawables().FillColor(Black).RoundRectangle(0, 0, w, h, r, r).Draw(a); // 内不透外反A
        a.Negate(Ch.Alpha); // 内透外原A
        using MImg corners = new(color, w, h);
        corners.Composite(a, CopyAlpha);
        img.Composite(corners, Over);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string CompositeCornersOpt() {
        using var img = Prepare(out var w, out var h, out var r, out var color);

        var notA = (ushort)~color.A;
        using MImg corners = new(new MColor(color) { A = notA }, w, h);
        new Drawables().FillColor(new MColor(color) { A = 65535 })
            .RoundRectangle(0, 0, w, h, r, r)
            .Draw(corners); // 内不透外反A
        corners.Negate(Ch.Alpha); // 内透外原A
        if (notA == 0) corners.Colorize(color, new(100)); // 若A为0，RGB会被置0，需重新上色
        img.Composite(corners, Over);

        // Write(img);
        return img.Signature;
    }

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
