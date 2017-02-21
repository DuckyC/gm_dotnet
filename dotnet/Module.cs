using GSharp.NativeClasses;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using GSharp;

namespace dotnet {
	public unsafe static class Module {
		[DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
		public static int Open(lua_state L) {
			IntPtr Tier0;
			IntPtr VCRPtr = InterfaceLoader.LoadVariable("tier0.dll", "g_pVCR", out Tier0);


			VCR_t* VCR = (VCR_t*)VCRPtr;

			// The one showing zeroes
			VCR_t* VCR2 = (VCR_t*)(Tier0.ToInt32() + 0x00035748);

			int* VCR3 = (int*)VCRPtr;
			int[] VCR4 = new int[37];
			for (int i = 0; i < VCR4.Length; i++) {
				VCR4[i] = VCR3[i];
			}

			// Tier0						0x0F6B0000
			// VCR - Tier0					0x000347E0
			// VCR							0x0FAE5748
			// VCR->Hook_recvfrom			0x00BBEEA0
			// VCR->Hook_GetCursorPos		0x00BBEBB0
			// VCR->Hook_ScreenToClient		0x00BBED20
			// VCR->Hook_Cmd_Exec			0x00BBF790

			IntPtr Delta = VCRPtr - Tier0.ToInt32();
			// Tier0						0x0FC90000
			// VCR - Tier0					0x000347E0
			// VCR							0x0FCC47E0
			// VCR->Hook_recvfrom			0x00BBEEA0
			// VCR->Hook_GetCursorPos		0x00BBEBB0
			// VCR->Hook_ScreenToClient		0x00BBED20
			// VCR->Hook_Cmd_Exec			0x00BBF790

			// Tier0						0x0f0f0000

			// VCR							0x0f125748
			// VCR - Tier0					0x00035748

			// VCR->Hook_recvfrom			0x0f125774
			// VCR->Hook_recvfrom - tier0	0x00035774

			// VCR->Hook_GetCursorPos		0x0f125778

			// VCR->Hook_ScreenToClient		0x0f12577c

			// VCR->Hook_Cmd_Exec			0x0f125780


			IntPtr HookCmdExecPtr = Tier0 - 0xEAF0870;

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
