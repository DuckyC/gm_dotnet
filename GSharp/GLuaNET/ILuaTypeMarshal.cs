namespace GSharp.GLuaNET
{
    public interface ILuaTypeMarshal
    {
        object Get(GLua GLua, int stackPos = -1);
        void Push(GLua GLua, object obj);
    }
}
