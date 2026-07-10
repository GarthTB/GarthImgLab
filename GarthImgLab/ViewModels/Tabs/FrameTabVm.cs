namespace GarthImgLab.ViewModels.Tabs;

using Contexts;
using Models;

public sealed class FrameTabVm(IPipelineBuilder pb, IPreviewCtx pc): FxTabVm(pb, pc) {
    public override TabTag Tag => TabTag.边框;
    protected override IReadOnlyList<IFx> Fxs => throw new NotImplementedException();
}
