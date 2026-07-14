// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.Frozen;
using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public sealed partial class ColorTabVm(IPipelineBuilder pb, IPreviewCtx pc): FxTabVm(pb, pc) {
    public override TabTag Tag => TabTag.色彩;

    protected override IReadOnlyList<IFx> Fxs => [
        new RgbMapper(
            SelMode,
            CGain,
            [RGain, GGain, BGain],
            [R2R, G2R, B2R, R2G, G2G, B2G, R2B, G2B, B2B])
    ];

    public static IReadOnlyDictionary<SaturateMode, string> SaturateModes =>
        Enum.GetValues<SaturateMode>().ToFrozenDictionary(static x => x, static x => x.ToString());

    #region 增益

    [ObservableProperty] public partial SaturateMode SelMode { get; set; }
    [ObservableProperty] public partial double CGain { get; set; }
    [ObservableProperty] public partial double RGain { get; set; } = 1;
    [ObservableProperty] public partial double GGain { get; set; } = 1;
    [ObservableProperty] public partial double BGain { get; set; } = 1;

    #endregion 增益

    #region 混色器

    [ObservableProperty] public partial double R2R { get; set; } = 1;
    [ObservableProperty] public partial double G2R { get; set; } = 0;
    [ObservableProperty] public partial double B2R { get; set; } = 0;
    [ObservableProperty] public partial double R2G { get; set; } = 0;
    [ObservableProperty] public partial double G2G { get; set; } = 1;
    [ObservableProperty] public partial double B2G { get; set; } = 0;
    [ObservableProperty] public partial double R2B { get; set; } = 0;
    [ObservableProperty] public partial double G2B { get; set; } = 0;
    [ObservableProperty] public partial double B2B { get; set; } = 1;

    #endregion 混色器
}
