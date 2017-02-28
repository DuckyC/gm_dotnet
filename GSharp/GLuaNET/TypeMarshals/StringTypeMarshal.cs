using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class StringTypeMarshal : ILuaTypeMarshal
    {
        static StringTypeMarshal()
        {
            GLua.RegisterMarshal(typeof(string), new StringTypeMarshal());
        }

        public object Get(GLua GLua)
        {
            if (GLua.IsType(-1, LuaType.String))
            {
                return GLua.LuaBase.GetString();
            }
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            if (obj is string)
            {
                var str = obj as string;
                GLua.LuaBase.PushString(str, Convert.ToUInt32(str.Length));
            }
        }
    }
}
