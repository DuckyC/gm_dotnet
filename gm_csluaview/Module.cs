using GSharp;
using GSharp.Generated.NativeClasses;
using GSharp.GLuaNET;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.StringTable;
using Libraria.Native;
using RGiesecke.DllExport;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace gm_csluaview
{
    public static class Module
    {
        private static GLua Lua;
        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(ref lua_State state)
        {
            ClientConsole.RerouteConsole();
            ClientConsole.Color = new Color(0, 150, 255);
            Lua = GLua.Get(state);

            Lua.CreateTable();

            Lua.Push<Action<string>>(Dump);
            Lua.SetField(-2, "Dump");

            Lua.SetField(GLua.LUA_GLOBALSINDEX, "csluaview");

            var container = NativeInterface.Load<INetworkStringTableContainer>("engine", StringTableInterfaceName.CLIENT);
            var tablePtr = container.FindTable("client_lua_files");
            var table = JIT.ConvertInstance<INetworkStringTable>(tablePtr);
            Console.WriteLine($"dotnet table ptr: {tablePtr.ToString("X8")}");
            //var path0 = table.GetString(0); //hangs here

            //for (int i = 0; i < table.GetNumStrings(); i++)
            //{
            //}

            //var stringTable = StringTable.FindTable<int>("client_lua_files");
            //var luaFiles = stringTable.Select(s => new LuaFile { Path = s.String, CRC = s.UserData }).ToList();

            Console.WriteLine("DotNet Clientside Lua Viewer Loaded!");
            return 0;
        }

        private static void Dump(string path)
        {
           

            Console.WriteLine(Path.GetFullPath("./Dumps")); 
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
