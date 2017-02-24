using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GSharp.Native.Classes
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct CUtlVector<T> where T : struct
	{
		public IntPtr CUtlVecPtr;

		bool NotNull(int Idx)
		{
			return Element(Idx) != IntPtr.Zero;
		}

		public int GetCount()
		{
			int i;
			for (i = 0; NotNull(i); i++) ;
			return i;
		}

		public IntPtr Element(int Idx)
		{
			IntPtr Mem = Marshal.ReadIntPtr(CUtlVecPtr);
			if (Mem == IntPtr.Zero)
				return IntPtr.Zero;

			int TSize = Marshal.SizeOf(typeof(T));
			return Marshal.ReadIntPtr(Mem, TSize * Idx);
		}

		public IntPtr this[int Idx]
		{
			get
			{
				return Element(Idx);
			}
		}

		public static implicit operator CUtlVector<T>(IntPtr Ptr)
		{
			CUtlVector<T> Vec = new CUtlVector<T>();
			Vec.CUtlVecPtr = Ptr;
			return Vec;
		}
	}
}