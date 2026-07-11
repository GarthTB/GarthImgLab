namespace GarthImgLab.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Views;

public static class MsgBox {
    public static Task InfoAsync(string msg, Window? owner = null) =>
        new MsgWindow("提示", msg).ShowDialog(owner ?? GetTopWindow());

    public static Task AlertAsync(this Exception ex, string op, Window? owner = null) {
        var msg = $"{op}时出错：{ex.Message}\n\n详情：\n\n{ex}";
        return new MsgWindow("错误", msg).ShowDialog(owner ?? GetTopWindow());
    }

    private static Window GetTopWindow() {
        if (Application.Current?.ApplicationLifetime is
            not IClassicDesktopStyleApplicationLifetime { MainWindow: {} w })
            throw new OpEx("无法获取主窗口");
        while (w.OwnedWindows is [.., var last]) w = last;
        return w;
    }
}
