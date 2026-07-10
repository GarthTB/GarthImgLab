namespace GarthImgLab.ViewModels.Contexts;

using Models;
using Tabs;
using Fxs = IReadOnlyList<Models.IFx>;

public sealed class PipelineBuilder: IPipelineBuilder {
    private readonly Dictionary<TabTag, (bool Enabled, Fxs? Fxs)> _map = Enum.GetValues<TabTag>()
        .ToDictionary(static x => x, static _ => (false, (Fxs?)null));

    public Saver? Saver { get; set; }

    public void SetEnabled(TabTag tag, bool enabled) => _map[tag] = (enabled, _map[tag].Fxs);
    public void UpdateFxs(TabTag tag, Fxs fxs) => _map[tag] = (_map[tag].Enabled, fxs);

    public Fxs Build() {
        List<IFx> pipeline = [];
        foreach (var tag in Enum.GetValues<TabTag>()) {
            var (enabled, fxs) = _map[tag];
            if (enabled && fxs?.Count > 0) pipeline.AddRange(fxs);
        }
        return pipeline;
    }
}
