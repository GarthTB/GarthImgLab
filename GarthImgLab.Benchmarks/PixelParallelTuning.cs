// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace GarthImgLab.Benchmarks;

using System.Diagnostics.CodeAnalysis;
using ImageMagick;
using static System.Collections.Concurrent.Partitioner;
using static Helper;
using static Parallel;

[MemoryDiagnoser, SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
public class PixelParallelTuning {
    private int _ch;
    private MagickImage _oriImg = new();
    private IMagickImage<ushort> _testImg = new MagickImage();
    private uint _w, _h;
    [Params("RGB8.tif", "RGB16.tif")] public string FileName { get; set; } = "RGB8.tif";

    [GlobalSetup]
    public void GlobalSetup() {
        _oriImg = new(Path.Combine(AppContext.BaseDirectory, FileName));
        _w = _oriImg.Width;
        _h = _oriImg.Height;
        _ch = (int)_oriImg.ChannelCount;
        _testImg = _oriImg.CloneArea(_w, _h);
        if (!Test()) throw new InvalidOperationException("结果不一致");
    }

    [GlobalCleanup]
    public void Cleanup() {
        _oriImg.Dispose();
        _testImg.Dispose();
    }

    private bool Test() {
        Sequential();
        var expected = _testImg.Signature;
        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);

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
        using var px = _testImg.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        var lim = ptr + _w * _h * _ch;
        for (var p = ptr; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
    }

    [Benchmark]
    public unsafe void ForPixel() {
        using var px = _testImg.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        For(
            0,
            (int)(_w * _h),
            i => {
                var p = ptr + i * _ch;
                MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }

    [Benchmark]
    public unsafe void ForEachPixelPartition() {
        using var px = _testImg.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        ForEach(
            Create(0, (int)(_w * _h)),
            range => {
                var (i, to) = range;
                var p = ptr + i * _ch;
                var lim = ptr + to * _ch;
                for (; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }

    [Benchmark]
    public unsafe void ForLine() {
        using var px = _testImg.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        For(
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
        using var px = _testImg.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        ForEach(
            Create(0, (int)_h),
            range => {
                var (y, to) = range;
                var p = ptr + y * _w * _ch;
                var lim = ptr + to * _w * _ch;
                for (; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
            });
    }
}
