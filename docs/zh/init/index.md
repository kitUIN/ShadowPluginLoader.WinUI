# 快速开始

本教程按当前仓库的加载器实现编写（项目版本 `3.1.9`）。宿主、SDK 和插件应使用兼容的包版本；仓库中部分示例项目仍引用旧包，不能直接作为当前 API 的依据。

## 前置条件

- Windows、支持 WinUI 3 的 Visual Studio，以及 Windows App SDK 开发环境。
- 了解 C#、XAML 和 DryIoc 依赖注入。
- 示例使用 `net8.0-windows10.0.19041.0`；加载器还多目标编译到 .NET 6 和 .NET 9。
- 编译当前加载器源码需要支持 C# 扩展块语法的编译器（例如 .NET 10 SDK），项目已设置 `LangVersion=preview`。目标框架版本与编译器版本是两回事。

## 开发顺序

1. [创建 SDK 项目](/zh/init/customloader)。
2. [定义元数据](/zh/init/metaplugin)和[插件基类](/zh/init/iplugin)。
3. [创建加载器并初始化宿主](/zh/init/customloaderclass)。
4. [创建插件](/zh/plugin/create)，然后[打包](/zh/plugin/pack)。
5. 在宿主中[安装、加载和管理插件](/zh/plugin/install)。

默认加载链为 `CreatePipeline()` → `Feed(...)` → `ProcessAsync()`；处理结束时会自动实例化插件。
