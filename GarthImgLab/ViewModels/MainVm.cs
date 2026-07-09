namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Tabs;

public sealed partial class MainVm: ObservableObject {
    public MainVm() {
        Tabs = [new HomeTabVm(Ws), new SaturateTabVm(Ws), new FrameTabVm(Ws), new SaveTabVm()];
        SelTab = Tabs[0];
    }

    private Workspace Ws { get; } = new();
    public IReadOnlyList<TabVm> Tabs { get; }
    [ObservableProperty] public partial TabVm? SelTab { get; set; }

    partial void OnSelTabChanged(TabVm? value) {
        if (value is FxTabVm tab)
            tab.OnActivated();
        else
            _ = Ws.UpdateAftAsync(null);
    }
}
