using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GSharp
{
    public class ValveBuffer : IDisposable
    {
        MemoryStream stream;
        BinaryWriter writer;

        public ValveBuffer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public void WriteByte(byte val)
        {
            writer.Write(val);
        }

        public void WriteShort(short val)
        {
            writer.Write(val);
        }

        public void WriteLong(int val)
        {
            writer.Write(val);
        }

        public void WriteFloat(float val)
        {
            writer.Write(val);
        }

        public void WriteLongLong(long val)
        {
            writer.Write(val);
        }

        public void WriteString(string val)
        {
            writer.Write(Encoding.UTF8.GetBytes(val));
            writer.Write((byte)0x00);
        }

        public byte[] ToArray()
        {
            return stream.ToArray();
        }

        public void Dispose()
        {
            writer.Dispose();
            stream.Dispose();
        }
    }
}
