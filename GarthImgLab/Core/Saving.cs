namespace GarthImgLab.Core;

using System.Collections.Frozen;
using ImageMagick;
using static ImageMagick.MagickFormat;
using static System.IO.Path;
using File = System.IO.File;

internal static class Saving
{
    public static readonly FrozenDictionary<string, string[]> Options
        = new Dictionary<string, string[]> {
            ["HEIC"] = ["420", "422", "444"],
            ["JPEG"] = ["4:2:0", "4:2:2", "4:4:4"],
            ["PNG"] = ["PNG24", "PNG32", "PNG48", "PNG64"],
            ["TIFF"] = ["LZMA", "LZW", "NoCompression", "ZIP", "Zstd"],
            ["WebP"] = []
        }.ToFrozenDictionary();

    public static Func<MagickImage, string, Task> GetSaver(string format, string option) =>
        format switch {
            "HEIC" => (img, inPath) => {
                img.Settings.SetDefine(Heic, "chroma", option);
                return img.WriteAsync(GenOutPath(inPath, "heic"), Heic);
            },
            "JPEG" => (img, inPath) => {
                img.Settings.SetDefine(Jpeg, "sampling-factor", option);
                return img.WriteAsync(GenOutPath(inPath, "jpg"), Pjpeg);
            },
            "PNG" when Enum.TryParse(option, true, out MagickFormat fmt) => (img, inPath) =>
                img.WriteAsync(GenOutPath(inPath, "png"), fmt),
            "TIFF" when Enum.TryParse(option, true, out CompressionMethod cmp) => (img, inPath) => {
                img.Settings.Compression = cmp;
                return img.WriteAsync(GenOutPath(inPath, "tif"), Tiff);
            },
            "WebP" => static (img, inPath) => img.WriteAsync(GenOutPath(inPath, "webp"), WebP),
            _ => throw new ArgumentException("保存配置无效")
        };

    private static string GenOutPath(string inPath, string ext) {
        var dir = GetDirectoryName(inPath) ?? ".";
        var name = GetFileNameWithoutExtension(inPath);
        var outPath = Combine(dir, $"{name}_GIL.{ext}");
        for (var i = 2; File.Exists(outPath); i++) outPath = Combine(dir, $"{name}_GIL_{i}.{ext}");
        return outPath;
    }
}
