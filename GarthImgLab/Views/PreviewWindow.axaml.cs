namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels;

public sealed partial class PreviewWindow: Window {
    private static PreviewWindow? _current;

    private PreviewWindow() {
        InitializeComponent();
        Title = $"{Meta.Name} - 预览";
    }

    public static void ShowOrActivate(IWorkspace ws) {
        if (_current is {}) {
            _current.Activate();
            return;
        }
        ws.SetPreviewActive(true);
        _current = new() { DataContext = ws };
        _current.Closed += (_, _) => {
            ws.SetPreviewActive(false);
            _current = null;
        };
        _current.Show();
    }

    public static void CloseCurrent() => _current?.Close();
}
