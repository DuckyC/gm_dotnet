using GSharp.Native.Classes;
using GSharp;
using GSharp.LuaLibrary;
using GSharp.Native;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csharp_sandbox
{
	public class CSharpSandbox
	{
		[DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
		public static int Open(lua_state L)
		{
			return 0;
		}

		[DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
		public static int Close(lua_state L)
		{
			return 0;
		}
	}
}