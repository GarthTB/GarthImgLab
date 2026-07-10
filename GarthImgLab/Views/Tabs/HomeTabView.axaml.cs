namespace GarthImgLab.Views.Tabs;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Common;
using ViewModels;
using ViewModels.Tabs;

public sealed partial class HomeTabView: UserControl {
    private readonly FilePickerOpenOptions _openOptions = new() {
        Title = "选取图像",
        AllowMultiple = true,
        SuggestedFileType = FileTypes.Img,
        FileTypeFilter = [FileTypes.Img, FilePickerFileTypes.All]
    };

    public HomeTabView() => InitializeComponent();

    private async void AddImg(object? _, RoutedEventArgs e) {
        try {
            var sp = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (DataContext is not HomeTabVm vm || sp is null) return;
            var files = await sp.OpenFilePickerAsync(_openOptions);
            if (files.Count == 0) return;

            foreach (var file in files)
                using (file)
                    vm.AddPathAsync(file.TryGetLocalPath() ?? file.Path.LocalPath);
        } catch (Exception ex) { await ex.AlertAsync("添加图像"); }
    }

    private void ShowPreviewWindow(object? _, RoutedEventArgs e) {
        if (DataContext is not HomeTabVm vm) return;
        PreviewWindow.ShowOrActivate(vm.Pc);
        if (vm.SelPath is {} path) vm.Pc.LoadBefAsync(path);
    }
}
