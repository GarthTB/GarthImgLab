namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;

public partial class PreviewWin: Window {
    public PreviewWin() {
        InitializeComponent();
        Title = $"{Meta.Name} - 预览";
    }
}
