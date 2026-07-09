namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public abstract class TabVm: ObservableObject {
    public abstract string Title { get; }
}

public abstract partial class FxTabVm(IWorkspaceCtx ctx): TabVm {
    protected abstract IReadOnlyList<IFx> Fxs { get; }
    [ObservableProperty] public partial bool Enabled { get; set; }
    partial void OnEnabledChanged(bool value) => ctx.Toggle(value);
    protected void Apply() => _ = ctx.UpdateAftAsync(Fxs);

    public void OnActivated() {
        ctx.Toggle(Enabled);
        if (Enabled) Apply();
    }
}
