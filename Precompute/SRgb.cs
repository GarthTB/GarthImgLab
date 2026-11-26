#!/usr/bin/env dotnet

/* 结果
L = 0.0031306684425006340328412384
S = 0.0404482362771081917043088001
*/

const decimal slope = 12.92m, gain = 1.055m, off = .055m;

var t = 1m; // t = L^(1/12)
for (var i = 0; i < 32; i++) {
    var t2 = t * t;
    var t4 = t2 * t2;
    var t7 = t4 * t2 * t;
    t -= (t4 * t * (slope * t7 - gain) + off) / (t4 * (12 * slope * t7 - 5 * gain));
}

var q = t * t * t * t;
var l = q * q * q;
var s = slope * l;
Console.WriteLine($"L = {l:R}");
Console.WriteLine($"S = {s:R}");
