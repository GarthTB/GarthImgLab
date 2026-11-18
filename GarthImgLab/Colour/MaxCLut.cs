namespace GarthImgLab.Colour;

sealed class MaxCLut(f32[] lut, f64 maxL) {
    const int Nl = 128, Nh = 360, Scan = 16, Bisect = 24;

    public f64 this[f64 l, f64 h] {
        get {
            var x = l / maxL * (Nl - 1);
            if (x is <= 0 or >= Nl - 1) return 0;

            var y = h * (Nh / 360d);
            var i = (int)x;
            var t = x - i;
            var j = (int)y;
            var u = y - j;
            j %= Nh; // 舍入至 360 折回 0
            var j1 = (j + 1) % Nh;

            var k = i * Nh;
            var v0 = lut[k + j] + (lut[k + j1] - lut[k + j]) * u;
            k += Nh;
            var v1 = lut[k + j] + (lut[k + j1] - lut[k + j]) * u;

            return v0 + (v1 - v0) * t;
        }
    }

    public static MaxCLut Create(f64 maxL, f64 maxC, Func<f64, f64, f64, Xyz> toXyz) {
        var lut = new f32[Nl * Nh];
        var lStep = maxL / (Nl - 1);
        for (var i = 1; i < Nl - 1; i++) { // 端点行恒 0
            var l = i * lStep;
            var row = i * Nh;
            for (var j = 0; j < Nh; j++)
                lut[row + j] = (f32)SolveC(l, j * (360d / Nh), maxC, toXyz);
        }
        return new(lut, maxL);
    }

    static f64 SolveC(f64 l, f64 h, f64 maxC, Func<f64, f64, f64, Xyz> toXyz) {
        var lo = 0d;
        var step = maxC / Scan;

        for (var k = 1; k <= Scan; k++) {
            var hi = k * step;
            if (SRgb.IsValid(toXyz(l, hi, h))) {
                lo = hi;
                continue;
            }

            for (var n = 0; n < Bisect; n++) {
                var mid = .5 * (lo + hi);
                if (SRgb.IsValid(toXyz(l, mid, h)))
                    lo = mid;
                else
                    hi = mid;
            }
            return lo; // 可能略低估
        }

        return maxC;
    }
}
