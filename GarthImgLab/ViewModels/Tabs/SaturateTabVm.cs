// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels.Tabs;

using CommunityToolkit.Mvvm.ComponentModel;
using Models;

public sealed partial class SaturateTabVm(IWorkspaceVm ws): FxTabVm(ws) {
    public override string Title => "饱和度";
    protected override IReadOnlyList<IFx> Fxs => [new Saturator(Mode, Strength)];
    [ObservableProperty] public partial SaturateMode Mode { get; set; }
    [ObservableProperty] public partial double Strength { get; set; }
    partial void OnModeChanged(SaturateMode value) => Apply();
    partial void OnStrengthChanged(double value) => Apply();
}
