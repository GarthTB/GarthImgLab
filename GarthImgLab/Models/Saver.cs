namespace GarthImgLab.Models;

using System.Collections.Frozen;
using ImageMagick;
using Fmt = ImageMagick.MagickFormat;

public sealed class Saver(string fmt, string option, byte q) {
    public static readonly FrozenDictionary<string, string[]> Map
        = new Dictionary<string, string[]> {
            ["JPEG"] = ["4:2:0", "4:2:2", "4:4:4"],
            ["PNG"] = ["Png24", "Png32", "Png48", "Png64"],
            ["TIFF"] = ["LZW", "NoCompression", "Zip"],
            ["WebP"] = ["Lossless", "Lossy"]
        }.ToFrozenDictionary();

    public Task SaveAsync(Img img, string iPath, CT ct) {
        img.Quality = q;
        return fmt switch {
            "JPEG" => SaveJpeg(img, iPath, ct),
            "PNG" => SavePng(img, iPath, ct),
            "TIFF" => SaveTiff(img, iPath, ct),
            "WebP" => SaveWebP(img, iPath, ct),
            _ => throw new NeverEx()
        };
    }

    private Task SaveJpeg(Img img, string iPath, CT ct) {
        var oPath = GetOutPath(iPath, "jpg");
        img.Settings.SetDefine(Fmt.Jpeg, "sampling-factor", option);
        return img.WriteAsync(oPath, Fmt.Pjpeg, ct);
    }

    private Task SavePng(Img img, string iPath, CT ct) {
        var oPath = GetOutPath(iPath, "png");
        if (!Enum.TryParse(option, out Fmt png)) throw new OpEx("PNG 格式名称无效");
        return img.WriteAsync(oPath, png, ct);
    }

    private Task SaveTiff(Img img, string iPath, CT ct) {
        var oPath = GetOutPath(iPath, "tif");
        if (!Enum.TryParse(option, out CompressionMethod cmp)) throw new OpEx("TIFF 压缩算法无效");
        img.Settings.Compression = cmp;
        return img.WriteAsync(oPath, Fmt.Tiff, ct);
    }

    private Task SaveWebP(Img img, string iPath, CT ct) {
        var oPath = GetOutPath(iPath, "webp");
        var v = option == "Lossless"
            ? "true"
            : "false";
        img.Settings.SetDefine(Fmt.WebP, "lossless", v);
        return img.WriteAsync(oPath, Fmt.WebP, ct);
    }

    private static string GetOutPath(string iPath, string ext) {
        var dir = Path.GetDirectoryName(iPath) ?? ".";
        var name = Path.GetFileNameWithoutExtension(iPath);
        var oPath = Path.Join(dir, $"{name}_GIL.{ext}");
        for (var i = 2; File.Exists(oPath); i++) oPath = Path.Join(dir, $"{name}_GIL_{i}.{ext}");
        return oPath;
    }
}
