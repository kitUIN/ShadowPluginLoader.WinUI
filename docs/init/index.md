# Quick Start

Want to add plugins to your WinUI 3 app? We'll create a shared SDK, write a small plugin, and load it in the app.

You'll work with three projects: the app displays the UI, the SDK provides shared APIs, and plugins add features.

## Before you start

- Install Visual Studio with WinUI 3 support and the Windows App SDK development tools.
- Have some familiarity with C# and XAML. We'll use DryIoc for dependency injection.
- The examples target .NET 8. If you also want to build the loader from source, install the .NET 10 SDK for its C# extension block support.

## Follow these steps

1. [Create the SDK project](/init/customloader) for your shared code.
2. [Describe your plugins with metadata](/init/metaplugin) and [create their base class](/init/iplugin).
3. [Create the loader](/init/customloaderclass) and connect it to your app.
4. [Write a plugin](/plugin/create) and [package it](/plugin/pack).
5. Back in the app, [load and manage your plugins](/plugin/install).
