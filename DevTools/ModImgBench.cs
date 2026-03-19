// ReSharper disable ClassCanBeSealed.Global

namespace DevTools;

using BenchmarkDotNet.Attributes;
using ImageMagick;

/* 结果：ModImgSpanPtr最优
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8037/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

| Method        | Mean     | Error    | StdDev   | Ratio | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|-------------- |---------:|---------:|---------:|------:|---------:|---------:|---------:|-----------:|------------:|
| ModImgSafe    | 18.40 ms | 0.061 ms | 0.054 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 | 1538.32 KB |       1.000 |
| ModImgUnsafe  | 18.36 ms | 0.043 ms | 0.038 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 | 1538.32 KB |       1.000 |
| ModImgRawPtr  | 17.65 ms | 0.062 ms | 0.051 ms |  0.96 |        - |        - |        - |    2.14 KB |       0.001 |
| ModImgSpanPtr | 17.65 ms | 0.073 ms | 0.065 ms |  0.96 |        - |        - |        - |    2.14 KB |       0.001 | */

[MemoryDiagnoser]
public class ModImgBench {
    private const double Max = 65535;
    private readonly MagickImage _img = new("F:/Photos/Test/lena_std.tif");

    [Benchmark(Baseline = true)]
    public string ModImgSafe() {
        var (w, h, ch) = (_img.Width, _img.Height, (int)_img.ChannelCount);
        using var clone = _img.CloneArea(w, h);

        using var pixels = clone.GetPixels();
        var span = pixels.GetValues().AsSpan(); // 与GetArea(0, 0, w, h)等效
        for (var i = 0; i < span.Length; i += ch) ModPx(span[i..(i + 3)]);
        pixels.SetPixels(span);

        // _img.Write("F:/Photos/Test/lena_std_ModImgSafeBef.tif");
        // clone.Write("F:/Photos/Test/lena_std_ModImgSafeAft.tif");
        return clone.Signature;
    }

    [Benchmark]
    public string ModImgUnsafe() {
        var (w, h, ch) = (_img.Width, _img.Height, (int)_img.ChannelCount);
        using var clone = _img.CloneArea(w, h);

        using var pixels = clone.GetPixelsUnsafe();
        var span = pixels.GetValues().AsSpan(); // 与GetArea(0, 0, w, h)等效
        for (var i = 0; i < span.Length; i += ch) ModPx(span[i..(i + 3)]);
        pixels.SetPixels(span);

        // _img.Write("F:/Photos/Test/lena_std_ModImgUnsafeBef.tif");
        // clone.Write("F:/Photos/Test/lena_std_ModImgUnsafeAft.tif");
        return clone.Signature;
    }

    [Benchmark]
    public unsafe string ModImgRawPtr() {
        var (w, h, ch) = (_img.Width, _img.Height, (int)_img.ChannelCount);
        using var clone = _img.CloneArea(w, h); // Clone仍修改原图

        using var pixels = clone.GetPixelsUnsafe();
        var ptr = (ushort*)pixels.GetAreaPointer(0, 0, w, h);
        for (var i = 0; i < w * h * ch; i += ch) { // 无法Span
            ptr[i] = (ushort)Math.Round(Math.Sqrt(ptr[i] / Max) * Max);
            ptr[i + 1] = (ushort)Math.Round(Math.Sqrt(ptr[i + 1] / Max) * Max);
            ptr[i + 2] = (ushort)Math.Round(Math.Sqrt(ptr[i + 2] / Max) * Max);
        }

        // _img.Write("F:/Photos/Test/lena_std_ModImgRawPtrBef.tif");
        // clone.Write("F:/Photos/Test/lena_std_ModImgRawPtrAft.tif");
        return clone.Signature;
    }

    [Benchmark]
    public unsafe string ModImgSpanPtr() {
        var (w, h, ch) = (_img.Width, _img.Height, (int)_img.ChannelCount);
        using var clone = _img.CloneArea(w, h); // Clone仍修改原图

        using var pixels = clone.GetPixelsUnsafe();
        Span<ushort> span = new((ushort*)pixels.GetAreaPointer(0, 0, w, h), (int)(w * h * ch));
        for (var i = 0; i < span.Length; i += ch) ModPx(span[i..(i + 3)]);

        // _img.Write("F:/Photos/Test/lena_std_ModImgSpanPtrBef.tif");
        // clone.Write("F:/Photos/Test/lena_std_ModImgSpanPtrAft.tif");
        return clone.Signature;
    }

    private static void ModPx(Span<ushort> rgb) {
        foreach (ref var v in rgb) v = (ushort)Math.Round(Math.Sqrt(v / Max) * Max); // demo
    }
}
