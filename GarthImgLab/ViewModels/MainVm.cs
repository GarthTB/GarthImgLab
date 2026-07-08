namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;

public sealed partial class MainVm: ObservableObject {
    public MainVm() {
        Tabs = [new FileTabVm(Ctx), new SaturateTabVm(Ctx), new SaveTabVm()];
        SelTab = Tabs[0];
    }

    public IWorkspaceCtx Ctx { get; } = new WorkspaceCtx();
    public IReadOnlyList<TabVm> Tabs { get; }
    [ObservableProperty] public partial TabVm? SelTab { get; set; }

    partial void OnSelTabChanged(TabVm? value) {
        if (value is FxTabVm fx) fx.OnActivated();
    }
}
