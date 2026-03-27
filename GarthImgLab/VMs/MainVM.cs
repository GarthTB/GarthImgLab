namespace GarthImgLab.VMs;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Microsoft.Win32;
using TabVMs;

internal sealed partial class MainVM(PreviewVM previewVM): ObservableObject, IDisposable {
    public SatTabVM SatTabVM { get; } = new();
    public FramingTabVM FramingTabVM { get; } = new();
    public SavingTabVM SavingTabVM { get; } = new();
    [ObservableProperty] public partial byte TabIdx { get; set; }
    public void Dispose() => FramingTabVM.Dispose();

    partial void OnTabIdxChanged(byte value) =>
        previewVM.CurFXTabVM = value switch { 1 => SatTabVM, 2 => FramingTabVM, _ => null };

    #region 路径

    public ObservableCollection<string> ImgPaths { get; } = [];
    [ObservableProperty] public partial string? SelImgPath { get; set; }
    private bool ImgPathSelected => SelImgPath is {};
    private bool HasImgPath => ImgPaths.Count > 0;

    partial void OnSelImgPathChanged(string? value) {
        _ = previewVM.SetSrcAsync(value);
        RemoveImgCommand.NotifyCanExecuteChanged();
    }

    public void AddImgs(IEnumerable<string> paths) {
        foreach (var p in paths.Where(p => !ImgPaths.Contains(p))) ImgPaths.Add(p);
        RunCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddImg() {
        const string filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.webp|所有文件|*.*";
        var ofd = new OpenFileDialog { Title = "添加图像", Multiselect = true, Filter = filter };
        if (ofd.ShowDialog() == true) AddImgs(ofd.FileNames);
    }

    [RelayCommand(CanExecute = nameof(ImgPathSelected))]
    private void RemoveImg() {
        ImgPaths.Remove(SelImgPath!);
        RunCommand.NotifyCanExecuteChanged();
    }

    #endregion 路径

    #region 批处理

    [ObservableProperty] public partial bool Idle { get; set; } = true;

    [RelayCommand(CanExecute = nameof(HasImgPath), IncludeCancelCommand = true)]
    private async Task RunAsync(CT ct) {
        try {
            previewVM.CancelAll();
            Idle = false;
            var save = Saving.Saver(SavingTabVM.SelFormat, SavingTabVM.SelOption);
            while (ImgPaths is [var path, ..]) {
                using MImg img = new();
                await img.ReadAsync(path, ct);
                if (SatTabVM.Enabled) await Task.Run(() => SatTabVM.Apply(img, ct), ct);
                if (FramingTabVM.Enabled) await Task.Run(() => FramingTabVM.Apply(img, ct), ct);
                img.Quality = SavingTabVM.Quality;
                await save(img, path, ct);
                ImgPaths.RemoveAt(0);
            }
            Show("全部处理完成", "完成", OK, Information);
        } catch (Exception ex) {
            if (ex is not (OCE or { InnerException: OCE })) Show($"批处理时：\n{ex}", "异常", OK, Error);
        } finally { Idle = true; }
    }

    #endregion 批处理
}
