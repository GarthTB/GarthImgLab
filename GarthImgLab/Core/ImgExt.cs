namespace GarthImgLab.Core;

using System.Collections.Concurrent;
using ImageMagick;
using ImageMagick.Drawing;
using static Math;

internal static class ImgExt
{
    public static void ToThumb(this IMagickImage img, double maxPx) {
        var w = img.Width;
        var w2 = (uint)Round(Sqrt(maxPx / w / img.Height) * w);
        if (w2 < w) img.Resize(w2, 0, FilterType.Mitchell);
    }

    extension(IMagickImage<ushort> img)
    {
        public void RoundCorner(double rPx, MagickColor color) {
            if (rPx == 0) return;

            var (w, h) = (img.Width, img.Height);
            var notA = (ushort)~color.A;
            using MagickImage mask = new(new MagickColor(color) { A = notA }, w, h);
            new Drawables().FillColor(new MagickColor(color) { A = 65535 })
                .RoundRectangle(0, 0, w, h, rPx, rPx)
                .Draw(mask); // 内不透外反A
            mask.Negate(Channels.Alpha); // 内透外原A
            if (notA == 0) mask.Colorize(color, new(100)); // 若A为0，RGB会被置0，需重新上色
            img.Composite(mask, CompositeOperator.Over);
        }

        public void AddFrame(uint ltrPx, uint bPx, MagickColor color) {
            if (ltrPx == 0 && bPx == 0) return;

            img.BackgroundColor = color;
            img.Extent(-(int)ltrPx, -(int)ltrPx, 2 * ltrPx + img.Width, ltrPx + img.Height + bPx);
        }

        public unsafe void MapRgb(Func<RGB, RGB> func, bool antiClip, CancellationToken token) {
            var ch = img.ChannelCount;
            if (ch < 3) throw new InvalidOperationException("单色图不能映射RGB");

            const double max = 65535;
            var (w, h) = (img.Width, img.Height);
            using var pixels = img.GetPixelsUnsafe();
            var p = (ushort*)pixels.GetAreaPointer(0, 0, w, h);
            Parallel.ForEach(
                Partitioner.Create(0, w * h),
                antiClip
                    ? range => {
                        for (var (px, end) = range; px < end; px++) {
                            if ((px & 0xFFFF) == 0) token.ThrowIfCancellationRequested();

                            var i = px * ch;
                            var (r, g, b) = NormRgb(i);
                            var (r2, g2, b2) = func((r, g, b));
                            var t = Sqrt(2 * Min(Dist(r), Min(Dist(g), Dist(b))));
                            (r2, g2, b2) = (Lerp(r, r2, t), Lerp(g, g2, t), Lerp(b, b2, t));
                            (p[i], p[i + 1], p[i + 2]) = (DeNorm(r2), DeNorm(g2), DeNorm(b2));
                        }
                    }
                    : range => {
                        for (var (px, end) = range; px < end; px++) {
                            if ((px & 0xFFFF) == 0) token.ThrowIfCancellationRequested();

                            var i = px * ch;
                            var (r, g, b) = func(NormRgb(i));
                            (p[i], p[i + 1], p[i + 2]) = (DeNorm(r), DeNorm(g), DeNorm(b));
                        }
                    });

            RGB NormRgb(long i) => (p[i] / max, p[i + 1] / max, p[i + 2] / max);
            static double Dist(double v) => Min(v, 1 - v);
            static double Lerp(double a, double b, double t) => a + t * (b - a);
            static ushort DeNorm(double v) => (ushort)Round(v * max);
        }
    }
}
