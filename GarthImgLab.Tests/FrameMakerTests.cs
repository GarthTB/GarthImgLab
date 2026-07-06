/* namespace GarthImgLab.Tests;

using System.Diagnostics;
using ImageMagick;
public sealed class FrameMakerTests {
    [Theory, InlineData(0), InlineData(1)]
    public void Common_Cancelled_UnchangedAndThrow(int id) {
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Action f = id switch {
            0 => () => FrameMaker.RoundCorner(img, .5, MagickColors.Red, cts.Token),
            1 => () => FrameMaker.AddFrame(img, .25, .5, MagickColors.Red, cts.Token),
            _ => throw new UnreachableException()
        };

        Throws<OperationCanceledException>(f);
        Equal(sign, img.Signature);
    }

    #region RoundCorner

    [Theory, InlineData("#0F0F", "#0F0"), InlineData("#0F08", "#780")]
    public void RoundCorner_VarAlpha_BlendOver(string hex, string expectedHex) {
        MagickColor c = new(hex), expected = new(expectedHex);
        using MagickImage img = new(MagickColors.Red, 64, 32);

        FrameMaker.RoundCorner(img, .5, c, CancellationToken.None);

        using var px = img.GetPixels();
        Equal(expected, px.GetPixel(0, 0).ToColor());
    }

    [Theory, InlineData(0, "#0F0F"), InlineData(.5, "#0F00")]
    public void RoundCorner_0RatioOr0Alpha_DoNothing(double ratio, string hex) {
        MagickColor c = new(hex);
        using MagickImage img = new(MagickColors.Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        FrameMaker.RoundCorner(img, ratio, c, CancellationToken.None);

        Equal(sign, img.Signature);
    }

    #endregion RoundCorner

    #region AddFrame

    [Fact]
    public void AddFrame_SizeAndPositionCorrectAndNoInterpolation() {
        MagickColor red = MagickColors.Red, lime = MagickColors.Lime;
        using MagickImage img = new(red, 64, 32);

        FrameMaker.AddFrame(img, .25, .5, lime, CancellationToken.None);

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

        FrameMaker.AddFrame(img, 0, 0, MagickColors.Red, CancellationToken.None);

        Equal(sign, img.Signature);
    }

    #endregion AddFrame
} */
