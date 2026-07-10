namespace GarthImgLab.ViewModels.Tabs;

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public enum TabTag: byte { 主页, 饱和度, 边框, 保存 }

public abstract class TabVm: ObservableObject {
    public abstract TabTag Tag { get; }
}

public abstract partial class FxTabVm(IPipelineBuilder pb, IPreviewCtx pc): TabVm {
    [ObservableProperty] public partial bool Enabled { get; set; }
    protected abstract IReadOnlyList<IFx> Fxs { get; }

    public void OnActivated() {
        pc.SetEnabled(Enabled);
        _ = pc.UpdateAftAsync(Fxs);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(Enabled)) {
            var enabled = Enabled;
            pb.SetEnabled(Tag, enabled);
            pc.SetEnabled(enabled);
        } else {
            var fxs = Fxs;
            pb.UpdateFxs(Tag, fxs);
            _ = pc.UpdateAftAsync(fxs);
        }
        base.OnPropertyChanged(e);
    }
}
