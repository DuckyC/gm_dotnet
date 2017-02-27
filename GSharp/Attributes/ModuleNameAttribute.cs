using System;
namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class ModuleNameAttribute : Attribute
    {
        public string ModuleName { get; set; }

        public ModuleNameAttribute(string moduleName)
        {
            ModuleName = moduleName;
        }
    }
}
