namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels;

public sealed partial class PreviewWindow: Window {
    public PreviewWindow() {
        InitializeComponent();
        Title = $"{Meta.Name} - 预览";
    }

    public PreviewWindow(MainVm vm): this() => DataContext = vm;
}
