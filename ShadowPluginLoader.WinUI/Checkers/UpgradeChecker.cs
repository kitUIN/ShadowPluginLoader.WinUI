using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// 
/// </summary>
public partial class UpgradeChecker : IUpgradeChecker
{
    /// <summary>
    /// lock
    /// </summary>
    private static readonly SemaphoreSlim PlanUpgradeLock = new SemaphoreSlim(1, 1);

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected InnerSdkConfig InnerConfig { get; }

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected BaseSdkConfig BaseSdkConfig { get; }


    /// <inheritdoc />
    public void PlanUpgrade(string id, string pluginPath, string zipPath)
    {
        PlanUpgradeLock.Wait();
        try
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
        finally
        {
            PlanUpgradeLock.Release();
        }
    }

    /// <inheritdoc />
    public void RevokedUpgrade(string id)
    {
        PlanUpgradeLock.Wait();
        try
        {
            var item = InnerConfig.PlanUpgrade.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                InnerConfig.PlanUpgrade.Remove(item);
            }
        }
        finally
        {
            PlanUpgradeLock.Release();
        }
    }


    /// <inheritdoc />
    public async Task<bool> CheckUpgradeAsync()
    {
        await PlanUpgradeLock.WaitAsync();
        try
        {
            foreach (var data in InnerConfig.PlanUpgrade.ToArray())
            {
                if (BaseSdkConfig.UpgradeMethod == PluginUpgradeMethod.Remove)
                {
                    if (Directory.Exists(data.TargetPath))
                        Directory.Delete(data.TargetPath, recursive: true);
                }

                await ZipHelper.UnZip(data.ZipPath, data.TargetPath);
                InnerConfig.PlanUpgrade.Remove(data);
            }

            StaticValues.UpgradeChecked = true;
            return StaticValues.UpgradeChecked;
        }
        finally
        {
            PlanUpgradeLock.Release();
        }
    }
}