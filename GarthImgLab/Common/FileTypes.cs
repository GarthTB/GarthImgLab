namespace GarthImgLab.Common;

using Avalonia.Platform.Storage;

public static class FileTypes {
    public static readonly FilePickerFileType Img = new("图像") {
        Patterns = ["*.bmp", "*.jpg", "*.jpeg", "*.png", "*.tif", "*.tiff", "*.webp"],
        MimeTypes = ["image/*"],
        AppleUniformTypeIdentifiers = ["public.image"]
    };
}
