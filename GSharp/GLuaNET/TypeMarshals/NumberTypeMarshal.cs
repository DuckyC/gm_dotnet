using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class NumberTypeMarshal : ILuaTypeMarshal
    {
        static NumberTypeMarshal()
        {
            GLua.RegisterMarshal(typeof(double), new NumberTypeMarshal());
            GLua.RegisterMarshal(typeof(float), new NumberTypeMarshal());
            GLua.RegisterMarshal(typeof(int), new NumberTypeMarshal());
            GLua.RegisterMarshal(typeof(long), new NumberTypeMarshal());
        }

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
