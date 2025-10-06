# Create Plugin Base Class

::: warning Note

You should first design the [plugin metadata](/init/metaplugin), then set up the plugin base class.

:::

```csharp [PluginBase.cs]
// Example code
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase: AbstractPlugin
{
    public abstract string GetEmoji();
}
```

- Plugin base class **must** inherit from `AbstractPlugin`
- (Optional) `AbstractPlugin` has some default functions set up, you can add or modify based on this
