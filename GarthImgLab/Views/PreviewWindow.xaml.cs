namespace GarthImgLab.Views;

using System.ComponentModel;

public sealed partial class PreviewWindow
{
    private bool _end;
    public PreviewWindow() => InitializeComponent();

    protected override void OnClosing(CancelEventArgs e) {
        if (!_end) {
            e.Cancel = true;
            Hide();
        } else
            base.OnClosing(e);
    }

    public void Shut() {
        _end = true;
        Close();
    }
}
