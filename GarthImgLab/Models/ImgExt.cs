namespace GarthImgLab.Models;

using ImageMagick;

public static class ImgExt {
    extension(MagickImage img) {
        public void MapPixel(Action<nint> f, CancellationToken ct) {
            if (img.ColorSpace is not (ColorSpace.RGB or ColorSpace.sRGB or ColorSpace.scRGB))
                throw new InvalidOperationException("不支持此色空间");

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
