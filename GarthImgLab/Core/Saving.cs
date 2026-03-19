namespace GarthImgLab.Core;

using ImageMagick;
using static ImageMagick.MagickFormat;
using static System.IO.Path;
using File = System.IO.File;

internal static class Saving {
    public static readonly Dictionary<string, string[]> Options = new() {
        ["JPEG"] = ["4:2:0", "4:2:2", "4:4:4"],
        ["PNG"] = ["Png24", "Png32", "Png48", "Png64"],
        ["TIFF"] = ["LZW", "NoCompression", "Zip"],
        ["WebP"] = []
    };

    public static Func<MImg, string, CT, Task> Saver(string format, string option) =>
        format switch {
            "JPEG" => (img, inPath, ct) => {
                img.Settings.SetDefine(Jpeg, "sampling-factor", option);
                return img.WriteAsync(OutPath(inPath, "jpg"), Pjpeg, ct);
            },
            "PNG" when Enum.TryParse(option, out MagickFormat f) => (img, inPath, ct) =>
                img.WriteAsync(OutPath(inPath, "png"), f, ct),
            "TIFF" when Enum.TryParse(option, out CompressionMethod m) => (img, inPath, ct) => {
                img.Settings.Compression = m;
                return img.WriteAsync(OutPath(inPath, "tif"), Tiff, ct);
            },
            "WebP" => static (img, inPath, ct) => img.WriteAsync(OutPath(inPath, "webp"), WebP, ct),
            _ => throw new ArgumentException("保存配置无效")
        };

    private static string OutPath(string inPath, string ext) {
        var dir = GetDirectoryName(inPath) ?? ".";
        var name = GetFileNameWithoutExtension(inPath);
        var outPath = Combine(dir, $"{name}_GIL.{ext}");
        for (var i = 2; File.Exists(outPath); i++) outPath = Combine(dir, $"{name}_GIL_{i}.{ext}");
        return outPath;
    }
}
