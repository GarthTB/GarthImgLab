namespace GarthImgLab.ViewModels.Tabs;

using CommunityToolkit.Mvvm.ComponentModel;
using Models;

public abstract class TabVm: ObservableObject {
    public abstract string Title { get; }
}

public abstract partial class FxTabVm(IWorkspaceVm ws): TabVm {
    protected abstract IReadOnlyList<IFx> Fxs { get; }
    [ObservableProperty] public partial bool Enabled { get; set; }
    partial void OnEnabledChanged(bool value) => ws.SetEnabled(value);
    protected void Apply() => _ = ws.UpdateAftAsync(Fxs);

    public void OnActivated() {
        ws.SetEnabled(Enabled);
        if (Enabled) Apply();
    }
}
