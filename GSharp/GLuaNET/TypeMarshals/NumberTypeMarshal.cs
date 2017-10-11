using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class NumberTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua)
        {
            if (GLua.IsType(-1, LuaType.Number))
            {
                return GLua.LuaBase.GetNumber();
            }
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            GLua.LuaBase.PushNumber(Convert.ToDouble(obj));
        }
    }
}
