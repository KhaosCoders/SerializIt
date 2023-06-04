using System;
using System.Runtime.InteropServices;

namespace SerializIt.Generator.Helpers;

public static class StructCaster
{
    /// <summary>
    /// Casts an instance of a struct to another struct of the same memory layout
    /// </summary>
    /// <param name="obj">struct to cast</param>
    /// <param name="type">type of struct to cast to</param>
    public static object CastTo(ref object obj, Type type)
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(type));
        try
        {
            Marshal.StructureToPtr(obj, ptr, false);
            return Marshal.PtrToStructure(ptr, type);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
