namespace GarthImgLab.Models;

using ImageMagick;
using ImageMagick.Drawing;

public static class ImgExt {
    extension(MagickImage img) {
        public void ToThumb(double maxPx, CancellationToken ct) {
            var w = img.Width;
            var w2 = (uint)Math.Round(Math.Sqrt(maxPx / w / img.Height) * w);
            if (w2 >= w) return;
            ct.ThrowIfCancellationRequested();

            img.Resize(w2, 0);
        }

        public void RoundCorner(double ratio, MagickColor c, CancellationToken ct) {
            uint w = img.Width, h = img.Height;
            var r = ratio * Math.Min(w, h);
            if (r == 0 || c.A == 0) return;
            ct.ThrowIfCancellationRequested();

            if (c.A == 65535) {
                using MagickImage mask = new(MagickColors.Black, w, h);
                ct.ThrowIfCancellationRequested();
                new Drawables().FillColor(MagickColors.White)
                    .RoundRectangle(0, 0, w, h, r, r)
                    .Draw(mask);
                ct.ThrowIfCancellationRequested();
                img.SetWriteMask(mask);
                try {
                    using MagickImage tmp = new(c, 1, 1);
                    img.Texture(tmp);
                } finally { img.RemoveWriteMask(); }
            } else {
                using MagickImage corners = new(new MagickColor(c) { A = (ushort)~c.A }, w, h);
                ct.ThrowIfCancellationRequested();
                new Drawables().FillColor(new MagickColor(c) { A = 65535 })
                    .RoundRectangle(0, 0, w, h, r, r)
                    .Draw(corners); // 中间不透明，四角 Alpha 反相
                ct.ThrowIfCancellationRequested();
                corners.Negate(Channels.Alpha); // 中间透明，四角 Alpha 还原
                ct.ThrowIfCancellationRequested();
                img.Composite(corners, CompositeOperator.Over);
            }
        }

        public void AddFrame(double ltrRatio, double bRatio, MagickColor c, CancellationToken ct) {
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

        public void MapPixel(Action<nint> f, CancellationToken ct) {
            if (img.ColorSpace is not (ColorSpace.RGB or ColorSpace.sRGB or ColorSpace.scRGB))
                throw new InvalidOperationException("不支持此色空间");
            ct.ThrowIfCancellationRequested();

            uint w = img.Width, h = img.Height;
            var bpp = (nint)img.ChannelCount * sizeof(ushort);
            using var px = img.GetPixelsUnsafe();
            var ptr = px.GetAreaPointer(0, 0, w, h);
            Parallel.For(0, (int)(w * h), new() { CancellationToken = ct }, Proc);

            void Proc(int i) {
                if ((i & 0xFFFF) == 0) ct.ThrowIfCancellationRequested();
                f(ptr + i * bpp);
            }
        }
    }
}
