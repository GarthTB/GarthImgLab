```
BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
Memory: 15.69 GB Total, 5.13 GB Available
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
```

| Method                |       Mean |    Error |   StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
|-----------------------|-----------:|---------:|---------:|------:|--------:|----------:|------------:|
| Sequential            | 3,406.1 μs | 29.96 μs | 25.02 μs |  1.00 |    0.00 |      64 B |        1.00 |
| ForPixel              |   828.3 μs | 22.41 μs | 65.39 μs |  0.24 |    0.02 |    4870 B |       76.09 |
| ForEachPixelPartition |   909.4 μs |  6.65 μs |  5.56 μs |  0.27 |    0.00 |    7282 B |      113.78 |
| ForLine               |   957.9 μs |  5.67 μs |  5.03 μs |  0.28 |    0.00 |    4852 B |       75.81 |
| ForEachLinePartition  |   904.1 μs |  9.11 μs |  8.53 μs |  0.27 |    0.00 |    7521 B |      117.52 |
