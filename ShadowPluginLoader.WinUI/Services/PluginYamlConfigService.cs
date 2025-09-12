using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Serilog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// 插件YAML配置管理服务
/// </summary>
public class PluginYamlConfigService
{
    private readonly ILogger _logger;
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;
    private readonly Dictionary<string, object> _configCache = new();
    private readonly Dictionary<string, string> _configFilePaths = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// 配置变更事件
    /// </summary>
    public event EventHandler<PluginConfigChangedEventArgs>? ConfigChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public PluginYamlConfigService(ILogger logger)
    {
        _logger = logger;
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    /// <summary>
    /// 注册插件配置文件路径
    /// </summary>
    /// <param name="pluginId">插件ID</param>
    /// <param name="configFilePath">配置文件路径</param>
    public void RegisterPluginConfig(string pluginId, string configFilePath)
    {
        lock (_lockObject)
        {
            _configFilePaths[pluginId] = configFilePath;
            _logger.Information("插件 {PluginId} 配置文件已注册: {ConfigPath}", pluginId, configFilePath);
        }
    }

    /// <summary>
    /// 获取插件配置
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="pluginId">插件ID</param>
    /// <param name="defaultConfig">默认配置</param>
    /// <returns>插件配置</returns>
    public T GetPluginConfig<T>(string pluginId, T defaultConfig = default) where T : class, new()
    {
        lock (_lockObject)
        {
            try
            {
                if (!_configFilePaths.TryGetValue(pluginId, out var configPath))
                {
                    _logger.Warning("插件 {PluginId} 未注册配置文件", pluginId);
                    return defaultConfig ?? new T();
                }

                if (!File.Exists(configPath))
                {
                    _logger.Information("插件 {PluginId} 配置文件不存在，创建默认配置", pluginId);
                    var newConfig = defaultConfig ?? new T();
                    SavePluginConfig(pluginId, newConfig);
                    return newConfig;
                }

                var yamlContent = File.ReadAllText(configPath);
                var config = _deserializer.Deserialize<T>(yamlContent);
                
                // 缓存配置
                _configCache[pluginId] = config;
                
                _logger.Debug("成功加载插件 {PluginId} 配置", pluginId);
                return config ?? defaultConfig ?? new T();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "加载插件 {PluginId} 配置失败", pluginId);
                return defaultConfig ?? new T();
            }
        }
    }

    /// <summary>
    /// 保存插件配置
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="pluginId">插件ID</param>
    /// <param name="config">配置对象</param>
    public void SavePluginConfig<T>(string pluginId, T config) where T : class
    {
        lock (_lockObject)
        {
            try
            {
                if (!_configFilePaths.TryGetValue(pluginId, out var configPath))
                {
                    _logger.Warning("插件 {PluginId} 未注册配置文件路径", pluginId);
                    return;
                }

                // 确保目录存在
                var directory = Path.GetDirectoryName(configPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var yamlContent = _serializer.Serialize(config);
                File.WriteAllText(configPath, yamlContent);

                // 更新缓存
                _configCache[pluginId] = config;

                _logger.Information("插件 {PluginId} 配置已保存到 {ConfigPath}", pluginId, configPath);

                // 触发配置变更事件
                ConfigChanged?.Invoke(this, new PluginConfigChangedEventArgs(pluginId, configPath));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "保存插件 {PluginId} 配置失败", pluginId);
                throw;
            }
        }
    }

    /// <summary>
    /// 异步保存插件配置
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="pluginId">插件ID</param>
    /// <param name="config">配置对象</param>
    public async Task SavePluginConfigAsync<T>(string pluginId, T config) where T : class
    {
        await Task.Run(() => SavePluginConfig(pluginId, config));
    }

    /// <summary>
    /// 更新插件配置（立即保存）
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="pluginId">插件ID</param>
    /// <param name="updateAction">更新操作</param>
    public void UpdatePluginConfig<T>(string pluginId, Action<T> updateAction) where T : class, new()
    {
        lock (_lockObject)
        {
            var config = GetPluginConfig<T>(pluginId);
            updateAction(config);
            SavePluginConfig(pluginId, config);
        }
    }

    /// <summary>
    /// 异步更新插件配置（立即保存）
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="pluginId">插件ID</param>
    /// <param name="updateAction">更新操作</param>
    public async Task UpdatePluginConfigAsync<T>(string pluginId, Action<T> updateAction) where T : class, new()
    {
        await Task.Run(() => UpdatePluginConfig(pluginId, updateAction));
    }

    /// <summary>
    /// 检查配置文件是否存在
    /// </summary>
    /// <param name="pluginId">插件ID</param>
    /// <returns>配置文件是否存在</returns>
    public bool ConfigFileExists(string pluginId)
    {
        lock (_lockObject)
        {
            return _configFilePaths.TryGetValue(pluginId, out var configPath) && File.Exists(configPath);
        }
    }

    /// <summary>
    /// 获取配置文件路径
    /// </summary>
    /// <param name="pluginId">插件ID</param>
    /// <returns>配置文件路径</returns>
    public string? GetConfigFilePath(string pluginId)
    {
        lock (_lockObject)
        {
            return _configFilePaths.TryGetValue(pluginId, out var configPath) ? configPath : null;
        }
    }

    /// <summary>
    /// 删除插件配置
    /// </summary>
    /// <param name="pluginId">插件ID</param>
    public void RemovePluginConfig(string pluginId)
    {
        lock (_lockObject)
        {
            if (_configFilePaths.TryGetValue(pluginId, out var configPath) && File.Exists(configPath))
            {
                try
                {
                    File.Delete(configPath);
                    _logger.Information("已删除插件 {PluginId} 配置文件: {ConfigPath}", pluginId, configPath);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "删除插件 {PluginId} 配置文件失败", pluginId);
                }
            }

            _configFilePaths.Remove(pluginId);
            _configCache.Remove(pluginId);
        }
    }
}

/// <summary>
/// 插件配置变更事件参数
/// </summary>
public class PluginConfigChangedEventArgs : EventArgs
{
    /// <summary>
    /// 插件ID
    /// </summary>
    public string PluginId { get; }

    /// <summary>
    /// 配置文件路径
    /// </summary>
    public string ConfigFilePath { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="pluginId">插件ID</param>
    /// <param name="configFilePath">配置文件路径</param>
    public PluginConfigChangedEventArgs(string pluginId, string configFilePath)
    {
        PluginId = pluginId;
        ConfigFilePath = configFilePath;
    }
}
