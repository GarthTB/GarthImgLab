// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.Frozen;
using CommunityToolkit.Mvvm.ComponentModel;
using Models;

public sealed partial class SaturateTabVm(IWorkspaceVm ws): FxTabVm(ws) {
    public override string Title => "饱和度";
    protected override IReadOnlyList<IFx> Fxs => [new Saturator(SelMode, Strength)];

    public static IReadOnlyDictionary<SaturateMode, string> SaturateModes =>
        Enum.GetValues<SaturateMode>().ToFrozenDictionary(static x => x, static x => x.ToString());

    [ObservableProperty] public partial SaturateMode SelMode { get; set; }
    [ObservableProperty] public partial double Strength { get; set; }
    partial void OnSelModeChanged(SaturateMode value) => Apply();
    partial void OnStrengthChanged(double value) => Apply();
}
