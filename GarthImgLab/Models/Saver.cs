namespace GarthImgLab.Models;

using System.Collections.Frozen;
using System.Diagnostics;
using ImageMagick;

public sealed class Saver(string fmt, string option, byte q) {
    public static readonly FrozenDictionary<string, string[]> Map
        = new Dictionary<string, string[]> {
            ["JPEG"] = ["4:2:0", "4:2:2", "4:4:4"],
            ["PNG"] = ["Png24", "Png32", "Png48", "Png64"],
            ["TIFF"] = ["LZW", "NoCompression", "Zip"],
            ["WebP"] = ["Lossless", "Lossy"]
        }.ToFrozenDictionary();

    public Task SaveAsync(MagickImage img, string iPath, CancellationToken ct) {
        img.Quality = q;
        return fmt switch {
            "JPEG" => SaveJpeg(img, iPath, ct),
            "PNG" => SavePng(img, iPath, ct),
            "TIFF" => SaveTiff(img, iPath, ct),
            "WebP" => SaveWebP(img, iPath, ct),
            _ => throw new UnreachableException()
        };
    }

    private Task SaveJpeg(MagickImage img, string iPath, CancellationToken ct) {
        var oPath = GetOutPath(iPath, "jpg");
        img.Settings.SetDefine(MagickFormat.Jpeg, "sampling-factor", option);
        return img.WriteAsync(oPath, MagickFormat.Pjpeg, ct);
    }

    private Task SavePng(MagickImage img, string iPath, CancellationToken ct) {
        var oPath = GetOutPath(iPath, "png");
        if (!Enum.TryParse(option, out MagickFormat png))
            throw new InvalidOperationException("PNG 格式名称无效");
        return img.WriteAsync(oPath, png, ct);
    }

    private Task SaveTiff(MagickImage img, string iPath, CancellationToken ct) {
        var oPath = GetOutPath(iPath, "tif");
        if (!Enum.TryParse(option, out CompressionMethod cmp))
            throw new InvalidOperationException("TIFF 压缩算法无效");
        img.Settings.Compression = cmp;
        return img.WriteAsync(oPath, MagickFormat.Tiff, ct);
    }

    private Task SaveWebP(MagickImage img, string iPath, CancellationToken ct) {
        var oPath = GetOutPath(iPath, "webp");
        var v = option == "Lossless"
            ? "true"
            : "false";
        img.Settings.SetDefine(MagickFormat.WebP, "lossless", v);
        return img.WriteAsync(oPath, MagickFormat.WebP, ct);
    }

    private static string GetOutPath(string iPath, string ext) {
        var dir = Path.GetDirectoryName(iPath) ?? ".";
        var name = Path.GetFileNameWithoutExtension(iPath);
        var oPath = Path.Join(dir, $"{name}_GIL.{ext}");
        for (var i = 2; File.Exists(oPath); i++) oPath = Path.Join(dir, $"{name}_GIL_{i}.{ext}");
        return oPath;
    }
}
