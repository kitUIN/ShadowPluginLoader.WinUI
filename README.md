<div align="right">
🌍<a href="">English</a> / 中文
</div>
<div align="center">

# ShadowPluginLoader.WinUI

✨ 在WinUI3中加载插件(拓展) ✨

</div>

<p align="center">
  <a href="https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/LICENSE.txt"><img src="https://img.shields.io/badge/license-MIT-green?style=flat-square" alt="license"></a>
  <a href="https://www.nuget.org/packages/ShadowPluginLoader.WinUI"><img src="https://img.shields.io/nuget/v/ShadowPluginLoader.WinUI?style=flat-square"  alt="nuget"></a>
  <a href="https://github.com/kitUIN/ShadowPluginLoader.WinUI/releases"><img src="https://img.shields.io/github/v/release/kitUIN/ShadowPluginLoader.WinUI?style=flat-square" alt="release"></a>
</p> 
<p align="center">
  📖<a href="https://plugin-loader.kituin.fun/zh/">文档</a>
  ·
  🐛<a href="https://github.com/kitUIN/ShadowPluginLoader.WinUI/issues/new">提交建议</a>
  
</p>

## ✨简介

在`Windows App SDK`上使用插件加载器加载插件

插件内以下内容可以被加载:
- [x] `C#代码`
- [x] 自定义`WinUI控件`(`Page`/`UserControl`)
- [x] 自定义`资源字典`(`ResourceDictionary`)
- [x] `图片资源`(`Assets`)
- [x] `i18n`(`Resw`)
## 🚀快速开始

仅需几个步骤即可构建属于你的插件加载器
- [快速开始](https://plugin-loader.kituin.fun/zh/init/)

## 🧐必要规范

由于特殊的加载模式,我们需要遵循一些代码准则
- [关于准则](https://plugin-loader.kituin.fun/zh/plugin/)

## 加载流程

```mermaid
graph TD;
    B[Type]-->A;
    O[IEnumerable&lt;Type&gt;]-->A;
    C[DirectoryInfo]-->A;
    P[ZipFile]-->A;
    A[FileInfo]-->D@{ shape: procs, label: "Load PluginMetaData"};
    D-->E[Check if PluginMetaData is valid];
    E-->F@{ shape: rounded, label: "Before Load Plugin Event" };
    F-->G[Load Main Plugin];
    G-->H@{ shape: rounded, label: "After Load Plugin Event" };
    H-->I@{ shape: stadium, label: "Finish" };
```
## 工具链流程
```mermaid
block-beta
  columns 4
  a[["Name"]] b[["Before Start"]] c[["Before Compile"]] d[["After Compile"]]
  e["SDK"] f["/"]:2 g["Export PluginMeta Define"]
  h["Plugin"] i["Load Define File"] j["Generate PluginMetadata"] k["Package To Zip"]
```
## 🥰特别鸣谢

- 📦[kitUIN.CustomExtensions.WinUI](https://github.com/kitUIN/CustomExtensions.WinUI) 提供插件加载支持
- 🛠️[ShadowPluginLoader.Tool](https://github.com/kitUIN/ShadowPluginLoader.Tool) 提供插件构建工具
- 🛠️[ShadowPluginLoader.SourceGenerator](https://github.com/kitUIN/ShadowPluginLoader.WinUI/tree/master/ShadowPluginLoader.SourceGenerator) 自动生成帮助类
- 🎉[winui-extensions](https://github.com/dnchattan/winui-extensions) 开创了WinUI3中加载插件的先河
- 📦[DryIoc](https://github.com/dadhi/DryIoc) 提供灵活的依赖注入
- 🛠️[SharpCompress](https://github.com/adamhathcock/sharpcompress) - 提供压缩解压支持  
- 📃[Serilog](https://serilog.net/) 提供开箱即用的日志
- 💡[WindowsAppSDK](https://github.com/microsoft/WindowsAppSDK) 提供框架支持
