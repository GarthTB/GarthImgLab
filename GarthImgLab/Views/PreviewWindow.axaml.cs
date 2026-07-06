namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels;

public sealed partial class PreviewWindow: Window {
    private readonly PreviewWindowVM _vm = new();

    public PreviewWindow() {
        InitializeComponent();
        DataContext = _vm;
        Title = $"{Meta.Name} - 预览";
    }
}
