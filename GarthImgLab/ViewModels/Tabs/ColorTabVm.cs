// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.Frozen;
using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public sealed partial class ColorTabVm(IPipelineBuilder pb, IPreviewCtx pc): FxTabVm(pb, pc) {
    public override TabTag Tag => TabTag.色彩;

    protected override IReadOnlyList<IFx> Fxs => [
        new Saturator(SelMode, CGain, RGain, GGain, BGain)
    ];

    public static IReadOnlyDictionary<SaturateMode, string> SaturateModes =>
        Enum.GetValues<SaturateMode>().ToFrozenDictionary(static x => x, static x => x.ToString());

    [ObservableProperty] public partial SaturateMode SelMode { get; set; }
    [ObservableProperty] public partial double CGain { get; set; }
    [ObservableProperty] public partial double RGain { get; set; } = 1;
    [ObservableProperty] public partial double GGain { get; set; } = 1;
    [ObservableProperty] public partial double BGain { get; set; } = 1;
}
