namespace GarthImgLab.VMs;

using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

internal sealed partial class PreviewVM: ObservableObject
{
    [ObservableProperty] private BitmapSource? _preview;
}
