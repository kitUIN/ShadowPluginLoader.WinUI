using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <inheritdoc />>
public class UpgradeChecker : IUpgradeChecker
{
    /// <inheritdoc />>
    public bool UpgradeChecked { get; private set; }

    /// <inheritdoc />>
    public async Task<bool> CheckUpgradeAsync()
    {
        UpgradeChecked = true;
        await Task.Delay(10);
        return UpgradeChecked;
    }
}