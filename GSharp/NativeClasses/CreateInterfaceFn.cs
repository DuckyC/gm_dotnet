using System;
using System.Runtime.InteropServices;

namespace GSharp.NativeClasses
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Ansi)]
    public delegate IntPtr CreateInterfaceFn(string version, IntPtr returnCode);
}
