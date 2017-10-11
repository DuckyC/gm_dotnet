using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using GSharp.GLuaNET;
using GSharp.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace GSharp.GLuaNET
{
    public partial class GLua
    {
        private static HashSet<string> JITNames = new HashSet<string>();

        private static ModuleBuilder moduleBuilder;

        private static void InitJIT()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("JIT"), AssemblyBuilderAccess.RunAndSave);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("LuaLibraryJIT", true);
        }

        private Dictionary<Tuple<Type, string>, Type> Libraries = new Dictionary<Tuple<Type, string>, Type>();

        public TClass WrapLibrary<TClass>(string globalName = null)
        {

            var targetInterface = typeof(TClass);

            var locationattr = targetInterface.GetCustomAttribute(typeof(LuaLibraryLocationAttribute)) as LuaLibraryLocationAttribute;
            if (globalName == null && locationattr == null)
            {
                throw new Exception("Define either LuaLibraryLocationAttribute or globalName");
            }
            if (locationattr != null) { globalName = locationattr.Path; }

            Type implClass;

            var existing = Libraries.Where(kv => kv.Key.Item1 == targetInterface && kv.Key.Item2 == globalName).Select(kv => kv.Value).FirstOrDefault();
            if (existing != null)
            {
                implClass = existing;
            }
            else
            {
                var interfaceName = targetInterface.Name + "_" + (IntPtr.Size * 8).ToString();

                while (JITNames.Contains(interfaceName))
                {
                    interfaceName += "X"; // im so super lazy fuck you
                }
                JITNames.Add(interfaceName);

                var builder = moduleBuilder.DefineType(interfaceName, TypeAttributes.Class, null, new Type[] { targetInterface });
                builder.AddInterfaceImplementation(targetInterface);

                var GLuaField = builder.DefineField("GLua", typeof(GLua), FieldAttributes.Public);

                var classInfo = new ClassLuaJITInfo(targetInterface);

                var getFieldMethodInfo = typeof(GLua).GetMethod(nameof(GLua.GetField));
                var pushMethodInfo = typeof(GLua).GetMethods().Where(m => m.Name == nameof(GLua.Push) && m.ContainsGenericParameters).FirstOrDefault();
                var getMethodInfo = typeof(GLua).GetMethods().Where(m => m.Name == nameof(GLua.Get) && m.ContainsGenericParameters).FirstOrDefault();
                var pcallMethodInfo = typeof(GLua).GetMethod(nameof(GLua.PCall));

                foreach (var method in classInfo.Methods)
                {
                    MethodBuilder mbuilder = builder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis);

                    mbuilder.SetReturnType(method.ReturnType);
                    mbuilder.SetParameters(method.Args.ToArray());
                    builder.DefineMethodOverride(mbuilder, method.MethodInfo);
                    var ilgen = mbuilder.GetILGenerator();

                    ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);
                    ilgen.EmitLDC(LUA_GLOBALSINDEX);
                    ilgen.Emit(OpCodes.Ldstr, globalName);
                    ilgen.EmitCall(OpCodes.Callvirt, getFieldMethodInfo, null);

                    ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);
                    ilgen.Emit(OpCodes.Ldc_I4_M1);
                    ilgen.Emit(OpCodes.Ldstr, method.Name);
                    ilgen.EmitCall(OpCodes.Callvirt, getFieldMethodInfo, null);

                    for (int i = 0; i < method.Args.Count; i++)
                    {
                        var param = method.Args[i];
                        ilgen.Emit(OpCodes.Nop);
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Ldfld, GLuaField);
                        ilgen.EmitLoadArg(i + 1);
                        ilgen.EmitCall(OpCodes.Callvirt, pushMethodInfo.MakeGenericMethod(param), null);
                    }

                    ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);
                    ilgen.EmitLDC(method.Args.Count);
                    if (method.ReturnType != typeof(void)) { ilgen.Emit(OpCodes.Ldc_I4_1); } else { ilgen.Emit(OpCodes.Ldc_I4_0); }
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                    ilgen.EmitCall(OpCodes.Callvirt, pcallMethodInfo, null);
                    ilgen.Emit(OpCodes.Pop);

                    if (method.ReturnType != typeof(void))
                    {

                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Ldfld, GLuaField);
                        ilgen.EmitCall(OpCodes.Callvirt, getMethodInfo.MakeGenericMethod(method.ReturnType), null);

                    }
                    ilgen.Emit(OpCodes.Ret);

                }

                implClass = builder.CreateType();

                Libraries.Add(new Tuple<Type, string>(targetInterface, globalName), implClass);

            }

            var instClass = Activator.CreateInstance(implClass);
            var GLuaInstanceField = implClass.GetField("GLua", BindingFlags.Public | BindingFlags.Instance);
            GLuaInstanceField.SetValue(instClass, this);

            return (TClass)instClass;
        }
    }

    public static class ILExtensions
    {
        public static void EmitLoadArg(this ILGenerator ilgen, int index)
        {
            switch (index)
            {
                case 0: ilgen.Emit(OpCodes.Ldarg_0); break;
                case 1: ilgen.Emit(OpCodes.Ldarg_1); break;
                case 2: ilgen.Emit(OpCodes.Ldarg_2); break;
                case 3: ilgen.Emit(OpCodes.Ldarg_3); break;
                default: ilgen.Emit(OpCodes.Ldarg_S, (byte)index); break;
            }
        }

        public static void EmitLDC(this ILGenerator ilgen, int index)
        {
            switch (index)
            {
                case -1: ilgen.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: ilgen.Emit(OpCodes.Ldc_I4_0); break;
                case 1: ilgen.Emit(OpCodes.Ldc_I4_1); break;
                case 2: ilgen.Emit(OpCodes.Ldc_I4_2); break;
                case 3: ilgen.Emit(OpCodes.Ldc_I4_3); break;
                case 4: ilgen.Emit(OpCodes.Ldc_I4_4); break;
                case 5: ilgen.Emit(OpCodes.Ldc_I4_5); break;
                case 6: ilgen.Emit(OpCodes.Ldc_I4_6); break;
                case 7: ilgen.Emit(OpCodes.Ldc_I4_7); break;
                case 8: ilgen.Emit(OpCodes.Ldc_I4_8); break;
                default: ilgen.Emit(OpCodes.Ldc_I4, index); break;
            }
        }
    }
}
