namespace GarthImgLab.Models;

using ImageMagick;
using ImageMagick.Drawing;

public sealed class FrameMaker(double ltrRatio, double bRatio, double rcRatio, MagickColor c): IFx {
    public void Apply(MagickImage img, CancellationToken ct) {
        RoundCorner(img, ct);
        AddFrame(img, ct);
    }

    private void RoundCorner(MagickImage img, CancellationToken ct) {
        uint w = img.Width, h = img.Height;
        var r = rcRatio * Math.Min(w, h);
        if (r == 0 || c.A == 0) return;
        ct.ThrowIfCancellationRequested();
        var notA = (ushort)~c.A;
        using MagickImage corners = new(new MagickColor(c) { A = notA }, w, h);
        ct.ThrowIfCancellationRequested();
        new Drawables().FillColor(new MagickColor(c) { A = 65535 })
            .RoundRectangle(0, 0, w, h, r, r)
            .Draw(corners); // 中间不透明，四角 Alpha 反相
        ct.ThrowIfCancellationRequested();
        corners.Negate(Channels.Alpha); // 中间透明，四角 Alpha 还原
        if (notA == 0) { // 若A为0，RGB会被置0，需重新上色
            ct.ThrowIfCancellationRequested();
            corners.Colorize(c, new(100));
        }
        ct.ThrowIfCancellationRequested();
        img.Composite(corners, CompositeOperator.Over);
    }

    private void AddFrame(MagickImage img, CancellationToken ct) {
        uint w = img.Width, h = img.Height;
        var minSide = Math.Min(w, h);
        var ltrPx = (uint)Math.Round(ltrRatio * minSide);
        var bPx = (uint)Math.Round(bRatio * minSide);
        if (ltrPx == 0 && bPx == 0) return;
        ct.ThrowIfCancellationRequested();

        var prev = img.BackgroundColor;
        img.BackgroundColor = c;
        try { img.Extent(-(int)ltrPx, -(int)ltrPx, 2 * ltrPx + w, ltrPx + h + bPx); } finally {
            img.BackgroundColor = prev;
        }
    }
}
