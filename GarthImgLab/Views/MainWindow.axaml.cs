namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels;

public sealed partial class MainWindow: Window {
    private readonly MainVm _vm = new();

    public MainWindow() {
        InitializeComponent();
        DataContext = _vm;
        Title = $"{Meta.Name} - {Meta.Version}";
    }
}
