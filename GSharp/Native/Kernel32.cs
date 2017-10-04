using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GSharp.Native.Classes;

namespace GSharp.Native
{

    //Source: https://github.com/cartman300/Libraria/blob/b38562abe1c45bc0e05101f664816702c0d8f072/Libraria.Native/Kernel32.cs
    [Flags]
    public enum MemProtection : uint
    {
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Exec = 0x10,
        ExecRead = 0x20,
        ExecReadWrite = 0x40,
        ExecWriteCopy = 0x80,
        PageGuard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    [Flags]
    public enum AllocationType : uint
    {
        COMMIT = 0x1000,
        RESERVE = 0x2000,
        DECOMMIT = 0x4000,
        RELEASE = 0x8000,
        RESET = 0x80000,
        LARGE_PAGES = 0x20000000,
        PHYSICAL = 0x400000,
        TOP_DOWN = 0x100000,
        WRITE_WATCH = 0x200000
    }

    [Flags]
    public enum MemoryProtection : uint
    {
        EXECUTE = 0x10,
        EXECUTE_READ = 0x20,
        EXECUTE_READWRITE = 0x40,
        EXECUTE_WRITECOPY = 0x80,
        NOACCESS = 0x01,
        READONLY = 0x02,
        READWRITE = 0x04,
        WRITECOPY = 0x08,
        GUARD_Modifierflag = 0x100,
        NOCACHE_Modifierflag = 0x200,
        WRITECOMBINE_Modifierflag = 0x400
    }

    public static class Kernel32
    {
        [DllImport("kernel32")]
        public static extern bool AllocConsole();

        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr Addr, IntPtr Size,
            AllocationType AllocType = AllocationType.COMMIT, MemoryProtection MemProtect = MemoryProtection.EXECUTE_READWRITE);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool VirtualFree(IntPtr Addr, IntPtr Size, AllocationType FreeType);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetProcAddress(IntPtr Lib, string ProcName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetModuleHandle(string ModuleName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool VirtualProtect(IntPtr Addr, uint Size, MemProtection NewProtect, out MemProtection OldProtect);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr LoadLibrary(string Name);

        public static bool VirtualProtect(IntPtr Addr, int Size, MemProtection NewProtect, out MemProtection OldProtect)
        {
            return VirtualProtect(Addr, (uint)Size, NewProtect, out OldProtect);
        }

        public static bool VirtualProtect(IntPtr Addr, uint Size, MemProtection NewProtect)
        {
            MemProtection Old;
            return VirtualProtect(Addr, Size, NewProtect, out Old);
        }

        public static bool VirtualProtect(IntPtr Addr, int Size, MemProtection NewProtect)
        {
            return VirtualProtect(Addr, (uint)Size, NewProtect);
        }

        public static bool VirtualFree(IntPtr Addr)
        {
            return VirtualFree(Addr, IntPtr.Zero, AllocationType.RELEASE);
        }
    }
}