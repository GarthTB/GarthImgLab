namespace GarthImgLab.Views;

using VMs;

public sealed partial class PreviewWindow
{
    private bool _end;

    public PreviewWindow() {
        InitializeComponent();
        IsVisibleChanged += (_, e) => (DataContext as PreviewVM)?.Visible = (bool)e.NewValue;
        Closing += (_, e) => {
            if (e.Cancel = !_end) Hide();
        };
        Closed += (_, _) => (DataContext as IDisposable)?.Dispose();
    }

    public void Shut() {
        _end = true;
        Close();
    }
}
