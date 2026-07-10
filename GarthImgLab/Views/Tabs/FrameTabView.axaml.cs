namespace GarthImgLab.Views.Tabs;

using Avalonia.Controls;
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

    public FrameTabView() => InitializeComponent();

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
