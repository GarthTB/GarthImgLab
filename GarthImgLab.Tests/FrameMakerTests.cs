namespace GarthImgLab.Tests;

using ImageMagick;

public sealed class FrameMakerTests {
    private static readonly CancellationToken NoneToken = CancellationToken.None;
    private static readonly MagickColor Red = MagickColors.Red;

    [Fact]
    public void Common_Cancelled_UnchangedAndThrow() {
        using MagickImage img = new(Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Throws<OperationCanceledException>(() =>
            new FrameMaker(.25, .5, .5, Red).Apply(img, cts.Token));

        Equal(sign, img.Signature);
    }

    [Theory, InlineData("#0F0F", 0), InlineData("#0F00", .5)]
    public void RoundCorner_0RatioOr0Alpha_DoNothing(string hex, double ratio) {
        using MagickImage img = new(Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        new FrameMaker(0, 0, ratio, new(hex)).Apply(img, NoneToken);

        Equal(sign, img.Signature);
    }

    [Theory, InlineData("#0F0F", "#0F0"), InlineData("#0F08", "#780")]
    public void RoundCorner_VarAlpha_BlendOver(string hex, string expected) {
        using MagickImage img = new(Red, 64, 32);

        new FrameMaker(0, 0, .5, new(hex)).Apply(img, NoneToken);

        using var px = img.GetPixels();
        Equal(new MagickColor(expected), px.GetPixel(0, 0).ToColor());
    }

    [Fact]
    public void AddFrame_SizeAndPositionCorrectAndNoInterpolation() {
        var lime = MagickColors.Lime;
        using MagickImage img = new(Red, 64, 32);

        new FrameMaker(.25, .5, 0, lime).Apply(img, NoneToken);

        Equal(80u, img.Width);
        Equal(56u, img.Height);
        using var px = img.GetPixels();
        Equal(lime, px.GetPixel(0, 0).ToColor());
        Equal(lime, px.GetPixel(7, 7).ToColor());
        Equal(Red, px.GetPixel(8, 8).ToColor());
        Equal(Red, px.GetPixel(40, 28).ToColor());
        Equal(Red, px.GetPixel(71, 39).ToColor());
        Equal(lime, px.GetPixel(72, 40).ToColor());
        Equal(lime, px.GetPixel(79, 55).ToColor());
    }
}
