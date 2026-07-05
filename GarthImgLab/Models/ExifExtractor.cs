// ReSharper disable InvertIf

namespace GarthImgLab.Models;

using System.Diagnostics;
using ImageMagick;
using ZLinq;
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

public class ExifExtractor(IEnumerable<TagName> tags, string separator) {
    private const string PlaceHolder = "????";

    private readonly Func<MagickImage, string>[] _extractors = tags.AsValueEnumerable()
        .Select(static x => x switch {
            快门 => (Func<MagickImage, string>)GetExposureTime,
            焦距 => GetFocalLength,
            等效焦距 => GetFocalLengthIn35MMFilm,
            光圈 => GetFNumber,
            感光度 => GetIsoSpeedRatings,
            拍摄时间 => GetDateTimeOriginal,
            作者 => GetArtist,
            版权 => GetCopyright,
            机身厂商 => GetMake,
            机身型号 => GetModel,
            机身序号 => GetBodySerialNumber,
            镜头厂商 => GetLensMake,
            镜头型号 => GetLensModel,
            镜头序号 => GetLensSerialNumber,
            软件 => GetSoftware,
            _ => throw new UnreachableException()
        })
        .ToArray();

    public string GetExifValues(MagickImage img) {
        var values = new string[_extractors.Length];
        for (var i = 0; i < _extractors.Length; i++) values[i] = _extractors[i](img);
        return string.Join(separator, values);
    }

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

    private static string GetArtist(MagickImage img) => GetStr(img, Artist, "tiff:artist");
    private static string GetCopyright(MagickImage img) => GetStr(img, Copyright, "tiff:copyright");
    private static string GetMake(MagickImage img) => GetStr(img, Make, "tiff:make");
    private static string GetModel(MagickImage img) => GetStr(img, Model, "tiff:model");

    private static string GetBodySerialNumber(MagickImage img) =>
        GetStr(img, SerialNumber, "exif:BodySerialNumber");

    private static string GetLensMake(MagickImage img) => GetStr(img, LensMake, "exif:LensMake");
    private static string GetLensModel(MagickImage img) => GetStr(img, LensModel, "exif:LensModel");

    private static string GetLensSerialNumber(MagickImage img) =>
        GetStr(img, LensSerialNumber, "exif:LensSerialNumber");

    private static string GetSoftware(MagickImage img) => GetStr(img, Software, "tiff:software");

    private static string GetStr(MagickImage img, ExifTag<string> tag, string attribute) {
        var v = img.GetExifProfile()?.GetValue(tag)?.Value;
        if (!string.IsNullOrWhiteSpace(v)) return v;
        var s = img.GetAttribute(attribute);
        if (!string.IsNullOrWhiteSpace(s)) return s;
        return PlaceHolder;
    }
}
