namespace GarthImgLab.ViewModels.Contexts;

using System.ComponentModel;
using Avalonia.Media;
using Models;

public interface IPreviewCtx: INotifyPropertyChanged {
    IImage? DisplayImg { get; }
    void Clear();
    void SetActive(bool active);
    void SetEnabled(bool enabled);
    Task LoadBefAsync(string path);
    Task UpdateAftAsync(IReadOnlyList<IFx> fxs);
}
