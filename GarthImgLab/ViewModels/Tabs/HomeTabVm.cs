namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contexts;

public sealed partial class HomeTabVm: TabVm {
    private readonly IPipelineBuilder _pb;

    public HomeTabVm(IPipelineBuilder pb, IPreviewCtx pc) {
        _pb = pb;
        Pc = pc;
        Paths.CollectionChanged += (_, _) => StartBatchCommand.NotifyCanExecuteChanged();
    }

    public override TabTag Tag => TabTag.主页;
    public IPreviewCtx Pc { get; }

    #region 文件

    public ObservableCollection<string> Paths { get; } = [];

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RemPathCommand))]
    public partial string? SelPath { get; set; }

    public bool HasPaths => Paths.Count > 0;
    private bool HasSelPath => SelPath is {};

    partial void OnSelPathChanged(string? value) {
        if (value is {})
            _ = Pc.LoadBefAsync(value);
        else
            Pc.Clear();
    }

    public void AddPath(string path) {
        if (!Paths.Contains(path)) Paths.Add(path);
    }

    [RelayCommand(CanExecute = nameof(HasSelPath))]
    private async Task RemPathAsync() {
        try {
            var path = SelPath ?? throw new OpEx("UI 错误");
            if (!Paths.Remove(path)) throw new OpEx("UI 移除图像失败");
        } catch (Exception ex) { await ex.AlertAsync("移除图像"); }
    }

    #endregion 文件

    #region 批处理

    [ObservableProperty] public partial bool AutoRem { get; set; }

    [RelayCommand(CanExecute = nameof(HasPaths), IncludeCancelCommand = true)]
    private async Task StartBatchAsync(CT ct) {
        HashSet<string> done = new(Paths.Count);
        try {
            var pipeline = _pb.Build();
            var saver = _pb.Saver ?? throw new OpEx("未配置保存参数");
            while (Paths.FirstOrDefault(p => !done.Contains(p)) is {} path) {
                ct.ThrowIfCancellationRequested();
                using Img img = new();
                await img.ReadAsync(path, ct);
                await Task.Run(
                    () => {
                        foreach (var fx in pipeline) fx.Apply(img, ct);
                    },
                    ct);
                await saver.SaveAsync(img, path, ct);
                done.Add(path);
                if (AutoRem) Paths.Remove(path);
            }
            await MsgBox.InfoAsync($"批处理完成，共{done.Count}张图像");
        } catch (OCEx) {
            var msg = done.Count > 0
                ? $"已处理{done.Count}张图像"
                : "未处理图像";
            await MsgBox.InfoAsync($"批处理打断，{msg}");
        } catch (Exception ex) { await ex.AlertAsync("批处理"); }
    }

    #endregion 批处理
}
