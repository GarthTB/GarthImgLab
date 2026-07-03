// ReSharper disable ClassCanBeSealed.Global

namespace GarthImgLab.Benchmarks;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ImageMagick;
using OpEx = InvalidOperationException;

[MemoryDiagnoser]
public class ModifyByPixel: IDisposable {
    private int _ch;
    private MagickImage? _oriImg;
    private IMagickImage<ushort>? _testImg;
    private uint _w, _h;

    public void Dispose() {
        _oriImg?.Dispose();
        _testImg?.Dispose();
        GC.SuppressFinalize(this);
    }

    // ReSharper disable once UnusedMember.Global
    public void Test() {
        GlobalSetup();
        GetValueSetPixel();
        var expected = _testImg.Signature;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetValueSetPixelParallel();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetValuesSetPixels();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetValuesSetPixelsParallel();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetAreaSetPixels();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetAreaSetPixelsParallel();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ToArraySetPixels();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ToArraySetPixelsParallel();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        AreaPointerUnsafe();
        if (_testImg.Signature != expected) throw new OpEx("不一致");

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        AreaPointerUnsafeParallel();
        if (_testImg.Signature != expected) throw new OpEx("不一致");
    }

    [GlobalSetup, MemberNotNull(nameof(_oriImg), nameof(_testImg))]
    public void GlobalSetup() {
        _oriImg = new(Path.Combine(AppContext.BaseDirectory, "lena_std.tif"));
        _testImg = _oriImg.CloneArea(_w, _h);
        _w = _oriImg.Width;
        _h = _oriImg.Height;
        _ch = (int)_oriImg.ChannelCount;
    }

    [Benchmark]
    public void GetValueSetPixel() {
        using var pxs = _testImg!.GetPixels();
        for (var y = 0; y < _h; y++)
        for (var x = 0; x < _w; x++) {
            var v = pxs.GetValue(x, y) ?? throw new OpEx("无法获取像素");
            ModPx(ref v[0], ref v[1], ref v[2]);
            pxs.SetPixel(x, y, v);
        }
    }

    [Benchmark]
    public void GetValueSetPixelParallel() {
        using var pxs = _testImg!.GetPixels();
        Parallel.ForEach(
            Partitioner.Create(0, (int)_h),
            range => {
                for (var (y, end) = range; y < end; y++)
                for (var x = 0; x < _w; x++) {
                    var v = pxs.GetValue(x, y) ?? throw new OpEx("无法获取像素");
                    ModPx(ref v[0], ref v[1], ref v[2]);
                    pxs.SetPixel(x, y, v);
                }
            });
    }

    [Benchmark(Baseline = true)]
    public void GetValuesSetPixels() {
        using var pxs = _testImg!.GetPixels();
        var v = pxs.GetValues() ?? throw new OpEx("无法获取像素");
        for (var i = 0; i < v.Length; i += _ch) ModPx(ref v[i], ref v[i + 1], ref v[i + 2]);
        pxs.SetPixels(v);
    }

    [Benchmark]
    public void GetValuesSetPixelsParallel() {
        using var pxs = _testImg!.GetPixels();
        var v = pxs.GetValues() ?? throw new OpEx("无法获取像素");
        if (v.Length % _ch != 0) throw new OpEx("像素数据不完整");
        Parallel.ForEach(
            Partitioner.Create(0, v.Length / _ch),
            range => {
                for (var (px, end) = range; px < end; px++) {
                    var i = px * _ch;
                    ModPx(ref v[i], ref v[i + 1], ref v[i + 2]);
                }
            });
        pxs.SetPixels(v);
    }

    [Benchmark]
    public void GetAreaSetPixels() {
        using var pxs = _testImg!.GetPixels();
        var v = pxs.GetArea(0, 0, _w, _h) ?? throw new OpEx("无法获取像素");
        for (var i = 0; i < v.Length; i += _ch) ModPx(ref v[i], ref v[i + 1], ref v[i + 2]);
        pxs.SetPixels(v);
    }

    [Benchmark]
    public void GetAreaSetPixelsParallel() {
        using var pxs = _testImg!.GetPixels();
        var v = pxs.GetArea(0, 0, _w, _h) ?? throw new OpEx("无法获取像素");
        if (v.Length % _ch != 0) throw new OpEx("像素数据不完整");
        Parallel.ForEach(
            Partitioner.Create(0, v.Length / _ch),
            range => {
                for (var (px, end) = range; px < end; px++) {
                    var i = px * _ch;
                    ModPx(ref v[i], ref v[i + 1], ref v[i + 2]);
                }
            });
        pxs.SetPixels(v);
    }

    [Benchmark]
    public void ToArraySetPixels() {
        using var pxs = _testImg!.GetPixels();
        var v = pxs.ToArray() ?? throw new OpEx("无法获取像素");
        for (var i = 0; i < v.Length; i += _ch) ModPx(ref v[i], ref v[i + 1], ref v[i + 2]);
        pxs.SetPixels(v);
    }

    [Benchmark]
    public void ToArraySetPixelsParallel() {
        using var pxs = _testImg!.GetPixels();
        var v = pxs.ToArray() ?? throw new OpEx("无法获取像素");
        if (v.Length % _ch != 0) throw new OpEx("像素数据不完整");
        Parallel.ForEach(
            Partitioner.Create(0, v.Length / _ch),
            range => {
                for (var (px, end) = range; px < end; px++) {
                    var i = px * _ch;
                    ModPx(ref v[i], ref v[i + 1], ref v[i + 2]);
                }
            });
        pxs.SetPixels(v);
    }

    [Benchmark]
    public unsafe void AreaPointerUnsafe() {
        using var pxs = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)pxs.GetAreaPointer(0, 0, _w, _h);
        for (var i = 0; i < _w * _h * _ch; i += _ch)
            ModPx(ref ptr[i], ref ptr[i + 1], ref ptr[i + 2]);
    }

    [Benchmark]
    public unsafe void AreaPointerUnsafeParallel() {
        using var pxs = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)pxs.GetAreaPointer(0, 0, _w, _h);
        Parallel.ForEach(
            Partitioner.Create(0, (int)_h),
            range => {
                for (var (y, end) = range; y < end; y++)
                for (var x = 0; x < _w; x++) {
                    var i = (y * _w + x) * _ch;
                    ModPx(ref ptr[i], ref ptr[i + 1], ref ptr[i + 2]);
                }
            });
    }

    private static void ModPx(ref ushort r, ref ushort g, ref ushort b) =>
        (r, g, b) = ((ushort)(g + b - r), (ushort)(r + b - g), (ushort)(r + g - b));
}
