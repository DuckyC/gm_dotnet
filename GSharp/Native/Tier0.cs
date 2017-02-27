using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GSharp.Native.Classes;

namespace GSharp.Native
{
	public static class Tier0
	{
		const string DllName = "tier0";
		const CallingConvention CConv = CallingConvention.Cdecl;

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConColorMsg(int Lvl, ref Color Clr, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConMsg(int Lvl, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConWarning(int Lvl, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConLog(int Lvl, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConDColorMsg(int Lvl, ref Color Clr, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConDMsg(int Lvl, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConDWarning(int Lvl, string Fmt, string Arg0);

		[DllImport(DllName, CallingConvention = CConv)]
		static extern void ConDLog(int Lvl, string Fmt, string Arg0);

		public static void ConColorMsg(int Lvl, ref Color Clr, string Str) => ConColorMsg(Lvl, ref Clr, "%s", Str);

		public static void ConMsg(int Lvl, string Str) => ConMsg(Lvl, "%s", Str);

		public static void ConWarning(int Lvl, string Str) => ConWarning(Lvl, "%s", Str);

		public static void ConLog(int Lvl, string Str) => ConLog(Lvl, "%s", Str);

		public static void ConDColorMsg(int Lvl, ref Color Clr, string Str) => ConDColorMsg(Lvl, ref Clr, "%s", Str);

		public static void ConDMsg(int Lvl, string Str) => ConDMsg(Lvl, "%s", Str);

		public static void ConDWarning(int Lvl, string Str) => ConDWarning(Lvl, "%s", Str);

		public static void ConDLog(int Lvl, string Str) => ConDLog(Lvl, "%s", Str);
	}
}