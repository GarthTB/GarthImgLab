namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contexts;
using ImageMagick;
using OpEx = InvalidOperationException;

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

    [ObservableProperty] public partial bool AutoRem { get; set; }
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

    public void AddPathAsync(string path) => Paths.Add(path);

    [RelayCommand(CanExecute = nameof(HasSelPath))]
    private async Task RemPathAsync() {
        try {
            var path = SelPath ?? throw new OpEx("UI 错误");
            if (!Paths.Remove(path)) throw new OpEx("UI 移除图像失败");
        } catch (Exception ex) { await ex.AlertAsync("移除图像"); }
    }

    #endregion 文件

    #region 批处理

    private CancellationTokenSource _batchCts = new();

    [RelayCommand(CanExecute = nameof(HasPaths))]
    private async Task StartBatchAsync() {
        try {
            var pipeline = _pb.Build();
            var saver = _pb.Saver ?? throw new OpEx("未配置保存参数");
            var ct = _batchCts.Token;
            foreach (var path in Paths) {
                ct.ThrowIfCancellationRequested();
                using MagickImage img = new();
                await img.ReadAsync(path, ct);
                foreach (var fx in pipeline) fx.Apply(img, ct);
                await saver.SaveAsync(img, path, ct);
                if (!AutoRem) continue;
                if (!Paths.Remove(path)) throw new OpEx("UI 移除图像失败");
            }
        } catch (Exception ex) {
            if (ex is not OperationCanceledException) await ex.AlertAsync("批处理");
        } finally {
            _batchCts.Dispose();
            _batchCts = new();
        }
    }

    #endregion 批处理
}
