namespace GarthImgLab.Benchmarks;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ImageMagick;

[MemoryDiagnoser, SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
public class PixelParallelTuning: IDisposable {
    private int _ch;
    private MagickImage? _oriImg;
    private IMagickImage<ushort>? _testImg;
    private uint _w, _h;

    public void Dispose() {
        _oriImg?.Dispose();
        _testImg?.Dispose();
        GC.SuppressFinalize(this);
    }

    [GlobalSetup]
    public void GlobalSetup() {
        _oriImg = new(Path.Combine(AppContext.BaseDirectory, "lena_std.tif"));
        _w = _oriImg.Width;
        _h = _oriImg.Height;
        _ch = (int)_oriImg.ChannelCount;
        _testImg = _oriImg.CloneArea(_w, _h);
        if (!Test()) throw new InvalidOperationException("结果不一致");
    }

    private bool Test() {
        Sequential();
        var expected = _testImg!.Signature;

        _testImg.Dispose();
        _testImg = _oriImg!.CloneArea(_w, _h);
        ForPixel();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ForEachPixelPartition();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ForLine();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ForEachLinePartition();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        return true;
    }

    [Benchmark(Baseline = true)]
    public unsafe void Sequential() {
        using var px = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        var lim = ptr + _w * _h * _ch;
        for (var p = ptr; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
    }

    [Benchmark]
    public unsafe void ForPixel() {
        using var px = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        Parallel.For(
            0,
            (int)(_w * _h),
            i => {
                var p = ptr + i * _ch;
                MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }

    [Benchmark]
    public unsafe void ForEachPixelPartition() {
        using var px = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        Parallel.ForEach(
            Partitioner.Create(0, (int)(_w * _h)),
            range => {
                var (i, to) = range;
                var p = ptr + i * _ch;
                var lim = ptr + to * _ch;
                for (; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }

    [Benchmark]
    public unsafe void ForLine() {
        using var px = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        Parallel.For(
            0,
            (int)_h,
            y => {
                var p = ptr + y * _w * _ch;
                var lim = ptr + (y + 1) * _w * _ch;
                for (; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }

    [Benchmark]
    public unsafe void ForEachLinePartition() {
        using var px = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        Parallel.ForEach(
            Partitioner.Create(0, (int)_h),
            range => {
                var (y, to) = range;
                var p = ptr + y * _w * _ch;
                var lim = ptr + to * _w * _ch;
                for (; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }

    private static void MockMap(ref ushort r, ref ushort g, ref ushort b) {
        double x = r / 65535d, y = g / 65535d, z = b / 65535d;
        r = (ushort)Math.Round(Math.Pow(Math.Clamp(y + z - x, 0, 1), 1.5) * 65535);
        g = (ushort)Math.Round(Math.Pow(Math.Clamp(x + z - y, 0, 1), 1.5) * 65535);
        b = (ushort)Math.Round(Math.Pow(Math.Clamp(x + y - z, 0, 1), 1.5) * 65535);
    }
}
