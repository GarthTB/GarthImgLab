namespace GarthImgLab.Benchmarks;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ImageMagick;

[MemoryDiagnoser, SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
public class ApplyPerPixel: IDisposable {
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
        GetValueSetPixel();
        var expected = _testImg!.Signature;

        _testImg.Dispose();
        _testImg = _oriImg!.CloneArea(_w, _h);
        ParGetValueSetPixel();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetValuesSetPixels();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ParGetValuesSetPixels();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        GetAreaSetPixels();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ParGetAreaSetPixels();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ToArraySetPixels();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ParToArraySetPixels();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        AreaPointerUnsafe();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        ParAreaPointerUnsafe();
        if (_testImg.Signature != expected) return false;

        _testImg.Dispose();
        _testImg = _oriImg.CloneArea(_w, _h);
        return true;
    }

    [Benchmark]
    public void GetValueSetPixel() {
        using var px = _testImg!.GetPixels();
        for (var y = 0; y < _h; y++)
        for (var x = 0; x < _w; x++) {
            var v = px.GetValue(x, y)!;
            MockMap(ref v[0], ref v[1], ref v[2]);
            px.SetPixel(x, y, v);
        }
    }

    [Benchmark]
    public void ParGetValueSetPixel() {
        using var px = _testImg!.GetPixels();
        Parallel.ForEach(
            Partitioner.Create(0, (int)_h),
            range => {
                for (var (y, to) = range; y < to; y++)
                for (var x = 0; x < _w; x++) {
                    var v = px.GetValue(x, y)!;
                    MockMap(ref v[0], ref v[1], ref v[2]);
                    px.SetPixel(x, y, v);
                }
            });
    }

    [Benchmark(Baseline = true)]
    public void GetValuesSetPixels() {
        using var px = _testImg!.GetPixels();
        var v = px.GetValues()!;
        for (var i = 0; i < v.Length; i += _ch) MockMap(ref v[i], ref v[i + 1], ref v[i + 2]);
        px.SetPixels(v);
    }

    [Benchmark]
    public void ParGetValuesSetPixels() {
        using var px = _testImg!.GetPixels();
        var v = px.GetValues()!;
        Parallel.ForEach(
            Partitioner.Create(0, (int)(_w * _h)),
            range => {
                for (var (i, to) = range; i < to; i++) {
                    var j = i * _ch;
                    MockMap(ref v[j], ref v[j + 1], ref v[j + 2]);
                }
            });
        px.SetPixels(v);
    }

    [Benchmark]
    public void GetAreaSetPixels() {
        using var px = _testImg!.GetPixels();
        var v = px.GetArea(0, 0, _w, _h)!;
        for (var i = 0; i < v.Length; i += _ch) MockMap(ref v[i], ref v[i + 1], ref v[i + 2]);
        px.SetPixels(v);
    }

    [Benchmark]
    public void ParGetAreaSetPixels() {
        using var px = _testImg!.GetPixels();
        var v = px.GetArea(0, 0, _w, _h)!;
        Parallel.ForEach(
            Partitioner.Create(0, (int)(_w * _h)),
            range => {
                for (var (i, to) = range; i < to; i++) {
                    var j = i * _ch;
                    MockMap(ref v[j], ref v[j + 1], ref v[j + 2]);
                }
            });
        px.SetPixels(v);
    }

    [Benchmark]
    public void ToArraySetPixels() {
        using var px = _testImg!.GetPixels();
        var v = px.ToArray()!;
        for (var i = 0; i < v.Length; i += _ch) MockMap(ref v[i], ref v[i + 1], ref v[i + 2]);
        px.SetPixels(v);
    }

    [Benchmark]
    public void ParToArraySetPixels() {
        using var px = _testImg!.GetPixels();
        var v = px.ToArray()!;
        Parallel.ForEach(
            Partitioner.Create(0, (int)(_w * _h)),
            range => {
                for (var (i, to) = range; i < to; i++) {
                    var j = i * _ch;
                    MockMap(ref v[j], ref v[j + 1], ref v[j + 2]);
                }
            });
        px.SetPixels(v);
    }

    [Benchmark]
    public unsafe void AreaPointerUnsafe() {
        using var px = _testImg!.GetPixelsUnsafe();
        var ptr = (ushort*)px.GetAreaPointer(0, 0, _w, _h);
        var lim = ptr + _w * _h * _ch;
        for (var p = ptr; p < lim; p += _ch) MockMap(ref p[0], ref p[1], ref p[2]);
    }

    [Benchmark]
    public unsafe void ParAreaPointerUnsafe() {
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

    private static void MockMap(ref ushort r, ref ushort g, ref ushort b) {
        double x = r / 65535d, y = g / 65535d, z = b / 65535d;
        r = (ushort)Math.Round(Math.Pow(Math.Clamp(y + z - x, 0, 1), 1.5) * 65535);
        g = (ushort)Math.Round(Math.Pow(Math.Clamp(x + z - y, 0, 1), 1.5) * 65535);
        b = (ushort)Math.Round(Math.Pow(Math.Clamp(x + y - z, 0, 1), 1.5) * 65535);
    }
}
