# Plugin Loading Flow

This describes the current default `InstallPipeline`, `MainProcessor`, and `AbstractPluginLoader` workflow.

```mermaid
flowchart TD
    A[CheckUpgradeAndRemoveAsync] --> B[CreatePipeline and Feed]
    B --> C[PreprocessAsync: material to workpiece]
    C --> D[Read metadata and extract archives]
    D --> E[Check SDK version and dependency order]
    E --> F[LoadExtensionAsync]
    F --> G[ToBase: resolve MainPlugin and EntryPoints]
    G --> H[Load configs and register main classes in DI]
    H --> I[Cache metadata and return products]
    I --> J[Outbound and Load by plugin ID]
    J --> K[BeforeLoadPlugin]
    K --> L[LoadMainPlugin]
    L --> M[AfterLoadPlugin]
    M --> N[Store instance and call Loaded]
    N --> O[PluginLoaded event]
    O --> P{Persisted enabled state?}
    P -- Yes --> Q[Enabled callback and PluginEnabled event]
    P -- No --> R[Remain disabled]
```

Preprocessors convert local JSON, archives, and HTTP downloads into workpieces. The main processor extracts archives, parses metadata, checks SDK versions and dependencies, loads assemblies, registers configurations/main classes, and returns plugin IDs. The loader resolves instances in that order and invokes their lifecycle.

Within a batch, duplicate IDs prefer a higher version, then a lower `Priority`. Already loaded assemblies are filtered out. Dependencies load before their dependents; otherwise lower `Priority` values go first. This does not support unloading or replacing assemblies at runtime.

See [Installation and Management](/plugin/install) and [Custom Loading Logic](/advance/customloadplugin).
