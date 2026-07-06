namespace GarthImgLab.Models;

public sealed record ImgItem(string Path) {
    public string Name { get; } = System.IO.Path.GetFileNameWithoutExtension(Path);
}
