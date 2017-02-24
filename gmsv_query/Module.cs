using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;

namespace gmsv_query
{
    public unsafe static class Module
    {
        static uint udpSock;
        static byte[] staticInfoPacket;

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            VCR_t* VCR = (VCR_t*)NativeInterface.LoadVariable<VCR_t>("tier0.dll", "g_pVCR");
            OHook_recvfrom = NativeInterface.OverwriteVCRHook(VCR, new_Hook_recvfrom = Hook_recvfrom_detour);
            var netsock = NetSocket.GetNetSocket();

            staticInfoPacket = (new ReplyInfoPacket
            {
                AmountBots = 10,
                AmountClients = 5,
                Appid = 4020,
                GameDirectory = "garrysmod",
                GamemodeName = "infinite wars",
                GameName = "this is my server name?",
                GameVersion = ReplyInfoPacket.default_game_version,
                MapName = "gm_fuckmynuts",
                MaxClients = 120,
                OS = ReplyInfoPacket.OSType.Windows,
                Passworded = false,
                Secure = false,
                Server = ReplyInfoPacket.ServerType.Dedicated,
                UDPPort = (short)netsock->nPort,
                SteamID = 0,
                Tags = "ayyy"

            }).GetBytes();

            Console.WriteLine("DotNet loaded");
            return 0;
        }

        static Hook_recvfrom new_Hook_recvfrom;
        static Hook_recvfrom OHook_recvfrom;
        public static int Hook_recvfrom_detour(int s, byte* buf, int len, int flags, IntPtr from, int fromlen)
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
                        NativeSocket.SendTo(udpSock, staticInfoPacket, staticInfoPacket.Length, 0, from, fromlen);
                        return -1;
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
