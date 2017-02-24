using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GSharp
{
    public static class Wrapper
    {

        public static IEnumerable<Type> GetAllDeclaredTypesUsed(Type type, HashSet<Type> typesUsed = null)
        {
            typesUsed = typesUsed ?? new HashSet<Type>();
            var declaredAssembly = type.Assembly.FullName;
            Action<Type> addType = null; // because recursive
            addType = (pierceType) =>
            {
                pierceType = (pierceType.IsByRef || pierceType.IsArray) ? pierceType.GetElementType() : pierceType;
                if (pierceType.Assembly.FullName == declaredAssembly && !pierceType.IsGenericParameter)
                {
                    if (typesUsed.Add(pierceType))
                    {
                        foreach (var t2 in GetAllDeclaredTypesUsed(pierceType, typesUsed))
                        {
                            addType(pierceType);
                        }
                    }
                }
            };

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                addType(field.FieldType);
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                addType(property.PropertyType);
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                addType(method.ReturnType);
                foreach (var parameter in method.GetParameters())
                {
                    addType(parameter.ParameterType);
                }
            }

            return typesUsed;
        }

        public static void WrapType()
        {

        }
    }
}
