namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;

internal abstract partial class FXTabVM: ObservableObject
{
    [ObservableProperty] private bool _enabled = true;
    public abstract void Apply(MImg img, CT ct);
}
