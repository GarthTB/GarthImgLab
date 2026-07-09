namespace GarthImgLab.ViewModels.Tabs;

using Models;

public sealed class FrameTabVm(IWorkspace ws): FxTabVm(ws) {
    public override string Title => "边框";
    public override IReadOnlyList<IFx> Fxs => throw new NotImplementedException();
}
