using System;
using System.Runtime.InteropServices;

namespace GSharp.Native.Classes
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr CreateInterfaceDelegate(string version, IntPtr returnCode);
}
