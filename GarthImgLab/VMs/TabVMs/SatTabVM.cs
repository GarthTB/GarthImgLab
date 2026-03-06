namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using Core;

internal sealed partial class SatTabVM: FXTabVM
{
    [ObservableProperty] private bool _antiClip = true;
    [ObservableProperty] private double _gain;
    [ObservableProperty] private string _strat = "";
    public static IReadOnlyCollection<string> Strats => Sat.Adjustors.Keys;
    partial void OnGainChanged(double value) => Gain = Math.Clamp(value, -1, 1);

    public override void Apply(MImg img, CT ct) =>
        Dialog.RunOrShowEx(
            "调整饱和度",
            () => {
                if (Gain != 0) img.MapRgb(Sat.Adjustors[Strat](Gain), AntiClip, ct);
            });
}
