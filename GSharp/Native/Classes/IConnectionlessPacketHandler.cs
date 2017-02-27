using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{
    public interface IConnectionlessPacketHandler
    {
        void dtorIConnectionlessPacketHandler();

        bool ProcessConnectionlessPacket(IntPtr packet);	// process a connectionless packet
    }
}
