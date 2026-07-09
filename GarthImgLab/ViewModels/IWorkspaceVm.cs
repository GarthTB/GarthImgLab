namespace GarthImgLab.ViewModels;

using System.ComponentModel;
using Avalonia.Media;
using Models;

public interface IWorkspaceVm: INotifyPropertyChanged {
    IImage? DisplayImg { get; }
    void Clear();
    void Toggle(bool on);
    Task LoadBefAsync(string path);
    Task UpdateAftAsync(IReadOnlyList<IFx> fxs);
}
