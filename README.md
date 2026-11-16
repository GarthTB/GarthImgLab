# GarthImgLab | Garth 的图像工具

![Windows x64](https://img.shields.io/badge/Windows-x64-0078d4)
![macOS arm64](https://img.shields.io/badge/macOS-arm64-0088ff?logo=macos)
![Linux x64](https://img.shields.io/badge/Linux-x64-e95420?logo=linux)

[![MIT License](https://img.shields.io/badge/MIT%20License-750014)](https://mit-license.org)
[![latest release](https://img.shields.io/github/v/release/GarthTB/GarthImgLab?color=0fbf3e&label=latest&logo=github)](https://github.com/GarthTB/GarthImgLab/releases/latest)
![downloads](https://img.shields.io/github/downloads/GarthTB/GarthImgLab/total?color=0fbf3e&label=downloads&logo=github)

[![GitHub repo](https://img.shields.io/badge/GitHub-0fbf3e?logo=github)](https://github.com/GarthTB/GarthImgLab)
[![Gitee repo](https://img.shields.io/badge/Gitee-c71d23?logo=gitee)](https://gitee.com/tb0/GarthImgLab)
[![.NET 10](https://img.shields.io/badge/.NET-10-512bd4?logo=dotnet)](https://dotnet.microsoft.com)
[![C# 14](https://img.shields.io/badge/C%23-14-7253dc)](https://github.com/dotnet/csharplang)
[![AvaloniaUI 12](https://img.shields.io/badge/AvaloniaUI-12-007df9?logo=avaloniaui)](https://avaloniaui.net)

一个轻量 GUI 应用，用于数字摄影图像的末级非线性处理

## ✨ 特性

- 💪 **稳定高效**：ImageMagick 内核，16位精度
- 🎨 **先进色彩**：感知性色空间，调色观感均匀
- 🏭 **批量处理**：多种效果组合，多图共享配置
- 👀 **实时预览**：响应参数变化，所见即所得

## 📥 安装

下载 [最新版发布包](https://github.com/GarthTB/GarthImgLab/releases/latest)，根据平台执行对应步骤。

### Windows

下载 .7z 后解压即用。

### macOS

下载 .dmg 后双击挂载，将 `GarthImgLab.app` 拖拽到 `Applications` 即可。首次打开需绕过系统限制（未签名应用）：

```bash
xattr -dr com.apple.quarantine /Applications/GarthImgLab.app
```

### Linux

下载 .tar.xz 后解压，进入目录执行安装脚本即可：

```bash
tar xJf GarthImgLab-*.tar.xz
cd GarthImgLab-*
./install.sh
```

## ℹ️ 关于

- 地址：https://github.com/GarthTB/GarthImgLab
- 作者：Garth TB | 天卜 <g-art-h@outlook.com>
- 版权：Copyright (c) 2026 Garth TB | 天卜
- 声明：使用前请备份原图。作者不对因使用本应用而造成的任何数据损失负责。
- 历史：[CHANGELOG](https://github.com/GarthTB/GarthImgLab/blob/master/CHANGELOG.md)
