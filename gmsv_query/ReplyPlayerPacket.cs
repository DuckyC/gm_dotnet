using GSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmsv_query
{
    public class ReplyPlayerPacket
    {
        public List<ReplyPlayer> Players { get; set; }

        public byte[] GetPacket()
        {
            using (var buffer = new ValveBuffer())
            {
                buffer.WriteLong(-1);
                buffer.WriteByte(Encoding.UTF8.GetBytes("D")[0]);

                buffer.WriteByte((byte)Players.Count);

                for (int i = 0; i < Players.Count; i++)
                {
                    var player = Players[i];
                    buffer.WriteByte((byte)i);
                    buffer.WriteString(player.Name);
                    buffer.WriteLong(player.Score);
                    buffer.WriteFloat(player.Time);
                }

                return buffer.ToArray();
            }
        }

    }

    public class ReplyPlayer
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public float Time { get; set; }
    }
}
