#pragma warning disable CS8618
namespace Benchmarks.Colour;

using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using GarthImgLab.Colour;

[MemoryDiagnoser]
public class MaxCLutCtor {
    static readonly Tri W = SRgb.ToXyz(new(1, 1, 1)),
        R = SRgb.ToXyz(new(1, 0, 0)),
        B = SRgb.ToXyz(new(0, 0, 1)),
        M = SRgb.ToXyz(new(1, 0, 1));

    [Params(5, 6, 7, 8, 9)] public int Cost;

    [Params("CIELCh(ab)", "CIELCh(uv)", "JzCzhz", "OkLCh", "OkLCh+", "sUCS")]
    public string SpaceName;

    double _maxL, _maxC;
    int _nl, _nh, _bisect;
    Func<Tri, Tri> _toXyz;

    [GlobalSetup]
    public void Setup() {
        _nl = (1 << Cost) + 1;
        _nh = 1 << (Cost + 2);
        _bisect = 2 * Cost;
        (_maxL, _maxC, _toXyz) = SpaceName switch {
            "CIELCh(ab)" =>
                (CieLChab.FromXyz(W).C1, CieLChab.FromXyz(B).C2, (Func<Tri, Tri>)CieLChab.ToXyz),
            "CIELCh(uv)" => (CieLChuv.FromXyz(W).C1, CieLChuv.FromXyz(R).C2, CieLChuv.ToXyz),
            "JzCzhz" => (JzCzhz.FromXyz(W).C1, JzCzhz.FromXyz(B).C2, JzCzhz.ToXyz),
            "OkLCh" => (OkLCh.FromXyz(W).C1, OkLCh.FromXyz(M).C2, OkLCh.ToXyz),
            "OkLCh+" => (OkLChP.FromXyz(W).C1, OkLChP.FromXyz(M).C2, OkLChP.ToXyz),
            _ => (SUcs.FromXyz(W).C1, SUcs.FromXyz(R).C2, SUcs.ToXyz)
        };
    }

    [Benchmark(Baseline = true)]
    public float[] NoPara() {
        var lut = new float[_nl * _nh];
        var lStep = _maxL / (_nl - 1);
        for (var i = 1; i < _nl - 1; i++) { // 黑点与白点色度为 0
            var l = i * lStep;
            var row = i * _nh;
            for (var j = 0; j < _nh; j++) lut[row + j] = (float)SolveC(l, j * (360d / _nh));
        }
        return lut;
    }

    [Benchmark]
    public float[] ParaFor() {
        var lut = new float[_nl * _nh];
        var lStep = _maxL / (_nl - 1);
        Parallel.For(
            1, // 黑点与白点色度为 0
            _nl - 1,
            i => {
                var l = i * lStep;
                var row = i * _nh;
                for (var j = 0; j < _nh; j++) lut[row + j] = (float)SolveC(l, j * (360d / _nh));
            });
        return lut;
    }

    [Benchmark]
    public float[] Partition() {
        var lut = new float[_nl * _nh];
        var lStep = _maxL / (_nl - 1);
        Parallel.ForEach(
            Partitioner.Create(1, _nl - 1), // 黑点与白点色度为 0
            rng => {
                for (var (i, to) = rng; i < to; i++) {
                    var l = i * lStep;
                    var row = i * _nh;
                    for (var j = 0; j < _nh; j++) lut[row + j] = (float)SolveC(l, j * (360d / _nh));
                }
            });
        return lut;
    }

    double SolveC(double l, double h) {
        double lo = 0, hi = _maxC;
        for (var n = 0; n < _bisect; n++) {
            var c = .5 * (lo + hi);
            if (SRgb.Contains(_toXyz(new(l, c, h))))
                lo = c;
            else
                hi = c;
        }
        return lo; // 略低估
    }
}
