# GarthImgLab Garth的图像工具 ![Tech Stack](https://skillicons.dev/icons?i=dotnet,cs,windows)

[![License MIT](https://img.shields.io/badge/License-MIT-750014)](https://mit-license.org)
[![Latest 1.0.1](https://img.shields.io/badge/Latest-1.0.1-0FBF3E?logo=github)](https://github.com/GarthTB/GarthImgLab/releases/latest)

为数字摄影后期处理设计的 Windows x64 单平台 GUI 应用程序。
集合多种非线性效果，提供丰富参数，支持批量处理、效果组合、
实时预览，用于在图像发布前进行细微润饰和装裱。

## ✨ 特点

- 🏭 一站式：多种功能，批量操作
- 📦 免安装：8MB便捷包，解压即用
- 💪 强大稳定：ImageMagick内核，16位精度
- 👀 实时预览：响应参数变化，所见即所得

## ⚙ 功能

- 饱和度：在多种感知均匀的现代色彩空间中（如 JzCzhz、OKLCh 等）调整饱和度，并提供像素值蒙版以杜绝高光和阴影溢出
- 边框：向图像四周添加胶片风格的纯色边框，并在下边框处嵌入图标、标注自动提取的 EXIF 元数据或自定义信息

## 📥 用法

### 系统要求：Windows x64

### 运行依赖：[.NET 10.0 运行时](https://dotnet.microsoft.com/download/dotnet/10.0)

### 使用步骤

1. 下载 [最新版本压缩包](https://github.com/GarthTB/GarthImgLab/releases/latest) 并解压
2. 运行 `GarthImgLab.exe`
3. 添加图像，选中其一，利用预览调整参数，启动处理
4. 结果自动输出至原图目录下，无覆写风险

## ℹ 关于

- 地址：https://github.com/GarthTB/GarthImgLab
- 技术：.NET 10.0/C# 14.0/WPF
- 依赖：CommunityToolkit.Mvvm/Magick.NET/Wacton.Unicolour
- 协议：[MIT 许可证](https://mit-license.org/)
- 作者：Garth TB | 天卜 <g-art-h@outlook.com>
- 版权：Copyright (c) 2026 Garth TB | 天卜
- 声明：本项目基于作者自用需求，追求极简高效而不确保完备

## 📝 版本

### v1.0.1 (20260319) 移除无效的TIFF压缩选项

### v1.0.0 (20260308) 首发

将以下项目继承并废弃：

- [FrameSeal](https://github.com/GarthTB/FrameSeal) 边框工具 (20250430 - 20260104)
- [OkSaturate](https://github.com/GarthTB/FrameSeal) 饱和度工具 (20250805 - 20260102)
