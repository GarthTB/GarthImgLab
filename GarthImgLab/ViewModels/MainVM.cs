namespace GarthImgLab.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

sealed class MainVM(
    Func<Task<IReadOnlyList<string>>> pickImgPathsAsync,
    Func<string, string, Task> showMsg): ObservableObject;
