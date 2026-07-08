namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using Contexts;

public sealed partial class PreviewWindow: Window {
    private static PreviewWindow? _current;

    public PreviewWindow() {
        InitializeComponent();
        Title = $"{Meta.Name} - 预览";
    }

    public static void ShowOrActivate(IWorkspaceCtx ctx) {
        if (_current is {}) {
            _current.Activate();
            return;
        }
        _current = new() { DataContext = ctx };
        _current.Closed += (_, _) => {
            ctx.Clear();
            _current = null;
        };
        _current.Show();
    }
}
