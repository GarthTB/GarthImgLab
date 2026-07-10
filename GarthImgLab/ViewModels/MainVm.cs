namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Tabs;

public sealed partial class MainVm: ObservableObject {
    public MainVm() =>
        Tabs = [
            new HomeTabVm(Pb, Pc),
            new SaturateTabVm(Pb, Pc),
            new FrameTabVm(Pb, Pc),
            new SaveTabVm(Pb)
        ];

    private PipelineBuilder Pb { get; } = new();
    private PreviewCtx Pc { get; } = new();

    #region 选项卡

    public IReadOnlyList<TabVm> Tabs { get; }
    [ObservableProperty] public partial TabVm SelTab { get; set; }

    partial void OnSelTabChanged(TabVm value) {
        if (value is FxTabVm tab)
            tab.OnActivated();
        else
            _ = Pc.UpdateAftAsync(null);
    }

    #endregion 选项卡
}
