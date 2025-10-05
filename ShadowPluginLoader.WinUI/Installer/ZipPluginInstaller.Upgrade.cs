using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Installer;

public partial class ZipPluginInstaller<TMeta>
{
    /// <summary>
    /// lock
    /// </summary>
    private readonly object _planUpgradeLock = new();

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected InnerSdkConfig InnerConfig { get; }


    /// <inheritdoc />
    public void PlanUpgrade(string id, string pluginPath, string zipPath)
    {
        lock (_planUpgradeLock)
        {
            if (InnerConfig.PlanUpgrade.Any(x => x.Id == id))
                throw new PlanUpgradeException($"Plugin[{id}] already plan to upgrade");
            InnerConfig.PlanUpgrade.Add(new PlanUpgradeData()
            {
                Id = id,
                TargetPath = pluginPath,
                ZipPath = zipPath
            });
        }
    }

    /// <inheritdoc />
    public void RevokedUpgrade(string id)
    {
        lock (_planUpgradeLock)
        {
            var item = InnerConfig.PlanUpgrade.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                InnerConfig.PlanUpgrade.Remove(item);
            }
        }
    }

    /// <inheritdoc />
    public Task<bool> CheckUpgradeAsync()
    {
        lock (_planUpgradeLock)
        {
            foreach (var data in InnerConfig.PlanUpgrade.ToArray())
            {
                if (BaseSdkConfig.UpgradeMethod == PluginUpgradeMethod.Remove)
                {
                    Directory.Delete(data.TargetPath, recursive: true);
                }
                UnZip(data.ZipPath, data.TargetPath);
                InnerConfig.PlanUpgrade.Remove(data);
            }
            StaticValues.UpgradeChecked = true;
            return Task.FromResult(StaticValues.UpgradeChecked);
        }
    }
}