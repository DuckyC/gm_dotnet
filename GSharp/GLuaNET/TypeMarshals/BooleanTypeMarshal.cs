using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class BooleanTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua)
        {
            if (GLua.IsType(-1, LuaType.Boolean))
            {
                return GLua.LuaBase.GetBool(-1);
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
