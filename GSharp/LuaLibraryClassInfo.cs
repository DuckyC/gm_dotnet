using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GSharp.Attributes;

namespace GSharp
{
    public class MethodLuaJITInfo
    {
        public Type ReturnType { get; set; }
        public List<Type> Args { get; set; }
        public string Name { get; set; }
        public MethodInfo MethodInfo { get; set; }

        public MethodLuaJITInfo()
        {

        }

        public MethodLuaJITInfo(MethodInfo method)
        {
            ReturnType = method.ReturnType;
            Name = method.Name;
            MethodInfo = method;

            Args = new List<Type>();

            foreach (ParameterInfo paramInfo in method.GetParameters())
            {
                Args.Add(paramInfo.ParameterType);
            }
        }
    }

    public class ClassLuaJITInfo
    {
        public List<MethodLuaJITInfo> Methods { get; set; } = new List<MethodLuaJITInfo>();

        public ClassLuaJITInfo()
        {
             
        }

        public ClassLuaJITInfo(Type classType)
        {
            MethodInfo[] methods = classType.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                Methods.Add(new MethodLuaJITInfo(methods[i]));
            }
        }
    }
}
