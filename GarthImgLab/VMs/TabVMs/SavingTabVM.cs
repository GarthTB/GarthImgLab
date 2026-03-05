namespace GarthImgLab.VMs.TabVMs;

using CommunityToolkit.Mvvm.ComponentModel;
using Core;

internal sealed partial class SavingTabVM: ObservableObject
{
    [ObservableProperty] private string _format = "", _option = "";
    [ObservableProperty] private string[] _options = [];
    [ObservableProperty] private byte _quality = 96;
    public static IReadOnlyCollection<string> Formats => Saving.Options.Keys;

    partial void OnFormatChanged(string value) {
        if (Saving.Options.TryGetValue(value, out var options))
            Option = (Options = options).FirstOrDefault("");
    }
}
