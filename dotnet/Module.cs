using GSharp.NativeClasses;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using GSharp;

namespace dotnet {
	public unsafe static class Module {
		[DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
		public static int Open(lua_state L) {
			IntPtr VCRPtr = InterfaceLoader.LoadVariable("tier0.dll", "g_pVCR");
			VCR_t* VCR = (VCR_t*)VCRPtr;
			
			recvfrom_orig = Marshal.GetDelegateForFunctionPointer<Hook_recvfrom>(VCR->Hook_recvfrom);
			VCR->Hook_recvfrom = Marshal.GetFunctionPointerForDelegate<Hook_recvfrom>(Hook_recvfrom_func);

			Console.WriteLine("DotNet loaded");
			return 0;
		}

		static Hook_recvfrom recvfrom_orig;
		public static int Hook_recvfrom_func(int s, IntPtr buf, int len, int flags, IntPtr from, IntPtr fromlen) {
			return recvfrom_orig(s, buf, len, flags, from, fromlen);
		}

		[DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
		public static int Close(IntPtr L) {
			return 0;
		}
	}
}
