namespace GarthImgLab.VMs;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using TabVMs;

internal sealed partial class MainVM(PreviewVM previewVM): ObservableObject, IDisposable
{
    [ObservableProperty] private byte _tabIdx;
    public SatTabVM SatTabVM { get; } = new();
    public FramingTabVM FramingTabVM { get; } = new();
    public SavingTabVM SavingTabVM { get; } = new();
    public void Dispose() => FramingTabVM.Dispose();

    partial void OnTabIdxChanged(byte value) =>
        previewVM.CurFXTabVM = value switch { 1 => SatTabVM, 2 => FramingTabVM, _ => null };

    #region 路径

    [ObservableProperty] private string? _imgPath;
    public ObservableCollection<string> ImgPaths { get; } = [];
    private bool ImgPathSelected => ImgPath is {};
    private bool HasImgPath => ImgPaths.Count > 0;

    partial void OnImgPathChanged(string? value) {
        _ = previewVM.SetSrcAsync(value);
        RemoveImgCommand.NotifyCanExecuteChanged();
    }

    public void AddImg(IEnumerable<string> paths) {
        foreach (var path in paths.Where(path => !ImgPaths.Contains(path))) ImgPaths.Add(path);
        RunCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddImg() => Dialog.RunOrShowEx("添加图像", () => AddImg(Dialog.PickImg("添加图像", true)));

    [RelayCommand(CanExecute = nameof(ImgPathSelected))]
    private void RemoveImg() =>
        Dialog.RunOrShowEx(
            "移除图像",
            () => {
                ImgPaths.Remove(ImgPath!);
                RunCommand.NotifyCanExecuteChanged();
            });

    #endregion 路径

    #region 批处理

    [ObservableProperty] private bool _idle = true;

    [RelayCommand(CanExecute = nameof(HasImgPath), IncludeCancelCommand = true)]
    private Task RunAsync(CT ct) =>
        Dialog.RunOrShowExAsync(
            "批处理",
            async () => {
                previewVM.CancelAll();
                Idle = false;
                var save = Saving.Saver(SavingTabVM.Format, SavingTabVM.Option);
                while (ImgPaths is [var path, ..]) {
                    using MImg img = new();
                    await img.ReadAsync(path, ct);
                    if (SatTabVM.Enabled) await Task.Run(() => SatTabVM.Apply(img, ct), ct);
                    if (FramingTabVM.Enabled) await Task.Run(() => FramingTabVM.Apply(img, ct), ct);
                    img.Quality = SavingTabVM.Quality;
                    await save(img, path, ct);
                    ImgPaths.RemoveAt(0);
                }
                Idle = true;
                Dialog.ShowInfo("完成", "全部处理完成");
            },
            () => Idle = true);

    #endregion 批处理
}
