namespace GarthImgLab.Services;

using Avalonia.Media.Imaging;
using Models;

public interface IPreviewService {
    event Action<Bitmap?>? ImgUpdated;
    Task SelectImgAsync(ImgItem? item);
    Task UpdateFxAsync(IFx? fx, bool enabled);
    void Clear();
}
