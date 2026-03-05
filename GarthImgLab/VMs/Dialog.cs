namespace GarthImgLab.VMs;

using Microsoft.Win32;
using static System.Windows.MessageBox;
using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;

internal static class Dialog
{
    public static void ShowInfo(string title, string text) => Show(text, title, OK, Information);

    public static string[] PickImg(string title, bool multi) {
        OpenFileDialog dialog = new() {
            Title = title,
            Multiselect = multi,
            CheckFileExists = true,
            Filter = "图像文件 (*.bmp;*.heic;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp)"
                   + "|*.bmp;*.heic;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp"
                   + "|所有文件 (*.*)|*.*"
        };
        return dialog.ShowDialog() == true
            ? dialog.FileNames
            : [];
    }

    public static void RunOrShowEx(string opName, Action op, Action? onEx = null) {
        try { op(); } catch (OperationCanceledException) {} catch (Exception ex) {
            if (ex.InnerException is OperationCanceledException) return;
            onEx?.Invoke();
            Show($"{opName}异常！\n{ex}", "异常", OK, Error);
        }
    }

    public static async Task RunOrShowExAsync(string opName, Func<Task> op, Action? onEx = null) {
        try { await op(); } catch (OperationCanceledException) {} catch (Exception ex) {
            if (ex.InnerException is OperationCanceledException) return;
            onEx?.Invoke();
            Show($"{opName}异常！\n{ex}", "异常", OK, Error);
        }
    }
}
