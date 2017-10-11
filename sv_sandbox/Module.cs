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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sv_sandbox
{
    public unsafe static class Module
    {
        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(IntPtr L)
        {
            var state = Marshal.PtrToStructure<lua_State>(L);
            var glua = GLua.Get(state);

            glua.CreateTable();

            glua.Push("woop was here");
            glua.SetField(-2, "var");

            glua.Push<CFunc>(RunFunction);
            glua.SetField(-2, "cfunc");

            glua.SetField(GLua.LUA_GLOBALSINDEX, "dotnet");

            Console.WriteLine("DotNet loaded");
            return 0;
        }

        public static int RunFunction(IntPtr L)
        {
            var state = Marshal.PtrToStructure<lua_State>(L);
            var glua = GLua.Get(state);
            var file = glua.WrapLibrary<IFile>("file");
            file.Append("yarp.txt", $"{DateTime.Now.ToShortTimeString()}: Yeaa\n");

            Console.WriteLine($"appended ");
            return 0;
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}