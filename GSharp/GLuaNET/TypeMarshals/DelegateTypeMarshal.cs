using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class DelegateTypeMarshal : ILuaTypeMarshal
    {
        public object Get(GLua GLua)
        {
            throw new NotImplementedException();
        }

        public void Push(GLua GLua, object obj)
        {
            //something something wrap something;
        }
    }
}
