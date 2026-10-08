# 快速开始

想给自己的 WinUI 3 应用加上插件功能？我们从一个 SDK 类库开始，再写一个简单的插件，最后让主程序把它加载进来。

这里会用到三个项目：主程序负责显示界面，SDK 提供大家共用的接口，插件负责实现具体功能。

## 开始前准备什么

- 安装支持 WinUI 3 开发的 Visual Studio 和 Windows App SDK 开发工具。
- 熟悉一些 C# 和 XAML 写法；依赖注入会用到 DryIoc。
- 示例使用 .NET 8。如果还要编译加载器源码，请安装 .NET 10 SDK，以支持其中用到的 C# 扩展块语法。

## 跟着这些步骤来

1. [创建 SDK 项目](/zh/init/customloader)，放置主程序和插件共用的代码。
2. [定义插件信息](/zh/init/metaplugin)，再[写好插件基类](/zh/init/iplugin)。
3. [创建加载器](/zh/init/customloaderclass)，接入主程序。
4. [写一个插件](/zh/plugin/create)，并把它[打包](/zh/plugin/pack)。
5. 回到主程序，[加载和管理插件](/zh/plugin/install)。
