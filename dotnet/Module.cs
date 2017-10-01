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
    /*static class cpp_sandbox
    {
        [DllImport("cpp_sandbox")]
        public static extern void cpp_test();
    }*/

    public unsafe static class Module
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void VirtualAction(IntPtr This);

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            
            var glua = new GLua(L);

#if CLIENT
            ClientConsole.RerouteConsole();
            ClientConsole.Color = new Color(255, 192, 203); // Make it pink, baby <3
            IGameConsole GameConsole = NativeInterface.Load<IGameConsole>("gameui.dll");

            VirtualAction ActivateOld = null;
            ActivateOld = VTable.GetVTable(GameConsole).Hook<VirtualAction>(nameof(IGameConsole.Activate), (This) =>
            {
                Console.WriteLine("The console has been activated!");
                ActivateOld(This);
            });
#endif

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