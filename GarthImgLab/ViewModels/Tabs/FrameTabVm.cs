namespace GarthImgLab.ViewModels.Tabs;

public sealed class FrameTabVm(IWorkspaceVm ws): TabVm {
    public override string Title => "边框";
}
