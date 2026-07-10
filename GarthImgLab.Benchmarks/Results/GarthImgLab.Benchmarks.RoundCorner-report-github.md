```
BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
Memory: 15.69 GB Total, 5.14 GB Available
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
```

| Method                 | Hex       | Radius  |          Mean |         Error |        StdDev |    Ratio |  RatioSD |    Allocated | Alloc Ratio |
|------------------------|-----------|---------|--------------:|--------------:|--------------:|---------:|---------:|-------------:|------------:|
| **WriteMaskComposite** | **#0F00** | **0**   | **13.059 ms** | **0.0488 ms** | **0.0433 ms** | **1.00** | **0.00** | **11.41 KB** |    **1.00** |
| WriteMaskTexture1      | #0F00     | 0       |  1,134.226 ms |     8.9006 ms |     7.8902 ms |    86.85 |     0.65 |     11.35 KB |        0.99 |
| WriteMaskTexture64     | #0F00     | 0       |     13.514 ms |     0.0743 ms |     0.0695 ms |     1.03 |     0.01 |     11.28 KB |        0.99 |
| WriteMaskTextureFull   | #0F00     | 0       |     12.959 ms |     0.0435 ms |     0.0386 ms |     0.99 |     0.00 |     11.35 KB |        0.99 |
| CompositeCorners1      | #0F00     | 0       |     18.814 ms |     0.0959 ms |     0.0749 ms |     1.44 |     0.01 |     11.48 KB |        1.01 |
| CompositeCorners2      | #0F00     | 0       |      8.046 ms |     0.0669 ms |     0.0593 ms |     0.62 |     0.00 |      6.04 KB |        0.53 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F00** | **0.2** | **35.684 ms** | **0.1933 ms** | **0.1808 ms** | **1.00** | **0.00** | **11.41 KB** |    **1.00** |
| WriteMaskTexture1      | #0F00     | 0.2     |  2,433.169 ms |    10.9918 ms |     9.7440 ms |    68.19 |     0.43 |     11.35 KB |        0.99 |
| WriteMaskTexture64     | #0F00     | 0.2     |     52.432 ms |     0.3662 ms |     0.3058 ms |     1.47 |     0.01 |     11.28 KB |        0.99 |
| WriteMaskTextureFull   | #0F00     | 0.2     |     35.924 ms |     0.1400 ms |     0.1310 ms |     1.01 |     0.01 |     11.35 KB |        0.99 |
| CompositeCorners1      | #0F00     | 0.2     |     39.451 ms |     0.2025 ms |     0.1894 ms |     1.11 |     0.01 |     11.48 KB |        1.01 |
| CompositeCorners2      | #0F00     | 0.2     |     29.019 ms |     0.2316 ms |     0.2166 ms |     0.81 |     0.01 |      6.04 KB |        0.53 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F00** | **0.4** | **46.168 ms** | **0.6442 ms** | **0.5711 ms** | **1.00** | **0.00** | **11.41 KB** |    **1.00** |
| WriteMaskTexture1      | #0F00     | 0.4     |  2,332.989 ms |    20.0941 ms |    16.7795 ms |    50.54 |     0.70 |     11.35 KB |        0.99 |
| WriteMaskTexture64     | #0F00     | 0.4     |     60.794 ms |     0.7887 ms |     0.7377 ms |     1.32 |     0.02 |     11.28 KB |        0.99 |
| WriteMaskTextureFull   | #0F00     | 0.4     |     44.919 ms |     0.1852 ms |     0.1642 ms |     0.97 |     0.01 |     11.35 KB |        0.99 |
| CompositeCorners1      | #0F00     | 0.4     |     48.343 ms |     0.3496 ms |     0.3270 ms |     1.05 |     0.01 |     11.48 KB |        1.01 |
| CompositeCorners2      | #0F00     | 0.4     |     37.874 ms |     0.2399 ms |     0.2127 ms |     0.82 |     0.01 |      6.04 KB |        0.53 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F08** | **0**   | **13.157 ms** | **0.0696 ms** | **0.0617 ms** | **1.00** | **0.00** | **11.41 KB** |    **1.00** |
| WriteMaskTexture1      | #0F08     | 0       |  1,142.030 ms |     6.9185 ms |     6.4716 ms |    86.80 |     0.62 |     11.35 KB |        0.99 |
| WriteMaskTexture64     | #0F08     | 0       |     13.911 ms |     0.2751 ms |     0.2702 ms |     1.06 |     0.02 |     11.28 KB |        0.99 |
| WriteMaskTextureFull   | #0F08     | 0       |     13.426 ms |     0.1073 ms |     0.1003 ms |     1.02 |     0.01 |     11.35 KB |        0.99 |
| CompositeCorners1      | #0F08     | 0       |     19.914 ms |     0.1242 ms |     0.1037 ms |     1.51 |     0.01 |     11.58 KB |        1.01 |
| CompositeCorners2      | #0F08     | 0       |      8.329 ms |     0.0829 ms |     0.0776 ms |     0.63 |     0.01 |      6.14 KB |        0.54 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F08** | **0.2** | **35.642 ms** | **0.1709 ms** | **0.1515 ms** | **1.00** | **0.00** | **11.41 KB** |    **1.00** |
| WriteMaskTexture1      | #0F08     | 0.2     |  2,445.420 ms |    14.5524 ms |    11.3616 ms |    68.61 |     0.42 |     11.35 KB |        0.99 |
| WriteMaskTexture64     | #0F08     | 0.2     |     52.562 ms |     0.1430 ms |     0.1268 ms |     1.47 |     0.01 |     11.28 KB |        0.99 |
| WriteMaskTextureFull   | #0F08     | 0.2     |     35.816 ms |     0.1170 ms |     0.1037 ms |     1.00 |     0.01 |     11.35 KB |        0.99 |
| CompositeCorners1      | #0F08     | 0.2     |     39.709 ms |     0.2199 ms |     0.1836 ms |     1.11 |     0.01 |     11.58 KB |        1.01 |
| CompositeCorners2      | #0F08     | 0.2     |     29.069 ms |     0.1338 ms |     0.1186 ms |     0.82 |     0.00 |      6.14 KB |        0.54 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F08** | **0.4** | **44.299 ms** | **0.1795 ms** | **0.1679 ms** | **1.00** | **0.00** | **11.41 KB** |    **1.00** |
| WriteMaskTexture1      | #0F08     | 0.4     |  2,333.884 ms |     7.9638 ms |     6.2176 ms |    52.69 |     0.24 |     11.35 KB |        0.99 |
| WriteMaskTexture64     | #0F08     | 0.4     |     60.207 ms |     0.2528 ms |     0.2364 ms |     1.36 |     0.01 |     11.28 KB |        0.99 |
| WriteMaskTextureFull   | #0F08     | 0.4     |     44.245 ms |     0.2405 ms |     0.2250 ms |     1.00 |     0.01 |     11.35 KB |        0.99 |
| CompositeCorners1      | #0F08     | 0.4     |     48.896 ms |     0.2736 ms |     0.2426 ms |     1.10 |     0.01 |     11.58 KB |        1.01 |
| CompositeCorners2      | #0F08     | 0.4     |     38.178 ms |     0.2657 ms |     0.2486 ms |     0.86 |     0.01 |      6.14 KB |        0.54 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F0F** | **0**   | **11.789 ms** | **0.0574 ms** | **0.0509 ms** | **1.00** | **0.00** | **11.31 KB** |    **1.00** |
| WriteMaskTexture1      | #0F0F     | 0       |      7.336 ms |     0.0437 ms |     0.0365 ms |     0.62 |     0.00 |     11.18 KB |        0.99 |
| WriteMaskTexture64     | #0F0F     | 0       |      7.248 ms |     0.0205 ms |     0.0181 ms |     0.61 |     0.00 |     11.18 KB |        0.99 |
| WriteMaskTextureFull   | #0F0F     | 0       |      8.427 ms |     0.0982 ms |     0.0919 ms |     0.71 |     0.01 |     11.25 KB |        0.99 |
| CompositeCorners1      | #0F0F     | 0       |     19.389 ms |     0.1229 ms |     0.1026 ms |     1.64 |     0.01 |     11.48 KB |        1.01 |
| CompositeCorners2      | #0F0F     | 0       |     13.128 ms |     0.0567 ms |     0.0530 ms |     1.11 |     0.01 |      6.35 KB |        0.56 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F0F** | **0.2** | **34.431 ms** | **0.1045 ms** | **0.0816 ms** | **1.00** | **0.00** | **11.31 KB** |    **1.00** |
| WriteMaskTexture1      | #0F0F     | 0.2     |     30.073 ms |     0.1451 ms |     0.1358 ms |     0.87 |     0.00 |     11.18 KB |        0.99 |
| WriteMaskTexture64     | #0F0F     | 0.2     |     30.026 ms |     0.1351 ms |     0.1263 ms |     0.87 |     0.00 |     11.18 KB |        0.99 |
| WriteMaskTextureFull   | #0F0F     | 0.2     |     31.037 ms |     0.0938 ms |     0.0832 ms |     0.90 |     0.00 |     11.25 KB |        0.99 |
| CompositeCorners1      | #0F0F     | 0.2     |     40.730 ms |     0.4890 ms |     0.4574 ms |     1.18 |     0.01 |     11.48 KB |        1.01 |
| CompositeCorners2      | #0F0F     | 0.2     |     33.775 ms |     0.2646 ms |     0.2475 ms |     0.98 |     0.01 |      6.35 KB |        0.56 |
|                        |           |         |               |               |               |          |          |              |             |
| **WriteMaskComposite** | **#0F0F** | **0.4** | **43.017 ms** | **0.2346 ms** | **0.2194 ms** | **1.00** | **0.00** | **11.31 KB** |    **1.00** |
| WriteMaskTexture1      | #0F0F     | 0.4     |     38.530 ms |     0.1250 ms |     0.0976 ms |     0.90 |     0.00 |     11.18 KB |        0.99 |
| WriteMaskTexture64     | #0F0F     | 0.4     |     38.353 ms |     0.1392 ms |     0.1302 ms |     0.89 |     0.01 |     11.18 KB |        0.99 |
| WriteMaskTextureFull   | #0F0F     | 0.4     |     39.790 ms |     0.1720 ms |     0.1609 ms |     0.92 |     0.01 |     11.25 KB |        0.99 |
| CompositeCorners1      | #0F0F     | 0.4     |     47.720 ms |     0.2891 ms |     0.2414 ms |     1.11 |     0.01 |     11.48 KB |        1.01 |
| CompositeCorners2      | #0F0F     | 0.4     |     42.467 ms |     0.1665 ms |     0.1557 ms |     0.99 |     0.01 |      6.35 KB |        0.56 |
