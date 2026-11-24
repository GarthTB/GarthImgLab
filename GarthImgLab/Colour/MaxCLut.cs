namespace GarthImgLab.Colour;

sealed class MaxCLut {
    const int Nl = 257, Nh = 1024, Bisect = 16;
    readonly f32[] _lut = new f32[Nl * Nh];
    readonly f64 _maxL;

    public MaxCLut(f64 maxL, f64 maxC, Func<Tri, Tri> toXyz) {
        _maxL = maxL;
        var lStep = _maxL / (Nl - 1);
        Parallel.For(
            1, // 黑点与白点色度为 0
            Nl - 1,
            i => {
                var l = i * lStep;
                var row = i * Nh;
                for (var j = 0; j < Nh; j++) _lut[row + j] = (f32)SolveC(l, j * (360d / Nh));
            });

        f64 SolveC(f64 l, f64 h) {
            f64 lo = 0, hi = maxC;
            for (var n = 0; n < Bisect; n++) {
                var c = .5 * (lo + hi);
                if (SRgb.Contains(toXyz(new(l, c, h))))
                    lo = c;
                else
                    hi = c;
            }
            return lo; // 略低估
        }
    }

    public f64 this[f64 l, f64 h] {
        get {
            if (l <= 0 || l >= _maxL) return 0;

            var x = l / _maxL * (Nl - 1);
            var i = (int)x;
            var t = x - i;

            var y = h * (Nh / 360d);
            var j = (int)y; // 360 出现
            var u = y - j;

            j %= Nh; // 360 -> 0
            var j1 = (j + 1) % Nh;

            var k = i * Nh;
            var v0 = _lut[k + j] + (_lut[k + j1] - _lut[k + j]) * u;
            k += Nh;
            var v1 = _lut[k + j] + (_lut[k + j1] - _lut[k + j]) * u;

            return v0 + (v1 - v0) * t;
        }
    }
}
