using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GSharp.Native.Classes
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CUtlVector
	{
		public IntPtr CUtlVecPtr;

		public IntPtr this[int Idx]
		{
			get
			{
				IntPtr Mem = Marshal.ReadIntPtr(CUtlVecPtr);
				if (Mem == IntPtr.Zero)
					return IntPtr.Zero;

				return Marshal.ReadIntPtr(Mem, IntPtr.Size * Idx);
			}

			set
			{
				IntPtr Mem = Marshal.ReadIntPtr(CUtlVecPtr);
				if (Mem == IntPtr.Zero)
					return;

				Marshal.WriteIntPtr(Mem, IntPtr.Size * Idx, value);
			}
		}

		public static implicit operator CUtlVector(IntPtr Ptr)
		{
			CUtlVector Vec = new CUtlVector();
			Vec.CUtlVecPtr = Ptr;
			return Vec;
		}
	}
}