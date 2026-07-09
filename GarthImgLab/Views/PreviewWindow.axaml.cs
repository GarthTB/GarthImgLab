namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels;

public sealed partial class PreviewWindow: Window {
    private static PreviewWindow? _current;

    public PreviewWindow() {
        InitializeComponent();
        Title = $"{Meta.Name} - 预览";
    }

    public static void ShowOrActivate(IWorkspaceVm ws) {
        if (_current is {}) {
            _current.Activate();
            return;
        }
        _current = new() { DataContext = ws };
        _current.Closed += (_, _) => {
            ws.Clear();
            _current = null;
        };
        _current.Show();
    }
}
