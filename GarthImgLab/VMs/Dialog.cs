namespace GarthImgLab.VMs;

using Microsoft.Win32;
using static System.Windows.MessageBox;
using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;
using OCE = OperationCanceledException;

internal static class Dialog
{
    public static string[] PickImg(string title, bool multi) {
        OpenFileDialog dialog = new() {
            Title = title,
            Multiselect = multi,
            CheckFileExists = true,
            Filter = "图像文件 (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp)"
                   + "|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp"
                   + "|所有文件 (*.*)|*.*"
        };
        return dialog.ShowDialog() == true
            ? dialog.FileNames
            : [];
    }

    public static void ShowInfo(string title, string text) => Show(text, title, OK, Information);

    public static void RunOrShowEx(string opName, Action op, Action? onEx = null) {
        try { op(); } catch (Exception ex) { OnEx(ex, opName, onEx); }
    }

    public static async Task RunOrShowExAsync(string opName, Func<Task> op, Action? onEx = null) {
        try { await op(); } catch (Exception ex) { OnEx(ex, opName, onEx); }
    }

    private static void OnEx(Exception ex, string opName, Action? onEx) {
        onEx?.Invoke();
        if (ex is not OCE and { InnerException: not OCE })
            Show($"{opName}异常！\n{ex}", "异常", OK, Error);
    }
}
