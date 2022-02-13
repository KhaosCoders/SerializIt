using System;
using System.Runtime.InteropServices;

namespace SerializIt.Generator.Helpers;

internal static class Caster
{
    public static object CastTo(ref object obj, Type type)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(type));
        Marshal.StructureToPtr(obj, ptr, false);
        return Marshal.PtrToStructure(ptr, type);
    }
}
