using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace WinRT.ShadowPluginLoader_WinUIGenericHelpers
{

// System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.IEnumerable<object>>
internal static class IReadOnlyList_System_Collections_Generic_IEnumerable_object_
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<object>, IntPtr>.InitCcw(
           &Do_Abi_GetAt_0,
           &Do_Abi_get_Size_1,
           &Do_Abi_IndexOf_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetAt_0(IntPtr thisPtr, uint index, IntPtr* __return_value__)
    {
        System.Collections.Generic.IEnumerable<object> ____return_value__ = default;
        *__return_value__ = default;
        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<object>>.Abi_GetAt_0(thisPtr, index);
            *__return_value__ = MarshalInterface<System.Collections.Generic.IEnumerable<object>>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_IndexOf_2(IntPtr thisPtr, IntPtr value, uint* index, byte* __return_value__)
    {
        bool ____return_value__ = default;
        
        *index = default;
        *__return_value__ = default;
        uint __index = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<object>>.Abi_IndexOf_2(thisPtr, MarshalInterface<System.Collections.Generic.IEnumerable<object>>.FromAbi(value), out __index);
            *index = __index;
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, uint startIndex, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;
         
        *__return_value__ = default;
        System.Collections.Generic.IEnumerable<object>[] __items = MarshalInterface<System.Collections.Generic.IEnumerable<object>>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<object>>.Abi_GetMany_3(thisPtr, startIndex, ref __items);
            MarshalInterface<System.Collections.Generic.IEnumerable<object>>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Size_1(IntPtr thisPtr, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<object>>.Abi_get_Size_1(thisPtr);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerable<char>
internal static class IEnumerable_char
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumerableMethods<char, ushort>.InitCcw(
           &Do_Abi_First_0
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_First_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        *__return_value__ = default;
        try
        {
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IEnumerator<char>>.
               FromManaged(global::ABI.System.Collections.Generic.IEnumerableMethods<char>.Abi_First_0(thisPtr));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerable<string>
internal static class IEnumerable_string
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumerableMethods<string, IntPtr>.InitCcw(
           &Do_Abi_First_0
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_First_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        *__return_value__ = default;
        try
        {
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IEnumerator<string>>.
               FromManaged(global::ABI.System.Collections.Generic.IEnumerableMethods<string>.Abi_First_0(thisPtr));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerator<object>
internal static class IEnumerator_object
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumeratorMethods<object, IntPtr>.InitCcw(
           &Do_Abi_get_Current_0,
           &Do_Abi_get_HasCurrent_1,
           &Do_Abi_MoveNext_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_MoveNext_2(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.Abi_MoveNext_2(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;
        object[] __items = MarshalInspectable<object>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.Abi_GetMany_3(thisPtr, ref __items);
            Marshaler<object>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Current_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        object ____return_value__ = default;
        
        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.Abi_get_Current_0(thisPtr);
            *__return_value__ = MarshalInspectable<object>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_HasCurrent_1(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<object>.Abi_get_HasCurrent_1(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IReadOnlyList<string>
internal static class IReadOnlyList_string
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IReadOnlyListMethods<string, IntPtr>.InitCcw(
           &Do_Abi_GetAt_0,
           &Do_Abi_get_Size_1,
           &Do_Abi_IndexOf_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetAt_0(IntPtr thisPtr, uint index, IntPtr* __return_value__)
    {
        string ____return_value__ = default;
        *__return_value__ = default;
        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<string>.Abi_GetAt_0(thisPtr, index);
            *__return_value__ = MarshalString.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_IndexOf_2(IntPtr thisPtr, IntPtr value, uint* index, byte* __return_value__)
    {
        bool ____return_value__ = default;
        
        *index = default;
        *__return_value__ = default;
        uint __index = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<string>.Abi_IndexOf_2(thisPtr, MarshalString.FromAbi(value), out __index);
            *index = __index;
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, uint startIndex, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;
         
        *__return_value__ = default;
        string[] __items = MarshalString.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<string>.Abi_GetMany_3(thisPtr, startIndex, ref __items);
            Marshaler<string>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Size_1(IntPtr thisPtr, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<string>.Abi_get_Size_1(thisPtr);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerator<global::System.Collections.Generic.IEnumerable<char>>
internal static class IEnumerator_System_Collections_Generic_IEnumerable_char_
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<char>, IntPtr>.InitCcw(
           &Do_Abi_get_Current_0,
           &Do_Abi_get_HasCurrent_1,
           &Do_Abi_MoveNext_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_MoveNext_2(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<char>>.Abi_MoveNext_2(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;
        System.Collections.Generic.IEnumerable<char>[] __items = MarshalInterface<System.Collections.Generic.IEnumerable<char>>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<char>>.Abi_GetMany_3(thisPtr, ref __items);
            MarshalInterface<System.Collections.Generic.IEnumerable<char>>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Current_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        System.Collections.Generic.IEnumerable<char> ____return_value__ = default;
        
        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<char>>.Abi_get_Current_0(thisPtr);
            *__return_value__ = MarshalInterface<System.Collections.Generic.IEnumerable<char>>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_HasCurrent_1(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<char>>.Abi_get_HasCurrent_1(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerator<string>
internal static class IEnumerator_string
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumeratorMethods<string, IntPtr>.InitCcw(
           &Do_Abi_get_Current_0,
           &Do_Abi_get_HasCurrent_1,
           &Do_Abi_MoveNext_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_MoveNext_2(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<string>.Abi_MoveNext_2(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;
        string[] __items = MarshalString.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<string>.Abi_GetMany_3(thisPtr, ref __items);
            Marshaler<string>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Current_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        string ____return_value__ = default;
        
        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<string>.Abi_get_Current_0(thisPtr);
            *__return_value__ = MarshalString.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_HasCurrent_1(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<string>.Abi_get_HasCurrent_1(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerator<global::System.Collections.Generic.IEnumerable<object>>
internal static class IEnumerator_System_Collections_Generic_IEnumerable_object_
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<object>, IntPtr>.InitCcw(
           &Do_Abi_get_Current_0,
           &Do_Abi_get_HasCurrent_1,
           &Do_Abi_MoveNext_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_MoveNext_2(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<object>>.Abi_MoveNext_2(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;
        System.Collections.Generic.IEnumerable<object>[] __items = MarshalInterface<System.Collections.Generic.IEnumerable<object>>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<object>>.Abi_GetMany_3(thisPtr, ref __items);
            MarshalInterface<System.Collections.Generic.IEnumerable<object>>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Current_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        System.Collections.Generic.IEnumerable<object> ____return_value__ = default;
        
        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<object>>.Abi_get_Current_0(thisPtr);
            *__return_value__ = MarshalInterface<System.Collections.Generic.IEnumerable<object>>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_HasCurrent_1(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.Generic.IEnumerable<object>>.Abi_get_HasCurrent_1(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerable<global::System.Collections.Generic.IEnumerable<object>>
internal static class IEnumerable_System_Collections_Generic_IEnumerable_object_
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumerableMethods<System.Collections.Generic.IEnumerable<object>, IntPtr>.InitCcw(
           &Do_Abi_First_0
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_First_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        *__return_value__ = default;
        try
        {
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IEnumerator<System.Collections.Generic.IEnumerable<object>>>.
               FromManaged(global::ABI.System.Collections.Generic.IEnumerableMethods<System.Collections.Generic.IEnumerable<object>>.Abi_First_0(thisPtr));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerable<global::System.Collections.Generic.IEnumerable<char>>
internal static class IEnumerable_System_Collections_Generic_IEnumerable_char_
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumerableMethods<System.Collections.Generic.IEnumerable<char>, IntPtr>.InitCcw(
           &Do_Abi_First_0
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_First_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        *__return_value__ = default;
        try
        {
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IEnumerator<System.Collections.Generic.IEnumerable<char>>>.
               FromManaged(global::ABI.System.Collections.Generic.IEnumerableMethods<System.Collections.Generic.IEnumerable<char>>.Abi_First_0(thisPtr));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IReadOnlyList<global::System.Collections.IEnumerable>
internal static class IReadOnlyList_System_Collections_IEnumerable
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.IEnumerable, IntPtr>.InitCcw(
           &Do_Abi_GetAt_0,
           &Do_Abi_get_Size_1,
           &Do_Abi_IndexOf_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetAt_0(IntPtr thisPtr, uint index, IntPtr* __return_value__)
    {
        System.Collections.IEnumerable ____return_value__ = default;
        *__return_value__ = default;
        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.IEnumerable>.Abi_GetAt_0(thisPtr, index);
            *__return_value__ = MarshalInterface<System.Collections.IEnumerable>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_IndexOf_2(IntPtr thisPtr, IntPtr value, uint* index, byte* __return_value__)
    {
        bool ____return_value__ = default;
        
        *index = default;
        *__return_value__ = default;
        uint __index = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.IEnumerable>.Abi_IndexOf_2(thisPtr, MarshalInterface<System.Collections.IEnumerable>.FromAbi(value), out __index);
            *index = __index;
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, uint startIndex, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;
         
        *__return_value__ = default;
        System.Collections.IEnumerable[] __items = MarshalInterface<System.Collections.IEnumerable>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.IEnumerable>.Abi_GetMany_3(thisPtr, startIndex, ref __items);
            MarshalInterface<System.Collections.IEnumerable>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Size_1(IntPtr thisPtr, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.IEnumerable>.Abi_get_Size_1(thisPtr);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IList<string>
internal static class IList_string
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IListMethods<string, IntPtr>.InitCcw(
           &Do_Abi_GetAt_0,
           &Do_Abi_get_Size_1,
           &Do_Abi_GetView_2,
           &Do_Abi_IndexOf_3,
           &Do_Abi_SetAt_4,
           &Do_Abi_InsertAt_5,
           &Do_Abi_RemoveAt_6,
           &Do_Abi_Append_7,
           &Do_Abi_RemoveAtEnd_8,
           &Do_Abi_Clear_9,
           &Do_Abi_GetMany_10,
           &Do_Abi_ReplaceAll_11
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetAt_0(IntPtr thisPtr, uint index, IntPtr* __return_value__)
    {
        string ____return_value__ = default;
        *__return_value__ = default;
        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IListMethods<string>.Abi_GetAt_0(thisPtr, index);
            *__return_value__ = MarshalString.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetView_2(IntPtr thisPtr, IntPtr* __return_value__)
    {
        global::System.Collections.Generic.IReadOnlyList<string> ____return_value__ = default;
        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IListMethods<string>.Abi_GetView_2(thisPtr);
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IReadOnlyList<string>>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_IndexOf_3(IntPtr thisPtr, IntPtr value, uint* index, byte* __return_value__)
    {
        bool ____return_value__ = default;
        
        *index = default;
        *__return_value__ = default;
        uint __index = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IListMethods<string>.Abi_IndexOf_3(thisPtr, MarshalString.FromAbi(value), out __index);
            *index = __index;
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_SetAt_4(IntPtr thisPtr, uint index, IntPtr value)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_SetAt_4(thisPtr, index, MarshalString.FromAbi(value));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_InsertAt_5(IntPtr thisPtr, uint index, IntPtr value)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_InsertAt_5(thisPtr, index, MarshalString.FromAbi(value));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_RemoveAt_6(IntPtr thisPtr, uint index)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_RemoveAt_6(thisPtr, index);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_Append_7(IntPtr thisPtr, IntPtr value)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_Append_7(thisPtr, MarshalString.FromAbi(value));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_RemoveAtEnd_8(IntPtr thisPtr)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_RemoveAtEnd_8(thisPtr);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_Clear_9(IntPtr thisPtr)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_Clear_9(thisPtr);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_10(IntPtr thisPtr, uint startIndex, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;
         
        *__return_value__ = default;
        string[] __items = MarshalString.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IListMethods<string>.Abi_GetMany_10(thisPtr, startIndex, ref __items);
            Marshaler<string>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_ReplaceAll_11(IntPtr thisPtr, int __itemsSize, IntPtr items)
    {
        try
        {
            global::ABI.System.Collections.Generic.IListMethods<string>.Abi_ReplaceAll_11(thisPtr, MarshalString.FromAbiArray((__itemsSize, items)));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Size_1(IntPtr thisPtr, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IListMethods<string>.Abi_get_Size_1(thisPtr);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.IEnumerable<char>>
internal static class IReadOnlyList_System_Collections_Generic_IEnumerable_char_
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<char>, IntPtr>.InitCcw(
           &Do_Abi_GetAt_0,
           &Do_Abi_get_Size_1,
           &Do_Abi_IndexOf_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetAt_0(IntPtr thisPtr, uint index, IntPtr* __return_value__)
    {
        System.Collections.Generic.IEnumerable<char> ____return_value__ = default;
        *__return_value__ = default;
        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<char>>.Abi_GetAt_0(thisPtr, index);
            *__return_value__ = MarshalInterface<System.Collections.Generic.IEnumerable<char>>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_IndexOf_2(IntPtr thisPtr, IntPtr value, uint* index, byte* __return_value__)
    {
        bool ____return_value__ = default;
        
        *index = default;
        *__return_value__ = default;
        uint __index = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<char>>.Abi_IndexOf_2(thisPtr, MarshalInterface<System.Collections.Generic.IEnumerable<char>>.FromAbi(value), out __index);
            *index = __index;
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, uint startIndex, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;
         
        *__return_value__ = default;
        System.Collections.Generic.IEnumerable<char>[] __items = MarshalInterface<System.Collections.Generic.IEnumerable<char>>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<char>>.Abi_GetMany_3(thisPtr, startIndex, ref __items);
            MarshalInterface<System.Collections.Generic.IEnumerable<char>>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Size_1(IntPtr thisPtr, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<System.Collections.Generic.IEnumerable<char>>.Abi_get_Size_1(thisPtr);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerable<global::System.Collections.IEnumerable>
internal static class IEnumerable_System_Collections_IEnumerable
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumerableMethods<System.Collections.IEnumerable, IntPtr>.InitCcw(
           &Do_Abi_First_0
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_First_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        *__return_value__ = default;
        try
        {
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IEnumerator<System.Collections.IEnumerable>>.
               FromManaged(global::ABI.System.Collections.Generic.IEnumerableMethods<System.Collections.IEnumerable>.Abi_First_0(thisPtr));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerable<object>
internal static class IEnumerable_object
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumerableMethods<object, IntPtr>.InitCcw(
           &Do_Abi_First_0
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_First_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        *__return_value__ = default;
        try
        {
            *__return_value__ = MarshalInterface<global::System.Collections.Generic.IEnumerator<object>>.
               FromManaged(global::ABI.System.Collections.Generic.IEnumerableMethods<object>.Abi_First_0(thisPtr));
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IEnumerator<global::System.Collections.IEnumerable>
internal static class IEnumerator_System_Collections_IEnumerable
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.IEnumerable, IntPtr>.InitCcw(
           &Do_Abi_get_Current_0,
           &Do_Abi_get_HasCurrent_1,
           &Do_Abi_MoveNext_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_MoveNext_2(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.IEnumerable>.Abi_MoveNext_2(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;
        System.Collections.IEnumerable[] __items = MarshalInterface<System.Collections.IEnumerable>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.IEnumerable>.Abi_GetMany_3(thisPtr, ref __items);
            MarshalInterface<System.Collections.IEnumerable>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Current_0(IntPtr thisPtr, IntPtr* __return_value__)
    {
        System.Collections.IEnumerable ____return_value__ = default;
        
        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.IEnumerable>.Abi_get_Current_0(thisPtr);
            *__return_value__ = MarshalInterface<System.Collections.IEnumerable>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_HasCurrent_1(IntPtr thisPtr, byte* __return_value__)
    {
        bool ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IEnumeratorMethods<System.Collections.IEnumerable>.Abi_get_HasCurrent_1(thisPtr);
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}

// System.Collections.Generic.IReadOnlyList<object>
internal static class IReadOnlyList_object
{
    private static readonly bool _initialized = Init();
    internal static bool Initialized => _initialized;

    private static unsafe bool Init()
    {
        return global::ABI.System.Collections.Generic.IReadOnlyListMethods<object, IntPtr>.InitCcw(
           &Do_Abi_GetAt_0,
           &Do_Abi_get_Size_1,
           &Do_Abi_IndexOf_2,
           &Do_Abi_GetMany_3
        );
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetAt_0(IntPtr thisPtr, uint index, IntPtr* __return_value__)
    {
        object ____return_value__ = default;
        *__return_value__ = default;
        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.Abi_GetAt_0(thisPtr, index);
            *__return_value__ = MarshalInspectable<object>.FromManaged(____return_value__);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_IndexOf_2(IntPtr thisPtr, IntPtr value, uint* index, byte* __return_value__)
    {
        bool ____return_value__ = default;
        
        *index = default;
        *__return_value__ = default;
        uint __index = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.Abi_IndexOf_2(thisPtr, MarshalInspectable<object>.FromAbi(value), out __index);
            *index = __index;
            *__return_value__ = (byte)(____return_value__ ? 1 : 0);
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_GetMany_3(IntPtr thisPtr, uint startIndex, int __itemsSize, IntPtr items, uint* __return_value__)
    {
        uint ____return_value__ = default;
         
        *__return_value__ = default;
        object[] __items = MarshalInspectable<object>.FromAbiArray((__itemsSize, items));

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.Abi_GetMany_3(thisPtr, startIndex, ref __items);
            Marshaler<object>.CopyManagedArray(__items, items);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe int Do_Abi_get_Size_1(IntPtr thisPtr, uint* __return_value__)
    {
        uint ____return_value__ = default;

        *__return_value__ = default;

        try
        {
            ____return_value__ = global::ABI.System.Collections.Generic.IReadOnlyListMethods<object>.Abi_get_Size_1(thisPtr);
            *__return_value__ = ____return_value__;
        }
        catch (global::System.Exception __exception__)
        {
            global::WinRT.ExceptionHelpers.SetErrorInfo(__exception__);
            return global::WinRT.ExceptionHelpers.GetHRForException(__exception__);
        }
        return 0;
    }
}
}
