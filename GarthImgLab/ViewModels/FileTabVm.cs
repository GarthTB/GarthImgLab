namespace GarthImgLab.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;

public sealed partial class FileTabVm(IWorkspaceCtx ctx): TabVm {
    public override string Title => "文件";

    public ObservableCollection<string> Paths { get; } = [];
    [ObservableProperty] public partial string? SelPath { get; set; }

    partial void OnSelPathChanged(string? value) {
        if (value is {})
            _ = ctx.LoadBefAsync(value);
        else
            ctx.Clear();
    }
}
