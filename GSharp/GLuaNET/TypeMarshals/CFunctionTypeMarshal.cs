using GSharp.Generated.NativeClasses;
using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class CFunctionTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua, int stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.Function))
            {
                return GLua.LuaBase.GetCFunction(stackPos);
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
