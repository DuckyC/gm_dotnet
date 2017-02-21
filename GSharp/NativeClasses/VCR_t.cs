using System;
using System.Runtime.InteropServices;
namespace GSharp.NativeClasses
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int Hook_recvfrom_func(int s, byte* buf, int len, int flags, IntPtr from, IntPtr fromlen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void Hook_Cmd_Exec_func(string[] Args);
	
	[StructLayout(LayoutKind.Sequential)]
    public unsafe struct VCR_t
    {
        public IntPtr Start;
        public IntPtr End;
        public IntPtr GetVCRTraceInterface;
        public IntPtr GetMode;
        public IntPtr SetEnabled;
        public IntPtr SyncToken;
        public IntPtr Hook_Sys_FloatTime;
        public IntPtr Hook_PeekMessage;
        public IntPtr Hook_RecordGameMsg;
        public IntPtr Hook_RecordEndGameMsg;
        public IntPtr Hook_PlaybackGameMsg;
        public IntPtr Hook_recvfrom;
        public IntPtr Hook_GetCursorPos;
        public IntPtr Hook_ScreenToClient;
        public IntPtr Hook_Cmd_Exec;
        public IntPtr Hook_GetCommandLine;
        public IntPtr Hook_RegOpenKeyEx;
        public IntPtr Hook_RegSetValueEx;
        public IntPtr Hook_RegQueryValueEx;
        public IntPtr Hook_RegCreateKeyEx;
        public IntPtr Hook_RegCloseKey;
        public IntPtr Hook_GetNumberOfConsoleInputEvents;
        public IntPtr Hook_ReadConsoleInput;
        public IntPtr Hook_LocalTime;
        public IntPtr Hook_GetKeyState;
        public IntPtr Hook_recv;
        public IntPtr Hook_send;
        public IntPtr GenericRecord;
        public IntPtr GenericPlayback;
        public IntPtr GenericValue;
        public IntPtr GetPercentCompleted;
        public IntPtr Hook_CreateThread;
        public IntPtr Hook_WaitForSingleObject;
        public IntPtr Hook_EnterCriticalSection;
        public IntPtr Hook_Time;
        public IntPtr GenericString;
        public IntPtr GenericValueVerify;
        public IntPtr Hook_WaitForMultipleObjects;
    }
}
