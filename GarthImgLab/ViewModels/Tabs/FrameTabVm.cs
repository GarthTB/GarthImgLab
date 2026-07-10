namespace GarthImgLab.ViewModels.Tabs;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using ImageMagick;
using Models;

public sealed partial class FrameTabVm(IPipelineBuilder pb, IPreviewCtx pc): FxTabVm(pb, pc) {
    public override TabTag Tag => TabTag.边框;

    protected override IReadOnlyList<IFx> Fxs => [
        new FrameMaker(LtrRatio, BRatio, RcRatio, ParseColor(FrameColor)),
        new Annotator(
            UseIcon
                ? Icon
                : null,
            Margin,
            LtrRatio,
            BRatio,
            TextRatio,
            ParseColor(TextColor),
            SelFont,
            Separator,
            [TagOrInfo1, TagOrInfo2, TagOrInfo3, TagOrInfo4, TagOrInfo5, TagOrInfo6])
    ];

    #region 图标

    // ReSharper disable once MemberCanBeMadeStatic.Local
    [ObservableProperty] private partial MagickImage? Icon { get; set; }
    [ObservableProperty] public partial bool UseIcon { get; set; }
    [ObservableProperty] public partial double Margin { get; set; } = 1.5;
    [ObservableProperty] public partial string IconPath { get; set; } = "";

    public async Task LoadIconAsync(string path) {
        Icon?.Dispose();
        Icon = new();
        await Icon.ReadAsync(path);
        IconPath = path;
    }

    #endregion 图标

    #region 边框

    [ObservableProperty] public partial double LtrRatio { get; set; } = .03;
    [ObservableProperty] public partial double BRatio { get; set; } = .06;
    [ObservableProperty] public partial double RcRatio { get; set; } = .03;
    [ObservableProperty] public partial double TextRatio { get; set; } = .36;
    [ObservableProperty] public partial string FrameColor { get; set; } = "#080808";
    [ObservableProperty] public partial string TextColor { get; set; } = "#B88E00";

    private static MagickColor ParseColor(string color) {
        try { return new(color); } catch { return MagickColors.Red; }
    }

    #endregion 边框

    #region 标注

    public static IReadOnlyList<string> Fonts => MagickNET.FontFamilies;
    [ObservableProperty] public partial string SelFont { get; set; } = Fonts[0];
    [ObservableProperty] public partial string Separator { get; set; } = "  ";
    public static IReadOnlyList<string> ExifTags => Enum.GetNames<AvailableExifTag>();
    [ObservableProperty] public partial string TagOrInfo1 { get; set; } = "";
    [ObservableProperty] public partial string TagOrInfo2 { get; set; } = "";
    [ObservableProperty] public partial string TagOrInfo3 { get; set; } = "";
    [ObservableProperty] public partial string TagOrInfo4 { get; set; } = "";
    [ObservableProperty] public partial string TagOrInfo5 { get; set; } = "";
    [ObservableProperty] public partial string TagOrInfo6 { get; set; } = "";

    #endregion 标注
}
