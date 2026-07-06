// ReSharper disable InvertIf

namespace GarthImgLab.Models;

using System.Diagnostics;
using ImageMagick;
using static ImageMagick.ExifTag;
using static TagName;

public enum TagName: byte {
    快门,
    焦距,
    等效焦距,
    光圈,
    感光度,
    拍摄时间,
    作者,
    版权,
    机身厂商,
    机身型号,
    机身序号,
    镜头厂商,
    镜头型号,
    镜头序号,
    软件
}

public static class ExifExtractor {
    private const string PlaceHolder = "????";

    public static string ExtractExif(this MagickImage img, TagName tag) =>
        tag switch {
            快门 => GetExposureTime(img),
            焦距 => GetFocalLength(img),
            等效焦距 => GetFocalLengthIn35MMFilm(img),
            光圈 => GetFNumber(img),
            感光度 => GetIsoSpeedRatings(img),
            拍摄时间 => GetDateTimeOriginal(img),
            作者 => GetStr(img, Artist, "tiff:artist"),
            版权 => GetStr(img, Copyright, "tiff:copyright"),
            机身厂商 => GetStr(img, Make, "tiff:make"),
            机身型号 => GetStr(img, Model, "tiff:model"),
            机身序号 => GetStr(img, SerialNumber, "exif:BodySerialNumber"),
            镜头厂商 => GetStr(img, LensMake, "exif:LensMake"),
            镜头型号 => GetStr(img, LensModel, "exif:LensModel"),
            镜头序号 => GetStr(img, LensSerialNumber, "exif:LensSerialNumber"),
            软件 => GetStr(img, Software, "tiff:software"),
            _ => throw new UnreachableException()
        };

    private static string GetExposureTime(MagickImage img) {
        var v = img.GetExifProfile()?.GetValue(ExposureTime)?.Value.ToDouble();
        if (v is null) {
            var s = img.GetAttribute("exif:ExposureTime");
            if (double.TryParse(s, out var d))
                v = d;
            else if (s?.Length > 0)
                return s;
            else
                return PlaceHolder;
        }
        return v switch { 0 => "0 s", < .3 => $"1/{1 / v:0} s", _ => $"{v:0.#} s" };
    }

    private static string GetFocalLength(MagickImage img) {
        var v = img.GetExifProfile()?.GetValue(FocalLength)?.Value.ToDouble();
        if (v is null) {
            var s = img.GetAttribute("exif:FocalLength");
            if (double.TryParse(s, out var d))
                v = d;
            else if (s?.Length > 0)
                return s;
            else
                return PlaceHolder;
        }
        return $"{v:0.##} mm";
    }

    private static string GetFocalLengthIn35MMFilm(MagickImage img) {
        var v = img.GetExifProfile()?.GetValue(FocalLengthIn35mmFilm)?.Value;
        if (v is null) {
            var s = img.GetAttribute("exif:FocalLengthIn35mmFilm");
            if (ushort.TryParse(s, out var us))
                v = us;
            else if (s?.Length > 0)
                return s;
            else
                return PlaceHolder;
        }
        return $"{v} mm";
    }

    private static string GetFNumber(MagickImage img) {
        var v = img.GetExifProfile()?.GetValue(FNumber)?.Value.ToDouble();
        if (v is null) {
            var s = img.GetAttribute("exif:FNumber");
            if (double.TryParse(s, out var d))
                v = d;
            else if (s?.Length > 0)
                return s;
            else
                return PlaceHolder;
        }
        return $"f/{v:0.##}";
    }

    private static string GetIsoSpeedRatings(MagickImage img) {
        var v = img.GetExifProfile()?.GetValue(ISOSpeedRatings)?.Value.FirstOrDefault();
        if (v is null or 0) {
            var s = img.GetAttribute("exif:ISOSpeedRatings");
            if (ushort.TryParse(s, out var us))
                v = us;
            else if (s?.Length > 0)
                return s;
            else
                return PlaceHolder;
        }
        return $"ISO {v}";
    }

    private static string GetDateTimeOriginal(MagickImage img) {
        var v = img.GetExifProfile()?.GetValue(DateTimeOriginal)?.Value;
        if (string.IsNullOrWhiteSpace(v)) {
            var s = img.GetAttribute("exif:DateTimeOriginal");
            if (string.IsNullOrWhiteSpace(s)) return PlaceHolder;
            v = s;
        }
        return v.Length == 19
            ? $"{v[..4]}-{v[5..7]}-{v[8..]}"
            : v;
    }

    private static string GetStr(MagickImage img, ExifTag<string> tag, string attribute) {
        var v = img.GetExifProfile()?.GetValue(tag)?.Value;
        if (!string.IsNullOrWhiteSpace(v)) return v;
        var s = img.GetAttribute(attribute);
        if (!string.IsNullOrWhiteSpace(s)) return s;
        return PlaceHolder;
    }
}
