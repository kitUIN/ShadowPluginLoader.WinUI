using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// Configuration change event arguments containing property change information and file information
/// </summary>
/// <param name="PropertyName">The name of the changed property</param>
/// <param name="OldValue">The old value of the property</param>
/// <param name="NewValue">The new value of the property</param>
/// <param name="PropertyType">The type of the property</param>
public record ConfigChangedEventArgs(
    string PropertyName,
    object OldValue,
    object NewValue,
    Type PropertyType
);

/// <summary>
/// Base configuration class providing property change notification and configuration change event functionality
/// Inherits from INotifyPropertyChanged to support data binding
/// </summary>
public abstract class BaseConfig : INotifyPropertyChanged
{
    private static readonly ISerializer _serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    /// <summary>
    /// The name of the configuration file
    /// </summary>
    protected abstract string FileName { get; }
    /// <summary>
    /// The name of the configuration directory
    /// </summary>
    protected abstract string DirectoryName { get; }


    /// <summary>
    /// The path of the configuration file
    /// </summary>
    protected abstract string ConfigPath { get; }
    /// <summary>
    /// Property changed event that is triggered when any property value changes
    /// Used to support WPF/WinUI data binding mechanism
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Configuration changed event that is triggered when configuration properties change
    /// Contains detailed change information and file path information
    /// </summary>
    public event EventHandler<ConfigChangedEventArgs> ConfigChanged;

    /// <summary>
    /// Triggers the property changed event
    /// Call this method when property values change to notify UI updates
    /// </summary>
    /// <param name="propertyName">The name of the changed property, usually provided automatically by the compiler</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Triggers the configuration changed event
    /// Call this method when configuration properties change, providing detailed change information and file path
    /// </summary>
    /// <param name="propertyName">The name of the changed property</param>
    /// <param name="oldValue">The old value of the property</param>
    /// <param name="newValue">The new value of the property</param>
    /// <param name="propertyType">The type of the property</param>
    protected void OnConfigChanged(string propertyName, object oldValue, object newValue, Type propertyType)
    {
        if (ConfigChanged != null)
        {
            var args = new ConfigChangedEventArgs(
                propertyName,
                oldValue,
                newValue,
                propertyType
            );

            ConfigChanged.Invoke(this, args);
        }
    }

    /// <summary>
    /// Saves the current configuration to a YAML file synchronously
    /// </summary>
    public virtual void SaveToYaml()
    {
        try
        {
            var yaml = _serializer.Serialize(this);
            File.WriteAllText(ConfigPath, yaml);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save configuration to YAML file: {ConfigPath}", ex);
        }
    }

    /// <summary>
    /// Loads configuration from a YAML file synchronously
    /// </summary>
    /// <typeparam name="T">The type of configuration to load</typeparam>
    /// <returns>The loaded configuration</returns>
    public T LoadFromYaml<T>() where T : BaseConfig
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                throw new FileNotFoundException($"Configuration file not found: {ConfigPath}");
            }

            var yaml = File.ReadAllText(ConfigPath);
            return _deserializer.Deserialize<T>(yaml);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load configuration from YAML file: {ConfigPath}", ex);
        }
    }


    /// <summary>
    /// Loads configuration from a YAML file or creates a new instance if the file doesn't exist (synchronous)
    /// </summary>
    /// <typeparam name="T">The type of configuration to load or create</typeparam>
    /// <returns>The loaded or new configuration</returns>
    public T LoadFromYamlOrCreate<T>() where T : BaseConfig, new()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                return LoadFromYaml<T>();
            }
            else
            {
                return new T();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load or create configuration: {ConfigPath}", ex);
        }
    }
}