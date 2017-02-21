namespace GSharp.GLuaNET.TypeMarshals
{
    public interface ILuaTypeMarshal
    {
        object Get(GLua GLua);
        void Set(GLua GLua, object obj);
    }
}
