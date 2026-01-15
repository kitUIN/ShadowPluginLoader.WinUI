namespace ShadowPluginLoader.WinUI.Products;

/// <summary>
/// 
/// </summary>
public class BaseProduct : IProduct
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public BaseProduct(string id)
    {
        Id = id;
    }

    /// <inheritdoc/>
    public string Id { get; }
}