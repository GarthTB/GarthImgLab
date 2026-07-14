using GarthImgLab.Models.ColorConverters;

Console.WriteLine(CieLCh.FromLinearSRgb(0, 0, 1).C);
Console.WriteLine(OkLCh.FromLinearSRgb(1, 0, 1).C);
Console.WriteLine(OkLrCh.FromLinearSRgb(1, 0, 1).C);
Console.WriteLine(JzCzhz.FromLinearSRgb(0, 0, 1).C);
