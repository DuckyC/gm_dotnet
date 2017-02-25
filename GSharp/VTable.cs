using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using GSharp.Attributes;
using GSharp.Native;

namespace GSharp
{
	public unsafe class VTable
	{
		public static VTable GetVTable(object Interface)
		{
			Type ObjType = Interface.GetType();
			FieldInfo VTableField = ObjType.GetField(nameof(VTable));
			if (VTableField == null || VTableField.FieldType != typeof(VTable))
				throw new Exception(Interface.ToString() + " not a valid interface type");

			VTable VT = (VTable)VTableField.GetValue(Interface);
			if (VT == null)
				VTableField.SetValue(Interface, VT = new VTable(Interface));

			return VT;
		}

		protected object Object;
		protected Type ObjectType;
		protected Type InterfaceType;

		protected IntPtr ObjectAddress;
		protected IntPtr* VTbl;

		protected Dictionary<int, IntPtr> Originals;
		protected List<GCHandle> NewHookHandles;

		private VTable(object Object)
		{
			Originals = new Dictionary<int, IntPtr>();
			NewHookHandles = new List<GCHandle>();

			this.Object = Object;
			ObjectType = Object.GetType();
			InterfaceType = ObjectType.GetInterfaces().First();

			ObjectAddress = (IntPtr)ObjectType.GetField("ObjectAddress").GetValue(Object);
			VTbl = *(IntPtr**)ObjectAddress;
		}

		public int GetIdxForName(string Name)
		{
			MethodInfo MethInfo = InterfaceType.GetMethod(Name);
			return MethInfo.GetCustomAttribute<VTableSlotAttribute>().Slot;
		}

		public IntPtr Hook(int Idx, IntPtr New)
		{
			if (!Originals.ContainsKey(Idx))
				Originals.Add(Idx, VTbl[Idx]);

			Protection P = Protection.PAGE_READWRITE;
			Kernel32.VirtualProtect((IntPtr)(&VTbl[Idx]), IntPtr.Size, P, out P);

			IntPtr Old = VTbl[Idx];
			VTbl[Idx] = New;
			return Old;
		}

		public IntPtr Hook(string Name, IntPtr New)
		{
			return Hook(GetIdxForName(Name), New);
		}

		public T Hook<T>(int Idx, T New) where T : class
		{
			NewHookHandles.Add(GCHandle.Alloc(New));
			return Marshal.GetDelegateForFunctionPointer(Hook(Idx, Marshal.GetFunctionPointerForDelegate(New)), typeof(T)) as T;
		}

		public T Hook<T>(string Name, T New) where T : class
		{
			return Hook<T>(GetIdxForName(Name), New);
		}

		public void Unhook(int Idx)
		{
			if (Originals.ContainsKey(Idx))
			{
				IntPtr Old = Originals[Idx];
				Originals.Remove(Idx);
				VTbl[Idx] = Old;
			}
		}

		public void UnhookAll()
		{
			int[] Hooked = Originals.Keys.ToArray();
			for (int i = 0; i < Hooked.Length; i++)
				Unhook(Hooked[i]);

			foreach (var Handle in NewHookHandles)
				Handle.Free();
			NewHookHandles.Clear();
		}
	}
}