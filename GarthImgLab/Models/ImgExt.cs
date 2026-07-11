namespace GarthImgLab.Models;

using System.Collections.Concurrent;
using ImageMagick;

public static class ImgExt {
    extension(Img img) {
        public void ToThumb(double maxPx, CT ct) {
            var w = img.Width;
            var w2 = (uint)Math.Round(Math.Sqrt(maxPx / w / img.Height) * w);
            if (w2 >= w) return;
            ct.ThrowIfCancellationRequested();

            img.Resize(w2, 0);
        }

        public void MapPixel(Action<nint> f, CT ct) {
            if (img.ColorSpace != ColorSpace.sRGB) throw new OpEx("不支持此色空间");
            ct.ThrowIfCancellationRequested();

            uint w = img.Width, h = img.Height;
            nint bpp = (nint)img.ChannelCount * sizeof(ushort), bpr = (nint)w * bpp;
            using var px = img.GetPixelsUnsafe();
            var ptr = px.GetAreaPointer(0, 0, w, h);
            Parallel.ForEach(Partitioner.Create(0, (int)h), new() { CancellationToken = ct }, Proc);

            void Proc(Tuple<int, int> range) {
                var (y, yTo) = range;
                var ypTo = ptr + yTo * bpr;
                for (var yp = ptr + y * bpr; yp < ypTo; yp += bpr, y++) {
                    if ((y & 0x0F) == 0) ct.ThrowIfCancellationRequested();
                    var pTo = yp + bpr;
                    for (var p = yp; p < pTo; p += bpp) f(p);
                }
            }
        }
    }
}
