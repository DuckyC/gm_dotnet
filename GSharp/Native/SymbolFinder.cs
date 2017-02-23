using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GSharp.Native
{
    public unsafe class SymbolFinder
    {
        private struct DynLibInfo
        {
            public IntPtr baseAddress;
            public uint memorySize;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGE_DOS_HEADER
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public ushort e_magic;       // Magic number
            public UInt16 e_cblp;    // Bytes on last page of file
            public UInt16 e_cp;      // Pages in file
            public UInt16 e_crlc;    // Relocations
            public UInt16 e_cparhdr;     // Size of header in paragraphs
            public UInt16 e_minalloc;    // Minimum extra paragraphs needed
            public UInt16 e_maxalloc;    // Maximum extra paragraphs needed
            public UInt16 e_ss;      // Initial (relative) SS value
            public UInt16 e_sp;      // Initial SP value
            public UInt16 e_csum;    // Checksum
            public UInt16 e_ip;      // Initial IP value
            public UInt16 e_cs;      // Initial (relative) CS value
            public UInt16 e_lfarlc;      // File address of relocation table
            public UInt16 e_ovno;    // Overlay number
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public fixed ushort e_res1[4];    // Reserved words
            public UInt16 e_oemid;       // OEM identifier (for e_oeminfo)
            public UInt16 e_oeminfo;     // OEM information; e_oemid specific
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public fixed ushort e_res2[10];    // Reserved words
            public Int32 e_lfanew;      // File address of new exe header
        }

        private enum MagicType : ushort
        {
            IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b,
            IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b
        }

        private enum SubSystemType : ushort
        {
            IMAGE_SUBSYSTEM_UNKNOWN = 0,
            IMAGE_SUBSYSTEM_NATIVE = 1,
            IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
            IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
            IMAGE_SUBSYSTEM_POSIX_CUI = 7,
            IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
            IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
            IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
            IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
            IMAGE_SUBSYSTEM_EFI_ROM = 13,
            IMAGE_SUBSYSTEM_XBOX = 14

        }
        private enum DllCharacteristicsType : ushort
        {
            RES_0 = 0x0001,
            RES_1 = 0x0002,
            RES_2 = 0x0004,
            RES_3 = 0x0008,
            IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE = 0x0040,
            IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
            IMAGE_DLL_CHARACTERISTICS_NX_COMPAT = 0x0100,
            IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
            IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
            IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
            RES_4 = 0x1000,
            IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
            IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct IMAGE_OPTIONAL_HEADER
        {
            [FieldOffset(0)]
            public MagicType Magic;

            [FieldOffset(2)]
            public byte MajorLinkerVersion;

            [FieldOffset(3)]
            public byte MinorLinkerVersion;

            [FieldOffset(4)]
            public uint SizeOfCode;

            [FieldOffset(8)]
            public uint SizeOfInitializedData;

            [FieldOffset(12)]
            public uint SizeOfUninitializedData;

            [FieldOffset(16)]
            public uint AddressOfEntryPoint;

            [FieldOffset(20)]
            public uint BaseOfCode;

            // PE32 contains this additional field
            [FieldOffset(24)]
            public uint BaseOfData;

            [FieldOffset(28)]
            public uint ImageBase;

            [FieldOffset(32)]
            public uint SectionAlignment;

            [FieldOffset(36)]
            public uint FileAlignment;

            [FieldOffset(40)]
            public ushort MajorOperatingSystemVersion;

            [FieldOffset(42)]
            public ushort MinorOperatingSystemVersion;

            [FieldOffset(44)]
            public ushort MajorImageVersion;

            [FieldOffset(46)]
            public ushort MinorImageVersion;

            [FieldOffset(48)]
            public ushort MajorSubsystemVersion;

            [FieldOffset(50)]
            public ushort MinorSubsystemVersion;

            [FieldOffset(52)]
            public uint Win32VersionValue;

            [FieldOffset(56)]
            public uint SizeOfImage;

            [FieldOffset(60)]
            public uint SizeOfHeaders;

            [FieldOffset(64)]
            public uint CheckSum;

            [FieldOffset(68)]
            public SubSystemType Subsystem;

            [FieldOffset(70)]
            public DllCharacteristicsType DllCharacteristics;

            [FieldOffset(72)]
            public uint SizeOfStackReserve;

            [FieldOffset(76)]
            public uint SizeOfStackCommit;

            [FieldOffset(80)]
            public uint SizeOfHeapReserve;

            [FieldOffset(84)]
            public uint SizeOfHeapCommit;

            [FieldOffset(88)]
            public uint LoaderFlags;

            [FieldOffset(92)]
            public uint NumberOfRvaAndSizes;

            [FieldOffset(96)]
            public IMAGE_DATA_DIRECTORY ExportTable;

            [FieldOffset(104)]
            public IMAGE_DATA_DIRECTORY ImportTable;

            [FieldOffset(112)]
            public IMAGE_DATA_DIRECTORY ResourceTable;

            [FieldOffset(120)]
            public IMAGE_DATA_DIRECTORY ExceptionTable;

            [FieldOffset(128)]
            public IMAGE_DATA_DIRECTORY CertificateTable;

            [FieldOffset(136)]
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;

            [FieldOffset(144)]
            public IMAGE_DATA_DIRECTORY Debug;

            [FieldOffset(152)]
            public IMAGE_DATA_DIRECTORY Architecture;

            [FieldOffset(160)]
            public IMAGE_DATA_DIRECTORY GlobalPtr;

            [FieldOffset(168)]
            public IMAGE_DATA_DIRECTORY TLSTable;

            [FieldOffset(176)]
            public IMAGE_DATA_DIRECTORY LoadConfigTable;

            [FieldOffset(184)]
            public IMAGE_DATA_DIRECTORY BoundImport;

            [FieldOffset(192)]
            public IMAGE_DATA_DIRECTORY IAT;

            [FieldOffset(200)]
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;

            [FieldOffset(208)]
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;

            [FieldOffset(216)]
            public IMAGE_DATA_DIRECTORY Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGE_DATA_DIRECTORY
        {
            public UInt32 VirtualAddress;
            public UInt32 Size;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct IMAGE_NT_HEADERS
        {
            [FieldOffset(0)]
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint Signature;

            [FieldOffset(0)]
            public fixed byte SignatureBytes[4];

            [FieldOffset(0)]
            public fixed char SignatureChars[4];

            [FieldOffset(4)]
            public IMAGE_FILE_HEADER FileHeader;

            [FieldOffset(24)]
            public IMAGE_OPTIONAL_HEADER OptionalHeader;
        }
        private const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;// "MZ";
        private const uint IMAGE_NT_SIGNATURE = 0x00004550;//"PE00";

        [DllImport("kernel32.dll")]
        private static extern uint VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool GetModuleHandleEx(UInt32 dwFlags, string lpModuleName, out IntPtr phModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        public static IntPtr ResolveOnBinary(string name, byte[] data)
        {
            if (data.Length != 0)
            {
                return FindPatternFromBinary(name, data);
            }

            return IntPtr.Zero;
        }

        private static IntPtr FindPatternFromBinary(string name, byte[] pattern)
        {
            var binary = IntPtr.Zero;
            if (GetModuleHandleEx(0, name, out binary) && binary != IntPtr.Zero)
            {
                return FindPattern(binary, pattern);
            }

            return IntPtr.Zero;
        }

        private static IntPtr FindPattern(IntPtr handle, byte[] pattern)
        {
            DynLibInfo lib;
            if (!GetLibraryInfo(handle, out lib))
                return IntPtr.Zero;

            byte* ptr = (byte*)lib.baseAddress;
            byte* end = ptr + lib.memorySize - pattern.Length;

            bool found = true;
            while (ptr < end)
            {
                for (var i = 0; i < pattern.Length; ++i)
                {
                    if (pattern[i] != 0x2A && pattern[i] != ptr[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return (IntPtr)ptr;

                ptr++;
                found = true;
            }

            return IntPtr.Zero;
        }

        private static bool GetLibraryInfo(IntPtr handle, out DynLibInfo lib)
        {
            lib = new DynLibInfo();
            if (handle == IntPtr.Zero)
                return false;


            MEMORY_BASIC_INFORMATION info;
            if (VirtualQuery(handle, out info, Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()) == 0)
                return false;

            //uintptr_t baseAddr = reinterpret_cast<uintptr_t>(info.AllocationBase);

            IMAGE_DOS_HEADER* dos = (IMAGE_DOS_HEADER*)info.AllocationBase;
            IMAGE_NT_HEADERS* pe = (IMAGE_NT_HEADERS*)(info.AllocationBase + dos->e_lfanew);
            IMAGE_FILE_HEADER* file = &pe->FileHeader;
            IMAGE_OPTIONAL_HEADER* opt = &pe->OptionalHeader;

            if (dos->e_magic != IMAGE_DOS_SIGNATURE)
                return false;

            if (pe->Signature != IMAGE_NT_SIGNATURE)
                return false;

            if (opt->Magic != MagicType.IMAGE_NT_OPTIONAL_HDR32_MAGIC)
                return false;

            if (file->Machine != 0x014c)// Intel 386.
                return false;

            if ((file->Characteristics & 0x2000) == 0)// File is a DLL.
                return false;

            lib.memorySize = opt->SizeOfImage;
            lib.baseAddress = info.AllocationBase;
            return true;
        }

    }
}