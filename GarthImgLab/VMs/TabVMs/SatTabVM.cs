namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using Core;

internal sealed partial class SatTabVM: FXTabVM {
    public static IReadOnlyCollection<string> Strats => Sat.Adjustors.Keys;
    [ObservableProperty] public partial string SelStrat { get; set; } = "";
    [ObservableProperty] public partial bool AntiClip { get; set; } = true;
    [ObservableProperty] public partial double Gain { get; set; }
    partial void OnGainChanged(double value) => Gain = Math.Clamp(value, -1, 1);

    public override void Apply(MImg img, CT ct) {
        try {
            if (Gain != 0) img.MapRgb(Sat.Adjustors[SelStrat](Gain), AntiClip, ct);
        } catch (Exception ex) {
            if (ex is not (OCE or { InnerException: OCE })) Show($"调整饱和度时：\n{ex}", "异常", OK, Error);
        }
    }
}
