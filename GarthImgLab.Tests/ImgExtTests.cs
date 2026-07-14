namespace GarthImgLab.Tests;

using System.Diagnostics;
using ImageMagick;

public sealed class ImgExtTests {
    [Theory, InlineData(0), InlineData(1)]
    public unsafe void Common_Cancelled_UnchangedAndThrow(int id) {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Action f = id switch {
            0 => () => img.ToThumb(512, cts.Token),
            1 => () => img.MapPx(static p => ((ushort*)p)[0] = 0, cts.Token),
            _ => throw new UnreachableException()
        };

        Throws<OperationCanceledException>(f);
        Equal(sign, img.Signature);
    }

    [Fact]
    public void ToThumb_ScaleDown_KeepAspect() {
        var red = MagickColors.Red;
        using MagickImage img = new(red, 64, 32);

        img.ToThumb(512, CancellationToken.None);

        Equal(32u, img.Width);
        Equal(16u, img.Height);
        using var px = img.GetPixels();
        Equal(red, px.GetPixel(0, 0).ToColor()); // 没有黑边
        Equal(red, px.GetPixel(31, 15).ToColor()); // 没有黑边
    }

    [Fact]
    public void ToThumb_ScaleUp_DoNothing() {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        img.ToThumb(8192, CancellationToken.None);

        Equal(sign, img.Signature);
    }

    [Fact]
    public void MapPixel_IterateAll() {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var cnt = 0;

        img.MapPx(_ => Interlocked.Increment(ref cnt), CancellationToken.None);

        Equal(2048, cnt);
    }

    [Theory, InlineData(ColorSpace.CMY), InlineData(ColorSpace.CMYK), InlineData(ColorSpace.Gray),
     InlineData(ColorSpace.Lab), InlineData(ColorSpace.XYZ), InlineData(ColorSpace.YUV)]
    public unsafe void MapPixel_NotRgb_UnchangedAndThrow(ColorSpace cs) {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        img.ColorSpace = cs;
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        Throws<InvalidOperationException>(() => img.MapPx(
            static p => ((ushort*)p)[0] = 0,
            CancellationToken.None));

        Equal(sign, img.Signature);
    }
}
