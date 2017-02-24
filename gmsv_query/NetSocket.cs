using GSharp.Native;
using GSharp.Native.Classes;
using System.Runtime.InteropServices;

namespace gmsv_query
{
    public unsafe static class NetSocket
    {
        static byte[] net_sockets_sig = new byte[] { 0x2A, 0x2A, 0x2A, 0x2A, 0x80, 0x7E, 0x04, 0x00, 0x0F, 0x84, 0x2A, 0x2A, 0x2A, 0x2A, 0xA1, 0x2A, 0x2A, 0x2A, 0x2A, 0xC7, 0x45, 0xF8, 0x10 };

        [StructLayout(LayoutKind.Sequential)]
        public struct netsocket_t
        {
            public int nPort;
            public bool bListening;
            public uint hUDP;
            public uint hTCP;
        };

        public static netsocket_t* GetNetSocket()
        {
            CUtlVector<netsocket_t> netsocks = SymbolFinder.ResolveOnBinary("engine.dll", net_sockets_sig);
            return (netsocket_t*)netsocks[1];
        }
    }
}
