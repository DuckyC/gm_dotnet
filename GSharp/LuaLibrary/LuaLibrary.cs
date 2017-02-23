using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace GSharp.LuaLibrary
{
    public static class LuaLibrary
    {
        private static ModuleBuilder moduleBuilder;

        static LuaLibrary()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("JIT"), AssemblyBuilderAccess.RunAndSave);

#if DEBUG
            Type daType = typeof(DebuggableAttribute);
            ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
            CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] {
            DebuggableAttribute.DebuggingModes.DisableOptimizations |
            DebuggableAttribute.DebuggingModes.Default });
            assemblyBuilder.SetCustomAttribute(daBuilder);
#endif

            moduleBuilder = assemblyBuilder.DefineDynamicModule("LuaLibraryJIT", true);
        }

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

        //TODO: REDO THIS WITH GLUA
        //public static TClass WrapLibrary<TClass>(this GLua GLua, string luaAccessor)
        //{

        //    var luaType = typeof(GLua);
        //    var targetInterface = typeof(TClass);

        //    var builder = moduleBuilder.DefineType(targetInterface.Name + "_" + (IntPtr.Size * 8).ToString(), TypeAttributes.Class, null, new Type[] { targetInterface });
        //    builder.AddInterfaceImplementation(targetInterface);

        //    var luaStateField = builder.DefineField("LuaState", typeof(lua_StatePtr), FieldAttributes.Public);

        //    var classInfo = new ClassLuaJITInfo(targetInterface);

        //    foreach (var method in classInfo.Methods)
        //    {
        //        MethodBuilder mbuilder = builder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis);

        //        mbuilder.SetReturnType(method.ReturnType);
        //        mbuilder.SetParameters(method.Args.ToArray());
        //        builder.DefineMethodOverride(mbuilder, method.MethodInfo);
        //        var ilgen = mbuilder.GetILGenerator();

        //        ilgen.Emit(OpCodes.Nop);
        //        ilgen.Emit(OpCodes.Ldarg_0);
        //        ilgen.Emit(OpCodes.Ldfld, luaStateField);
        //        ilgen.Emit(OpCodes.Ldstr, luaAccessor);
        //        ilgen.EmitCall(OpCodes.Call, luaType.GetMethod(nameof(Lua.lua_getglobal)), null);
        //        ilgen.Emit(OpCodes.Nop);

        //        ilgen.Emit(OpCodes.Ldarg_0);
        //        ilgen.Emit(OpCodes.Ldfld, luaStateField);
        //        ilgen.Emit(OpCodes.Ldc_I4_M1);
        //        ilgen.Emit(OpCodes.Ldstr, method.Name);
        //        ilgen.EmitCall(OpCodes.Call, luaType.GetMethod(nameof(Lua.lua_getfield)), null);
        //        ilgen.Emit(OpCodes.Nop);
        //        for (int i = 0; i < method.Args.Count; i++)
        //        {
        //            var param = method.Args[i];
        //            ilgen.Emit(OpCodes.Ldarg_0);
        //            ilgen.Emit(OpCodes.Ldfld, luaStateField);
        //            ilgen.EmitLoadArg(i + 1);
        //            ilgen.EmitCall(OpCodes.Call, typeof(LuaAdvanced).GetMethod(nameof(LuaAdvanced.Push)), null);
        //            ilgen.Emit(OpCodes.Pop);
        //        }

        //        ilgen.Emit(OpCodes.Ldarg_0);
        //        ilgen.Emit(OpCodes.Ldfld, luaStateField);
        //        ilgen.Emit(OpCodes.Ldc_I4, method.Args.Count);
        //        ilgen.Emit(OpCodes.Ldc_I4_0);
        //        ilgen.Emit(OpCodes.Ldc_I4_0);
        //        ilgen.EmitCall(OpCodes.Call, luaType.GetMethod(nameof(Lua.lua_pcall)), null);
        //        ilgen.Emit(OpCodes.Pop);
        //        ilgen.Emit(OpCodes.Ret);

        //    }

        //    Type implClass = builder.CreateType();
        //    var instClass = Activator.CreateInstance(implClass);

        //    var luaStateInstanceField = implClass.GetField("LuaState", BindingFlags.Public | BindingFlags.Instance);
        //    luaStateInstanceField.SetValue(instClass, L);

        //    return (TClass)instClass;
        //}
    }
}
