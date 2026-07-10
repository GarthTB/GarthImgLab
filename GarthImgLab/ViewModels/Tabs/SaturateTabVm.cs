// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.Frozen;
using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public sealed partial class SaturateTabVm(IPipelineBuilder pb, IPreviewCtx pc): FxTabVm(pb, pc) {
    public override TabTag Tag => TabTag.饱和度;
    protected override IReadOnlyList<IFx> Fxs => [new Saturator(SelMode, Strength)];

    public static IReadOnlyDictionary<SaturateMode, string> SaturateModes =>
        Enum.GetValues<SaturateMode>().ToFrozenDictionary(static x => x, static x => x.ToString());

    [ObservableProperty] public partial SaturateMode SelMode { get; set; }
    [ObservableProperty] public partial double Strength { get; set; }
}
