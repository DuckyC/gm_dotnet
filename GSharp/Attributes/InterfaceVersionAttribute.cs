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

    public class InterfaceVersions
    {
        public static string GetInterfaceIdentifier(Type targetClass)
        {
            foreach (InterfaceVersionAttribute attribute in targetClass.GetCustomAttributes(typeof(InterfaceVersionAttribute), false))
            {
                return attribute.Identifier;
            }

            throw new Exception("Version identifier not found for class " + targetClass);
        }
    }
}
