using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class BooleanTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua, int stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.Boolean))
            {
                return GLua.LuaBase.GetBool(stackPos);
            }
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            if (obj is bool val)
            {
                GLua.LuaBase.PushBool(val);
            }
        }
    }
}
