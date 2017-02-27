using GSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GSharp.Native.Classes
{

    [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Ansi)]
    public delegate void pfnStringChanged(IntPtr obj, IntPtr networkStringTable, int stringNumber, string newSstring, IntPtr newUserData);

    public interface INetworkStringTable
    {
        [VTableOffset(1)]
        // Table Info
        string GetTableName();
        int GetTableId();
        int GetNumStrings();
        int GetMaxStrings();
        int GetEntryBits();

        // Networking   
        void SetTick(int tick);
        bool ChangedSinceTick(int tick);

        // Accessors (length -1 means don't change user data if string already exits)
        int AddString(bool bIsServer, string value, int length = -1, IntPtr userdata = default(IntPtr));

        string GetString(int stringNumber);
        void SetStringUserData(int stringNumber, int length, IntPtr userdata);
        IntPtr GetStringUserData(int stringNumber, IntPtr length);
        int FindStringIndex(string str); // returns INVALID_STRING_INDEX if not found

        // Callbacks
        void SetStringChangedCallback(IntPtr obj, pfnStringChanged changeFunc);
    }
}
