﻿using UnhollowerBaseLib;

namespace BloonsTD6.Mod.SalariedMonkeys.Utilities.Hooks;

public struct Il2CppNativeHookVariable<TType> // Either IL2CPP Object or String
{
    /// <summary>
    /// Pointer to the raw object in memory.
    /// </summary>
    public IntPtr RawValue { get; set; }

    /// <summary>
    /// Gets the abstracted type using the pointer.
    /// </summary>
    public TType? GetObject()
    {
        if (RawValue == IntPtr.Zero)
            return default;
        
        return IL2CPP.PointerToValueGeneric<TType>(RawValue, false, false);
    }
}