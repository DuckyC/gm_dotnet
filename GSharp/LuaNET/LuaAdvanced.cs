using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using GSharp.Attributes;

namespace GSharp.LuaNET
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class LuaRegister : Attribute
    {
        public readonly string Name;

        public LuaRegister(string Name)
        {
            this.Name = Name;
        }

        public LuaRegister() : this(null)
        {
        }
    }

    public static class LuaAdvanced
    {
        static List<GCHandle> DelegateHandles;
        static Dictionary<Type, Func<lua_StatePtr, int, object>> LuaToNetMarshals;
        static Dictionary<Type, Func<lua_StatePtr, object, int>> NetToLuaMarshals;

        static LuaAdvanced()
        {
            DelegateHandles = new List<GCHandle>();
            NetToLuaMarshals = new Dictionary<Type, Func<lua_StatePtr, object, int>>();
            LuaToNetMarshals = new Dictionary<Type, Func<lua_StatePtr, int, object>>();

            LuaAdvanced.AddTypeMarshal(typeof(object), (State, Idx) =>
            {
                int LT = Lua.lua_type(State, Idx);
                Type T = null;
                switch (LT)
                {
                    case Lua.LUA_TNONE:
                        Lua.luaL_argerror(State, Idx, "expected a value");
                        return null;
                    case Lua.LUA_TNIL:
                        return null;
                    case Lua.LUA_TBOOLEAN:
                        T = typeof(bool);
                        break;
                    case Lua.LUA_TNUMBER:
                        T = typeof(double);
                        break;
                    case Lua.LUA_TSTRING:
                        T = typeof(string);
                        break;
                    case Lua.LUA_TLIGHTUSERDATA:
                        T = typeof(IntPtr);
                        break;
                    case Lua.LUA_TFUNCTION:
                        if (Lua.lua_iscfunction(State, Idx))
                            T = typeof(lua_CFunction);
                        else
                            throw new NotImplementedException();
                        break;
                    case Lua.LUA_TTABLE:
                    case Lua.LUA_TTHREAD:
                    case Lua.LUA_TUSERDATA:
                    default:
                        throw new NotImplementedException();
                }

                return Pop(State, Idx, T);
            }, (State, Obj) =>
            {
                if (Obj == null)
                {
                    Lua.lua_pushnil(State);
                    return 1;
                }
                throw new NotImplementedException();
            });

            LuaAdvanced.AddTypeMarshal(typeof(string), (State, Idx) =>
            {
                return Lua.luaL_checkstring(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushstring(State, (string)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(double), (State, Idx) =>
            {
                return Lua.luaL_checknumber(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushnumber(State, (double)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(float), (State, Idx) =>
            {
                return (float)Lua.luaL_checknumber(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushnumber(State, (float)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(int), (State, Idx) =>
            {
                return Lua.luaL_checkinteger(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushinteger(State, (int)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(long), (State, Idx) =>
            {
                return (long)Lua.luaL_checkinteger(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushinteger(State, (int)(long)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(bool), (State, Idx) =>
            {
                Lua.luaL_checktype(State, Idx, Lua.LUA_TBOOLEAN);
                return Lua.lua_toboolean(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushboolean(State, (bool)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(lua_CFunction), (State, Idx) =>
            {
                Lua.luaL_checktype(State, Idx, Lua.LUA_TFUNCTION);
                return Lua.lua_tocfunction(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushcfunction(State, (lua_CFunction)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(Delegate), (State, Idx) =>
            {
                throw new NotImplementedException();
            }, (State, Obj) =>
            {
                Push(State, Wrap((Delegate)Obj));
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(IntPtr), (State, Idx) =>
            {
                Lua.luaL_checktype(State, Idx, Lua.LUA_TLIGHTUSERDATA);
                return Lua.lua_touserdata(State, Idx);
            }, (State, Obj) =>
            {
                Lua.lua_pushlightuserdata(State, (IntPtr)Obj);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(lua_StatePtr), (State, Idx) =>
            {
                Lua.luaL_checktype(State, Idx, Lua.LUA_TLIGHTUSERDATA);
                return new lua_StatePtr(Lua.lua_touserdata(State, Idx));
            }, (State, Obj) =>
            {
                Lua.lua_pushlightuserdata(State, ((lua_StatePtr)Obj).StatePtr);
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(Type), (State, Idx) =>
            {
                Lua.luaL_checktype(State, Idx, Lua.LUA_TLIGHTUSERDATA);
                return GetTypeFromHandle(Lua.lua_touserdata(State, Idx));
            }, (State, Obj) =>
            {
                Lua.lua_pushlightuserdata(State, GetHandleFromType((Type)Obj));
                return 1;
            });

            LuaAdvanced.AddTypeMarshal(typeof(string[]), (State, Idx) =>
            {
                return PopArray<string>(State);
            }, (State, Obj) =>
            {
                PushArray<string>(State, (string[])Obj); return 1;
            });
        }

        public static int Push(lua_StatePtr L, object Ret)
        {
            Type T = typeof(object);
            if (Ret != null)
                T = Ret.GetType();

            if (NetToLuaMarshals.ContainsKey(T))
                return NetToLuaMarshals[T](L, Ret);
            else
                foreach (var KV in NetToLuaMarshals)
                    if (KV.Key.IsAssignableFrom(T))
                        try
                        {
                            return KV.Value(L, Ret);
                        }
                        catch (NotImplementedException)
                        {
                            continue;
                        }

            throw new Exception("Unsupported Lua marshal type " + T);
        }

        public static object Pop(lua_StatePtr L, int N, Type T)
        {
            if (LuaToNetMarshals.ContainsKey(T))
                return LuaToNetMarshals[T](L, N);
            else
                foreach (var KV in LuaToNetMarshals)
                    if (KV.Key.IsAssignableFrom(T))
                        try
                        {
                            return KV.Value(L, N);
                        }
                        catch (NotImplementedException)
                        {
                            continue;
                        }

            throw new Exception("Unsupported Lua marshal type " + T);
        }

        public static T PopReturnType<T>(lua_StatePtr L) where T : new()
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
                var value = Pop(L, -1, pops[i].PropertyType);
                pops[i].SetValue(newReturnObject, value);
                Lua.lua_pop(L, 1);
            }

            return newReturnObject;
        }

        public static void PushArray<T>(lua_StatePtr L, T[] array)
        {
            var count = array.Count();
            Lua.lua_createtable(L, count, 0);
            for (int i = 0; i < count; i++)
            {
                Push(L, array[i]);
                Lua.lua_rawseti(L, -2, i + 1);
                Lua.lua_pop(L, 1);
            }
        }

        public static T[] PopArray<T>(lua_StatePtr L) where T : class
        {
            var count = Lua.lua_objlen(L, -1);
            var newArray = new T[count];
            for (int i = 1; i <= count; i++)
            {
                Lua.lua_rawgeti(L, -1, i);
                newArray[i - 1] = Pop(L, -1, typeof(T)) as T;
                Lua.lua_pop(L, 1);
            }

            return newArray;
        }

        public static void AddTypeMarshal(Type T, Func<lua_StatePtr, int, object> LuaToNet,
            Func<lua_StatePtr, object, int> NetToLua)
        {
            if (LuaToNetMarshals.ContainsKey(T))
                throw new Exception("Marshal type " + T + " already registered");
            LuaToNetMarshals.Add(T, LuaToNet);
            NetToLuaMarshals.Add(T, NetToLua);
        }

        public static MethodInfo GetMethodInfo(Expression<Action> A)
        {
            return (A.Body as MethodCallExpression).Method;
        }

        public static Type GetTypeFromHandle(IntPtr H)
        {
            MethodInfo M = typeof(Type).GetMethod("GetTypeFromHandleUnsafe", BindingFlags.Static | BindingFlags.NonPublic);
            return (Type)M.Invoke(null, new object[] { H });
        }

        public static IntPtr GetHandleFromType(Type T)
        {
            return T.TypeHandle.Value;
        }

        public static lua_CFunction Wrap(Delegate D)
        {
            DelegateHandles.Add(GCHandle.Alloc(D));
            return Wrap(D.Method);
        }

        public static lua_CFunction Wrap(MethodInfo Method)
        {
            MethodInfo PushMethod = GetMethodInfo(() => Push(default(lua_StatePtr), null));
            MethodInfo PopMethod = GetMethodInfo(() => Pop(default(lua_StatePtr), 0, typeof(void)));
            List<Expression> LuaToCS = new List<Expression>();
            ParameterInfo[] Params = Method.GetParameters();

            ParameterExpression L = Expression.Parameter(typeof(lua_StatePtr), "L");

            for (int i = 0; i < Params.Length; i++)
            {
                MethodCallExpression PopMethodCall = Expression.Call(PopMethod, L, Expression.Constant(i + 1),
                    Expression.Constant(Params[i].ParameterType, typeof(Type)));
                LuaToCS.Add(Expression.Convert(PopMethodCall, Params[i].ParameterType));
            }

            List<Expression> Body = new List<Expression>();
            MethodCallExpression Call = Expression.Call(Method, LuaToCS.ToArray());
            if (Method.ReturnType != typeof(void))
                Body.Add(Expression.Call(PushMethod, L, Expression.Convert(Call, typeof(object))));
            else
            {
                Body.Add(Call);
                Body.Add(Expression.Constant(0));
            }

            Expression CFunc = Body[0];
            if (Body.Count > 1)
                CFunc = Expression.Block(Body.ToArray());
            lua_CFunction Func = Expression.Lambda<lua_CFunction>(CFunc, L).Compile();
            DelegateHandles.Add(GCHandle.Alloc(Func));
            return Func;
        }

        public static void SetGlobal(lua_StatePtr L, string Name, object Obj)
        {
            Push(L, Obj);
            Lua.lua_setglobal(L, Name);
        }

        public static T GetGlobal<T>(lua_StatePtr L, string Name)
        {
            Lua.lua_getglobal(L, Name);
            return (T)Pop(L, -1, typeof(T));
        }

        public static void OpenAllLibs(lua_StatePtr L, Assembly Asm)
        {
            Type[] Types = Asm.GetExportedTypes();

            foreach (var T in Types)
            {
                if (T.IsClass && T.IsAbstract && T.IsSealed)
                    OpenLib(L, T);
            }
        }

        public static void OpenLib(lua_StatePtr L, Type StaticType, int NUp = 0, bool SkipInvalid = true)
        {
            if (StaticType.IsClass && StaticType.IsAbstract && StaticType.IsSealed)
            {
                List<luaL_Reg> Regs = new List<luaL_Reg>();
                MethodInfo[] Methods = StaticType.GetMethods(BindingFlags.Static | BindingFlags.Public);
                if (Methods.Length == 0)
                    return;

                LuaRegister LuaReg;
                for (int i = 0; i < Methods.Length; i++)
                {
                    if ((LuaReg = Methods[i].GetCustomAttribute<LuaRegister>()) == null)
                        continue;

                    try
                    {
                        Regs.Add(new luaL_Reg(LuaReg.Name ?? Methods[i].Name, Wrap(Methods[i])));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Regs.Add(luaL_Reg.NULL);

                LuaReg = StaticType.GetCustomAttribute<LuaRegister>();
                Lua.luaL_openlib(L, LuaReg != null ? (LuaReg.Name ?? StaticType.Name) : StaticType.Name, Regs.ToArray(), NUp);
            }
            else
                throw new Exception("Cannot register non static class as library");
        }
    }
}