using System; 
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace WinRT.ShadowPluginLoader_WinUIGenericHelpers
{

    internal static class GlobalVtableLookup
    {

        [System.Runtime.CompilerServices.ModuleInitializer]
        internal static void InitializeGlobalVtableLookup()
        {
            ComWrappersSupport.RegisterTypeComInterfaceEntriesLookup(new Func<Type, ComWrappers.ComInterfaceEntry[]>(LookupVtableEntries));
            ComWrappersSupport.RegisterTypeRuntimeClassNameLookup(new Func<Type, string>(LookupRuntimeClassName));
        }

        private static ComWrappers.ComInterfaceEntry[] LookupVtableEntries(Type type)
        {
            string typeName = type.ToString();
            if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Collections.IEnumerable]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_IEnumerable.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.IDisposableMethods.IID,
                Vtable = global::ABI.System.IDisposableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "System.Collections.ObjectModel.ReadOnlyCollection`1[System.Text.Json.Serialization.JsonConverter]"
            || typeName == "System.Collections.Generic.List`1[System.Text.Json.Serialization.JsonConverter]"
            || typeName == "System.Collections.Generic.List`1[ShadowPluginLoader.WinUI.Models.SortPluginData`1[TMeta]]"
            || typeName == "System.Collections.ObjectModel.ReadOnlyCollection`1[ShadowPluginLoader.WinUI.Models.SortPluginData`1[TMeta]]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IReadOnlyList_object.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_object.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.IID,
                Vtable = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumerableMethods<object>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumerableMethods<object>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.IListMethods.IID,
                Vtable = global::ABI.System.Collections.IListMethods.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.IEnumerableMethods.IID,
                Vtable = global::ABI.System.Collections.IEnumerableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Collections.Generic.IEnumerable`1[System.Char]]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_char.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_Generic_IEnumerable_char_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_IEnumerable.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<char>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<char>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.IDisposableMethods.IID,
                Vtable = global::ABI.System.IDisposableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.String]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_string.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_char.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_Generic_IEnumerable_char_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_object.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_Generic_IEnumerable_object_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_IEnumerable.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_object.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<string>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<string>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<char>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<char>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<object>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<object>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.IDisposableMethods.IID,
                Vtable = global::ABI.System.IDisposableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Collections.Generic.IEnumerable`1[System.Object]]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_object.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_Generic_IEnumerable_object_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_System_Collections_IEnumerable.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<object>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.Generic.IEnumerable<object>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<global::System.Collections.IEnumerable>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.IDisposableMethods.IID,
                Vtable = global::ABI.System.IDisposableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Object]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerator_object.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.IDisposableMethods.IID,
                Vtable = global::ABI.System.IDisposableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "System.Collections.Generic.List`1[System.String]"
            || typeName == "System.Collections.ObjectModel.ReadOnlyCollection`1[System.String]"
            )
            {
                        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IList_string.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IReadOnlyList_string.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_char.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IReadOnlyList_System_Collections_Generic_IEnumerable_char_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_object.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IReadOnlyList_System_Collections_Generic_IEnumerable_object_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IReadOnlyList_System_Collections_IEnumerable.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IReadOnlyList_object.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_string.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_System_Collections_Generic_IEnumerable_char_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_System_Collections_Generic_IEnumerable_object_.Initialized;
        _ = global::WinRT.ShadowPluginLoader_WinUIGenericHelpers.IEnumerable_System_Collections_IEnumerable.Initialized;

        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IListMethods<string>.IID,
                Vtable = global::ABI.System.Collections.Generic.IListMethods<string>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IReadOnlyListMethods<string>.IID,
                Vtable = global::ABI.System.Collections.Generic.IReadOnlyListMethods<string>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IReadOnlyListMethods<global::System.Collections.Generic.IEnumerable<char>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IReadOnlyListMethods<global::System.Collections.Generic.IEnumerable<char>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IReadOnlyListMethods<global::System.Collections.Generic.IEnumerable<object>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IReadOnlyListMethods<global::System.Collections.Generic.IEnumerable<object>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IReadOnlyListMethods<global::System.Collections.IEnumerable>.IID,
                Vtable = global::ABI.System.Collections.Generic.IReadOnlyListMethods<global::System.Collections.IEnumerable>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.IID,
                Vtable = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumerableMethods<string>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumerableMethods<string>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumerableMethods<global::System.Collections.Generic.IEnumerable<char>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumerableMethods<global::System.Collections.Generic.IEnumerable<char>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumerableMethods<global::System.Collections.Generic.IEnumerable<object>>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumerableMethods<global::System.Collections.Generic.IEnumerable<object>>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumerableMethods<global::System.Collections.IEnumerable>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumerableMethods<global::System.Collections.IEnumerable>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.Generic.IEnumerableMethods<object>.IID,
                Vtable = global::ABI.System.Collections.Generic.IEnumerableMethods<object>.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.IListMethods.IID,
                Vtable = global::ABI.System.Collections.IListMethods.AbiToProjectionVftablePtr
            },
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.Collections.IEnumerableMethods.IID,
                Vtable = global::ABI.System.Collections.IEnumerableMethods.AbiToProjectionVftablePtr
            },
};

            }
            if (typeName == "System.Threading.Tasks.Task`1[System.String]"
            )
            {
                
        return new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
        {
            new global::System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
            {
                IID = global::ABI.System.IDisposableMethods.IID,
                Vtable = global::ABI.System.IDisposableMethods.AbiToProjectionVftablePtr
            },
};

            }
            return default;
        }
private static string LookupRuntimeClassName(Type type)
{
    string typeName = type.ToString();
if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Collections.IEnumerable]"
)
{
    return "Windows.Foundation.Collections.IIterator`1<Microsoft.UI.Xaml.Interop.IBindableIterable>";
}
if (typeName == "System.Collections.ObjectModel.ReadOnlyCollection`1[System.Text.Json.Serialization.JsonConverter]"
|| typeName == "System.Collections.Generic.List`1[System.Text.Json.Serialization.JsonConverter]"
|| typeName == "System.Collections.Generic.List`1[ShadowPluginLoader.WinUI.Models.SortPluginData`1[TMeta]]"
|| typeName == "System.Collections.ObjectModel.ReadOnlyCollection`1[ShadowPluginLoader.WinUI.Models.SortPluginData`1[TMeta]]"
)
{
    return "Windows.Foundation.Collections.IVectorView`1<Object>";
}
if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Collections.Generic.IEnumerable`1[System.Char]]"
)
{
    return "Windows.Foundation.Collections.IIterator`1<Windows.Foundation.Collections.IIterable`1<Char>>";
}
if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.String]"
)
{
    return "Windows.Foundation.Collections.IIterator`1<String>";
}
if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Collections.Generic.IEnumerable`1[System.Object]]"
)
{
    return "Windows.Foundation.Collections.IIterator`1<Windows.Foundation.Collections.IIterable`1<Object>>";
}
if (typeName == "ABI.System.Collections.Generic.ToAbiEnumeratorAdapter`1[System.Object]"
)
{
    return "Windows.Foundation.Collections.IIterator`1<Object>";
}
if (typeName == "System.Collections.Generic.List`1[System.String]"
|| typeName == "System.Collections.ObjectModel.ReadOnlyCollection`1[System.String]"
)
{
    return "Windows.Foundation.Collections.IVector`1<String>";
}
if (typeName == "System.Threading.Tasks.Task`1[System.String]"
)
{
    return "Windows.Foundation.IClosable";
}
            return default;
        }
    }
}
