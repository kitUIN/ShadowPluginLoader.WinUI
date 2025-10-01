using System.Linq;
using DryIoc;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <inheritdoc />
public class RemoveChecker : IRemoveChecker
{
    /// <summary>
    /// lock
    /// </summary>
    private readonly object _planRemoveLock = new();

    /// <inheritdoc />
    public bool RemoveChecked { get; private set; }

    /// <inheritdoc />
    public void PlanRemove(string id, string path)
    {
        var inner = DiFactory.Services.Resolve<InnerSdkConfig>();
        lock (_planRemoveLock)
        {
            if (inner.PlanRemove.Any(x => x.Id == id))
                throw new PlanRemoveException($"Plugin[{id}] already plan to remove");
            inner.PlanRemove.Add(new PlanRemoveData()
            {
                Id = id,
                Path = path
            });
        }
    }

    /// <inheritdoc />
    public void RevokedRemove(string id)
    {
        var inner = DiFactory.Services.Resolve<InnerSdkConfig>();
        lock (_planRemoveLock)
        {
            var item = inner.PlanRemove.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                inner.PlanRemove.Remove(item);
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> CheckRemoveAsync()
    {
        RemoveChecked = true;
        await Task.Delay(10);
        return RemoveChecked;
    }
}