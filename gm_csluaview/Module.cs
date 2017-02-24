using GSharp;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.JIT;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


            //var stringTableContainer = NativeInterface.Load<INetworkStringTableContainer>("engine.dll", InterfaceNameStringTable.SERVER);
            //var stringTablePointer = stringTableContainer.FindTable("client_lua_files");
            //var stringTable = JITEngine.GenerateClass<INetworkStringTable>(stringTablePointer);
            //var luaFiles = new List<string>();
            //for (int i = 0; i < stringTable.GetNumStrings(); i++)
            //{
            //    luaFiles.Add(stringTable.GetString(i) ?? "nonexistent stringtable index");
            //}
            byte idx = 0;
            var luaShared = NativeInterface.Load<ILuaShared>("lua_shared.dll");
            var luaInterfacePointer = luaShared.GetLuaInterface(idx);
            if (luaInterfacePointer == IntPtr.Zero)
            {
                Debug.WriteLine($"i {idx}, is invalid");
                return 0;
            }

            var luaInterface = JITEngine.GenerateClass<ILuaInterface>(luaInterfacePointer);
            //var client = luaInterface.IsClient();
            //var server = luaInterface.IsServer();
            //Debug.WriteLine($"i {2}, client {client}, server {server}");
            luaInterface.RunStringEx("", "", "print[[HI THERE FROM RUNSTRING]]");
            //var engineptr = NativeInterface.LoadLibrary("lua_shared.dll");
            //var factoryptr = NativeInterface.GetProcAddress(engineptr, "CreateInterface");
            //var factory = Marshal.GetDelegateForFunctionPointer<CreateInterfaceDelegate>(factoryptr);
            //int successful = 0;
            //var rtnptr = Marshal.AllocHGlobal(successful);
            //var luainterfaceptr = factory("LUASHARED003", rtnptr);
            //successful = Marshal.ReadInt32(rtnptr);

            return 0;
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
