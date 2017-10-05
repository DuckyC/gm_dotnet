using GSharp;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.StringTable;
using RGiesecke.DllExport;
using System;
using System.Linq;
using System.Runtime.InteropServices;


namespace gm_csluaview
{


    public unsafe class Module
    {
        public static void RunString(string code)
        {
            var luaShared = NativeInterface.Load<ILuaShared>("lua_shared.dll");
            var luaInterfacePointer = luaShared.GetLuaInterface(0);
            var luaInterface = JITEngine.GenerateClass<ILuaInterface>(luaInterfacePointer);
            luaInterface.RunStringEx("", "", code);
        }

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            ClientConsole.RerouteConsole();
            ClientConsole.Color = new Color(0, 150, 255);
            Console.WriteLine("DotNet Clientside Lua Viewer Loaded!");

            var stringTable = StringTable.FindTable<int>("client_lua_files");
            var luaFiles = stringTable.Select(s => new LuaFile { Path = s.String, CRC = s.UserData }).ToList();



            return 0;
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
