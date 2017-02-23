using System;
using GSharp.Attributes;

namespace GSharp.Native.JIT
{
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
