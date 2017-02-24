using GSharp.Native.JIT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GSharp.Native.Classes;

namespace GSharp.Native
{
    public unsafe static class NativeInterface
    {
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

        private static CreateInterfaceDelegate LoadCreateInterface(string dllPath)
        {
            var hModule = LoadLibrary(dllPath);
            var functionAddress = GetProcAddress(hModule, "CreateInterface");
            return (CreateInterfaceDelegate)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(CreateInterfaceDelegate));
        }

        public static TClass Load<TClass>(string dllname) where TClass : class
        {
            return JITEngine.GetFromFactory<TClass>(LoadCreateInterface(dllname));
        }

        public static TClass Load<TClass>(string dllname, string interfaceVersionString) where TClass : class
        {
            var factory = LoadCreateInterface(dllname);
            var classptr = factory(interfaceVersionString, IntPtr.Zero);
            return JITEngine.GenerateClass<TClass>(classptr);
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

        public static T OverwriteVCRHook<T>(IntPtr VCR, T newDelegate) where T : class
        {
            var hookName = typeof(T).Name;
            if (typeof(VCR_t).GetField(hookName) == null)
                throw new Exception("Could not find hook " + hookName);
            int offset = (int)Marshal.OffsetOf<VCR_t>(hookName);

            var original = Marshal.GetDelegateForFunctionPointer<T>(Marshal.ReadIntPtr(VCR, offset));
            var newHookPointer = Marshal.GetFunctionPointerForDelegate(newDelegate);

            Marshal.WriteIntPtr(VCR, offset, newHookPointer);
            return original;
        }

        public static T OverwriteVCRHook<T>(VCR_t* VCR, T newDelegate) where T : class
        {
            return OverwriteVCRHook((IntPtr)VCR, newDelegate);
        }

    }
}
