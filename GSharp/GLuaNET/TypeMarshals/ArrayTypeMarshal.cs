namespace GSharp.GLuaNET.TypeMarshals
{
    public class ArrayTypeMarshal<T> : ILuaTypeMarshal where T : class
    {
        public object Get(GLua GLua)
        {
            GLua.GetArray<T>();
            return null;
        }

        public void Push(GLua GLua, object obj)
        {
            GLua.SetArray<T>(obj as T[]);
        }
    }
}
