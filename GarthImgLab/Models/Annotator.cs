namespace GarthImgLab.Models;

using ImageMagick;
using ImageMagick.Drawing;
using static Math;

public sealed class Annotator: IFx {
    private readonly double _bRatio, _textRatio, _margin;
    private readonly Func<MagickImage, string> _getInfo;
    private readonly MagickImage? _icon;
    private readonly IDrawables<ushort> _pen;

    public Annotator(
        MagickImage? icon,
        double margin,
        double ltrRatio,
        double bRatio,
        double textRatio,
        MagickColor c,
        string font,
        string separator,
        IEnumerable<string> tagInfoUnions) {
        _pen = new Drawables().Font(font).FillColor(c).StrokeColor(c).StrokeOpacity(new(37));
        var funcs = tagInfoUnions.Select(ExifExtractor.GetExtractor).ToArray();
        _getInfo = img => {
            var parts = funcs.Select(x => x(img)).Where(static x => !string.IsNullOrWhiteSpace(x));
            return string.Join(separator, parts);
        };
        _bRatio = bRatio / (ltrRatio + 1 + bRatio);
        _textRatio = textRatio;
        _icon = icon;
        _margin = margin;
    }

    public void Apply(MagickImage img, CancellationToken ct) {
        uint w = img.Width, h = img.Height;
        var bPx = (uint)Round(_bRatio * Min(w, h));
        if (bPx == 0) return;
        ct.ThrowIfCancellationRequested();

        var info = _getInfo(img);
        var tgtH = _textRatio * bPx;
        var (textW, textH, ascent) = string.IsNullOrWhiteSpace(info)
            ? (0, tgtH, 0)
            : MeasureText(info, tgtH);
        var y = (int)Round(h - (bPx + textH) / 2);
        var textX = (w - textW) / 2;
        var iconH = (uint)Round(textH);

        if (_icon is {} && iconH > 0) {
            ct.ThrowIfCancellationRequested();
            using var mIcon = _icon.CloneAndMutate(m => m.Resize(0, iconH));

            ct.ThrowIfCancellationRequested();
            var margin = _margin * textH;
            textX -= (margin + mIcon.Width) / 2;
            img.Composite(mIcon, (int)Round(textX + textW + margin), y, CompositeOperator.Over);
        }

        if (textW < 1) return;
        ct.ThrowIfCancellationRequested();
        _pen.Text(textX, y + ascent, info).Draw(img);
    }

    private (double, double, double) MeasureText(string text, double tgtH) {
        for (var (i, size) = (0, 14d); i < 3; i++) {
            var m = _pen.FontPointSize(size).StrokeWidth(.03 * size).FontTypeMetrics(text)
                 ?? throw new InvalidOperationException("无法测量文字尺寸");
            if (Abs(m.TextHeight - tgtH) < .1 * tgtH) return (m.TextWidth, m.TextHeight, m.Ascent);
            size *= tgtH / m.TextHeight;
        }
        throw new InvalidOperationException("无法自动调整字号");
    }
}
