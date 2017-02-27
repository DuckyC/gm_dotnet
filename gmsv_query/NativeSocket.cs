using System;
using System.Runtime.InteropServices;

namespace gmsv_query
{
    public class NativeSocket
    {
        [DllImport("ws2_32.dll", EntryPoint = "sendto")]
        public static extern int SendTo(uint Socket, byte[] buff, int len, int flags, IntPtr To, int tomlen);

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WSAGetLastError();
    }
}
