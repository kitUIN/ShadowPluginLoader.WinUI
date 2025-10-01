using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <inheritdoc />>
public class RemoveChecker : IRemoveChecker
{
    /// <inheritdoc />>
    public bool RemoveChecked { get; private set; }

    /// <inheritdoc />>
    public async Task<bool> CheckRemoveAsync()
    {
        RemoveChecked = true;
        await Task.Delay(10);
        return RemoveChecked;
    }
}