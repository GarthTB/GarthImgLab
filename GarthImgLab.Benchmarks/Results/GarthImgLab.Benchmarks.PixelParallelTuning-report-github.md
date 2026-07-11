```
BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
Memory: 15.69 GB Total, 5.39 GB Available
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
```

| Method                | FileName      |          Mean |        Error |       StdDev |        Median |    Ratio |  RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------|---------------|--------------:|-------------:|-------------:|--------------:|---------:|---------:|-------:|----------:|------------:|
| **Sequential**        | **RGB16.tif** | **222.79 μs** | **2.880 μs** | **2.694 μs** | **223.22 μs** | **1.00** | **0.00** |  **-** |  **64 B** |    **1.00** |
| ForPixel              | RGB16.tif     |     164.55 μs |     3.279 μs |     8.696 μs |     165.31 μs |     0.74 |     0.04 | 0.4883 |    4861 B |       75.95 |
| ForEachPixelPartition | RGB16.tif     |      82.27 μs |     1.608 μs |     1.652 μs |      81.61 μs |     0.37 |     0.01 | 0.7324 |    7143 B |      111.61 |
| ForLine               | RGB16.tif     |      81.63 μs |     1.404 μs |     1.172 μs |      81.39 μs |     0.37 |     0.01 | 0.3662 |    3807 B |       59.48 |
| ForEachLinePartition  | RGB16.tif     |      82.29 μs |     1.280 μs |     1.069 μs |      82.59 μs |     0.37 |     0.01 | 0.7324 |    7528 B |      117.62 |
|                       |               |               |              |              |               |          |          |        |           |             |
| **Sequential**        | **RGB8.tif**  | **228.35 μs** | **1.631 μs** | **1.526 μs** | **228.56 μs** | **1.00** | **0.00** |  **-** |  **64 B** |    **1.00** |
| ForPixel              | RGB8.tif      |     161.91 μs |     3.821 μs |    11.086 μs |     165.96 μs |     0.71 |     0.05 | 0.4883 |    4857 B |       75.89 |
| ForEachPixelPartition | RGB8.tif      |      79.97 μs |     0.743 μs |     0.695 μs |      79.95 μs |     0.35 |     0.00 | 0.7324 |    7402 B |      115.66 |
| ForLine               | RGB8.tif      |      83.51 μs |     1.125 μs |     0.997 μs |      83.28 μs |     0.37 |     0.00 | 0.3662 |    3825 B |       59.77 |
| ForEachLinePartition  | RGB8.tif      |      79.16 μs |     1.133 μs |     0.946 μs |      78.93 μs |     0.35 |     0.00 | 0.7324 |    7258 B |      113.41 |
