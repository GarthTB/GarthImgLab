#!/usr/bin/env dotnet

/* 结果
X  = 71740714395483087745/75479154764200873016
Y  = 1
Z  = 10272984088266781770/9434894345525109127
u' = 57392571516386470196/290095930795379789093
v' = 679312392877807857144/1450479653976898945465
*/

using System.Globalization;
using static System.Console;
using BigInt = System.Numerics.BigInteger;

const string d65 = "Data/CIE_std_illum_D65.csv";
const string obs = "Data/CIE_xyz_1931_2deg.csv";
const int lo = 360, hi = 830;

Dictionary<int, decimal> sMap = new(531);
foreach (var cols in File.ReadLines(d65).Select(static line => line.Split(',')))
    sMap[int.Parse(cols[0])] = Dec(cols[1]);

var sum = new decimal[3];
foreach (var cols in File.ReadLines(obs).Select(static line => line.Split(','))) {
    var wl = int.Parse(cols[0]);
    if (wl is < lo or > hi) continue;
    var s = sMap[wl];
    for (var i = 0; i < 3; i++) sum[i] += s * Dec(cols[i + 1]);
}

var (ma, ea) = Parts(sum[0]);
var (mb, eb) = Parts(sum[1]);
var (mc, ec) = Parts(sum[2]);
var e = Math.Max(ea, Math.Max(eb, ec));
var a = ma * BigInt.Pow(10, e - ea);
var b = mb * BigInt.Pow(10, e - eb);
var c = mc * BigInt.Pow(10, e - ec);
var d = a + 15 * b + 3 * c;

WriteLine($"X  = {Frac(a, b)}");
WriteLine("Y  = 1");
WriteLine($"Z  = {Frac(c, b)}");
WriteLine($"u' = {Frac(4 * a, d)}");
WriteLine($"v' = {Frac(9 * b, d)}");

static decimal Dec(string s) => decimal.Parse(s, CultureInfo.InvariantCulture);

static (BigInt m, int e) Parts(decimal v) {
    var b = decimal.GetBits(v);
    var m = (uint)b[0] + ((BigInt)(uint)b[1] << 32) + ((BigInt)(uint)b[2] << 64);
    return (m, (b[3] >> 16) & 0xFF);
}

static string Frac(BigInt n, BigInt d) {
    var g = BigInt.GreatestCommonDivisor(n, d);
    n /= g;
    d /= g;
    if (d.IsOne) return $"{n}";
    return $"{n}/{d}";
}
