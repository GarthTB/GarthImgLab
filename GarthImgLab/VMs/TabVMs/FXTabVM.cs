namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;

internal abstract partial class FXTabVM: ObservableObject {
    [ObservableProperty] public partial bool Enabled { get; set; } = true;
    public abstract void Apply(MImg img, CT ct);
}
