namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Tabs;

public sealed partial class MainVm: ObservableObject {
    public MainVm() {
        var home = new HomeTabVm(Pb, Pc);
        Tabs = [home, new SaturateTabVm(Pb, Pc), new FrameTabVm(Pb, Pc), new SaveTabVm(Pb)];
        SelTab = home;
    }

    private PipelineBuilder Pb { get; } = new();
    private PreviewCtx Pc { get; } = new();

    #region 选项卡

    public IReadOnlyList<TabVm> Tabs { get; }
    [ObservableProperty] public partial TabVm SelTab { get; set; }

    partial void OnSelTabChanged(TabVm value) {
        if (value is FxTabVm tab)
            tab.OnActivated();
        else
            Pc.SetEnabled(false);
    }

    #endregion 选项卡
}
