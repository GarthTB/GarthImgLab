namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using Core;

internal sealed partial class SavingTabVM: ObservableObject {
    public static IReadOnlyCollection<string> Formats => Saving.Options.Keys;
    [ObservableProperty] public partial string SelFormat { get; set; } = "";
    [ObservableProperty] public partial string[] Options { get; private set; } = [];
    [ObservableProperty] public partial string SelOption { get; set; } = "";
    [ObservableProperty] public partial byte Quality { get; set; } = 96;

    partial void OnSelFormatChanged(string value) {
        if (Saving.Options.TryGetValue(value, out var options))
            SelOption = (Options = options).FirstOrDefault("");
    }
}
