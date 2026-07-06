namespace GarthImgLab.Models;

using ImageMagick;
using ImageMagick.Drawing;
using static Math;
using InfoGetter = Func<ImageMagick.MagickImage, string>;
using PenMetrics = (ImageMagick.Drawing.IDrawables<ushort> Pen, double W, double H, double A);

public sealed class Annotator(
    IEnumerable<string> tagInfoUnions,
    string separator,
    double bRatio,
    double tRatio,
    string font,
    MagickColor c,
    string? iconPath,
    double margin): IDisposable, IFx {
    private readonly InfoGetter _getInfo = GetInfoGetter(tagInfoUnions, separator);

    private readonly MagickImage? _icon = iconPath is {}
        ? new(iconPath)
        : null;

    public void Dispose() => _icon?.Dispose();

    public void Apply(MagickImage img, CancellationToken ct) {
        uint w = img.Width, h = img.Height;
        var bPx = (uint)Round(bRatio * Min(w, h));
        if (bPx == 0) return;
        ct.ThrowIfCancellationRequested();

        var info = _getInfo(img);
        var tgtH = tRatio * bPx;
        var (pen, textW, textH, ascent) = info.Length > 0
            ? GetPenMetrics(info, tgtH)
            : (null, 0, tgtH, 0);
        var textX = (img.Width - textW) / 2;
        var iconY = (int)Round(img.Height - (bPx + textH) / 2);

        if (_icon is {}) {
            ct.ThrowIfCancellationRequested();
            using var mIcon = _icon.CloneAndMutate(m => m.Resize(0, (uint)Round(textH)));

            ct.ThrowIfCancellationRequested();
            var gap = margin * textH;
            textX -= (gap + mIcon.Width) / 2;
            img.Composite(mIcon, (int)Round(textX + textW + gap), iconY, CompositeOperator.Over);
        }

        if (pen is null) return;
        ct.ThrowIfCancellationRequested();
        pen.Text(textX, iconY + ascent, info).Draw(img);
    }

    private static InfoGetter GetInfoGetter(IEnumerable<string> tagInfoUnions, string separator) {
        var funcs = tagInfoUnions.Select(ExifExtractor.GetExtractor).ToArray();
        return img => {
            var parts = new string[funcs.Length];
            for (var i = 0; i < funcs.Length; i++) parts[i] = funcs[i](img);
            return string.Join(separator, parts);
        };
    }

    private PenMetrics GetPenMetrics(string text, double tgtH) {
        var pen = new Drawables().Font(font).FillColor(c).StrokeColor(c).StrokeOpacity(new(50));
        for (var (i, size) = (0, 14d); i < 3; i++) {
            var m = pen.FontPointSize(size).StrokeWidth(.03 * size).FontTypeMetrics(text)
                 ?? throw new InvalidOperationException("无法测量文字尺寸");
            if (Abs(m.TextHeight - tgtH) < .1 * tgtH)
                return (pen, m.TextWidth, m.TextHeight, m.Ascent);
            size *= tgtH / m.TextHeight;
        }
        throw new InvalidOperationException("无法自动调整字号");
    }
}
