namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using ImageMagick;
using ImageMagick.Drawing;
using Microsoft.Win32;
using static Math;

internal sealed partial class FramingTabVM: FXTabVM, IDisposable {
    [ObservableProperty] public partial bool UseIcon { get; set; }
    [ObservableProperty] public partial MImg? Icon { get; set; }
    [ObservableProperty] public partial double GapRatio { get; set; } = 1.2;
    [ObservableProperty] public partial string FrameColor { get; set; } = "#080808";
    [ObservableProperty] public partial double CornerRatio { get; set; } = .03;
    [ObservableProperty] public partial double LtrRatio { get; set; } = .03;
    [ObservableProperty] public partial double BRatio { get; set; } = .06;
    [ObservableProperty] public partial double TextRatio { get; set; } = .36;
    public static IReadOnlyList<string> FontFamilies => MagickNET.FontFamilies;
    [ObservableProperty] public partial string SelFontFamily { get; set; } = "";
    [ObservableProperty] public partial string TextColor { get; set; } = "#D0A010";
    public static IReadOnlyCollection<string> ExifTags => Exif.Extractors.Keys;
    [ObservableProperty] public partial string ExifKey1 { get; set; } = "";
    [ObservableProperty] public partial string CustomInfo1 { get; set; } = "";
    [ObservableProperty] public partial string ExifKey2 { get; set; } = "";
    [ObservableProperty] public partial string CustomInfo2 { get; set; } = "";
    [ObservableProperty] public partial string ExifKey3 { get; set; } = "";
    [ObservableProperty] public partial string CustomInfo3 { get; set; } = "";
    [ObservableProperty] public partial string ExifKey4 { get; set; } = "";
    [ObservableProperty] public partial string CustomInfo4 { get; set; } = "";
    [ObservableProperty] public partial string ExifKey5 { get; set; } = "";
    [ObservableProperty] public partial string CustomInfo5 { get; set; } = "";
    public void Dispose() => Icon?.Dispose();
    partial void OnCornerRatioChanged(double value) => CornerRatio = Clamp(value, 0, .5);
    partial void OnLtrRatioChanged(double value) => LtrRatio = Clamp(value, 0, 1);
    partial void OnBRatioChanged(double value) => BRatio = Clamp(value, 0, 1);
    partial void OnTextRatioChanged(double value) => TextRatio = Clamp(value, 0, 1);

    partial void OnIconChanged(MImg? oldValue, MImg? newValue) {
        oldValue?.Dispose();
        if (newValue is null) UseIcon = false;
    }

    partial void OnUseIconChanged(bool value) {
        if (value && Icon is null) {
            PickIcon();
            if (Icon is null) UseIcon = false;
        }
        PickIconCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(UseIcon))]
    private void PickIcon() {
        const string filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp|所有文件|*.*";
        OpenFileDialog ofd = new() { Title = "选取图标", Filter = filter };
        try {
            if (ofd.ShowDialog() == true) Icon = new(ofd.FileName);
        } catch (Exception ex) {
            if (ex is not (OCE or { InnerException: OCE })) Show($"加载图标时：\n{ex}", "异常", OK, Error);
            Icon = null;
        }
    }

    public override void Apply(MImg img, CT ct) {
        try {
            var minSide = Min(img.Width, img.Height);
            var frameColor = ParseColor(FrameColor);
            img.RoundCorner(CornerRatio * minSide, frameColor, ct);

            ct.ThrowIfCancellationRequested();
            var bPx = (uint)Round(BRatio * minSide);
            img.AddFrame((uint)Round(LtrRatio * minSide), bPx, frameColor);

            if (bPx < 1) return;

            var text = GetText(img.GetExifProfile());
            var tgtTextH = TextRatio * bPx;
            var (pen, textW, textH, ascent) = string.IsNullOrWhiteSpace(text)
                ? (null, 0, tgtTextH, 0)
                : GetPenMetrics(text, tgtTextH);
            var textX = (img.Width - textW) / 2;
            var iconY = (int)Round(img.Height - (bPx + textH) / 2);

            if (UseIcon && Icon is {}) {
                ct.ThrowIfCancellationRequested();
                using var mIcon = Icon.CloneAndMutate(m => m.Resize(0, (uint)Round(textH)));

                ct.ThrowIfCancellationRequested();
                var gap = GapRatio * textH;
                textX -= (gap + mIcon.Width) / 2;
                var iconX = (int)Round(textX + textW + gap);
                img.Composite(mIcon, iconX, iconY, CompositeOperator.Over);
            }

            if (pen is {}) ct.ThrowIfCancellationRequested();
            pen?.Text(textX, iconY + ascent, text).Draw(img);
        } catch (Exception ex) {
            if (ex is not (OCE or { InnerException: OCE })) Show($"添加边框时：\n{ex}", "异常", OK, Error);
        }
    }

    private static MagickColor ParseColor(string color) {
        try { return new(color); } catch { return MagickColors.Red; }
    }

    private string GetText(IExifProfile? exif) =>
        string.Join(
            "  ",
            ((IEnumerable<string>) [
                (Exif.Extractors[ExifKey1] ?? (_ => CustomInfo1))(exif) ?? "--",
                (Exif.Extractors[ExifKey2] ?? (_ => CustomInfo2))(exif) ?? "--",
                (Exif.Extractors[ExifKey3] ?? (_ => CustomInfo3))(exif) ?? "--",
                (Exif.Extractors[ExifKey4] ?? (_ => CustomInfo4))(exif) ?? "--",
                (Exif.Extractors[ExifKey5] ?? (_ => CustomInfo5))(exif) ?? "--"
            ]).Where(static s => !string.IsNullOrWhiteSpace(s)));

    private (IDrawables<ushort>, double, double, double) GetPenMetrics(string text, double tgtH) {
        var color = ParseColor(TextColor);
        var pen = new Drawables().Font(SelFontFamily)
            .FillColor(color)
            .StrokeColor(color)
            .StrokeOpacity(new(50));
        for (var (i, size) = (0, 14d); i < 3; i++) {
            var metrics = pen.FontPointSize(size).StrokeWidth(.03 * size).FontTypeMetrics(text)
                       ?? throw new InvalidOperationException("无法测量文字尺寸");
            if (Abs(metrics.TextHeight - tgtH) < .1 * tgtH)
                return (pen, metrics.TextWidth, metrics.TextHeight, metrics.Ascent);
            size *= tgtH / metrics.TextHeight;
        }
        throw new InvalidOperationException("无法自动调整字号");
    }
}
