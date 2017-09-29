using GSharp;
using System.IO;
using System.Text;

namespace gmsv_query
{
    public class ReplyInfoPacket
    {
        public enum ServerType : byte
        {
            Dedicated = 0x64, //UTF8 d
            Listen = 0x6C, // UTF8 l
            SourceTV = 0x70,//UTF8 p
        }

        public enum OSType : byte
        {
            Linux = 0x6C, //UTF8 l
            Windows = 0x77, //UTF8 w
            Mac = 0x6D//UTF8 m
        }

        public const string default_game_version = "16.02.26";
        public const byte default_proto_version = 17;

        public string GameName { get; set; }
        public string MapName { get; set; }
        public string GameDirectory { get; set; }
        public string GamemodeName { get; set; }
        public byte AmountClients { get; set; }
        public byte MaxClients { get; set; }
        public byte AmountBots { get; set; }
        public ServerType Server { get; set; }
        public OSType OS { get; set; }
        public bool Passworded { get; set; }
        public bool Secure { get; set; }
        public string GameVersion { get; set; }
        public short UDPPort { get; set; }
        public string Tags { get; set; }
        public long Appid { get; set; }
        public long SteamID { get; set; }

        public byte[] GetPacket()
        {
            using (var buffer = new ValveBuffer())
            {
                buffer.WriteLong(-1); // non-split packet header
                buffer.WriteByte(Encoding.UTF8.GetBytes("I")[0]); // packet type is always 'I'
                buffer.WriteByte(default_proto_version);

                buffer.WriteString(GameName);
                buffer.WriteString(MapName);
                buffer.WriteString(GameDirectory);
                buffer.WriteString(GamemodeName);

                buffer.WriteShort((short)Appid);

                buffer.WriteByte(AmountClients);
                buffer.WriteByte(MaxClients);
                buffer.WriteByte(AmountBots);
                buffer.WriteByte((byte)Server);
                buffer.WriteByte((byte)OS);
                buffer.WriteByte((byte)(Passworded ? 1 : 0));
                buffer.WriteByte((byte)(Secure ? 1 : 0));
                buffer.WriteString(GameVersion);

                if (string.IsNullOrEmpty(Tags))
                {
                    // 0x80 - port number is present
                    // 0x10 - server steamid is present
                    // 0x01 - game long appid is present
                    buffer.WriteByte((byte)(0x80 | 0x10 | 0x01));
                    buffer.WriteShort(UDPPort);
                    buffer.WriteLongLong(SteamID);
                    buffer.WriteLongLong(Appid);
                }
                else
                {
                    // 0x80 - port number is present
                    // 0x10 - server steamid is present
                    // 0x20 - tags are present
                    // 0x01 - game long appid is present
                    buffer.WriteByte((byte)(0x80 | 0x10 | 0x20 | 0x01));
                    buffer.WriteShort(UDPPort);
                    buffer.WriteLongLong(SteamID);
                    buffer.WriteString(Tags);
                    buffer.WriteLongLong(Appid);
                }

                return buffer.ToArray();
            }
        }
    }
}
