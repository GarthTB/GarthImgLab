namespace GarthImgLab.Tools;

using Models.ColorConverters;

/* 14-bit
CIELCh MaxC=133.80841634911246  RGB=(0,0,16383)
OkLCh  MaxC=0.32249098837223966 RGB=(16383,0,16383)
OkLrCh MaxC=0.32249098837223966 RGB=(16383,0,16383)
JzCzhz MaxC=0.190289699629025   RGB=(0,0,16383) */

public static class MaxSatFinder {
    private const int N = 1 << 14;
    private const double MaxD = N - 1;
    private const ushort MaxU = N - 1;

    public static void Run() {
        var v = new (double C, ushort R, ushort G, ushort B)[4];

        object o = new();
        Parallel.For(
            0,
            N,
            static () => new (double C, ushort R, ushort G, ushort B)[4],
            static (i, _, local) => {
                var ri = i / MaxD;
                var si = (ushort)i;
                for (var j = 0; j < N; j++) {
                    var rj = j / MaxD;
                    var sj = (ushort)j;

                    Update(0, ri, rj, 0, si, sj, local);
                    Update(1, ri, rj, MaxU, si, sj, local);
                    Update(ri, 0, rj, si, 0, sj, local);
                    Update(ri, 1, rj, si, MaxU, sj, local);
                    Update(ri, rj, 0, si, sj, 0, local);
                    Update(ri, rj, 1, si, sj, MaxU, local);
                }
                return local;
            },
            local => {
                lock (o)
                    for (var k = 0; k < 4; k++)
                        if (local[k].C > v[k].C)
                            v[k] = local[k];
            });

        Console.WriteLine($"CIELCh  MaxC={v[0].C}  RGB=({v[0].R},{v[0].G},{v[0].B})");
        Console.WriteLine($"OkLCh   MaxC={v[1].C}  RGB=({v[1].R},{v[1].G},{v[1].B})");
        Console.WriteLine($"OkLrCh  MaxC={v[2].C}  RGB=({v[2].R},{v[2].G},{v[2].B})");
        Console.WriteLine($"JzCzhz  MaxC={v[3].C}  RGB=({v[3].R},{v[3].G},{v[3].B})");
    }

    private static void Update(
        double rd,
        double gd,
        double bd,
        ushort rs,
        ushort gs,
        ushort bs,
        (double C, ushort R, ushort G, ushort B)[] localV) {
        var c0 = CieLCh.FromSRgb(rd, gd, bd).C;
        if (c0 > localV[0].C) localV[0] = (c0, rs, gs, bs);

        var c1 = OkLCh.FromSRgb(rd, gd, bd).C;
        if (c1 > localV[1].C) localV[1] = (c1, rs, gs, bs);

        var c2 = OkLrCh.FromSRgb(rd, gd, bd).C;
        if (c2 > localV[2].C) localV[2] = (c2, rs, gs, bs);

        var c3 = JzCzhz.FromSRgb(rd, gd, bd).C;
        if (c3 > localV[3].C) localV[3] = (c3, rs, gs, bs);
    }
}
