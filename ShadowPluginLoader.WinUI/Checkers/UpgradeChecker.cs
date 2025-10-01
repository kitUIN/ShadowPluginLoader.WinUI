using System;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <inheritdoc />
public class UpgradeChecker : IUpgradeChecker
{
    /// <summary>
    /// lock
    /// </summary>
    private readonly object _planUpgradeLock = new();

    /// <inheritdoc />
    public bool UpgradeChecked { get; private set; }


    /// <inheritdoc />
    public void PlanUpgrade(string id, string pluginPath, string zipPath)
    {
        var inner = DiFactory.Services.Resolve<InnerSdkConfig>();
        lock (_planUpgradeLock)
        {
            if (inner.PlanUpgrade.Any(x => x.Id == id))
                throw new PlanUpgradeException($"Plugin[{id}] already plan to upgrade");
            inner.PlanUpgrade.Add(new PlanUpgradeData()
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
        var inner = DiFactory.Services.Resolve<InnerSdkConfig>();
        lock (_planUpgradeLock)
        {
            var item = inner.PlanUpgrade.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                inner.PlanUpgrade.Remove(item);
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> CheckUpgradeAsync()
    {
        var inner = DiFactory.Services.Resolve<InnerSdkConfig>();
        inner.PlanUpgrade.Clear();
        UpgradeChecked = true;
        await Task.Delay(10);
        return UpgradeChecked;
    }
}