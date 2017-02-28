using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class CFunctionTypeMarshal : ILuaTypeMarshal
    {
        static CFunctionTypeMarshal()
        {
            GLua.RegisterMarshal(typeof(lua_CFunction), new CFunctionTypeMarshal());
        }

        public object Get(GLua GLua)
        {
            if (GLua.IsType(-1, LuaType.Function))
            {
                return GLua.LuaBase.GetCFunction();
            }
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            if (obj is lua_CFunction)
            {
                var func = obj as lua_CFunction;
                throw new NotImplementedException(); //ILuaBase is missing PushCFunction because JitEngine doesnt support delagates as parameters
                //GLua.LuaBase.PushCFunction(func);
            }
        }
    }
}
