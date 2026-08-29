namespace GarthImgLab.Common;

using System.Reflection;

public static class Meta {
    public const string Name = "Garth 的图像工具";

    public static readonly string Version = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}
