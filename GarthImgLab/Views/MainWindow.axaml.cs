namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels;

public sealed partial class MainWindow: Window {
    private readonly MainWindowVM _vm = new();

    public MainWindow() {
        InitializeComponent();
        DataContext = _vm;
        Title = $"{Meta.Name} - {Meta.Version}";
    }
}
