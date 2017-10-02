using GSharp;
using GSharp.GLuaNET;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using RGiesecke.DllExport;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace dotnet
{
    public unsafe static class Module
    {
        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            var glua = new GLua(L);
            var file = new fileImpl(glua);

            var rtn = file.Find("*", "GAME");
            Console.WriteLine("Found: ");
            foreach (var item in rtn.Directories)
            {
                Console.WriteLine(item);
            }
            foreach (var item in rtn.Files)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("DotNet loaded");
            return 0;
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}