namespace DevTools;

using CS = Wacton.Unicolour.ColourSpace;
using Uc = Wacton.Unicolour.Unicolour;

/* 12bit结果
Hsb    (4095,0,0)       1
Hsl    (4095,0,0)       1
Hsi    (4095,0,0)       1
Lchab  (0,0,4095)       133.80761432012983
Lchuv  (4095,0,0)       179.04142708939614
Hsluv  (4095,4095,0)    100.05922027251566
Hpluv  (4095,4095,0)    1784.328864093446
Tsl    (4095,0,0)       1
Jzczhz (0,0,4095)       0.19027906590136512
Oklch  (4095,0,4095)    0.32249096477516476
Okhsv  (0,631,3101)     1.0119794128679827
Okhsl  (4095,3861,2768) 1.0146240005026508
Oklrch (4095,0,4095)    0.32249096477516476
Hct    (4095,0,0)       113.35620829574427 */

internal static class MaxSatFinder {
    private const int MaxVal = (1 << 12) - 1;

    public static void Run() {
        var maxSatArr = GetRgb()
            .AsParallel()
            .Aggregate(
                static () => Enumerable.Repeat(((0d, 0d, 0d), S: double.MinValue), 14).ToArray(),
                static (maxS, rgb) => {
                    Uc uc = new(CS.Rgb, rgb.R / MaxVal, rgb.G / MaxVal, rgb.B / MaxVal);
                    Update(0, uc.Hsb.S);
                    Update(1, uc.Hsl.S);
                    Update(2, uc.Hsi.S);
                    Update(3, uc.Lchab.C);
                    Update(4, uc.Lchuv.C);
                    Update(5, uc.Hsluv.S);
                    Update(6, uc.Hpluv.S);
                    Update(7, uc.Tsl.S);
                    Update(8, uc.Jzczhz.C);
                    Update(9, uc.Oklch.C);
                    Update(10, uc.Okhsv.S);
                    Update(11, uc.Okhsl.S);
                    Update(12, uc.Oklrch.C);
                    Update(13, uc.Hct.C);
                    return maxS;

                    void Update(int i, double s) {
                        if (s > maxS[i].S) maxS[i] = (rgb, s);
                    }
                },
                static (globalMaxS, localMaxS) => {
                    for (var i = 0; i < 14; i++)
                        if (localMaxS[i].S > globalMaxS[i].S)
                            globalMaxS[i] = localMaxS[i];
                    return globalMaxS;
                },
                static x => x);
        foreach (var ((r, g, b), s) in maxSatArr) Console.WriteLine($"({r},{g},{b})\t{s}");
    }

    private static IEnumerable<(double R, double G, double B)> GetRgb() {
        for (var g = 0; g <= MaxVal; g++)
        for (var b = 0; b <= MaxVal; b++) {
            yield return (0, g, b);
            yield return (MaxVal, g, b);
        }
        for (var r = 1; r < MaxVal; r++)
        for (var b = 0; b <= MaxVal; b++) {
            yield return (r, 0, b);
            yield return (r, MaxVal, b);
        }
        for (var r = 1; r < MaxVal; r++)
        for (var g = 1; g < MaxVal; g++) {
            yield return (r, g, 0);
            yield return (r, g, MaxVal);
        }
    }
}
