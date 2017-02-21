using GSharp.NativeClasses;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using GSharp;

namespace dotnet
{
    enum PacketType
    {
        Ignore = -1,
        Good,
        Info,
        Player,
        Fake,
    };

    public unsafe static class Module
    {
        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            VCR_t* VCR = (VCR_t*)InterfaceLoader.LoadVariable<VCR_t>("tier0.dll", "g_pVCR");

            OHook_recvfrom = Marshal.GetDelegateForFunctionPointer<Hook_recvfrom_func>(VCR->Hook_recvfrom);
            Hook_recvfrom_func d = Hook_recvfrom_detour;
            GCHandle.Alloc(d);
            new_Hook_recvfrom = Marshal.GetFunctionPointerForDelegate(d);
            VCR->Hook_recvfrom = new_Hook_recvfrom;

            Console.WriteLine("DotNet loaded");
            return 0;
        }

        static IntPtr new_Hook_recvfrom;
        static Hook_recvfrom_func OHook_recvfrom;
        public static int Hook_recvfrom_detour(int s, byte* buf, int len, int flags, IntPtr from, IntPtr fromlen)
        {
            var channel = (int*)buf;
            var challenge = (int*)(buf + 5);
            var type = (byte*)(buf + 4);
            if (*channel == -1)
            {
                if (*challenge != -1)
                {
                    if (*type == 'T')
                    {

                    }
                }
            }

            return OHook_recvfrom(s, buf, len, flags, from, fromlen);
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
