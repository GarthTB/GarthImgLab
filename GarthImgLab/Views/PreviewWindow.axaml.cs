namespace GarthImgLab.Views;

using Avalonia.Controls;
using Common;
using ViewModels.Contexts;

public sealed partial class PreviewWindow: Window {
    private static PreviewWindow? _current;

    private PreviewWindow() {
        InitializeComponent();
        Title = $"{Meta.Name} - 预览";
    }

    public static void ShowOrActivate(IPreviewCtx pc) {
        if (_current is {}) {
            _current.Activate();
            return;
        }
        pc.SetActive(true);
        _current = new() { DataContext = pc };
        _current.Closed += (_, _) => {
            pc.SetActive(false);
            _current = null;
        };
        _current.Show();
    }

    public static void CloseCurrent() => _current?.Close();
}
