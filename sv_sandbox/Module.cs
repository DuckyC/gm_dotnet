using GSharp;
using GSharp.Generated.LuaLibraries;
using GSharp.Generated.NativeClasses;
using GSharp.GLuaNET;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using Libraria.Native;
using RGiesecke.DllExport;
using SDILReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace sv_sandbox
{
    public unsafe static class Module
    {
        private static GLua GLua;

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(IntPtr L)
        {
            var state = Marshal.PtrToStructure<lua_State>(L);
            GLua = GLua.Get(state);

            GLua.CreateTable();

            GLua.Push("woop was here");
            GLua.SetField(-2, "var");

            GLua.Push<Func<string, int, string>>(DoAThing);
            GLua.SetField(-2, "wrapped");

            GLua.SetField(GLua.LUA_GLOBALSINDEX, "dotnet");

            Console.WriteLine("DotNet loaded");
            return 0;
        }

        public static string DoAThing(string str, int num)
        {
            return string.Concat(Enumerable.Repeat(str, num));
        }
      

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}