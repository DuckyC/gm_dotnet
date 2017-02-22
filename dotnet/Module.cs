using GSharp;
using GSharp.NativeClasses;
using GSharp.NativeClasses.VCR;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;

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
        const string net_sockets_sig = "\x2A\x2A\x2A\x2A\x80\x7E\x04\x00\x0F\x84\x2A\x2A\x2A\x2A\xA1\x2A\x2A\x2A\x2A\xC7\x45\xF8\x10";


        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            //VCR_t* VCR = (VCR_t*)InterfaceLoader.LoadVariable<VCR_t>("tier0.dll", "g_pVCR");
            //new_Hook_recvfrom = Hook_recvfrom_detour;
            //GCHandle.Alloc(new_Hook_recvfrom);
            //OHook_recvfrom = InterfaceLoader.OverwriteVCRHook(VCR, new_Hook_recvfrom);

            var netsockptr = SymbolFinder.ResolveOnBinary("engine.dll", net_sockets_sig);
            //netsockptr: 0x00c7d30c

            Console.WriteLine("DotNet loaded");
            return 0;
        }

        static Hook_recvfrom new_Hook_recvfrom;
        static Hook_recvfrom OHook_recvfrom;
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
