using GSharp.Attributes;
using GSharp.GLuaNET;
using System;
using System.Runtime.InteropServices;

namespace GSharp.Native.Classes
{

    [StructLayout(LayoutKind.Sequential)]
    public struct lua_state
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 69)]
        byte[] _ignore_this_common_lua_header_;
        public IntPtr luabase;

    }

    public interface ILuaBase
    {
        [VTableSlot(0)]
        int Top();
        [VTableSlot(1)]
        void Push(int iStackPos);
        [VTableSlot(2)]
        void Pop(int iAmt = 1);
        [VTableSlot(3)]
        void GetTable(int iStackPos);
        [VTableSlot(4)]
        void GetField(int iStackPos, string strName);
        [VTableSlot(5)]
        void SetField(int iStackPos, string strName);
        [VTableSlot(6)]
        void CreateTable();
        [VTableSlot(7)]
        void SetTable(int i);
        [VTableSlot(8)]
        void SetMetaTable(int i);
        [VTableSlot(9)]
        bool GetMetaTable(int i);
        [VTableSlot(10)]
        void Call(int iArgs, int iResults);
        [VTableSlot(11)]
        int PCall(int iArgs, int iResults, int iErrorFunc);
        [VTableSlot(12)]
        int Equal(int iA, int iB);
        [VTableSlot(13)]
        int RawEqual(int iA, int iB);
        [VTableSlot(14)]
        void Insert(int iStackPos);
        [VTableSlot(15)]
        void Remove(int iStackPos);
        [VTableSlot(16)]
        int Next(int iStackPos);
        [VTableSlot(17)]
        IntPtr NewUserdata(uint iSize);
        [VTableSlot(18)]
        void ThrowError(string strError);
        [VTableSlot(19)]
        void CheckType(int iStackPos, int iType);
        [VTableSlot(20)]
        void ArgError(int iArgNum, string strMessage);
        [VTableSlot(21)]
        void RawGet(int iStackPos);
        [VTableSlot(22)]
        void RawSet(int iStackPos);

        [VTableSlot(23)]
        string GetString(int iStackPos = -1, IntPtr iOutLen = default(IntPtr));
        [VTableSlot(24)]
        double GetNumber(int iStackPos = -1);
        [VTableSlot(25)]
        bool GetBool(int iStackPos = -1);
        [VTableSlot(26)]
        lua_CFunction GetCFunction(int iStackPos = -1);
        [VTableSlot(27)]
        IntPtr GetUserdata(int iStackPos = -1);

        [VTableSlot(28)]
        void PushNil();
        [VTableSlot(29)]
        void PushString(string val, uint iLen);
        [VTableSlot(30)]
        void PushNumber(double val);
        [VTableSlot(31)]
        void PushBool(bool val);
        [VTableSlot(32)]
        void PushCFunction(lua_CFunction val);
        //[VTableSlot(33)]
        //void PushCClosure(lua_CFunction val, int iVars);
        [VTableSlot(34)]
        void PushUserdata(IntPtr pointer);

        //
        // If you create a reference - don't forget to free it!
        //
        [VTableSlot(35)]
        int ReferenceCreate();
        [VTableSlot(36)]
        void ReferenceFree(int i);
        [VTableSlot(37)]
        void ReferencePush(int i);

        //
        // Push a special value onto the top of the stack ( see below )
        //
        [VTableSlot(38)]
        void PushSpecial(int iType);

        //
        // For type enums see Types.h 
        //
        [VTableSlot(39)]
        bool IsType(int iStackPos, int iType);
        [VTableSlot(40)]
        int GetType(int iStackPos);
        [VTableSlot(41)]
        string GetTypeName(int iType);

        //
        // Creates a new meta table of string and type and leaves it on the stack.
        // Will return the old meta table of this name if it already exists.
        //
        [VTableSlot(42)]
        void CreateMetaTableType(string strName, int iType);

        //
        // Like Get* but throws errors and returns if they're not of the expected type
        //
        [VTableSlot(43)]
        string CheckString(int iStackPos = -1);
        [VTableSlot(44)]
        double CheckNumber(int iStackPos = -1);
        [VTableSlot(45)]
        int ObjLen(int index);
    }
}
