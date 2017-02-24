using GSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{

    [InterfaceVersion("LUASHARED003")]
    public interface ILuaShared
    {
        [VTableSlot(1)]
        void Init(IntPtr param1, bool param2, IntPtr CSteamAPIContext, IntPtr IGet);
        [VTableSlot(2)]
        void Shutdown();
        [VTableSlot(3)]
        void DumpStats();
        [VTableSlot(4)]
        IntPtr CreateLuaInterface(byte param1, bool param2); //returns ILuaInterface*
        [VTableSlot(5)]
        void CloseLuaInterface(IntPtr ILuaInterface); // takes ILuaInterface*
        [VTableSlot(6)]
        /// <summary>
        /// Gets the lua interface for the specified state
        /// </summary>
        /// <param name="state">1: client, 2: server, 3: menu </param>
        /// <returns>ILuaInterface*</returns>
        IntPtr GetLuaInterface(byte state); 
        [VTableSlot(7)]
        void LoadFile(IntPtr param1, IntPtr param2, bool param3, bool param4);
        [VTableSlot(8)]
        void GetCache(IntPtr param1);
        [VTableSlot(9)]
        void MountLua(string param1);
        [VTableSlot(10)]
        void MountLuaAdd(string param1, string param2);
        [VTableSlot(11)]
        void UnMountLua(string param1);
        [VTableSlot(12)]
        void SetFileContents(string param1, string param2);
        [VTableSlot(13)]
        void SetLuaFindHook(IntPtr param1);
        [VTableSlot(14)]
        void FindScripts(IntPtr param1, IntPtr param2, IntPtr param3);
    }
}
