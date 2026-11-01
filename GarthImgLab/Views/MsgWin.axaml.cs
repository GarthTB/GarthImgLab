namespace GarthImgLab.Views;

using Avalonia.Controls;

sealed partial class MsgWin: Window {
    public MsgWin() {
        InitializeComponent();
        Btn.Click += (_, _) => Close();
    }
}
