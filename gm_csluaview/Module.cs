using GSharp;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.JIT;
using RGiesecke.DllExport;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace gm_csluaview
{
    public class Module
    {
        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            ClientConsole.RerouteConsole(); // route Console.WriteLine to ingame Console
            Console.WriteLine("Testing ingame console from C#!");


            var stringTable = StringTable.FindTable("client_lua_files");
            var lua_files = stringTable.Cast<string>();

            var luaShared = NativeInterface.Load<ILuaShared>("lua_shared.dll");
            var luaInterfacePointer = luaShared.GetLuaInterface(0);
            var luaInterface = JITEngine.GenerateClass<ILuaInterface>(luaInterfacePointer);
            luaInterface.RunStringEx("", "", "print[[HI THERE FROM RUNSTRING]]");

            return 0;
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
