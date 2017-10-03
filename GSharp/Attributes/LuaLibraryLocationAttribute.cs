using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class LuaLibraryLocationAttribute : Attribute
    {
        public string Path { get; set; }

        public LuaLibraryLocationAttribute(string path)
        {
            Path = path;
        }
    }
}
