using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class DelegateTypeMarshal : ILuaTypeMarshal
    {
        static DelegateTypeMarshal()
        {
            GLua.RegisterMarshal(typeof(Delegate), new DelegateTypeMarshal());
        }

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
