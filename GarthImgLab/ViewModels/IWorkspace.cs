namespace GarthImgLab.ViewModels;

using System.ComponentModel;
using Avalonia.Media;
using Models;

public interface IWorkspace: INotifyPropertyChanged {
    IImage? DisplayImg { get; }
    void Clear();
    void SetPreviewActive(bool active);
    void SetEnabled(bool enabled);
    Task LoadBefAsync(string path);
    Task UpdateAftAsync(IReadOnlyList<IFx> fxs);
}
