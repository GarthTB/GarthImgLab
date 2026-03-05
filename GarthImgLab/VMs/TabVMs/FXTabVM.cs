namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;

internal abstract partial class FXTabVM: ObservableObject
{
    [ObservableProperty] private bool _enabled = true;
    public abstract void Apply(IMagickImage<ushort> img, CancellationToken token);
}
