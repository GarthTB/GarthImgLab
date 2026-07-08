namespace GarthImgLab.Contexts;

using System.ComponentModel;
using Avalonia.Media;
using Models;

public interface IWorkspaceCtx: INotifyPropertyChanged {
    IImage? DisplayImg { get; }
    bool AftActive { set; }
    Task LoadBefAsync(string path);
    Task UpdateAftAsync(IReadOnlyList<IFx> fxs);
    void Clear();
}
