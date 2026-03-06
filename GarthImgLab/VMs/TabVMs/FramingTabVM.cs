namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using ImageMagick;
using ImageMagick.Drawing;
using static Math;

internal sealed partial class FramingTabVM: FXTabVM, IDisposable
{
    [ObservableProperty]
    private string _frameColor = "#080808",
        _fontFamily = "",
        _textColor = "#D0A010",
        _exifKey1 = "",
        _exifKey2 = "",
        _exifKey3 = "",
        _exifKey4 = "",
        _exifKey5 = "",
        _customInfo1 = "",
        _customInfo2 = "",
        _customInfo3 = "",
        _customInfo4 = "",
        _customInfo5 = "";

    [ObservableProperty]
    private double _gapRatio = 1.4,
        _cornerRatio = .03,
        _ltrRatio = .03,
        _bRatio = .06,
        _textRatio = .36;

    [ObservableProperty] private MImg? _icon;
    [ObservableProperty] private bool _useIcon;
    public static IReadOnlyCollection<string> ExifTags => Exif.Extractors.Keys;
    public static IReadOnlyList<string> FontFamilies => MagickNET.FontFamilies;
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
    private void PickIcon() =>
        Dialog.RunOrShowEx(
            "选取图标",
            () => {
                if (Dialog.PickImg("选取图标", false) is [var path, ..])
                    _ = Task.Run(() => Icon = new(path));
            },
            () => Icon = null);

    public override void Apply(MImg img, CT ct) =>
        Dialog.RunOrShowEx(
            "添加边框",
            () => {
                var (imgW, imgH) = (img.Width, img.Height);
                var minSide = Min(imgW, imgH);
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
                var textX = (imgW - textW) / 2;
                var iconY = (int)Round(imgH - (bPx + textH) / 2);

                if (UseIcon && Icon is {}) {
                    ct.ThrowIfCancellationRequested();
                    using var mIcon = Icon.CloneAndMutate(m => m.Resize(0, (uint)Round(textH)));

                    ct.ThrowIfCancellationRequested();
                    var shift = (GapRatio * textH + mIcon.Width) / 2;
                    var iconX = (int)Round(textX + textW + shift);
                    img.Composite(mIcon, iconX, iconY, CompositeOperator.Over);

                    textX -= shift;
                }
                if (pen is {}) ct.ThrowIfCancellationRequested();
                pen?.Text(textX, iconY + ascent, text).Draw(img);
            });

    private static MagickColor ParseColor(string color) {
        try { return new(color); } catch { return MagickColors.Red; }
    }

    private string GetText(IExifProfile? exif) =>
        string.Join(
            "  ",
            ((IEnumerable<string?>) [
                (Exif.Extractors[ExifKey1] ?? (_ => CustomInfo1))(exif),
                (Exif.Extractors[ExifKey2] ?? (_ => CustomInfo2))(exif),
                (Exif.Extractors[ExifKey3] ?? (_ => CustomInfo3))(exif),
                (Exif.Extractors[ExifKey4] ?? (_ => CustomInfo4))(exif),
                (Exif.Extractors[ExifKey5] ?? (_ => CustomInfo5))(exif)
            ]).Where(static s => !string.IsNullOrWhiteSpace(s)));

    private (IDrawables<ushort>, double, double, double) GetPenMetrics(string text, double tgtH) {
        var color = ParseColor(TextColor);
        var pen = new Drawables().Font(FontFamily)
            .FillColor(color)
            .StrokeColor(color)
            .StrokeOpacity(new(50));
        for (var (i, size) = (0, 14d); i < 3; i++) {
            var metrics = pen.FontPointSize(size).StrokeWidth(.03 * size).FontTypeMetrics(text)
                       ?? throw new InvalidOperationException("文字尺寸无效");
            if (Abs(metrics.TextHeight - tgtH) < .06 * tgtH)
                return (pen, metrics.TextWidth, metrics.TextHeight, metrics.Ascent);
            size *= tgtH / metrics.TextHeight;
        }
        throw new InvalidOperationException("无法自动调整字号");
    }
}
