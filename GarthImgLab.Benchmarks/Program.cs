using BenchmarkDotNet.Running;
using GarthImgLab.Benchmarks;

BenchmarkRunner.Run<ApplyPerPixel>();
BenchmarkRunner.Run<PixelParallelTuning>();
BenchmarkRunner.Run<RoundCorner>();
