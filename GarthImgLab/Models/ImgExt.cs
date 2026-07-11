namespace GarthImgLab.Models;

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
            var bpp = (nint)img.ChannelCount * sizeof(ushort);
            using var px = img.GetPixelsUnsafe();
            var p = px.GetAreaPointer(0, 0, w, h);
            Parallel.For(0, (int)(w * h), new() { CancellationToken = ct }, Proc);

            void Proc(int i) {
                if ((i & 0xFFFF) == 0) ct.ThrowIfCancellationRequested();
                f(p + i * bpp);
            }
        }
    }
}
