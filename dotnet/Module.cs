using GSharp;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using RGiesecke.DllExport;
using System;
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
			ClientConsole.RerouteConsole();
			ClientConsole.Color = new Color(255, 192, 203); // Make it pink, baby <3

			IGameConsole GameConsole = NativeInterface.Load<IGameConsole>("gameui.dll");

			VirtualAction ActivateOld = null;
			ActivateOld = VTable.GetVTable(GameConsole).Hook<VirtualAction>(nameof(IGameConsole.Activate), (This) =>
			{
				Console.WriteLine("The console has been activated!");
				ActivateOld(This);
			});

			Hook_Cmd_Exec Hook_Cmd_Exec_old = null;
			Hook_Cmd_Exec_old = NativeInterface.OverwriteVCRHook<Hook_Cmd_Exec>((Args) =>
			{
				Console.WriteLine("Cmd_Exec:\n{0}", string.Join("\n", Args));
				Hook_Cmd_Exec_old(Args);
			});

			LuaViewer.Spawn(L);
			return 0;
		}

		[DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
		public static int Close(IntPtr L)
		{
			return 0;
		}
	}
}