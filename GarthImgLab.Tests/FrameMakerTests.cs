namespace GarthImgLab.Tests;

using ImageMagick;

public sealed class FrameMakerTests {
    private static readonly CancellationToken NoneToken = CancellationToken.None;
    private static readonly MagickColor Red = MagickColors.Red;

    [Fact]
    public async Task Common_Cancelled_UnchangedAndThrow() {
        using MagickImage img = new(Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();

        Throws<OperationCanceledException>(() =>
            new FrameMaker(Red, .5, .25, .5).Apply(img, cts.Token));

        Equal(sign, img.Signature);
    }

    [Theory, InlineData("#0F0F", 0), InlineData("#0F00", .5)]
    public void RoundCorner_0RatioOr0Alpha_DoNothing(string hex, double ratio) {
        using MagickImage img = new(Red, 64, 32);
        var sign = img.Signature;
        img.RemoveAttribute("signature");

        new FrameMaker(new(hex), ratio, 0, 0).Apply(img, NoneToken);

        Equal(sign, img.Signature);
    }

    [Theory, InlineData("#0F0F", "#0F0"), InlineData("#0F08", "#780")]
    public void RoundCorner_VarAlpha_BlendOver(string hex, string expected) {
        using MagickImage img = new(Red, 64, 32);

        new FrameMaker(new(hex), .5, 0, 0).Apply(img, NoneToken);

        using var px = img.GetPixels();
        Equal(new MagickColor(expected), px.GetPixel(0, 0).ToColor());
    }

    [Fact]
    public void AddFrame_SizeAndPositionCorrectAndNoInterpolation() {
        var lime = MagickColors.Lime;
        using MagickImage img = new(Red, 64, 32);

        new FrameMaker(lime, 0, .25, .5).Apply(img, NoneToken);

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
