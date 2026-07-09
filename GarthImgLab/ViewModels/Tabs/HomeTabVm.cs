namespace GarthImgLab.ViewModels.Tabs;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class HomeTabVm(IWorkspaceVm ws): TabVm {
    public IWorkspaceVm Ws { get; } = ws;
    public override string Title => "文件";

    public ObservableCollection<string> Paths { get; } = [];
    [ObservableProperty] public partial string? SelPath { get; set; }

    [ObservableProperty] public partial bool AutoRem { get; set; }

    partial void OnSelPathChanged(string? value) {
        if (value is {})
            _ = Ws.LoadBefAsync(value);
        else
            Ws.Clear();
    }
}
