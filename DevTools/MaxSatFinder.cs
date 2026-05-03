namespace DevTools;

using System.Collections.Concurrent;
using Wacton.Unicolour;

/* 12bit结果
Hsb    (0,4080,0)       1
Hsl    (0,4080,0)       1
Hsi    (0,4080,0)       1
Lchab  (0,0,4095)       133.80761432012983
Lchuv  (4095,0,0)       179.04142708939614
Hsluv  (4095,4095,0)    100.05922027251566
Hpluv  (4095,4095,0)    1784.328864093446
Tsl    (0,4080,0)       1
Jzczhz (0,0,4095)       0.19027906590136512
Oklch  (4095,0,4095)    0.32249096477516476
Okhsv  (0,631,3101)     1.0119794128679827
Okhsl  (4095,3861,2768) 1.0146240005026508
Oklrch (4095,0,4095)    0.32249096477516476
Hct    (4095,0,0)       113.35620829574427 */

internal static class MaxSatFinder {
    private const int Max = (1 << 12) - 1;
    private const double InvMax = 1d / Max;

    public static void Run() {
        var global = Enumerable.Repeat((.0, .0, .0, S: double.NegativeInfinity), 14).ToArray();

        Parallel.ForEach(
            Partitioner.Create(0, Max + 1),
            static () => Enumerable.Repeat((.0, .0, .0, S: double.NegativeInfinity), 14).ToArray(),
            static (range, _, local) => {
                for (var (i, end) = range; i < end; i++)
                for (var j = 0; j <= Max; j++) {
                    Calc(0, i, j);
                    Calc(i, 0, j);
                    Calc(i, j, 0);
                    Calc(Max, i, j);
                    Calc(i, Max, j);
                    Calc(i, j, Max);
                }
                return local;

                void Calc(double r, double g, double b) {
                    Unicolour uc = new(ColourSpace.Rgb, r * InvMax, g * InvMax, b * InvMax);
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

                    void Update(int i, double s) {
                        if (s > local[i].S) local[i] = (r, g, b, s);
                    }
                }
            },
            local => {
                lock (global)
                    for (var i = 0; i < 14; i++)
                        if (local[i].S > global[i].S)
                            global[i] = local[i];
            });

        foreach (var (r, g, b, s) in global) Console.WriteLine($"({r},{g},{b})\t{s}");
    }
}
