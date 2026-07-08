// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public sealed partial class SaturateTabVm(IWorkspaceCtx ctx): FxTabVm(ctx) {
    public override string Title => "饱和度";
    protected override IReadOnlyList<IFx> Fxs => [new Saturator(Mode, Strength)];
    [ObservableProperty] public partial SaturateMode Mode { get; set; }
    [ObservableProperty] public partial double Strength { get; set; }
    partial void OnModeChanged(SaturateMode value) => Apply();
    partial void OnStrengthChanged(double value) => Apply();
}
