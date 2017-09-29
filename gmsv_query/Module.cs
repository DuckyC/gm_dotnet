using GSharp;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace gmsv_query
{
    public unsafe static class Module
    {
        static uint udpSock;
        static byte[] staticInfoPacket;
        static byte[] staticPlayerPacket;

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            VCR_t* VCR = (VCR_t*)NativeInterface.LoadVariable<VCR_t>("tier0.dll", "g_pVCR");
            OHook_recvfrom = NativeInterface.OverwriteVCRHook(VCR, new_Hook_recvfrom = Hook_recvfrom_detour);
            var netsock = Symbols.GetNetSocket();
            udpSock = netsock->hUDP;

            var iserver = Symbols.GetIServer();
            var gamedll = NativeInterface.Load<IServerGameDLL>();
            var filesystem = NativeInterface.Load<IFileSystem>();
            var engineServer = NativeInterface.Load<IVEngineServer>();

            var description = gamedll.GetGameDescription();
            


            var infoPacket = new ReplyInfoPacket
            {
                AmountBots = 0,
                AmountClients = 50,
                Appid = engineServer.GetAppID(),
                GameDirectory = "garrysmod",
                GamemodeName = "infinite wars",
                GameName = "this is my server name?",
                GameVersion = ReplyInfoPacket.default_game_version,
                MapName = "gm_fuckmynuts",
                MaxClients = 60,
                OS = ReplyInfoPacket.OSType.Windows,
                Passworded = false,
                Secure = false,
                Server = ReplyInfoPacket.ServerType.Dedicated,
                UDPPort = (short)netsock->nPort,
                SteamID = engineServer.GetGameServerSteamID(),
                Tags = "ayyy"

            };

            var playerPacket = new ReplyPlayerPacket
            {
                Players = new List<ReplyPlayer>
                {
                    new ReplyPlayer{Name = "Duck", Score = 123, Time = 10},
                    new ReplyPlayer{Name = "Fuck", Score = 456, Time = 9},
                    new ReplyPlayer{Name = "Suck", Score = 789, Time = 8},
                }
            };

            staticInfoPacket = infoPacket.GetPacket();
            staticPlayerPacket = playerPacket.GetPacket();

            Console.WriteLine("DotNet Query loaded");
            return 0;
        }

        public static int HandleNetError(int len)
        {
            if(len == -1)
            {
                NativeSocket.WSASetLastError(NativeSocket.WSAEWOULDBLOCK);
            }
            return len;
        }

        public static int HandlePacket(int s, byte* buf, int buflen, int flags, IntPtr from, IntPtr fromlenptr)
        {
            if (buflen == -1)
            {
                return -1;
            }
            var type = ClassifyPacket(buf, buflen);

           
            switch (type)
            {
                case PacketType.Good:
                default:
                    var len = OHook_recvfrom(s, buf, buflen, flags, from, fromlenptr);
                    return len;
                case PacketType.Invalid:
                    return -1;
                case PacketType.Info:
                    var addr = (SockAddrIn*)from;
                    var IP = new IPAddress(addr->Addr);
                    //Console.WriteLine("A2S_INFO REQUEST: " + IP);
                    NativeSocket.SendTo(udpSock, staticInfoPacket, staticInfoPacket.Length, 0, from, *((int*)fromlenptr));
                    return -1;
                case PacketType.Player:
                    NativeSocket.SendTo(udpSock, staticPlayerPacket, staticPlayerPacket.Length, 0, from, *((int*)fromlenptr));
                    return -1;
            }
        }

        static Hook_recvfrom new_Hook_recvfrom;
        static Hook_recvfrom OHook_recvfrom;
        public static int Hook_recvfrom_detour(int s, byte* buf, int len, int flags, IntPtr from, IntPtr fromlenptr)
        {
            return HandleNetError(HandlePacket(s, buf, len, flags, from, fromlenptr));
        }

        public enum PacketType
        {
            Invalid = -1,
            Good,
            Info,
            Player,
        }

        public static PacketType ClassifyPacket(byte* data, int len)
        {
            if(len == 0 || len == -1)
                return PacketType.Invalid;
            if (len < 5)
                return PacketType.Good;

            var channel = *((int*)data);
            if (channel == -2)
                return PacketType.Invalid;
            if (channel != -1)
                return PacketType.Good;

            var challenge = *((int*)(data + 5));
            if (challenge == -1)
                return PacketType.Good;

            char type = (char)(*((byte*)(data + 4)));
            switch (type)
            {
                case 'T':
                    return PacketType.Info;
                case 'U':
                    return PacketType.Player;
                case 'W':
                default:
                    return PacketType.Good;
            }

        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
