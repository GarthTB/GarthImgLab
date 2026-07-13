namespace GarthImgLab.Views.Tabs;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Common;
using ViewModels;
using ViewModels.Tabs;

public sealed partial class FrameTabView: UserControl {
    private readonly FilePickerOpenOptions _openOptions = new() {
        Title = "选取图标",
        SuggestedFileType = FileTypes.Img,
        FileTypeFilter = [FileTypes.Img, FilePickerFileTypes.All]
    };

    public FrameTabView() {
        InitializeComponent();
        IconCheckBox.AddHandler(DragDrop.DragOverEvent, DragOverIcon);
        IconCheckBox.AddHandler(DragDrop.DropEvent, DropIcon);
        IconBtn.AddHandler(DragDrop.DragOverEvent, DragOverIcon);
        IconBtn.AddHandler(DragDrop.DropEvent, DropIcon);
        IconTextBox.AddHandler(DragDrop.DragOverEvent, DragOverIcon);
        IconTextBox.AddHandler(DragDrop.DropEvent, DropIcon);
    }

    private static void DragOverIcon(object? _, DragEventArgs e) =>
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

    private async void DropIcon(object? _, DragEventArgs e) {
        try {
            if (DataContext is not FrameTabVm vm) return;
            var files = e.DataTransfer.TryGetFiles();
            if (files is not { Length: > 0 }) return;
            using var file = files[0];
            await vm.LoadIconAsync(file.TryGetLocalPath() ?? file.Path.LocalPath);
        } catch (Exception ex) { await ex.AlertAsync("拖放图标"); } finally { e.Handled = true; }
    }

    private async void SelectIcon(object? _, RoutedEventArgs e) {
        try {
            var sp = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (DataContext is not FrameTabVm vm || sp is null) return;
            var files = await sp.OpenFilePickerAsync(_openOptions);
            if (files.Count == 0) return;
            using var file = files[0];
            await vm.LoadIconAsync(file.TryGetLocalPath() ?? file.Path.LocalPath);
        } catch (Exception ex) { await ex.AlertAsync("选取图标"); }
    }
}
