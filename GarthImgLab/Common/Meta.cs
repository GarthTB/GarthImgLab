namespace GarthImgLab.Common;

using System.Reflection;

public static class Meta {
    public const string Name = "Garth 图像工具";

    public static string Version { get; } = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}
