using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class NumberTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua, int stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.Number))
            {
                return GLua.LuaBase.GetNumber(stackPos);
            }
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            GLua.LuaBase.PushNumber(Convert.ToDouble(obj));
        }
    }
}
