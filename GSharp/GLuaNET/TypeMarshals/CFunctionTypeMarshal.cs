using GSharp.Generated.NativeClasses;
using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class CFunctionTypeMarshal : ILuaTypeMarshal
    {
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
            if (obj is CFunc func)
            {
                GLua.PushCFunction(func);
            }
        }
    }
}
