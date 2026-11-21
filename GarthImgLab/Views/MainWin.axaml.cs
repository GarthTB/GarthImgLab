namespace GarthImgLab.Views;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Common;
using ViewModels;

sealed partial class MainWin: Window {
    static readonly FilePickerFileType ImgType = new("图像") {
        Patterns = [
            "*.avif",
            "*.bmp",
            "*.heic",
            "*.heif",
            "*.jpg",
            "*.jpeg",
            "*.png",
            "*.tif",
            "*.tiff",
            "*.webp"
        ],
        MimeTypes = ["image/*"],
        AppleUniformTypeIdentifiers = ["public.image"]
    };

    static readonly FilePickerOpenOptions OpenOptions = new() {
        Title = "选取图像",
        AllowMultiple = true,
        SuggestedFileType = ImgType,
        FileTypeFilter = [ImgType, FilePickerFileTypes.All]
    };

    public MainWin() {
        InitializeComponent();
        DataContext = new MainVM(PickImgPathsAsync, ShowMsg);
        Title = $"{Meta.Name} {Meta.Version}";
    }

    async Task<IReadOnlyList<string>> PickImgPathsAsync() => [
        ..(await StorageProvider.OpenFilePickerAsync(OpenOptions)).Select(static f =>
            f.TryGetLocalPath() ?? throw new Never())
    ];

    Task ShowMsg(string title, string msg) =>
        new MsgWin { Title = title, Msg = { Text = msg } }.ShowDialog(this);
}
