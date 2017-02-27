using System;
namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class VTableOffsetAttribute : Attribute
    {
        public int Offset { get; set; }

        public VTableOffsetAttribute(int s)
        {
            Offset = s;
        }
    }
}
