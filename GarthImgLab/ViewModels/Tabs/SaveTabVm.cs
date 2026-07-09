// ReSharper disable UnusedParameterInPartialMethod

namespace GarthImgLab.ViewModels.Tabs;

using CommunityToolkit.Mvvm.ComponentModel;
using Models;

public sealed partial class SaveTabVm: TabVm {
    public SaveTabVm() => SelFormat = Formats[0];
    public override string Title => "保存";
    public Saver Saver => new(SelFormat, SelOption, Quality);

    public static IReadOnlyList<string> Formats => Saver.Map.Keys;
    [ObservableProperty] public partial string SelFormat { get; set; }

    public IReadOnlyList<string> Options => Saver.Map[SelFormat];
    [ObservableProperty] public partial string SelOption { get; set; } = "";

    [ObservableProperty] public partial byte Quality { get; set; } = 96;

    partial void OnSelFormatChanged(string value) {
        OnPropertyChanged(nameof(Options));
        SelOption = Options[0];
    }
}
