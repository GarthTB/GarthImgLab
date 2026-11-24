#!/usr/bin/env dotnet
#:project ../GarthImgLab/GarthImgLab.csproj

/* 结果
CIELCh(ab)  RGB=(   0,   0,1024)  C=133.80758240174484
CIELCh(uv)  RGB=(1024,   0,   0)  C=179.0415038066068
JzCzhz      RGB=(   0,   0,1024)  C=0.1901807500611325
OkLCh       RGB=(1024,   0,1024)  C=0.32249292672617635
OkLCh+      RGB=(1024,   0,1024)  C=0.488504016253527
sUCS        RGB=(1024,   0,   0)  C=55.66852557351276
*/

using GarthImgLab.Colour;
using MaxTup = (double C, int R, int G, int B);

const int n = 1024, shell = 87, cut = n - shell;

var v = new double[n + 1];
for (var i = 0; i <= n; i++) v[i] = (double)i / n;

var global = new MaxTup[5];
object obj = new();
Parallel.For(
    0,
    n + 1,
    static () => new MaxTup[5],
    (r, _, local) => {
        var vr = v[r];
        var fr = r >= cut;
        for (var g = 0; g <= n; g++) {
            var vg = v[g];
            var fg = fr || g >= cut;
            for (var b = 0; b <= n; b++) {
                var xyz = SRgb.ToXyz(new(vr, vg, v[b]));

                Take(ref local[2], JzCzhz.FromXyz(xyz).C2, r, g, b);
                if (!fg && b < cut) continue;

                Take(ref local[0], CieLChab.FromXyz(xyz).C2, r, g, b);
                Take(ref local[1], CieLChuv.FromXyz(xyz).C2, r, g, b);
                Take(ref local[3], OkLCh.FromXyz(xyz).C2, r, g, b);
                Take(ref local[4], SUcs.FromXyz(xyz).C2, r, g, b);
            }
        }
        return local;
    },
    local => {
        lock (obj)
            for (var i = 0; i < 5; i++)
                if (local[i].C > global[i].C)
                    global[i] = local[i];
    });

var okTup = global[3];
var okXyz = SRgb.ToXyz(new(v[okTup.R], v[okTup.G], v[okTup.B]));
var okLChPTup = okTup with { C = OkLChP.FromXyz(okXyz).C2 };

Print("CIELCh(ab)", global[0]);
Print("CIELCh(uv)", global[1]);
Print("JzCzhz", global[2]);
Print("OkLCh", okTup);
Print("OkLCh+", okLChPTup);
Print("sUCS", global[4]);

static void Take(ref MaxTup tup, double c, int r, int g, int b) {
    if (c > tup.C) tup = (c, r, g, b);
}

static void Print(string name, MaxTup tup) =>
    Console.WriteLine($"{name,-10}  RGB=({tup.R,4},{tup.G,4},{tup.B,4})  C={tup.C:R}");
