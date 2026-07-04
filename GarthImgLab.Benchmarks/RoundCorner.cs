// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace GarthImgLab.Benchmarks;

using System.Diagnostics.CodeAnalysis;
using ImageMagick;
using ImageMagick.Drawing;

[MemoryDiagnoser, SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
public class RoundCorner: IDisposable {
    private MagickImage? _oriImg;
    private IMagickImage<ushort>? _testImg;
    private uint _w, _h;
    [Params("#0F0F", "#0F08", "#0F00")] public string Hex = "#0F08";
    private MagickColor Color => new(Hex);
    [Params(0, .2, .4)] public double Radius { get => field * Math.Min(_w, _h); set; } = .2;

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
        _testImg = _oriImg.CloneArea(_w, _h);
    }

    public void Test() {
        GlobalSetup();
        string[] hexes = ["#0F0F", "#0F08", "#0F00"];
        foreach (var hex in hexes) {
            Hex = hex;

            WriteMaskComposite();
            _testImg!.Write($"WriteMaskComposite_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg!.CloneArea(_w, _h);

            WriteMaskDraw();
            _testImg.Write($"WriteMaskDraw_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            WriteMaskCopyAlpha();
            _testImg.Write($"WriteMaskCopyAlpha_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            WriteMaskEvaluate();
            _testImg.Write($"WriteMaskEvaluate_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            WriteMaskTexture1();
            _testImg.Write($"WriteMaskTexture1_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            WriteMaskTexture64();
            _testImg.Write($"WriteMaskTexture64_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            WriteMaskTextureFull();
            _testImg.Write($"WriteMaskTextureFull_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            CompositeCorners0();
            _testImg.Write($"CompositeCorners0_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            CompositeCorners1();
            _testImg.Write($"CompositeCorners1_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            CompositeCorners2();
            _testImg.Write($"CompositeCorners2_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);

            WriteMaskColorize();
            _testImg.Write($"WriteMaskColorize_{hex}.tif");
            _testImg.Dispose();
            _testImg = _oriImg.CloneArea(_w, _h);
        }
    }

    [Benchmark(Baseline = true)]
    public void WriteMaskComposite() {
        var r = Radius;
        using MagickImage mask = new(MagickColors.Black, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        _testImg!.SetWriteMask(mask);
        try {
            using MagickImage tmp = new(Color, _w, _h);
            _testImg.Composite(tmp, CompositeOperator.Over);
        } finally { _testImg.RemoveWriteMask(); }
    }

    // 有黑边，效果错误
    public void WriteMaskDraw() {
        var r = Radius;
        using MagickImage mask = new(MagickColors.Black, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        _testImg!.SetWriteMask(mask);
        try { new Drawables().FillColor(Color).Rectangle(0, 0, _w, _h).Draw(_testImg); } finally {
            _testImg.RemoveWriteMask();
        }
    }

    // 有黑边，效果错误
    public void WriteMaskCopyAlpha() {
        var r = Radius;
        using var corners = new MagickImage(Color, _w, _h);
        corners.Alpha(AlphaOption.Set);
        using var mask = new MagickImage(MagickColors.White, _w, _h);
        new Drawables().FillColor(MagickColors.Black).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        corners.SetWriteMask(mask);
        using var a = new MagickImage(MagickColors.Transparent, _w, _h);
        corners.Composite(a, CompositeOperator.CopyAlpha);
        _testImg!.Composite(corners, CompositeOperator.Over);
    }

    // 过渡生硬
    public void WriteMaskEvaluate() {
        var r = Radius;
        using var corners = new MagickImage(Color, _w, _h);
        corners.Alpha(AlphaOption.Set);
        using var mask = new MagickImage(MagickColors.White, _w, _h);
        new Drawables().FillColor(MagickColors.Black).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        corners.SetWriteMask(mask);
        corners.Evaluate(Channels.Alpha, EvaluateOperator.Set, 0);
        _testImg!.Composite(corners, CompositeOperator.Over);
    }

    [Benchmark]
    public void WriteMaskTexture1() {
        var r = Radius;
        using MagickImage mask = new(MagickColors.Black, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        _testImg!.SetWriteMask(mask);
        try {
            using MagickImage tmp = new(Color, 1, 1);
            _testImg.Texture(tmp);
        } finally { _testImg.RemoveWriteMask(); }
    }

    [Benchmark]
    public void WriteMaskTexture64() {
        var r = Radius;
        using MagickImage mask = new(MagickColors.Black, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        _testImg!.SetWriteMask(mask);
        try {
            using MagickImage tmp = new(Color, 64, 64);
            _testImg.Texture(tmp);
        } finally { _testImg.RemoveWriteMask(); }
    }

    [Benchmark]
    public void WriteMaskTextureFull() {
        var r = Radius;
        using MagickImage mask = new(MagickColors.Black, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        _testImg!.SetWriteMask(mask);
        try {
            using MagickImage tmp = new(Color, _w, _h);
            _testImg.Texture(tmp);
        } finally { _testImg.RemoveWriteMask(); }
    }

    // 若A为0，RGB会被置0，效果错误
    public void CompositeCorners0() {
        var r = Radius;
        using MagickImage a = new(MagickColors.None, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(a);
        using MagickImage corners = new(Color, _w, _h);
        corners.Composite(a, CompositeOperator.DstOut);
        _testImg!.Composite(corners, CompositeOperator.Over);
    }

    [Benchmark]
    public void CompositeCorners1() {
        var c = Color;
        var r = Radius;
        using MagickImage a = new(new MagickColor { A = (ushort)~c.A }, _w, _h);
        new Drawables().FillColor(MagickColors.Black)
            .RoundRectangle(0, 0, _w, _h, r, r)
            .Draw(a); // 中间不透明，四角 Alpha 反相
        a.Negate(Channels.Alpha); // 中间透明，四角 Alpha 还原
        using MagickImage corners = new(c, _w, _h);
        corners.Composite(a, CompositeOperator.CopyAlpha);
        _testImg!.Composite(corners, CompositeOperator.Over);
    }

    [Benchmark]
    public void CompositeCorners2() {
        var c = Color;
        var r = Radius;
        var notA = (ushort)~c.A;
        using MagickImage corners = new(new MagickColor(c) { A = notA }, _w, _h);
        new Drawables().FillColor(new MagickColor(c) { A = 65535 })
            .RoundRectangle(0, 0, _w, _h, r, r)
            .Draw(corners); // 中间不透明，四角 Alpha 反相
        corners.Negate(Channels.Alpha); // 中间透明，四角 Alpha 还原
        if (notA == 0) corners.Colorize(c, new(100)); // 若A为0，RGB会被置0，需重新上色
        _testImg!.Composite(corners, CompositeOperator.Over);
    }

    // 有透明度时效果错误
    public void WriteMaskColorize() {
        var r = Radius;
        using MagickImage mask = new(MagickColors.Black, _w, _h);
        new Drawables().FillColor(MagickColors.White).RoundRectangle(0, 0, _w, _h, r, r).Draw(mask);
        _testImg!.SetWriteMask(mask);
        try { _testImg.Colorize(Color, new(100)); } finally { _testImg.RemoveWriteMask(); }
    }
}
