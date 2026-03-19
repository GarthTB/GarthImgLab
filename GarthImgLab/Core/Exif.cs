namespace GarthImgLab.Core;

using ImageMagick;
using static ImageMagick.ExifTag;

internal static class Exif {
    public static readonly Dictionary<string, Func<IExifProfile?, string?>?> Extractors = new() {
        ["手写"] = null,
        ["快门"] = static x => x?.GetValue(ExposureTime)?.Value.ToDouble() is {} t and > 0
            ? t < .4
                ? $"1/{1 / t:0} s"
                : $"{t:0.#} s"
            : null,
        ["焦距"] = static x => x?.GetValue(FocalLength)?.Value.ToDouble() is {} f and > 0
            ? $"{f:0.##} mm"
            : null,
        ["35mm焦距"] = static x => x?.GetValue(FocalLengthIn35mmFilm)?.Value is {} f and > 0
            ? $"{f} mm"
            : null,
        ["光圈F值"] = static x => x?.GetValue(FNumber)?.Value.ToDouble() is {} a and > 0
            ? $"f/{a:0.##}"
            : null,
        ["ISO感光度"] = static x => x?.GetValue(ISOSpeedRatings)?.Value is [var i and > 0, ..]
            ? $"ISO {i}"
            : null,
        ["拍摄时间"] = static x => x?.GetValue(DateTimeOriginal)?.Value is { Length: > 0 } t
            ? t.Length == 19
                ? $"{t[..4]}-{t[5..7]}-{t[8..]}"
                : t
            : null,
        ["作者"] = static x => x?.GetValue(Artist)?.Value,
        ["版权"] = static x => x?.GetValue(Copyright)?.Value,
        ["相机型号"] = static x => x?.GetValue(Model)?.Value,
        ["相机厂商"] = static x => x?.GetValue(Make)?.Value,
        ["镜头型号"] = static x => x?.GetValue(LensModel)?.Value,
        ["镜头厂商"] = static x => x?.GetValue(LensMake)?.Value,
        ["序列号"] = static x => x?.GetValue(SerialNumber)?.Value,
        ["软件"] = static x => x?.GetValue(Software)?.Value
    };
}
