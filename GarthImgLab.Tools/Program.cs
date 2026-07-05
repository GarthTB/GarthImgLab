using GarthImgLab.Models.ColorConverters;

Console.WriteLine(CieLCh.FromSRgb(0, 0, 1).C);
Console.WriteLine(OkLCh.FromSRgb(1, 0, 1).C);
Console.WriteLine(OkLrCh.FromSRgb(1, 0, 1).C);
Console.WriteLine(JzCzhz.FromSRgb(0, 0, 1).C);
