using CustomExtensions.WinUI;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShadowExample;


[MarkupExtensionReturnType(ReturnType = typeof(string))]
public class PluginImage : MarkupExtension
{
    /// <summary>
    /// Gets or sets the <see cref="string"/> representing the image to display.
    /// </summary>
    public string Source { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        MethodBase methodBase = new StackFrame(5, needFileInfo: false)?.GetMethod();
        Debug.WriteLine(methodBase.Name);
        Debug.WriteLine(methodBase.DeclaringType.Name);
        return methodBase.DeclaringType.Assembly.FullName;
    }
}
