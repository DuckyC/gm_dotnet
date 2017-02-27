using System;
using System.Runtime.InteropServices;
namespace GSharp.Native.Classes
{
    namespace VCR
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate double Hook_Sys_FloatTime(double time);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int Hook_PeekMessage(IntPtr msg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_RecordGameMsg(IntPtr InputEvent_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_RecordEndGameMsg();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate bool Hook_PlaybackGameMsg(IntPtr pEvent);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int Hook_recvfrom(int s, byte* buf, int len, int flags, IntPtr from, IntPtr fromlen);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_GetCursorPos(IntPtr pt);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_ScreenToClient(IntPtr hWnd, IntPtr pt);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_Cmd_Exec(string[] Args);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate string Hook_GetCommandLine();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate long Hook_RegOpenKeyEx(IntPtr hKey, string lpSubKey, ulong ulOptions, ulong samDesired, IntPtr pHKey);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate long Hook_RegSetValueEx(IntPtr hKey, string lpValueName, ulong Reserved, ulong dwType, IntPtr lpData, ulong cbData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate long Hook_RegQueryValueEx(IntPtr hKey, string lpValueName, ulong* lpReserved, ulong* lpType, byte* lpData, ulong* lpcbData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate long Hook_RegCreateKeyEx(IntPtr hKey, string lpSubKey, ulong Reserved, string lpClass, ulong dwOptions, ulong samDesired, IntPtr lpSecurityAttributes, IntPtr phkResult, ulong* lpdwDisposition);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_RegCloseKey(IntPtr hKey);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int Hook_GetNumberOfConsoleInputEvents(IntPtr hInput, ulong* pNumEvents);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int Hook_ReadConsoleInput(IntPtr hInput, IntPtr pRecs, int nMaxRecs, ulong* pNumRead);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_LocalTime(IntPtr today);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate short Hook_GetKeyState(int nVirtKey);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int Hook_recv(int s, char* buf, int len, int flags);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int Hook_send(int s, string buf, int len, int flags);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr Hook_CreateThread(IntPtr lpThreadAttributes, ulong dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ulong dwCreationFlags, ulong* lpThreadID);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate ulong Hook_WaitForSingleObject(IntPtr handle, ulong dwMilliseconds);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_EnterCriticalSection(IntPtr pCS);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_Time(long* pTime);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate ulong Hook_WaitForMultipleObjects(uint nHandles, IntPtr pHandles, int bWaitAll, uint timeout);
    }


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

		public T OverwriteHook<T>(T NewDelegate) where T : class
		{
			fixed (VCR_t* ThisFixed = &this)
				return NativeInterface.OverwriteVCRHook<T>(ThisFixed, NewDelegate);
		}
    }
}

