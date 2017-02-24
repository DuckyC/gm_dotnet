using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.Classes.VCR;
using RGiesecke.DllExport;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace dotnet
{

    public static class Win
    {
        [DllImport("ws2_32.dll", EntryPoint = "sendto")]
        public static extern int SendTo(uint Socket, byte[] buff, int len, int flags, ref SockAddr To, int tomlen);
    }

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
        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void WriteNullTerminatedString(this BinaryWriter bw, string str)
        {
            bw.Write(Encoding.UTF8.GetBytes(str));
            bw.Write((byte)0x00);
        }


        static byte[] net_sockets_sig = new byte[] { 0x2A, 0x2A, 0x2A, 0x2A, 0x80, 0x7E, 0x04, 0x00, 0x0F, 0x84, 0x2A, 0x2A, 0x2A, 0x2A, 0xA1, 0x2A, 0x2A, 0x2A, 0x2A, 0xC7, 0x45, 0xF8, 0x10 };

        [StructLayout(LayoutKind.Sequential)]
        struct netsocket_t
        {
            public int nPort;
            public bool bListening;
            public uint hUDP;
            public uint hTCP;
        };

        static uint udpPort;
        static byte[] staticInfoPacket;

        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            VCR_t* VCR = (VCR_t*)NativeInterface.LoadVariable<VCR_t>("tier0.dll", "g_pVCR");
            OHook_recvfrom = NativeInterface.OverwriteVCRHook(VCR, new_Hook_recvfrom = Hook_recvfrom_detour);
            //old_Hook_Cmd_Exec = InterfaceLoader.OverwriteVCRHook(VCR, new_Hook_Cmd_Exec = Hook_Cmd_Exec);

            CUtlVector netsocks = SymbolFinder.ResolveOnBinary("engine.dll", net_sockets_sig);
            netsocket_t* first = (netsocket_t*)netsocks[0];
            netsocket_t* second = (netsocket_t*)netsocks[1];

            udpPort = first->hUDP;

            

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

        static Hook_Cmd_Exec new_Hook_Cmd_Exec;
        static Hook_Cmd_Exec old_Hook_Cmd_Exec;
        public static void Hook_Cmd_Exec(string[] Args)
        {
            old_Hook_Cmd_Exec(Args);
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }


    public class ReplyInfoPacket
    {
        const string default_game_version = "16.02.26";
        const byte default_proto_version = 17;

        public string GameName { get; set; }
        public string MapName { get; set; }
        public string GameDirectory { get; set; }
        public string GamemodeName { get; set; }
        public byte AmountClients { get; set; }
        public byte MaxClients { get; set; }
        public byte AmountBots { get; set; }
        public byte ServerType { get; set; }
        public byte OsType { get; set; }
        public bool Passworded { get; set; }
        public bool Secure { get; set; }
        public string GameVersion { get; set; }
        public short UDPPort { get; set; }
        public string Tags { get; set; }
        public long Appid { get; set; }
        public long SteamID { get; set; }

        public byte[] GetBytes()
        {
            using (var stream = new MemoryStream())
            {
                using (var binary = new BinaryWriter(stream))
                {
                    binary.Write((long)-1); // connectionless packet header
                    binary.Write(Encoding.UTF8.GetBytes("I")[0]); // packet type is always 'I'
                    binary.Write(default_proto_version);

                    binary.WriteNullTerminatedString(GameName); //TODO: make strings null terminated
                    binary.WriteNullTerminatedString(MapName);
                    binary.WriteNullTerminatedString(GameDirectory);
                    binary.WriteNullTerminatedString(GamemodeName);

                    binary.Write((short)Appid);

                    binary.Write(AmountClients);
                    binary.Write(MaxClients);
                    binary.Write(AmountBots);
                    binary.Write(ServerType);
                    binary.Write(OsType);
                    binary.Write((byte)(Passworded ? 1 : 0));

                    // if vac protected, it activates itself some time after startup
                    binary.Write((byte)(Secure ? 1 : 0));
                    binary.WriteNullTerminatedString(GameVersion);

                    if (string.IsNullOrEmpty(Tags))
                    {
                        // 0x80 - port number is present
                        // 0x10 - server steamid is present
                        // 0x01 - game long appid is present
                        binary.Write((byte)(0x80 | 0x10 | 0x01));
                        binary.Write(UDPPort);
                        binary.Write(SteamID);
                        binary.Write(Appid);
                    }
                    else
                    {
                        // 0x80 - port number is present
                        // 0x10 - server steamid is present
                        // 0x20 - tags are present
                        // 0x01 - game long appid is present
                        binary.Write((byte)(0x80 | 0x10 | 0x20 | 0x01));
                        binary.Write(UDPPort);
                        binary.Write(SteamID);
                        binary.WriteNullTerminatedString(Tags); 
                        binary.Write(Appid);
                    }


                    return stream.ReadFully();
                }
            }
        }
    }
}
