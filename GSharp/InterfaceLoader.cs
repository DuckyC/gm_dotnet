using GSharp.JIT;
using GSharp.NativeClasses;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GSharp {
    public unsafe static class InterfaceLoader {
        [DllImport("kernel32", EntryPoint = "LoadLibrary")]
        private static extern IntPtr LoadLibraryInternal(string path);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(string path);

        private static Dictionary<string, IntPtr> LibraryCache = new Dictionary<string, IntPtr>();

        public static IntPtr LoadLibrary(string path)
        {
            if (LibraryCache.ContainsKey(path)) return LibraryCache[path];

            IntPtr handle = GetModuleHandle(path);
            if (handle == IntPtr.Zero)
                handle = LoadLibraryInternal(path);
            if (handle == IntPtr.Zero)
                return IntPtr.Zero;

            LibraryCache.Add(path, handle);
            return handle;
        }

        private static CreateInterfaceFn LoadCreateInterface(string dllPath)
        {
            var hModule = LoadLibrary(dllPath);
            var functionAddress = GetProcAddress(hModule, "CreateInterface");
            return (CreateInterfaceFn)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(CreateInterfaceFn));
        }

        public static TClass Load<TClass>(string dllname) where TClass : class
        {
            return JITEngine.GetFromFactory<TClass>(LoadCreateInterface(dllname));
        }

        public static IntPtr LoadVariable(string dllname, string variableName) 
        {
            var dllhandle = LoadLibrary(dllname);
            var varHandle = GetProcAddress(dllhandle, variableName);
            return varHandle;
        }

        public static IntPtr LoadVariable<T>(string dllname, string variableName) where T : struct
        {
            return LoadVariable(dllname, variableName) - Marshal.SizeOf(typeof(T));
        }

        public static T OverwriteVCRHook<T>(VCR_t* VCR, T newDelegate)
        {
            var hookName = typeof(T).Name;
            var field = (*VCR).GetType().GetField(hookName);
            var originalFP = (IntPtr)field.GetValue(*VCR);
            var original = Marshal.GetDelegateForFunctionPointer<T>(originalFP);
            var newHookPointer = Marshal.GetFunctionPointerForDelegate(newDelegate);
            field.SetValue(*VCR, newHookPointer);
            return original;
        }

    }
}
