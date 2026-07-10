namespace GarthImgLab.Views;

using Avalonia.Controls;

public sealed partial class MsgWindow: Window {
    // ReSharper disable once UnusedMember.Global
    /// <summary> 设计器用 </summary>
    public MsgWindow(): this("标题", "内容") {}

    public MsgWindow(string title, string msg) {
        InitializeComponent();
        Title = title;
        Msg.Text = msg;
    }
}
