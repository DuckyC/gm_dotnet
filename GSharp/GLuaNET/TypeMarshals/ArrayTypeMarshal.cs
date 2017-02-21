namespace GSharp.GLuaNET.TypeMarshals
{
    public class ArrayTypeMarshal<T> : ILuaTypeMarshal where T : class
    {
        static ArrayTypeMarshal()
        {
            GLua.RegisterMarshal(typeof(string[]), new ArrayTypeMarshal<string>());
        }

        public object Get(GLua GLua)
        {
            GLua.GetArray<T>();
            return null;
        }

        public void Set(GLua GLua, object obj)
        {
            GLua.SetArray<T>(obj as T[]);
        }
    }
}
