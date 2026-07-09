namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public sealed partial class HomeTabVm(IWorkspaceVm ws): TabVm {
    public IWorkspaceVm Ws { get; } = ws;
    public override string Title => "文件";

    public ObservableCollection<string> Paths { get; } = [];
    [ObservableProperty] public partial string? SelPath { get; set; }
    private bool HasSelPath => SelPath is {};

    [ObservableProperty] public partial bool AutoRem { get; set; }

    partial void OnSelPathChanged(string? value) {
        if (value is {})
            _ = Ws.LoadBefAsync(value);
        else
            Ws.Clear();
        RemPathCommand.NotifyCanExecuteChanged();
    }

    public void AddPathAsync(string path) => Paths.Add(path);

    [RelayCommand(CanExecute = nameof(HasSelPath))]
    private async Task RemPathAsync() {
        try {
            var path = SelPath ?? throw new InvalidOperationException("UI 错误");
            if (!Paths.Remove(path)) throw new InvalidOperationException("UI 移除图像失败");
        } catch (Exception ex) { await ex.AlertAsync("移除图像"); }
    }
}
