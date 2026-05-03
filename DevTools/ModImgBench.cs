// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable UnusedMember.Local

namespace DevTools;

using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using ImageMagick;
using static Math;

/* 结果：GetPixelsUnsafe_GetAreaPointer最优
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.300-preview.0.26177.108
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

| Method                              | Mean     | Error    | StdDev   | Ratio | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|------------------------------------ |---------:|---------:|---------:|------:|---------:|---------:|---------:|-----------:|------------:|
| GetPixels_GetValues_SetPixels       | 17.53 ms | 0.071 ms | 0.060 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 | 1538.32 KB |       1.000 |
| GetPixelsUnsafe_GetValues_SetPixels | 17.59 ms | 0.119 ms | 0.106 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 | 1538.32 KB |       1.000 |
| GetPixels_GetArea_SetPixels         | 17.58 ms | 0.122 ms | 0.115 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 | 1538.32 KB |       1.000 |
| GetPixelsUnsafe_GetArea_SetPixels   | 17.56 ms | 0.050 ms | 0.047 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 | 1538.32 KB |       1.000 |
| GetPixelsUnsafe_GetAreaPointer      | 16.81 ms | 0.094 ms | 0.078 ms |  0.96 |        - |        - |        - |    2.14 KB |       0.001 | */

[MemoryDiagnoser]
public class ModImgBench {
    private const int Max = (1 << 16) - 1;
    private const double InvMax = 1d / Max;
    private readonly MagickImage _img = new("F:/Photos/Test/lena_std.tif");

    [Benchmark(Baseline = true)]
    public string GetPixels_GetValues_SetPixels() {
        using var img = Prepare(out _, out _, out var ch);

        using var px = img.GetPixels();
        var arr = px.GetValues()!;
        for (var i = 0; i < arr.Length; i += ch) ModPx(ref arr[i], ref arr[i + 1], ref arr[i + 2]);
        px.SetPixels(arr);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string GetPixelsUnsafe_GetValues_SetPixels() {
        using var img = Prepare(out _, out _, out var ch);

        using var px = img.GetPixelsUnsafe();
        var arr = px.GetValues()!;
        for (var i = 0; i < arr.Length; i += ch) ModPx(ref arr[i], ref arr[i + 1], ref arr[i + 2]);
        px.SetPixels(arr);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string GetPixels_GetArea_SetPixels() {
        using var img = Prepare(out var w, out var h, out var ch);

        using var px = img.GetPixels();
        var arr = px.GetArea(0, 0, w, h)!;
        for (var i = 0; i < arr.Length; i += ch) ModPx(ref arr[i], ref arr[i + 1], ref arr[i + 2]);
        px.SetPixels(arr);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public string GetPixelsUnsafe_GetArea_SetPixels() {
        using var img = Prepare(out var w, out var h, out var ch);

        using var px = img.GetPixelsUnsafe();
        var arr = px.GetArea(0, 0, w, h)!;
        for (var i = 0; i < arr.Length; i += ch) ModPx(ref arr[i], ref arr[i + 1], ref arr[i + 2]);
        px.SetPixels(arr);

        // Write(img);
        return img.Signature;
    }

    [Benchmark]
    public unsafe string GetPixelsUnsafe_GetAreaPointer() {
        using var img = Prepare(out var w, out var h, out var ch);

        using var px = img.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, w, h);
        for (var i = 0; i < w * h * ch; i += ch) ModPx(ref ptr[i], ref ptr[i + 1], ref ptr[i + 2]);

        // Write(img);
        return img.Signature;
    }

    private IMagickImage<ushort> Prepare(out uint w, out uint h, out int ch) {
        (w, h, ch) = (_img.Width, _img.Height, (int)_img.ChannelCount);
        return _img.CloneArea(w, h); // 彻底隔离原图
    }

    private static void ModPx(ref ushort r, ref ushort g, ref ushort b) {
        r = (ushort)Round(Sqrt(r * InvMax) * Max);
        g = (ushort)Round(Sqrt(g * InvMax) * Max);
        b = (ushort)Round(Sqrt(b * InvMax) * Max);
    }

    private void Write(IMagickImage<ushort> img, [CallerMemberName] string name = "") {
        _img.Write($"F:/Photos/Test/lena_std_{name}_Bef.tif");
        img.Write($"F:/Photos/Test/lena_std_{name}_Aft.tif");
    }
}
