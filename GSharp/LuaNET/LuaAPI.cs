#undef lua51 // Done
#undef lua52 // Not implemented
#undef lua53 // Not implemented
#undef luajit // Done
/////////////
#define lua51

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GSharp.LuaNET
{
    public enum LuaVersion
    {
        Unknown,
        Lua51,
        Lua52,
        Lua53,
        LuaJIT
    }

    internal class Settings
    {
        public const string DllName =
#if lua51
 "lua_shared.dll"
#elif lua52
 "lua_shared.dll"
#elif lua53
 "lua_shared.dll"
#elif luajit
 "lua_shared.dll"
#else
 ""
#error Unknown lua version
#endif
;

        public const CallingConvention CConv = CallingConvention.Cdecl;
        public const CharSet CSet = CharSet.Ansi;
    }

    internal class LuaStringMarshal : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr NativeData)
        {
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return IntPtr.Zero;
        }

        public object MarshalNativeToManaged(IntPtr NativeData)
        {
            return Marshal.PtrToStringAnsi(NativeData);
        }

        internal static LuaStringMarshal Singleton;
        public static ICustomMarshaler GetInstance(string Cookie)
        {
            if (Singleton == null)
                Singleton = new LuaStringMarshal();
            return Singleton;
        }
    }

    internal class LuaFunctionMarshal : ICustomMarshaler
    {
        Dictionary<lua_CFunction, GCHandle> lua_CFunction_Handles;

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr NativeData)
        {
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            lua_CFunction F = (lua_CFunction)ManagedObj;
            if (lua_CFunction_Handles == null)
                lua_CFunction_Handles = new Dictionary<lua_CFunction, GCHandle>();
            if (!lua_CFunction_Handles.ContainsKey(F))
                lua_CFunction_Handles.Add(F, GCHandle.Alloc(F));
            return Marshal.GetFunctionPointerForDelegate(F);
        }

        public object MarshalNativeToManaged(IntPtr NativeData)
        {
            return (lua_CFunction)Marshal.GetDelegateForFunctionPointer(NativeData, typeof(lua_CFunction));
        }

        internal static LuaFunctionMarshal Singleton;
        public static ICustomMarshaler GetInstance(string Cookie)
        {
            if (Singleton == null)
                Singleton = new LuaFunctionMarshal();
            return Singleton;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct lua_StatePtr
    {
        public IntPtr StatePtr;

        public lua_StatePtr(IntPtr State)
        {
            this.StatePtr = State;
        }

        public override string ToString()
        {
            return string.Format("0x{0:X}", StatePtr.ToInt64());
        }

        public static lua_StatePtr NULL
        {
            get
            {
                return new lua_StatePtr(IntPtr.Zero);
            }
        }

        public static implicit operator IntPtr(lua_StatePtr ptr)
        {
            return ptr.StatePtr;
        }
        public static implicit operator lua_StatePtr(IntPtr ptr)
        {
            return new lua_StatePtr(ptr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct lua_DebugPtr
    {
        public lua_Debug* DebugPtr;

        public lua_Debug Debug
        {
            get
            {
                return *DebugPtr;
            }
        }

        public override string ToString()
        {
            return string.Format("0x{0:X}", (long)DebugPtr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StrPtr
    {
        public IntPtr Pointer;

        public override string ToString()
        {
            return Pointer.ToString();
        }

        public string ToString(IntPtr Len)
        {
            byte[] Bytes = new byte[Len.ToInt64()];
            Marshal.Copy(Pointer, Bytes, 0, Bytes.Length);
            return Encoding.ASCII.GetString(Bytes);
        }

        public static implicit operator IntPtr(StrPtr SPtr)
        {
            return SPtr.Pointer;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct lua_Debug
    {
        public int Event;
        public IntPtr Name;
        public IntPtr NameWhat;
        public IntPtr What;
        public IntPtr Source;
        public int CurrentLine;
        public int NUps;
        public int LineDefined;
        public int LastLineDefined;
        public fixed byte ShortSrc[Lua.LUA_IDSIZE];
        public int i_ci;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct luaL_Reg
    {
        public IntPtr Name;
        IntPtr Fnc;

        public lua_CFunction Func
        {
            get
            {
                return (lua_CFunction)LuaFunctionMarshal.GetInstance("").MarshalNativeToManaged(Fnc);
            }
            set
            {
                Fnc = LuaFunctionMarshal.GetInstance("").MarshalManagedToNative(value);
            }
        }

        public luaL_Reg(string Name, lua_CFunction Func)
        {
            this.Name = Marshal.StringToHGlobalAnsi(Name);
            this.Fnc = IntPtr.Zero;
            this.Func = Func;
        }

        public static luaL_Reg NULL
        {
            get
            {
                luaL_Reg Ret = new luaL_Reg();
                Ret.Name = IntPtr.Zero;
                Ret.Fnc = IntPtr.Zero;
                return Ret;
            }
        }
    }

    [UnmanagedFunctionPointer(Settings.CConv, CharSet = Settings.CSet)]
    public delegate int lua_CFunction(lua_StatePtr State);

    [UnmanagedFunctionPointer(Settings.CConv, CharSet = Settings.CSet)]
    public delegate string lua_Reader(lua_StatePtr State, IntPtr UD, IntPtr Size);

    [UnmanagedFunctionPointer(Settings.CConv, CharSet = Settings.CSet)]
    public delegate int lua_Writer(lua_StatePtr State, IntPtr P, IntPtr Sz, IntPtr UD);

    [UnmanagedFunctionPointer(Settings.CConv, CharSet = Settings.CSet)]
    public delegate IntPtr lua_Alloc(IntPtr UD, IntPtr Ptr, IntPtr OSize, IntPtr NSize);

    [UnmanagedFunctionPointer(Settings.CConv, CharSet = Settings.CSet)]
    public delegate void lua_Hook(lua_StatePtr L, lua_DebugPtr AR);

#if luajit
	public enum LuaJITMode : uint {
		Engine,
		Debug,
		Func,
		AllFunc,
		AllSubFunc,
		Trace,
		WrapCFunc = 0x10,
		Max
	}
#endif

    public static class Lua
    {
        public const LuaVersion VERSION =
#if lua51
 LuaVersion.Lua51
#elif lua52
 LuaVersion.Lua52
#elif lua53
 LuaVersion.Lua53
#elif luajit
 LuaVersion.LuaJIT
#else
 LuaVersion.Unknown
#error Unknown lua version
#endif
;

#if luajit
		public const int LUAJIT_MODE_MASK = 0xFF;
		public const int LUAJIT_MODE_OFF = 0x0;
		public const int LUAJIT_MODE_ON = 0x100;
		public const int LUAJIT_MODE_FLUSH = 0x200;

		[DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
		public static extern int luaJIT_setmode(lua_StatePtr L, int Idx, int Mode);

		[DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
		public static extern void LUAJIT_VERSION_SYM();
#endif
        public const int LUA_IDSIZE = 60;

        // Option for multiple returns in 'lua_pcall' and 'lua_call'
        public const int LUA_MULTRET = -1;

        // Pseudo-indices
        public const int LUA_REGISTRYINDEX = -10000;
#if lua51
        public const int LUA_ENVIRONINDEX = -10001;
        public const int LUA_GLOBALSINDEX = -10002;
#endif

        public static int lua_upvalueindex(int I)
        {
            return
#if lua51
 LUA_GLOBALSINDEX
#else
 LUA_REGISTRYINDEX
#endif
 - I;
        }

#if !lua51
		public const int LUA_OPEQ = 0;
		public const int LUA_OPLT = 1;
		public const int LUA_OPLE = 2;
#endif

        // Thread status; 0 is OK
        public const int LUA_YIELD = 1;
        public const int LUA_ERRRUN = 2;
        public const int LUA_ERRSYNTAX = 3;
        public const int LUA_ERRMEM = 4;
        public const int LUA_ERRERR = 5;

        // Basic types
        public const int LUA_TNONE = -1;
        public const int LUA_TNIL = 0;
        public const int LUA_TBOOLEAN = 1;
        public const int LUA_TLIGHTUSERDATA = 2;
        public const int LUA_TNUMBER = 3;
        public const int LUA_TSTRING = 4;
        public const int LUA_TTABLE = 5;
        public const int LUA_TFUNCTION = 6;
        public const int LUA_TUSERDATA = 7;
        public const int LUA_TTHREAD = 8;

        // Minimum Lua stack available to a C function
        public const int LUA_MINSTACK = 20;

        // State manipulation

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern lua_StatePtr lua_newstate(lua_Alloc F, IntPtr UD);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_close(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern lua_StatePtr lua_newthread(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaFunctionMarshal))]
        public static extern lua_CFunction lua_atpanic(lua_StatePtr L, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaFunctionMarshal))] lua_CFunction PanicF);

        // Basic stack manipulation

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_gettop(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_settop(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushvalue(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_remove(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_insert(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_replace(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_checkstack(lua_StatePtr L, int Sz);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_xmove(lua_StatePtr From, lua_StatePtr To, int N);

        // Access functions (stack -> C#)

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern bool lua_isnumber(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern bool lua_isstring(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern bool lua_iscfunction(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern bool lua_isuserdata(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_type(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string lua_typename(lua_StatePtr L, int Tp);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_equal(lua_StatePtr L, int Idx1, int Idx2);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_rawequal(lua_StatePtr L, int Idx1, int Idx2);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_lessthan(lua_StatePtr L, int Idx1, int Idx2);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern double lua_tonumber(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_tointeger(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern bool lua_toboolean(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string lua_tolstring(lua_StatePtr L, int Idx, out IntPtr Len);

        [DllImport(Settings.DllName, EntryPoint = "lua_tolstring", CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern StrPtr lua_tolstring_ptr(lua_StatePtr L, int Idx, out IntPtr Len);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int
#if lua51
 lua_objlen
#else
 lua_rawlen
#endif
(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        /*[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaFunctionMarshal))]*/
        public static extern /*lua_CFunction*/ IntPtr lua_tocfunction(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern IntPtr lua_touserdata(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern lua_StatePtr lua_tothread(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern IntPtr lua_topointer(lua_StatePtr L, int Idx);

        // Push functions (C# -> stack)

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushnil(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushnumber(lua_StatePtr L, double N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushinteger(lua_StatePtr L, int Int);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushlstring(lua_StatePtr L, string Str, IntPtr Size);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushstring(lua_StatePtr L, string Str);
        // lua_pushvfstring

        // lua_pushfstring

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushcclosure(lua_StatePtr L, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaFunctionMarshal))] lua_CFunction Fn, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushboolean(lua_StatePtr L, bool B);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_pushlightuserdata(lua_StatePtr L, IntPtr P);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_pushthread(lua_StatePtr L);

        // Get functions (Lua -> stack)

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_gettable(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_getfield(lua_StatePtr L, int Idx, string K);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_rawget(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_rawgeti(lua_StatePtr L, int Idx, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_createtable(lua_StatePtr L, int NArr, int NRec);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern IntPtr lua_newuserdata(lua_StatePtr L, IntPtr Sz);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_getmetatable(lua_StatePtr L, int ObjIdx);

#if lua51
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_getfenv(lua_StatePtr L, int Idx);
#endif

        // Set functions (stack -> Lua)

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_settable(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_setfield(lua_StatePtr L, int Idx, string K);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_rawset(lua_StatePtr L, int Idx);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_rawseti(lua_StatePtr L, int Idx, int N);

#if !lua51
		[DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
		public static extern int luaL_setfuncs(lua_StatePtr L, luaL_Reg[] l, int nup);
#endif

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_setmetatable(lua_StatePtr L, int ObjIdx);

#if lua51
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_setfenv(lua_StatePtr L, int Idx);
#endif

        // 'load' and 'call' functions (load and run Lua code)

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
#if lua51
        public static extern void lua_call(lua_StatePtr L, int NArgs, int NResults);
#else
		public static extern void lua_callk(lua_StatePtr L, int NArgs, int NResults, int Ctx, lua_CFunction K);

		public static void lua_call(lua_StatePtr L, int NArgs, int NResults) {
			lua_callk(L, NArgs, NResults, 0, null);
		}
#endif

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
#if lua51
        public static extern int lua_pcall(lua_StatePtr L, int Nargs, int NResults, int ErrFunc);
#else
		public static extern int lua_pcallk(lua_StatePtr L, int Nargs, int NResults, int ErrFunc, int Ctx, lua_CFunction K);

		public static int lua_pcall(lua_StatePtr L, int NArgs, int NResults, int ErrFunc) {
			return lua_pcallk(L, NArgs, NResults, ErrFunc, 0, null);
		}
#endif

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_cpcall(lua_StatePtr L, /*[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaFunctionMarshal))] lua_CFunction*/ IntPtr Func, IntPtr UD);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_load(lua_StatePtr L, lua_Reader Reader, IntPtr DT, string ChunkName
#if !lua51
, string Mode = null
#endif
);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_dump(lua_StatePtr L, lua_Writer Writer, IntPtr Data);

        // Coroutine functions

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_yield(lua_StatePtr L, int NResults);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_resume(lua_StatePtr L,
#if lua51
#else
 lua_StatePtr From,
#endif
 int NArg);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_status(lua_StatePtr L);

        // Garbage collection function and options

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_gc(lua_StatePtr L, int What, int Data);

        // Miscellaneous functions

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_error(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_next(lua_StatePtr L, int Idx);

#if !lua51
		[DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
		public static extern int lua_compare(lua_StatePtr L, int Idx1, int Idx2, int Op);
#endif

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_concat(lua_StatePtr L, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern lua_Alloc lua_getallocf(lua_StatePtr L, ref IntPtr UD);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_setallocf(lua_StatePtr L, lua_Alloc F, IntPtr UD);

        // Some useful macros

        public static void lua_pop(lua_StatePtr L, int N)
        {
            lua_settop(L, -(N) - 1);
        }

        public static void lua_newtable(lua_StatePtr L)
        {
            lua_createtable(L, 0, 0);
        }

        public static void lua_register(lua_StatePtr L, string N, lua_CFunction F)
        {
            lua_pushcfunction(L, F);
            lua_setglobal(L, N);
        }

        public static void lua_pushcfunction(lua_StatePtr L, lua_CFunction F)
        {
            lua_pushcclosure(L, F, 0);
        }

        public static int lua_strlen(lua_StatePtr L, int I)
        {
            return
#if lua51
 lua_objlen
#else
 lua_rawlen
#endif
(L, I);
        }

        public static bool lua_isfunction(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TFUNCTION;
        }

        public static bool lua_istable(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TTABLE;
        }

        public static bool lua_islightuserdata(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TLIGHTUSERDATA;
        }

        public static bool lua_isnil(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TNIL;
        }

        public static bool lua_isboolean(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TBOOLEAN;
        }

        public static bool lua_isthread(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TTHREAD;
        }

        public static bool lua_isnone(lua_StatePtr L, int N)
        {
            return lua_type(L, N) == LUA_TNONE;
        }

        public static bool lua_isnoneornil(lua_StatePtr L, int N)
        {
            return lua_type(L, N) <= 0;
        }

        public static void lua_pushliteral(lua_StatePtr L, string S)
        {
            lua_pushlstring(L, S, new IntPtr(S.Length));
        }

#if lua51
        public static void lua_setglobal(lua_StatePtr L, string S)
        {
            lua_setfield(L, LUA_GLOBALSINDEX, S);
        }

        public static void lua_getglobal(lua_StatePtr L, string S)
        {
            lua_getfield(L, LUA_GLOBALSINDEX, S);
        }
#else
		[DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
		public static extern void lua_setglobal(lua_StatePtr L, string S);

		[DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
		public static extern void lua_getglobal(lua_StatePtr L, string S);
#endif


        public static string lua_tostring(lua_StatePtr L, int I)
        {
            IntPtr Len;
            return lua_tolstring(L, I, out Len);
        }

        // Hack
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void lua_setlevel(lua_StatePtr From, lua_StatePtr To);

        // Debug API
        public const int LUA_HOOKCALL = 0;
        public const int LUA_HOOKRET = 1;
        public const int LUA_HOOKLINE = 2;
        public const int LUA_HOOKCOUNT = 3;
        public const int LUA_HOOKTAILRET = 4;

        // Event masks
        public const int LUA_MASKCALL = 1 << LUA_HOOKCALL;
        public const int LUA_MASKRET = 1 << LUA_HOOKRET;
        public const int LUA_MASKLINE = 1 << LUA_HOOKLINE;
        public const int LUA_MASKCOUNT = 1 << LUA_HOOKCOUNT;

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_getstack(lua_StatePtr L, int Level, lua_DebugPtr AR);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_getinfo(lua_StatePtr L, string What, lua_DebugPtr AR);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string lua_getlocal(lua_StatePtr L, lua_DebugPtr AR, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string lua_setlocal(lua_StatePtr L, lua_DebugPtr AR, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string lua_getupvalue(lua_StatePtr L, int FuncIdx, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string lua_sestupvalue(lua_StatePtr L, int FuncIdx, int N);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_sethook(lua_StatePtr L, lua_Hook Fnc, int Mask, int Count);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern lua_Hook lua_gethook(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_gethookmask(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int lua_gethookcount(lua_StatePtr L);

        public const int LUA_ERRFILE = Lua.LUA_ERRERR + 1;

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_openlib(lua_StatePtr L, string Libname, luaL_Reg[] Lib, int NUp);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_register(lua_StatePtr L, string LibName, luaL_Reg[] Lib);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_getmetafield(lua_StatePtr L, int Obj, string E);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_callmeta(lua_StatePtr L, int Obj, string E);

#if lua51
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_typerror(lua_StatePtr L, int NArg, string TName);
#endif

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_argerror(lua_StatePtr L, int NumArg, string ExtraMsg);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string luaL_checklstring(lua_StatePtr L, int NumArg, ref IntPtr Len);

        [DllImport(Settings.DllName, EntryPoint = "luaL_checklstring", CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern StrPtr luaL_checklstring_ptr(lua_StatePtr L, int NumArg, ref IntPtr Len);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string luaL_optlstring(lua_StatePtr L, int NumArg, string Def, ref IntPtr Len);

        [DllImport(Settings.DllName, EntryPoint = "luaL_optlstring", CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern StrPtr luaL_optlstring_ptr(lua_StatePtr L, int NumArg, string Def, ref IntPtr Len);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern double luaL_checknumber(lua_StatePtr L, int NumArg);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern double luaL_optnumber(lua_StatePtr L, int NArg, double Def);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_checkinteger(lua_StatePtr L, int NumArg);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_optinteger(lua_StatePtr L, int NArg, int Def);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_checkstack(lua_StatePtr L, int Sz, string Msg);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_checktype(lua_StatePtr L, int NArg, int T);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_checkany(lua_StatePtr L, int NArg);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_newmetatable(lua_StatePtr L, string TName);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern IntPtr luaL_checkudata(lua_StatePtr L, int UD, string TName);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_where(lua_StatePtr L, int Lvl);

        // luaL_error

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_checkoption(lua_StatePtr L, int NArg, string Def, string[] Lst);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_ref(lua_StatePtr L, int T);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_unref(lua_StatePtr L, int T, int Ref);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_loadfile(lua_StatePtr L, string Filename);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_loadbuffer(lua_StatePtr L, string Buff, IntPtr Sz, string Name);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaL_loadstring(lua_StatePtr L, string S);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern lua_StatePtr luaL_newstate();

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string luaL_gsub(lua_StatePtr L, string S, string P, string R);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LuaStringMarshal))]
        public static extern string luaL_findtable(lua_StatePtr L, int Idx, string FName, int SZHint);

        // Some useful macros

        public static void luaL_argcheck(lua_StatePtr L, bool Cond, int NumArg, string ExtraMsg)
        {
            if (!Cond)
                luaL_argerror(L, NumArg, ExtraMsg);
        }

        public static string luaL_checkstring(lua_StatePtr L, int N)
        {
            IntPtr Len = IntPtr.Zero;
            return luaL_checklstring(L, N, ref Len);
        }

        public static string luaL_optstring(lua_StatePtr L, int N, string Def)
        {
            IntPtr Len = IntPtr.Zero;
            return luaL_optlstring(L, N, Def, ref Len);
        }

        public static int luaL_checkint(lua_StatePtr L, int N)
        {
            return luaL_checkinteger(L, N);
        }

        public static int luaL_optint(lua_StatePtr L, int N, int Def)
        {
            return luaL_optinteger(L, N, Def);
        }

        public static long luaL_checklong(lua_StatePtr L, int N)
        {
            return (long)luaL_checkinteger(L, N);
        }

        public static long luaL_optlong(lua_StatePtr L, int N, long Def)
        {
            return (long)luaL_optinteger(L, N, (int)Def);
        }

        public static string luaL_typename(lua_StatePtr L, int I)
        {
            return Lua.lua_typename(L, Lua.lua_type(L, I));
        }

        public static int luaL_dofile(lua_StatePtr L, string FName)
        {
            int Ret = 0;
            if ((Ret = luaL_loadfile(L, FName)) == 0)
                Ret = Lua.lua_pcall(L, 0, Lua.LUA_MULTRET, 0);
            return Ret;
        }

        public static int luaL_dostring(lua_StatePtr L, string Str)
        {
            int Ret = 0;
            if ((Ret = luaL_loadstring(L, Str)) == 0)
                Ret = Lua.lua_pcall(L, 0, Lua.LUA_MULTRET, 0);
            return Ret;
        }

        public static void luaL_getmetatable(lua_StatePtr L, string N)
        {
            Lua.lua_getfield(L, Lua.LUA_REGISTRYINDEX, N);
        }

        public static T luaL_opt<T>(lua_StatePtr L, Func<lua_StatePtr, int, T> F, int N, T Def)
        {
            if (Lua.lua_isnoneornil(L, N))
                return Def;
            return F(L, N);
        }

        public const int LUA_NOREF = -2;
        public const int LUA_REFNIL = -1;

        public static int lua_ref(lua_StatePtr L, bool Lock)
        {
            if (Lock)
                return luaL_ref(L, Lua.LUA_REGISTRYINDEX);
            Lua.lua_pushstring(L, "unlocked references are obsolete");
            Lua.lua_error(L);
            return LUA_NOREF;
        }

        public static void lua_unref(lua_StatePtr L, int Ref)
        {
            luaL_unref(L, Lua.LUA_REGISTRYINDEX, Ref);
        }

        public static void lua_getref(lua_StatePtr L, int Ref)
        {
            Lua.lua_rawgeti(L, Lua.LUA_REGISTRYINDEX, Ref);
        }

        public const string LUA_COLIBNAME = "coroutine";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_base(lua_StatePtr L);

        public const string LUA_TABLIBNAME = "table";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_table(lua_StatePtr L);

        public const string LUA_IOLIBNAME = "io";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_io(lua_StatePtr L);

        public const string LUA_OSLIBNAME = "os";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_os(lua_StatePtr L);

        public const string LUA_STRLIBNAME = "string";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_string(lua_StatePtr L);

        public const string LUA_MATHLIBNAME = "math";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_math(lua_StatePtr L);

        public const string LUA_DBLIBNAME = "debug";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_debug(lua_StatePtr L);

        public const string LUA_LOADLIBNAME = "package";
        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern int luaopen_package(lua_StatePtr L);

        [DllImport(Settings.DllName, CharSet = Settings.CSet, CallingConvention = Settings.CConv)]
        public static extern void luaL_openlibs(lua_StatePtr L);
    }
}