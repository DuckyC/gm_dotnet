using GSharp.JIT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GSharp
{
    public static class InterfaceLoader
    {
        [DllImport("Kernel32.dll", EntryPoint = "LoadLibrary")]
        private static extern IntPtr LoadLibraryInternal(string path);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        private static Dictionary<string, IntPtr> LibraryCache = new Dictionary<string, IntPtr>();

        private static IntPtr LoadLibrary(string path)
        {
            if (LibraryCache.ContainsKey(path)) return LibraryCache[path];
            var handle = LoadLibraryInternal(path);
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

            var CreateInterface = LoadCreateInterface(dllname);

            IntPtr address = CreateInterface(InterfaceVersions.GetInterfaceIdentifier(typeof(TClass)), IntPtr.Zero);
            if (address == IntPtr.Zero)
                return default(TClass);

            var ret = JITEngine.GenerateClass<TClass>(address);
            return ret;
        }

        public static IntPtr LoadVariable(string dllname, string variableName)
        {
            var handle = LoadLibrary(dllname);
            var varHandle = GetProcAddress(handle, variableName);
            return varHandle;
            
        }
    }
}
