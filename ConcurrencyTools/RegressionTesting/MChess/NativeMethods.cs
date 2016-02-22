using System.Runtime.InteropServices;
using System;

public class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern int CloseHandle(
        IntPtr handle
        );

    //
    // CreateThread():
    //
    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateThread(
        IntPtr lpThreadAttributes,
        IntPtr dwStackSize,
        IntPtr lpStartAddress,
        IntPtr lpParameter,
        int dwCreationFlags,
        IntPtr lpThreadId
        );

    [DllImport("kernel32.dll")]
    public unsafe static extern int WaitForMultipleObjects(
        int nCount,
        global::System.IntPtr* lpHandles,
        bool bWaitAll,
        int dwMilliseconds
        );

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetCurrentThread();

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll")]
    public static extern bool DuplicateHandle(
        global::System.IntPtr hSourceProcessHandle,
        global::System.IntPtr hSourceHandle,
        global::System.IntPtr hTargetProcessHandle,
        out global::System.IntPtr lpTargetHandle,
        uint dwDesiredAccess,
        bool bInheritHandle,
        uint dwOptions
        );

    [DllImport("kernel32.dll")]
    public static extern global::System.IntPtr CreateEvent(
        global::System.IntPtr lpEventAttributes,
        bool bManualReset,
        bool bInitialState,
        global::System.String lpName
        );

    [DllImport("kernel32.dll")]
    public static extern bool SetEvent(
        global::System.IntPtr hEvent
        );

}
