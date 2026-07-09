namespace GarthImgLab.Views.Tabs;

using Avalonia.Controls;
using Avalonia.Interactivity;
using ViewModels.Tabs;

public sealed partial class HomeTabView: UserControl {
    public HomeTabView() => InitializeComponent();

    private void ShowPreviewWindow(object? _, RoutedEventArgs e) {
        if (DataContext is HomeTabVm vm) PreviewWindow.ShowOrActivate(vm.Ws);
    }
}
