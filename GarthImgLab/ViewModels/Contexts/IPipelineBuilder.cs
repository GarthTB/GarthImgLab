namespace GarthImgLab.ViewModels.Contexts;

using Models;
using Tabs;

public interface IPipelineBuilder {
    Saver? Saver { get; set; }
    void SetEnabled(TabTag tag, bool enabled);
    void UpdateFxs(TabTag tag, IReadOnlyList<IFx> fxs);
    IReadOnlyList<IFx> Build();
}
