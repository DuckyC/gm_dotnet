using GSharp.NativeClasses;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;

namespace dotnet
{
    public static class Module
    {

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            Console.WriteLine("DotNet loaded");
            return 0;
        }

        [DllExport("gmod13_close")]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
