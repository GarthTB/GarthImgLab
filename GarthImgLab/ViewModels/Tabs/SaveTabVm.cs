namespace GarthImgLab.ViewModels.Tabs;

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Contexts;
using Models;

public sealed partial class SaveTabVm: TabVm {
    private readonly IPipelineBuilder _pb;
    public SaveTabVm(IPipelineBuilder pb) => (_pb = pb).Saver = new(SelFormat, SelOption, Quality);
    public override TabTag Tag => TabTag.保存;

    public static IReadOnlyList<string> Formats => Saver.Map.Keys;
    [ObservableProperty] public partial string SelFormat { get; set; } = Saver.Map.Keys[0];
    public IReadOnlyList<string> Options => Saver.Map[SelFormat];
    [ObservableProperty] public partial string SelOption { get; set; } = "";
    [ObservableProperty] public partial byte Quality { get; set; } = 96;

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnSelFormatChanged(string value) {
        OnPropertyChanged(nameof(Options));
        SelOption = Options[0];
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        _pb.Saver = new(SelFormat, SelOption, Quality);
        base.OnPropertyChanged(e);
    }
}
