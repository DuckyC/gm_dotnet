using System;

namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ReturnIndexAttribute : Attribute
    {
        public int ReturnIndex { get; set; }

        // This is a positional argument
        public ReturnIndexAttribute(int returnIndex)
        {
            this.ReturnIndex = returnIndex;
        }
    }
}
