namespace GarthImgLab.ViewModels;

using Contexts;

public sealed class FrameTabVm(IWorkspaceCtx ctx): TabVm {
    public override string Title => "边框";
}
