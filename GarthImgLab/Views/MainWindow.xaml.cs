namespace GarthImgLab.Views;

using System.IO;
using System.Windows;
using VMs;

public sealed partial class MainWindow {
    private readonly PreviewWindow _previewWindow = new();

    public MainWindow() {
        InitializeComponent();
        DataContext = new MainVM((PreviewVM)_previewWindow.DataContext);
        Closing += (_, _) => _previewWindow.Shut();
        Closed += (_, _) => (DataContext as IDisposable)?.Dispose();
    }

    private void DropImgPaths(object s, DragEventArgs e) {
        if (DataContext is MainVM vm && e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            vm.AddImgs(paths.Where(File.Exists));
    }

    private void ShowPreview(object s, RoutedEventArgs e) => _previewWindow.Show();
}
