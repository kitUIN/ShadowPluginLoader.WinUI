// Mirrors the default SDK implementation; this is a teaching model, not a live loader.
export type Locale = 'zh' | 'en'
export type Scenario = 'enabled' | 'disabled' | 'sdk-error' | 'loaded-error' | 'disable-after'
export type Source = 'archive' | 'local' | 'http'
type Text = [string, string]
export type Step = {
  id: string
  phase: number
  title: Text
  detail: Text
  code: string
  file: string
  kind: 'work' | 'callback' | 'event' | 'error'
  event?: string
  sender?: string
  progress?: string
}
export const phases: Text[] = [
  ['准备输入', 'Prepare'], ['预处理', 'Preprocess'], ['检查与预加载', 'Inspect & preload'],
  ['创建实例', 'Create instance'], ['加载与启用', 'Load & enable'], ['完成', 'Finish'],
]
export const scenarios: { value: Scenario; label: Text }[] = [
  { value: 'enabled', label: ['正常加载并启用', 'Load and enable'] },
  { value: 'disabled', label: ['加载但保持禁用', 'Load, remain disabled'] },
  { value: 'sdk-error', label: ['SDK 版本不匹配', 'SDK version mismatch'] },
  { value: 'loaded-error', label: ['Loaded() 抛出异常', 'Loaded() throws'] },
  { value: 'disable-after', label: ['加载后手动禁用', 'Disable after loading'] },
]
const loader = 'AbstractPluginLoader.cs'
const main = 'Processors/MainProcessor.cs'
const pipeline = 'Pipelines/InstallPipeline.cs'
export function createSteps(scenario: Scenario, source: Source): Step[] {
  const steps: Step[] = []
  const add = (id: string, phase: number, title: Text, detail: Text, code: string,
    file: string, extra: Partial<Step> = {}) => steps.push({ id, phase, title, detail, code, file, kind: 'work', ...extra })

  add('plans', 0, ['处理上次的删除与升级计划', 'Apply pending plans'],
    ['主程序先等待删除检查，再等待升级检查。这是 ProcessAsync() 之前的独立调用，不会自动发出 PluginRemoved 或 PluginUpgraded。', 'The host awaits removal, then upgrade checks. This separate call precedes ProcessAsync(); it does not automatically emit PluginRemoved or PluginUpgraded.'],
    'await loader.CheckUpgradeAndRemoveAsync();', 'AbstractPluginLoader.Install.cs')
  add('feed', 0, ['创建流水线并添加输入', 'Create pipeline and feed input'],
    ['Feed() 只收集原料；ProcessAsync() 才开始处理。Feeding 虽有枚举值，默认 Feed() 不报告该进度。', 'Feed() collects materials; ProcessAsync() starts processing. Although Feeding is an enum value, the default Feed() does not report it.'],
    'var pipeline = loader.CreatePipeline();\npipeline.Feed(material);\nawait pipeline.ProcessAsync(progress);', pipeline)
  const inputs: Record<Source, [Text, string, string]> = {
    archive: [['从压缩包读取 plugin.json，返回 CompressedWorkpiece；此时尚未解压完整插件。', 'Read plugin.json from the archive and return a CompressedWorkpiece; full extraction happens later.'], 'CompressedFilePreprocessingProcessor.PreprocessAsync()', 'CompressedFilePreprocessingProcessor.cs'],
    local: [['读取本地 JSON，返回 LocalWorkpiece，后续使用同目录的 DLL。', 'Read local JSON into a LocalWorkpiece; use the DLL in the same directory later.'], 'LocalFilePreprocessingProcessor.PreprocessAsync()', 'LocalFilePreprocessingProcessor.cs'],
    http: [['先下载文件到本地，再按压缩包读取 plugin.json，返回 CompressedWorkpiece。', 'Download the file, then read plugin.json as an archive and return a CompressedWorkpiece.'], 'HttpPreprocessingProcessor.PreprocessAsync()\n  → CompressedFilePreprocessingProcessor.PreprocessAsync()', 'HttpPreprocessingProcessor.cs'],
  }
  const input = inputs[source]
  add('preprocess', 1, ['原料 → 工作件', 'Material → workpiece'], input[0], input[1], `Processors/${input[2]}`,
    { progress: 'Preprocessing / None' })
  add('read', 2, ['读取元数据' + (source === 'local' ? '' : '并解压'), source === 'local' ? 'Read metadata' : 'Read metadata and extract'],
    ['反序列化元数据；压缩工作件在这里解压。批量读取最多并发 6 个，读取失败的工作件会记录错误并被过滤。', 'Deserialize metadata and extract compressed workpieces. Reads allow up to six concurrent workers; failed workpieces are logged and filtered out.'],
    'ReadWorkpiece(workpieces)\n  → MetaDataHelper.ToMeta<TMeta>(...)', main, { progress: 'MainProcessing / ReadWorkpiece' })
  add('sdk', 2, ['检查 SDK 版本', 'Check SDK version'],
    ['检查插件声明的 SdkVersion 是否接受当前 SDK 版本。不匹配会抛出 PluginScanException，尚未创建插件实例。', 'Check the plugin SdkVersion range against the current SDK. A mismatch throws PluginScanException before any plugin instance is created.'],
    'CheckSdkVersion(beforeSorts);', main)
  if (scenario === 'sdk-error') {
    steps[steps.length - 1] = { ...steps[steps.length - 1], kind: 'error', code: 'throw new PluginScanException("… Sdk Version Not Match …");' }
    return steps
  }
  add('sort', 2, ['去重、检查依赖并排序', 'Deduplicate and order dependencies'],
    ['同 ID 选高版本，跳过已加载 DLL；依赖优先，其余按 Priority 从小到大。缺失或不兼容的依赖会抛出 PluginDependencyException。', 'Choose the highest version per ID and skip loaded DLLs. Dependencies come first, otherwise lower Priority first. Missing or incompatible dependencies throw PluginDependencyException.'],
    'DependencyChecker.DetermineLoadOrder(beforeSorts);', 'Checkers/DependencyChecker.cs')
  add('assembly', 2, ['加载程序集', 'Load the assembly'],
    ['通过 WinUI 扩展宿主加载 DLL。多个插件的预加载任务可以并发，图中展示单个插件的调用顺序。', 'Load the DLL through the WinUI extension host. Preload tasks may run concurrently across plugins; this view follows one plugin.'],
    'await ApplicationExtensionHost.Current.LoadExtensionAsync(dllFilePath);', main)
  add('types', 2, ['解析主类与入口点', 'Resolve main type and entry points'],
    ['ToBase() 解析 MainPlugin 和入口点类型。这里只准备类型信息，尚未调用 Loaded()。', 'ToBase() resolves MainPlugin and entry point types. This prepares type information; Loaded() has not run yet.'],
    'sortPluginData.MetaData.ToBase(assembly);', main)
  add('di', 2, ['加载配置并注册 DI', 'Load configuration and register DI'],
    ['加载带 ObservableConfig 标记的配置并注册实例，然后按插件 ID 注册主类单例。配置加载异常会记录日志；预加载进度在 finally 中报告，不代表一定成功。', 'Load attributed ObservableConfig types, register their instances, then register the main class as a keyed singleton. Configuration errors are logged; preload progress is reported in finally, even on failure.'],
    'await Task.WhenAll(configTasks);\nDiFactory.Services.Register(typeof(TAPlugin), mainPlugin,\n    reuse: Reuse.Singleton, serviceKey: pluginId, ...);', main,
    { progress: 'MainProcessing / PluginPreLoad' })
  add('cache', 2, ['缓存元数据并产出', 'Cache metadata and produce output'],
    ['全部预加载任务完成后，缓存 LoadedMetas，并按排序结果返回 BaseProduct。', 'After all preload tasks finish, cache LoadedMetas and return BaseProduct values in the determined order.'],
    'DependencyChecker.LoadedMetas[id] = meta;\nresults.Add(new BaseProduct(id));', main)
  add('outbound', 3, ['按 ID 开始实例加载', 'Load instances by ID'],
    ['Outbound() 调用 Load()，后者按产出顺序取出元数据，再逐个调用 LoadPlugin(meta)。', 'Outbound() calls Load(), which reads cached metadata and invokes LoadPlugin(meta) in product order.'],
    'Outbound(Products, progress)\n  → Load(pluginIds, progress)\n  → LoadPlugin(meta)', 'AbstractPluginLoader.Assignable.cs')
  add('before', 3, ['创建前钩子', 'Before creation hook'],
    ['可重写以准备实例所需服务。此时 DLL 已经加载；它不是加载 DLL 之前的事件。', 'Override to prepare services for the instance. The DLL is already loaded; this is not a pre-DLL event.'],
    'BeforeLoadPlugin(meta.MainPlugin, meta);', loader, { kind: 'callback' })
  add('resolve', 3, ['从 DI 获取插件实例', 'Resolve the plugin instance'],
    ['默认 LoadMainPlugin 按插件 ID 解析单例。首次构造执行基类 Init()，合并 ResourceDictionaries。', 'The default LoadMainPlugin resolves the keyed singleton. First construction runs the base Init() and merges ResourceDictionaries.'],
    'LoadMainPlugin(meta.MainPlugin, meta)\n  → DiFactory.Services.Resolve<TAPlugin>(serviceKey: meta.Id)\n  → constructor → Init()', loader)
  add('after', 3, ['创建后钩子', 'After creation hook'],
    ['实例已创建，可使用传入的 instance；还未放入加载器字典，首次加载时 GetPlugin(id) 尚找不到它。', 'Use the provided instance. It has not been stored in the loader dictionary, so GetPlugin(id) cannot find it on first load.'],
    'AfterLoadPlugin(meta.MainPlugin, instance, meta);', loader, { kind: 'callback' })
  add('store', 4, ['保存实例并读取启用设置', 'Store instance and read settings'],
    ['先保存到 _plugins，再读取持久化启用状态；没有设置时默认为 true。此时尚未按该设置启用插件。', 'Store in _plugins, then read the persisted enabled setting (true when absent). The setting has not yet been applied to the instance.'],
    '_plugins[meta.Id] = instance;\nvar enabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);', loader)
  add('loaded', 4, ['执行加载回调', 'Run the loaded callback'],
    ['调用插件的 Loaded()。回调正常返回后，才会发出公共事件。', 'Invoke Loaded() on the plugin. The public event fires only after the callback returns successfully.'],
    'instance.Loaded();', loader, { kind: 'callback' })
  if (scenario === 'loaded-error') {
    steps[steps.length - 1] = { ...steps[steps.length - 1], kind: 'error',
      detail: ['Loaded() 抛出异常：记录警告并重新抛出，后续事件和 Success 进度均不触发。实例已经保存在 _plugins 中，默认实现不会回滚它。', 'Loaded() throws: log and rethrow; later events and Success progress never fire. The instance is already in _plugins and the default implementation does not roll it back.'],
      code: 'instance.Loaded(); // throws\n// LoadPlugin catches, logs, then rethrows' }
    return steps
  }
  add('loaded-event', 4, ['发出加载完成事件', 'Emit the loaded event'],
    ['主程序可在此更新插件列表。事件由加载器发出，此时还没有自动启用插件。', 'The host can refresh its plugin list. The sender is the loader; automatic enabling has not happened yet.'],
    'PluginEventService.InvokePluginLoaded(this,\n    new PluginEventArgs(meta.Id, PluginStatus.Loaded));', loader,
    { kind: 'event', event: 'PluginLoaded', sender: 'loader' })
  add('record', 4, ['记录已加载依赖', 'Record the loaded dependency'],
    ['使用 DllName 与 Version 更新 LoadedPlugins。该步骤位于 PluginLoaded 之后、自动启用之前。', 'Update LoadedPlugins with DllName and Version, after PluginLoaded and before automatic enabling.'],
    'DependencyChecker.LoadedPlugins.TryAdd(meta.DllName, meta.Version);', loader)
  if (scenario === 'disabled') {
    add('remain-disabled', 4, ['保持禁用', 'Remain disabled'],
      ['保存的启用状态为 false，LoadPlugin 直接返回。不会调用 Enabled()、Disabled()，也不会发出 PluginEnabled 或 PluginDisabled。', 'The saved setting is false, so LoadPlugin returns. Neither Enabled() nor Disabled() runs, and neither state-change event fires.'],
      'if (!enabled) return;', loader)
  } else {
    add('enable', 4, ['恢复启用并执行回调', 'Enable and run the callback'],
      ['IsEnabled 从 false 变为 true，写入设置后调用 Enabled()。只有值发生变化才调用回调并发送事件。', 'IsEnabled changes from false to true, persists the setting, then calls Enabled(). Callbacks and events require an actual state change.'],
      'instance.IsEnabled = true;\n  → Enabled();', 'AbstractPlugin.cs', { kind: 'callback' })
    add('enabled-event', 4, ['发出启用事件', 'Emit the enabled event'],
      ['Enabled() 正常返回后发出；sender 为插件实例。若回调抛出异常，就不会发出该事件。', 'Emitted after Enabled() returns successfully; the sender is the plugin instance. A throwing callback prevents this event.'],
      'PluginEventService.InvokePluginEnabled(this,\n    new PluginEventArgs(Id, PluginStatus.Enabled));', 'AbstractPlugin.cs',
      { kind: 'event', event: 'PluginEnabled', sender: 'instance' })
  }
  add('outbound-progress', 5, ['报告实例加载进度', 'Report instance loading progress'],
    ['LoadPlugin 返回后增加完成计数，报告 Outbounding。进度通知与 IPluginEventService 事件是两个不同通道。', 'After LoadPlugin returns, increment the count and report Outbounding. Progress and IPluginEventService events are separate channels.'],
    'count++;\nprogress?.Report(new PipelineProgress(...));', 'AbstractPluginLoader.Assignable.cs',
    { progress: 'Outbounding / Outbounding' })
  add('success', 5, ['流水线完成', 'Pipeline completed'],
    ['有产出且 Outbound 正常返回时，报告 Success。空输入、无工作件或无产出会提前返回，不报告 Success。', 'Report Success when there are products and Outbound returns normally. Empty input, no workpieces, or no products return early without Success.'],
    'progress?.Report(new PipelineProgress(\n    TotalPercentage: 1D, Step: InstallPipelineStep.Success));', pipeline,
    { progress: 'Success' })
  if (scenario === 'disable-after') {
    add('disable', 5, ['主程序手动禁用', 'The host disables the plugin'],
      ['流水线已经完成。此时主程序调用 DisablePlugin，IsEnabled 从 true 变为 false，先执行 Disabled()。', 'The pipeline has finished. The host now calls DisablePlugin; IsEnabled changes from true to false and runs Disabled() first.'],
      'loader.DisablePlugin(id);\n  → instance.IsEnabled = false;\n  → Disabled();', 'AbstractPlugin.cs', { kind: 'callback' })
    add('disabled-event', 5, ['发出禁用事件', 'Emit the disabled event'],
      ['Disabled() 返回后，由插件实例发出 PluginDisabled。重复禁用不会再发事件。', 'After Disabled() returns, the plugin instance emits PluginDisabled. Repeated disabling emits nothing.'],
      'PluginEventService.InvokePluginDisabled(this,\n    new PluginEventArgs(Id, PluginStatus.Disabled));', 'AbstractPlugin.cs',
      { kind: 'event', event: 'PluginDisabled', sender: 'instance' })
  }
  return steps
}
