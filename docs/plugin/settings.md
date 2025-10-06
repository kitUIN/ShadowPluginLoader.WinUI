# Plugin Settings

`ShadowPluginLoader.WinUI` version >= `1.4.1`

## Plugin Settings Enum

Plugin settings should be set as enum form, with each enum member corresponding to a setting, and the enum member name being the setting name.

## Declare Settings

Use the `ShadowSetting` annotation to declare a setting.

Parameter description:

| Parameter | Type | Description |
| ------------- | :------: | ---------- |
| `settingType` | `Type` | Setting type |
| `defaultVal` | `string` | Default value |
| `comment` | `string` | Comment |

## Settings Class Generation Configuration

Use the `ShadowSettingClass` annotation to configure the settings class.

Parameter description:

| Parameter | Type | Default | Description |
| ----------- | :------: | :------: |-------------------- |
| `Container` | `string` | Project level namespace | Container name where settings are stored |
| `ClassName` | `string` | `Settings` | Settings class name |
| `PluginAccessorName` | `string` | `Settings` | Settings accessor name in plugin class |

## Example

```csharp [BikaConfigKey.cs]
using ShadowPluginLoader.MetaAttributes;

namespace ShadowExample.Plugin.Emoji;

[ShadowSettingClass(Container = "ShadowExample.Plugin.Emoji", ClassName = "EmojiSetting")]
public enum BikaConfigKey
{
    [ShadowSetting(typeof(int), "1", "Api分流")]
    ApiShunt,

    [ShadowSetting(typeof(bool), "true", "登陆后记住我")]
    RememberMe,

    [ShadowSetting(typeof(string), "tt",comment: "测试名称")]
    TestName
}
```

The automatically generated file is as follows:

```csharp [EmojiSetting.g.cs]
// Automatic Generate From ShadowPluginLoader.SourceGenerator
using ShadowPluginLoader.WinUI.Helpers;

namespace ShadowExample.Plugin.Emoji;

public partial class EmojiSetting
{
    const string Container = "ShadowExample.Plugin.Emoji";

    /// <summary>
    /// Api分流
    /// </summary>
    public System.Int32 ApiShunt
    {
        get => SettingsHelper.Get<System.Int32>(Container, "ApiShunt")!;
        set => SettingsHelper.Set(Container, "ApiShunt", value);
    }
    /// <summary>
    /// 登陆后记住我
    /// </summary>
    public System.Boolean RememberMe
    {
        get => SettingsHelper.Get<System.Boolean>(Container, "RememberMe")!;
        set => SettingsHelper.Set(Container, "RememberMe", value);
    }
    /// <summary>
    /// 测试名称
    /// </summary>
    public System.String TestName
    {
        get => SettingsHelper.Get<System.String>(Container, "TestName")!;
        set => SettingsHelper.Set(Container, "TestName", value);
    }

    public EmojiSetting()
    {
        BeforeInit();
        Init();
        AfterInit();
    }
    private void Init()
    {
        if(!SettingsHelper.Contains(Container, "ApiShunt"))
        {
            ApiShunt = 1;
        }
        if(!SettingsHelper.Contains(Container, "RememberMe"))
        {
            RememberMe = true;
        }
        if(!SettingsHelper.Contains(Container, "TestName"))
        {
            TestName = "tt";
        }
    }
    partial void BeforeInit();

    partial void AfterInit();
}
```

```csharp [EmojiPlugin_Settings.g.cs]
// Automatic Generate From ShadowPluginLoader.SourceGenerator

namespace ShadowExample.Plugin.Emoji;

public partial class EmojiPlugin
{
    /// <summary>
    /// Settings
    /// </summary>
    public static ShadowExample.Plugin.Emoji.EmojiSetting Setting { get; } = new ShadowExample.Plugin.Emoji.EmojiSetting();
}
```
