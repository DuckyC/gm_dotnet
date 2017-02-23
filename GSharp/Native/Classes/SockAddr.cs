using System;
using System.Net;
using System.Runtime.InteropServices;

namespace GSharp.Native.Classes
{
    public enum SockAddrFamily
    {
        Inet = 2,
        Inet6 = 23
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SockAddr
    {
        public ushort Family;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] Data;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct SockAddrIn
    {
        public ushort Family;
        public ushort Port;
        public uint Addr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Zero;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SockAddrIn6
    {
        public ushort Family;
        public ushort Port;
        public uint FlowInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Addr;
        public uint ScopeId;
    };

    public static class SockAddrHelpers
    {
        public static IPAddress ConvertSockAddrPtrToIPAddress(IntPtr sockAddrPtr)
        {
            SockAddr sockAddr = (SockAddr)Marshal.PtrToStructure(sockAddrPtr, typeof(SockAddr));
            switch ((SockAddrFamily)sockAddr.Family)
            {
                case SockAddrFamily.Inet:
                    {
                        SockAddrIn sockAddrIn = (SockAddrIn)Marshal.PtrToStructure(sockAddrPtr, typeof(SockAddrIn));
                        return new IPAddress(sockAddrIn.Addr);
                    }
                case SockAddrFamily.Inet6:
                    {
                        SockAddrIn6 sockAddrIn6 = (SockAddrIn6)Marshal.PtrToStructure(sockAddrPtr, typeof(SockAddrIn6));
                        return new IPAddress(sockAddrIn6.Addr);
                    }
                default:
                    throw new Exception(string.Format("Non-IP address family: {0}", sockAddr.Family));
            }
        }
    }
}
