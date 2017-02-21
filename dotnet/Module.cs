using GSharp.NativeClasses;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using GSharp;

namespace dotnet {
	public unsafe static class Module {
		[DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
		public static int Open(lua_state L) {
			VCR_t* VCR = (VCR_t*)InterfaceLoader.LoadVariable<VCR_t>("tier0.dll", "g_pVCR");
			
			CmdExec_old = Marshal.GetDelegateForFunctionPointer<Hook_Cmd_Exec_func>(VCR->Hook_Cmd_Exec);
			VCR->Hook_Cmd_Exec = Marshal.GetFunctionPointerForDelegate<Hook_Cmd_Exec_func>(CmdExec);

			Console.WriteLine("DotNet loaded");
			return 0;
		}

		static Hook_Cmd_Exec_func CmdExec_old;
		public static void CmdExec(string[] Args) {

			CmdExec_old(Args);
		}

		[DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
		public static int Close(IntPtr L) {
			return 0;
		}
	}
}
