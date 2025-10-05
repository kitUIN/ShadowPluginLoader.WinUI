using System.IO;
using System.Linq;
using DryIoc;
using System.Threading.Tasks;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <inheritdoc />
public partial class RemoveChecker : IRemoveChecker
{
    /// <summary>
    /// lock
    /// </summary>
    private readonly object _planRemoveLock = new();

    /// <inheritdoc />
    public bool RemoveChecked { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected InnerSdkConfig InnerConfig { get; }

    /// <inheritdoc />
    public void PlanRemove(string id, string path)
    {
        lock (_planRemoveLock)
        {
            if (InnerConfig.PlanRemove.Any(x => x.Id == id))
                throw new PlanRemoveException($"Plugin[{id}] already plan to remove");
            InnerConfig.PlanRemove.Add(new PlanRemoveData()
            {
                Id = id,
                Path = path
            });
        }
    }

    /// <inheritdoc />
    public void RevokedRemove(string id)
    {
        lock (_planRemoveLock)
        {
            var item = InnerConfig.PlanRemove.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                InnerConfig.PlanRemove.Remove(item);
            }
        }
    }

    /// <inheritdoc />
    public Task<bool> CheckRemoveAsync()
    {
        lock (_planRemoveLock)
        {
            RemoveChecked = true;
            foreach (var plan in InnerConfig.PlanRemove.ToArray())
            {
                Directory.Delete(plan.Path, recursive: true);
                InnerConfig.PlanRemove.Remove(plan);
            }
            return Task.FromResult(RemoveChecked);
        }
    }
}