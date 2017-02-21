using System;
namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class InterfaceVersionAttribute : Attribute
    {
        public string Identifier { get; set; }

        public InterfaceVersionAttribute(string versionIdentifier)
        {
            Identifier = versionIdentifier;
        }
    }
}
