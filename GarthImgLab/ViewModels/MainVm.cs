namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Tabs;

public sealed partial class MainVm: ObservableObject {
    private readonly PipelineBuilder _pb = new();
    private readonly PreviewCtx _pc = new();

    public MainVm() {
        var home = new HomeTabVm(_pb, _pc);
        Tabs = [home, new ColorTabVm(_pb, _pc), new FrameTabVm(_pb, _pc), new SaveTabVm(_pb)];
        SelTab = home;
    }

    public IReadOnlyList<TabVm> Tabs { get; }
    [ObservableProperty] public partial TabVm SelTab { get; set; }

    partial void OnSelTabChanged(TabVm value) {
        if (value is FxTabVm tab)
            tab.OnActivated();
        else
            _pc.SetEnabled(false);
    }
}
