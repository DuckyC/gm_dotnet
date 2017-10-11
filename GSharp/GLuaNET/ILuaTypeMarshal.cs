namespace GSharp.GLuaNET
{
    public interface ILuaTypeMarshal
    {
        object Get(GLua GLua);
        void Push(GLua GLua, object obj);
    }
}
