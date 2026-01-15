# Install/Update/Remove

## Install

Need to initialize the loader first, scan plugins, and finally instantiate.

### Initialize

- Call `CheckUpgradeAndRemoveAsync()`

### Scan

- `Feed(Type plugin type)`
- `Feed<TPlugin>()`
- `Feed(IEnumerable<Type> plugin type list)`
- `Feed(DirectoryInfo folder)`
- `Feed(FileInfo plguin.json file)`
- `Feed(Uri plugin.json local path)`

```csharp
var pipeline = loader.StartPipeline();
await pipeline
    .Feed(type)
    .ProcessAsync()
```

### Instantiate

- Call `Load()`

## Update

Due to WindowsAppSdk limitations, there's currently no way to update plugins directly at runtime, only after restarting the program.

- `UpgradePlugin(string id, string newVersionZip)`
  - Supports local zip path
  - Supports network zip path (auto download)

After executing this function, the plugin will be automatically updated when `CheckUpgradeAndRemoveAsync` is called after restarting the program.

## Remove

Due to WindowsAppSdk limitations, there's currently no way to remove plugins directly at runtime, only after restarting the program.

- `RemovePlugin(string id)`

After executing this function, the plugin will be automatically removed when `CheckUpgradeAndRemoveAsync` is called after restarting the program.
