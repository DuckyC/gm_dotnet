using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class StringTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua, int stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.String))
            {
                return GLua.LuaBase.GetString(stackPos, IntPtr.Zero);
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
