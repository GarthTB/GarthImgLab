namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;
using ImageMagick.Drawing;
using Microsoft.Win32;
using static Math;
using static ImageMagick.ExifTag;

internal sealed partial class FramingTabVM: FXTabVM, IDisposable {
    private static readonly Dictionary<string, Func<IExifProfile?, string?>?> ExifFuncs = new() {
        ["手写"] = null,
        ["快门"] = static x => x?.GetValue(ExposureTime)?.Value.ToDouble() is {} t and > 0
            ? t < .4
                ? $"1/{1 / t:0} s"
                : $"{t:0.#} s"
            : null,
        ["焦距"] = static x => x?.GetValue(FocalLength)?.Value.ToDouble() is {} f and > 0
            ? $"{f:0.##} mm"
            : null,
        ["35mm焦距"] = static x => x?.GetValue(FocalLengthIn35mmFilm)?.Value is {} f and > 0
            ? $"{f} mm"
            : null,
        ["光圈F值"] = static x => x?.GetValue(FNumber)?.Value.ToDouble() is {} a and > 0
            ? $"f/{a:0.##}"
            : null,
        ["ISO感光度"] = static x => x?.GetValue(ISOSpeedRatings)?.Value is [var i and > 0]
            ? $"ISO {i}"
            : null,
        ["拍摄时间"] = static x => x?.GetValue(DateTimeOriginal)?.Value is { Length: > 0 } t
            ? t.Length == 19
                ? $"{t[..4]}-{t[5..7]}-{t[8..]}"
                : t
            : null,
        ["作者"] = static x => x?.GetValue(Artist)?.Value,
        ["版权"] = static x => x?.GetValue(Copyright)?.Value,
        ["相机型号"] = static x => x?.GetValue(Model)?.Value,
        ["相机厂商"] = static x => x?.GetValue(Make)?.Value,
        ["镜头型号"] = static x => x?.GetValue(LensModel)?.Value,
        ["镜头厂商"] = static x => x?.GetValue(LensMake)?.Value,
        ["软件"] = static x => x?.GetValue(Software)?.Value
    };

    public bool UseIcon {
        get;
        set {
            if (value && Icon is null) {
                PickIcon();
                if (Icon is null) {
                    OnPropertyChanged(); // 退回旧值
                    return;
                }
            }
            if (SetProperty(ref field, value)) PickIconCommand.NotifyCanExecuteChanged();
        }
    }

    private MImg? Icon {
        get;
        set {
            var old = field;
            if (!SetProperty(ref field, value)) return;
            old?.Dispose();
            if (value is null) UseIcon = false;
        }
    }

    [ObservableProperty] public partial double GapRatio { get; set; } = 1.5;
    [ObservableProperty] public partial string FrameColor { get; set; } = "#080808";
    public double CornerRatio { get; set => SetProperty(ref field, Clamp(value, 0, .5)); } = .03;
    public double LtrRatio { get; set => SetProperty(ref field, Clamp(value, 0, 1)); } = .03;
    public double BRatio { get; set => SetProperty(ref field, Clamp(value, 0, 1)); } = .06;
    public double TextRatio { get; set => SetProperty(ref field, Clamp(value, 0, 1)); } = .36;
    public static IReadOnlyList<string> FontFamilies => MagickNET.FontFamilies;
    [ObservableProperty] public partial string SelFontFamily { get; set; } = "";
    [ObservableProperty] public partial string TextColor { get; set; } = "#D0A010";
    public static IReadOnlyCollection<string> ExifTags => ExifFuncs.Keys;
    [ObservableProperty] public partial string Separator { get; set; } = "   ";
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
    [ObservableProperty] public partial string ExifKey6 { get; set; } = "";
    [ObservableProperty] public partial string CustomInfo6 { get; set; } = "";
    public void Dispose() => Icon?.Dispose();

    [RelayCommand(CanExecute = nameof(UseIcon))]
    private void PickIcon() {
        const string filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp|所有文件|*.*";
        var ofd = new OpenFileDialog { Title = "选取图标", Filter = filter };
        if (ofd.ShowDialog() != true) return;
        try { Icon = new(ofd.FileName); } catch (Exception ex) {
            Icon = null;
            Show($"加载图标时：\n{ex}", "异常", OK, Error);
        }
    }

    public override void Apply(MImg img, CT ct) {
        if (!Enabled) return;

        var minSide = Min(img.Width, img.Height);
        var frameColor = ParseColor(FrameColor);
        img.RoundCorner(CornerRatio * minSide, frameColor, ct);

        ct.ThrowIfCancellationRequested();
        var bPx = (uint)Round(BRatio * minSide);
        img.AddFrame((uint)Round(LtrRatio * minSide), bPx, frameColor);

        if (bPx < 1) return;

        var text = GetText(img.GetExifProfile());
        var tgtH = TextRatio * bPx;
        var (pen, textW, textH, ascent) = text.Length > 0
            ? GetPenMetrics(text, tgtH)
            : (null, 0, tgtH, 0);
        var textX = (img.Width - textW) / 2;
        var iconY = (int)Round(img.Height - (bPx + textH) / 2);

        if (UseIcon && Icon is {}) {
            ct.ThrowIfCancellationRequested();
            using var mIcon = Icon.CloneAndMutate(m => m.Resize(0, (uint)Round(textH)));

            ct.ThrowIfCancellationRequested();
            var gap = GapRatio * textH;
            textX -= (gap + mIcon.Width) / 2;
            img.Composite(mIcon, (int)Round(textX + textW + gap), iconY, CompositeOperator.Over);
        }
        if (pen is null) return;
        ct.ThrowIfCancellationRequested();
        pen.Text(textX, iconY + ascent, text).Draw(img);
    }

    private static MagickColor ParseColor(string color) {
        try { return new(color); } catch { return MagickColors.Red; }
    }

    private string GetText(IExifProfile? exif) =>
        string.Join(
            Separator,
            new[] {
                (ExifFuncs[ExifKey1] ?? (_ => CustomInfo1))(exif) ?? "???",
                (ExifFuncs[ExifKey2] ?? (_ => CustomInfo2))(exif) ?? "???",
                (ExifFuncs[ExifKey3] ?? (_ => CustomInfo3))(exif) ?? "???",
                (ExifFuncs[ExifKey4] ?? (_ => CustomInfo4))(exif) ?? "???",
                (ExifFuncs[ExifKey5] ?? (_ => CustomInfo5))(exif) ?? "???",
                (ExifFuncs[ExifKey6] ?? (_ => CustomInfo6))(exif) ?? "???"
            }.Where(static s => !string.IsNullOrWhiteSpace(s)));

    private (IDrawables<ushort>, double, double, double) GetPenMetrics(string text, double tgtH) {
        var color = ParseColor(TextColor);
        var pen = new Drawables().Font(SelFontFamily)
            .FillColor(color)
            .StrokeColor(color)
            .StrokeOpacity(new(50));
        for (var (i, size) = (0, 14d); i < 3; i++) {
            var m = pen.FontPointSize(size).StrokeWidth(.03 * size).FontTypeMetrics(text)
                 ?? throw new InvalidOperationException("无法测量文字尺寸");
            if (Abs(m.TextHeight - tgtH) < .1 * tgtH)
                return (pen, m.TextWidth, m.TextHeight, m.Ascent);
            size *= tgtH / m.TextHeight;
        }
        throw new InvalidOperationException("无法自动调整字号");
    }
}
