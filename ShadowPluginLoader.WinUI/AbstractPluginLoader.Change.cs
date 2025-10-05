using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Checkers;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// UpgradeChecker
    /// </summary>
    protected IUpgradeChecker UpgradeChecker { get; }

    /// <summary>
    /// RemoveChecker
    /// </summary>
    protected IRemoveChecker RemoveChecker { get; }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await RemoveChecker.CheckRemoveAsync();
        await UpgradeChecker.CheckUpgradeAsync();
    }
}