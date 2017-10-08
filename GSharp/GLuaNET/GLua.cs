using GSharp.Attributes;
using GSharp.GLuaNET.TypeMarshals;
using GSharp.Native.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using GSharp.Generated.NativeClasses;

namespace GSharp.GLuaNET
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int lua_CFunction(IntPtr State);

    public class GLua
    {
        static List<GCHandle> DelegateHandles;

        public const int LUA_REGISTRYINDEX = -10000;
        public const int LUA_ENVIRONINDEX = -10001;
        public const int LUA_GLOBALSINDEX = -10002;

        private static Dictionary<Type, ILuaTypeMarshal> Marshals = new Dictionary<Type, ILuaTypeMarshal>();

        public static void RegisterMarshal(Type t, ILuaTypeMarshal marshal)
        {
            Marshals.Add(t, marshal);
        }

        public lua_State State { get; protected set; }

        public ILuaBase LuaBase { get; protected set; }

        public GLua(lua_State luaState)
        {
            State = luaState;
            LuaBase = JIT.ConvertInstance<ILuaBase>(luaState.luabase);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public void Push<T>(T obj)
        {
            Push(obj, typeof(T));
        }

        public object Get(Type type)
        {
            if (Marshals.ContainsKey(type))
            {
                var marshal = Marshals[type] as ILuaTypeMarshal;
                return Convert.ChangeType(marshal.Get(this), type);
            }
            return null;
        }

        public void Push(object obj, Type type)
        {
            if (Marshals.ContainsKey(type))
            {
                var marshal = Marshals[type] as ILuaTypeMarshal;
                marshal.Push(this, obj);
            }
        }

        public void SetArray<T>(T[] array)
        {
            CreateTable();
            for (int i = 0; i < array.Length; i++)
            {
                Push(array[i]);
                Push<int>(i);
                RawSet(-3);
                Pop(2);
            }
        }

        public T[] GetArray<T>()
        {
            var count = ObjLen();
            var newArray = new T[count];
            for (int i = 1; i <= count; i++)
            {
                Push<int>(i);
                RawGet(-2);
                newArray[i - 1] = Get<T>();
                Pop(2);
            }

            return newArray;
        }

        public T GetReturnType<T>() where T : new()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => Attribute.IsDefined(p, typeof(ReturnIndexAttribute)));
            var pops = new PropertyInfo[properties.Count()];
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute(typeof(ReturnIndexAttribute)) as ReturnIndexAttribute;
                if (attr != null)
                {
                    pops[attr.ReturnIndex] = prop;
                }
            }
            Array.Reverse(pops);
            var newReturnObject = new T();
            for (int i = 0; i < pops.Length; i++)
            {
                var value = Get(pops[i].PropertyType);
                pops[i].SetValue(newReturnObject, value);
                Pop(1);
            }

            return newReturnObject;
        }

        public void SetGlobal<T>(string Name, T Obj)
        {
            Push(Obj);
            SetField(LUA_GLOBALSINDEX, Name);
        }

        public T GetGlobal<T>(string Name)
        {
            GetField(LUA_GLOBALSINDEX, Name);
            return Get<T>();
        }

        /// <summary>
        /// Runs the action for every "next", with key at -2 and the value at -1, on the table at the top of the stack
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action action)
        {
            if(!IsType(-1, LuaType.Table)) { return; }
            PushNil();
            while (Next(-2) != 0)
            {
                action?.Invoke();
                Pop(1);
            }
        }



        #region ILuaBase Passthrough(Some of it)
        public void PushCFunction(lua_CFunction val)
        {
            var ptr = Marshal.GetFunctionPointerForDelegate(val);
            LuaBase.PushCFunction(ptr);
        }
        public void PushCFunction(IntPtr val)
        {
            LuaBase.PushCFunction(val);
        }

        public int Top()
        {
            return LuaBase.Top();
        }

        public void Push(int iStackPos)
        {
            LuaBase.Push(iStackPos);
        }

        public void Pop(int iAmt = 1)
        {
            LuaBase.Pop(iAmt);
        }

        public void GetTable(int iStackPos)
        {
            LuaBase.GetTable(iStackPos);
        }

        public void GetField(int iStackPos, string strName)
        {
            LuaBase.GetField(iStackPos, strName);
        }

        public void SetField(int iStackPos, string strName)
        {
            LuaBase.SetField(iStackPos, strName);
        }

        public void CreateTable()
        {
            LuaBase.CreateTable();
        }

        public void SetTable(int i)
        {
            LuaBase.SetTable(i);
        }

        public void SetMetaTable(int i)
        {
            LuaBase.SetMetaTable(i);
        }

        public bool GetMetaTable(int i)
        {
            return LuaBase.GetMetaTable(i);
        }

        public void Call(int iArgs, int iResults)
        {
            LuaBase.Call(iArgs, iResults);
        }

        public int PCall(int iArgs, int iResults, int iErrorFunc)
        {
            return LuaBase.PCall(iArgs, iResults, iErrorFunc);
        }

        public int Equal(int iA, int iB)
        {
            return LuaBase.Equal(iA, iB);
        }

        public int RawEqual(int iA, int iB)
        {
            return LuaBase.RawEqual(iA, iB);
        }

        public void Insert(int iStackPos)
        {
            LuaBase.Insert(iStackPos);
        }

        public void Remove(int iStackPos)
        {
            LuaBase.Remove(iStackPos);
        }

        public int Next(int iStackPos)
        {
            return LuaBase.Next(iStackPos);
        }

        public IntPtr NewUserdata(uint iSize)
        {
            return LuaBase.NewUserdata(iSize);
        }

        public void ThrowError(string strError)
        {
            LuaBase.ThrowError(strError);
        }

        public void CheckType(int iStackPos, int iType)
        {
            LuaBase.CheckType(iStackPos, iType);
        }

        public void ArgError(int iArgNum, string strMessage)
        {
            LuaBase.ArgError(iArgNum, strMessage);
        }

        public void RawGet(int iStackPos)
        {
            LuaBase.RawGet(iStackPos);
        }

        public void RawSet(int iStackPos)
        {
            LuaBase.RawSet(iStackPos);
        }

        public IntPtr GetUserdata(int iStackPos = -1)
        {
            return LuaBase.GetUserdata(iStackPos);
        }

        public void PushNil()
        {
            LuaBase.PushNil();
        }

        public void PushUserdata(IntPtr pointer)
        {
            LuaBase.PushUserdata(pointer);
        }

        public int ReferenceCreate()
        {
            return LuaBase.ReferenceCreate();
        }

        public void ReferenceFree(int i)
        {
            LuaBase.ReferenceFree(i);
        }

        public void ReferencePush(int i)
        {
            LuaBase.ReferencePush(i);
        }

        public void PushSpecial(LuaType iType)
        {
            LuaBase.PushSpecial(iType);
        }

        public bool IsType(int iStackPos, LuaType iType)
        {
            return LuaBase.IsType(iStackPos, iType);
        }

        public LuaType GetLuaType(int iStackPos = -1)
        {
            return LuaBase.GetType(iStackPos);
        }

        public string GetTypeName(LuaType iType)
        {
            return LuaBase.GetTypeName(iType);
        }

        public void CreateMetaTableType(string strName, LuaType iType)
        {
            LuaBase.CreateMetaTableType(strName, iType);
        }

        public int ObjLen(int index = -1)
        {
            return LuaBase.ObjLen(index);
        }
        #endregion
    }
}
