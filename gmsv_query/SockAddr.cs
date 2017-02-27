using System.Runtime.InteropServices;

namespace GSharp.Native.Classes
{
    public enum SockAddrFamily
    {
        Inet = 2,
        Inet6 = 23
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SockAddr
    {
        public ushort Family;
        public fixed byte Data[14];
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SockAddrIn
    {
        public ushort Family;
        public ushort Port;
        public uint Addr;
        public fixed byte Zero[8];
    }
}
