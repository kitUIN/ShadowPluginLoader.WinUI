# Quick Start

This tutorial follows the current loader source (project version `3.1.9`). Use compatible package versions in the host, SDK, and plugins. Some repository sample projects still reference older packages and do not represent every current API.

## Prerequisites

- Windows, Visual Studio with WinUI 3 support, and the Windows App SDK development tools.
- Familiarity with C#, XAML, and DryIoc dependency injection.
- Examples target `net8.0-windows10.0.19041.0`; the loader also targets .NET 6 and .NET 9.
- Building the current loader source requires a compiler with C# extension block support, such as the .NET 10 SDK. The project sets `LangVersion=preview`. The compiler version is separate from the target framework.

## Development order

1. [Create an SDK project](/init/customloader).
2. Define [metadata](/init/metaplugin) and a [plugin base class](/init/iplugin).
3. [Create the loader and initialize the host](/init/customloaderclass).
4. [Create a plugin](/plugin/create) and [package it](/plugin/pack).
5. [Install, load, and manage plugins](/plugin/install) in the host.

The default loading sequence is `CreatePipeline()` → `Feed(...)` → `ProcessAsync()`. Processing automatically instantiates the plugins.
