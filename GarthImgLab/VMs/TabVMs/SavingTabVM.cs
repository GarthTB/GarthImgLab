namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using static ImageMagick.MagickFormat;
using static System.IO.Path;
using File = System.IO.File;

internal sealed partial class SavingTabVM: ObservableObject {
    private static readonly Dictionary<string, string[]> OptionsMap = new() {
        ["JPEG"] = ["4:2:0", "4:2:2", "4:4:4"],
        ["PNG"] = ["Png24", "Png32", "Png48", "Png64"],
        ["TIFF"] = ["LZW", "NoCompression", "Zip"],
        ["WebP"] = []
    };

    public static IReadOnlyCollection<string> Formats => OptionsMap.Keys;
    [ObservableProperty] public partial string SelFormat { get; set; } = "";
    [ObservableProperty] public partial string[] Options { get; private set; } = [];
    [ObservableProperty] public partial string SelOption { get; set; } = "";
    [ObservableProperty] public partial byte Quality { get; set; } = 96;

    public Func<MImg, string, CT, Task> Saver =>
        SelFormat switch {
            "JPEG" => (img, iPath, ct) => {
                img.Settings.SetDefine(Jpeg, "sampling-factor", SelOption);
                return img.WriteAsync(OutPath(iPath, "jpg"), Pjpeg, ct);
            },
            "PNG" when Enum.TryParse(SelOption, out MagickFormat fmt) => (img, iPath, ct) =>
                img.WriteAsync(OutPath(iPath, "png"), fmt, ct),
            "TIFF" when Enum.TryParse(SelOption, out CompressionMethod cmp) => (img, iPath, ct) => {
                img.Settings.Compression = cmp;
                return img.WriteAsync(OutPath(iPath, "tif"), Tiff, ct);
            },
            "WebP" => static (img, iPath, ct) => img.WriteAsync(OutPath(iPath, "webp"), WebP, ct),
            _ => throw new ArgumentException("保存配置无效")
        };

    partial void OnSelFormatChanged(string value) {
        if (OptionsMap.TryGetValue(value, out var options))
            SelOption = (Options = options).FirstOrDefault("");
    }

    private static string OutPath(string iPath, string ext) {
        var dir = GetDirectoryName(iPath) ?? ".";
        var name = GetFileNameWithoutExtension(iPath);
        var oPath = Combine(dir, $"{name}_GIL.{ext}");
        for (var i = 2; File.Exists(oPath); i++) oPath = Combine(dir, $"{name}_GIL_{i}.{ext}");
        return oPath;
    }
}
