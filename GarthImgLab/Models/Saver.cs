namespace GarthImgLab.Models;

using System.Collections.Frozen;
using System.Diagnostics;
using ImageMagick;

public sealed class Saver(string fmt, string option, byte q): IFx {
    public static readonly FrozenDictionary<string, string[]> Map
        = new Dictionary<string, string[]> {
            ["JPEG"] = ["4:2:0", "4:2:2", "4:4:4"],
            ["PNG"] = ["Png24", "Png32", "Png48", "Png64"],
            ["TIFF"] = ["LZW", "NoCompression", "Zip"],
            ["WebP"] = ["Lossless", "Lossy"]
        }.ToFrozenDictionary();

    public Task Apply(MagickImage img, CancellationToken ct) {
        img.Quality = q;
        return fmt switch {
            "JPEG" => SaveJpeg(img, ct),
            "PNG" => SavePng(img, ct),
            "TIFF" => SaveTiff(img, ct),
            "WebP" => SaveWebP(img, ct),
            _ => throw new UnreachableException()
        };
    }

    private Task SaveJpeg(MagickImage img, CancellationToken ct) {
        var path = GetOutPath(img, "jpg");
        img.Settings.SetDefine(MagickFormat.Jpeg, "sampling-factor", option);
        return img.WriteAsync(path, MagickFormat.Pjpeg, ct);
    }

    private Task SavePng(MagickImage img, CancellationToken ct) {
        var path = GetOutPath(img, "png");
        if (!Enum.TryParse(option, out MagickFormat png))
            throw new InvalidOperationException("PNG 格式名称无效");
        return img.WriteAsync(path, png, ct);
    }

    private Task SaveTiff(MagickImage img, CancellationToken ct) {
        var path = GetOutPath(img, "tif");
        if (!Enum.TryParse(option, out CompressionMethod cmp))
            throw new InvalidOperationException("TIFF 压缩算法无效");
        img.Settings.Compression = cmp;
        return img.WriteAsync(path, MagickFormat.Tiff, ct);
    }

    private Task SaveWebP(MagickImage img, CancellationToken ct) {
        var path = GetOutPath(img, "webp");
        var v = option == "Lossless"
            ? "true"
            : "false";
        img.Settings.SetDefine(MagickFormat.WebP, "lossless", v);
        return img.WriteAsync(path, MagickFormat.WebP, ct);
    }

    private static string GetOutPath(MagickImage img, string ext) {
        if (img.FileName is not {} iPath) throw new InvalidOperationException("原路径不明");
        var dir = Path.GetDirectoryName(iPath) ?? ".";
        var name = Path.GetFileNameWithoutExtension(iPath);
        var oPath = Path.Join(dir, $"{name}_GIL.{ext}");
        for (var i = 2; File.Exists(oPath); i++) oPath = Path.Join(dir, $"{name}_GIL_{i}.{ext}");
        return oPath;
    }
}
