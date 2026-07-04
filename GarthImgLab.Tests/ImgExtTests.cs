namespace GarthImgLab.Tests;

using System.Diagnostics;
using ImageMagick;

public sealed class ImgExtTests {
    [Theory, InlineData(0), InlineData(1), InlineData(2), InlineData(3)]
    public unsafe void Common_Cancelled_UnchangedAndThrow(int id) {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Action f = id switch {
            0 => () => img.ToThumb(512, cts.Token),
            1 => () => img.RoundCorner(.5, MagickColors.Red, cts.Token),
            2 => () => img.AddFrame(.25, .5, MagickColors.Red, cts.Token),
            3 => () => img.MapPixel(static p => ((ushort*)p)[1] = 0, cts.Token),
            _ => throw new UnreachableException()
        };

        Throws<OperationCanceledException>(f);
        Equal(sign, img.Signature);
    }

    #region ToThumb

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

    #endregion ToThumb

    #region RoundCorner

    [Theory, InlineData("#0F0F", "#0F0"), InlineData("#0F08", "#780")]
    public void RoundCorner_VarAlpha_BlendOver(string hex, string expectedHex) {
        MagickColor c = new(hex), expected = new(expectedHex);
        using MagickImage img = new(MagickColors.Red, 64, 32);

        img.RoundCorner(.5, c, CancellationToken.None);

        using var px = img.GetPixels();
        Equal(expected, px.GetPixel(0, 0).ToColor());
    }

    [Theory, InlineData(0, "#0F0F"), InlineData(.5, "#0F00")]
    public void RoundCorner_0RatioOr0Alpha_DoNothing(double ratio, string hex) {
        MagickColor c = new(hex);
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        img.RoundCorner(ratio, c, CancellationToken.None);

        Equal(sign, img.Signature);
    }

    #endregion RoundCorner

    #region AddFrame

    [Fact]
    public void AddFrame_SizeAndPositionCorrectAndNoInterpolation() {
        MagickColor red = MagickColors.Red, lime = MagickColors.Lime;
        using MagickImage img = new(red, 64, 32);

        img.AddFrame(.25, .5, lime, CancellationToken.None);

        Equal(80u, img.Width);
        Equal(56u, img.Height);
        using var px = img.GetPixels();
        Equal(lime, px.GetPixel(0, 0).ToColor());
        Equal(lime, px.GetPixel(7, 7).ToColor());
        Equal(red, px.GetPixel(8, 8).ToColor());
        Equal(red, px.GetPixel(40, 28).ToColor());
        Equal(red, px.GetPixel(71, 39).ToColor());
        Equal(lime, px.GetPixel(72, 40).ToColor());
        Equal(lime, px.GetPixel(79, 55).ToColor());
    }

    [Fact]
    public void AddFrame_0Ratio_DoNothing() {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        img.AddFrame(0, 0, MagickColors.Red, CancellationToken.None);

        Equal(sign, img.Signature);
    }

    #endregion AddFrame

    #region MapPixel

    [Fact]
    public void MapPixel_IterateAll() {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var cnt = 0;

        img.MapPixel(_ => Interlocked.Increment(ref cnt), CancellationToken.None);

        Equal(2048, cnt);
    }

    [Theory, InlineData(ColorSpace.CMY), InlineData(ColorSpace.CMYK), InlineData(ColorSpace.Gray),
     InlineData(ColorSpace.Lab), InlineData(ColorSpace.XYZ), InlineData(ColorSpace.YUV)]
    public unsafe void MapPixel_NotRgb_UnchangedAndThrow(ColorSpace cs) {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        img.ColorSpace = cs;
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        Throws<InvalidOperationException>(() => img.MapPixel(
            static p => ((ushort*)p)[1] = 0,
            CancellationToken.None));

        Equal(sign, img.Signature);
    }

    #endregion MapPixel
}
