/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define WRAP_CreateIoCompletionPort
#define WRAP_CreateThread
#define WRAP_CreateTimerQueue
#define WRAP_CreateTimerQueueTimer
#define WRAP_DeleteTimerQueue
#define WRAP_DeleteTimerQueueEx
#define WRAP_DeleteTimerQueueTimer
#define WRAP_DuplicateHandle
#define WRAP_EnterCriticalSection
#define WRAP_GetQueuedCompletionStatus
#define WRAP_InterlockedCompareExchange
#define WRAP_InterlockedDecrement
#define WRAP_InterlockedExchange
#define WRAP_InterlockedIncrement
#define WRAP_LeaveCriticalSection
#define WRAP_PostQueuedCompletionStatus
#define WRAP_PulseEvent
#define WRAP_QueueUserAPC
#define WRAP_QueueUserWorkItem
#define WRAP_ReadFile
#define WRAP_ReadFileEx
#define WRAP_ReleaseMutex
#define WRAP_ReleaseSemaphore
#define WRAP_ResetEvent
#define WRAP_ResumeThread
#define WRAP_SetEvent
#define WRAP_SignalObjectAndWait
#define WRAP_Sleep
#define WRAP_SleepEx
#define WRAP_SuspendThread
#define WRAP_SwitchToThread
#define WRAP_TryEnterCriticalSection
#define WRAP_WaitForMultipleObjects
#define WRAP_WaitForMultipleObjectsEx
#define WRAP_WaitForSingleObject
#define WRAP_WaitForSingleObjectEx
#define WRAP_WriteFile
#define WRAP_WriteFileEx

BOOL (WINAPI * Real_ActivateActCtx)( HANDLE hActCtx, ULONG_PTR *lpCookie )
  = ActivateActCtx;

__declspec(dllexport) BOOL WINAPI Mine_ActivateActCtx( HANDLE hActCtx, ULONG_PTR *lpCookie ){
  if(ChessWrapperSentry::Wrap("ActivateActCtx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ActivateActCtx");
   }
  return Real_ActivateActCtx(hActCtx, lpCookie);
}
ATOM (WINAPI * Real_AddAtomA)( LPCSTR lpString )
  = AddAtomA;

__declspec(dllexport) ATOM WINAPI Mine_AddAtomA( LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("AddAtomA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddAtomA");
   }
  return Real_AddAtomA(lpString);
}
ATOM (WINAPI * Real_AddAtomW)( LPCWSTR lpString )
  = AddAtomW;

__declspec(dllexport) ATOM WINAPI Mine_AddAtomW( LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("AddAtomW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddAtomW");
   }
  return Real_AddAtomW(lpString);
}
BOOL (WINAPI * Real_AddConsoleAliasA)( LPSTR Source, LPSTR Target, LPSTR ExeName)
  = AddConsoleAliasA;

__declspec(dllexport) BOOL WINAPI Mine_AddConsoleAliasA( LPSTR Source, LPSTR Target, LPSTR ExeName){
  if(ChessWrapperSentry::Wrap("AddConsoleAliasA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddConsoleAliasA");
   }
  return Real_AddConsoleAliasA(Source, Target, ExeName);
}
BOOL (WINAPI * Real_AddConsoleAliasW)( LPWSTR Source, LPWSTR Target, LPWSTR ExeName)
  = AddConsoleAliasW;

__declspec(dllexport) BOOL WINAPI Mine_AddConsoleAliasW( LPWSTR Source, LPWSTR Target, LPWSTR ExeName){
  if(ChessWrapperSentry::Wrap("AddConsoleAliasW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddConsoleAliasW");
   }
  return Real_AddConsoleAliasW(Source, Target, ExeName);
}
void (WINAPI * Real_AddRefActCtx)( HANDLE hActCtx )
  = AddRefActCtx;

__declspec(dllexport) void WINAPI Mine_AddRefActCtx( HANDLE hActCtx ){
  if(ChessWrapperSentry::Wrap("AddRefActCtx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddRefActCtx");
   }
  return Real_AddRefActCtx(hActCtx);
}
PVOID (WINAPI * Real_AddVectoredContinueHandler)( ULONG First, PVECTORED_EXCEPTION_HANDLER Handler )
  = AddVectoredContinueHandler;

__declspec(dllexport) PVOID WINAPI Mine_AddVectoredContinueHandler( ULONG First, PVECTORED_EXCEPTION_HANDLER Handler ){
  if(ChessWrapperSentry::Wrap("AddVectoredContinueHandler")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddVectoredContinueHandler");
   }
  return Real_AddVectoredContinueHandler(First, Handler);
}
PVOID (WINAPI * Real_AddVectoredExceptionHandler)( ULONG First, PVECTORED_EXCEPTION_HANDLER Handler )
  = AddVectoredExceptionHandler;

__declspec(dllexport) PVOID WINAPI Mine_AddVectoredExceptionHandler( ULONG First, PVECTORED_EXCEPTION_HANDLER Handler ){
  if(ChessWrapperSentry::Wrap("AddVectoredExceptionHandler")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AddVectoredExceptionHandler");
   }
  return Real_AddVectoredExceptionHandler(First, Handler);
}
BOOL (WINAPI * Real_AllocConsole)( void )
  = AllocConsole;

__declspec(dllexport) BOOL WINAPI Mine_AllocConsole( void ){
  if(ChessWrapperSentry::Wrap("AllocConsole")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AllocConsole");
   }
  return Real_AllocConsole();
}
BOOL (WINAPI * Real_AllocateUserPhysicalPages)( HANDLE hProcess, PULONG_PTR NumberOfPages, PULONG_PTR PageArray )
  = AllocateUserPhysicalPages;

__declspec(dllexport) BOOL WINAPI Mine_AllocateUserPhysicalPages( HANDLE hProcess, PULONG_PTR NumberOfPages, PULONG_PTR PageArray ){
  if(ChessWrapperSentry::Wrap("AllocateUserPhysicalPages")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AllocateUserPhysicalPages");
   }
  return Real_AllocateUserPhysicalPages(hProcess, NumberOfPages, PageArray);
}
BOOL (WINAPI * Real_AreFileApisANSI)( void )
  = AreFileApisANSI;

__declspec(dllexport) BOOL WINAPI Mine_AreFileApisANSI( void ){
  if(ChessWrapperSentry::Wrap("AreFileApisANSI")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AreFileApisANSI");
   }
  return Real_AreFileApisANSI();
}
BOOL (WINAPI * Real_AssignProcessToJobObject)( HANDLE hJob, HANDLE hProcess )
  = AssignProcessToJobObject;

__declspec(dllexport) BOOL WINAPI Mine_AssignProcessToJobObject( HANDLE hJob, HANDLE hProcess ){
  if(ChessWrapperSentry::Wrap("AssignProcessToJobObject")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AssignProcessToJobObject");
   }
  return Real_AssignProcessToJobObject(hJob, hProcess);
}
BOOL (WINAPI * Real_AttachConsole)( DWORD dwProcessId )
  = AttachConsole;

__declspec(dllexport) BOOL WINAPI Mine_AttachConsole( DWORD dwProcessId ){
  if(ChessWrapperSentry::Wrap("AttachConsole")){
     ChessWrapperSentry sentry;
     Chess::LogCall("AttachConsole");
   }
  return Real_AttachConsole(dwProcessId);
}
BOOL (WINAPI * Real_BackupRead)( HANDLE hFile, LPBYTE lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, BOOL bAbort, BOOL bProcessSecurity, LPVOID *lpContext )
  = BackupRead;

__declspec(dllexport) BOOL WINAPI Mine_BackupRead( HANDLE hFile, LPBYTE lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, BOOL bAbort, BOOL bProcessSecurity, LPVOID *lpContext ){
  if(ChessWrapperSentry::Wrap("BackupRead")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BackupRead");
   }
  return Real_BackupRead(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, bAbort, bProcessSecurity, lpContext);
}
BOOL (WINAPI * Real_BackupSeek)( HANDLE hFile, DWORD dwLowBytesToSeek, DWORD dwHighBytesToSeek, LPDWORD lpdwLowByteSeeked, LPDWORD lpdwHighByteSeeked, LPVOID *lpContext )
  = BackupSeek;

__declspec(dllexport) BOOL WINAPI Mine_BackupSeek( HANDLE hFile, DWORD dwLowBytesToSeek, DWORD dwHighBytesToSeek, LPDWORD lpdwLowByteSeeked, LPDWORD lpdwHighByteSeeked, LPVOID *lpContext ){
  if(ChessWrapperSentry::Wrap("BackupSeek")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BackupSeek");
   }
  return Real_BackupSeek(hFile, dwLowBytesToSeek, dwHighBytesToSeek, lpdwLowByteSeeked, lpdwHighByteSeeked, lpContext);
}
BOOL (WINAPI * Real_BackupWrite)( HANDLE hFile, LPBYTE lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, BOOL bAbort, BOOL bProcessSecurity, LPVOID *lpContext )
  = BackupWrite;

__declspec(dllexport) BOOL WINAPI Mine_BackupWrite( HANDLE hFile, LPBYTE lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, BOOL bAbort, BOOL bProcessSecurity, LPVOID *lpContext ){
  if(ChessWrapperSentry::Wrap("BackupWrite")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BackupWrite");
   }
  return Real_BackupWrite(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, bAbort, bProcessSecurity, lpContext);
}
BOOL (WINAPI * Real_Beep)( DWORD dwFreq, DWORD dwDuration )
  = Beep;

__declspec(dllexport) BOOL WINAPI Mine_Beep( DWORD dwFreq, DWORD dwDuration ){
  if(ChessWrapperSentry::Wrap("Beep")){
     ChessWrapperSentry sentry;
     Chess::LogCall("Beep");
   }
  return Real_Beep(dwFreq, dwDuration);
}
HANDLE (WINAPI * Real_BeginUpdateResourceA)( LPCSTR pFileName, BOOL bDeleteExistingResources )
  = BeginUpdateResourceA;

__declspec(dllexport) HANDLE WINAPI Mine_BeginUpdateResourceA( LPCSTR pFileName, BOOL bDeleteExistingResources ){
  if(ChessWrapperSentry::Wrap("BeginUpdateResourceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BeginUpdateResourceA");
   }
  return Real_BeginUpdateResourceA(pFileName, bDeleteExistingResources);
}
HANDLE (WINAPI * Real_BeginUpdateResourceW)( LPCWSTR pFileName, BOOL bDeleteExistingResources )
  = BeginUpdateResourceW;

__declspec(dllexport) HANDLE WINAPI Mine_BeginUpdateResourceW( LPCWSTR pFileName, BOOL bDeleteExistingResources ){
  if(ChessWrapperSentry::Wrap("BeginUpdateResourceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BeginUpdateResourceW");
   }
  return Real_BeginUpdateResourceW(pFileName, bDeleteExistingResources);
}
BOOL (WINAPI * Real_BindIoCompletionCallback)( HANDLE FileHandle, LPOVERLAPPED_COMPLETION_ROUTINE Function, ULONG Flags )
  = BindIoCompletionCallback;

__declspec(dllexport) BOOL WINAPI Mine_BindIoCompletionCallback( HANDLE FileHandle, LPOVERLAPPED_COMPLETION_ROUTINE Function, ULONG Flags ){
  if(ChessWrapperSentry::Wrap("BindIoCompletionCallback")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BindIoCompletionCallback");
   }
  return Real_BindIoCompletionCallback(FileHandle, Function, Flags);
}
BOOL (WINAPI * Real_BuildCommDCBA)( LPCSTR lpDef, LPDCB lpDCB )
  = BuildCommDCBA;

__declspec(dllexport) BOOL WINAPI Mine_BuildCommDCBA( LPCSTR lpDef, LPDCB lpDCB ){
  if(ChessWrapperSentry::Wrap("BuildCommDCBA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BuildCommDCBA");
   }
  return Real_BuildCommDCBA(lpDef, lpDCB);
}
BOOL (WINAPI * Real_BuildCommDCBAndTimeoutsA)( LPCSTR lpDef, LPDCB lpDCB, LPCOMMTIMEOUTS lpCommTimeouts )
  = BuildCommDCBAndTimeoutsA;

__declspec(dllexport) BOOL WINAPI Mine_BuildCommDCBAndTimeoutsA( LPCSTR lpDef, LPDCB lpDCB, LPCOMMTIMEOUTS lpCommTimeouts ){
  if(ChessWrapperSentry::Wrap("BuildCommDCBAndTimeoutsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BuildCommDCBAndTimeoutsA");
   }
  return Real_BuildCommDCBAndTimeoutsA(lpDef, lpDCB, lpCommTimeouts);
}
BOOL (WINAPI * Real_BuildCommDCBAndTimeoutsW)( LPCWSTR lpDef, LPDCB lpDCB, LPCOMMTIMEOUTS lpCommTimeouts )
  = BuildCommDCBAndTimeoutsW;

__declspec(dllexport) BOOL WINAPI Mine_BuildCommDCBAndTimeoutsW( LPCWSTR lpDef, LPDCB lpDCB, LPCOMMTIMEOUTS lpCommTimeouts ){
  if(ChessWrapperSentry::Wrap("BuildCommDCBAndTimeoutsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BuildCommDCBAndTimeoutsW");
   }
  return Real_BuildCommDCBAndTimeoutsW(lpDef, lpDCB, lpCommTimeouts);
}
BOOL (WINAPI * Real_BuildCommDCBW)( LPCWSTR lpDef, LPDCB lpDCB )
  = BuildCommDCBW;

__declspec(dllexport) BOOL WINAPI Mine_BuildCommDCBW( LPCWSTR lpDef, LPDCB lpDCB ){
  if(ChessWrapperSentry::Wrap("BuildCommDCBW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("BuildCommDCBW");
   }
  return Real_BuildCommDCBW(lpDef, lpDCB);
}
BOOL (WINAPI * Real_CallNamedPipeA)( LPCSTR lpNamedPipeName, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesRead, DWORD nTimeOut )
  = CallNamedPipeA;

__declspec(dllexport) BOOL WINAPI Mine_CallNamedPipeA( LPCSTR lpNamedPipeName, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesRead, DWORD nTimeOut ){
  if(ChessWrapperSentry::Wrap("CallNamedPipeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CallNamedPipeA");
   }
  return Real_CallNamedPipeA(lpNamedPipeName, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesRead, nTimeOut);
}
BOOL (WINAPI * Real_CallNamedPipeW)( LPCWSTR lpNamedPipeName, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesRead, DWORD nTimeOut )
  = CallNamedPipeW;

__declspec(dllexport) BOOL WINAPI Mine_CallNamedPipeW( LPCWSTR lpNamedPipeName, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesRead, DWORD nTimeOut ){
  if(ChessWrapperSentry::Wrap("CallNamedPipeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CallNamedPipeW");
   }
  return Real_CallNamedPipeW(lpNamedPipeName, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesRead, nTimeOut);
}
BOOL (WINAPI * Real_CancelDeviceWakeupRequest)( HANDLE hDevice )
  = CancelDeviceWakeupRequest;

__declspec(dllexport) BOOL WINAPI Mine_CancelDeviceWakeupRequest( HANDLE hDevice ){
  if(ChessWrapperSentry::Wrap("CancelDeviceWakeupRequest")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CancelDeviceWakeupRequest");
   }
  return Real_CancelDeviceWakeupRequest(hDevice);
}
BOOL (WINAPI * Real_CancelIo)( HANDLE hFile )
  = CancelIo;

__declspec(dllexport) BOOL WINAPI Mine_CancelIo( HANDLE hFile ){
  if(ChessWrapperSentry::Wrap("CancelIo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CancelIo");
   }
  return Real_CancelIo(hFile);
}
BOOL (WINAPI * Real_CancelTimerQueueTimer)( HANDLE TimerQueue, HANDLE Timer )
  = CancelTimerQueueTimer;

__declspec(dllexport) BOOL WINAPI Mine_CancelTimerQueueTimer( HANDLE TimerQueue, HANDLE Timer ){
  if(ChessWrapperSentry::Wrap("CancelTimerQueueTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CancelTimerQueueTimer");
   }
  return Real_CancelTimerQueueTimer(TimerQueue, Timer);
}
BOOL (WINAPI * Real_CancelWaitableTimer)( HANDLE hTimer )
  = CancelWaitableTimer;

__declspec(dllexport) BOOL WINAPI Mine_CancelWaitableTimer( HANDLE hTimer ){
  if(ChessWrapperSentry::Wrap("CancelWaitableTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CancelWaitableTimer");
   }
  return Real_CancelWaitableTimer(hTimer);
}
BOOL (WINAPI * Real_ChangeTimerQueueTimer)( HANDLE TimerQueue, HANDLE Timer, ULONG DueTime, ULONG Period )
  = ChangeTimerQueueTimer;

__declspec(dllexport) BOOL WINAPI Mine_ChangeTimerQueueTimer( HANDLE TimerQueue, HANDLE Timer, ULONG DueTime, ULONG Period ){
  if(ChessWrapperSentry::Wrap("ChangeTimerQueueTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ChangeTimerQueueTimer");
   }
  return Real_ChangeTimerQueueTimer(TimerQueue, Timer, DueTime, Period);
}
BOOL (WINAPI * Real_CheckNameLegalDOS8Dot3A)( LPCSTR lpName, LPSTR lpOemName, DWORD OemNameSize, PBOOL pbNameContainsSpaces, PBOOL pbNameLegal )
  = CheckNameLegalDOS8Dot3A;

__declspec(dllexport) BOOL WINAPI Mine_CheckNameLegalDOS8Dot3A( LPCSTR lpName, LPSTR lpOemName, DWORD OemNameSize, PBOOL pbNameContainsSpaces, PBOOL pbNameLegal ){
  if(ChessWrapperSentry::Wrap("CheckNameLegalDOS8Dot3A")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CheckNameLegalDOS8Dot3A");
   }
  return Real_CheckNameLegalDOS8Dot3A(lpName, lpOemName, OemNameSize, pbNameContainsSpaces, pbNameLegal);
}
BOOL (WINAPI * Real_CheckNameLegalDOS8Dot3W)( LPCWSTR lpName, LPSTR lpOemName, DWORD OemNameSize, PBOOL pbNameContainsSpaces, PBOOL pbNameLegal )
  = CheckNameLegalDOS8Dot3W;

__declspec(dllexport) BOOL WINAPI Mine_CheckNameLegalDOS8Dot3W( LPCWSTR lpName, LPSTR lpOemName, DWORD OemNameSize, PBOOL pbNameContainsSpaces, PBOOL pbNameLegal ){
  if(ChessWrapperSentry::Wrap("CheckNameLegalDOS8Dot3W")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CheckNameLegalDOS8Dot3W");
   }
  return Real_CheckNameLegalDOS8Dot3W(lpName, lpOemName, OemNameSize, pbNameContainsSpaces, pbNameLegal);
}
BOOL (WINAPI * Real_CheckRemoteDebuggerPresent)( HANDLE hProcess, PBOOL pbDebuggerPresent )
  = CheckRemoteDebuggerPresent;

__declspec(dllexport) BOOL WINAPI Mine_CheckRemoteDebuggerPresent( HANDLE hProcess, PBOOL pbDebuggerPresent ){
  if(ChessWrapperSentry::Wrap("CheckRemoteDebuggerPresent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CheckRemoteDebuggerPresent");
   }
  return Real_CheckRemoteDebuggerPresent(hProcess, pbDebuggerPresent);
}
BOOL (WINAPI * Real_ClearCommBreak)( HANDLE hFile )
  = ClearCommBreak;

__declspec(dllexport) BOOL WINAPI Mine_ClearCommBreak( HANDLE hFile ){
  if(ChessWrapperSentry::Wrap("ClearCommBreak")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ClearCommBreak");
   }
  return Real_ClearCommBreak(hFile);
}
BOOL (WINAPI * Real_ClearCommError)( HANDLE hFile, LPDWORD lpErrors, LPCOMSTAT lpStat )
  = ClearCommError;

__declspec(dllexport) BOOL WINAPI Mine_ClearCommError( HANDLE hFile, LPDWORD lpErrors, LPCOMSTAT lpStat ){
  if(ChessWrapperSentry::Wrap("ClearCommError")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ClearCommError");
   }
  return Real_ClearCommError(hFile, lpErrors, lpStat);
}
BOOL (WINAPI * Real_CloseHandle)( HANDLE hObject )
  = CloseHandle;

__declspec(dllexport) BOOL WINAPI Mine_CloseHandle( HANDLE hObject ){
  if(ChessWrapperSentry::Wrap("CloseHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CloseHandle");
   }
  return Real_CloseHandle(hObject);
}
BOOL (WINAPI * Real_CommConfigDialogA)( LPCSTR lpszName, HWND hWnd, LPCOMMCONFIG lpCC )
  = CommConfigDialogA;

__declspec(dllexport) BOOL WINAPI Mine_CommConfigDialogA( LPCSTR lpszName, HWND hWnd, LPCOMMCONFIG lpCC ){
  if(ChessWrapperSentry::Wrap("CommConfigDialogA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CommConfigDialogA");
   }
  return Real_CommConfigDialogA(lpszName, hWnd, lpCC);
}
BOOL (WINAPI * Real_CommConfigDialogW)( LPCWSTR lpszName, HWND hWnd, LPCOMMCONFIG lpCC )
  = CommConfigDialogW;

__declspec(dllexport) BOOL WINAPI Mine_CommConfigDialogW( LPCWSTR lpszName, HWND hWnd, LPCOMMCONFIG lpCC ){
  if(ChessWrapperSentry::Wrap("CommConfigDialogW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CommConfigDialogW");
   }
  return Real_CommConfigDialogW(lpszName, hWnd, lpCC);
}
LONG (WINAPI * Real_CompareFileTime)( const FILETIME *lpFileTime1, const FILETIME *lpFileTime2 )
  = CompareFileTime;

__declspec(dllexport) LONG WINAPI Mine_CompareFileTime( const FILETIME *lpFileTime1, const FILETIME *lpFileTime2 ){
  if(ChessWrapperSentry::Wrap("CompareFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CompareFileTime");
   }
  return Real_CompareFileTime(lpFileTime1, lpFileTime2);
}
int (WINAPI * Real_CompareStringA)( LCID Locale, DWORD dwCmpFlags, LPCSTR lpString1, int cchCount1, LPCSTR lpString2, int cchCount2)
  = CompareStringA;

__declspec(dllexport) int WINAPI Mine_CompareStringA( LCID Locale, DWORD dwCmpFlags, LPCSTR lpString1, int cchCount1, LPCSTR lpString2, int cchCount2){
  if(ChessWrapperSentry::Wrap("CompareStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CompareStringA");
   }
  return Real_CompareStringA(Locale, dwCmpFlags, lpString1, cchCount1, lpString2, cchCount2);
}
int (WINAPI * Real_CompareStringW)( LCID Locale, DWORD dwCmpFlags, LPCWSTR lpString1, int cchCount1, LPCWSTR lpString2, int cchCount2)
  = CompareStringW;

__declspec(dllexport) int WINAPI Mine_CompareStringW( LCID Locale, DWORD dwCmpFlags, LPCWSTR lpString1, int cchCount1, LPCWSTR lpString2, int cchCount2){
  if(ChessWrapperSentry::Wrap("CompareStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CompareStringW");
   }
  return Real_CompareStringW(Locale, dwCmpFlags, lpString1, cchCount1, lpString2, cchCount2);
}
BOOL (WINAPI * Real_ConnectNamedPipe)( HANDLE hNamedPipe, LPOVERLAPPED lpOverlapped )
  = ConnectNamedPipe;

__declspec(dllexport) BOOL WINAPI Mine_ConnectNamedPipe( HANDLE hNamedPipe, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("ConnectNamedPipe")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ConnectNamedPipe");
   }
  return Real_ConnectNamedPipe(hNamedPipe, lpOverlapped);
}
BOOL (WINAPI * Real_ContinueDebugEvent)( DWORD dwProcessId, DWORD dwThreadId, DWORD dwContinueStatus )
  = ContinueDebugEvent;

__declspec(dllexport) BOOL WINAPI Mine_ContinueDebugEvent( DWORD dwProcessId, DWORD dwThreadId, DWORD dwContinueStatus ){
  if(ChessWrapperSentry::Wrap("ContinueDebugEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ContinueDebugEvent");
   }
  return Real_ContinueDebugEvent(dwProcessId, dwThreadId, dwContinueStatus);
}
LCID (WINAPI * Real_ConvertDefaultLocale)( LCID Locale)
  = ConvertDefaultLocale;

__declspec(dllexport) LCID WINAPI Mine_ConvertDefaultLocale( LCID Locale){
  if(ChessWrapperSentry::Wrap("ConvertDefaultLocale")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ConvertDefaultLocale");
   }
  return Real_ConvertDefaultLocale(Locale);
}
BOOL (WINAPI * Real_ConvertFiberToThread)( void )
  = ConvertFiberToThread;

__declspec(dllexport) BOOL WINAPI Mine_ConvertFiberToThread( void ){
  if(ChessWrapperSentry::Wrap("ConvertFiberToThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ConvertFiberToThread");
   }
  return Real_ConvertFiberToThread();
}
LPVOID (WINAPI * Real_ConvertThreadToFiber)( LPVOID lpParameter )
  = ConvertThreadToFiber;

__declspec(dllexport) LPVOID WINAPI Mine_ConvertThreadToFiber( LPVOID lpParameter ){
  if(ChessWrapperSentry::Wrap("ConvertThreadToFiber")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ConvertThreadToFiber");
   }
  return Real_ConvertThreadToFiber(lpParameter);
}
LPVOID (WINAPI * Real_ConvertThreadToFiberEx)( LPVOID lpParameter, DWORD dwFlags )
  = ConvertThreadToFiberEx;

__declspec(dllexport) LPVOID WINAPI Mine_ConvertThreadToFiberEx( LPVOID lpParameter, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("ConvertThreadToFiberEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ConvertThreadToFiberEx");
   }
  return Real_ConvertThreadToFiberEx(lpParameter, dwFlags);
}
BOOL (WINAPI * Real_CopyFileA)( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, BOOL bFailIfExists )
  = CopyFileA;

__declspec(dllexport) BOOL WINAPI Mine_CopyFileA( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, BOOL bFailIfExists ){
  if(ChessWrapperSentry::Wrap("CopyFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CopyFileA");
   }
  return Real_CopyFileA(lpExistingFileName, lpNewFileName, bFailIfExists);
}
BOOL (WINAPI * Real_CopyFileExA)( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, LPBOOL pbCancel, DWORD dwCopyFlags )
  = CopyFileExA;

__declspec(dllexport) BOOL WINAPI Mine_CopyFileExA( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, LPBOOL pbCancel, DWORD dwCopyFlags ){
  if(ChessWrapperSentry::Wrap("CopyFileExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CopyFileExA");
   }
  return Real_CopyFileExA(lpExistingFileName, lpNewFileName, lpProgressRoutine, lpData, pbCancel, dwCopyFlags);
}
BOOL (WINAPI * Real_CopyFileExW)( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, LPBOOL pbCancel, DWORD dwCopyFlags )
  = CopyFileExW;

__declspec(dllexport) BOOL WINAPI Mine_CopyFileExW( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, LPBOOL pbCancel, DWORD dwCopyFlags ){
  if(ChessWrapperSentry::Wrap("CopyFileExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CopyFileExW");
   }
  return Real_CopyFileExW(lpExistingFileName, lpNewFileName, lpProgressRoutine, lpData, pbCancel, dwCopyFlags);
}
BOOL (WINAPI * Real_CopyFileW)( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, BOOL bFailIfExists )
  = CopyFileW;

__declspec(dllexport) BOOL WINAPI Mine_CopyFileW( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, BOOL bFailIfExists ){
  if(ChessWrapperSentry::Wrap("CopyFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CopyFileW");
   }
  return Real_CopyFileW(lpExistingFileName, lpNewFileName, bFailIfExists);
}
HANDLE (WINAPI * Real_CreateActCtxA)( PCACTCTXA pActCtx )
  = CreateActCtxA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateActCtxA( PCACTCTXA pActCtx ){
  if(ChessWrapperSentry::Wrap("CreateActCtxA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateActCtxA");
   }
  return Real_CreateActCtxA(pActCtx);
}
HANDLE (WINAPI * Real_CreateActCtxW)( PCACTCTXW pActCtx )
  = CreateActCtxW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateActCtxW( PCACTCTXW pActCtx ){
  if(ChessWrapperSentry::Wrap("CreateActCtxW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateActCtxW");
   }
  return Real_CreateActCtxW(pActCtx);
}
HANDLE (WINAPI * Real_CreateConsoleScreenBuffer)( DWORD dwDesiredAccess, DWORD dwShareMode, const SECURITY_ATTRIBUTES *lpSecurityAttributes, DWORD dwFlags, LPVOID lpScreenBufferData )
  = CreateConsoleScreenBuffer;

__declspec(dllexport) HANDLE WINAPI Mine_CreateConsoleScreenBuffer( DWORD dwDesiredAccess, DWORD dwShareMode, const SECURITY_ATTRIBUTES *lpSecurityAttributes, DWORD dwFlags, LPVOID lpScreenBufferData ){
  if(ChessWrapperSentry::Wrap("CreateConsoleScreenBuffer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateConsoleScreenBuffer");
   }
  return Real_CreateConsoleScreenBuffer(dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwFlags, lpScreenBufferData);
}
BOOL (WINAPI * Real_CreateDirectoryA)( LPCSTR lpPathName, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateDirectoryA;

__declspec(dllexport) BOOL WINAPI Mine_CreateDirectoryA( LPCSTR lpPathName, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateDirectoryA");
   }
  return Real_CreateDirectoryA(lpPathName, lpSecurityAttributes);
}
BOOL (WINAPI * Real_CreateDirectoryExA)( LPCSTR lpTemplateDirectory, LPCSTR lpNewDirectory, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateDirectoryExA;

__declspec(dllexport) BOOL WINAPI Mine_CreateDirectoryExA( LPCSTR lpTemplateDirectory, LPCSTR lpNewDirectory, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateDirectoryExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateDirectoryExA");
   }
  return Real_CreateDirectoryExA(lpTemplateDirectory, lpNewDirectory, lpSecurityAttributes);
}
BOOL (WINAPI * Real_CreateDirectoryExW)( LPCWSTR lpTemplateDirectory, LPCWSTR lpNewDirectory, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateDirectoryExW;

__declspec(dllexport) BOOL WINAPI Mine_CreateDirectoryExW( LPCWSTR lpTemplateDirectory, LPCWSTR lpNewDirectory, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateDirectoryExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateDirectoryExW");
   }
  return Real_CreateDirectoryExW(lpTemplateDirectory, lpNewDirectory, lpSecurityAttributes);
}
BOOL (WINAPI * Real_CreateDirectoryW)( LPCWSTR lpPathName, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateDirectoryW;

__declspec(dllexport) BOOL WINAPI Mine_CreateDirectoryW( LPCWSTR lpPathName, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateDirectoryW");
   }
  return Real_CreateDirectoryW(lpPathName, lpSecurityAttributes);
}
HANDLE (WINAPI * Real_CreateEventA)( LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCSTR lpName )
  = CreateEventA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateEventA( LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateEventA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateEventA");
   }
  return Real_CreateEventA(lpEventAttributes, bManualReset, bInitialState, lpName);
}
HANDLE (WINAPI * Real_CreateEventW)( LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCWSTR lpName )
  = CreateEventW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateEventW( LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateEventW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateEventW");
   }
  return Real_CreateEventW(lpEventAttributes, bManualReset, bInitialState, lpName);
}
LPVOID (WINAPI * Real_CreateFiber)( SIZE_T dwStackSize, LPFIBER_START_ROUTINE lpStartAddress, LPVOID lpParameter )
  = CreateFiber;

__declspec(dllexport) LPVOID WINAPI Mine_CreateFiber( SIZE_T dwStackSize, LPFIBER_START_ROUTINE lpStartAddress, LPVOID lpParameter ){
  if(ChessWrapperSentry::Wrap("CreateFiber")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateFiber");
   }
  return Real_CreateFiber(dwStackSize, lpStartAddress, lpParameter);
}
LPVOID (WINAPI * Real_CreateFiberEx)( SIZE_T dwStackCommitSize, SIZE_T dwStackReserveSize, DWORD dwFlags, LPFIBER_START_ROUTINE lpStartAddress, LPVOID lpParameter )
  = CreateFiberEx;

__declspec(dllexport) LPVOID WINAPI Mine_CreateFiberEx( SIZE_T dwStackCommitSize, SIZE_T dwStackReserveSize, DWORD dwFlags, LPFIBER_START_ROUTINE lpStartAddress, LPVOID lpParameter ){
  if(ChessWrapperSentry::Wrap("CreateFiberEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateFiberEx");
   }
  return Real_CreateFiberEx(dwStackCommitSize, dwStackReserveSize, dwFlags, lpStartAddress, lpParameter);
}
HANDLE (WINAPI * Real_CreateFileA)( LPCSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile )
  = CreateFileA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateFileA( LPCSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile ){
  if(ChessWrapperSentry::Wrap("CreateFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateFileA");
   }
  return Real_CreateFileA(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
}
HANDLE (WINAPI * Real_CreateFileMappingA)( HANDLE hFile, LPSECURITY_ATTRIBUTES lpFileMappingAttributes, DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCSTR lpName )
  = CreateFileMappingA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateFileMappingA( HANDLE hFile, LPSECURITY_ATTRIBUTES lpFileMappingAttributes, DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateFileMappingA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateFileMappingA");
   }
  return Real_CreateFileMappingA(hFile, lpFileMappingAttributes, flProtect, dwMaximumSizeHigh, dwMaximumSizeLow, lpName);
}
HANDLE (WINAPI * Real_CreateFileMappingW)( HANDLE hFile, LPSECURITY_ATTRIBUTES lpFileMappingAttributes, DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCWSTR lpName )
  = CreateFileMappingW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateFileMappingW( HANDLE hFile, LPSECURITY_ATTRIBUTES lpFileMappingAttributes, DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateFileMappingW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateFileMappingW");
   }
  return Real_CreateFileMappingW(hFile, lpFileMappingAttributes, flProtect, dwMaximumSizeHigh, dwMaximumSizeLow, lpName);
}
HANDLE (WINAPI * Real_CreateFileW)( LPCWSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile )
  = CreateFileW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateFileW( LPCWSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile ){
  if(ChessWrapperSentry::Wrap("CreateFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateFileW");
   }
  return Real_CreateFileW(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
}
BOOL (WINAPI * Real_CreateHardLinkA)( LPCSTR lpFileName, LPCSTR lpExistingFileName, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateHardLinkA;

__declspec(dllexport) BOOL WINAPI Mine_CreateHardLinkA( LPCSTR lpFileName, LPCSTR lpExistingFileName, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateHardLinkA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateHardLinkA");
   }
  return Real_CreateHardLinkA(lpFileName, lpExistingFileName, lpSecurityAttributes);
}
BOOL (WINAPI * Real_CreateHardLinkW)( LPCWSTR lpFileName, LPCWSTR lpExistingFileName, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateHardLinkW;

__declspec(dllexport) BOOL WINAPI Mine_CreateHardLinkW( LPCWSTR lpFileName, LPCWSTR lpExistingFileName, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateHardLinkW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateHardLinkW");
   }
  return Real_CreateHardLinkW(lpFileName, lpExistingFileName, lpSecurityAttributes);
}
HANDLE (WINAPI * Real_CreateIoCompletionPort)( HANDLE FileHandle, HANDLE ExistingCompletionPort, ULONG_PTR CompletionKey, DWORD NumberOfConcurrentThreads )
   = CreateIoCompletionPort;

__declspec(dllexport) HANDLE WINAPI Mine_CreateIoCompletionPort( HANDLE FileHandle, HANDLE ExistingCompletionPort, ULONG_PTR CompletionKey, DWORD NumberOfConcurrentThreads ){
#ifdef WRAP_CreateIoCompletionPort
  if(ChessWrapperSentry::Wrap("CreateIoCompletionPort")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateIoCompletionPort");
     HANDLE res = __wrapper_CreateIoCompletionPort(FileHandle, ExistingCompletionPort, CompletionKey, NumberOfConcurrentThreads);
     return res;
  }
#endif
  return Real_CreateIoCompletionPort(FileHandle, ExistingCompletionPort, CompletionKey, NumberOfConcurrentThreads);
}
HANDLE (WINAPI * Real_CreateJobObjectA)( LPSECURITY_ATTRIBUTES lpJobAttributes, LPCSTR lpName )
  = CreateJobObjectA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateJobObjectA( LPSECURITY_ATTRIBUTES lpJobAttributes, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateJobObjectA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateJobObjectA");
   }
  return Real_CreateJobObjectA(lpJobAttributes, lpName);
}
HANDLE (WINAPI * Real_CreateJobObjectW)( LPSECURITY_ATTRIBUTES lpJobAttributes, LPCWSTR lpName )
  = CreateJobObjectW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateJobObjectW( LPSECURITY_ATTRIBUTES lpJobAttributes, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateJobObjectW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateJobObjectW");
   }
  return Real_CreateJobObjectW(lpJobAttributes, lpName);
}
BOOL (WINAPI * Real_CreateJobSet)( ULONG NumJob, PJOB_SET_ARRAY UserJobSet, ULONG Flags)
  = CreateJobSet;

__declspec(dllexport) BOOL WINAPI Mine_CreateJobSet( ULONG NumJob, PJOB_SET_ARRAY UserJobSet, ULONG Flags){
  if(ChessWrapperSentry::Wrap("CreateJobSet")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateJobSet");
   }
  return Real_CreateJobSet(NumJob, UserJobSet, Flags);
}
HANDLE (WINAPI * Real_CreateMailslotA)( LPCSTR lpName, DWORD nMaxMessageSize, DWORD lReadTimeout, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateMailslotA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateMailslotA( LPCSTR lpName, DWORD nMaxMessageSize, DWORD lReadTimeout, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateMailslotA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateMailslotA");
   }
  return Real_CreateMailslotA(lpName, nMaxMessageSize, lReadTimeout, lpSecurityAttributes);
}
HANDLE (WINAPI * Real_CreateMailslotW)( LPCWSTR lpName, DWORD nMaxMessageSize, DWORD lReadTimeout, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateMailslotW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateMailslotW( LPCWSTR lpName, DWORD nMaxMessageSize, DWORD lReadTimeout, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateMailslotW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateMailslotW");
   }
  return Real_CreateMailslotW(lpName, nMaxMessageSize, lReadTimeout, lpSecurityAttributes);
}
HANDLE (WINAPI * Real_CreateMemoryResourceNotification)( MEMORY_RESOURCE_NOTIFICATION_TYPE NotificationType )
  = CreateMemoryResourceNotification;

__declspec(dllexport) HANDLE WINAPI Mine_CreateMemoryResourceNotification( MEMORY_RESOURCE_NOTIFICATION_TYPE NotificationType ){
  if(ChessWrapperSentry::Wrap("CreateMemoryResourceNotification")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateMemoryResourceNotification");
   }
  return Real_CreateMemoryResourceNotification(NotificationType);
}
HANDLE (WINAPI * Real_CreateMutexA)( LPSECURITY_ATTRIBUTES lpMutexAttributes, BOOL bInitialOwner, LPCSTR lpName )
  = CreateMutexA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateMutexA( LPSECURITY_ATTRIBUTES lpMutexAttributes, BOOL bInitialOwner, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateMutexA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateMutexA");
   }
  return Real_CreateMutexA(lpMutexAttributes, bInitialOwner, lpName);
}
HANDLE (WINAPI * Real_CreateMutexW)( LPSECURITY_ATTRIBUTES lpMutexAttributes, BOOL bInitialOwner, LPCWSTR lpName )
  = CreateMutexW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateMutexW( LPSECURITY_ATTRIBUTES lpMutexAttributes, BOOL bInitialOwner, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateMutexW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateMutexW");
   }
  return Real_CreateMutexW(lpMutexAttributes, bInitialOwner, lpName);
}
HANDLE (WINAPI * Real_CreateNamedPipeA)( LPCSTR lpName, DWORD dwOpenMode, DWORD dwPipeMode, DWORD nMaxInstances, DWORD nOutBufferSize, DWORD nInBufferSize, DWORD nDefaultTimeOut, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateNamedPipeA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateNamedPipeA( LPCSTR lpName, DWORD dwOpenMode, DWORD dwPipeMode, DWORD nMaxInstances, DWORD nOutBufferSize, DWORD nInBufferSize, DWORD nDefaultTimeOut, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateNamedPipeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateNamedPipeA");
   }
  return Real_CreateNamedPipeA(lpName, dwOpenMode, dwPipeMode, nMaxInstances, nOutBufferSize, nInBufferSize, nDefaultTimeOut, lpSecurityAttributes);
}
HANDLE (WINAPI * Real_CreateNamedPipeW)( LPCWSTR lpName, DWORD dwOpenMode, DWORD dwPipeMode, DWORD nMaxInstances, DWORD nOutBufferSize, DWORD nInBufferSize, DWORD nDefaultTimeOut, LPSECURITY_ATTRIBUTES lpSecurityAttributes )
  = CreateNamedPipeW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateNamedPipeW( LPCWSTR lpName, DWORD dwOpenMode, DWORD dwPipeMode, DWORD nMaxInstances, DWORD nOutBufferSize, DWORD nInBufferSize, DWORD nDefaultTimeOut, LPSECURITY_ATTRIBUTES lpSecurityAttributes ){
  if(ChessWrapperSentry::Wrap("CreateNamedPipeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateNamedPipeW");
   }
  return Real_CreateNamedPipeW(lpName, dwOpenMode, dwPipeMode, nMaxInstances, nOutBufferSize, nInBufferSize, nDefaultTimeOut, lpSecurityAttributes);
}
BOOL (WINAPI * Real_CreatePipe)( PHANDLE hReadPipe, PHANDLE hWritePipe, LPSECURITY_ATTRIBUTES lpPipeAttributes, DWORD nSize )
  = CreatePipe;

__declspec(dllexport) BOOL WINAPI Mine_CreatePipe( PHANDLE hReadPipe, PHANDLE hWritePipe, LPSECURITY_ATTRIBUTES lpPipeAttributes, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("CreatePipe")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreatePipe");
   }
  return Real_CreatePipe(hReadPipe, hWritePipe, lpPipeAttributes, nSize);
}
BOOL (WINAPI * Real_CreateProcessA)( LPCSTR lpApplicationName, LPSTR lpCommandLine, LPSECURITY_ATTRIBUTES lpProcessAttributes, LPSECURITY_ATTRIBUTES lpThreadAttributes, BOOL bInheritHandles, DWORD dwCreationFlags, LPVOID lpEnvironment, LPCSTR lpCurrentDirectory, LPSTARTUPINFOA lpStartupInfo, LPPROCESS_INFORMATION lpProcessInformation )
  = CreateProcessA;

__declspec(dllexport) BOOL WINAPI Mine_CreateProcessA( LPCSTR lpApplicationName, LPSTR lpCommandLine, LPSECURITY_ATTRIBUTES lpProcessAttributes, LPSECURITY_ATTRIBUTES lpThreadAttributes, BOOL bInheritHandles, DWORD dwCreationFlags, LPVOID lpEnvironment, LPCSTR lpCurrentDirectory, LPSTARTUPINFOA lpStartupInfo, LPPROCESS_INFORMATION lpProcessInformation ){
  if(ChessWrapperSentry::Wrap("CreateProcessA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateProcessA");
   }
  return Real_CreateProcessA(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);
}
BOOL (WINAPI * Real_CreateProcessW)( LPCWSTR lpApplicationName, LPWSTR lpCommandLine, LPSECURITY_ATTRIBUTES lpProcessAttributes, LPSECURITY_ATTRIBUTES lpThreadAttributes, BOOL bInheritHandles, DWORD dwCreationFlags, LPVOID lpEnvironment, LPCWSTR lpCurrentDirectory, LPSTARTUPINFOW lpStartupInfo, LPPROCESS_INFORMATION lpProcessInformation )
  = CreateProcessW;

__declspec(dllexport) BOOL WINAPI Mine_CreateProcessW( LPCWSTR lpApplicationName, LPWSTR lpCommandLine, LPSECURITY_ATTRIBUTES lpProcessAttributes, LPSECURITY_ATTRIBUTES lpThreadAttributes, BOOL bInheritHandles, DWORD dwCreationFlags, LPVOID lpEnvironment, LPCWSTR lpCurrentDirectory, LPSTARTUPINFOW lpStartupInfo, LPPROCESS_INFORMATION lpProcessInformation ){
  if(ChessWrapperSentry::Wrap("CreateProcessW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateProcessW");
   }
  return Real_CreateProcessW(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);
}
HANDLE (WINAPI * Real_CreateRemoteThread)( HANDLE hProcess, LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, LPVOID lpParameter, DWORD dwCreationFlags, LPDWORD lpThreadId )
  = CreateRemoteThread;

__declspec(dllexport) HANDLE WINAPI Mine_CreateRemoteThread( HANDLE hProcess, LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, LPVOID lpParameter, DWORD dwCreationFlags, LPDWORD lpThreadId ){
  if(ChessWrapperSentry::Wrap("CreateRemoteThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateRemoteThread");
   }
  return Real_CreateRemoteThread(hProcess, lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);
}
HANDLE (WINAPI * Real_CreateSemaphoreA)( LPSECURITY_ATTRIBUTES lpSemaphoreAttributes, LONG lInitialCount, LONG lMaximumCount, LPCSTR lpName )
  = CreateSemaphoreA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateSemaphoreA( LPSECURITY_ATTRIBUTES lpSemaphoreAttributes, LONG lInitialCount, LONG lMaximumCount, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateSemaphoreA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateSemaphoreA");
   }
  return Real_CreateSemaphoreA(lpSemaphoreAttributes, lInitialCount, lMaximumCount, lpName);
}
HANDLE (WINAPI * Real_CreateSemaphoreW)( LPSECURITY_ATTRIBUTES lpSemaphoreAttributes, LONG lInitialCount, LONG lMaximumCount, LPCWSTR lpName )
  = CreateSemaphoreW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateSemaphoreW( LPSECURITY_ATTRIBUTES lpSemaphoreAttributes, LONG lInitialCount, LONG lMaximumCount, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("CreateSemaphoreW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateSemaphoreW");
   }
  return Real_CreateSemaphoreW(lpSemaphoreAttributes, lInitialCount, lMaximumCount, lpName);
}
DWORD (WINAPI * Real_CreateTapePartition)( HANDLE hDevice, DWORD dwPartitionMethod, DWORD dwCount, DWORD dwSize )
  = CreateTapePartition;

__declspec(dllexport) DWORD WINAPI Mine_CreateTapePartition( HANDLE hDevice, DWORD dwPartitionMethod, DWORD dwCount, DWORD dwSize ){
  if(ChessWrapperSentry::Wrap("CreateTapePartition")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateTapePartition");
   }
  return Real_CreateTapePartition(hDevice, dwPartitionMethod, dwCount, dwSize);
}
HANDLE (WINAPI * Real_CreateThread)( LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, LPVOID lpParameter, DWORD dwCreationFlags, LPDWORD lpThreadId )
   = CreateThread;

__declspec(dllexport) HANDLE WINAPI Mine_CreateThread( LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, LPVOID lpParameter, DWORD dwCreationFlags, LPDWORD lpThreadId ){
#ifdef WRAP_CreateThread
  if(ChessWrapperSentry::Wrap("CreateThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateThread");
     HANDLE res = __wrapper_CreateThread(lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);
     return res;
  }
#endif
  return Real_CreateThread(lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);
}
HANDLE (WINAPI * Real_CreateTimerQueue)( void )
   = CreateTimerQueue;

__declspec(dllexport) HANDLE WINAPI Mine_CreateTimerQueue( void ){
#ifdef WRAP_CreateTimerQueue
  if(ChessWrapperSentry::Wrap("CreateTimerQueue")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateTimerQueue");
     HANDLE res = __wrapper_CreateTimerQueue();
     return res;
  }
#endif
  return Real_CreateTimerQueue();
}
BOOL (WINAPI * Real_CreateTimerQueueTimer)( PHANDLE phNewTimer, HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, ULONG Flags )
   = CreateTimerQueueTimer;

__declspec(dllexport) BOOL WINAPI Mine_CreateTimerQueueTimer( PHANDLE phNewTimer, HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, ULONG Flags ){
#ifdef WRAP_CreateTimerQueueTimer
  if(ChessWrapperSentry::Wrap("CreateTimerQueueTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateTimerQueueTimer");
     BOOL res = __wrapper_CreateTimerQueueTimer(phNewTimer, TimerQueue, Callback, Parameter, DueTime, Period, Flags);
     return res;
  }
#endif
  return Real_CreateTimerQueueTimer(phNewTimer, TimerQueue, Callback, Parameter, DueTime, Period, Flags);
}
HANDLE (WINAPI * Real_CreateWaitableTimerA)( LPSECURITY_ATTRIBUTES lpTimerAttributes, BOOL bManualReset, LPCSTR lpTimerName )
  = CreateWaitableTimerA;

__declspec(dllexport) HANDLE WINAPI Mine_CreateWaitableTimerA( LPSECURITY_ATTRIBUTES lpTimerAttributes, BOOL bManualReset, LPCSTR lpTimerName ){
  if(ChessWrapperSentry::Wrap("CreateWaitableTimerA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateWaitableTimerA");
   }
  return Real_CreateWaitableTimerA(lpTimerAttributes, bManualReset, lpTimerName);
}
HANDLE (WINAPI * Real_CreateWaitableTimerW)( LPSECURITY_ATTRIBUTES lpTimerAttributes, BOOL bManualReset, LPCWSTR lpTimerName )
  = CreateWaitableTimerW;

__declspec(dllexport) HANDLE WINAPI Mine_CreateWaitableTimerW( LPSECURITY_ATTRIBUTES lpTimerAttributes, BOOL bManualReset, LPCWSTR lpTimerName ){
  if(ChessWrapperSentry::Wrap("CreateWaitableTimerW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("CreateWaitableTimerW");
   }
  return Real_CreateWaitableTimerW(lpTimerAttributes, bManualReset, lpTimerName);
}
BOOL (WINAPI * Real_DeactivateActCtx)( DWORD dwFlags, ULONG_PTR ulCookie )
  = DeactivateActCtx;

__declspec(dllexport) BOOL WINAPI Mine_DeactivateActCtx( DWORD dwFlags, ULONG_PTR ulCookie ){
  if(ChessWrapperSentry::Wrap("DeactivateActCtx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeactivateActCtx");
   }
  return Real_DeactivateActCtx(dwFlags, ulCookie);
}
BOOL (WINAPI * Real_DebugActiveProcess)( DWORD dwProcessId )
  = DebugActiveProcess;

__declspec(dllexport) BOOL WINAPI Mine_DebugActiveProcess( DWORD dwProcessId ){
  if(ChessWrapperSentry::Wrap("DebugActiveProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DebugActiveProcess");
   }
  return Real_DebugActiveProcess(dwProcessId);
}
BOOL (WINAPI * Real_DebugActiveProcessStop)( DWORD dwProcessId )
  = DebugActiveProcessStop;

__declspec(dllexport) BOOL WINAPI Mine_DebugActiveProcessStop( DWORD dwProcessId ){
  if(ChessWrapperSentry::Wrap("DebugActiveProcessStop")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DebugActiveProcessStop");
   }
  return Real_DebugActiveProcessStop(dwProcessId);
}
void (WINAPI * Real_DebugBreak)( void )
  = DebugBreak;

__declspec(dllexport) void WINAPI Mine_DebugBreak( void ){
  if(ChessWrapperSentry::Wrap("DebugBreak")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DebugBreak");
   }
  return Real_DebugBreak();
}
BOOL (WINAPI * Real_DebugBreakProcess)( HANDLE Process )
  = DebugBreakProcess;

__declspec(dllexport) BOOL WINAPI Mine_DebugBreakProcess( HANDLE Process ){
  if(ChessWrapperSentry::Wrap("DebugBreakProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DebugBreakProcess");
   }
  return Real_DebugBreakProcess(Process);
}
BOOL (WINAPI * Real_DebugSetProcessKillOnExit)( BOOL KillOnExit )
  = DebugSetProcessKillOnExit;

__declspec(dllexport) BOOL WINAPI Mine_DebugSetProcessKillOnExit( BOOL KillOnExit ){
  if(ChessWrapperSentry::Wrap("DebugSetProcessKillOnExit")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DebugSetProcessKillOnExit");
   }
  return Real_DebugSetProcessKillOnExit(KillOnExit);
}
PVOID (WINAPI * Real_DecodePointer)( PVOID Ptr )
  = DecodePointer;

__declspec(dllexport) PVOID WINAPI Mine_DecodePointer( PVOID Ptr ){
  if(ChessWrapperSentry::Wrap("DecodePointer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DecodePointer");
   }
  return Real_DecodePointer(Ptr);
}
PVOID (WINAPI * Real_DecodeSystemPointer)( PVOID Ptr )
  = DecodeSystemPointer;

__declspec(dllexport) PVOID WINAPI Mine_DecodeSystemPointer( PVOID Ptr ){
  if(ChessWrapperSentry::Wrap("DecodeSystemPointer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DecodeSystemPointer");
   }
  return Real_DecodeSystemPointer(Ptr);
}
BOOL (WINAPI * Real_DefineDosDeviceA)( DWORD dwFlags, LPCSTR lpDeviceName, LPCSTR lpTargetPath )
  = DefineDosDeviceA;

__declspec(dllexport) BOOL WINAPI Mine_DefineDosDeviceA( DWORD dwFlags, LPCSTR lpDeviceName, LPCSTR lpTargetPath ){
  if(ChessWrapperSentry::Wrap("DefineDosDeviceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DefineDosDeviceA");
   }
  return Real_DefineDosDeviceA(dwFlags, lpDeviceName, lpTargetPath);
}
BOOL (WINAPI * Real_DefineDosDeviceW)( DWORD dwFlags, LPCWSTR lpDeviceName, LPCWSTR lpTargetPath )
  = DefineDosDeviceW;

__declspec(dllexport) BOOL WINAPI Mine_DefineDosDeviceW( DWORD dwFlags, LPCWSTR lpDeviceName, LPCWSTR lpTargetPath ){
  if(ChessWrapperSentry::Wrap("DefineDosDeviceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DefineDosDeviceW");
   }
  return Real_DefineDosDeviceW(dwFlags, lpDeviceName, lpTargetPath);
}
ATOM (WINAPI * Real_DeleteAtom)( ATOM nAtom )
  = DeleteAtom;

__declspec(dllexport) ATOM WINAPI Mine_DeleteAtom( ATOM nAtom ){
  if(ChessWrapperSentry::Wrap("DeleteAtom")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteAtom");
   }
  return Real_DeleteAtom(nAtom);
}
void (WINAPI * Real_DeleteCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
  = DeleteCriticalSection;

__declspec(dllexport) void WINAPI Mine_DeleteCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
  if(ChessWrapperSentry::Wrap("DeleteCriticalSection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteCriticalSection");
   }
  return Real_DeleteCriticalSection(lpCriticalSection);
}
void (WINAPI * Real_DeleteFiber)( LPVOID lpFiber )
  = DeleteFiber;

__declspec(dllexport) void WINAPI Mine_DeleteFiber( LPVOID lpFiber ){
  if(ChessWrapperSentry::Wrap("DeleteFiber")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteFiber");
   }
  return Real_DeleteFiber(lpFiber);
}
BOOL (WINAPI * Real_DeleteFileA)( LPCSTR lpFileName )
  = DeleteFileA;

__declspec(dllexport) BOOL WINAPI Mine_DeleteFileA( LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("DeleteFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteFileA");
   }
  return Real_DeleteFileA(lpFileName);
}
BOOL (WINAPI * Real_DeleteFileW)( LPCWSTR lpFileName )
  = DeleteFileW;

__declspec(dllexport) BOOL WINAPI Mine_DeleteFileW( LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("DeleteFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteFileW");
   }
  return Real_DeleteFileW(lpFileName);
}
BOOL (WINAPI * Real_DeleteTimerQueue)( HANDLE TimerQueue )
   = DeleteTimerQueue;

__declspec(dllexport) BOOL WINAPI Mine_DeleteTimerQueue( HANDLE TimerQueue ){
#ifdef WRAP_DeleteTimerQueue
  if(ChessWrapperSentry::Wrap("DeleteTimerQueue")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteTimerQueue");
     BOOL res = __wrapper_DeleteTimerQueue(TimerQueue);
     return res;
  }
#endif
  return Real_DeleteTimerQueue(TimerQueue);
}
BOOL (WINAPI * Real_DeleteTimerQueueEx)( HANDLE TimerQueue, HANDLE CompletionEvent )
   = DeleteTimerQueueEx;

__declspec(dllexport) BOOL WINAPI Mine_DeleteTimerQueueEx( HANDLE TimerQueue, HANDLE CompletionEvent ){
#ifdef WRAP_DeleteTimerQueueEx
  if(ChessWrapperSentry::Wrap("DeleteTimerQueueEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteTimerQueueEx");
     BOOL res = __wrapper_DeleteTimerQueueEx(TimerQueue, CompletionEvent);
     return res;
  }
#endif
  return Real_DeleteTimerQueueEx(TimerQueue, CompletionEvent);
}
BOOL (WINAPI * Real_DeleteTimerQueueTimer)( HANDLE TimerQueue, HANDLE Timer, HANDLE CompletionEvent )
   = DeleteTimerQueueTimer;

__declspec(dllexport) BOOL WINAPI Mine_DeleteTimerQueueTimer( HANDLE TimerQueue, HANDLE Timer, HANDLE CompletionEvent ){
#ifdef WRAP_DeleteTimerQueueTimer
  if(ChessWrapperSentry::Wrap("DeleteTimerQueueTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteTimerQueueTimer");
     BOOL res = __wrapper_DeleteTimerQueueTimer(TimerQueue, Timer, CompletionEvent);
     return res;
  }
#endif
  return Real_DeleteTimerQueueTimer(TimerQueue, Timer, CompletionEvent);
}
BOOL (WINAPI * Real_DeleteVolumeMountPointA)( LPCSTR lpszVolumeMountPoint )
  = DeleteVolumeMountPointA;

__declspec(dllexport) BOOL WINAPI Mine_DeleteVolumeMountPointA( LPCSTR lpszVolumeMountPoint ){
  if(ChessWrapperSentry::Wrap("DeleteVolumeMountPointA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteVolumeMountPointA");
   }
  return Real_DeleteVolumeMountPointA(lpszVolumeMountPoint);
}
BOOL (WINAPI * Real_DeleteVolumeMountPointW)( LPCWSTR lpszVolumeMountPoint )
  = DeleteVolumeMountPointW;

__declspec(dllexport) BOOL WINAPI Mine_DeleteVolumeMountPointW( LPCWSTR lpszVolumeMountPoint ){
  if(ChessWrapperSentry::Wrap("DeleteVolumeMountPointW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeleteVolumeMountPointW");
   }
  return Real_DeleteVolumeMountPointW(lpszVolumeMountPoint);
}
BOOL (WINAPI * Real_DeviceIoControl)( HANDLE hDevice, DWORD dwIoControlCode, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped )
  = DeviceIoControl;

__declspec(dllexport) BOOL WINAPI Mine_DeviceIoControl( HANDLE hDevice, DWORD dwIoControlCode, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("DeviceIoControl")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DeviceIoControl");
   }
  return Real_DeviceIoControl(hDevice, dwIoControlCode, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesReturned, lpOverlapped);
}
BOOL (WINAPI * Real_DisableThreadLibraryCalls)( HMODULE hLibModule )
  = DisableThreadLibraryCalls;

__declspec(dllexport) BOOL WINAPI Mine_DisableThreadLibraryCalls( HMODULE hLibModule ){
  if(ChessWrapperSentry::Wrap("DisableThreadLibraryCalls")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DisableThreadLibraryCalls");
   }
  return Real_DisableThreadLibraryCalls(hLibModule);
}
BOOL (WINAPI * Real_DisconnectNamedPipe)( HANDLE hNamedPipe )
  = DisconnectNamedPipe;

__declspec(dllexport) BOOL WINAPI Mine_DisconnectNamedPipe( HANDLE hNamedPipe ){
  if(ChessWrapperSentry::Wrap("DisconnectNamedPipe")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DisconnectNamedPipe");
   }
  return Real_DisconnectNamedPipe(hNamedPipe);
}
BOOL (WINAPI * Real_DnsHostnameToComputerNameA)( LPCSTR Hostname, LPSTR ComputerName, LPDWORD nSize )
  = DnsHostnameToComputerNameA;

__declspec(dllexport) BOOL WINAPI Mine_DnsHostnameToComputerNameA( LPCSTR Hostname, LPSTR ComputerName, LPDWORD nSize ){
  if(ChessWrapperSentry::Wrap("DnsHostnameToComputerNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DnsHostnameToComputerNameA");
   }
  return Real_DnsHostnameToComputerNameA(Hostname, ComputerName, nSize);
}
BOOL (WINAPI * Real_DnsHostnameToComputerNameW)( LPCWSTR Hostname, LPWSTR ComputerName, LPDWORD nSize )
  = DnsHostnameToComputerNameW;

__declspec(dllexport) BOOL WINAPI Mine_DnsHostnameToComputerNameW( LPCWSTR Hostname, LPWSTR ComputerName, LPDWORD nSize ){
  if(ChessWrapperSentry::Wrap("DnsHostnameToComputerNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DnsHostnameToComputerNameW");
   }
  return Real_DnsHostnameToComputerNameW(Hostname, ComputerName, nSize);
}
BOOL (WINAPI * Real_DosDateTimeToFileTime)( WORD wFatDate, WORD wFatTime, LPFILETIME lpFileTime )
  = DosDateTimeToFileTime;

__declspec(dllexport) BOOL WINAPI Mine_DosDateTimeToFileTime( WORD wFatDate, WORD wFatTime, LPFILETIME lpFileTime ){
  if(ChessWrapperSentry::Wrap("DosDateTimeToFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DosDateTimeToFileTime");
   }
  return Real_DosDateTimeToFileTime(wFatDate, wFatTime, lpFileTime);
}
BOOL (WINAPI * Real_DuplicateHandle)( HANDLE hSourceProcessHandle, HANDLE hSourceHandle, HANDLE hTargetProcessHandle, LPHANDLE lpTargetHandle, DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwOptions )
   = DuplicateHandle;

__declspec(dllexport) BOOL WINAPI Mine_DuplicateHandle( HANDLE hSourceProcessHandle, HANDLE hSourceHandle, HANDLE hTargetProcessHandle, LPHANDLE lpTargetHandle, DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwOptions ){
#ifdef WRAP_DuplicateHandle
  if(ChessWrapperSentry::Wrap("DuplicateHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("DuplicateHandle");
     BOOL res = __wrapper_DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, lpTargetHandle, dwDesiredAccess, bInheritHandle, dwOptions);
     return res;
  }
#endif
  return Real_DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, lpTargetHandle, dwDesiredAccess, bInheritHandle, dwOptions);
}
PVOID (WINAPI * Real_EncodePointer)( PVOID Ptr )
  = EncodePointer;

__declspec(dllexport) PVOID WINAPI Mine_EncodePointer( PVOID Ptr ){
  if(ChessWrapperSentry::Wrap("EncodePointer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EncodePointer");
   }
  return Real_EncodePointer(Ptr);
}
PVOID (WINAPI * Real_EncodeSystemPointer)( PVOID Ptr )
  = EncodeSystemPointer;

__declspec(dllexport) PVOID WINAPI Mine_EncodeSystemPointer( PVOID Ptr ){
  if(ChessWrapperSentry::Wrap("EncodeSystemPointer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EncodeSystemPointer");
   }
  return Real_EncodeSystemPointer(Ptr);
}
BOOL (WINAPI * Real_EndUpdateResourceA)( HANDLE hUpdate, BOOL fDiscard )
  = EndUpdateResourceA;

__declspec(dllexport) BOOL WINAPI Mine_EndUpdateResourceA( HANDLE hUpdate, BOOL fDiscard ){
  if(ChessWrapperSentry::Wrap("EndUpdateResourceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EndUpdateResourceA");
   }
  return Real_EndUpdateResourceA(hUpdate, fDiscard);
}
BOOL (WINAPI * Real_EndUpdateResourceW)( HANDLE hUpdate, BOOL fDiscard )
  = EndUpdateResourceW;

__declspec(dllexport) BOOL WINAPI Mine_EndUpdateResourceW( HANDLE hUpdate, BOOL fDiscard ){
  if(ChessWrapperSentry::Wrap("EndUpdateResourceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EndUpdateResourceW");
   }
  return Real_EndUpdateResourceW(hUpdate, fDiscard);
}
void (WINAPI * Real_EnterCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
   = EnterCriticalSection;

__declspec(dllexport) void WINAPI Mine_EnterCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
#ifdef WRAP_EnterCriticalSection
  if(ChessWrapperSentry::Wrap("EnterCriticalSection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnterCriticalSection");
     __wrapper_EnterCriticalSection(lpCriticalSection);
     return;
  }
#endif
  return Real_EnterCriticalSection(lpCriticalSection);
}
BOOL (WINAPI * Real_EnumCalendarInfoA)( CALINFO_ENUMPROCA lpCalInfoEnumProc, LCID Locale, CALID Calendar, CALTYPE CalType)
  = EnumCalendarInfoA;

__declspec(dllexport) BOOL WINAPI Mine_EnumCalendarInfoA( CALINFO_ENUMPROCA lpCalInfoEnumProc, LCID Locale, CALID Calendar, CALTYPE CalType){
  if(ChessWrapperSentry::Wrap("EnumCalendarInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumCalendarInfoA");
   }
  return Real_EnumCalendarInfoA(lpCalInfoEnumProc, Locale, Calendar, CalType);
}
BOOL (WINAPI * Real_EnumCalendarInfoExA)( CALINFO_ENUMPROCEXA lpCalInfoEnumProcEx, LCID Locale, CALID Calendar, CALTYPE CalType)
  = EnumCalendarInfoExA;

__declspec(dllexport) BOOL WINAPI Mine_EnumCalendarInfoExA( CALINFO_ENUMPROCEXA lpCalInfoEnumProcEx, LCID Locale, CALID Calendar, CALTYPE CalType){
  if(ChessWrapperSentry::Wrap("EnumCalendarInfoExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumCalendarInfoExA");
   }
  return Real_EnumCalendarInfoExA(lpCalInfoEnumProcEx, Locale, Calendar, CalType);
}
BOOL (WINAPI * Real_EnumCalendarInfoExW)( CALINFO_ENUMPROCEXW lpCalInfoEnumProcEx, LCID Locale, CALID Calendar, CALTYPE CalType)
  = EnumCalendarInfoExW;

__declspec(dllexport) BOOL WINAPI Mine_EnumCalendarInfoExW( CALINFO_ENUMPROCEXW lpCalInfoEnumProcEx, LCID Locale, CALID Calendar, CALTYPE CalType){
  if(ChessWrapperSentry::Wrap("EnumCalendarInfoExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumCalendarInfoExW");
   }
  return Real_EnumCalendarInfoExW(lpCalInfoEnumProcEx, Locale, Calendar, CalType);
}
BOOL (WINAPI * Real_EnumCalendarInfoW)( CALINFO_ENUMPROCW lpCalInfoEnumProc, LCID Locale, CALID Calendar, CALTYPE CalType)
  = EnumCalendarInfoW;

__declspec(dllexport) BOOL WINAPI Mine_EnumCalendarInfoW( CALINFO_ENUMPROCW lpCalInfoEnumProc, LCID Locale, CALID Calendar, CALTYPE CalType){
  if(ChessWrapperSentry::Wrap("EnumCalendarInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumCalendarInfoW");
   }
  return Real_EnumCalendarInfoW(lpCalInfoEnumProc, Locale, Calendar, CalType);
}
BOOL (WINAPI * Real_EnumDateFormatsA)( DATEFMT_ENUMPROCA lpDateFmtEnumProc, LCID Locale, DWORD dwFlags)
  = EnumDateFormatsA;

__declspec(dllexport) BOOL WINAPI Mine_EnumDateFormatsA( DATEFMT_ENUMPROCA lpDateFmtEnumProc, LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumDateFormatsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumDateFormatsA");
   }
  return Real_EnumDateFormatsA(lpDateFmtEnumProc, Locale, dwFlags);
}
BOOL (WINAPI * Real_EnumDateFormatsExA)( DATEFMT_ENUMPROCEXA lpDateFmtEnumProcEx, LCID Locale, DWORD dwFlags)
  = EnumDateFormatsExA;

__declspec(dllexport) BOOL WINAPI Mine_EnumDateFormatsExA( DATEFMT_ENUMPROCEXA lpDateFmtEnumProcEx, LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumDateFormatsExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumDateFormatsExA");
   }
  return Real_EnumDateFormatsExA(lpDateFmtEnumProcEx, Locale, dwFlags);
}
BOOL (WINAPI * Real_EnumDateFormatsExW)( DATEFMT_ENUMPROCEXW lpDateFmtEnumProcEx, LCID Locale, DWORD dwFlags)
  = EnumDateFormatsExW;

__declspec(dllexport) BOOL WINAPI Mine_EnumDateFormatsExW( DATEFMT_ENUMPROCEXW lpDateFmtEnumProcEx, LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumDateFormatsExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumDateFormatsExW");
   }
  return Real_EnumDateFormatsExW(lpDateFmtEnumProcEx, Locale, dwFlags);
}
BOOL (WINAPI * Real_EnumDateFormatsW)( DATEFMT_ENUMPROCW lpDateFmtEnumProc, LCID Locale, DWORD dwFlags)
  = EnumDateFormatsW;

__declspec(dllexport) BOOL WINAPI Mine_EnumDateFormatsW( DATEFMT_ENUMPROCW lpDateFmtEnumProc, LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumDateFormatsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumDateFormatsW");
   }
  return Real_EnumDateFormatsW(lpDateFmtEnumProc, Locale, dwFlags);
}
BOOL (WINAPI * Real_EnumLanguageGroupLocalesA)( LANGGROUPLOCALE_ENUMPROCA lpLangGroupLocaleEnumProc, LGRPID LanguageGroup, DWORD dwFlags, LONG_PTR lParam)
  = EnumLanguageGroupLocalesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumLanguageGroupLocalesA( LANGGROUPLOCALE_ENUMPROCA lpLangGroupLocaleEnumProc, LGRPID LanguageGroup, DWORD dwFlags, LONG_PTR lParam){
  if(ChessWrapperSentry::Wrap("EnumLanguageGroupLocalesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumLanguageGroupLocalesA");
   }
  return Real_EnumLanguageGroupLocalesA(lpLangGroupLocaleEnumProc, LanguageGroup, dwFlags, lParam);
}
BOOL (WINAPI * Real_EnumLanguageGroupLocalesW)( LANGGROUPLOCALE_ENUMPROCW lpLangGroupLocaleEnumProc, LGRPID LanguageGroup, DWORD dwFlags, LONG_PTR lParam)
  = EnumLanguageGroupLocalesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumLanguageGroupLocalesW( LANGGROUPLOCALE_ENUMPROCW lpLangGroupLocaleEnumProc, LGRPID LanguageGroup, DWORD dwFlags, LONG_PTR lParam){
  if(ChessWrapperSentry::Wrap("EnumLanguageGroupLocalesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumLanguageGroupLocalesW");
   }
  return Real_EnumLanguageGroupLocalesW(lpLangGroupLocaleEnumProc, LanguageGroup, dwFlags, lParam);
}
BOOL (WINAPI * Real_EnumResourceLanguagesA)( HMODULE hModule, LPCSTR lpType, LPCSTR lpName, ENUMRESLANGPROCA lpEnumFunc, LONG_PTR lParam )
  = EnumResourceLanguagesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumResourceLanguagesA( HMODULE hModule, LPCSTR lpType, LPCSTR lpName, ENUMRESLANGPROCA lpEnumFunc, LONG_PTR lParam ){
  if(ChessWrapperSentry::Wrap("EnumResourceLanguagesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumResourceLanguagesA");
   }
  return Real_EnumResourceLanguagesA(hModule, lpType, lpName, lpEnumFunc, lParam);
}
BOOL (WINAPI * Real_EnumResourceLanguagesW)( HMODULE hModule, LPCWSTR lpType, LPCWSTR lpName, ENUMRESLANGPROCW lpEnumFunc, LONG_PTR lParam )
  = EnumResourceLanguagesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumResourceLanguagesW( HMODULE hModule, LPCWSTR lpType, LPCWSTR lpName, ENUMRESLANGPROCW lpEnumFunc, LONG_PTR lParam ){
  if(ChessWrapperSentry::Wrap("EnumResourceLanguagesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumResourceLanguagesW");
   }
  return Real_EnumResourceLanguagesW(hModule, lpType, lpName, lpEnumFunc, lParam);
}
BOOL (WINAPI * Real_EnumResourceNamesA)( HMODULE hModule, LPCSTR lpType, ENUMRESNAMEPROCA lpEnumFunc, LONG_PTR lParam )
  = EnumResourceNamesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumResourceNamesA( HMODULE hModule, LPCSTR lpType, ENUMRESNAMEPROCA lpEnumFunc, LONG_PTR lParam ){
  if(ChessWrapperSentry::Wrap("EnumResourceNamesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumResourceNamesA");
   }
  return Real_EnumResourceNamesA(hModule, lpType, lpEnumFunc, lParam);
}
BOOL (WINAPI * Real_EnumResourceNamesW)( HMODULE hModule, LPCWSTR lpType, ENUMRESNAMEPROCW lpEnumFunc, LONG_PTR lParam )
  = EnumResourceNamesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumResourceNamesW( HMODULE hModule, LPCWSTR lpType, ENUMRESNAMEPROCW lpEnumFunc, LONG_PTR lParam ){
  if(ChessWrapperSentry::Wrap("EnumResourceNamesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumResourceNamesW");
   }
  return Real_EnumResourceNamesW(hModule, lpType, lpEnumFunc, lParam);
}
BOOL (WINAPI * Real_EnumResourceTypesA)( HMODULE hModule, ENUMRESTYPEPROCA lpEnumFunc, LONG_PTR lParam )
  = EnumResourceTypesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumResourceTypesA( HMODULE hModule, ENUMRESTYPEPROCA lpEnumFunc, LONG_PTR lParam ){
  if(ChessWrapperSentry::Wrap("EnumResourceTypesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumResourceTypesA");
   }
  return Real_EnumResourceTypesA(hModule, lpEnumFunc, lParam);
}
BOOL (WINAPI * Real_EnumResourceTypesW)( HMODULE hModule, ENUMRESTYPEPROCW lpEnumFunc, LONG_PTR lParam )
  = EnumResourceTypesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumResourceTypesW( HMODULE hModule, ENUMRESTYPEPROCW lpEnumFunc, LONG_PTR lParam ){
  if(ChessWrapperSentry::Wrap("EnumResourceTypesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumResourceTypesW");
   }
  return Real_EnumResourceTypesW(hModule, lpEnumFunc, lParam);
}
BOOL (WINAPI * Real_EnumSystemCodePagesA)( CODEPAGE_ENUMPROCA lpCodePageEnumProc, DWORD dwFlags)
  = EnumSystemCodePagesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemCodePagesA( CODEPAGE_ENUMPROCA lpCodePageEnumProc, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumSystemCodePagesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemCodePagesA");
   }
  return Real_EnumSystemCodePagesA(lpCodePageEnumProc, dwFlags);
}
BOOL (WINAPI * Real_EnumSystemCodePagesW)( CODEPAGE_ENUMPROCW lpCodePageEnumProc, DWORD dwFlags)
  = EnumSystemCodePagesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemCodePagesW( CODEPAGE_ENUMPROCW lpCodePageEnumProc, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumSystemCodePagesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemCodePagesW");
   }
  return Real_EnumSystemCodePagesW(lpCodePageEnumProc, dwFlags);
}
UINT (WINAPI * Real_EnumSystemFirmwareTables)( DWORD FirmwareTableProviderSignature, PVOID pFirmwareTableEnumBuffer, DWORD BufferSize )
  = EnumSystemFirmwareTables;

__declspec(dllexport) UINT WINAPI Mine_EnumSystemFirmwareTables( DWORD FirmwareTableProviderSignature, PVOID pFirmwareTableEnumBuffer, DWORD BufferSize ){
  if(ChessWrapperSentry::Wrap("EnumSystemFirmwareTables")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemFirmwareTables");
   }
  return Real_EnumSystemFirmwareTables(FirmwareTableProviderSignature, pFirmwareTableEnumBuffer, BufferSize);
}
BOOL (WINAPI * Real_EnumSystemGeoID)( GEOCLASS GeoClass, GEOID ParentGeoId, GEO_ENUMPROC lpGeoEnumProc)
  = EnumSystemGeoID;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemGeoID( GEOCLASS GeoClass, GEOID ParentGeoId, GEO_ENUMPROC lpGeoEnumProc){
  if(ChessWrapperSentry::Wrap("EnumSystemGeoID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemGeoID");
   }
  return Real_EnumSystemGeoID(GeoClass, ParentGeoId, lpGeoEnumProc);
}
BOOL (WINAPI * Real_EnumSystemLanguageGroupsA)( LANGUAGEGROUP_ENUMPROCA lpLanguageGroupEnumProc, DWORD dwFlags, LONG_PTR lParam)
  = EnumSystemLanguageGroupsA;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemLanguageGroupsA( LANGUAGEGROUP_ENUMPROCA lpLanguageGroupEnumProc, DWORD dwFlags, LONG_PTR lParam){
  if(ChessWrapperSentry::Wrap("EnumSystemLanguageGroupsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemLanguageGroupsA");
   }
  return Real_EnumSystemLanguageGroupsA(lpLanguageGroupEnumProc, dwFlags, lParam);
}
BOOL (WINAPI * Real_EnumSystemLanguageGroupsW)( LANGUAGEGROUP_ENUMPROCW lpLanguageGroupEnumProc, DWORD dwFlags, LONG_PTR lParam)
  = EnumSystemLanguageGroupsW;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemLanguageGroupsW( LANGUAGEGROUP_ENUMPROCW lpLanguageGroupEnumProc, DWORD dwFlags, LONG_PTR lParam){
  if(ChessWrapperSentry::Wrap("EnumSystemLanguageGroupsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemLanguageGroupsW");
   }
  return Real_EnumSystemLanguageGroupsW(lpLanguageGroupEnumProc, dwFlags, lParam);
}
BOOL (WINAPI * Real_EnumSystemLocalesA)( LOCALE_ENUMPROCA lpLocaleEnumProc, DWORD dwFlags)
  = EnumSystemLocalesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemLocalesA( LOCALE_ENUMPROCA lpLocaleEnumProc, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumSystemLocalesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemLocalesA");
   }
  return Real_EnumSystemLocalesA(lpLocaleEnumProc, dwFlags);
}
BOOL (WINAPI * Real_EnumSystemLocalesW)( LOCALE_ENUMPROCW lpLocaleEnumProc, DWORD dwFlags)
  = EnumSystemLocalesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumSystemLocalesW( LOCALE_ENUMPROCW lpLocaleEnumProc, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumSystemLocalesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumSystemLocalesW");
   }
  return Real_EnumSystemLocalesW(lpLocaleEnumProc, dwFlags);
}
BOOL (WINAPI * Real_EnumTimeFormatsA)( TIMEFMT_ENUMPROCA lpTimeFmtEnumProc, LCID Locale, DWORD dwFlags)
  = EnumTimeFormatsA;

__declspec(dllexport) BOOL WINAPI Mine_EnumTimeFormatsA( TIMEFMT_ENUMPROCA lpTimeFmtEnumProc, LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumTimeFormatsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumTimeFormatsA");
   }
  return Real_EnumTimeFormatsA(lpTimeFmtEnumProc, Locale, dwFlags);
}
BOOL (WINAPI * Real_EnumTimeFormatsW)( TIMEFMT_ENUMPROCW lpTimeFmtEnumProc, LCID Locale, DWORD dwFlags)
  = EnumTimeFormatsW;

__declspec(dllexport) BOOL WINAPI Mine_EnumTimeFormatsW( TIMEFMT_ENUMPROCW lpTimeFmtEnumProc, LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("EnumTimeFormatsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumTimeFormatsW");
   }
  return Real_EnumTimeFormatsW(lpTimeFmtEnumProc, Locale, dwFlags);
}
BOOL (WINAPI * Real_EnumUILanguagesA)( UILANGUAGE_ENUMPROCA lpUILanguageEnumProc, DWORD dwFlags, LONG_PTR lParam)
  = EnumUILanguagesA;

__declspec(dllexport) BOOL WINAPI Mine_EnumUILanguagesA( UILANGUAGE_ENUMPROCA lpUILanguageEnumProc, DWORD dwFlags, LONG_PTR lParam){
  if(ChessWrapperSentry::Wrap("EnumUILanguagesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumUILanguagesA");
   }
  return Real_EnumUILanguagesA(lpUILanguageEnumProc, dwFlags, lParam);
}
BOOL (WINAPI * Real_EnumUILanguagesW)( UILANGUAGE_ENUMPROCW lpUILanguageEnumProc, DWORD dwFlags, LONG_PTR lParam)
  = EnumUILanguagesW;

__declspec(dllexport) BOOL WINAPI Mine_EnumUILanguagesW( UILANGUAGE_ENUMPROCW lpUILanguageEnumProc, DWORD dwFlags, LONG_PTR lParam){
  if(ChessWrapperSentry::Wrap("EnumUILanguagesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EnumUILanguagesW");
   }
  return Real_EnumUILanguagesW(lpUILanguageEnumProc, dwFlags, lParam);
}
DWORD (WINAPI * Real_EraseTape)( HANDLE hDevice, DWORD dwEraseType, BOOL bImmediate )
  = EraseTape;

__declspec(dllexport) DWORD WINAPI Mine_EraseTape( HANDLE hDevice, DWORD dwEraseType, BOOL bImmediate ){
  if(ChessWrapperSentry::Wrap("EraseTape")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EraseTape");
   }
  return Real_EraseTape(hDevice, dwEraseType, bImmediate);
}
BOOL (WINAPI * Real_EscapeCommFunction)( HANDLE hFile, DWORD dwFunc )
  = EscapeCommFunction;

__declspec(dllexport) BOOL WINAPI Mine_EscapeCommFunction( HANDLE hFile, DWORD dwFunc ){
  if(ChessWrapperSentry::Wrap("EscapeCommFunction")){
     ChessWrapperSentry sentry;
     Chess::LogCall("EscapeCommFunction");
   }
  return Real_EscapeCommFunction(hFile, dwFunc);
}
void (WINAPI * Real_ExitProcess)( UINT uExitCode )
  = ExitProcess;

__declspec(dllexport) void WINAPI Mine_ExitProcess( UINT uExitCode ){
  if(ChessWrapperSentry::Wrap("ExitProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ExitProcess");
   }
  return Real_ExitProcess(uExitCode);
}
void (WINAPI * Real_ExitThread)( DWORD dwExitCode )
  = ExitThread;

__declspec(dllexport) void WINAPI Mine_ExitThread( DWORD dwExitCode ){
  if(ChessWrapperSentry::Wrap("ExitThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ExitThread");
   }
  return Real_ExitThread(dwExitCode);
}
DWORD (WINAPI * Real_ExpandEnvironmentStringsA)( LPCSTR lpSrc, LPSTR lpDst, DWORD nSize )
  = ExpandEnvironmentStringsA;

__declspec(dllexport) DWORD WINAPI Mine_ExpandEnvironmentStringsA( LPCSTR lpSrc, LPSTR lpDst, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("ExpandEnvironmentStringsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ExpandEnvironmentStringsA");
   }
  return Real_ExpandEnvironmentStringsA(lpSrc, lpDst, nSize);
}
DWORD (WINAPI * Real_ExpandEnvironmentStringsW)( LPCWSTR lpSrc, LPWSTR lpDst, DWORD nSize )
  = ExpandEnvironmentStringsW;

__declspec(dllexport) DWORD WINAPI Mine_ExpandEnvironmentStringsW( LPCWSTR lpSrc, LPWSTR lpDst, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("ExpandEnvironmentStringsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ExpandEnvironmentStringsW");
   }
  return Real_ExpandEnvironmentStringsW(lpSrc, lpDst, nSize);
}
void (WINAPI * Real_FatalAppExitA)( UINT uAction, LPCSTR lpMessageText )
  = FatalAppExitA;

__declspec(dllexport) void WINAPI Mine_FatalAppExitA( UINT uAction, LPCSTR lpMessageText ){
  if(ChessWrapperSentry::Wrap("FatalAppExitA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FatalAppExitA");
   }
  return Real_FatalAppExitA(uAction, lpMessageText);
}
void (WINAPI * Real_FatalAppExitW)( UINT uAction, LPCWSTR lpMessageText )
  = FatalAppExitW;

__declspec(dllexport) void WINAPI Mine_FatalAppExitW( UINT uAction, LPCWSTR lpMessageText ){
  if(ChessWrapperSentry::Wrap("FatalAppExitW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FatalAppExitW");
   }
  return Real_FatalAppExitW(uAction, lpMessageText);
}
void (WINAPI * Real_FatalExit)( int ExitCode )
  = FatalExit;

__declspec(dllexport) void WINAPI Mine_FatalExit( int ExitCode ){
  if(ChessWrapperSentry::Wrap("FatalExit")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FatalExit");
   }
  return Real_FatalExit(ExitCode);
}
BOOL (WINAPI * Real_FileTimeToDosDateTime)( const FILETIME *lpFileTime, LPWORD lpFatDate, LPWORD lpFatTime )
  = FileTimeToDosDateTime;

__declspec(dllexport) BOOL WINAPI Mine_FileTimeToDosDateTime( const FILETIME *lpFileTime, LPWORD lpFatDate, LPWORD lpFatTime ){
  if(ChessWrapperSentry::Wrap("FileTimeToDosDateTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FileTimeToDosDateTime");
   }
  return Real_FileTimeToDosDateTime(lpFileTime, lpFatDate, lpFatTime);
}
BOOL (WINAPI * Real_FileTimeToLocalFileTime)( const FILETIME *lpFileTime, LPFILETIME lpLocalFileTime )
  = FileTimeToLocalFileTime;

__declspec(dllexport) BOOL WINAPI Mine_FileTimeToLocalFileTime( const FILETIME *lpFileTime, LPFILETIME lpLocalFileTime ){
  if(ChessWrapperSentry::Wrap("FileTimeToLocalFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FileTimeToLocalFileTime");
   }
  return Real_FileTimeToLocalFileTime(lpFileTime, lpLocalFileTime);
}
BOOL (WINAPI * Real_FileTimeToSystemTime)( const FILETIME *lpFileTime, LPSYSTEMTIME lpSystemTime )
  = FileTimeToSystemTime;

__declspec(dllexport) BOOL WINAPI Mine_FileTimeToSystemTime( const FILETIME *lpFileTime, LPSYSTEMTIME lpSystemTime ){
  if(ChessWrapperSentry::Wrap("FileTimeToSystemTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FileTimeToSystemTime");
   }
  return Real_FileTimeToSystemTime(lpFileTime, lpSystemTime);
}
BOOL (WINAPI * Real_FillConsoleOutputAttribute)( HANDLE hConsoleOutput, WORD wAttribute, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfAttrsWritten )
  = FillConsoleOutputAttribute;

__declspec(dllexport) BOOL WINAPI Mine_FillConsoleOutputAttribute( HANDLE hConsoleOutput, WORD wAttribute, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfAttrsWritten ){
  if(ChessWrapperSentry::Wrap("FillConsoleOutputAttribute")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FillConsoleOutputAttribute");
   }
  return Real_FillConsoleOutputAttribute(hConsoleOutput, wAttribute, nLength, dwWriteCoord, lpNumberOfAttrsWritten);
}
BOOL (WINAPI * Real_FillConsoleOutputCharacterA)( HANDLE hConsoleOutput, CHAR cCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten )
  = FillConsoleOutputCharacterA;

__declspec(dllexport) BOOL WINAPI Mine_FillConsoleOutputCharacterA( HANDLE hConsoleOutput, CHAR cCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten ){
  if(ChessWrapperSentry::Wrap("FillConsoleOutputCharacterA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FillConsoleOutputCharacterA");
   }
  return Real_FillConsoleOutputCharacterA(hConsoleOutput, cCharacter, nLength, dwWriteCoord, lpNumberOfCharsWritten);
}
BOOL (WINAPI * Real_FillConsoleOutputCharacterW)( HANDLE hConsoleOutput, WCHAR cCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten )
  = FillConsoleOutputCharacterW;

__declspec(dllexport) BOOL WINAPI Mine_FillConsoleOutputCharacterW( HANDLE hConsoleOutput, WCHAR cCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten ){
  if(ChessWrapperSentry::Wrap("FillConsoleOutputCharacterW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FillConsoleOutputCharacterW");
   }
  return Real_FillConsoleOutputCharacterW(hConsoleOutput, cCharacter, nLength, dwWriteCoord, lpNumberOfCharsWritten);
}
BOOL (WINAPI * Real_FindActCtxSectionGuid)( DWORD dwFlags, const GUID *lpExtensionGuid, ULONG ulSectionId, const GUID *lpGuidToFind, PACTCTX_SECTION_KEYED_DATA ReturnedData )
  = FindActCtxSectionGuid;

__declspec(dllexport) BOOL WINAPI Mine_FindActCtxSectionGuid( DWORD dwFlags, const GUID *lpExtensionGuid, ULONG ulSectionId, const GUID *lpGuidToFind, PACTCTX_SECTION_KEYED_DATA ReturnedData ){
  if(ChessWrapperSentry::Wrap("FindActCtxSectionGuid")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindActCtxSectionGuid");
   }
  return Real_FindActCtxSectionGuid(dwFlags, lpExtensionGuid, ulSectionId, lpGuidToFind, ReturnedData);
}
BOOL (WINAPI * Real_FindActCtxSectionStringA)( DWORD dwFlags, const GUID *lpExtensionGuid, ULONG ulSectionId, LPCSTR lpStringToFind, PACTCTX_SECTION_KEYED_DATA ReturnedData )
  = FindActCtxSectionStringA;

__declspec(dllexport) BOOL WINAPI Mine_FindActCtxSectionStringA( DWORD dwFlags, const GUID *lpExtensionGuid, ULONG ulSectionId, LPCSTR lpStringToFind, PACTCTX_SECTION_KEYED_DATA ReturnedData ){
  if(ChessWrapperSentry::Wrap("FindActCtxSectionStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindActCtxSectionStringA");
   }
  return Real_FindActCtxSectionStringA(dwFlags, lpExtensionGuid, ulSectionId, lpStringToFind, ReturnedData);
}
BOOL (WINAPI * Real_FindActCtxSectionStringW)( DWORD dwFlags, const GUID *lpExtensionGuid, ULONG ulSectionId, LPCWSTR lpStringToFind, PACTCTX_SECTION_KEYED_DATA ReturnedData )
  = FindActCtxSectionStringW;

__declspec(dllexport) BOOL WINAPI Mine_FindActCtxSectionStringW( DWORD dwFlags, const GUID *lpExtensionGuid, ULONG ulSectionId, LPCWSTR lpStringToFind, PACTCTX_SECTION_KEYED_DATA ReturnedData ){
  if(ChessWrapperSentry::Wrap("FindActCtxSectionStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindActCtxSectionStringW");
   }
  return Real_FindActCtxSectionStringW(dwFlags, lpExtensionGuid, ulSectionId, lpStringToFind, ReturnedData);
}
ATOM (WINAPI * Real_FindAtomA)( LPCSTR lpString )
  = FindAtomA;

__declspec(dllexport) ATOM WINAPI Mine_FindAtomA( LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("FindAtomA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindAtomA");
   }
  return Real_FindAtomA(lpString);
}
ATOM (WINAPI * Real_FindAtomW)( LPCWSTR lpString )
  = FindAtomW;

__declspec(dllexport) ATOM WINAPI Mine_FindAtomW( LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("FindAtomW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindAtomW");
   }
  return Real_FindAtomW(lpString);
}
BOOL (WINAPI * Real_FindClose)( HANDLE hFindFile )
  = FindClose;

__declspec(dllexport) BOOL WINAPI Mine_FindClose( HANDLE hFindFile ){
  if(ChessWrapperSentry::Wrap("FindClose")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindClose");
   }
  return Real_FindClose(hFindFile);
}
BOOL (WINAPI * Real_FindCloseChangeNotification)( HANDLE hChangeHandle )
  = FindCloseChangeNotification;

__declspec(dllexport) BOOL WINAPI Mine_FindCloseChangeNotification( HANDLE hChangeHandle ){
  if(ChessWrapperSentry::Wrap("FindCloseChangeNotification")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindCloseChangeNotification");
   }
  return Real_FindCloseChangeNotification(hChangeHandle);
}
HANDLE (WINAPI * Real_FindFirstChangeNotificationA)( LPCSTR lpPathName, BOOL bWatchSubtree, DWORD dwNotifyFilter )
  = FindFirstChangeNotificationA;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstChangeNotificationA( LPCSTR lpPathName, BOOL bWatchSubtree, DWORD dwNotifyFilter ){
  if(ChessWrapperSentry::Wrap("FindFirstChangeNotificationA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstChangeNotificationA");
   }
  return Real_FindFirstChangeNotificationA(lpPathName, bWatchSubtree, dwNotifyFilter);
}
HANDLE (WINAPI * Real_FindFirstChangeNotificationW)( LPCWSTR lpPathName, BOOL bWatchSubtree, DWORD dwNotifyFilter )
  = FindFirstChangeNotificationW;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstChangeNotificationW( LPCWSTR lpPathName, BOOL bWatchSubtree, DWORD dwNotifyFilter ){
  if(ChessWrapperSentry::Wrap("FindFirstChangeNotificationW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstChangeNotificationW");
   }
  return Real_FindFirstChangeNotificationW(lpPathName, bWatchSubtree, dwNotifyFilter);
}
HANDLE (WINAPI * Real_FindFirstFileA)( LPCSTR lpFileName, LPWIN32_FIND_DATAA lpFindFileData )
  = FindFirstFileA;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstFileA( LPCSTR lpFileName, LPWIN32_FIND_DATAA lpFindFileData ){
  if(ChessWrapperSentry::Wrap("FindFirstFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstFileA");
   }
  return Real_FindFirstFileA(lpFileName, lpFindFileData);
}
HANDLE (WINAPI * Real_FindFirstFileExA)( LPCSTR lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, LPVOID lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, LPVOID lpSearchFilter, DWORD dwAdditionalFlags )
  = FindFirstFileExA;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstFileExA( LPCSTR lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, LPVOID lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, LPVOID lpSearchFilter, DWORD dwAdditionalFlags ){
  if(ChessWrapperSentry::Wrap("FindFirstFileExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstFileExA");
   }
  return Real_FindFirstFileExA(lpFileName, fInfoLevelId, lpFindFileData, fSearchOp, lpSearchFilter, dwAdditionalFlags);
}
HANDLE (WINAPI * Real_FindFirstFileExW)( LPCWSTR lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, LPVOID lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, LPVOID lpSearchFilter, DWORD dwAdditionalFlags )
  = FindFirstFileExW;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstFileExW( LPCWSTR lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, LPVOID lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, LPVOID lpSearchFilter, DWORD dwAdditionalFlags ){
  if(ChessWrapperSentry::Wrap("FindFirstFileExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstFileExW");
   }
  return Real_FindFirstFileExW(lpFileName, fInfoLevelId, lpFindFileData, fSearchOp, lpSearchFilter, dwAdditionalFlags);
}
HANDLE (WINAPI * Real_FindFirstFileW)( LPCWSTR lpFileName, LPWIN32_FIND_DATAW lpFindFileData )
  = FindFirstFileW;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstFileW( LPCWSTR lpFileName, LPWIN32_FIND_DATAW lpFindFileData ){
  if(ChessWrapperSentry::Wrap("FindFirstFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstFileW");
   }
  return Real_FindFirstFileW(lpFileName, lpFindFileData);
}
HANDLE (WINAPI * Real_FindFirstVolumeA)( LPSTR lpszVolumeName, DWORD cchBufferLength )
  = FindFirstVolumeA;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstVolumeA( LPSTR lpszVolumeName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindFirstVolumeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstVolumeA");
   }
  return Real_FindFirstVolumeA(lpszVolumeName, cchBufferLength);
}
HANDLE (WINAPI * Real_FindFirstVolumeMountPointA)( LPCSTR lpszRootPathName, LPSTR lpszVolumeMountPoint, DWORD cchBufferLength )
  = FindFirstVolumeMountPointA;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstVolumeMountPointA( LPCSTR lpszRootPathName, LPSTR lpszVolumeMountPoint, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindFirstVolumeMountPointA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstVolumeMountPointA");
   }
  return Real_FindFirstVolumeMountPointA(lpszRootPathName, lpszVolumeMountPoint, cchBufferLength);
}
HANDLE (WINAPI * Real_FindFirstVolumeMountPointW)( LPCWSTR lpszRootPathName, LPWSTR lpszVolumeMountPoint, DWORD cchBufferLength )
  = FindFirstVolumeMountPointW;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstVolumeMountPointW( LPCWSTR lpszRootPathName, LPWSTR lpszVolumeMountPoint, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindFirstVolumeMountPointW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstVolumeMountPointW");
   }
  return Real_FindFirstVolumeMountPointW(lpszRootPathName, lpszVolumeMountPoint, cchBufferLength);
}
HANDLE (WINAPI * Real_FindFirstVolumeW)( LPWSTR lpszVolumeName, DWORD cchBufferLength )
  = FindFirstVolumeW;

__declspec(dllexport) HANDLE WINAPI Mine_FindFirstVolumeW( LPWSTR lpszVolumeName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindFirstVolumeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindFirstVolumeW");
   }
  return Real_FindFirstVolumeW(lpszVolumeName, cchBufferLength);
}
BOOL (WINAPI * Real_FindNextChangeNotification)( HANDLE hChangeHandle )
  = FindNextChangeNotification;

__declspec(dllexport) BOOL WINAPI Mine_FindNextChangeNotification( HANDLE hChangeHandle ){
  if(ChessWrapperSentry::Wrap("FindNextChangeNotification")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextChangeNotification");
   }
  return Real_FindNextChangeNotification(hChangeHandle);
}
BOOL (WINAPI * Real_FindNextFileA)( HANDLE hFindFile, LPWIN32_FIND_DATAA lpFindFileData )
  = FindNextFileA;

__declspec(dllexport) BOOL WINAPI Mine_FindNextFileA( HANDLE hFindFile, LPWIN32_FIND_DATAA lpFindFileData ){
  if(ChessWrapperSentry::Wrap("FindNextFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextFileA");
   }
  return Real_FindNextFileA(hFindFile, lpFindFileData);
}
BOOL (WINAPI * Real_FindNextFileW)( HANDLE hFindFile, LPWIN32_FIND_DATAW lpFindFileData )
  = FindNextFileW;

__declspec(dllexport) BOOL WINAPI Mine_FindNextFileW( HANDLE hFindFile, LPWIN32_FIND_DATAW lpFindFileData ){
  if(ChessWrapperSentry::Wrap("FindNextFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextFileW");
   }
  return Real_FindNextFileW(hFindFile, lpFindFileData);
}
BOOL (WINAPI * Real_FindNextVolumeA)( HANDLE hFindVolume, LPSTR lpszVolumeName, DWORD cchBufferLength )
  = FindNextVolumeA;

__declspec(dllexport) BOOL WINAPI Mine_FindNextVolumeA( HANDLE hFindVolume, LPSTR lpszVolumeName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindNextVolumeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextVolumeA");
   }
  return Real_FindNextVolumeA(hFindVolume, lpszVolumeName, cchBufferLength);
}
BOOL (WINAPI * Real_FindNextVolumeMountPointA)( HANDLE hFindVolumeMountPoint, LPSTR lpszVolumeMountPoint, DWORD cchBufferLength )
  = FindNextVolumeMountPointA;

__declspec(dllexport) BOOL WINAPI Mine_FindNextVolumeMountPointA( HANDLE hFindVolumeMountPoint, LPSTR lpszVolumeMountPoint, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindNextVolumeMountPointA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextVolumeMountPointA");
   }
  return Real_FindNextVolumeMountPointA(hFindVolumeMountPoint, lpszVolumeMountPoint, cchBufferLength);
}
BOOL (WINAPI * Real_FindNextVolumeMountPointW)( HANDLE hFindVolumeMountPoint, LPWSTR lpszVolumeMountPoint, DWORD cchBufferLength )
  = FindNextVolumeMountPointW;

__declspec(dllexport) BOOL WINAPI Mine_FindNextVolumeMountPointW( HANDLE hFindVolumeMountPoint, LPWSTR lpszVolumeMountPoint, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindNextVolumeMountPointW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextVolumeMountPointW");
   }
  return Real_FindNextVolumeMountPointW(hFindVolumeMountPoint, lpszVolumeMountPoint, cchBufferLength);
}
BOOL (WINAPI * Real_FindNextVolumeW)( HANDLE hFindVolume, LPWSTR lpszVolumeName, DWORD cchBufferLength )
  = FindNextVolumeW;

__declspec(dllexport) BOOL WINAPI Mine_FindNextVolumeW( HANDLE hFindVolume, LPWSTR lpszVolumeName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("FindNextVolumeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindNextVolumeW");
   }
  return Real_FindNextVolumeW(hFindVolume, lpszVolumeName, cchBufferLength);
}
HRSRC (WINAPI * Real_FindResourceA)( HMODULE hModule, LPCSTR lpName, LPCSTR lpType )
  = FindResourceA;

__declspec(dllexport) HRSRC WINAPI Mine_FindResourceA( HMODULE hModule, LPCSTR lpName, LPCSTR lpType ){
  if(ChessWrapperSentry::Wrap("FindResourceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindResourceA");
   }
  return Real_FindResourceA(hModule, lpName, lpType);
}
HRSRC (WINAPI * Real_FindResourceExA)( HMODULE hModule, LPCSTR lpType, LPCSTR lpName, WORD wLanguage )
  = FindResourceExA;

__declspec(dllexport) HRSRC WINAPI Mine_FindResourceExA( HMODULE hModule, LPCSTR lpType, LPCSTR lpName, WORD wLanguage ){
  if(ChessWrapperSentry::Wrap("FindResourceExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindResourceExA");
   }
  return Real_FindResourceExA(hModule, lpType, lpName, wLanguage);
}
HRSRC (WINAPI * Real_FindResourceExW)( HMODULE hModule, LPCWSTR lpType, LPCWSTR lpName, WORD wLanguage )
  = FindResourceExW;

__declspec(dllexport) HRSRC WINAPI Mine_FindResourceExW( HMODULE hModule, LPCWSTR lpType, LPCWSTR lpName, WORD wLanguage ){
  if(ChessWrapperSentry::Wrap("FindResourceExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindResourceExW");
   }
  return Real_FindResourceExW(hModule, lpType, lpName, wLanguage);
}
HRSRC (WINAPI * Real_FindResourceW)( HMODULE hModule, LPCWSTR lpName, LPCWSTR lpType )
  = FindResourceW;

__declspec(dllexport) HRSRC WINAPI Mine_FindResourceW( HMODULE hModule, LPCWSTR lpName, LPCWSTR lpType ){
  if(ChessWrapperSentry::Wrap("FindResourceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindResourceW");
   }
  return Real_FindResourceW(hModule, lpName, lpType);
}
BOOL (WINAPI * Real_FindVolumeClose)( HANDLE hFindVolume )
  = FindVolumeClose;

__declspec(dllexport) BOOL WINAPI Mine_FindVolumeClose( HANDLE hFindVolume ){
  if(ChessWrapperSentry::Wrap("FindVolumeClose")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindVolumeClose");
   }
  return Real_FindVolumeClose(hFindVolume);
}
BOOL (WINAPI * Real_FindVolumeMountPointClose)( HANDLE hFindVolumeMountPoint )
  = FindVolumeMountPointClose;

__declspec(dllexport) BOOL WINAPI Mine_FindVolumeMountPointClose( HANDLE hFindVolumeMountPoint ){
  if(ChessWrapperSentry::Wrap("FindVolumeMountPointClose")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FindVolumeMountPointClose");
   }
  return Real_FindVolumeMountPointClose(hFindVolumeMountPoint);
}
DWORD (WINAPI * Real_FlsAlloc)( PFLS_CALLBACK_FUNCTION lpCallback )
  = FlsAlloc;

__declspec(dllexport) DWORD WINAPI Mine_FlsAlloc( PFLS_CALLBACK_FUNCTION lpCallback ){
  if(ChessWrapperSentry::Wrap("FlsAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlsAlloc");
   }
  return Real_FlsAlloc(lpCallback);
}
BOOL (WINAPI * Real_FlsFree)( DWORD dwFlsIndex )
  = FlsFree;

__declspec(dllexport) BOOL WINAPI Mine_FlsFree( DWORD dwFlsIndex ){
  if(ChessWrapperSentry::Wrap("FlsFree")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlsFree");
   }
  return Real_FlsFree(dwFlsIndex);
}
PVOID (WINAPI * Real_FlsGetValue)( DWORD dwFlsIndex )
  = FlsGetValue;

__declspec(dllexport) PVOID WINAPI Mine_FlsGetValue( DWORD dwFlsIndex ){
  if(ChessWrapperSentry::Wrap("FlsGetValue")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlsGetValue");
   }
  return Real_FlsGetValue(dwFlsIndex);
}
BOOL (WINAPI * Real_FlsSetValue)( DWORD dwFlsIndex, PVOID lpFlsData )
  = FlsSetValue;

__declspec(dllexport) BOOL WINAPI Mine_FlsSetValue( DWORD dwFlsIndex, PVOID lpFlsData ){
  if(ChessWrapperSentry::Wrap("FlsSetValue")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlsSetValue");
   }
  return Real_FlsSetValue(dwFlsIndex, lpFlsData);
}
BOOL (WINAPI * Real_FlushConsoleInputBuffer)( HANDLE hConsoleInput )
  = FlushConsoleInputBuffer;

__declspec(dllexport) BOOL WINAPI Mine_FlushConsoleInputBuffer( HANDLE hConsoleInput ){
  if(ChessWrapperSentry::Wrap("FlushConsoleInputBuffer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlushConsoleInputBuffer");
   }
  return Real_FlushConsoleInputBuffer(hConsoleInput);
}
BOOL (WINAPI * Real_FlushFileBuffers)( HANDLE hFile )
  = FlushFileBuffers;

__declspec(dllexport) BOOL WINAPI Mine_FlushFileBuffers( HANDLE hFile ){
  if(ChessWrapperSentry::Wrap("FlushFileBuffers")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlushFileBuffers");
   }
  return Real_FlushFileBuffers(hFile);
}
BOOL (WINAPI * Real_FlushInstructionCache)( HANDLE hProcess, LPCVOID lpBaseAddress, SIZE_T dwSize )
  = FlushInstructionCache;

__declspec(dllexport) BOOL WINAPI Mine_FlushInstructionCache( HANDLE hProcess, LPCVOID lpBaseAddress, SIZE_T dwSize ){
  if(ChessWrapperSentry::Wrap("FlushInstructionCache")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlushInstructionCache");
   }
  return Real_FlushInstructionCache(hProcess, lpBaseAddress, dwSize);
}
BOOL (WINAPI * Real_FlushViewOfFile)( LPCVOID lpBaseAddress, SIZE_T dwNumberOfBytesToFlush )
  = FlushViewOfFile;

__declspec(dllexport) BOOL WINAPI Mine_FlushViewOfFile( LPCVOID lpBaseAddress, SIZE_T dwNumberOfBytesToFlush ){
  if(ChessWrapperSentry::Wrap("FlushViewOfFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FlushViewOfFile");
   }
  return Real_FlushViewOfFile(lpBaseAddress, dwNumberOfBytesToFlush);
}
int (WINAPI * Real_FoldStringA)( DWORD dwMapFlags, LPCSTR lpSrcStr, int cchSrc, LPSTR lpDestStr, int cchDest)
  = FoldStringA;

__declspec(dllexport) int WINAPI Mine_FoldStringA( DWORD dwMapFlags, LPCSTR lpSrcStr, int cchSrc, LPSTR lpDestStr, int cchDest){
  if(ChessWrapperSentry::Wrap("FoldStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FoldStringA");
   }
  return Real_FoldStringA(dwMapFlags, lpSrcStr, cchSrc, lpDestStr, cchDest);
}
int (WINAPI * Real_FoldStringW)( DWORD dwMapFlags, LPCWSTR lpSrcStr, int cchSrc, LPWSTR lpDestStr, int cchDest)
  = FoldStringW;

__declspec(dllexport) int WINAPI Mine_FoldStringW( DWORD dwMapFlags, LPCWSTR lpSrcStr, int cchSrc, LPWSTR lpDestStr, int cchDest){
  if(ChessWrapperSentry::Wrap("FoldStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FoldStringW");
   }
  return Real_FoldStringW(dwMapFlags, lpSrcStr, cchSrc, lpDestStr, cchDest);
}
DWORD (WINAPI * Real_FormatMessageA)( DWORD dwFlags, LPCVOID lpSource, DWORD dwMessageId, DWORD dwLanguageId, LPSTR lpBuffer, DWORD nSize, va_list *Arguments )
  = FormatMessageA;

__declspec(dllexport) DWORD WINAPI Mine_FormatMessageA( DWORD dwFlags, LPCVOID lpSource, DWORD dwMessageId, DWORD dwLanguageId, LPSTR lpBuffer, DWORD nSize, va_list *Arguments ){
  if(ChessWrapperSentry::Wrap("FormatMessageA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FormatMessageA");
   }
  return Real_FormatMessageA(dwFlags, lpSource, dwMessageId, dwLanguageId, lpBuffer, nSize, Arguments);
}
DWORD (WINAPI * Real_FormatMessageW)( DWORD dwFlags, LPCVOID lpSource, DWORD dwMessageId, DWORD dwLanguageId, LPWSTR lpBuffer, DWORD nSize, va_list *Arguments )
  = FormatMessageW;

__declspec(dllexport) DWORD WINAPI Mine_FormatMessageW( DWORD dwFlags, LPCVOID lpSource, DWORD dwMessageId, DWORD dwLanguageId, LPWSTR lpBuffer, DWORD nSize, va_list *Arguments ){
  if(ChessWrapperSentry::Wrap("FormatMessageW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FormatMessageW");
   }
  return Real_FormatMessageW(dwFlags, lpSource, dwMessageId, dwLanguageId, lpBuffer, nSize, Arguments);
}
BOOL (WINAPI * Real_FreeConsole)( void )
  = FreeConsole;

__declspec(dllexport) BOOL WINAPI Mine_FreeConsole( void ){
  if(ChessWrapperSentry::Wrap("FreeConsole")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeConsole");
   }
  return Real_FreeConsole();
}
BOOL (WINAPI * Real_FreeEnvironmentStringsA)( LPCH  dummy)
  = FreeEnvironmentStringsA;

__declspec(dllexport) BOOL WINAPI Mine_FreeEnvironmentStringsA( LPCH  dummy){
  if(ChessWrapperSentry::Wrap("FreeEnvironmentStringsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeEnvironmentStringsA");
   }
  return Real_FreeEnvironmentStringsA(dummy);
}
BOOL (WINAPI * Real_FreeEnvironmentStringsW)( LPWCH  dummy)
  = FreeEnvironmentStringsW;

__declspec(dllexport) BOOL WINAPI Mine_FreeEnvironmentStringsW( LPWCH  dummy){
  if(ChessWrapperSentry::Wrap("FreeEnvironmentStringsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeEnvironmentStringsW");
   }
  return Real_FreeEnvironmentStringsW(dummy);
}
BOOL (WINAPI * Real_FreeLibrary)( HMODULE hLibModule )
  = FreeLibrary;

__declspec(dllexport) BOOL WINAPI Mine_FreeLibrary( HMODULE hLibModule ){
  if(ChessWrapperSentry::Wrap("FreeLibrary")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeLibrary");
   }
  return Real_FreeLibrary(hLibModule);
}
void (WINAPI * Real_FreeLibraryAndExitThread)( HMODULE hLibModule, DWORD dwExitCode )
  = FreeLibraryAndExitThread;

__declspec(dllexport) void WINAPI Mine_FreeLibraryAndExitThread( HMODULE hLibModule, DWORD dwExitCode ){
  if(ChessWrapperSentry::Wrap("FreeLibraryAndExitThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeLibraryAndExitThread");
   }
  return Real_FreeLibraryAndExitThread(hLibModule, dwExitCode);
}
BOOL (WINAPI * Real_FreeResource)( HGLOBAL hResData )
  = FreeResource;

__declspec(dllexport) BOOL WINAPI Mine_FreeResource( HGLOBAL hResData ){
  if(ChessWrapperSentry::Wrap("FreeResource")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeResource");
   }
  return Real_FreeResource(hResData);
}
BOOL (WINAPI * Real_FreeUserPhysicalPages)( HANDLE hProcess, PULONG_PTR NumberOfPages, PULONG_PTR PageArray )
  = FreeUserPhysicalPages;

__declspec(dllexport) BOOL WINAPI Mine_FreeUserPhysicalPages( HANDLE hProcess, PULONG_PTR NumberOfPages, PULONG_PTR PageArray ){
  if(ChessWrapperSentry::Wrap("FreeUserPhysicalPages")){
     ChessWrapperSentry sentry;
     Chess::LogCall("FreeUserPhysicalPages");
   }
  return Real_FreeUserPhysicalPages(hProcess, NumberOfPages, PageArray);
}
BOOL (WINAPI * Real_GenerateConsoleCtrlEvent)( DWORD dwCtrlEvent, DWORD dwProcessGroupId )
  = GenerateConsoleCtrlEvent;

__declspec(dllexport) BOOL WINAPI Mine_GenerateConsoleCtrlEvent( DWORD dwCtrlEvent, DWORD dwProcessGroupId ){
  if(ChessWrapperSentry::Wrap("GenerateConsoleCtrlEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GenerateConsoleCtrlEvent");
   }
  return Real_GenerateConsoleCtrlEvent(dwCtrlEvent, dwProcessGroupId);
}
UINT (WINAPI * Real_GetACP)(void)
  = GetACP;

__declspec(dllexport) UINT WINAPI Mine_GetACP(void){
  if(ChessWrapperSentry::Wrap("GetACP")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetACP");
   }
  return Real_GetACP();
}
UINT (WINAPI * Real_GetAtomNameA)( ATOM nAtom, LPSTR lpBuffer, int nSize )
  = GetAtomNameA;

__declspec(dllexport) UINT WINAPI Mine_GetAtomNameA( ATOM nAtom, LPSTR lpBuffer, int nSize ){
  if(ChessWrapperSentry::Wrap("GetAtomNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetAtomNameA");
   }
  return Real_GetAtomNameA(nAtom, lpBuffer, nSize);
}
UINT (WINAPI * Real_GetAtomNameW)( ATOM nAtom, LPWSTR lpBuffer, int nSize )
  = GetAtomNameW;

__declspec(dllexport) UINT WINAPI Mine_GetAtomNameW( ATOM nAtom, LPWSTR lpBuffer, int nSize ){
  if(ChessWrapperSentry::Wrap("GetAtomNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetAtomNameW");
   }
  return Real_GetAtomNameW(nAtom, lpBuffer, nSize);
}
BOOL (WINAPI * Real_GetBinaryTypeA)( LPCSTR lpApplicationName, LPDWORD lpBinaryType )
  = GetBinaryTypeA;

__declspec(dllexport) BOOL WINAPI Mine_GetBinaryTypeA( LPCSTR lpApplicationName, LPDWORD lpBinaryType ){
  if(ChessWrapperSentry::Wrap("GetBinaryTypeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetBinaryTypeA");
   }
  return Real_GetBinaryTypeA(lpApplicationName, lpBinaryType);
}
BOOL (WINAPI * Real_GetBinaryTypeW)( LPCWSTR lpApplicationName, LPDWORD lpBinaryType )
  = GetBinaryTypeW;

__declspec(dllexport) BOOL WINAPI Mine_GetBinaryTypeW( LPCWSTR lpApplicationName, LPDWORD lpBinaryType ){
  if(ChessWrapperSentry::Wrap("GetBinaryTypeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetBinaryTypeW");
   }
  return Real_GetBinaryTypeW(lpApplicationName, lpBinaryType);
}
BOOL (WINAPI * Real_GetCPInfo)( UINT CodePage, LPCPINFO lpCPInfo)
  = GetCPInfo;

__declspec(dllexport) BOOL WINAPI Mine_GetCPInfo( UINT CodePage, LPCPINFO lpCPInfo){
  if(ChessWrapperSentry::Wrap("GetCPInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCPInfo");
   }
  return Real_GetCPInfo(CodePage, lpCPInfo);
}
BOOL (WINAPI * Real_GetCPInfoExA)( UINT CodePage, DWORD dwFlags, LPCPINFOEXA lpCPInfoEx)
  = GetCPInfoExA;

__declspec(dllexport) BOOL WINAPI Mine_GetCPInfoExA( UINT CodePage, DWORD dwFlags, LPCPINFOEXA lpCPInfoEx){
  if(ChessWrapperSentry::Wrap("GetCPInfoExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCPInfoExA");
   }
  return Real_GetCPInfoExA(CodePage, dwFlags, lpCPInfoEx);
}
BOOL (WINAPI * Real_GetCPInfoExW)( UINT CodePage, DWORD dwFlags, LPCPINFOEXW lpCPInfoEx)
  = GetCPInfoExW;

__declspec(dllexport) BOOL WINAPI Mine_GetCPInfoExW( UINT CodePage, DWORD dwFlags, LPCPINFOEXW lpCPInfoEx){
  if(ChessWrapperSentry::Wrap("GetCPInfoExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCPInfoExW");
   }
  return Real_GetCPInfoExW(CodePage, dwFlags, lpCPInfoEx);
}
int (WINAPI * Real_GetCalendarInfoA)( LCID Locale, CALID Calendar, CALTYPE CalType, LPSTR lpCalData, int cchData, LPDWORD lpValue)
  = GetCalendarInfoA;

__declspec(dllexport) int WINAPI Mine_GetCalendarInfoA( LCID Locale, CALID Calendar, CALTYPE CalType, LPSTR lpCalData, int cchData, LPDWORD lpValue){
  if(ChessWrapperSentry::Wrap("GetCalendarInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCalendarInfoA");
   }
  return Real_GetCalendarInfoA(Locale, Calendar, CalType, lpCalData, cchData, lpValue);
}
int (WINAPI * Real_GetCalendarInfoW)( LCID Locale, CALID Calendar, CALTYPE CalType, LPWSTR lpCalData, int cchData, LPDWORD lpValue)
  = GetCalendarInfoW;

__declspec(dllexport) int WINAPI Mine_GetCalendarInfoW( LCID Locale, CALID Calendar, CALTYPE CalType, LPWSTR lpCalData, int cchData, LPDWORD lpValue){
  if(ChessWrapperSentry::Wrap("GetCalendarInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCalendarInfoW");
   }
  return Real_GetCalendarInfoW(Locale, Calendar, CalType, lpCalData, cchData, lpValue);
}
BOOL (WINAPI * Real_GetCommConfig)( HANDLE hCommDev, LPCOMMCONFIG lpCC, LPDWORD lpdwSize )
  = GetCommConfig;

__declspec(dllexport) BOOL WINAPI Mine_GetCommConfig( HANDLE hCommDev, LPCOMMCONFIG lpCC, LPDWORD lpdwSize ){
  if(ChessWrapperSentry::Wrap("GetCommConfig")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommConfig");
   }
  return Real_GetCommConfig(hCommDev, lpCC, lpdwSize);
}
BOOL (WINAPI * Real_GetCommMask)( HANDLE hFile, LPDWORD lpEvtMask )
  = GetCommMask;

__declspec(dllexport) BOOL WINAPI Mine_GetCommMask( HANDLE hFile, LPDWORD lpEvtMask ){
  if(ChessWrapperSentry::Wrap("GetCommMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommMask");
   }
  return Real_GetCommMask(hFile, lpEvtMask);
}
BOOL (WINAPI * Real_GetCommModemStatus)( HANDLE hFile, LPDWORD lpModemStat )
  = GetCommModemStatus;

__declspec(dllexport) BOOL WINAPI Mine_GetCommModemStatus( HANDLE hFile, LPDWORD lpModemStat ){
  if(ChessWrapperSentry::Wrap("GetCommModemStatus")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommModemStatus");
   }
  return Real_GetCommModemStatus(hFile, lpModemStat);
}
BOOL (WINAPI * Real_GetCommProperties)( HANDLE hFile, LPCOMMPROP lpCommProp )
  = GetCommProperties;

__declspec(dllexport) BOOL WINAPI Mine_GetCommProperties( HANDLE hFile, LPCOMMPROP lpCommProp ){
  if(ChessWrapperSentry::Wrap("GetCommProperties")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommProperties");
   }
  return Real_GetCommProperties(hFile, lpCommProp);
}
BOOL (WINAPI * Real_GetCommState)( HANDLE hFile, LPDCB lpDCB )
  = GetCommState;

__declspec(dllexport) BOOL WINAPI Mine_GetCommState( HANDLE hFile, LPDCB lpDCB ){
  if(ChessWrapperSentry::Wrap("GetCommState")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommState");
   }
  return Real_GetCommState(hFile, lpDCB);
}
BOOL (WINAPI * Real_GetCommTimeouts)( HANDLE hFile, LPCOMMTIMEOUTS lpCommTimeouts )
  = GetCommTimeouts;

__declspec(dllexport) BOOL WINAPI Mine_GetCommTimeouts( HANDLE hFile, LPCOMMTIMEOUTS lpCommTimeouts ){
  if(ChessWrapperSentry::Wrap("GetCommTimeouts")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommTimeouts");
   }
  return Real_GetCommTimeouts(hFile, lpCommTimeouts);
}
LPSTR (WINAPI * Real_GetCommandLineA)( void )
  = GetCommandLineA;

__declspec(dllexport) LPSTR WINAPI Mine_GetCommandLineA( void ){
  if(ChessWrapperSentry::Wrap("GetCommandLineA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommandLineA");
   }
  return Real_GetCommandLineA();
}
LPWSTR (WINAPI * Real_GetCommandLineW)( void )
  = GetCommandLineW;

__declspec(dllexport) LPWSTR WINAPI Mine_GetCommandLineW( void ){
  if(ChessWrapperSentry::Wrap("GetCommandLineW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCommandLineW");
   }
  return Real_GetCommandLineW();
}
DWORD (WINAPI * Real_GetCompressedFileSizeA)( LPCSTR lpFileName, LPDWORD lpFileSizeHigh )
  = GetCompressedFileSizeA;

__declspec(dllexport) DWORD WINAPI Mine_GetCompressedFileSizeA( LPCSTR lpFileName, LPDWORD lpFileSizeHigh ){
  if(ChessWrapperSentry::Wrap("GetCompressedFileSizeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCompressedFileSizeA");
   }
  return Real_GetCompressedFileSizeA(lpFileName, lpFileSizeHigh);
}
DWORD (WINAPI * Real_GetCompressedFileSizeW)( LPCWSTR lpFileName, LPDWORD lpFileSizeHigh )
  = GetCompressedFileSizeW;

__declspec(dllexport) DWORD WINAPI Mine_GetCompressedFileSizeW( LPCWSTR lpFileName, LPDWORD lpFileSizeHigh ){
  if(ChessWrapperSentry::Wrap("GetCompressedFileSizeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCompressedFileSizeW");
   }
  return Real_GetCompressedFileSizeW(lpFileName, lpFileSizeHigh);
}
BOOL (WINAPI * Real_GetComputerNameA)( LPSTR lpBuffer, LPDWORD nSize )
  = GetComputerNameA;

__declspec(dllexport) BOOL WINAPI Mine_GetComputerNameA( LPSTR lpBuffer, LPDWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetComputerNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetComputerNameA");
   }
  return Real_GetComputerNameA(lpBuffer, nSize);
}
BOOL (WINAPI * Real_GetComputerNameExA)( COMPUTER_NAME_FORMAT NameType, LPSTR lpBuffer, LPDWORD nSize )
  = GetComputerNameExA;

__declspec(dllexport) BOOL WINAPI Mine_GetComputerNameExA( COMPUTER_NAME_FORMAT NameType, LPSTR lpBuffer, LPDWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetComputerNameExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetComputerNameExA");
   }
  return Real_GetComputerNameExA(NameType, lpBuffer, nSize);
}
BOOL (WINAPI * Real_GetComputerNameExW)( COMPUTER_NAME_FORMAT NameType, LPWSTR lpBuffer, LPDWORD nSize )
  = GetComputerNameExW;

__declspec(dllexport) BOOL WINAPI Mine_GetComputerNameExW( COMPUTER_NAME_FORMAT NameType, LPWSTR lpBuffer, LPDWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetComputerNameExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetComputerNameExW");
   }
  return Real_GetComputerNameExW(NameType, lpBuffer, nSize);
}
BOOL (WINAPI * Real_GetComputerNameW)( LPWSTR lpBuffer, LPDWORD nSize )
  = GetComputerNameW;

__declspec(dllexport) BOOL WINAPI Mine_GetComputerNameW( LPWSTR lpBuffer, LPDWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetComputerNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetComputerNameW");
   }
  return Real_GetComputerNameW(lpBuffer, nSize);
}
DWORD (WINAPI * Real_GetConsoleAliasA)( LPSTR Source, LPSTR TargetBuffer, DWORD TargetBufferLength, LPSTR ExeName)
  = GetConsoleAliasA;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasA( LPSTR Source, LPSTR TargetBuffer, DWORD TargetBufferLength, LPSTR ExeName){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasA");
   }
  return Real_GetConsoleAliasA(Source, TargetBuffer, TargetBufferLength, ExeName);
}
DWORD (WINAPI * Real_GetConsoleAliasExesA)( LPSTR ExeNameBuffer, DWORD ExeNameBufferLength)
  = GetConsoleAliasExesA;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasExesA( LPSTR ExeNameBuffer, DWORD ExeNameBufferLength){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasExesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasExesA");
   }
  return Real_GetConsoleAliasExesA(ExeNameBuffer, ExeNameBufferLength);
}
DWORD (WINAPI * Real_GetConsoleAliasExesLengthA)( void)
  = GetConsoleAliasExesLengthA;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasExesLengthA( void){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasExesLengthA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasExesLengthA");
   }
  return Real_GetConsoleAliasExesLengthA();
}
DWORD (WINAPI * Real_GetConsoleAliasExesLengthW)( void)
  = GetConsoleAliasExesLengthW;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasExesLengthW( void){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasExesLengthW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasExesLengthW");
   }
  return Real_GetConsoleAliasExesLengthW();
}
DWORD (WINAPI * Real_GetConsoleAliasExesW)( LPWSTR ExeNameBuffer, DWORD ExeNameBufferLength)
  = GetConsoleAliasExesW;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasExesW( LPWSTR ExeNameBuffer, DWORD ExeNameBufferLength){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasExesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasExesW");
   }
  return Real_GetConsoleAliasExesW(ExeNameBuffer, ExeNameBufferLength);
}
DWORD (WINAPI * Real_GetConsoleAliasW)( LPWSTR Source, LPWSTR TargetBuffer, DWORD TargetBufferLength, LPWSTR ExeName)
  = GetConsoleAliasW;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasW( LPWSTR Source, LPWSTR TargetBuffer, DWORD TargetBufferLength, LPWSTR ExeName){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasW");
   }
  return Real_GetConsoleAliasW(Source, TargetBuffer, TargetBufferLength, ExeName);
}
DWORD (WINAPI * Real_GetConsoleAliasesA)( LPSTR AliasBuffer, DWORD AliasBufferLength, LPSTR ExeName)
  = GetConsoleAliasesA;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasesA( LPSTR AliasBuffer, DWORD AliasBufferLength, LPSTR ExeName){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasesA");
   }
  return Real_GetConsoleAliasesA(AliasBuffer, AliasBufferLength, ExeName);
}
DWORD (WINAPI * Real_GetConsoleAliasesLengthA)( LPSTR ExeName)
  = GetConsoleAliasesLengthA;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasesLengthA( LPSTR ExeName){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasesLengthA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasesLengthA");
   }
  return Real_GetConsoleAliasesLengthA(ExeName);
}
DWORD (WINAPI * Real_GetConsoleAliasesLengthW)( LPWSTR ExeName)
  = GetConsoleAliasesLengthW;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasesLengthW( LPWSTR ExeName){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasesLengthW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasesLengthW");
   }
  return Real_GetConsoleAliasesLengthW(ExeName);
}
DWORD (WINAPI * Real_GetConsoleAliasesW)( LPWSTR AliasBuffer, DWORD AliasBufferLength, LPWSTR ExeName)
  = GetConsoleAliasesW;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleAliasesW( LPWSTR AliasBuffer, DWORD AliasBufferLength, LPWSTR ExeName){
  if(ChessWrapperSentry::Wrap("GetConsoleAliasesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleAliasesW");
   }
  return Real_GetConsoleAliasesW(AliasBuffer, AliasBufferLength, ExeName);
}
UINT (WINAPI * Real_GetConsoleCP)( void )
  = GetConsoleCP;

__declspec(dllexport) UINT WINAPI Mine_GetConsoleCP( void ){
  if(ChessWrapperSentry::Wrap("GetConsoleCP")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleCP");
   }
  return Real_GetConsoleCP();
}
BOOL (WINAPI * Real_GetConsoleCursorInfo)( HANDLE hConsoleOutput, PCONSOLE_CURSOR_INFO lpConsoleCursorInfo )
  = GetConsoleCursorInfo;

__declspec(dllexport) BOOL WINAPI Mine_GetConsoleCursorInfo( HANDLE hConsoleOutput, PCONSOLE_CURSOR_INFO lpConsoleCursorInfo ){
  if(ChessWrapperSentry::Wrap("GetConsoleCursorInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleCursorInfo");
   }
  return Real_GetConsoleCursorInfo(hConsoleOutput, lpConsoleCursorInfo);
}
BOOL (WINAPI * Real_GetConsoleDisplayMode)( LPDWORD lpModeFlags )
  = GetConsoleDisplayMode;

__declspec(dllexport) BOOL WINAPI Mine_GetConsoleDisplayMode( LPDWORD lpModeFlags ){
  if(ChessWrapperSentry::Wrap("GetConsoleDisplayMode")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleDisplayMode");
   }
  return Real_GetConsoleDisplayMode(lpModeFlags);
}
COORD (WINAPI * Real_GetConsoleFontSize)( HANDLE hConsoleOutput, DWORD nFont )
  = GetConsoleFontSize;

__declspec(dllexport) COORD WINAPI Mine_GetConsoleFontSize( HANDLE hConsoleOutput, DWORD nFont ){
  if(ChessWrapperSentry::Wrap("GetConsoleFontSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleFontSize");
   }
  return Real_GetConsoleFontSize(hConsoleOutput, nFont);
}
BOOL (WINAPI * Real_GetConsoleMode)( HANDLE hConsoleHandle, LPDWORD lpMode )
  = GetConsoleMode;

__declspec(dllexport) BOOL WINAPI Mine_GetConsoleMode( HANDLE hConsoleHandle, LPDWORD lpMode ){
  if(ChessWrapperSentry::Wrap("GetConsoleMode")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleMode");
   }
  return Real_GetConsoleMode(hConsoleHandle, lpMode);
}
UINT (WINAPI * Real_GetConsoleOutputCP)( void )
  = GetConsoleOutputCP;

__declspec(dllexport) UINT WINAPI Mine_GetConsoleOutputCP( void ){
  if(ChessWrapperSentry::Wrap("GetConsoleOutputCP")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleOutputCP");
   }
  return Real_GetConsoleOutputCP();
}
DWORD (WINAPI * Real_GetConsoleProcessList)( LPDWORD lpdwProcessList, DWORD dwProcessCount)
  = GetConsoleProcessList;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleProcessList( LPDWORD lpdwProcessList, DWORD dwProcessCount){
  if(ChessWrapperSentry::Wrap("GetConsoleProcessList")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleProcessList");
   }
  return Real_GetConsoleProcessList(lpdwProcessList, dwProcessCount);
}
BOOL (WINAPI * Real_GetConsoleScreenBufferInfo)( HANDLE hConsoleOutput, PCONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo )
  = GetConsoleScreenBufferInfo;

__declspec(dllexport) BOOL WINAPI Mine_GetConsoleScreenBufferInfo( HANDLE hConsoleOutput, PCONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo ){
  if(ChessWrapperSentry::Wrap("GetConsoleScreenBufferInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleScreenBufferInfo");
   }
  return Real_GetConsoleScreenBufferInfo(hConsoleOutput, lpConsoleScreenBufferInfo);
}
BOOL (WINAPI * Real_GetConsoleSelectionInfo)( PCONSOLE_SELECTION_INFO lpConsoleSelectionInfo )
  = GetConsoleSelectionInfo;

__declspec(dllexport) BOOL WINAPI Mine_GetConsoleSelectionInfo( PCONSOLE_SELECTION_INFO lpConsoleSelectionInfo ){
  if(ChessWrapperSentry::Wrap("GetConsoleSelectionInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleSelectionInfo");
   }
  return Real_GetConsoleSelectionInfo(lpConsoleSelectionInfo);
}
DWORD (WINAPI * Real_GetConsoleTitleA)( LPSTR lpConsoleTitle, DWORD nSize )
  = GetConsoleTitleA;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleTitleA( LPSTR lpConsoleTitle, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetConsoleTitleA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleTitleA");
   }
  return Real_GetConsoleTitleA(lpConsoleTitle, nSize);
}
DWORD (WINAPI * Real_GetConsoleTitleW)( LPWSTR lpConsoleTitle, DWORD nSize )
  = GetConsoleTitleW;

__declspec(dllexport) DWORD WINAPI Mine_GetConsoleTitleW( LPWSTR lpConsoleTitle, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetConsoleTitleW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleTitleW");
   }
  return Real_GetConsoleTitleW(lpConsoleTitle, nSize);
}
HWND (WINAPI * Real_GetConsoleWindow)( void )
  = GetConsoleWindow;

__declspec(dllexport) HWND WINAPI Mine_GetConsoleWindow( void ){
  if(ChessWrapperSentry::Wrap("GetConsoleWindow")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetConsoleWindow");
   }
  return Real_GetConsoleWindow();
}
int (WINAPI * Real_GetCurrencyFormatA)( LCID Locale, DWORD dwFlags, LPCSTR lpValue, const CURRENCYFMTA *lpFormat, LPSTR lpCurrencyStr, int cchCurrency)
  = GetCurrencyFormatA;

__declspec(dllexport) int WINAPI Mine_GetCurrencyFormatA( LCID Locale, DWORD dwFlags, LPCSTR lpValue, const CURRENCYFMTA *lpFormat, LPSTR lpCurrencyStr, int cchCurrency){
  if(ChessWrapperSentry::Wrap("GetCurrencyFormatA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrencyFormatA");
   }
  return Real_GetCurrencyFormatA(Locale, dwFlags, lpValue, lpFormat, lpCurrencyStr, cchCurrency);
}
int (WINAPI * Real_GetCurrencyFormatW)( LCID Locale, DWORD dwFlags, LPCWSTR lpValue, const CURRENCYFMTW *lpFormat, LPWSTR lpCurrencyStr, int cchCurrency)
  = GetCurrencyFormatW;

__declspec(dllexport) int WINAPI Mine_GetCurrencyFormatW( LCID Locale, DWORD dwFlags, LPCWSTR lpValue, const CURRENCYFMTW *lpFormat, LPWSTR lpCurrencyStr, int cchCurrency){
  if(ChessWrapperSentry::Wrap("GetCurrencyFormatW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrencyFormatW");
   }
  return Real_GetCurrencyFormatW(Locale, dwFlags, lpValue, lpFormat, lpCurrencyStr, cchCurrency);
}
BOOL (WINAPI * Real_GetCurrentActCtx)( HANDLE *lphActCtx)
  = GetCurrentActCtx;

__declspec(dllexport) BOOL WINAPI Mine_GetCurrentActCtx( HANDLE *lphActCtx){
  if(ChessWrapperSentry::Wrap("GetCurrentActCtx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentActCtx");
   }
  return Real_GetCurrentActCtx(lphActCtx);
}
BOOL (WINAPI * Real_GetCurrentConsoleFont)( HANDLE hConsoleOutput, BOOL bMaximumWindow, PCONSOLE_FONT_INFO lpConsoleCurrentFont )
  = GetCurrentConsoleFont;

__declspec(dllexport) BOOL WINAPI Mine_GetCurrentConsoleFont( HANDLE hConsoleOutput, BOOL bMaximumWindow, PCONSOLE_FONT_INFO lpConsoleCurrentFont ){
  if(ChessWrapperSentry::Wrap("GetCurrentConsoleFont")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentConsoleFont");
   }
  return Real_GetCurrentConsoleFont(hConsoleOutput, bMaximumWindow, lpConsoleCurrentFont);
}
DWORD (WINAPI * Real_GetCurrentDirectoryA)( DWORD nBufferLength, LPSTR lpBuffer )
  = GetCurrentDirectoryA;

__declspec(dllexport) DWORD WINAPI Mine_GetCurrentDirectoryA( DWORD nBufferLength, LPSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetCurrentDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentDirectoryA");
   }
  return Real_GetCurrentDirectoryA(nBufferLength, lpBuffer);
}
DWORD (WINAPI * Real_GetCurrentDirectoryW)( DWORD nBufferLength, LPWSTR lpBuffer )
  = GetCurrentDirectoryW;

__declspec(dllexport) DWORD WINAPI Mine_GetCurrentDirectoryW( DWORD nBufferLength, LPWSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetCurrentDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentDirectoryW");
   }
  return Real_GetCurrentDirectoryW(nBufferLength, lpBuffer);
}
HANDLE (WINAPI * Real_GetCurrentProcess)( void )
  = GetCurrentProcess;

__declspec(dllexport) HANDLE WINAPI Mine_GetCurrentProcess( void ){
  if(ChessWrapperSentry::Wrap("GetCurrentProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentProcess");
   }
  return Real_GetCurrentProcess();
}
DWORD (WINAPI * Real_GetCurrentProcessId)( void )
  = GetCurrentProcessId;

__declspec(dllexport) DWORD WINAPI Mine_GetCurrentProcessId( void ){
  if(ChessWrapperSentry::Wrap("GetCurrentProcessId")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentProcessId");
   }
  return Real_GetCurrentProcessId();
}
DWORD (WINAPI * Real_GetCurrentProcessorNumber)( void )
  = GetCurrentProcessorNumber;

__declspec(dllexport) DWORD WINAPI Mine_GetCurrentProcessorNumber( void ){
  if(ChessWrapperSentry::Wrap("GetCurrentProcessorNumber")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentProcessorNumber");
   }
  return Real_GetCurrentProcessorNumber();
}
HANDLE (WINAPI * Real_GetCurrentThread)( void )
  = GetCurrentThread;

__declspec(dllexport) HANDLE WINAPI Mine_GetCurrentThread( void ){
  if(ChessWrapperSentry::Wrap("GetCurrentThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentThread");
   }
  return Real_GetCurrentThread();
}
DWORD (WINAPI * Real_GetCurrentThreadId)( void )
  = GetCurrentThreadId;

__declspec(dllexport) DWORD WINAPI Mine_GetCurrentThreadId( void ){
  if(ChessWrapperSentry::Wrap("GetCurrentThreadId")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetCurrentThreadId");
   }
  return Real_GetCurrentThreadId();
}
int (WINAPI * Real_GetDateFormatA)( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpDate, LPCSTR lpFormat, LPSTR lpDateStr, int cchDate)
  = GetDateFormatA;

__declspec(dllexport) int WINAPI Mine_GetDateFormatA( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpDate, LPCSTR lpFormat, LPSTR lpDateStr, int cchDate){
  if(ChessWrapperSentry::Wrap("GetDateFormatA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDateFormatA");
   }
  return Real_GetDateFormatA(Locale, dwFlags, lpDate, lpFormat, lpDateStr, cchDate);
}
int (WINAPI * Real_GetDateFormatW)( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpDate, LPCWSTR lpFormat, LPWSTR lpDateStr, int cchDate)
  = GetDateFormatW;

__declspec(dllexport) int WINAPI Mine_GetDateFormatW( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpDate, LPCWSTR lpFormat, LPWSTR lpDateStr, int cchDate){
  if(ChessWrapperSentry::Wrap("GetDateFormatW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDateFormatW");
   }
  return Real_GetDateFormatW(Locale, dwFlags, lpDate, lpFormat, lpDateStr, cchDate);
}
BOOL (WINAPI * Real_GetDefaultCommConfigA)( LPCSTR lpszName, LPCOMMCONFIG lpCC, LPDWORD lpdwSize )
  = GetDefaultCommConfigA;

__declspec(dllexport) BOOL WINAPI Mine_GetDefaultCommConfigA( LPCSTR lpszName, LPCOMMCONFIG lpCC, LPDWORD lpdwSize ){
  if(ChessWrapperSentry::Wrap("GetDefaultCommConfigA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDefaultCommConfigA");
   }
  return Real_GetDefaultCommConfigA(lpszName, lpCC, lpdwSize);
}
BOOL (WINAPI * Real_GetDefaultCommConfigW)( LPCWSTR lpszName, LPCOMMCONFIG lpCC, LPDWORD lpdwSize )
  = GetDefaultCommConfigW;

__declspec(dllexport) BOOL WINAPI Mine_GetDefaultCommConfigW( LPCWSTR lpszName, LPCOMMCONFIG lpCC, LPDWORD lpdwSize ){
  if(ChessWrapperSentry::Wrap("GetDefaultCommConfigW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDefaultCommConfigW");
   }
  return Real_GetDefaultCommConfigW(lpszName, lpCC, lpdwSize);
}
BOOL (WINAPI * Real_GetDevicePowerState)( HANDLE hDevice, BOOL *pfOn )
  = GetDevicePowerState;

__declspec(dllexport) BOOL WINAPI Mine_GetDevicePowerState( HANDLE hDevice, BOOL *pfOn ){
  if(ChessWrapperSentry::Wrap("GetDevicePowerState")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDevicePowerState");
   }
  return Real_GetDevicePowerState(hDevice, pfOn);
}
BOOL (WINAPI * Real_GetDiskFreeSpaceA)( LPCSTR lpRootPathName, LPDWORD lpSectorsPerCluster, LPDWORD lpBytesPerSector, LPDWORD lpNumberOfFreeClusters, LPDWORD lpTotalNumberOfClusters )
  = GetDiskFreeSpaceA;

__declspec(dllexport) BOOL WINAPI Mine_GetDiskFreeSpaceA( LPCSTR lpRootPathName, LPDWORD lpSectorsPerCluster, LPDWORD lpBytesPerSector, LPDWORD lpNumberOfFreeClusters, LPDWORD lpTotalNumberOfClusters ){
  if(ChessWrapperSentry::Wrap("GetDiskFreeSpaceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDiskFreeSpaceA");
   }
  return Real_GetDiskFreeSpaceA(lpRootPathName, lpSectorsPerCluster, lpBytesPerSector, lpNumberOfFreeClusters, lpTotalNumberOfClusters);
}
BOOL (WINAPI * Real_GetDiskFreeSpaceExA)( LPCSTR lpDirectoryName, PULARGE_INTEGER lpFreeBytesAvailableToCaller, PULARGE_INTEGER lpTotalNumberOfBytes, PULARGE_INTEGER lpTotalNumberOfFreeBytes )
  = GetDiskFreeSpaceExA;

__declspec(dllexport) BOOL WINAPI Mine_GetDiskFreeSpaceExA( LPCSTR lpDirectoryName, PULARGE_INTEGER lpFreeBytesAvailableToCaller, PULARGE_INTEGER lpTotalNumberOfBytes, PULARGE_INTEGER lpTotalNumberOfFreeBytes ){
  if(ChessWrapperSentry::Wrap("GetDiskFreeSpaceExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDiskFreeSpaceExA");
   }
  return Real_GetDiskFreeSpaceExA(lpDirectoryName, lpFreeBytesAvailableToCaller, lpTotalNumberOfBytes, lpTotalNumberOfFreeBytes);
}
BOOL (WINAPI * Real_GetDiskFreeSpaceExW)( LPCWSTR lpDirectoryName, PULARGE_INTEGER lpFreeBytesAvailableToCaller, PULARGE_INTEGER lpTotalNumberOfBytes, PULARGE_INTEGER lpTotalNumberOfFreeBytes )
  = GetDiskFreeSpaceExW;

__declspec(dllexport) BOOL WINAPI Mine_GetDiskFreeSpaceExW( LPCWSTR lpDirectoryName, PULARGE_INTEGER lpFreeBytesAvailableToCaller, PULARGE_INTEGER lpTotalNumberOfBytes, PULARGE_INTEGER lpTotalNumberOfFreeBytes ){
  if(ChessWrapperSentry::Wrap("GetDiskFreeSpaceExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDiskFreeSpaceExW");
   }
  return Real_GetDiskFreeSpaceExW(lpDirectoryName, lpFreeBytesAvailableToCaller, lpTotalNumberOfBytes, lpTotalNumberOfFreeBytes);
}
BOOL (WINAPI * Real_GetDiskFreeSpaceW)( LPCWSTR lpRootPathName, LPDWORD lpSectorsPerCluster, LPDWORD lpBytesPerSector, LPDWORD lpNumberOfFreeClusters, LPDWORD lpTotalNumberOfClusters )
  = GetDiskFreeSpaceW;

__declspec(dllexport) BOOL WINAPI Mine_GetDiskFreeSpaceW( LPCWSTR lpRootPathName, LPDWORD lpSectorsPerCluster, LPDWORD lpBytesPerSector, LPDWORD lpNumberOfFreeClusters, LPDWORD lpTotalNumberOfClusters ){
  if(ChessWrapperSentry::Wrap("GetDiskFreeSpaceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDiskFreeSpaceW");
   }
  return Real_GetDiskFreeSpaceW(lpRootPathName, lpSectorsPerCluster, lpBytesPerSector, lpNumberOfFreeClusters, lpTotalNumberOfClusters);
}
DWORD (WINAPI * Real_GetDllDirectoryA)( DWORD nBufferLength, LPSTR lpBuffer )
  = GetDllDirectoryA;

__declspec(dllexport) DWORD WINAPI Mine_GetDllDirectoryA( DWORD nBufferLength, LPSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetDllDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDllDirectoryA");
   }
  return Real_GetDllDirectoryA(nBufferLength, lpBuffer);
}
DWORD (WINAPI * Real_GetDllDirectoryW)( DWORD nBufferLength, LPWSTR lpBuffer )
  = GetDllDirectoryW;

__declspec(dllexport) DWORD WINAPI Mine_GetDllDirectoryW( DWORD nBufferLength, LPWSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetDllDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDllDirectoryW");
   }
  return Real_GetDllDirectoryW(nBufferLength, lpBuffer);
}
UINT (WINAPI * Real_GetDriveTypeA)( LPCSTR lpRootPathName )
  = GetDriveTypeA;

__declspec(dllexport) UINT WINAPI Mine_GetDriveTypeA( LPCSTR lpRootPathName ){
  if(ChessWrapperSentry::Wrap("GetDriveTypeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDriveTypeA");
   }
  return Real_GetDriveTypeA(lpRootPathName);
}
UINT (WINAPI * Real_GetDriveTypeW)( LPCWSTR lpRootPathName )
  = GetDriveTypeW;

__declspec(dllexport) UINT WINAPI Mine_GetDriveTypeW( LPCWSTR lpRootPathName ){
  if(ChessWrapperSentry::Wrap("GetDriveTypeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetDriveTypeW");
   }
  return Real_GetDriveTypeW(lpRootPathName);
}
LPCH (WINAPI * Real_GetEnvironmentStrings)( void )
  = GetEnvironmentStrings;

__declspec(dllexport) LPCH WINAPI Mine_GetEnvironmentStrings( void ){
  if(ChessWrapperSentry::Wrap("GetEnvironmentStrings")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetEnvironmentStrings");
   }
  return Real_GetEnvironmentStrings();
}
LPWCH (WINAPI * Real_GetEnvironmentStringsW)( void )
  = GetEnvironmentStringsW;

__declspec(dllexport) LPWCH WINAPI Mine_GetEnvironmentStringsW( void ){
  if(ChessWrapperSentry::Wrap("GetEnvironmentStringsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetEnvironmentStringsW");
   }
  return Real_GetEnvironmentStringsW();
}
DWORD (WINAPI * Real_GetEnvironmentVariableA)( LPCSTR lpName, LPSTR lpBuffer, DWORD nSize )
  = GetEnvironmentVariableA;

__declspec(dllexport) DWORD WINAPI Mine_GetEnvironmentVariableA( LPCSTR lpName, LPSTR lpBuffer, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetEnvironmentVariableA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetEnvironmentVariableA");
   }
  return Real_GetEnvironmentVariableA(lpName, lpBuffer, nSize);
}
DWORD (WINAPI * Real_GetEnvironmentVariableW)( LPCWSTR lpName, LPWSTR lpBuffer, DWORD nSize )
  = GetEnvironmentVariableW;

__declspec(dllexport) DWORD WINAPI Mine_GetEnvironmentVariableW( LPCWSTR lpName, LPWSTR lpBuffer, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetEnvironmentVariableW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetEnvironmentVariableW");
   }
  return Real_GetEnvironmentVariableW(lpName, lpBuffer, nSize);
}
BOOL (WINAPI * Real_GetExitCodeProcess)( HANDLE hProcess, LPDWORD lpExitCode )
  = GetExitCodeProcess;

__declspec(dllexport) BOOL WINAPI Mine_GetExitCodeProcess( HANDLE hProcess, LPDWORD lpExitCode ){
  if(ChessWrapperSentry::Wrap("GetExitCodeProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetExitCodeProcess");
   }
  return Real_GetExitCodeProcess(hProcess, lpExitCode);
}
BOOL (WINAPI * Real_GetExitCodeThread)( HANDLE hThread, LPDWORD lpExitCode )
  = GetExitCodeThread;

__declspec(dllexport) BOOL WINAPI Mine_GetExitCodeThread( HANDLE hThread, LPDWORD lpExitCode ){
  if(ChessWrapperSentry::Wrap("GetExitCodeThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetExitCodeThread");
   }
  return Real_GetExitCodeThread(hThread, lpExitCode);
}
DWORD (WINAPI * Real_GetFileAttributesA)( LPCSTR lpFileName )
  = GetFileAttributesA;

__declspec(dllexport) DWORD WINAPI Mine_GetFileAttributesA( LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetFileAttributesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileAttributesA");
   }
  return Real_GetFileAttributesA(lpFileName);
}
BOOL (WINAPI * Real_GetFileAttributesExA)( LPCSTR lpFileName, GET_FILEEX_INFO_LEVELS fInfoLevelId, LPVOID lpFileInformation )
  = GetFileAttributesExA;

__declspec(dllexport) BOOL WINAPI Mine_GetFileAttributesExA( LPCSTR lpFileName, GET_FILEEX_INFO_LEVELS fInfoLevelId, LPVOID lpFileInformation ){
  if(ChessWrapperSentry::Wrap("GetFileAttributesExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileAttributesExA");
   }
  return Real_GetFileAttributesExA(lpFileName, fInfoLevelId, lpFileInformation);
}
BOOL (WINAPI * Real_GetFileAttributesExW)( LPCWSTR lpFileName, GET_FILEEX_INFO_LEVELS fInfoLevelId, LPVOID lpFileInformation )
  = GetFileAttributesExW;

__declspec(dllexport) BOOL WINAPI Mine_GetFileAttributesExW( LPCWSTR lpFileName, GET_FILEEX_INFO_LEVELS fInfoLevelId, LPVOID lpFileInformation ){
  if(ChessWrapperSentry::Wrap("GetFileAttributesExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileAttributesExW");
   }
  return Real_GetFileAttributesExW(lpFileName, fInfoLevelId, lpFileInformation);
}
DWORD (WINAPI * Real_GetFileAttributesW)( LPCWSTR lpFileName )
  = GetFileAttributesW;

__declspec(dllexport) DWORD WINAPI Mine_GetFileAttributesW( LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetFileAttributesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileAttributesW");
   }
  return Real_GetFileAttributesW(lpFileName);
}
BOOL (WINAPI * Real_GetFileInformationByHandle)( HANDLE hFile, LPBY_HANDLE_FILE_INFORMATION lpFileInformation )
  = GetFileInformationByHandle;

__declspec(dllexport) BOOL WINAPI Mine_GetFileInformationByHandle( HANDLE hFile, LPBY_HANDLE_FILE_INFORMATION lpFileInformation ){
  if(ChessWrapperSentry::Wrap("GetFileInformationByHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileInformationByHandle");
   }
  return Real_GetFileInformationByHandle(hFile, lpFileInformation);
}
DWORD (WINAPI * Real_GetFileSize)( HANDLE hFile, LPDWORD lpFileSizeHigh )
  = GetFileSize;

__declspec(dllexport) DWORD WINAPI Mine_GetFileSize( HANDLE hFile, LPDWORD lpFileSizeHigh ){
  if(ChessWrapperSentry::Wrap("GetFileSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileSize");
   }
  return Real_GetFileSize(hFile, lpFileSizeHigh);
}
BOOL (WINAPI * Real_GetFileSizeEx)( HANDLE hFile, PLARGE_INTEGER lpFileSize )
  = GetFileSizeEx;

__declspec(dllexport) BOOL WINAPI Mine_GetFileSizeEx( HANDLE hFile, PLARGE_INTEGER lpFileSize ){
  if(ChessWrapperSentry::Wrap("GetFileSizeEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileSizeEx");
   }
  return Real_GetFileSizeEx(hFile, lpFileSize);
}
BOOL (WINAPI * Real_GetFileTime)( HANDLE hFile, LPFILETIME lpCreationTime, LPFILETIME lpLastAccessTime, LPFILETIME lpLastWriteTime )
  = GetFileTime;

__declspec(dllexport) BOOL WINAPI Mine_GetFileTime( HANDLE hFile, LPFILETIME lpCreationTime, LPFILETIME lpLastAccessTime, LPFILETIME lpLastWriteTime ){
  if(ChessWrapperSentry::Wrap("GetFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileTime");
   }
  return Real_GetFileTime(hFile, lpCreationTime, lpLastAccessTime, lpLastWriteTime);
}
DWORD (WINAPI * Real_GetFileType)( HANDLE hFile )
  = GetFileType;

__declspec(dllexport) DWORD WINAPI Mine_GetFileType( HANDLE hFile ){
  if(ChessWrapperSentry::Wrap("GetFileType")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFileType");
   }
  return Real_GetFileType(hFile);
}
DWORD (WINAPI * Real_GetFirmwareEnvironmentVariableA)( LPCSTR lpName, LPCSTR lpGuid, PVOID pBuffer, DWORD nSize )
  = GetFirmwareEnvironmentVariableA;

__declspec(dllexport) DWORD WINAPI Mine_GetFirmwareEnvironmentVariableA( LPCSTR lpName, LPCSTR lpGuid, PVOID pBuffer, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetFirmwareEnvironmentVariableA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFirmwareEnvironmentVariableA");
   }
  return Real_GetFirmwareEnvironmentVariableA(lpName, lpGuid, pBuffer, nSize);
}
DWORD (WINAPI * Real_GetFirmwareEnvironmentVariableW)( LPCWSTR lpName, LPCWSTR lpGuid, PVOID pBuffer, DWORD nSize )
  = GetFirmwareEnvironmentVariableW;

__declspec(dllexport) DWORD WINAPI Mine_GetFirmwareEnvironmentVariableW( LPCWSTR lpName, LPCWSTR lpGuid, PVOID pBuffer, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetFirmwareEnvironmentVariableW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFirmwareEnvironmentVariableW");
   }
  return Real_GetFirmwareEnvironmentVariableW(lpName, lpGuid, pBuffer, nSize);
}
DWORD (WINAPI * Real_GetFullPathNameA)( LPCSTR lpFileName, DWORD nBufferLength, LPSTR lpBuffer, LPSTR *lpFilePart )
  = GetFullPathNameA;

__declspec(dllexport) DWORD WINAPI Mine_GetFullPathNameA( LPCSTR lpFileName, DWORD nBufferLength, LPSTR lpBuffer, LPSTR *lpFilePart ){
  if(ChessWrapperSentry::Wrap("GetFullPathNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFullPathNameA");
   }
  return Real_GetFullPathNameA(lpFileName, nBufferLength, lpBuffer, lpFilePart);
}
DWORD (WINAPI * Real_GetFullPathNameW)( LPCWSTR lpFileName, DWORD nBufferLength, LPWSTR lpBuffer, LPWSTR *lpFilePart )
  = GetFullPathNameW;

__declspec(dllexport) DWORD WINAPI Mine_GetFullPathNameW( LPCWSTR lpFileName, DWORD nBufferLength, LPWSTR lpBuffer, LPWSTR *lpFilePart ){
  if(ChessWrapperSentry::Wrap("GetFullPathNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetFullPathNameW");
   }
  return Real_GetFullPathNameW(lpFileName, nBufferLength, lpBuffer, lpFilePart);
}
int (WINAPI * Real_GetGeoInfoA)( GEOID Location, GEOTYPE GeoType, LPSTR lpGeoData, int cchData, LANGID LangId)
  = GetGeoInfoA;

__declspec(dllexport) int WINAPI Mine_GetGeoInfoA( GEOID Location, GEOTYPE GeoType, LPSTR lpGeoData, int cchData, LANGID LangId){
  if(ChessWrapperSentry::Wrap("GetGeoInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetGeoInfoA");
   }
  return Real_GetGeoInfoA(Location, GeoType, lpGeoData, cchData, LangId);
}
int (WINAPI * Real_GetGeoInfoW)( GEOID Location, GEOTYPE GeoType, LPWSTR lpGeoData, int cchData, LANGID LangId)
  = GetGeoInfoW;

__declspec(dllexport) int WINAPI Mine_GetGeoInfoW( GEOID Location, GEOTYPE GeoType, LPWSTR lpGeoData, int cchData, LANGID LangId){
  if(ChessWrapperSentry::Wrap("GetGeoInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetGeoInfoW");
   }
  return Real_GetGeoInfoW(Location, GeoType, lpGeoData, cchData, LangId);
}
BOOL (WINAPI * Real_GetHandleInformation)( HANDLE hObject, LPDWORD lpdwFlags )
  = GetHandleInformation;

__declspec(dllexport) BOOL WINAPI Mine_GetHandleInformation( HANDLE hObject, LPDWORD lpdwFlags ){
  if(ChessWrapperSentry::Wrap("GetHandleInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetHandleInformation");
   }
  return Real_GetHandleInformation(hObject, lpdwFlags);
}
SIZE_T (WINAPI * Real_GetLargePageMinimum)( void )
  = GetLargePageMinimum;

__declspec(dllexport) SIZE_T WINAPI Mine_GetLargePageMinimum( void ){
  if(ChessWrapperSentry::Wrap("GetLargePageMinimum")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLargePageMinimum");
   }
  return Real_GetLargePageMinimum();
}
COORD (WINAPI * Real_GetLargestConsoleWindowSize)( HANDLE hConsoleOutput )
  = GetLargestConsoleWindowSize;

__declspec(dllexport) COORD WINAPI Mine_GetLargestConsoleWindowSize( HANDLE hConsoleOutput ){
  if(ChessWrapperSentry::Wrap("GetLargestConsoleWindowSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLargestConsoleWindowSize");
   }
  return Real_GetLargestConsoleWindowSize(hConsoleOutput);
}
DWORD (WINAPI * Real_GetLastError)( void )
  = GetLastError;

__declspec(dllexport) DWORD WINAPI Mine_GetLastError( void ){
  if(ChessWrapperSentry::Wrap("GetLastError")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLastError");
   }
  return Real_GetLastError();
}
void (WINAPI * Real_GetLocalTime)( LPSYSTEMTIME lpSystemTime )
  = GetLocalTime;

__declspec(dllexport) void WINAPI Mine_GetLocalTime( LPSYSTEMTIME lpSystemTime ){
  if(ChessWrapperSentry::Wrap("GetLocalTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLocalTime");
   }
  return Real_GetLocalTime(lpSystemTime);
}
int (WINAPI * Real_GetLocaleInfoA)( LCID Locale, LCTYPE LCType, LPSTR lpLCData, int cchData)
  = GetLocaleInfoA;

__declspec(dllexport) int WINAPI Mine_GetLocaleInfoA( LCID Locale, LCTYPE LCType, LPSTR lpLCData, int cchData){
  if(ChessWrapperSentry::Wrap("GetLocaleInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLocaleInfoA");
   }
  return Real_GetLocaleInfoA(Locale, LCType, lpLCData, cchData);
}
int (WINAPI * Real_GetLocaleInfoW)( LCID Locale, LCTYPE LCType, LPWSTR lpLCData, int cchData)
  = GetLocaleInfoW;

__declspec(dllexport) int WINAPI Mine_GetLocaleInfoW( LCID Locale, LCTYPE LCType, LPWSTR lpLCData, int cchData){
  if(ChessWrapperSentry::Wrap("GetLocaleInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLocaleInfoW");
   }
  return Real_GetLocaleInfoW(Locale, LCType, lpLCData, cchData);
}
DWORD (WINAPI * Real_GetLogicalDriveStringsA)( DWORD nBufferLength, LPSTR lpBuffer )
  = GetLogicalDriveStringsA;

__declspec(dllexport) DWORD WINAPI Mine_GetLogicalDriveStringsA( DWORD nBufferLength, LPSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetLogicalDriveStringsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLogicalDriveStringsA");
   }
  return Real_GetLogicalDriveStringsA(nBufferLength, lpBuffer);
}
DWORD (WINAPI * Real_GetLogicalDriveStringsW)( DWORD nBufferLength, LPWSTR lpBuffer )
  = GetLogicalDriveStringsW;

__declspec(dllexport) DWORD WINAPI Mine_GetLogicalDriveStringsW( DWORD nBufferLength, LPWSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetLogicalDriveStringsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLogicalDriveStringsW");
   }
  return Real_GetLogicalDriveStringsW(nBufferLength, lpBuffer);
}
DWORD (WINAPI * Real_GetLogicalDrives)( void )
  = GetLogicalDrives;

__declspec(dllexport) DWORD WINAPI Mine_GetLogicalDrives( void ){
  if(ChessWrapperSentry::Wrap("GetLogicalDrives")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLogicalDrives");
   }
  return Real_GetLogicalDrives();
}
BOOL (WINAPI * Real_GetLogicalProcessorInformation)( PSYSTEM_LOGICAL_PROCESSOR_INFORMATION Buffer, PDWORD ReturnedLength )
  = GetLogicalProcessorInformation;

__declspec(dllexport) BOOL WINAPI Mine_GetLogicalProcessorInformation( PSYSTEM_LOGICAL_PROCESSOR_INFORMATION Buffer, PDWORD ReturnedLength ){
  if(ChessWrapperSentry::Wrap("GetLogicalProcessorInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLogicalProcessorInformation");
   }
  return Real_GetLogicalProcessorInformation(Buffer, ReturnedLength);
}
DWORD (WINAPI * Real_GetLongPathNameA)( LPCSTR lpszShortPath, LPSTR lpszLongPath, DWORD cchBuffer )
  = GetLongPathNameA;

__declspec(dllexport) DWORD WINAPI Mine_GetLongPathNameA( LPCSTR lpszShortPath, LPSTR lpszLongPath, DWORD cchBuffer ){
  if(ChessWrapperSentry::Wrap("GetLongPathNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLongPathNameA");
   }
  return Real_GetLongPathNameA(lpszShortPath, lpszLongPath, cchBuffer);
}
DWORD (WINAPI * Real_GetLongPathNameW)( LPCWSTR lpszShortPath, LPWSTR lpszLongPath, DWORD cchBuffer )
  = GetLongPathNameW;

__declspec(dllexport) DWORD WINAPI Mine_GetLongPathNameW( LPCWSTR lpszShortPath, LPWSTR lpszLongPath, DWORD cchBuffer ){
  if(ChessWrapperSentry::Wrap("GetLongPathNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetLongPathNameW");
   }
  return Real_GetLongPathNameW(lpszShortPath, lpszLongPath, cchBuffer);
}
BOOL (WINAPI * Real_GetMailslotInfo)( HANDLE hMailslot, LPDWORD lpMaxMessageSize, LPDWORD lpNextSize, LPDWORD lpMessageCount, LPDWORD lpReadTimeout )
  = GetMailslotInfo;

__declspec(dllexport) BOOL WINAPI Mine_GetMailslotInfo( HANDLE hMailslot, LPDWORD lpMaxMessageSize, LPDWORD lpNextSize, LPDWORD lpMessageCount, LPDWORD lpReadTimeout ){
  if(ChessWrapperSentry::Wrap("GetMailslotInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetMailslotInfo");
   }
  return Real_GetMailslotInfo(hMailslot, lpMaxMessageSize, lpNextSize, lpMessageCount, lpReadTimeout);
}
DWORD (WINAPI * Real_GetModuleFileNameA)( HMODULE hModule, LPCH lpFilename, DWORD nSize )
  = GetModuleFileNameA;

__declspec(dllexport) DWORD WINAPI Mine_GetModuleFileNameA( HMODULE hModule, LPCH lpFilename, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetModuleFileNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetModuleFileNameA");
   }
  return Real_GetModuleFileNameA(hModule, lpFilename, nSize);
}
DWORD (WINAPI * Real_GetModuleFileNameW)( HMODULE hModule, LPWCH lpFilename, DWORD nSize )
  = GetModuleFileNameW;

__declspec(dllexport) DWORD WINAPI Mine_GetModuleFileNameW( HMODULE hModule, LPWCH lpFilename, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetModuleFileNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetModuleFileNameW");
   }
  return Real_GetModuleFileNameW(hModule, lpFilename, nSize);
}
HMODULE (WINAPI * Real_GetModuleHandleA)( LPCSTR lpModuleName )
  = GetModuleHandleA;

__declspec(dllexport) HMODULE WINAPI Mine_GetModuleHandleA( LPCSTR lpModuleName ){
  if(ChessWrapperSentry::Wrap("GetModuleHandleA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetModuleHandleA");
   }
  return Real_GetModuleHandleA(lpModuleName);
}
BOOL (WINAPI * Real_GetModuleHandleExA)( DWORD dwFlags, LPCSTR lpModuleName, HMODULE* phModule )
  = GetModuleHandleExA;

__declspec(dllexport) BOOL WINAPI Mine_GetModuleHandleExA( DWORD dwFlags, LPCSTR lpModuleName, HMODULE* phModule ){
  if(ChessWrapperSentry::Wrap("GetModuleHandleExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetModuleHandleExA");
   }
  return Real_GetModuleHandleExA(dwFlags, lpModuleName, phModule);
}
BOOL (WINAPI * Real_GetModuleHandleExW)( DWORD dwFlags, LPCWSTR lpModuleName, HMODULE* phModule )
  = GetModuleHandleExW;

__declspec(dllexport) BOOL WINAPI Mine_GetModuleHandleExW( DWORD dwFlags, LPCWSTR lpModuleName, HMODULE* phModule ){
  if(ChessWrapperSentry::Wrap("GetModuleHandleExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetModuleHandleExW");
   }
  return Real_GetModuleHandleExW(dwFlags, lpModuleName, phModule);
}
HMODULE (WINAPI * Real_GetModuleHandleW)( LPCWSTR lpModuleName )
  = GetModuleHandleW;

__declspec(dllexport) HMODULE WINAPI Mine_GetModuleHandleW( LPCWSTR lpModuleName ){
  if(ChessWrapperSentry::Wrap("GetModuleHandleW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetModuleHandleW");
   }
  return Real_GetModuleHandleW(lpModuleName);
}
BOOL (WINAPI * Real_GetNLSVersion)( NLS_FUNCTION Function, LCID Locale, LPNLSVERSIONINFO lpVersionInformation)
  = GetNLSVersion;

__declspec(dllexport) BOOL WINAPI Mine_GetNLSVersion( NLS_FUNCTION Function, LCID Locale, LPNLSVERSIONINFO lpVersionInformation){
  if(ChessWrapperSentry::Wrap("GetNLSVersion")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNLSVersion");
   }
  return Real_GetNLSVersion(Function, Locale, lpVersionInformation);
}
BOOL (WINAPI * Real_GetNamedPipeHandleStateA)( HANDLE hNamedPipe, LPDWORD lpState, LPDWORD lpCurInstances, LPDWORD lpMaxCollectionCount, LPDWORD lpCollectDataTimeout, LPSTR lpUserName, DWORD nMaxUserNameSize )
  = GetNamedPipeHandleStateA;

__declspec(dllexport) BOOL WINAPI Mine_GetNamedPipeHandleStateA( HANDLE hNamedPipe, LPDWORD lpState, LPDWORD lpCurInstances, LPDWORD lpMaxCollectionCount, LPDWORD lpCollectDataTimeout, LPSTR lpUserName, DWORD nMaxUserNameSize ){
  if(ChessWrapperSentry::Wrap("GetNamedPipeHandleStateA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNamedPipeHandleStateA");
   }
  return Real_GetNamedPipeHandleStateA(hNamedPipe, lpState, lpCurInstances, lpMaxCollectionCount, lpCollectDataTimeout, lpUserName, nMaxUserNameSize);
}
BOOL (WINAPI * Real_GetNamedPipeHandleStateW)( HANDLE hNamedPipe, LPDWORD lpState, LPDWORD lpCurInstances, LPDWORD lpMaxCollectionCount, LPDWORD lpCollectDataTimeout, LPWSTR lpUserName, DWORD nMaxUserNameSize )
  = GetNamedPipeHandleStateW;

__declspec(dllexport) BOOL WINAPI Mine_GetNamedPipeHandleStateW( HANDLE hNamedPipe, LPDWORD lpState, LPDWORD lpCurInstances, LPDWORD lpMaxCollectionCount, LPDWORD lpCollectDataTimeout, LPWSTR lpUserName, DWORD nMaxUserNameSize ){
  if(ChessWrapperSentry::Wrap("GetNamedPipeHandleStateW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNamedPipeHandleStateW");
   }
  return Real_GetNamedPipeHandleStateW(hNamedPipe, lpState, lpCurInstances, lpMaxCollectionCount, lpCollectDataTimeout, lpUserName, nMaxUserNameSize);
}
BOOL (WINAPI * Real_GetNamedPipeInfo)( HANDLE hNamedPipe, LPDWORD lpFlags, LPDWORD lpOutBufferSize, LPDWORD lpInBufferSize, LPDWORD lpMaxInstances )
  = GetNamedPipeInfo;

__declspec(dllexport) BOOL WINAPI Mine_GetNamedPipeInfo( HANDLE hNamedPipe, LPDWORD lpFlags, LPDWORD lpOutBufferSize, LPDWORD lpInBufferSize, LPDWORD lpMaxInstances ){
  if(ChessWrapperSentry::Wrap("GetNamedPipeInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNamedPipeInfo");
   }
  return Real_GetNamedPipeInfo(hNamedPipe, lpFlags, lpOutBufferSize, lpInBufferSize, lpMaxInstances);
}
void (WINAPI * Real_GetNativeSystemInfo)( LPSYSTEM_INFO lpSystemInfo )
  = GetNativeSystemInfo;

__declspec(dllexport) void WINAPI Mine_GetNativeSystemInfo( LPSYSTEM_INFO lpSystemInfo ){
  if(ChessWrapperSentry::Wrap("GetNativeSystemInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNativeSystemInfo");
   }
  return Real_GetNativeSystemInfo(lpSystemInfo);
}
BOOL (WINAPI * Real_GetNumaAvailableMemoryNode)( UCHAR Node, PULONGLONG AvailableBytes )
  = GetNumaAvailableMemoryNode;

__declspec(dllexport) BOOL WINAPI Mine_GetNumaAvailableMemoryNode( UCHAR Node, PULONGLONG AvailableBytes ){
  if(ChessWrapperSentry::Wrap("GetNumaAvailableMemoryNode")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumaAvailableMemoryNode");
   }
  return Real_GetNumaAvailableMemoryNode(Node, AvailableBytes);
}
BOOL (WINAPI * Real_GetNumaHighestNodeNumber)( PULONG HighestNodeNumber )
  = GetNumaHighestNodeNumber;

__declspec(dllexport) BOOL WINAPI Mine_GetNumaHighestNodeNumber( PULONG HighestNodeNumber ){
  if(ChessWrapperSentry::Wrap("GetNumaHighestNodeNumber")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumaHighestNodeNumber");
   }
  return Real_GetNumaHighestNodeNumber(HighestNodeNumber);
}
BOOL (WINAPI * Real_GetNumaNodeProcessorMask)( UCHAR Node, PULONGLONG ProcessorMask )
  = GetNumaNodeProcessorMask;

__declspec(dllexport) BOOL WINAPI Mine_GetNumaNodeProcessorMask( UCHAR Node, PULONGLONG ProcessorMask ){
  if(ChessWrapperSentry::Wrap("GetNumaNodeProcessorMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumaNodeProcessorMask");
   }
  return Real_GetNumaNodeProcessorMask(Node, ProcessorMask);
}
BOOL (WINAPI * Real_GetNumaProcessorNode)( UCHAR Processor, PUCHAR NodeNumber )
  = GetNumaProcessorNode;

__declspec(dllexport) BOOL WINAPI Mine_GetNumaProcessorNode( UCHAR Processor, PUCHAR NodeNumber ){
  if(ChessWrapperSentry::Wrap("GetNumaProcessorNode")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumaProcessorNode");
   }
  return Real_GetNumaProcessorNode(Processor, NodeNumber);
}
int (WINAPI * Real_GetNumberFormatA)( LCID Locale, DWORD dwFlags, LPCSTR lpValue, const NUMBERFMTA *lpFormat, LPSTR lpNumberStr, int cchNumber)
  = GetNumberFormatA;

__declspec(dllexport) int WINAPI Mine_GetNumberFormatA( LCID Locale, DWORD dwFlags, LPCSTR lpValue, const NUMBERFMTA *lpFormat, LPSTR lpNumberStr, int cchNumber){
  if(ChessWrapperSentry::Wrap("GetNumberFormatA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumberFormatA");
   }
  return Real_GetNumberFormatA(Locale, dwFlags, lpValue, lpFormat, lpNumberStr, cchNumber);
}
int (WINAPI * Real_GetNumberFormatW)( LCID Locale, DWORD dwFlags, LPCWSTR lpValue, const NUMBERFMTW *lpFormat, LPWSTR lpNumberStr, int cchNumber)
  = GetNumberFormatW;

__declspec(dllexport) int WINAPI Mine_GetNumberFormatW( LCID Locale, DWORD dwFlags, LPCWSTR lpValue, const NUMBERFMTW *lpFormat, LPWSTR lpNumberStr, int cchNumber){
  if(ChessWrapperSentry::Wrap("GetNumberFormatW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumberFormatW");
   }
  return Real_GetNumberFormatW(Locale, dwFlags, lpValue, lpFormat, lpNumberStr, cchNumber);
}
BOOL (WINAPI * Real_GetNumberOfConsoleInputEvents)( HANDLE hConsoleInput, LPDWORD lpNumberOfEvents )
  = GetNumberOfConsoleInputEvents;

__declspec(dllexport) BOOL WINAPI Mine_GetNumberOfConsoleInputEvents( HANDLE hConsoleInput, LPDWORD lpNumberOfEvents ){
  if(ChessWrapperSentry::Wrap("GetNumberOfConsoleInputEvents")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumberOfConsoleInputEvents");
   }
  return Real_GetNumberOfConsoleInputEvents(hConsoleInput, lpNumberOfEvents);
}
BOOL (WINAPI * Real_GetNumberOfConsoleMouseButtons)( LPDWORD lpNumberOfMouseButtons )
  = GetNumberOfConsoleMouseButtons;

__declspec(dllexport) BOOL WINAPI Mine_GetNumberOfConsoleMouseButtons( LPDWORD lpNumberOfMouseButtons ){
  if(ChessWrapperSentry::Wrap("GetNumberOfConsoleMouseButtons")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetNumberOfConsoleMouseButtons");
   }
  return Real_GetNumberOfConsoleMouseButtons(lpNumberOfMouseButtons);
}
UINT (WINAPI * Real_GetOEMCP)(void)
  = GetOEMCP;

__declspec(dllexport) UINT WINAPI Mine_GetOEMCP(void){
  if(ChessWrapperSentry::Wrap("GetOEMCP")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetOEMCP");
   }
  return Real_GetOEMCP();
}
BOOL (WINAPI * Real_GetOverlappedResult)( HANDLE hFile, LPOVERLAPPED lpOverlapped, LPDWORD lpNumberOfBytesTransferred, BOOL bWait )
  = GetOverlappedResult;

__declspec(dllexport) BOOL WINAPI Mine_GetOverlappedResult( HANDLE hFile, LPOVERLAPPED lpOverlapped, LPDWORD lpNumberOfBytesTransferred, BOOL bWait ){
  if(ChessWrapperSentry::Wrap("GetOverlappedResult")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetOverlappedResult");
   }
  return Real_GetOverlappedResult(hFile, lpOverlapped, lpNumberOfBytesTransferred, bWait);
}
DWORD (WINAPI * Real_GetPriorityClass)( HANDLE hProcess )
  = GetPriorityClass;

__declspec(dllexport) DWORD WINAPI Mine_GetPriorityClass( HANDLE hProcess ){
  if(ChessWrapperSentry::Wrap("GetPriorityClass")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPriorityClass");
   }
  return Real_GetPriorityClass(hProcess);
}
UINT (WINAPI * Real_GetPrivateProfileIntA)( LPCSTR lpAppName, LPCSTR lpKeyName, INT nDefault, LPCSTR lpFileName )
  = GetPrivateProfileIntA;

__declspec(dllexport) UINT WINAPI Mine_GetPrivateProfileIntA( LPCSTR lpAppName, LPCSTR lpKeyName, INT nDefault, LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileIntA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileIntA");
   }
  return Real_GetPrivateProfileIntA(lpAppName, lpKeyName, nDefault, lpFileName);
}
UINT (WINAPI * Real_GetPrivateProfileIntW)( LPCWSTR lpAppName, LPCWSTR lpKeyName, INT nDefault, LPCWSTR lpFileName )
  = GetPrivateProfileIntW;

__declspec(dllexport) UINT WINAPI Mine_GetPrivateProfileIntW( LPCWSTR lpAppName, LPCWSTR lpKeyName, INT nDefault, LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileIntW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileIntW");
   }
  return Real_GetPrivateProfileIntW(lpAppName, lpKeyName, nDefault, lpFileName);
}
DWORD (WINAPI * Real_GetPrivateProfileSectionA)( LPCSTR lpAppName, LPSTR lpReturnedString, DWORD nSize, LPCSTR lpFileName )
  = GetPrivateProfileSectionA;

__declspec(dllexport) DWORD WINAPI Mine_GetPrivateProfileSectionA( LPCSTR lpAppName, LPSTR lpReturnedString, DWORD nSize, LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileSectionA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileSectionA");
   }
  return Real_GetPrivateProfileSectionA(lpAppName, lpReturnedString, nSize, lpFileName);
}
DWORD (WINAPI * Real_GetPrivateProfileSectionNamesA)( LPSTR lpszReturnBuffer, DWORD nSize, LPCSTR lpFileName )
  = GetPrivateProfileSectionNamesA;

__declspec(dllexport) DWORD WINAPI Mine_GetPrivateProfileSectionNamesA( LPSTR lpszReturnBuffer, DWORD nSize, LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileSectionNamesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileSectionNamesA");
   }
  return Real_GetPrivateProfileSectionNamesA(lpszReturnBuffer, nSize, lpFileName);
}
DWORD (WINAPI * Real_GetPrivateProfileSectionNamesW)( LPWSTR lpszReturnBuffer, DWORD nSize, LPCWSTR lpFileName )
  = GetPrivateProfileSectionNamesW;

__declspec(dllexport) DWORD WINAPI Mine_GetPrivateProfileSectionNamesW( LPWSTR lpszReturnBuffer, DWORD nSize, LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileSectionNamesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileSectionNamesW");
   }
  return Real_GetPrivateProfileSectionNamesW(lpszReturnBuffer, nSize, lpFileName);
}
DWORD (WINAPI * Real_GetPrivateProfileSectionW)( LPCWSTR lpAppName, LPWSTR lpReturnedString, DWORD nSize, LPCWSTR lpFileName )
  = GetPrivateProfileSectionW;

__declspec(dllexport) DWORD WINAPI Mine_GetPrivateProfileSectionW( LPCWSTR lpAppName, LPWSTR lpReturnedString, DWORD nSize, LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileSectionW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileSectionW");
   }
  return Real_GetPrivateProfileSectionW(lpAppName, lpReturnedString, nSize, lpFileName);
}
DWORD (WINAPI * Real_GetPrivateProfileStringA)( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpDefault, LPSTR lpReturnedString, DWORD nSize, LPCSTR lpFileName )
  = GetPrivateProfileStringA;

__declspec(dllexport) DWORD WINAPI Mine_GetPrivateProfileStringA( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpDefault, LPSTR lpReturnedString, DWORD nSize, LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileStringA");
   }
  return Real_GetPrivateProfileStringA(lpAppName, lpKeyName, lpDefault, lpReturnedString, nSize, lpFileName);
}
DWORD (WINAPI * Real_GetPrivateProfileStringW)( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpDefault, LPWSTR lpReturnedString, DWORD nSize, LPCWSTR lpFileName )
  = GetPrivateProfileStringW;

__declspec(dllexport) DWORD WINAPI Mine_GetPrivateProfileStringW( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpDefault, LPWSTR lpReturnedString, DWORD nSize, LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileStringW");
   }
  return Real_GetPrivateProfileStringW(lpAppName, lpKeyName, lpDefault, lpReturnedString, nSize, lpFileName);
}
BOOL (WINAPI * Real_GetPrivateProfileStructA)( LPCSTR lpszSection, LPCSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCSTR szFile )
  = GetPrivateProfileStructA;

__declspec(dllexport) BOOL WINAPI Mine_GetPrivateProfileStructA( LPCSTR lpszSection, LPCSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCSTR szFile ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileStructA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileStructA");
   }
  return Real_GetPrivateProfileStructA(lpszSection, lpszKey, lpStruct, uSizeStruct, szFile);
}
BOOL (WINAPI * Real_GetPrivateProfileStructW)( LPCWSTR lpszSection, LPCWSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCWSTR szFile )
  = GetPrivateProfileStructW;

__declspec(dllexport) BOOL WINAPI Mine_GetPrivateProfileStructW( LPCWSTR lpszSection, LPCWSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCWSTR szFile ){
  if(ChessWrapperSentry::Wrap("GetPrivateProfileStructW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetPrivateProfileStructW");
   }
  return Real_GetPrivateProfileStructW(lpszSection, lpszKey, lpStruct, uSizeStruct, szFile);
}
FARPROC (WINAPI * Real_GetProcAddress)( HMODULE hModule, LPCSTR lpProcName )
  = GetProcAddress;

__declspec(dllexport) FARPROC WINAPI Mine_GetProcAddress( HMODULE hModule, LPCSTR lpProcName ){
  if(ChessWrapperSentry::Wrap("GetProcAddress")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcAddress");
   }
  return Real_GetProcAddress(hModule, lpProcName);
}
BOOL (WINAPI * Real_GetProcessAffinityMask)( HANDLE hProcess, PDWORD_PTR lpProcessAffinityMask, PDWORD_PTR lpSystemAffinityMask )
  = GetProcessAffinityMask;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessAffinityMask( HANDLE hProcess, PDWORD_PTR lpProcessAffinityMask, PDWORD_PTR lpSystemAffinityMask ){
  if(ChessWrapperSentry::Wrap("GetProcessAffinityMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessAffinityMask");
   }
  return Real_GetProcessAffinityMask(hProcess, lpProcessAffinityMask, lpSystemAffinityMask);
}
BOOL (WINAPI * Real_GetProcessHandleCount)( HANDLE hProcess, PDWORD pdwHandleCount )
  = GetProcessHandleCount;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessHandleCount( HANDLE hProcess, PDWORD pdwHandleCount ){
  if(ChessWrapperSentry::Wrap("GetProcessHandleCount")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessHandleCount");
   }
  return Real_GetProcessHandleCount(hProcess, pdwHandleCount);
}
HANDLE (WINAPI * Real_GetProcessHeap)( void )
  = GetProcessHeap;

__declspec(dllexport) HANDLE WINAPI Mine_GetProcessHeap( void ){
  if(ChessWrapperSentry::Wrap("GetProcessHeap")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessHeap");
   }
  return Real_GetProcessHeap();
}
DWORD (WINAPI * Real_GetProcessHeaps)( DWORD NumberOfHeaps, PHANDLE ProcessHeaps )
  = GetProcessHeaps;

__declspec(dllexport) DWORD WINAPI Mine_GetProcessHeaps( DWORD NumberOfHeaps, PHANDLE ProcessHeaps ){
  if(ChessWrapperSentry::Wrap("GetProcessHeaps")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessHeaps");
   }
  return Real_GetProcessHeaps(NumberOfHeaps, ProcessHeaps);
}
DWORD (WINAPI * Real_GetProcessId)( HANDLE Process )
  = GetProcessId;

__declspec(dllexport) DWORD WINAPI Mine_GetProcessId( HANDLE Process ){
  if(ChessWrapperSentry::Wrap("GetProcessId")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessId");
   }
  return Real_GetProcessId(Process);
}
DWORD (WINAPI * Real_GetProcessIdOfThread)( HANDLE Thread )
  = GetProcessIdOfThread;

__declspec(dllexport) DWORD WINAPI Mine_GetProcessIdOfThread( HANDLE Thread ){
  if(ChessWrapperSentry::Wrap("GetProcessIdOfThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessIdOfThread");
   }
  return Real_GetProcessIdOfThread(Thread);
}
BOOL (WINAPI * Real_GetProcessIoCounters)( HANDLE hProcess, PIO_COUNTERS lpIoCounters )
  = GetProcessIoCounters;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessIoCounters( HANDLE hProcess, PIO_COUNTERS lpIoCounters ){
  if(ChessWrapperSentry::Wrap("GetProcessIoCounters")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessIoCounters");
   }
  return Real_GetProcessIoCounters(hProcess, lpIoCounters);
}
BOOL (WINAPI * Real_GetProcessPriorityBoost)( HANDLE hProcess, PBOOL pDisablePriorityBoost )
  = GetProcessPriorityBoost;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessPriorityBoost( HANDLE hProcess, PBOOL pDisablePriorityBoost ){
  if(ChessWrapperSentry::Wrap("GetProcessPriorityBoost")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessPriorityBoost");
   }
  return Real_GetProcessPriorityBoost(hProcess, pDisablePriorityBoost);
}
BOOL (WINAPI * Real_GetProcessShutdownParameters)( LPDWORD lpdwLevel, LPDWORD lpdwFlags )
  = GetProcessShutdownParameters;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessShutdownParameters( LPDWORD lpdwLevel, LPDWORD lpdwFlags ){
  if(ChessWrapperSentry::Wrap("GetProcessShutdownParameters")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessShutdownParameters");
   }
  return Real_GetProcessShutdownParameters(lpdwLevel, lpdwFlags);
}
BOOL (WINAPI * Real_GetProcessTimes)( HANDLE hProcess, LPFILETIME lpCreationTime, LPFILETIME lpExitTime, LPFILETIME lpKernelTime, LPFILETIME lpUserTime )
  = GetProcessTimes;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessTimes( HANDLE hProcess, LPFILETIME lpCreationTime, LPFILETIME lpExitTime, LPFILETIME lpKernelTime, LPFILETIME lpUserTime ){
  if(ChessWrapperSentry::Wrap("GetProcessTimes")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessTimes");
   }
  return Real_GetProcessTimes(hProcess, lpCreationTime, lpExitTime, lpKernelTime, lpUserTime);
}
DWORD (WINAPI * Real_GetProcessVersion)( DWORD ProcessId )
  = GetProcessVersion;

__declspec(dllexport) DWORD WINAPI Mine_GetProcessVersion( DWORD ProcessId ){
  if(ChessWrapperSentry::Wrap("GetProcessVersion")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessVersion");
   }
  return Real_GetProcessVersion(ProcessId);
}
BOOL (WINAPI * Real_GetProcessWorkingSetSize)( HANDLE hProcess, PSIZE_T lpMinimumWorkingSetSize, PSIZE_T lpMaximumWorkingSetSize )
  = GetProcessWorkingSetSize;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessWorkingSetSize( HANDLE hProcess, PSIZE_T lpMinimumWorkingSetSize, PSIZE_T lpMaximumWorkingSetSize ){
  if(ChessWrapperSentry::Wrap("GetProcessWorkingSetSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessWorkingSetSize");
   }
  return Real_GetProcessWorkingSetSize(hProcess, lpMinimumWorkingSetSize, lpMaximumWorkingSetSize);
}
BOOL (WINAPI * Real_GetProcessWorkingSetSizeEx)( HANDLE hProcess, PSIZE_T lpMinimumWorkingSetSize, PSIZE_T lpMaximumWorkingSetSize, PDWORD Flags )
  = GetProcessWorkingSetSizeEx;

__declspec(dllexport) BOOL WINAPI Mine_GetProcessWorkingSetSizeEx( HANDLE hProcess, PSIZE_T lpMinimumWorkingSetSize, PSIZE_T lpMaximumWorkingSetSize, PDWORD Flags ){
  if(ChessWrapperSentry::Wrap("GetProcessWorkingSetSizeEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProcessWorkingSetSizeEx");
   }
  return Real_GetProcessWorkingSetSizeEx(hProcess, lpMinimumWorkingSetSize, lpMaximumWorkingSetSize, Flags);
}
UINT (WINAPI * Real_GetProfileIntA)( LPCSTR lpAppName, LPCSTR lpKeyName, INT nDefault )
  = GetProfileIntA;

__declspec(dllexport) UINT WINAPI Mine_GetProfileIntA( LPCSTR lpAppName, LPCSTR lpKeyName, INT nDefault ){
  if(ChessWrapperSentry::Wrap("GetProfileIntA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProfileIntA");
   }
  return Real_GetProfileIntA(lpAppName, lpKeyName, nDefault);
}
UINT (WINAPI * Real_GetProfileIntW)( LPCWSTR lpAppName, LPCWSTR lpKeyName, INT nDefault )
  = GetProfileIntW;

__declspec(dllexport) UINT WINAPI Mine_GetProfileIntW( LPCWSTR lpAppName, LPCWSTR lpKeyName, INT nDefault ){
  if(ChessWrapperSentry::Wrap("GetProfileIntW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProfileIntW");
   }
  return Real_GetProfileIntW(lpAppName, lpKeyName, nDefault);
}
DWORD (WINAPI * Real_GetProfileSectionA)( LPCSTR lpAppName, LPSTR lpReturnedString, DWORD nSize )
  = GetProfileSectionA;

__declspec(dllexport) DWORD WINAPI Mine_GetProfileSectionA( LPCSTR lpAppName, LPSTR lpReturnedString, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetProfileSectionA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProfileSectionA");
   }
  return Real_GetProfileSectionA(lpAppName, lpReturnedString, nSize);
}
DWORD (WINAPI * Real_GetProfileSectionW)( LPCWSTR lpAppName, LPWSTR lpReturnedString, DWORD nSize )
  = GetProfileSectionW;

__declspec(dllexport) DWORD WINAPI Mine_GetProfileSectionW( LPCWSTR lpAppName, LPWSTR lpReturnedString, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetProfileSectionW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProfileSectionW");
   }
  return Real_GetProfileSectionW(lpAppName, lpReturnedString, nSize);
}
DWORD (WINAPI * Real_GetProfileStringA)( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpDefault, LPSTR lpReturnedString, DWORD nSize )
  = GetProfileStringA;

__declspec(dllexport) DWORD WINAPI Mine_GetProfileStringA( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpDefault, LPSTR lpReturnedString, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetProfileStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProfileStringA");
   }
  return Real_GetProfileStringA(lpAppName, lpKeyName, lpDefault, lpReturnedString, nSize);
}
DWORD (WINAPI * Real_GetProfileStringW)( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpDefault, LPWSTR lpReturnedString, DWORD nSize )
  = GetProfileStringW;

__declspec(dllexport) DWORD WINAPI Mine_GetProfileStringW( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpDefault, LPWSTR lpReturnedString, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("GetProfileStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetProfileStringW");
   }
  return Real_GetProfileStringW(lpAppName, lpKeyName, lpDefault, lpReturnedString, nSize);
}
BOOL (WINAPI * Real_GetQueuedCompletionStatus)( HANDLE CompletionPort, LPDWORD lpNumberOfBytesTransferred, PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds )
   = GetQueuedCompletionStatus;

__declspec(dllexport) BOOL WINAPI Mine_GetQueuedCompletionStatus( HANDLE CompletionPort, LPDWORD lpNumberOfBytesTransferred, PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds ){
#ifdef WRAP_GetQueuedCompletionStatus
  if(ChessWrapperSentry::Wrap("GetQueuedCompletionStatus")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetQueuedCompletionStatus");
     BOOL res = __wrapper_GetQueuedCompletionStatus(CompletionPort, lpNumberOfBytesTransferred, lpCompletionKey, lpOverlapped, dwMilliseconds);
     return res;
  }
#endif
  return Real_GetQueuedCompletionStatus(CompletionPort, lpNumberOfBytesTransferred, lpCompletionKey, lpOverlapped, dwMilliseconds);
}
DWORD (WINAPI * Real_GetShortPathNameA)( LPCSTR lpszLongPath, LPSTR lpszShortPath, DWORD cchBuffer )
  = GetShortPathNameA;

__declspec(dllexport) DWORD WINAPI Mine_GetShortPathNameA( LPCSTR lpszLongPath, LPSTR lpszShortPath, DWORD cchBuffer ){
  if(ChessWrapperSentry::Wrap("GetShortPathNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetShortPathNameA");
   }
  return Real_GetShortPathNameA(lpszLongPath, lpszShortPath, cchBuffer);
}
DWORD (WINAPI * Real_GetShortPathNameW)( LPCWSTR lpszLongPath, LPWSTR lpszShortPath, DWORD cchBuffer )
  = GetShortPathNameW;

__declspec(dllexport) DWORD WINAPI Mine_GetShortPathNameW( LPCWSTR lpszLongPath, LPWSTR lpszShortPath, DWORD cchBuffer ){
  if(ChessWrapperSentry::Wrap("GetShortPathNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetShortPathNameW");
   }
  return Real_GetShortPathNameW(lpszLongPath, lpszShortPath, cchBuffer);
}
void (WINAPI * Real_GetStartupInfoA)( LPSTARTUPINFOA lpStartupInfo )
  = GetStartupInfoA;

__declspec(dllexport) void WINAPI Mine_GetStartupInfoA( LPSTARTUPINFOA lpStartupInfo ){
  if(ChessWrapperSentry::Wrap("GetStartupInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStartupInfoA");
   }
  return Real_GetStartupInfoA(lpStartupInfo);
}
void (WINAPI * Real_GetStartupInfoW)( LPSTARTUPINFOW lpStartupInfo )
  = GetStartupInfoW;

__declspec(dllexport) void WINAPI Mine_GetStartupInfoW( LPSTARTUPINFOW lpStartupInfo ){
  if(ChessWrapperSentry::Wrap("GetStartupInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStartupInfoW");
   }
  return Real_GetStartupInfoW(lpStartupInfo);
}
HANDLE (WINAPI * Real_GetStdHandle)( DWORD nStdHandle )
  = GetStdHandle;

__declspec(dllexport) HANDLE WINAPI Mine_GetStdHandle( DWORD nStdHandle ){
  if(ChessWrapperSentry::Wrap("GetStdHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStdHandle");
   }
  return Real_GetStdHandle(nStdHandle);
}
BOOL (WINAPI * Real_GetStringTypeA)( LCID Locale, DWORD dwInfoType, LPCSTR lpSrcStr, int cchSrc, LPWORD lpCharType)
  = GetStringTypeA;

__declspec(dllexport) BOOL WINAPI Mine_GetStringTypeA( LCID Locale, DWORD dwInfoType, LPCSTR lpSrcStr, int cchSrc, LPWORD lpCharType){
  if(ChessWrapperSentry::Wrap("GetStringTypeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStringTypeA");
   }
  return Real_GetStringTypeA(Locale, dwInfoType, lpSrcStr, cchSrc, lpCharType);
}
BOOL (WINAPI * Real_GetStringTypeExA)( LCID Locale, DWORD dwInfoType, LPCSTR lpSrcStr, int cchSrc, LPWORD lpCharType)
  = GetStringTypeExA;

__declspec(dllexport) BOOL WINAPI Mine_GetStringTypeExA( LCID Locale, DWORD dwInfoType, LPCSTR lpSrcStr, int cchSrc, LPWORD lpCharType){
  if(ChessWrapperSentry::Wrap("GetStringTypeExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStringTypeExA");
   }
  return Real_GetStringTypeExA(Locale, dwInfoType, lpSrcStr, cchSrc, lpCharType);
}
BOOL (WINAPI * Real_GetStringTypeExW)( LCID Locale, DWORD dwInfoType, LPCWSTR lpSrcStr, int cchSrc, LPWORD lpCharType)
  = GetStringTypeExW;

__declspec(dllexport) BOOL WINAPI Mine_GetStringTypeExW( LCID Locale, DWORD dwInfoType, LPCWSTR lpSrcStr, int cchSrc, LPWORD lpCharType){
  if(ChessWrapperSentry::Wrap("GetStringTypeExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStringTypeExW");
   }
  return Real_GetStringTypeExW(Locale, dwInfoType, lpSrcStr, cchSrc, lpCharType);
}
BOOL (WINAPI * Real_GetStringTypeW)( DWORD dwInfoType, LPCWSTR lpSrcStr, int cchSrc, LPWORD lpCharType)
  = GetStringTypeW;

__declspec(dllexport) BOOL WINAPI Mine_GetStringTypeW( DWORD dwInfoType, LPCWSTR lpSrcStr, int cchSrc, LPWORD lpCharType){
  if(ChessWrapperSentry::Wrap("GetStringTypeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetStringTypeW");
   }
  return Real_GetStringTypeW(dwInfoType, lpSrcStr, cchSrc, lpCharType);
}
LCID (WINAPI * Real_GetSystemDefaultLCID)(void)
  = GetSystemDefaultLCID;

__declspec(dllexport) LCID WINAPI Mine_GetSystemDefaultLCID(void){
  if(ChessWrapperSentry::Wrap("GetSystemDefaultLCID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemDefaultLCID");
   }
  return Real_GetSystemDefaultLCID();
}
LANGID (WINAPI * Real_GetSystemDefaultLangID)(void)
  = GetSystemDefaultLangID;

__declspec(dllexport) LANGID WINAPI Mine_GetSystemDefaultLangID(void){
  if(ChessWrapperSentry::Wrap("GetSystemDefaultLangID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemDefaultLangID");
   }
  return Real_GetSystemDefaultLangID();
}
LANGID (WINAPI * Real_GetSystemDefaultUILanguage)(void)
  = GetSystemDefaultUILanguage;

__declspec(dllexport) LANGID WINAPI Mine_GetSystemDefaultUILanguage(void){
  if(ChessWrapperSentry::Wrap("GetSystemDefaultUILanguage")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemDefaultUILanguage");
   }
  return Real_GetSystemDefaultUILanguage();
}
UINT (WINAPI * Real_GetSystemDirectoryA)( LPSTR lpBuffer, UINT uSize )
  = GetSystemDirectoryA;

__declspec(dllexport) UINT WINAPI Mine_GetSystemDirectoryA( LPSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetSystemDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemDirectoryA");
   }
  return Real_GetSystemDirectoryA(lpBuffer, uSize);
}
UINT (WINAPI * Real_GetSystemDirectoryW)( LPWSTR lpBuffer, UINT uSize )
  = GetSystemDirectoryW;

__declspec(dllexport) UINT WINAPI Mine_GetSystemDirectoryW( LPWSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetSystemDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemDirectoryW");
   }
  return Real_GetSystemDirectoryW(lpBuffer, uSize);
}
BOOL (WINAPI * Real_GetSystemFileCacheSize)( PSIZE_T lpMinimumFileCacheSize, PSIZE_T lpMaximumFileCacheSize, PDWORD lpFlags )
  = GetSystemFileCacheSize;

__declspec(dllexport) BOOL WINAPI Mine_GetSystemFileCacheSize( PSIZE_T lpMinimumFileCacheSize, PSIZE_T lpMaximumFileCacheSize, PDWORD lpFlags ){
  if(ChessWrapperSentry::Wrap("GetSystemFileCacheSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemFileCacheSize");
   }
  return Real_GetSystemFileCacheSize(lpMinimumFileCacheSize, lpMaximumFileCacheSize, lpFlags);
}
UINT (WINAPI * Real_GetSystemFirmwareTable)( DWORD FirmwareTableProviderSignature, DWORD FirmwareTableID, PVOID pFirmwareTableBuffer, DWORD BufferSize )
  = GetSystemFirmwareTable;

__declspec(dllexport) UINT WINAPI Mine_GetSystemFirmwareTable( DWORD FirmwareTableProviderSignature, DWORD FirmwareTableID, PVOID pFirmwareTableBuffer, DWORD BufferSize ){
  if(ChessWrapperSentry::Wrap("GetSystemFirmwareTable")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemFirmwareTable");
   }
  return Real_GetSystemFirmwareTable(FirmwareTableProviderSignature, FirmwareTableID, pFirmwareTableBuffer, BufferSize);
}
void (WINAPI * Real_GetSystemInfo)( LPSYSTEM_INFO lpSystemInfo )
  = GetSystemInfo;

__declspec(dllexport) void WINAPI Mine_GetSystemInfo( LPSYSTEM_INFO lpSystemInfo ){
  if(ChessWrapperSentry::Wrap("GetSystemInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemInfo");
   }
  return Real_GetSystemInfo(lpSystemInfo);
}
BOOL (WINAPI * Real_GetSystemRegistryQuota)( PDWORD pdwQuotaAllowed, PDWORD pdwQuotaUsed )
  = GetSystemRegistryQuota;

__declspec(dllexport) BOOL WINAPI Mine_GetSystemRegistryQuota( PDWORD pdwQuotaAllowed, PDWORD pdwQuotaUsed ){
  if(ChessWrapperSentry::Wrap("GetSystemRegistryQuota")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemRegistryQuota");
   }
  return Real_GetSystemRegistryQuota(pdwQuotaAllowed, pdwQuotaUsed);
}
void (WINAPI * Real_GetSystemTime)( LPSYSTEMTIME lpSystemTime )
  = GetSystemTime;

__declspec(dllexport) void WINAPI Mine_GetSystemTime( LPSYSTEMTIME lpSystemTime ){
  if(ChessWrapperSentry::Wrap("GetSystemTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemTime");
   }
  return Real_GetSystemTime(lpSystemTime);
}
BOOL (WINAPI * Real_GetSystemTimeAdjustment)( PDWORD lpTimeAdjustment, PDWORD lpTimeIncrement, PBOOL lpTimeAdjustmentDisabled )
  = GetSystemTimeAdjustment;

__declspec(dllexport) BOOL WINAPI Mine_GetSystemTimeAdjustment( PDWORD lpTimeAdjustment, PDWORD lpTimeIncrement, PBOOL lpTimeAdjustmentDisabled ){
  if(ChessWrapperSentry::Wrap("GetSystemTimeAdjustment")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemTimeAdjustment");
   }
  return Real_GetSystemTimeAdjustment(lpTimeAdjustment, lpTimeIncrement, lpTimeAdjustmentDisabled);
}
void (WINAPI * Real_GetSystemTimeAsFileTime)( LPFILETIME lpSystemTimeAsFileTime )
  = GetSystemTimeAsFileTime;

__declspec(dllexport) void WINAPI Mine_GetSystemTimeAsFileTime( LPFILETIME lpSystemTimeAsFileTime ){
  if(ChessWrapperSentry::Wrap("GetSystemTimeAsFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemTimeAsFileTime");
   }
  return Real_GetSystemTimeAsFileTime(lpSystemTimeAsFileTime);
}
UINT (WINAPI * Real_GetSystemWindowsDirectoryA)( LPSTR lpBuffer, UINT uSize )
  = GetSystemWindowsDirectoryA;

__declspec(dllexport) UINT WINAPI Mine_GetSystemWindowsDirectoryA( LPSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetSystemWindowsDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemWindowsDirectoryA");
   }
  return Real_GetSystemWindowsDirectoryA(lpBuffer, uSize);
}
UINT (WINAPI * Real_GetSystemWindowsDirectoryW)( LPWSTR lpBuffer, UINT uSize )
  = GetSystemWindowsDirectoryW;

__declspec(dllexport) UINT WINAPI Mine_GetSystemWindowsDirectoryW( LPWSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetSystemWindowsDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemWindowsDirectoryW");
   }
  return Real_GetSystemWindowsDirectoryW(lpBuffer, uSize);
}
UINT (WINAPI * Real_GetSystemWow64DirectoryA)( LPSTR lpBuffer, UINT uSize )
  = GetSystemWow64DirectoryA;

__declspec(dllexport) UINT WINAPI Mine_GetSystemWow64DirectoryA( LPSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetSystemWow64DirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemWow64DirectoryA");
   }
  return Real_GetSystemWow64DirectoryA(lpBuffer, uSize);
}
UINT (WINAPI * Real_GetSystemWow64DirectoryW)( LPWSTR lpBuffer, UINT uSize )
  = GetSystemWow64DirectoryW;

__declspec(dllexport) UINT WINAPI Mine_GetSystemWow64DirectoryW( LPWSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetSystemWow64DirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetSystemWow64DirectoryW");
   }
  return Real_GetSystemWow64DirectoryW(lpBuffer, uSize);
}
DWORD (WINAPI * Real_GetTapeParameters)( HANDLE hDevice, DWORD dwOperation, LPDWORD lpdwSize, LPVOID lpTapeInformation )
  = GetTapeParameters;

__declspec(dllexport) DWORD WINAPI Mine_GetTapeParameters( HANDLE hDevice, DWORD dwOperation, LPDWORD lpdwSize, LPVOID lpTapeInformation ){
  if(ChessWrapperSentry::Wrap("GetTapeParameters")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTapeParameters");
   }
  return Real_GetTapeParameters(hDevice, dwOperation, lpdwSize, lpTapeInformation);
}
DWORD (WINAPI * Real_GetTapePosition)( HANDLE hDevice, DWORD dwPositionType, LPDWORD lpdwPartition, LPDWORD lpdwOffsetLow, LPDWORD lpdwOffsetHigh )
  = GetTapePosition;

__declspec(dllexport) DWORD WINAPI Mine_GetTapePosition( HANDLE hDevice, DWORD dwPositionType, LPDWORD lpdwPartition, LPDWORD lpdwOffsetLow, LPDWORD lpdwOffsetHigh ){
  if(ChessWrapperSentry::Wrap("GetTapePosition")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTapePosition");
   }
  return Real_GetTapePosition(hDevice, dwPositionType, lpdwPartition, lpdwOffsetLow, lpdwOffsetHigh);
}
DWORD (WINAPI * Real_GetTapeStatus)( HANDLE hDevice )
  = GetTapeStatus;

__declspec(dllexport) DWORD WINAPI Mine_GetTapeStatus( HANDLE hDevice ){
  if(ChessWrapperSentry::Wrap("GetTapeStatus")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTapeStatus");
   }
  return Real_GetTapeStatus(hDevice);
}
UINT (WINAPI * Real_GetTempFileNameA)( LPCSTR lpPathName, LPCSTR lpPrefixString, UINT uUnique, LPSTR lpTempFileName )
  = GetTempFileNameA;

__declspec(dllexport) UINT WINAPI Mine_GetTempFileNameA( LPCSTR lpPathName, LPCSTR lpPrefixString, UINT uUnique, LPSTR lpTempFileName ){
  if(ChessWrapperSentry::Wrap("GetTempFileNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTempFileNameA");
   }
  return Real_GetTempFileNameA(lpPathName, lpPrefixString, uUnique, lpTempFileName);
}
UINT (WINAPI * Real_GetTempFileNameW)( LPCWSTR lpPathName, LPCWSTR lpPrefixString, UINT uUnique, LPWSTR lpTempFileName )
  = GetTempFileNameW;

__declspec(dllexport) UINT WINAPI Mine_GetTempFileNameW( LPCWSTR lpPathName, LPCWSTR lpPrefixString, UINT uUnique, LPWSTR lpTempFileName ){
  if(ChessWrapperSentry::Wrap("GetTempFileNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTempFileNameW");
   }
  return Real_GetTempFileNameW(lpPathName, lpPrefixString, uUnique, lpTempFileName);
}
DWORD (WINAPI * Real_GetTempPathA)( DWORD nBufferLength, LPSTR lpBuffer )
  = GetTempPathA;

__declspec(dllexport) DWORD WINAPI Mine_GetTempPathA( DWORD nBufferLength, LPSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetTempPathA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTempPathA");
   }
  return Real_GetTempPathA(nBufferLength, lpBuffer);
}
DWORD (WINAPI * Real_GetTempPathW)( DWORD nBufferLength, LPWSTR lpBuffer )
  = GetTempPathW;

__declspec(dllexport) DWORD WINAPI Mine_GetTempPathW( DWORD nBufferLength, LPWSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("GetTempPathW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTempPathW");
   }
  return Real_GetTempPathW(nBufferLength, lpBuffer);
}
BOOL (WINAPI * Real_GetThreadContext)( HANDLE hThread, LPCONTEXT lpContext )
  = GetThreadContext;

__declspec(dllexport) BOOL WINAPI Mine_GetThreadContext( HANDLE hThread, LPCONTEXT lpContext ){
  if(ChessWrapperSentry::Wrap("GetThreadContext")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadContext");
   }
  return Real_GetThreadContext(hThread, lpContext);
}
BOOL (WINAPI * Real_GetThreadIOPendingFlag)( HANDLE hThread, PBOOL lpIOIsPending )
  = GetThreadIOPendingFlag;

__declspec(dllexport) BOOL WINAPI Mine_GetThreadIOPendingFlag( HANDLE hThread, PBOOL lpIOIsPending ){
  if(ChessWrapperSentry::Wrap("GetThreadIOPendingFlag")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadIOPendingFlag");
   }
  return Real_GetThreadIOPendingFlag(hThread, lpIOIsPending);
}
DWORD (WINAPI * Real_GetThreadId)( HANDLE Thread )
  = GetThreadId;

__declspec(dllexport) DWORD WINAPI Mine_GetThreadId( HANDLE Thread ){
  if(ChessWrapperSentry::Wrap("GetThreadId")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadId");
   }
  return Real_GetThreadId(Thread);
}
LCID (WINAPI * Real_GetThreadLocale)(void)
  = GetThreadLocale;

__declspec(dllexport) LCID WINAPI Mine_GetThreadLocale(void){
  if(ChessWrapperSentry::Wrap("GetThreadLocale")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadLocale");
   }
  return Real_GetThreadLocale();
}
int (WINAPI * Real_GetThreadPriority)( HANDLE hThread )
  = GetThreadPriority;

__declspec(dllexport) int WINAPI Mine_GetThreadPriority( HANDLE hThread ){
  if(ChessWrapperSentry::Wrap("GetThreadPriority")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadPriority");
   }
  return Real_GetThreadPriority(hThread);
}
BOOL (WINAPI * Real_GetThreadPriorityBoost)( HANDLE hThread, PBOOL pDisablePriorityBoost )
  = GetThreadPriorityBoost;

__declspec(dllexport) BOOL WINAPI Mine_GetThreadPriorityBoost( HANDLE hThread, PBOOL pDisablePriorityBoost ){
  if(ChessWrapperSentry::Wrap("GetThreadPriorityBoost")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadPriorityBoost");
   }
  return Real_GetThreadPriorityBoost(hThread, pDisablePriorityBoost);
}
BOOL (WINAPI * Real_GetThreadSelectorEntry)( HANDLE hThread, DWORD dwSelector, LPLDT_ENTRY lpSelectorEntry )
  = GetThreadSelectorEntry;

__declspec(dllexport) BOOL WINAPI Mine_GetThreadSelectorEntry( HANDLE hThread, DWORD dwSelector, LPLDT_ENTRY lpSelectorEntry ){
  if(ChessWrapperSentry::Wrap("GetThreadSelectorEntry")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadSelectorEntry");
   }
  return Real_GetThreadSelectorEntry(hThread, dwSelector, lpSelectorEntry);
}
BOOL (WINAPI * Real_GetThreadTimes)( HANDLE hThread, LPFILETIME lpCreationTime, LPFILETIME lpExitTime, LPFILETIME lpKernelTime, LPFILETIME lpUserTime )
  = GetThreadTimes;

__declspec(dllexport) BOOL WINAPI Mine_GetThreadTimes( HANDLE hThread, LPFILETIME lpCreationTime, LPFILETIME lpExitTime, LPFILETIME lpKernelTime, LPFILETIME lpUserTime ){
  if(ChessWrapperSentry::Wrap("GetThreadTimes")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetThreadTimes");
   }
  return Real_GetThreadTimes(hThread, lpCreationTime, lpExitTime, lpKernelTime, lpUserTime);
}
DWORD (WINAPI * Real_GetTickCount)( void )
  = GetTickCount;

__declspec(dllexport) DWORD WINAPI Mine_GetTickCount( void ){
  if(ChessWrapperSentry::Wrap("GetTickCount")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTickCount");
   }
  return Real_GetTickCount();
}
int (WINAPI * Real_GetTimeFormatA)( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpTime, LPCSTR lpFormat, LPSTR lpTimeStr, int cchTime)
  = GetTimeFormatA;

__declspec(dllexport) int WINAPI Mine_GetTimeFormatA( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpTime, LPCSTR lpFormat, LPSTR lpTimeStr, int cchTime){
  if(ChessWrapperSentry::Wrap("GetTimeFormatA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTimeFormatA");
   }
  return Real_GetTimeFormatA(Locale, dwFlags, lpTime, lpFormat, lpTimeStr, cchTime);
}
int (WINAPI * Real_GetTimeFormatW)( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpTime, LPCWSTR lpFormat, LPWSTR lpTimeStr, int cchTime)
  = GetTimeFormatW;

__declspec(dllexport) int WINAPI Mine_GetTimeFormatW( LCID Locale, DWORD dwFlags, const SYSTEMTIME *lpTime, LPCWSTR lpFormat, LPWSTR lpTimeStr, int cchTime){
  if(ChessWrapperSentry::Wrap("GetTimeFormatW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTimeFormatW");
   }
  return Real_GetTimeFormatW(Locale, dwFlags, lpTime, lpFormat, lpTimeStr, cchTime);
}
DWORD (WINAPI * Real_GetTimeZoneInformation)( LPTIME_ZONE_INFORMATION lpTimeZoneInformation )
  = GetTimeZoneInformation;

__declspec(dllexport) DWORD WINAPI Mine_GetTimeZoneInformation( LPTIME_ZONE_INFORMATION lpTimeZoneInformation ){
  if(ChessWrapperSentry::Wrap("GetTimeZoneInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetTimeZoneInformation");
   }
  return Real_GetTimeZoneInformation(lpTimeZoneInformation);
}
LCID (WINAPI * Real_GetUserDefaultLCID)(void)
  = GetUserDefaultLCID;

__declspec(dllexport) LCID WINAPI Mine_GetUserDefaultLCID(void){
  if(ChessWrapperSentry::Wrap("GetUserDefaultLCID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetUserDefaultLCID");
   }
  return Real_GetUserDefaultLCID();
}
LANGID (WINAPI * Real_GetUserDefaultLangID)(void)
  = GetUserDefaultLangID;

__declspec(dllexport) LANGID WINAPI Mine_GetUserDefaultLangID(void){
  if(ChessWrapperSentry::Wrap("GetUserDefaultLangID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetUserDefaultLangID");
   }
  return Real_GetUserDefaultLangID();
}
LANGID (WINAPI * Real_GetUserDefaultUILanguage)(void)
  = GetUserDefaultUILanguage;

__declspec(dllexport) LANGID WINAPI Mine_GetUserDefaultUILanguage(void){
  if(ChessWrapperSentry::Wrap("GetUserDefaultUILanguage")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetUserDefaultUILanguage");
   }
  return Real_GetUserDefaultUILanguage();
}
GEOID (WINAPI * Real_GetUserGeoID)( GEOCLASS GeoClass)
  = GetUserGeoID;

__declspec(dllexport) GEOID WINAPI Mine_GetUserGeoID( GEOCLASS GeoClass){
  if(ChessWrapperSentry::Wrap("GetUserGeoID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetUserGeoID");
   }
  return Real_GetUserGeoID(GeoClass);
}
DWORD (WINAPI * Real_GetVersion)( void )
  = GetVersion;

__declspec(dllexport) DWORD WINAPI Mine_GetVersion( void ){
  if(ChessWrapperSentry::Wrap("GetVersion")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVersion");
   }
  return Real_GetVersion();
}
BOOL (WINAPI * Real_GetVersionExA)( LPOSVERSIONINFOA lpVersionInformation )
  = GetVersionExA;

__declspec(dllexport) BOOL WINAPI Mine_GetVersionExA( LPOSVERSIONINFOA lpVersionInformation ){
  if(ChessWrapperSentry::Wrap("GetVersionExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVersionExA");
   }
  return Real_GetVersionExA(lpVersionInformation);
}
BOOL (WINAPI * Real_GetVersionExW)( LPOSVERSIONINFOW lpVersionInformation )
  = GetVersionExW;

__declspec(dllexport) BOOL WINAPI Mine_GetVersionExW( LPOSVERSIONINFOW lpVersionInformation ){
  if(ChessWrapperSentry::Wrap("GetVersionExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVersionExW");
   }
  return Real_GetVersionExW(lpVersionInformation);
}
BOOL (WINAPI * Real_GetVolumeInformationA)( LPCSTR lpRootPathName, LPSTR lpVolumeNameBuffer, DWORD nVolumeNameSize, LPDWORD lpVolumeSerialNumber, LPDWORD lpMaximumComponentLength, LPDWORD lpFileSystemFlags, LPSTR lpFileSystemNameBuffer, DWORD nFileSystemNameSize )
  = GetVolumeInformationA;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumeInformationA( LPCSTR lpRootPathName, LPSTR lpVolumeNameBuffer, DWORD nVolumeNameSize, LPDWORD lpVolumeSerialNumber, LPDWORD lpMaximumComponentLength, LPDWORD lpFileSystemFlags, LPSTR lpFileSystemNameBuffer, DWORD nFileSystemNameSize ){
  if(ChessWrapperSentry::Wrap("GetVolumeInformationA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumeInformationA");
   }
  return Real_GetVolumeInformationA(lpRootPathName, lpVolumeNameBuffer, nVolumeNameSize, lpVolumeSerialNumber, lpMaximumComponentLength, lpFileSystemFlags, lpFileSystemNameBuffer, nFileSystemNameSize);
}
BOOL (WINAPI * Real_GetVolumeInformationW)( LPCWSTR lpRootPathName, LPWSTR lpVolumeNameBuffer, DWORD nVolumeNameSize, LPDWORD lpVolumeSerialNumber, LPDWORD lpMaximumComponentLength, LPDWORD lpFileSystemFlags, LPWSTR lpFileSystemNameBuffer, DWORD nFileSystemNameSize )
  = GetVolumeInformationW;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumeInformationW( LPCWSTR lpRootPathName, LPWSTR lpVolumeNameBuffer, DWORD nVolumeNameSize, LPDWORD lpVolumeSerialNumber, LPDWORD lpMaximumComponentLength, LPDWORD lpFileSystemFlags, LPWSTR lpFileSystemNameBuffer, DWORD nFileSystemNameSize ){
  if(ChessWrapperSentry::Wrap("GetVolumeInformationW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumeInformationW");
   }
  return Real_GetVolumeInformationW(lpRootPathName, lpVolumeNameBuffer, nVolumeNameSize, lpVolumeSerialNumber, lpMaximumComponentLength, lpFileSystemFlags, lpFileSystemNameBuffer, nFileSystemNameSize);
}
BOOL (WINAPI * Real_GetVolumeNameForVolumeMountPointA)( LPCSTR lpszVolumeMountPoint, LPSTR lpszVolumeName, DWORD cchBufferLength )
  = GetVolumeNameForVolumeMountPointA;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumeNameForVolumeMountPointA( LPCSTR lpszVolumeMountPoint, LPSTR lpszVolumeName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("GetVolumeNameForVolumeMountPointA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumeNameForVolumeMountPointA");
   }
  return Real_GetVolumeNameForVolumeMountPointA(lpszVolumeMountPoint, lpszVolumeName, cchBufferLength);
}
BOOL (WINAPI * Real_GetVolumeNameForVolumeMountPointW)( LPCWSTR lpszVolumeMountPoint, LPWSTR lpszVolumeName, DWORD cchBufferLength )
  = GetVolumeNameForVolumeMountPointW;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumeNameForVolumeMountPointW( LPCWSTR lpszVolumeMountPoint, LPWSTR lpszVolumeName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("GetVolumeNameForVolumeMountPointW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumeNameForVolumeMountPointW");
   }
  return Real_GetVolumeNameForVolumeMountPointW(lpszVolumeMountPoint, lpszVolumeName, cchBufferLength);
}
BOOL (WINAPI * Real_GetVolumePathNameA)( LPCSTR lpszFileName, LPSTR lpszVolumePathName, DWORD cchBufferLength )
  = GetVolumePathNameA;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumePathNameA( LPCSTR lpszFileName, LPSTR lpszVolumePathName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("GetVolumePathNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumePathNameA");
   }
  return Real_GetVolumePathNameA(lpszFileName, lpszVolumePathName, cchBufferLength);
}
BOOL (WINAPI * Real_GetVolumePathNameW)( LPCWSTR lpszFileName, LPWSTR lpszVolumePathName, DWORD cchBufferLength )
  = GetVolumePathNameW;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumePathNameW( LPCWSTR lpszFileName, LPWSTR lpszVolumePathName, DWORD cchBufferLength ){
  if(ChessWrapperSentry::Wrap("GetVolumePathNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumePathNameW");
   }
  return Real_GetVolumePathNameW(lpszFileName, lpszVolumePathName, cchBufferLength);
}
BOOL (WINAPI * Real_GetVolumePathNamesForVolumeNameA)( LPCSTR lpszVolumeName, LPCH lpszVolumePathNames, DWORD cchBufferLength, PDWORD lpcchReturnLength )
  = GetVolumePathNamesForVolumeNameA;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumePathNamesForVolumeNameA( LPCSTR lpszVolumeName, LPCH lpszVolumePathNames, DWORD cchBufferLength, PDWORD lpcchReturnLength ){
  if(ChessWrapperSentry::Wrap("GetVolumePathNamesForVolumeNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumePathNamesForVolumeNameA");
   }
  return Real_GetVolumePathNamesForVolumeNameA(lpszVolumeName, lpszVolumePathNames, cchBufferLength, lpcchReturnLength);
}
BOOL (WINAPI * Real_GetVolumePathNamesForVolumeNameW)( LPCWSTR lpszVolumeName, LPWCH lpszVolumePathNames, DWORD cchBufferLength, PDWORD lpcchReturnLength )
  = GetVolumePathNamesForVolumeNameW;

__declspec(dllexport) BOOL WINAPI Mine_GetVolumePathNamesForVolumeNameW( LPCWSTR lpszVolumeName, LPWCH lpszVolumePathNames, DWORD cchBufferLength, PDWORD lpcchReturnLength ){
  if(ChessWrapperSentry::Wrap("GetVolumePathNamesForVolumeNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetVolumePathNamesForVolumeNameW");
   }
  return Real_GetVolumePathNamesForVolumeNameW(lpszVolumeName, lpszVolumePathNames, cchBufferLength, lpcchReturnLength);
}
UINT (WINAPI * Real_GetWindowsDirectoryA)( LPSTR lpBuffer, UINT uSize )
  = GetWindowsDirectoryA;

__declspec(dllexport) UINT WINAPI Mine_GetWindowsDirectoryA( LPSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetWindowsDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetWindowsDirectoryA");
   }
  return Real_GetWindowsDirectoryA(lpBuffer, uSize);
}
UINT (WINAPI * Real_GetWindowsDirectoryW)( LPWSTR lpBuffer, UINT uSize )
  = GetWindowsDirectoryW;

__declspec(dllexport) UINT WINAPI Mine_GetWindowsDirectoryW( LPWSTR lpBuffer, UINT uSize ){
  if(ChessWrapperSentry::Wrap("GetWindowsDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetWindowsDirectoryW");
   }
  return Real_GetWindowsDirectoryW(lpBuffer, uSize);
}
UINT (WINAPI * Real_GetWriteWatch)( DWORD dwFlags, PVOID lpBaseAddress, SIZE_T dwRegionSize, PVOID *lpAddresses, ULONG_PTR *lpdwCount, PULONG lpdwGranularity )
  = GetWriteWatch;

__declspec(dllexport) UINT WINAPI Mine_GetWriteWatch( DWORD dwFlags, PVOID lpBaseAddress, SIZE_T dwRegionSize, PVOID *lpAddresses, ULONG_PTR *lpdwCount, PULONG lpdwGranularity ){
  if(ChessWrapperSentry::Wrap("GetWriteWatch")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GetWriteWatch");
   }
  return Real_GetWriteWatch(dwFlags, lpBaseAddress, dwRegionSize, lpAddresses, lpdwCount, lpdwGranularity);
}
ATOM (WINAPI * Real_GlobalAddAtomA)( LPCSTR lpString )
  = GlobalAddAtomA;

__declspec(dllexport) ATOM WINAPI Mine_GlobalAddAtomA( LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("GlobalAddAtomA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalAddAtomA");
   }
  return Real_GlobalAddAtomA(lpString);
}
ATOM (WINAPI * Real_GlobalAddAtomW)( LPCWSTR lpString )
  = GlobalAddAtomW;

__declspec(dllexport) ATOM WINAPI Mine_GlobalAddAtomW( LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("GlobalAddAtomW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalAddAtomW");
   }
  return Real_GlobalAddAtomW(lpString);
}
HGLOBAL (WINAPI * Real_GlobalAlloc)( UINT uFlags, SIZE_T dwBytes )
  = GlobalAlloc;

__declspec(dllexport) HGLOBAL WINAPI Mine_GlobalAlloc( UINT uFlags, SIZE_T dwBytes ){
  if(ChessWrapperSentry::Wrap("GlobalAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalAlloc");
   }
  return Real_GlobalAlloc(uFlags, dwBytes);
}
SIZE_T (WINAPI * Real_GlobalCompact)( DWORD dwMinFree )
  = GlobalCompact;

__declspec(dllexport) SIZE_T WINAPI Mine_GlobalCompact( DWORD dwMinFree ){
  if(ChessWrapperSentry::Wrap("GlobalCompact")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalCompact");
   }
  return Real_GlobalCompact(dwMinFree);
}
ATOM (WINAPI * Real_GlobalDeleteAtom)( ATOM nAtom )
  = GlobalDeleteAtom;

__declspec(dllexport) ATOM WINAPI Mine_GlobalDeleteAtom( ATOM nAtom ){
  if(ChessWrapperSentry::Wrap("GlobalDeleteAtom")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalDeleteAtom");
   }
  return Real_GlobalDeleteAtom(nAtom);
}
ATOM (WINAPI * Real_GlobalFindAtomA)( LPCSTR lpString )
  = GlobalFindAtomA;

__declspec(dllexport) ATOM WINAPI Mine_GlobalFindAtomA( LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("GlobalFindAtomA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalFindAtomA");
   }
  return Real_GlobalFindAtomA(lpString);
}
ATOM (WINAPI * Real_GlobalFindAtomW)( LPCWSTR lpString )
  = GlobalFindAtomW;

__declspec(dllexport) ATOM WINAPI Mine_GlobalFindAtomW( LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("GlobalFindAtomW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalFindAtomW");
   }
  return Real_GlobalFindAtomW(lpString);
}
void (WINAPI * Real_GlobalFix)( HGLOBAL hMem )
  = GlobalFix;

__declspec(dllexport) void WINAPI Mine_GlobalFix( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalFix")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalFix");
   }
  return Real_GlobalFix(hMem);
}
UINT (WINAPI * Real_GlobalFlags)( HGLOBAL hMem )
  = GlobalFlags;

__declspec(dllexport) UINT WINAPI Mine_GlobalFlags( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalFlags")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalFlags");
   }
  return Real_GlobalFlags(hMem);
}
HGLOBAL (WINAPI * Real_GlobalFree)( HGLOBAL hMem )
  = GlobalFree;

__declspec(dllexport) HGLOBAL WINAPI Mine_GlobalFree( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalFree")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalFree");
   }
  return Real_GlobalFree(hMem);
}
UINT (WINAPI * Real_GlobalGetAtomNameA)( ATOM nAtom, LPSTR lpBuffer, int nSize )
  = GlobalGetAtomNameA;

__declspec(dllexport) UINT WINAPI Mine_GlobalGetAtomNameA( ATOM nAtom, LPSTR lpBuffer, int nSize ){
  if(ChessWrapperSentry::Wrap("GlobalGetAtomNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalGetAtomNameA");
   }
  return Real_GlobalGetAtomNameA(nAtom, lpBuffer, nSize);
}
UINT (WINAPI * Real_GlobalGetAtomNameW)( ATOM nAtom, LPWSTR lpBuffer, int nSize )
  = GlobalGetAtomNameW;

__declspec(dllexport) UINT WINAPI Mine_GlobalGetAtomNameW( ATOM nAtom, LPWSTR lpBuffer, int nSize ){
  if(ChessWrapperSentry::Wrap("GlobalGetAtomNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalGetAtomNameW");
   }
  return Real_GlobalGetAtomNameW(nAtom, lpBuffer, nSize);
}
HGLOBAL (WINAPI * Real_GlobalHandle)( LPCVOID pMem )
  = GlobalHandle;

__declspec(dllexport) HGLOBAL WINAPI Mine_GlobalHandle( LPCVOID pMem ){
  if(ChessWrapperSentry::Wrap("GlobalHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalHandle");
   }
  return Real_GlobalHandle(pMem);
}
LPVOID (WINAPI * Real_GlobalLock)( HGLOBAL hMem )
  = GlobalLock;

__declspec(dllexport) LPVOID WINAPI Mine_GlobalLock( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalLock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalLock");
   }
  return Real_GlobalLock(hMem);
}
void (WINAPI * Real_GlobalMemoryStatus)( LPMEMORYSTATUS lpBuffer )
  = GlobalMemoryStatus;

__declspec(dllexport) void WINAPI Mine_GlobalMemoryStatus( LPMEMORYSTATUS lpBuffer ){
  if(ChessWrapperSentry::Wrap("GlobalMemoryStatus")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalMemoryStatus");
   }
  return Real_GlobalMemoryStatus(lpBuffer);
}
BOOL (WINAPI * Real_GlobalMemoryStatusEx)( LPMEMORYSTATUSEX lpBuffer )
  = GlobalMemoryStatusEx;

__declspec(dllexport) BOOL WINAPI Mine_GlobalMemoryStatusEx( LPMEMORYSTATUSEX lpBuffer ){
  if(ChessWrapperSentry::Wrap("GlobalMemoryStatusEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalMemoryStatusEx");
   }
  return Real_GlobalMemoryStatusEx(lpBuffer);
}
HGLOBAL (WINAPI * Real_GlobalReAlloc)( HGLOBAL hMem, SIZE_T dwBytes, UINT uFlags )
  = GlobalReAlloc;

__declspec(dllexport) HGLOBAL WINAPI Mine_GlobalReAlloc( HGLOBAL hMem, SIZE_T dwBytes, UINT uFlags ){
  if(ChessWrapperSentry::Wrap("GlobalReAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalReAlloc");
   }
  return Real_GlobalReAlloc(hMem, dwBytes, uFlags);
}
SIZE_T (WINAPI * Real_GlobalSize)( HGLOBAL hMem )
  = GlobalSize;

__declspec(dllexport) SIZE_T WINAPI Mine_GlobalSize( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalSize");
   }
  return Real_GlobalSize(hMem);
}
BOOL (WINAPI * Real_GlobalUnWire)( HGLOBAL hMem )
  = GlobalUnWire;

__declspec(dllexport) BOOL WINAPI Mine_GlobalUnWire( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalUnWire")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalUnWire");
   }
  return Real_GlobalUnWire(hMem);
}
void (WINAPI * Real_GlobalUnfix)( HGLOBAL hMem )
  = GlobalUnfix;

__declspec(dllexport) void WINAPI Mine_GlobalUnfix( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalUnfix")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalUnfix");
   }
  return Real_GlobalUnfix(hMem);
}
BOOL (WINAPI * Real_GlobalUnlock)( HGLOBAL hMem )
  = GlobalUnlock;

__declspec(dllexport) BOOL WINAPI Mine_GlobalUnlock( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalUnlock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalUnlock");
   }
  return Real_GlobalUnlock(hMem);
}
LPVOID (WINAPI * Real_GlobalWire)( HGLOBAL hMem )
  = GlobalWire;

__declspec(dllexport) LPVOID WINAPI Mine_GlobalWire( HGLOBAL hMem ){
  if(ChessWrapperSentry::Wrap("GlobalWire")){
     ChessWrapperSentry sentry;
     Chess::LogCall("GlobalWire");
   }
  return Real_GlobalWire(hMem);
}
LPVOID (WINAPI * Real_HeapAlloc)( HANDLE hHeap, DWORD dwFlags, SIZE_T dwBytes )
  = HeapAlloc;

__declspec(dllexport) LPVOID WINAPI Mine_HeapAlloc( HANDLE hHeap, DWORD dwFlags, SIZE_T dwBytes ){
  if(ChessWrapperSentry::Wrap("HeapAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapAlloc");
   }
  return Real_HeapAlloc(hHeap, dwFlags, dwBytes);
}
SIZE_T (WINAPI * Real_HeapCompact)( HANDLE hHeap, DWORD dwFlags )
  = HeapCompact;

__declspec(dllexport) SIZE_T WINAPI Mine_HeapCompact( HANDLE hHeap, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("HeapCompact")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapCompact");
   }
  return Real_HeapCompact(hHeap, dwFlags);
}
HANDLE (WINAPI * Real_HeapCreate)( DWORD flOptions, SIZE_T dwInitialSize, SIZE_T dwMaximumSize )
  = HeapCreate;

__declspec(dllexport) HANDLE WINAPI Mine_HeapCreate( DWORD flOptions, SIZE_T dwInitialSize, SIZE_T dwMaximumSize ){
  if(ChessWrapperSentry::Wrap("HeapCreate")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapCreate");
   }
  return Real_HeapCreate(flOptions, dwInitialSize, dwMaximumSize);
}
BOOL (WINAPI * Real_HeapDestroy)( HANDLE hHeap )
  = HeapDestroy;

__declspec(dllexport) BOOL WINAPI Mine_HeapDestroy( HANDLE hHeap ){
  if(ChessWrapperSentry::Wrap("HeapDestroy")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapDestroy");
   }
  return Real_HeapDestroy(hHeap);
}
BOOL (WINAPI * Real_HeapFree)( HANDLE hHeap, DWORD dwFlags, LPVOID lpMem )
  = HeapFree;

__declspec(dllexport) BOOL WINAPI Mine_HeapFree( HANDLE hHeap, DWORD dwFlags, LPVOID lpMem ){
  if(ChessWrapperSentry::Wrap("HeapFree")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapFree");
   }
  return Real_HeapFree(hHeap, dwFlags, lpMem);
}
BOOL (WINAPI * Real_HeapLock)( HANDLE hHeap )
  = HeapLock;

__declspec(dllexport) BOOL WINAPI Mine_HeapLock( HANDLE hHeap ){
  if(ChessWrapperSentry::Wrap("HeapLock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapLock");
   }
  return Real_HeapLock(hHeap);
}
BOOL (WINAPI * Real_HeapQueryInformation)( HANDLE HeapHandle, HEAP_INFORMATION_CLASS HeapInformationClass, PVOID HeapInformation, SIZE_T HeapInformationLength, PSIZE_T ReturnLength )
  = HeapQueryInformation;

__declspec(dllexport) BOOL WINAPI Mine_HeapQueryInformation( HANDLE HeapHandle, HEAP_INFORMATION_CLASS HeapInformationClass, PVOID HeapInformation, SIZE_T HeapInformationLength, PSIZE_T ReturnLength ){
  if(ChessWrapperSentry::Wrap("HeapQueryInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapQueryInformation");
   }
  return Real_HeapQueryInformation(HeapHandle, HeapInformationClass, HeapInformation, HeapInformationLength, ReturnLength);
}
LPVOID (WINAPI * Real_HeapReAlloc)( HANDLE hHeap, DWORD dwFlags, LPVOID lpMem, SIZE_T dwBytes )
  = HeapReAlloc;

__declspec(dllexport) LPVOID WINAPI Mine_HeapReAlloc( HANDLE hHeap, DWORD dwFlags, LPVOID lpMem, SIZE_T dwBytes ){
  if(ChessWrapperSentry::Wrap("HeapReAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapReAlloc");
   }
  return Real_HeapReAlloc(hHeap, dwFlags, lpMem, dwBytes);
}
BOOL (WINAPI * Real_HeapSetInformation)( HANDLE HeapHandle, HEAP_INFORMATION_CLASS HeapInformationClass, PVOID HeapInformation, SIZE_T HeapInformationLength )
  = HeapSetInformation;

__declspec(dllexport) BOOL WINAPI Mine_HeapSetInformation( HANDLE HeapHandle, HEAP_INFORMATION_CLASS HeapInformationClass, PVOID HeapInformation, SIZE_T HeapInformationLength ){
  if(ChessWrapperSentry::Wrap("HeapSetInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapSetInformation");
   }
  return Real_HeapSetInformation(HeapHandle, HeapInformationClass, HeapInformation, HeapInformationLength);
}
SIZE_T (WINAPI * Real_HeapSize)( HANDLE hHeap, DWORD dwFlags, LPCVOID lpMem )
  = HeapSize;

__declspec(dllexport) SIZE_T WINAPI Mine_HeapSize( HANDLE hHeap, DWORD dwFlags, LPCVOID lpMem ){
  if(ChessWrapperSentry::Wrap("HeapSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapSize");
   }
  return Real_HeapSize(hHeap, dwFlags, lpMem);
}
BOOL (WINAPI * Real_HeapUnlock)( HANDLE hHeap )
  = HeapUnlock;

__declspec(dllexport) BOOL WINAPI Mine_HeapUnlock( HANDLE hHeap ){
  if(ChessWrapperSentry::Wrap("HeapUnlock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapUnlock");
   }
  return Real_HeapUnlock(hHeap);
}
BOOL (WINAPI * Real_HeapValidate)( HANDLE hHeap, DWORD dwFlags, LPCVOID lpMem )
  = HeapValidate;

__declspec(dllexport) BOOL WINAPI Mine_HeapValidate( HANDLE hHeap, DWORD dwFlags, LPCVOID lpMem ){
  if(ChessWrapperSentry::Wrap("HeapValidate")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapValidate");
   }
  return Real_HeapValidate(hHeap, dwFlags, lpMem);
}
BOOL (WINAPI * Real_HeapWalk)( HANDLE hHeap, LPPROCESS_HEAP_ENTRY lpEntry )
  = HeapWalk;

__declspec(dllexport) BOOL WINAPI Mine_HeapWalk( HANDLE hHeap, LPPROCESS_HEAP_ENTRY lpEntry ){
  if(ChessWrapperSentry::Wrap("HeapWalk")){
     ChessWrapperSentry sentry;
     Chess::LogCall("HeapWalk");
   }
  return Real_HeapWalk(hHeap, lpEntry);
}
BOOL (WINAPI * Real_InitAtomTable)( DWORD nSize )
  = InitAtomTable;

__declspec(dllexport) BOOL WINAPI Mine_InitAtomTable( DWORD nSize ){
  if(ChessWrapperSentry::Wrap("InitAtomTable")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InitAtomTable");
   }
  return Real_InitAtomTable(nSize);
}
void (WINAPI * Real_InitializeCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
  = InitializeCriticalSection;

__declspec(dllexport) void WINAPI Mine_InitializeCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
  if(ChessWrapperSentry::Wrap("InitializeCriticalSection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InitializeCriticalSection");
   }
  return Real_InitializeCriticalSection(lpCriticalSection);
}
BOOL (WINAPI * Real_InitializeCriticalSectionAndSpinCount)( LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount )
  = InitializeCriticalSectionAndSpinCount;

__declspec(dllexport) BOOL WINAPI Mine_InitializeCriticalSectionAndSpinCount( LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount ){
  if(ChessWrapperSentry::Wrap("InitializeCriticalSectionAndSpinCount")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InitializeCriticalSectionAndSpinCount");
   }
  return Real_InitializeCriticalSectionAndSpinCount(lpCriticalSection, dwSpinCount);
}
void (WINAPI * Real_InitializeSListHead)( PSLIST_HEADER ListHead )
  = InitializeSListHead;

__declspec(dllexport) void WINAPI Mine_InitializeSListHead( PSLIST_HEADER ListHead ){
  if(ChessWrapperSentry::Wrap("InitializeSListHead")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InitializeSListHead");
   }
  return Real_InitializeSListHead(ListHead);
}
LONG (WINAPI * Real_InterlockedCompareExchange)( LONG volatile *Destination, LONG Exchange, LONG Comperand )
   = InterlockedCompareExchange;

__declspec(dllexport) LONG WINAPI Mine_InterlockedCompareExchange( LONG volatile *Destination, LONG Exchange, LONG Comperand ){
#ifdef WRAP_InterlockedCompareExchange
  if(ChessWrapperSentry::Wrap("InterlockedCompareExchange")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedCompareExchange");
     LONG res = __wrapper_InterlockedCompareExchange(Destination, Exchange, Comperand);
     return res;
  }
#endif
  return Real_InterlockedCompareExchange(Destination, Exchange, Comperand);
}

LONG (WINAPI * Real_InterlockedDecrement)( LONG volatile *lpAddend )
   = InterlockedDecrement;

__declspec(dllexport) LONG WINAPI Mine_InterlockedDecrement( LONG volatile *lpAddend ){
#ifdef WRAP_InterlockedDecrement
  if(ChessWrapperSentry::Wrap("InterlockedDecrement")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedDecrement");
     LONG res = __wrapper_InterlockedDecrement(lpAddend);
     return res;
  }
#endif
  return Real_InterlockedDecrement(lpAddend);
}
LONG (WINAPI * Real_InterlockedExchange)( LONG volatile *Target, LONG Value )
   = InterlockedExchange;

__declspec(dllexport) LONG WINAPI Mine_InterlockedExchange( LONG volatile *Target, LONG Value ){
#ifdef WRAP_InterlockedExchange
  if(ChessWrapperSentry::Wrap("InterlockedExchange")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedExchange");
     LONG res = __wrapper_InterlockedExchange(Target, Value);
     return res;
  }
#endif
  return Real_InterlockedExchange(Target, Value);
}
LONG (WINAPI * Real_InterlockedExchangeAdd)( LONG volatile *Addend, LONG Value )
  = InterlockedExchangeAdd;

__declspec(dllexport) LONG WINAPI Mine_InterlockedExchangeAdd( LONG volatile *Addend, LONG Value ){
  if(ChessWrapperSentry::Wrap("InterlockedExchangeAdd")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedExchangeAdd");
   }
  return Real_InterlockedExchangeAdd(Addend, Value);
}
PSINGLE_LIST_ENTRY (WINAPI * Real_InterlockedFlushSList)( PSLIST_HEADER ListHead )
  = InterlockedFlushSList;

__declspec(dllexport) PSINGLE_LIST_ENTRY WINAPI Mine_InterlockedFlushSList( PSLIST_HEADER ListHead ){
  if(ChessWrapperSentry::Wrap("InterlockedFlushSList")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedFlushSList");
   }
  return Real_InterlockedFlushSList(ListHead);
}
LONG (WINAPI * Real_InterlockedIncrement)( LONG volatile *lpAddend )
   = InterlockedIncrement;

__declspec(dllexport) LONG WINAPI Mine_InterlockedIncrement( LONG volatile *lpAddend ){
#ifdef WRAP_InterlockedIncrement
  if(ChessWrapperSentry::Wrap("InterlockedIncrement")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedIncrement");
     LONG res = __wrapper_InterlockedIncrement(lpAddend);
     return res;
  }
#endif
  return Real_InterlockedIncrement(lpAddend);
}
PSINGLE_LIST_ENTRY (WINAPI * Real_InterlockedPopEntrySList)( PSLIST_HEADER ListHead )
  = InterlockedPopEntrySList;

__declspec(dllexport) PSINGLE_LIST_ENTRY WINAPI Mine_InterlockedPopEntrySList( PSLIST_HEADER ListHead ){
  if(ChessWrapperSentry::Wrap("InterlockedPopEntrySList")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedPopEntrySList");
   }
  return Real_InterlockedPopEntrySList(ListHead);
}
PSINGLE_LIST_ENTRY (WINAPI * Real_InterlockedPushEntrySList)( PSLIST_HEADER ListHead, PSINGLE_LIST_ENTRY ListEntry )
  = InterlockedPushEntrySList;

__declspec(dllexport) PSINGLE_LIST_ENTRY WINAPI Mine_InterlockedPushEntrySList( PSLIST_HEADER ListHead, PSINGLE_LIST_ENTRY ListEntry ){
  if(ChessWrapperSentry::Wrap("InterlockedPushEntrySList")){
     ChessWrapperSentry sentry;
     Chess::LogCall("InterlockedPushEntrySList");
   }
  return Real_InterlockedPushEntrySList(ListHead, ListEntry);
}
BOOL (WINAPI * Real_IsBadCodePtr)( FARPROC lpfn )
  = IsBadCodePtr;

__declspec(dllexport) BOOL WINAPI Mine_IsBadCodePtr( FARPROC lpfn ){
  if(ChessWrapperSentry::Wrap("IsBadCodePtr")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadCodePtr");
   }
  return Real_IsBadCodePtr(lpfn);
}
BOOL (WINAPI * Real_IsBadHugeReadPtr)( const void *lp, UINT_PTR ucb )
  = IsBadHugeReadPtr;

__declspec(dllexport) BOOL WINAPI Mine_IsBadHugeReadPtr( const void *lp, UINT_PTR ucb ){
  if(ChessWrapperSentry::Wrap("IsBadHugeReadPtr")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadHugeReadPtr");
   }
  return Real_IsBadHugeReadPtr(lp, ucb);
}
BOOL (WINAPI * Real_IsBadHugeWritePtr)( LPVOID lp, UINT_PTR ucb )
  = IsBadHugeWritePtr;

__declspec(dllexport) BOOL WINAPI Mine_IsBadHugeWritePtr( LPVOID lp, UINT_PTR ucb ){
  if(ChessWrapperSentry::Wrap("IsBadHugeWritePtr")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadHugeWritePtr");
   }
  return Real_IsBadHugeWritePtr(lp, ucb);
}
BOOL (WINAPI * Real_IsBadReadPtr)( const void *lp, UINT_PTR ucb )
  = IsBadReadPtr;

__declspec(dllexport) BOOL WINAPI Mine_IsBadReadPtr( const void *lp, UINT_PTR ucb ){
  if(ChessWrapperSentry::Wrap("IsBadReadPtr")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadReadPtr");
   }
  return Real_IsBadReadPtr(lp, ucb);
}
BOOL (WINAPI * Real_IsBadStringPtrA)( LPCSTR lpsz, UINT_PTR ucchMax )
  = IsBadStringPtrA;

__declspec(dllexport) BOOL WINAPI Mine_IsBadStringPtrA( LPCSTR lpsz, UINT_PTR ucchMax ){
  if(ChessWrapperSentry::Wrap("IsBadStringPtrA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadStringPtrA");
   }
  return Real_IsBadStringPtrA(lpsz, ucchMax);
}
BOOL (WINAPI * Real_IsBadStringPtrW)( LPCWSTR lpsz, UINT_PTR ucchMax )
  = IsBadStringPtrW;

__declspec(dllexport) BOOL WINAPI Mine_IsBadStringPtrW( LPCWSTR lpsz, UINT_PTR ucchMax ){
  if(ChessWrapperSentry::Wrap("IsBadStringPtrW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadStringPtrW");
   }
  return Real_IsBadStringPtrW(lpsz, ucchMax);
}
BOOL (WINAPI * Real_IsBadWritePtr)( LPVOID lp, UINT_PTR ucb )
  = IsBadWritePtr;

__declspec(dllexport) BOOL WINAPI Mine_IsBadWritePtr( LPVOID lp, UINT_PTR ucb ){
  if(ChessWrapperSentry::Wrap("IsBadWritePtr")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsBadWritePtr");
   }
  return Real_IsBadWritePtr(lp, ucb);
}
BOOL (WINAPI * Real_IsDBCSLeadByte)( BYTE TestChar)
  = IsDBCSLeadByte;

__declspec(dllexport) BOOL WINAPI Mine_IsDBCSLeadByte( BYTE TestChar){
  if(ChessWrapperSentry::Wrap("IsDBCSLeadByte")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsDBCSLeadByte");
   }
  return Real_IsDBCSLeadByte(TestChar);
}
BOOL (WINAPI * Real_IsDBCSLeadByteEx)( UINT CodePage, BYTE TestChar)
  = IsDBCSLeadByteEx;

__declspec(dllexport) BOOL WINAPI Mine_IsDBCSLeadByteEx( UINT CodePage, BYTE TestChar){
  if(ChessWrapperSentry::Wrap("IsDBCSLeadByteEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsDBCSLeadByteEx");
   }
  return Real_IsDBCSLeadByteEx(CodePage, TestChar);
}
BOOL (WINAPI * Real_IsDebuggerPresent)( void )
  = IsDebuggerPresent;

__declspec(dllexport) BOOL WINAPI Mine_IsDebuggerPresent( void ){
  if(ChessWrapperSentry::Wrap("IsDebuggerPresent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsDebuggerPresent");
   }
  return Real_IsDebuggerPresent();
}
BOOL (WINAPI * Real_IsNLSDefinedString)( NLS_FUNCTION Function, DWORD dwFlags, LPNLSVERSIONINFO lpVersionInformation, LPCWSTR lpString, INT cchStr)
  = IsNLSDefinedString;

__declspec(dllexport) BOOL WINAPI Mine_IsNLSDefinedString( NLS_FUNCTION Function, DWORD dwFlags, LPNLSVERSIONINFO lpVersionInformation, LPCWSTR lpString, INT cchStr){
  if(ChessWrapperSentry::Wrap("IsNLSDefinedString")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsNLSDefinedString");
   }
  return Real_IsNLSDefinedString(Function, dwFlags, lpVersionInformation, lpString, cchStr);
}
BOOL (WINAPI * Real_IsProcessInJob)( HANDLE ProcessHandle, HANDLE JobHandle, PBOOL Result )
  = IsProcessInJob;

__declspec(dllexport) BOOL WINAPI Mine_IsProcessInJob( HANDLE ProcessHandle, HANDLE JobHandle, PBOOL Result ){
  if(ChessWrapperSentry::Wrap("IsProcessInJob")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsProcessInJob");
   }
  return Real_IsProcessInJob(ProcessHandle, JobHandle, Result);
}
BOOL (WINAPI * Real_IsProcessorFeaturePresent)( DWORD ProcessorFeature )
  = IsProcessorFeaturePresent;

__declspec(dllexport) BOOL WINAPI Mine_IsProcessorFeaturePresent( DWORD ProcessorFeature ){
  if(ChessWrapperSentry::Wrap("IsProcessorFeaturePresent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsProcessorFeaturePresent");
   }
  return Real_IsProcessorFeaturePresent(ProcessorFeature);
}
BOOL (WINAPI * Real_IsSystemResumeAutomatic)( void )
  = IsSystemResumeAutomatic;

__declspec(dllexport) BOOL WINAPI Mine_IsSystemResumeAutomatic( void ){
  if(ChessWrapperSentry::Wrap("IsSystemResumeAutomatic")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsSystemResumeAutomatic");
   }
  return Real_IsSystemResumeAutomatic();
}
BOOL (WINAPI * Real_IsValidCodePage)( UINT CodePage)
  = IsValidCodePage;

__declspec(dllexport) BOOL WINAPI Mine_IsValidCodePage( UINT CodePage){
  if(ChessWrapperSentry::Wrap("IsValidCodePage")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsValidCodePage");
   }
  return Real_IsValidCodePage(CodePage);
}
BOOL (WINAPI * Real_IsValidLanguageGroup)( LGRPID LanguageGroup, DWORD dwFlags)
  = IsValidLanguageGroup;

__declspec(dllexport) BOOL WINAPI Mine_IsValidLanguageGroup( LGRPID LanguageGroup, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("IsValidLanguageGroup")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsValidLanguageGroup");
   }
  return Real_IsValidLanguageGroup(LanguageGroup, dwFlags);
}
BOOL (WINAPI * Real_IsValidLocale)( LCID Locale, DWORD dwFlags)
  = IsValidLocale;

__declspec(dllexport) BOOL WINAPI Mine_IsValidLocale( LCID Locale, DWORD dwFlags){
  if(ChessWrapperSentry::Wrap("IsValidLocale")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsValidLocale");
   }
  return Real_IsValidLocale(Locale, dwFlags);
}
BOOL (WINAPI * Real_IsWow64Process)( HANDLE hProcess, PBOOL Wow64Process )
  = IsWow64Process;

__declspec(dllexport) BOOL WINAPI Mine_IsWow64Process( HANDLE hProcess, PBOOL Wow64Process ){
  if(ChessWrapperSentry::Wrap("IsWow64Process")){
     ChessWrapperSentry sentry;
     Chess::LogCall("IsWow64Process");
   }
  return Real_IsWow64Process(hProcess, Wow64Process);
}
int (WINAPI * Real_LCMapStringA)( LCID Locale, DWORD dwMapFlags, LPCSTR lpSrcStr, int cchSrc, LPSTR lpDestStr, int cchDest)
  = LCMapStringA;

__declspec(dllexport) int WINAPI Mine_LCMapStringA( LCID Locale, DWORD dwMapFlags, LPCSTR lpSrcStr, int cchSrc, LPSTR lpDestStr, int cchDest){
  if(ChessWrapperSentry::Wrap("LCMapStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LCMapStringA");
   }
  return Real_LCMapStringA(Locale, dwMapFlags, lpSrcStr, cchSrc, lpDestStr, cchDest);
}
int (WINAPI * Real_LCMapStringW)( LCID Locale, DWORD dwMapFlags, LPCWSTR lpSrcStr, int cchSrc, LPWSTR lpDestStr, int cchDest)
  = LCMapStringW;

__declspec(dllexport) int WINAPI Mine_LCMapStringW( LCID Locale, DWORD dwMapFlags, LPCWSTR lpSrcStr, int cchSrc, LPWSTR lpDestStr, int cchDest){
  if(ChessWrapperSentry::Wrap("LCMapStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LCMapStringW");
   }
  return Real_LCMapStringW(Locale, dwMapFlags, lpSrcStr, cchSrc, lpDestStr, cchDest);
}
void (WINAPI * Real_LeaveCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
   = LeaveCriticalSection;

__declspec(dllexport) void WINAPI Mine_LeaveCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
#ifdef WRAP_LeaveCriticalSection
  if(ChessWrapperSentry::Wrap("LeaveCriticalSection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LeaveCriticalSection");
     __wrapper_LeaveCriticalSection(lpCriticalSection);
     return;
  }
#endif
  return Real_LeaveCriticalSection(lpCriticalSection);
}
HMODULE (WINAPI * Real_LoadLibraryA)( LPCSTR lpLibFileName )
  = LoadLibraryA;

__declspec(dllexport) HMODULE WINAPI Mine_LoadLibraryA( LPCSTR lpLibFileName ){
  if(ChessWrapperSentry::Wrap("LoadLibraryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LoadLibraryA");
   }
  return Real_LoadLibraryA(lpLibFileName);
}
HMODULE (WINAPI * Real_LoadLibraryExA)( LPCSTR lpLibFileName, HANDLE hFile, DWORD dwFlags )
  = LoadLibraryExA;

__declspec(dllexport) HMODULE WINAPI Mine_LoadLibraryExA( LPCSTR lpLibFileName, HANDLE hFile, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("LoadLibraryExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LoadLibraryExA");
   }
  return Real_LoadLibraryExA(lpLibFileName, hFile, dwFlags);
}
HMODULE (WINAPI * Real_LoadLibraryExW)( LPCWSTR lpLibFileName, HANDLE hFile, DWORD dwFlags )
  = LoadLibraryExW;

__declspec(dllexport) HMODULE WINAPI Mine_LoadLibraryExW( LPCWSTR lpLibFileName, HANDLE hFile, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("LoadLibraryExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LoadLibraryExW");
   }
  return Real_LoadLibraryExW(lpLibFileName, hFile, dwFlags);
}
HMODULE (WINAPI * Real_LoadLibraryW)( LPCWSTR lpLibFileName )
  = LoadLibraryW;

__declspec(dllexport) HMODULE WINAPI Mine_LoadLibraryW( LPCWSTR lpLibFileName ){
  if(ChessWrapperSentry::Wrap("LoadLibraryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LoadLibraryW");
   }
  return Real_LoadLibraryW(lpLibFileName);
}
DWORD (WINAPI * Real_LoadModule)( LPCSTR lpModuleName, LPVOID lpParameterBlock )
  = LoadModule;

__declspec(dllexport) DWORD WINAPI Mine_LoadModule( LPCSTR lpModuleName, LPVOID lpParameterBlock ){
  if(ChessWrapperSentry::Wrap("LoadModule")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LoadModule");
   }
  return Real_LoadModule(lpModuleName, lpParameterBlock);
}
HGLOBAL (WINAPI * Real_LoadResource)( HMODULE hModule, HRSRC hResInfo )
  = LoadResource;

__declspec(dllexport) HGLOBAL WINAPI Mine_LoadResource( HMODULE hModule, HRSRC hResInfo ){
  if(ChessWrapperSentry::Wrap("LoadResource")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LoadResource");
   }
  return Real_LoadResource(hModule, hResInfo);
}
HLOCAL (WINAPI * Real_LocalAlloc)( UINT uFlags, SIZE_T uBytes )
  = LocalAlloc;

__declspec(dllexport) HLOCAL WINAPI Mine_LocalAlloc( UINT uFlags, SIZE_T uBytes ){
  if(ChessWrapperSentry::Wrap("LocalAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalAlloc");
   }
  return Real_LocalAlloc(uFlags, uBytes);
}
SIZE_T (WINAPI * Real_LocalCompact)( UINT uMinFree )
  = LocalCompact;

__declspec(dllexport) SIZE_T WINAPI Mine_LocalCompact( UINT uMinFree ){
  if(ChessWrapperSentry::Wrap("LocalCompact")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalCompact");
   }
  return Real_LocalCompact(uMinFree);
}
BOOL (WINAPI * Real_LocalFileTimeToFileTime)( const FILETIME *lpLocalFileTime, LPFILETIME lpFileTime )
  = LocalFileTimeToFileTime;

__declspec(dllexport) BOOL WINAPI Mine_LocalFileTimeToFileTime( const FILETIME *lpLocalFileTime, LPFILETIME lpFileTime ){
  if(ChessWrapperSentry::Wrap("LocalFileTimeToFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalFileTimeToFileTime");
   }
  return Real_LocalFileTimeToFileTime(lpLocalFileTime, lpFileTime);
}
UINT (WINAPI * Real_LocalFlags)( HLOCAL hMem )
  = LocalFlags;

__declspec(dllexport) UINT WINAPI Mine_LocalFlags( HLOCAL hMem ){
  if(ChessWrapperSentry::Wrap("LocalFlags")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalFlags");
   }
  return Real_LocalFlags(hMem);
}
HLOCAL (WINAPI * Real_LocalFree)( HLOCAL hMem )
  = LocalFree;

__declspec(dllexport) HLOCAL WINAPI Mine_LocalFree( HLOCAL hMem ){
  if(ChessWrapperSentry::Wrap("LocalFree")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalFree");
   }
  return Real_LocalFree(hMem);
}
HLOCAL (WINAPI * Real_LocalHandle)( LPCVOID pMem )
  = LocalHandle;

__declspec(dllexport) HLOCAL WINAPI Mine_LocalHandle( LPCVOID pMem ){
  if(ChessWrapperSentry::Wrap("LocalHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalHandle");
   }
  return Real_LocalHandle(pMem);
}
LPVOID (WINAPI * Real_LocalLock)( HLOCAL hMem )
  = LocalLock;

__declspec(dllexport) LPVOID WINAPI Mine_LocalLock( HLOCAL hMem ){
  if(ChessWrapperSentry::Wrap("LocalLock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalLock");
   }
  return Real_LocalLock(hMem);
}
HLOCAL (WINAPI * Real_LocalReAlloc)( HLOCAL hMem, SIZE_T uBytes, UINT uFlags )
  = LocalReAlloc;

__declspec(dllexport) HLOCAL WINAPI Mine_LocalReAlloc( HLOCAL hMem, SIZE_T uBytes, UINT uFlags ){
  if(ChessWrapperSentry::Wrap("LocalReAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalReAlloc");
   }
  return Real_LocalReAlloc(hMem, uBytes, uFlags);
}
SIZE_T (WINAPI * Real_LocalShrink)( HLOCAL hMem, UINT cbNewSize )
  = LocalShrink;

__declspec(dllexport) SIZE_T WINAPI Mine_LocalShrink( HLOCAL hMem, UINT cbNewSize ){
  if(ChessWrapperSentry::Wrap("LocalShrink")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalShrink");
   }
  return Real_LocalShrink(hMem, cbNewSize);
}
SIZE_T (WINAPI * Real_LocalSize)( HLOCAL hMem )
  = LocalSize;

__declspec(dllexport) SIZE_T WINAPI Mine_LocalSize( HLOCAL hMem ){
  if(ChessWrapperSentry::Wrap("LocalSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalSize");
   }
  return Real_LocalSize(hMem);
}
BOOL (WINAPI * Real_LocalUnlock)( HLOCAL hMem )
  = LocalUnlock;

__declspec(dllexport) BOOL WINAPI Mine_LocalUnlock( HLOCAL hMem ){
  if(ChessWrapperSentry::Wrap("LocalUnlock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LocalUnlock");
   }
  return Real_LocalUnlock(hMem);
}
BOOL (WINAPI * Real_LockFile)( HANDLE hFile, DWORD dwFileOffsetLow, DWORD dwFileOffsetHigh, DWORD nNumberOfBytesToLockLow, DWORD nNumberOfBytesToLockHigh )
  = LockFile;

__declspec(dllexport) BOOL WINAPI Mine_LockFile( HANDLE hFile, DWORD dwFileOffsetLow, DWORD dwFileOffsetHigh, DWORD nNumberOfBytesToLockLow, DWORD nNumberOfBytesToLockHigh ){
  if(ChessWrapperSentry::Wrap("LockFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LockFile");
   }
  return Real_LockFile(hFile, dwFileOffsetLow, dwFileOffsetHigh, nNumberOfBytesToLockLow, nNumberOfBytesToLockHigh);
}
BOOL (WINAPI * Real_LockFileEx)( HANDLE hFile, DWORD dwFlags, DWORD dwReserved, DWORD nNumberOfBytesToLockLow, DWORD nNumberOfBytesToLockHigh, LPOVERLAPPED lpOverlapped )
  = LockFileEx;

__declspec(dllexport) BOOL WINAPI Mine_LockFileEx( HANDLE hFile, DWORD dwFlags, DWORD dwReserved, DWORD nNumberOfBytesToLockLow, DWORD nNumberOfBytesToLockHigh, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("LockFileEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LockFileEx");
   }
  return Real_LockFileEx(hFile, dwFlags, dwReserved, nNumberOfBytesToLockLow, nNumberOfBytesToLockHigh, lpOverlapped);
}
LPVOID (WINAPI * Real_LockResource)( HGLOBAL hResData )
  = LockResource;

__declspec(dllexport) LPVOID WINAPI Mine_LockResource( HGLOBAL hResData ){
  if(ChessWrapperSentry::Wrap("LockResource")){
     ChessWrapperSentry sentry;
     Chess::LogCall("LockResource");
   }
  return Real_LockResource(hResData);
}
BOOL (WINAPI * Real_MapUserPhysicalPages)( PVOID VirtualAddress, ULONG_PTR NumberOfPages, PULONG_PTR PageArray )
  = MapUserPhysicalPages;

__declspec(dllexport) BOOL WINAPI Mine_MapUserPhysicalPages( PVOID VirtualAddress, ULONG_PTR NumberOfPages, PULONG_PTR PageArray ){
  if(ChessWrapperSentry::Wrap("MapUserPhysicalPages")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MapUserPhysicalPages");
   }
  return Real_MapUserPhysicalPages(VirtualAddress, NumberOfPages, PageArray);
}
BOOL (WINAPI * Real_MapUserPhysicalPagesScatter)( PVOID *VirtualAddresses, ULONG_PTR NumberOfPages, PULONG_PTR PageArray )
  = MapUserPhysicalPagesScatter;

__declspec(dllexport) BOOL WINAPI Mine_MapUserPhysicalPagesScatter( PVOID *VirtualAddresses, ULONG_PTR NumberOfPages, PULONG_PTR PageArray ){
  if(ChessWrapperSentry::Wrap("MapUserPhysicalPagesScatter")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MapUserPhysicalPagesScatter");
   }
  return Real_MapUserPhysicalPagesScatter(VirtualAddresses, NumberOfPages, PageArray);
}
LPVOID (WINAPI * Real_MapViewOfFile)( HANDLE hFileMappingObject, DWORD dwDesiredAccess, DWORD dwFileOffsetHigh, DWORD dwFileOffsetLow, SIZE_T dwNumberOfBytesToMap )
  = MapViewOfFile;

__declspec(dllexport) LPVOID WINAPI Mine_MapViewOfFile( HANDLE hFileMappingObject, DWORD dwDesiredAccess, DWORD dwFileOffsetHigh, DWORD dwFileOffsetLow, SIZE_T dwNumberOfBytesToMap ){
  if(ChessWrapperSentry::Wrap("MapViewOfFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MapViewOfFile");
   }
  return Real_MapViewOfFile(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumberOfBytesToMap);
}
LPVOID (WINAPI * Real_MapViewOfFileEx)( HANDLE hFileMappingObject, DWORD dwDesiredAccess, DWORD dwFileOffsetHigh, DWORD dwFileOffsetLow, SIZE_T dwNumberOfBytesToMap, LPVOID lpBaseAddress )
  = MapViewOfFileEx;

__declspec(dllexport) LPVOID WINAPI Mine_MapViewOfFileEx( HANDLE hFileMappingObject, DWORD dwDesiredAccess, DWORD dwFileOffsetHigh, DWORD dwFileOffsetLow, SIZE_T dwNumberOfBytesToMap, LPVOID lpBaseAddress ){
  if(ChessWrapperSentry::Wrap("MapViewOfFileEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MapViewOfFileEx");
   }
  return Real_MapViewOfFileEx(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumberOfBytesToMap, lpBaseAddress);
}
BOOL (WINAPI * Real_MoveFileA)( LPCSTR lpExistingFileName, LPCSTR lpNewFileName )
  = MoveFileA;

__declspec(dllexport) BOOL WINAPI Mine_MoveFileA( LPCSTR lpExistingFileName, LPCSTR lpNewFileName ){
  if(ChessWrapperSentry::Wrap("MoveFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MoveFileA");
   }
  return Real_MoveFileA(lpExistingFileName, lpNewFileName);
}
BOOL (WINAPI * Real_MoveFileExA)( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, DWORD dwFlags )
  = MoveFileExA;

__declspec(dllexport) BOOL WINAPI Mine_MoveFileExA( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("MoveFileExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MoveFileExA");
   }
  return Real_MoveFileExA(lpExistingFileName, lpNewFileName, dwFlags);
}
BOOL (WINAPI * Real_MoveFileExW)( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, DWORD dwFlags )
  = MoveFileExW;

__declspec(dllexport) BOOL WINAPI Mine_MoveFileExW( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("MoveFileExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MoveFileExW");
   }
  return Real_MoveFileExW(lpExistingFileName, lpNewFileName, dwFlags);
}
BOOL (WINAPI * Real_MoveFileW)( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName )
  = MoveFileW;

__declspec(dllexport) BOOL WINAPI Mine_MoveFileW( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName ){
  if(ChessWrapperSentry::Wrap("MoveFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MoveFileW");
   }
  return Real_MoveFileW(lpExistingFileName, lpNewFileName);
}
BOOL (WINAPI * Real_MoveFileWithProgressA)( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, DWORD dwFlags )
  = MoveFileWithProgressA;

__declspec(dllexport) BOOL WINAPI Mine_MoveFileWithProgressA( LPCSTR lpExistingFileName, LPCSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("MoveFileWithProgressA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MoveFileWithProgressA");
   }
  return Real_MoveFileWithProgressA(lpExistingFileName, lpNewFileName, lpProgressRoutine, lpData, dwFlags);
}
BOOL (WINAPI * Real_MoveFileWithProgressW)( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, DWORD dwFlags )
  = MoveFileWithProgressW;

__declspec(dllexport) BOOL WINAPI Mine_MoveFileWithProgressW( LPCWSTR lpExistingFileName, LPCWSTR lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, LPVOID lpData, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("MoveFileWithProgressW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MoveFileWithProgressW");
   }
  return Real_MoveFileWithProgressW(lpExistingFileName, lpNewFileName, lpProgressRoutine, lpData, dwFlags);
}
int (WINAPI * Real_MulDiv)( int nNumber, int nNumerator, int nDenominator )
  = MulDiv;

__declspec(dllexport) int WINAPI Mine_MulDiv( int nNumber, int nNumerator, int nDenominator ){
  if(ChessWrapperSentry::Wrap("MulDiv")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MulDiv");
   }
  return Real_MulDiv(nNumber, nNumerator, nDenominator);
}
int (WINAPI * Real_MultiByteToWideChar)( UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte, LPWSTR lpWideCharStr, int cchWideChar)
  = MultiByteToWideChar;

__declspec(dllexport) int WINAPI Mine_MultiByteToWideChar( UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte, LPWSTR lpWideCharStr, int cchWideChar){
  if(ChessWrapperSentry::Wrap("MultiByteToWideChar")){
     ChessWrapperSentry sentry;
     Chess::LogCall("MultiByteToWideChar");
   }
  return Real_MultiByteToWideChar(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, lpWideCharStr, cchWideChar);
}
BOOL (WINAPI * Real_NeedCurrentDirectoryForExePathA)( LPCSTR ExeName )
  = NeedCurrentDirectoryForExePathA;

__declspec(dllexport) BOOL WINAPI Mine_NeedCurrentDirectoryForExePathA( LPCSTR ExeName ){
  if(ChessWrapperSentry::Wrap("NeedCurrentDirectoryForExePathA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("NeedCurrentDirectoryForExePathA");
   }
  return Real_NeedCurrentDirectoryForExePathA(ExeName);
}
BOOL (WINAPI * Real_NeedCurrentDirectoryForExePathW)( LPCWSTR ExeName )
  = NeedCurrentDirectoryForExePathW;

__declspec(dllexport) BOOL WINAPI Mine_NeedCurrentDirectoryForExePathW( LPCWSTR ExeName ){
  if(ChessWrapperSentry::Wrap("NeedCurrentDirectoryForExePathW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("NeedCurrentDirectoryForExePathW");
   }
  return Real_NeedCurrentDirectoryForExePathW(ExeName);
}
HANDLE (WINAPI * Real_OpenEventA)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName )
  = OpenEventA;

__declspec(dllexport) HANDLE WINAPI Mine_OpenEventA( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenEventA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenEventA");
   }
  return Real_OpenEventA(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenEventW)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName )
  = OpenEventW;

__declspec(dllexport) HANDLE WINAPI Mine_OpenEventW( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenEventW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenEventW");
   }
  return Real_OpenEventW(dwDesiredAccess, bInheritHandle, lpName);
}
HFILE (WINAPI * Real_OpenFile)( LPCSTR lpFileName, LPOFSTRUCT lpReOpenBuff, UINT uStyle )
  = OpenFile;

__declspec(dllexport) HFILE WINAPI Mine_OpenFile( LPCSTR lpFileName, LPOFSTRUCT lpReOpenBuff, UINT uStyle ){
  if(ChessWrapperSentry::Wrap("OpenFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenFile");
   }
  return Real_OpenFile(lpFileName, lpReOpenBuff, uStyle);
}
HANDLE (WINAPI * Real_OpenFileMappingA)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName )
  = OpenFileMappingA;

__declspec(dllexport) HANDLE WINAPI Mine_OpenFileMappingA( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenFileMappingA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenFileMappingA");
   }
  return Real_OpenFileMappingA(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenFileMappingW)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName )
  = OpenFileMappingW;

__declspec(dllexport) HANDLE WINAPI Mine_OpenFileMappingW( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenFileMappingW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenFileMappingW");
   }
  return Real_OpenFileMappingW(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenJobObjectA)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName )
  = OpenJobObjectA;

__declspec(dllexport) HANDLE WINAPI Mine_OpenJobObjectA( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenJobObjectA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenJobObjectA");
   }
  return Real_OpenJobObjectA(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenJobObjectW)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName )
  = OpenJobObjectW;

__declspec(dllexport) HANDLE WINAPI Mine_OpenJobObjectW( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenJobObjectW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenJobObjectW");
   }
  return Real_OpenJobObjectW(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenMutexA)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName )
  = OpenMutexA;

__declspec(dllexport) HANDLE WINAPI Mine_OpenMutexA( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenMutexA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenMutexA");
   }
  return Real_OpenMutexA(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenMutexW)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName )
  = OpenMutexW;

__declspec(dllexport) HANDLE WINAPI Mine_OpenMutexW( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenMutexW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenMutexW");
   }
  return Real_OpenMutexW(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenProcess)( DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwProcessId )
  = OpenProcess;

__declspec(dllexport) HANDLE WINAPI Mine_OpenProcess( DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwProcessId ){
  if(ChessWrapperSentry::Wrap("OpenProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenProcess");
   }
  return Real_OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId);
}
HANDLE (WINAPI * Real_OpenSemaphoreA)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName )
  = OpenSemaphoreA;

__declspec(dllexport) HANDLE WINAPI Mine_OpenSemaphoreA( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenSemaphoreA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenSemaphoreA");
   }
  return Real_OpenSemaphoreA(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenSemaphoreW)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName )
  = OpenSemaphoreW;

__declspec(dllexport) HANDLE WINAPI Mine_OpenSemaphoreW( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName ){
  if(ChessWrapperSentry::Wrap("OpenSemaphoreW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenSemaphoreW");
   }
  return Real_OpenSemaphoreW(dwDesiredAccess, bInheritHandle, lpName);
}
HANDLE (WINAPI * Real_OpenThread)( DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwThreadId )
  = OpenThread;

__declspec(dllexport) HANDLE WINAPI Mine_OpenThread( DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwThreadId ){
  if(ChessWrapperSentry::Wrap("OpenThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenThread");
   }
  return Real_OpenThread(dwDesiredAccess, bInheritHandle, dwThreadId);
}
HANDLE (WINAPI * Real_OpenWaitableTimerA)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpTimerName )
  = OpenWaitableTimerA;

__declspec(dllexport) HANDLE WINAPI Mine_OpenWaitableTimerA( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpTimerName ){
  if(ChessWrapperSentry::Wrap("OpenWaitableTimerA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenWaitableTimerA");
   }
  return Real_OpenWaitableTimerA(dwDesiredAccess, bInheritHandle, lpTimerName);
}
HANDLE (WINAPI * Real_OpenWaitableTimerW)( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpTimerName )
  = OpenWaitableTimerW;

__declspec(dllexport) HANDLE WINAPI Mine_OpenWaitableTimerW( DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpTimerName ){
  if(ChessWrapperSentry::Wrap("OpenWaitableTimerW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OpenWaitableTimerW");
   }
  return Real_OpenWaitableTimerW(dwDesiredAccess, bInheritHandle, lpTimerName);
}
void (WINAPI * Real_OutputDebugStringA)( LPCSTR lpOutputString )
  = OutputDebugStringA;

__declspec(dllexport) void WINAPI Mine_OutputDebugStringA( LPCSTR lpOutputString ){
  if(ChessWrapperSentry::Wrap("OutputDebugStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OutputDebugStringA");
   }
  return Real_OutputDebugStringA(lpOutputString);
}
void (WINAPI * Real_OutputDebugStringW)( LPCWSTR lpOutputString )
  = OutputDebugStringW;

__declspec(dllexport) void WINAPI Mine_OutputDebugStringW( LPCWSTR lpOutputString ){
  if(ChessWrapperSentry::Wrap("OutputDebugStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("OutputDebugStringW");
   }
  return Real_OutputDebugStringW(lpOutputString);
}
BOOL (WINAPI * Real_PeekConsoleInputA)( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead )
  = PeekConsoleInputA;

__declspec(dllexport) BOOL WINAPI Mine_PeekConsoleInputA( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead ){
  if(ChessWrapperSentry::Wrap("PeekConsoleInputA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PeekConsoleInputA");
   }
  return Real_PeekConsoleInputA(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);
}
BOOL (WINAPI * Real_PeekConsoleInputW)( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead )
  = PeekConsoleInputW;

__declspec(dllexport) BOOL WINAPI Mine_PeekConsoleInputW( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead ){
  if(ChessWrapperSentry::Wrap("PeekConsoleInputW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PeekConsoleInputW");
   }
  return Real_PeekConsoleInputW(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);
}
BOOL (WINAPI * Real_PeekNamedPipe)( HANDLE hNamedPipe, LPVOID lpBuffer, DWORD nBufferSize, LPDWORD lpBytesRead, LPDWORD lpTotalBytesAvail, LPDWORD lpBytesLeftThisMessage )
  = PeekNamedPipe;

__declspec(dllexport) BOOL WINAPI Mine_PeekNamedPipe( HANDLE hNamedPipe, LPVOID lpBuffer, DWORD nBufferSize, LPDWORD lpBytesRead, LPDWORD lpTotalBytesAvail, LPDWORD lpBytesLeftThisMessage ){
  if(ChessWrapperSentry::Wrap("PeekNamedPipe")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PeekNamedPipe");
   }
  return Real_PeekNamedPipe(hNamedPipe, lpBuffer, nBufferSize, lpBytesRead, lpTotalBytesAvail, lpBytesLeftThisMessage);
}
BOOL (WINAPI * Real_PostQueuedCompletionStatus)( HANDLE CompletionPort, DWORD dwNumberOfBytesTransferred, ULONG_PTR dwCompletionKey, LPOVERLAPPED lpOverlapped )
   = PostQueuedCompletionStatus;

__declspec(dllexport) BOOL WINAPI Mine_PostQueuedCompletionStatus( HANDLE CompletionPort, DWORD dwNumberOfBytesTransferred, ULONG_PTR dwCompletionKey, LPOVERLAPPED lpOverlapped ){
#ifdef WRAP_PostQueuedCompletionStatus
  if(ChessWrapperSentry::Wrap("PostQueuedCompletionStatus")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PostQueuedCompletionStatus");
     BOOL res = __wrapper_PostQueuedCompletionStatus(CompletionPort, dwNumberOfBytesTransferred, dwCompletionKey, lpOverlapped);
     return res;
  }
#endif
  return Real_PostQueuedCompletionStatus(CompletionPort, dwNumberOfBytesTransferred, dwCompletionKey, lpOverlapped);
}
DWORD (WINAPI * Real_PrepareTape)( HANDLE hDevice, DWORD dwOperation, BOOL bImmediate )
  = PrepareTape;

__declspec(dllexport) DWORD WINAPI Mine_PrepareTape( HANDLE hDevice, DWORD dwOperation, BOOL bImmediate ){
  if(ChessWrapperSentry::Wrap("PrepareTape")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PrepareTape");
   }
  return Real_PrepareTape(hDevice, dwOperation, bImmediate);
}
BOOL (WINAPI * Real_ProcessIdToSessionId)( DWORD dwProcessId, DWORD *pSessionId )
  = ProcessIdToSessionId;

__declspec(dllexport) BOOL WINAPI Mine_ProcessIdToSessionId( DWORD dwProcessId, DWORD *pSessionId ){
  if(ChessWrapperSentry::Wrap("ProcessIdToSessionId")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ProcessIdToSessionId");
   }
  return Real_ProcessIdToSessionId(dwProcessId, pSessionId);
}
BOOL (WINAPI * Real_PulseEvent)( HANDLE hEvent )
   = PulseEvent;

__declspec(dllexport) BOOL WINAPI Mine_PulseEvent( HANDLE hEvent ){
#ifdef WRAP_PulseEvent
  if(ChessWrapperSentry::Wrap("PulseEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PulseEvent");
     BOOL res = __wrapper_PulseEvent(hEvent);
     return res;
  }
#endif
  return Real_PulseEvent(hEvent);
}
BOOL (WINAPI * Real_PurgeComm)( HANDLE hFile, DWORD dwFlags )
  = PurgeComm;

__declspec(dllexport) BOOL WINAPI Mine_PurgeComm( HANDLE hFile, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("PurgeComm")){
     ChessWrapperSentry sentry;
     Chess::LogCall("PurgeComm");
   }
  return Real_PurgeComm(hFile, dwFlags);
}
BOOL (WINAPI * Real_QueryActCtxW)( DWORD dwFlags, HANDLE hActCtx, PVOID pvSubInstance, ULONG ulInfoClass, PVOID pvBuffer, SIZE_T cbBuffer, SIZE_T *pcbWrittenOrRequired )
  = QueryActCtxW;

__declspec(dllexport) BOOL WINAPI Mine_QueryActCtxW( DWORD dwFlags, HANDLE hActCtx, PVOID pvSubInstance, ULONG ulInfoClass, PVOID pvBuffer, SIZE_T cbBuffer, SIZE_T *pcbWrittenOrRequired ){
  if(ChessWrapperSentry::Wrap("QueryActCtxW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryActCtxW");
   }
  return Real_QueryActCtxW(dwFlags, hActCtx, pvSubInstance, ulInfoClass, pvBuffer, cbBuffer, pcbWrittenOrRequired);
}
USHORT (WINAPI * Real_QueryDepthSList)( PSLIST_HEADER ListHead )
  = QueryDepthSList;

__declspec(dllexport) USHORT WINAPI Mine_QueryDepthSList( PSLIST_HEADER ListHead ){
  if(ChessWrapperSentry::Wrap("QueryDepthSList")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryDepthSList");
   }
  return Real_QueryDepthSList(ListHead);
}
DWORD (WINAPI * Real_QueryDosDeviceA)( LPCSTR lpDeviceName, LPSTR lpTargetPath, DWORD ucchMax )
  = QueryDosDeviceA;

__declspec(dllexport) DWORD WINAPI Mine_QueryDosDeviceA( LPCSTR lpDeviceName, LPSTR lpTargetPath, DWORD ucchMax ){
  if(ChessWrapperSentry::Wrap("QueryDosDeviceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryDosDeviceA");
   }
  return Real_QueryDosDeviceA(lpDeviceName, lpTargetPath, ucchMax);
}
DWORD (WINAPI * Real_QueryDosDeviceW)( LPCWSTR lpDeviceName, LPWSTR lpTargetPath, DWORD ucchMax )
  = QueryDosDeviceW;

__declspec(dllexport) DWORD WINAPI Mine_QueryDosDeviceW( LPCWSTR lpDeviceName, LPWSTR lpTargetPath, DWORD ucchMax ){
  if(ChessWrapperSentry::Wrap("QueryDosDeviceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryDosDeviceW");
   }
  return Real_QueryDosDeviceW(lpDeviceName, lpTargetPath, ucchMax);
}
BOOL (WINAPI * Real_QueryInformationJobObject)( HANDLE hJob, JOBOBJECTINFOCLASS JobObjectInformationClass, LPVOID lpJobObjectInformation, DWORD cbJobObjectInformationLength, LPDWORD lpReturnLength )
  = QueryInformationJobObject;

__declspec(dllexport) BOOL WINAPI Mine_QueryInformationJobObject( HANDLE hJob, JOBOBJECTINFOCLASS JobObjectInformationClass, LPVOID lpJobObjectInformation, DWORD cbJobObjectInformationLength, LPDWORD lpReturnLength ){
  if(ChessWrapperSentry::Wrap("QueryInformationJobObject")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryInformationJobObject");
   }
  return Real_QueryInformationJobObject(hJob, JobObjectInformationClass, lpJobObjectInformation, cbJobObjectInformationLength, lpReturnLength);
}
BOOL (WINAPI * Real_QueryMemoryResourceNotification)( HANDLE ResourceNotificationHandle, PBOOL ResourceState )
  = QueryMemoryResourceNotification;

__declspec(dllexport) BOOL WINAPI Mine_QueryMemoryResourceNotification( HANDLE ResourceNotificationHandle, PBOOL ResourceState ){
  if(ChessWrapperSentry::Wrap("QueryMemoryResourceNotification")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryMemoryResourceNotification");
   }
  return Real_QueryMemoryResourceNotification(ResourceNotificationHandle, ResourceState);
}
BOOL (WINAPI * Real_QueryPerformanceCounter)( LARGE_INTEGER *lpPerformanceCount )
  = QueryPerformanceCounter;

__declspec(dllexport) BOOL WINAPI Mine_QueryPerformanceCounter( LARGE_INTEGER *lpPerformanceCount ){
  if(ChessWrapperSentry::Wrap("QueryPerformanceCounter")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryPerformanceCounter");
   }
  return Real_QueryPerformanceCounter(lpPerformanceCount);
}
BOOL (WINAPI * Real_QueryPerformanceFrequency)( LARGE_INTEGER *lpFrequency )
  = QueryPerformanceFrequency;

__declspec(dllexport) BOOL WINAPI Mine_QueryPerformanceFrequency( LARGE_INTEGER *lpFrequency ){
  if(ChessWrapperSentry::Wrap("QueryPerformanceFrequency")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueryPerformanceFrequency");
   }
  return Real_QueryPerformanceFrequency(lpFrequency);
}
DWORD (WINAPI * Real_QueueUserAPC)( PAPCFUNC pfnAPC, HANDLE hThread, ULONG_PTR dwData )
   = QueueUserAPC;

__declspec(dllexport) DWORD WINAPI Mine_QueueUserAPC( PAPCFUNC pfnAPC, HANDLE hThread, ULONG_PTR dwData ){
#ifdef WRAP_QueueUserAPC
  if(ChessWrapperSentry::Wrap("QueueUserAPC")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueueUserAPC");
     DWORD res = __wrapper_QueueUserAPC(pfnAPC, hThread, dwData);
     return res;
  }
#endif
  return Real_QueueUserAPC(pfnAPC, hThread, dwData);
}
BOOL (WINAPI * Real_QueueUserWorkItem)( LPTHREAD_START_ROUTINE Function, PVOID Context, ULONG Flags )
   = QueueUserWorkItem;

__declspec(dllexport) BOOL WINAPI Mine_QueueUserWorkItem( LPTHREAD_START_ROUTINE Function, PVOID Context, ULONG Flags ){
#ifdef WRAP_QueueUserWorkItem
  if(ChessWrapperSentry::Wrap("QueueUserWorkItem")){
     ChessWrapperSentry sentry;
     Chess::LogCall("QueueUserWorkItem");
     BOOL res = __wrapper_QueueUserWorkItem(Function, Context, Flags);
     return res;
  }
#endif
  return Real_QueueUserWorkItem(Function, Context, Flags);
}
void (WINAPI * Real_RaiseException)( DWORD dwExceptionCode, DWORD dwExceptionFlags, DWORD nNumberOfArguments, const ULONG_PTR *lpArguments )
  = RaiseException;

__declspec(dllexport) void WINAPI Mine_RaiseException( DWORD dwExceptionCode, DWORD dwExceptionFlags, DWORD nNumberOfArguments, const ULONG_PTR *lpArguments ){
  if(ChessWrapperSentry::Wrap("RaiseException")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RaiseException");
   }
  return Real_RaiseException(dwExceptionCode, dwExceptionFlags, nNumberOfArguments, lpArguments);
}
HANDLE (WINAPI * Real_ReOpenFile)( HANDLE hOriginalFile, DWORD dwDesiredAccess, DWORD dwShareMode, DWORD dwFlagsAndAttributes )
  = ReOpenFile;

__declspec(dllexport) HANDLE WINAPI Mine_ReOpenFile( HANDLE hOriginalFile, DWORD dwDesiredAccess, DWORD dwShareMode, DWORD dwFlagsAndAttributes ){
  if(ChessWrapperSentry::Wrap("ReOpenFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReOpenFile");
   }
  return Real_ReOpenFile(hOriginalFile, dwDesiredAccess, dwShareMode, dwFlagsAndAttributes);
}
BOOL (WINAPI * Real_ReadConsoleA)( HANDLE hConsoleInput, LPVOID lpBuffer, DWORD nNumberOfCharsToRead, LPDWORD lpNumberOfCharsRead, LPVOID lpReserved )
  = ReadConsoleA;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleA( HANDLE hConsoleInput, LPVOID lpBuffer, DWORD nNumberOfCharsToRead, LPDWORD lpNumberOfCharsRead, LPVOID lpReserved ){
  if(ChessWrapperSentry::Wrap("ReadConsoleA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleA");
   }
  return Real_ReadConsoleA(hConsoleInput, lpBuffer, nNumberOfCharsToRead, lpNumberOfCharsRead, lpReserved);
}
BOOL (WINAPI * Real_ReadConsoleInputA)( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead )
  = ReadConsoleInputA;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleInputA( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead ){
  if(ChessWrapperSentry::Wrap("ReadConsoleInputA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleInputA");
   }
  return Real_ReadConsoleInputA(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);
}
BOOL (WINAPI * Real_ReadConsoleInputW)( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead )
  = ReadConsoleInputW;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleInputW( HANDLE hConsoleInput, PINPUT_RECORD lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsRead ){
  if(ChessWrapperSentry::Wrap("ReadConsoleInputW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleInputW");
   }
  return Real_ReadConsoleInputW(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);
}
BOOL (WINAPI * Real_ReadConsoleOutputA)( HANDLE hConsoleOutput, PCHAR_INFO lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpReadRegion )
  = ReadConsoleOutputA;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleOutputA( HANDLE hConsoleOutput, PCHAR_INFO lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpReadRegion ){
  if(ChessWrapperSentry::Wrap("ReadConsoleOutputA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleOutputA");
   }
  return Real_ReadConsoleOutputA(hConsoleOutput, lpBuffer, dwBufferSize, dwBufferCoord, lpReadRegion);
}
BOOL (WINAPI * Real_ReadConsoleOutputAttribute)( HANDLE hConsoleOutput, LPWORD lpAttribute, DWORD nLength, COORD dwReadCoord, LPDWORD lpNumberOfAttrsRead )
  = ReadConsoleOutputAttribute;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleOutputAttribute( HANDLE hConsoleOutput, LPWORD lpAttribute, DWORD nLength, COORD dwReadCoord, LPDWORD lpNumberOfAttrsRead ){
  if(ChessWrapperSentry::Wrap("ReadConsoleOutputAttribute")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleOutputAttribute");
   }
  return Real_ReadConsoleOutputAttribute(hConsoleOutput, lpAttribute, nLength, dwReadCoord, lpNumberOfAttrsRead);
}
BOOL (WINAPI * Real_ReadConsoleOutputCharacterA)( HANDLE hConsoleOutput, LPSTR lpCharacter, DWORD nLength, COORD dwReadCoord, LPDWORD lpNumberOfCharsRead )
  = ReadConsoleOutputCharacterA;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleOutputCharacterA( HANDLE hConsoleOutput, LPSTR lpCharacter, DWORD nLength, COORD dwReadCoord, LPDWORD lpNumberOfCharsRead ){
  if(ChessWrapperSentry::Wrap("ReadConsoleOutputCharacterA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleOutputCharacterA");
   }
  return Real_ReadConsoleOutputCharacterA(hConsoleOutput, lpCharacter, nLength, dwReadCoord, lpNumberOfCharsRead);
}
BOOL (WINAPI * Real_ReadConsoleOutputCharacterW)( HANDLE hConsoleOutput, LPWSTR lpCharacter, DWORD nLength, COORD dwReadCoord, LPDWORD lpNumberOfCharsRead )
  = ReadConsoleOutputCharacterW;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleOutputCharacterW( HANDLE hConsoleOutput, LPWSTR lpCharacter, DWORD nLength, COORD dwReadCoord, LPDWORD lpNumberOfCharsRead ){
  if(ChessWrapperSentry::Wrap("ReadConsoleOutputCharacterW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleOutputCharacterW");
   }
  return Real_ReadConsoleOutputCharacterW(hConsoleOutput, lpCharacter, nLength, dwReadCoord, lpNumberOfCharsRead);
}
BOOL (WINAPI * Real_ReadConsoleOutputW)( HANDLE hConsoleOutput, PCHAR_INFO lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpReadRegion )
  = ReadConsoleOutputW;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleOutputW( HANDLE hConsoleOutput, PCHAR_INFO lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpReadRegion ){
  if(ChessWrapperSentry::Wrap("ReadConsoleOutputW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleOutputW");
   }
  return Real_ReadConsoleOutputW(hConsoleOutput, lpBuffer, dwBufferSize, dwBufferCoord, lpReadRegion);
}
BOOL (WINAPI * Real_ReadConsoleW)( HANDLE hConsoleInput, LPVOID lpBuffer, DWORD nNumberOfCharsToRead, LPDWORD lpNumberOfCharsRead, LPVOID lpReserved )
  = ReadConsoleW;

__declspec(dllexport) BOOL WINAPI Mine_ReadConsoleW( HANDLE hConsoleInput, LPVOID lpBuffer, DWORD nNumberOfCharsToRead, LPDWORD lpNumberOfCharsRead, LPVOID lpReserved ){
  if(ChessWrapperSentry::Wrap("ReadConsoleW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadConsoleW");
   }
  return Real_ReadConsoleW(hConsoleInput, lpBuffer, nNumberOfCharsToRead, lpNumberOfCharsRead, lpReserved);
}
BOOL (WINAPI * Real_ReadDirectoryChangesW)( HANDLE hDirectory, LPVOID lpBuffer, DWORD nBufferLength, BOOL bWatchSubtree, DWORD dwNotifyFilter, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
  = ReadDirectoryChangesW;

__declspec(dllexport) BOOL WINAPI Mine_ReadDirectoryChangesW( HANDLE hDirectory, LPVOID lpBuffer, DWORD nBufferLength, BOOL bWatchSubtree, DWORD dwNotifyFilter, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine ){
  if(ChessWrapperSentry::Wrap("ReadDirectoryChangesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadDirectoryChangesW");
   }
  return Real_ReadDirectoryChangesW(hDirectory, lpBuffer, nBufferLength, bWatchSubtree, dwNotifyFilter, lpBytesReturned, lpOverlapped, lpCompletionRoutine);
}
BOOL (WINAPI * Real_ReadFile)( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, LPOVERLAPPED lpOverlapped )
   = ReadFile;

__declspec(dllexport) BOOL WINAPI Mine_ReadFile( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, LPOVERLAPPED lpOverlapped ){
#ifdef WRAP_ReadFile
  if(ChessWrapperSentry::Wrap("ReadFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadFile");
     BOOL res = __wrapper_ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);
     return res;
  }
#endif
  return Real_ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);
}
BOOL (WINAPI * Real_ReadFileEx)( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
   = ReadFileEx;

__declspec(dllexport) BOOL WINAPI Mine_ReadFileEx( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine ){
#ifdef WRAP_ReadFileEx
  if(ChessWrapperSentry::Wrap("ReadFileEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadFileEx");
     BOOL res = __wrapper_ReadFileEx(hFile, lpBuffer, nNumberOfBytesToRead, lpOverlapped, lpCompletionRoutine);
     return res;
  }
#endif
  return Real_ReadFileEx(hFile, lpBuffer, nNumberOfBytesToRead, lpOverlapped, lpCompletionRoutine);
}
BOOL (WINAPI * Real_ReadFileScatter)( HANDLE hFile, FILE_SEGMENT_ELEMENT aSegmentArray[], DWORD nNumberOfBytesToRead, LPDWORD lpReserved, LPOVERLAPPED lpOverlapped )
  = ReadFileScatter;

__declspec(dllexport) BOOL WINAPI Mine_ReadFileScatter( HANDLE hFile, FILE_SEGMENT_ELEMENT aSegmentArray[], DWORD nNumberOfBytesToRead, LPDWORD lpReserved, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("ReadFileScatter")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadFileScatter");
   }
  return Real_ReadFileScatter(hFile, aSegmentArray, nNumberOfBytesToRead, lpReserved, lpOverlapped);
}
BOOL (WINAPI * Real_ReadProcessMemory)( HANDLE hProcess, LPCVOID lpBaseAddress, LPVOID lpBuffer, SIZE_T nSize, SIZE_T * lpNumberOfBytesRead )
  = ReadProcessMemory;

__declspec(dllexport) BOOL WINAPI Mine_ReadProcessMemory( HANDLE hProcess, LPCVOID lpBaseAddress, LPVOID lpBuffer, SIZE_T nSize, SIZE_T * lpNumberOfBytesRead ){
  if(ChessWrapperSentry::Wrap("ReadProcessMemory")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReadProcessMemory");
   }
  return Real_ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesRead);
}
BOOL (WINAPI * Real_RegisterWaitForSingleObject)( PHANDLE phNewWaitObject, HANDLE hObject, WAITORTIMERCALLBACK Callback, PVOID Context, ULONG dwMilliseconds, ULONG dwFlags )
  = RegisterWaitForSingleObject;

__declspec(dllexport) BOOL WINAPI Mine_RegisterWaitForSingleObject( PHANDLE phNewWaitObject, HANDLE hObject, WAITORTIMERCALLBACK Callback, PVOID Context, ULONG dwMilliseconds, ULONG dwFlags ){
  if(ChessWrapperSentry::Wrap("RegisterWaitForSingleObject")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RegisterWaitForSingleObject");
   }
  return Real_RegisterWaitForSingleObject(phNewWaitObject, hObject, Callback, Context, dwMilliseconds, dwFlags);
}
HANDLE (WINAPI * Real_RegisterWaitForSingleObjectEx)( HANDLE hObject, WAITORTIMERCALLBACK Callback, PVOID Context, ULONG dwMilliseconds, ULONG dwFlags )
  = RegisterWaitForSingleObjectEx;

__declspec(dllexport) HANDLE WINAPI Mine_RegisterWaitForSingleObjectEx( HANDLE hObject, WAITORTIMERCALLBACK Callback, PVOID Context, ULONG dwMilliseconds, ULONG dwFlags ){
  if(ChessWrapperSentry::Wrap("RegisterWaitForSingleObjectEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RegisterWaitForSingleObjectEx");
   }
  return Real_RegisterWaitForSingleObjectEx(hObject, Callback, Context, dwMilliseconds, dwFlags);
}
void (WINAPI * Real_ReleaseActCtx)( HANDLE hActCtx )
  = ReleaseActCtx;

__declspec(dllexport) void WINAPI Mine_ReleaseActCtx( HANDLE hActCtx ){
  if(ChessWrapperSentry::Wrap("ReleaseActCtx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReleaseActCtx");
   }
  return Real_ReleaseActCtx(hActCtx);
}
BOOL (WINAPI * Real_ReleaseMutex)( HANDLE hMutex )
   = ReleaseMutex;

__declspec(dllexport) BOOL WINAPI Mine_ReleaseMutex( HANDLE hMutex ){
#ifdef WRAP_ReleaseMutex
  if(ChessWrapperSentry::Wrap("ReleaseMutex")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReleaseMutex");
     BOOL res = __wrapper_ReleaseMutex(hMutex);
     return res;
  }
#endif
  return Real_ReleaseMutex(hMutex);
}
BOOL (WINAPI * Real_ReleaseSemaphore)( HANDLE hSemaphore, LONG lReleaseCount, LPLONG lpPreviousCount )
   = ReleaseSemaphore;

__declspec(dllexport) BOOL WINAPI Mine_ReleaseSemaphore( HANDLE hSemaphore, LONG lReleaseCount, LPLONG lpPreviousCount ){
#ifdef WRAP_ReleaseSemaphore
  if(ChessWrapperSentry::Wrap("ReleaseSemaphore")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReleaseSemaphore");
     BOOL res = __wrapper_ReleaseSemaphore(hSemaphore, lReleaseCount, lpPreviousCount);
     return res;
  }
#endif
  return Real_ReleaseSemaphore(hSemaphore, lReleaseCount, lpPreviousCount);
}
BOOL (WINAPI * Real_RemoveDirectoryA)( LPCSTR lpPathName )
  = RemoveDirectoryA;

__declspec(dllexport) BOOL WINAPI Mine_RemoveDirectoryA( LPCSTR lpPathName ){
  if(ChessWrapperSentry::Wrap("RemoveDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RemoveDirectoryA");
   }
  return Real_RemoveDirectoryA(lpPathName);
}
BOOL (WINAPI * Real_RemoveDirectoryW)( LPCWSTR lpPathName )
  = RemoveDirectoryW;

__declspec(dllexport) BOOL WINAPI Mine_RemoveDirectoryW( LPCWSTR lpPathName ){
  if(ChessWrapperSentry::Wrap("RemoveDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RemoveDirectoryW");
   }
  return Real_RemoveDirectoryW(lpPathName);
}
ULONG (WINAPI * Real_RemoveVectoredContinueHandler)( PVOID Handle )
  = RemoveVectoredContinueHandler;

__declspec(dllexport) ULONG WINAPI Mine_RemoveVectoredContinueHandler( PVOID Handle ){
  if(ChessWrapperSentry::Wrap("RemoveVectoredContinueHandler")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RemoveVectoredContinueHandler");
   }
  return Real_RemoveVectoredContinueHandler(Handle);
}
ULONG (WINAPI * Real_RemoveVectoredExceptionHandler)( PVOID Handle )
  = RemoveVectoredExceptionHandler;

__declspec(dllexport) ULONG WINAPI Mine_RemoveVectoredExceptionHandler( PVOID Handle ){
  if(ChessWrapperSentry::Wrap("RemoveVectoredExceptionHandler")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RemoveVectoredExceptionHandler");
   }
  return Real_RemoveVectoredExceptionHandler(Handle);
}
BOOL (WINAPI * Real_ReplaceFileA)( LPCSTR lpReplacedFileName, LPCSTR lpReplacementFileName, LPCSTR lpBackupFileName, DWORD dwReplaceFlags, LPVOID lpExclude, LPVOID lpReserved )
  = ReplaceFileA;

__declspec(dllexport) BOOL WINAPI Mine_ReplaceFileA( LPCSTR lpReplacedFileName, LPCSTR lpReplacementFileName, LPCSTR lpBackupFileName, DWORD dwReplaceFlags, LPVOID lpExclude, LPVOID lpReserved ){
  if(ChessWrapperSentry::Wrap("ReplaceFileA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReplaceFileA");
   }
  return Real_ReplaceFileA(lpReplacedFileName, lpReplacementFileName, lpBackupFileName, dwReplaceFlags, lpExclude, lpReserved);
}
BOOL (WINAPI * Real_ReplaceFileW)( LPCWSTR lpReplacedFileName, LPCWSTR lpReplacementFileName, LPCWSTR lpBackupFileName, DWORD dwReplaceFlags, LPVOID lpExclude, LPVOID lpReserved )
  = ReplaceFileW;

__declspec(dllexport) BOOL WINAPI Mine_ReplaceFileW( LPCWSTR lpReplacedFileName, LPCWSTR lpReplacementFileName, LPCWSTR lpBackupFileName, DWORD dwReplaceFlags, LPVOID lpExclude, LPVOID lpReserved ){
  if(ChessWrapperSentry::Wrap("ReplaceFileW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ReplaceFileW");
   }
  return Real_ReplaceFileW(lpReplacedFileName, lpReplacementFileName, lpBackupFileName, dwReplaceFlags, lpExclude, lpReserved);
}
BOOL (WINAPI * Real_RequestDeviceWakeup)( HANDLE hDevice )
  = RequestDeviceWakeup;

__declspec(dllexport) BOOL WINAPI Mine_RequestDeviceWakeup( HANDLE hDevice ){
  if(ChessWrapperSentry::Wrap("RequestDeviceWakeup")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RequestDeviceWakeup");
   }
  return Real_RequestDeviceWakeup(hDevice);
}
BOOL (WINAPI * Real_RequestWakeupLatency)( LATENCY_TIME latency )
  = RequestWakeupLatency;

__declspec(dllexport) BOOL WINAPI Mine_RequestWakeupLatency( LATENCY_TIME latency ){
  if(ChessWrapperSentry::Wrap("RequestWakeupLatency")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RequestWakeupLatency");
   }
  return Real_RequestWakeupLatency(latency);
}
BOOL (WINAPI * Real_ResetEvent)( HANDLE hEvent )
   = ResetEvent;

__declspec(dllexport) BOOL WINAPI Mine_ResetEvent( HANDLE hEvent ){
#ifdef WRAP_ResetEvent
  if(ChessWrapperSentry::Wrap("ResetEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ResetEvent");
     BOOL res = __wrapper_ResetEvent(hEvent);
     return res;
  }
#endif
  return Real_ResetEvent(hEvent);
}
UINT (WINAPI * Real_ResetWriteWatch)( LPVOID lpBaseAddress, SIZE_T dwRegionSize )
  = ResetWriteWatch;

__declspec(dllexport) UINT WINAPI Mine_ResetWriteWatch( LPVOID lpBaseAddress, SIZE_T dwRegionSize ){
  if(ChessWrapperSentry::Wrap("ResetWriteWatch")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ResetWriteWatch");
   }
  return Real_ResetWriteWatch(lpBaseAddress, dwRegionSize);
}
DWORD (WINAPI * Real_ResumeThread)( HANDLE hThread )
   = ResumeThread;

__declspec(dllexport) DWORD WINAPI Mine_ResumeThread( HANDLE hThread ){
#ifdef WRAP_ResumeThread
  if(ChessWrapperSentry::Wrap("ResumeThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ResumeThread");
     DWORD res = __wrapper_ResumeThread(hThread);
     return res;
  }
#endif
  return Real_ResumeThread(hThread);
}
void (WINAPI * Real_RtlCaptureContext)( PCONTEXT ContextRecord )
  = RtlCaptureContext;

__declspec(dllexport) void WINAPI Mine_RtlCaptureContext( PCONTEXT ContextRecord ){
  if(ChessWrapperSentry::Wrap("RtlCaptureContext")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RtlCaptureContext");
   }
  return Real_RtlCaptureContext(ContextRecord);
}
void (WINAPI * Real_RtlUnwind)( PVOID TargetFrame, PVOID TargetIp, PEXCEPTION_RECORD ExceptionRecord, PVOID ReturnValue )
  = RtlUnwind;

__declspec(dllexport) void WINAPI Mine_RtlUnwind( PVOID TargetFrame, PVOID TargetIp, PEXCEPTION_RECORD ExceptionRecord, PVOID ReturnValue ){
  if(ChessWrapperSentry::Wrap("RtlUnwind")){
     ChessWrapperSentry sentry;
     Chess::LogCall("RtlUnwind");
   }
  return Real_RtlUnwind(TargetFrame, TargetIp, ExceptionRecord, ReturnValue);
}
BOOL (WINAPI * Real_ScrollConsoleScreenBufferA)( HANDLE hConsoleOutput, const SMALL_RECT *lpScrollRectangle, const SMALL_RECT *lpClipRectangle, COORD dwDestinationOrigin, const CHAR_INFO *lpFill )
  = ScrollConsoleScreenBufferA;

__declspec(dllexport) BOOL WINAPI Mine_ScrollConsoleScreenBufferA( HANDLE hConsoleOutput, const SMALL_RECT *lpScrollRectangle, const SMALL_RECT *lpClipRectangle, COORD dwDestinationOrigin, const CHAR_INFO *lpFill ){
  if(ChessWrapperSentry::Wrap("ScrollConsoleScreenBufferA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ScrollConsoleScreenBufferA");
   }
  return Real_ScrollConsoleScreenBufferA(hConsoleOutput, lpScrollRectangle, lpClipRectangle, dwDestinationOrigin, lpFill);
}
BOOL (WINAPI * Real_ScrollConsoleScreenBufferW)( HANDLE hConsoleOutput, const SMALL_RECT *lpScrollRectangle, const SMALL_RECT *lpClipRectangle, COORD dwDestinationOrigin, const CHAR_INFO *lpFill )
  = ScrollConsoleScreenBufferW;

__declspec(dllexport) BOOL WINAPI Mine_ScrollConsoleScreenBufferW( HANDLE hConsoleOutput, const SMALL_RECT *lpScrollRectangle, const SMALL_RECT *lpClipRectangle, COORD dwDestinationOrigin, const CHAR_INFO *lpFill ){
  if(ChessWrapperSentry::Wrap("ScrollConsoleScreenBufferW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ScrollConsoleScreenBufferW");
   }
  return Real_ScrollConsoleScreenBufferW(hConsoleOutput, lpScrollRectangle, lpClipRectangle, dwDestinationOrigin, lpFill);
}
DWORD (WINAPI * Real_SearchPathA)( LPCSTR lpPath, LPCSTR lpFileName, LPCSTR lpExtension, DWORD nBufferLength, LPSTR lpBuffer, LPSTR *lpFilePart )
  = SearchPathA;

__declspec(dllexport) DWORD WINAPI Mine_SearchPathA( LPCSTR lpPath, LPCSTR lpFileName, LPCSTR lpExtension, DWORD nBufferLength, LPSTR lpBuffer, LPSTR *lpFilePart ){
  if(ChessWrapperSentry::Wrap("SearchPathA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SearchPathA");
   }
  return Real_SearchPathA(lpPath, lpFileName, lpExtension, nBufferLength, lpBuffer, lpFilePart);
}
DWORD (WINAPI * Real_SearchPathW)( LPCWSTR lpPath, LPCWSTR lpFileName, LPCWSTR lpExtension, DWORD nBufferLength, LPWSTR lpBuffer, LPWSTR *lpFilePart )
  = SearchPathW;

__declspec(dllexport) DWORD WINAPI Mine_SearchPathW( LPCWSTR lpPath, LPCWSTR lpFileName, LPCWSTR lpExtension, DWORD nBufferLength, LPWSTR lpBuffer, LPWSTR *lpFilePart ){
  if(ChessWrapperSentry::Wrap("SearchPathW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SearchPathW");
   }
  return Real_SearchPathW(lpPath, lpFileName, lpExtension, nBufferLength, lpBuffer, lpFilePart);
}
BOOL (WINAPI * Real_SetCalendarInfoA)( LCID Locale, CALID Calendar, CALTYPE CalType, LPCSTR lpCalData)
  = SetCalendarInfoA;

__declspec(dllexport) BOOL WINAPI Mine_SetCalendarInfoA( LCID Locale, CALID Calendar, CALTYPE CalType, LPCSTR lpCalData){
  if(ChessWrapperSentry::Wrap("SetCalendarInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCalendarInfoA");
   }
  return Real_SetCalendarInfoA(Locale, Calendar, CalType, lpCalData);
}
BOOL (WINAPI * Real_SetCalendarInfoW)( LCID Locale, CALID Calendar, CALTYPE CalType, LPCWSTR lpCalData)
  = SetCalendarInfoW;

__declspec(dllexport) BOOL WINAPI Mine_SetCalendarInfoW( LCID Locale, CALID Calendar, CALTYPE CalType, LPCWSTR lpCalData){
  if(ChessWrapperSentry::Wrap("SetCalendarInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCalendarInfoW");
   }
  return Real_SetCalendarInfoW(Locale, Calendar, CalType, lpCalData);
}
BOOL (WINAPI * Real_SetCommBreak)( HANDLE hFile )
  = SetCommBreak;

__declspec(dllexport) BOOL WINAPI Mine_SetCommBreak( HANDLE hFile ){
  if(ChessWrapperSentry::Wrap("SetCommBreak")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCommBreak");
   }
  return Real_SetCommBreak(hFile);
}
BOOL (WINAPI * Real_SetCommConfig)( HANDLE hCommDev, LPCOMMCONFIG lpCC, DWORD dwSize )
  = SetCommConfig;

__declspec(dllexport) BOOL WINAPI Mine_SetCommConfig( HANDLE hCommDev, LPCOMMCONFIG lpCC, DWORD dwSize ){
  if(ChessWrapperSentry::Wrap("SetCommConfig")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCommConfig");
   }
  return Real_SetCommConfig(hCommDev, lpCC, dwSize);
}
BOOL (WINAPI * Real_SetCommMask)( HANDLE hFile, DWORD dwEvtMask )
  = SetCommMask;

__declspec(dllexport) BOOL WINAPI Mine_SetCommMask( HANDLE hFile, DWORD dwEvtMask ){
  if(ChessWrapperSentry::Wrap("SetCommMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCommMask");
   }
  return Real_SetCommMask(hFile, dwEvtMask);
}
BOOL (WINAPI * Real_SetCommState)( HANDLE hFile, LPDCB lpDCB )
  = SetCommState;

__declspec(dllexport) BOOL WINAPI Mine_SetCommState( HANDLE hFile, LPDCB lpDCB ){
  if(ChessWrapperSentry::Wrap("SetCommState")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCommState");
   }
  return Real_SetCommState(hFile, lpDCB);
}
BOOL (WINAPI * Real_SetCommTimeouts)( HANDLE hFile, LPCOMMTIMEOUTS lpCommTimeouts )
  = SetCommTimeouts;

__declspec(dllexport) BOOL WINAPI Mine_SetCommTimeouts( HANDLE hFile, LPCOMMTIMEOUTS lpCommTimeouts ){
  if(ChessWrapperSentry::Wrap("SetCommTimeouts")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCommTimeouts");
   }
  return Real_SetCommTimeouts(hFile, lpCommTimeouts);
}
BOOL (WINAPI * Real_SetComputerNameA)( LPCSTR lpComputerName )
  = SetComputerNameA;

__declspec(dllexport) BOOL WINAPI Mine_SetComputerNameA( LPCSTR lpComputerName ){
  if(ChessWrapperSentry::Wrap("SetComputerNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetComputerNameA");
   }
  return Real_SetComputerNameA(lpComputerName);
}
BOOL (WINAPI * Real_SetComputerNameExA)( COMPUTER_NAME_FORMAT NameType, LPCSTR lpBuffer )
  = SetComputerNameExA;

__declspec(dllexport) BOOL WINAPI Mine_SetComputerNameExA( COMPUTER_NAME_FORMAT NameType, LPCSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("SetComputerNameExA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetComputerNameExA");
   }
  return Real_SetComputerNameExA(NameType, lpBuffer);
}
BOOL (WINAPI * Real_SetComputerNameExW)( COMPUTER_NAME_FORMAT NameType, LPCWSTR lpBuffer )
  = SetComputerNameExW;

__declspec(dllexport) BOOL WINAPI Mine_SetComputerNameExW( COMPUTER_NAME_FORMAT NameType, LPCWSTR lpBuffer ){
  if(ChessWrapperSentry::Wrap("SetComputerNameExW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetComputerNameExW");
   }
  return Real_SetComputerNameExW(NameType, lpBuffer);
}
BOOL (WINAPI * Real_SetComputerNameW)( LPCWSTR lpComputerName )
  = SetComputerNameW;

__declspec(dllexport) BOOL WINAPI Mine_SetComputerNameW( LPCWSTR lpComputerName ){
  if(ChessWrapperSentry::Wrap("SetComputerNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetComputerNameW");
   }
  return Real_SetComputerNameW(lpComputerName);
}
BOOL (WINAPI * Real_SetConsoleActiveScreenBuffer)( HANDLE hConsoleOutput )
  = SetConsoleActiveScreenBuffer;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleActiveScreenBuffer( HANDLE hConsoleOutput ){
  if(ChessWrapperSentry::Wrap("SetConsoleActiveScreenBuffer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleActiveScreenBuffer");
   }
  return Real_SetConsoleActiveScreenBuffer(hConsoleOutput);
}
BOOL (WINAPI * Real_SetConsoleCP)( UINT wCodePageID )
  = SetConsoleCP;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleCP( UINT wCodePageID ){
  if(ChessWrapperSentry::Wrap("SetConsoleCP")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleCP");
   }
  return Real_SetConsoleCP(wCodePageID);
}
BOOL (WINAPI * Real_SetConsoleCtrlHandler)( PHANDLER_ROUTINE HandlerRoutine, BOOL Add )
  = SetConsoleCtrlHandler;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleCtrlHandler( PHANDLER_ROUTINE HandlerRoutine, BOOL Add ){
  if(ChessWrapperSentry::Wrap("SetConsoleCtrlHandler")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleCtrlHandler");
   }
  return Real_SetConsoleCtrlHandler(HandlerRoutine, Add);
}
BOOL (WINAPI * Real_SetConsoleCursorInfo)( HANDLE hConsoleOutput, const CONSOLE_CURSOR_INFO *lpConsoleCursorInfo )
  = SetConsoleCursorInfo;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleCursorInfo( HANDLE hConsoleOutput, const CONSOLE_CURSOR_INFO *lpConsoleCursorInfo ){
  if(ChessWrapperSentry::Wrap("SetConsoleCursorInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleCursorInfo");
   }
  return Real_SetConsoleCursorInfo(hConsoleOutput, lpConsoleCursorInfo);
}
BOOL (WINAPI * Real_SetConsoleCursorPosition)( HANDLE hConsoleOutput, COORD dwCursorPosition )
  = SetConsoleCursorPosition;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleCursorPosition( HANDLE hConsoleOutput, COORD dwCursorPosition ){
  if(ChessWrapperSentry::Wrap("SetConsoleCursorPosition")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleCursorPosition");
   }
  return Real_SetConsoleCursorPosition(hConsoleOutput, dwCursorPosition);
}
BOOL (WINAPI * Real_SetConsoleMode)( HANDLE hConsoleHandle, DWORD dwMode )
  = SetConsoleMode;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleMode( HANDLE hConsoleHandle, DWORD dwMode ){
  if(ChessWrapperSentry::Wrap("SetConsoleMode")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleMode");
   }
  return Real_SetConsoleMode(hConsoleHandle, dwMode);
}
BOOL (WINAPI * Real_SetConsoleOutputCP)( UINT wCodePageID )
  = SetConsoleOutputCP;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleOutputCP( UINT wCodePageID ){
  if(ChessWrapperSentry::Wrap("SetConsoleOutputCP")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleOutputCP");
   }
  return Real_SetConsoleOutputCP(wCodePageID);
}
BOOL (WINAPI * Real_SetConsoleScreenBufferSize)( HANDLE hConsoleOutput, COORD dwSize )
  = SetConsoleScreenBufferSize;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleScreenBufferSize( HANDLE hConsoleOutput, COORD dwSize ){
  if(ChessWrapperSentry::Wrap("SetConsoleScreenBufferSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleScreenBufferSize");
   }
  return Real_SetConsoleScreenBufferSize(hConsoleOutput, dwSize);
}
BOOL (WINAPI * Real_SetConsoleTextAttribute)( HANDLE hConsoleOutput, WORD wAttributes )
  = SetConsoleTextAttribute;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleTextAttribute( HANDLE hConsoleOutput, WORD wAttributes ){
  if(ChessWrapperSentry::Wrap("SetConsoleTextAttribute")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleTextAttribute");
   }
  return Real_SetConsoleTextAttribute(hConsoleOutput, wAttributes);
}
BOOL (WINAPI * Real_SetConsoleTitleA)( LPCSTR lpConsoleTitle )
  = SetConsoleTitleA;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleTitleA( LPCSTR lpConsoleTitle ){
  if(ChessWrapperSentry::Wrap("SetConsoleTitleA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleTitleA");
   }
  return Real_SetConsoleTitleA(lpConsoleTitle);
}
BOOL (WINAPI * Real_SetConsoleTitleW)( LPCWSTR lpConsoleTitle )
  = SetConsoleTitleW;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleTitleW( LPCWSTR lpConsoleTitle ){
  if(ChessWrapperSentry::Wrap("SetConsoleTitleW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleTitleW");
   }
  return Real_SetConsoleTitleW(lpConsoleTitle);
}
BOOL (WINAPI * Real_SetConsoleWindowInfo)( HANDLE hConsoleOutput, BOOL bAbsolute, const SMALL_RECT *lpConsoleWindow )
  = SetConsoleWindowInfo;

__declspec(dllexport) BOOL WINAPI Mine_SetConsoleWindowInfo( HANDLE hConsoleOutput, BOOL bAbsolute, const SMALL_RECT *lpConsoleWindow ){
  if(ChessWrapperSentry::Wrap("SetConsoleWindowInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetConsoleWindowInfo");
   }
  return Real_SetConsoleWindowInfo(hConsoleOutput, bAbsolute, lpConsoleWindow);
}
DWORD (WINAPI * Real_SetCriticalSectionSpinCount)( LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount )
  = SetCriticalSectionSpinCount;

__declspec(dllexport) DWORD WINAPI Mine_SetCriticalSectionSpinCount( LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount ){
  if(ChessWrapperSentry::Wrap("SetCriticalSectionSpinCount")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCriticalSectionSpinCount");
   }
  return Real_SetCriticalSectionSpinCount(lpCriticalSection, dwSpinCount);
}
BOOL (WINAPI * Real_SetCurrentDirectoryA)( LPCSTR lpPathName )
  = SetCurrentDirectoryA;

__declspec(dllexport) BOOL WINAPI Mine_SetCurrentDirectoryA( LPCSTR lpPathName ){
  if(ChessWrapperSentry::Wrap("SetCurrentDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCurrentDirectoryA");
   }
  return Real_SetCurrentDirectoryA(lpPathName);
}
BOOL (WINAPI * Real_SetCurrentDirectoryW)( LPCWSTR lpPathName )
  = SetCurrentDirectoryW;

__declspec(dllexport) BOOL WINAPI Mine_SetCurrentDirectoryW( LPCWSTR lpPathName ){
  if(ChessWrapperSentry::Wrap("SetCurrentDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetCurrentDirectoryW");
   }
  return Real_SetCurrentDirectoryW(lpPathName);
}
BOOL (WINAPI * Real_SetDefaultCommConfigA)( LPCSTR lpszName, LPCOMMCONFIG lpCC, DWORD dwSize )
  = SetDefaultCommConfigA;

__declspec(dllexport) BOOL WINAPI Mine_SetDefaultCommConfigA( LPCSTR lpszName, LPCOMMCONFIG lpCC, DWORD dwSize ){
  if(ChessWrapperSentry::Wrap("SetDefaultCommConfigA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetDefaultCommConfigA");
   }
  return Real_SetDefaultCommConfigA(lpszName, lpCC, dwSize);
}
BOOL (WINAPI * Real_SetDefaultCommConfigW)( LPCWSTR lpszName, LPCOMMCONFIG lpCC, DWORD dwSize )
  = SetDefaultCommConfigW;

__declspec(dllexport) BOOL WINAPI Mine_SetDefaultCommConfigW( LPCWSTR lpszName, LPCOMMCONFIG lpCC, DWORD dwSize ){
  if(ChessWrapperSentry::Wrap("SetDefaultCommConfigW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetDefaultCommConfigW");
   }
  return Real_SetDefaultCommConfigW(lpszName, lpCC, dwSize);
}
BOOL (WINAPI * Real_SetDllDirectoryA)( LPCSTR lpPathName )
  = SetDllDirectoryA;

__declspec(dllexport) BOOL WINAPI Mine_SetDllDirectoryA( LPCSTR lpPathName ){
  if(ChessWrapperSentry::Wrap("SetDllDirectoryA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetDllDirectoryA");
   }
  return Real_SetDllDirectoryA(lpPathName);
}
BOOL (WINAPI * Real_SetDllDirectoryW)( LPCWSTR lpPathName )
  = SetDllDirectoryW;

__declspec(dllexport) BOOL WINAPI Mine_SetDllDirectoryW( LPCWSTR lpPathName ){
  if(ChessWrapperSentry::Wrap("SetDllDirectoryW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetDllDirectoryW");
   }
  return Real_SetDllDirectoryW(lpPathName);
}
BOOL (WINAPI * Real_SetEndOfFile)( HANDLE hFile )
  = SetEndOfFile;

__declspec(dllexport) BOOL WINAPI Mine_SetEndOfFile( HANDLE hFile ){
  if(ChessWrapperSentry::Wrap("SetEndOfFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetEndOfFile");
   }
  return Real_SetEndOfFile(hFile);
}
BOOL (WINAPI * Real_SetEnvironmentStringsA)( LPCH NewEnvironment )
  = SetEnvironmentStringsA;

__declspec(dllexport) BOOL WINAPI Mine_SetEnvironmentStringsA( LPCH NewEnvironment ){
  if(ChessWrapperSentry::Wrap("SetEnvironmentStringsA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetEnvironmentStringsA");
   }
  return Real_SetEnvironmentStringsA(NewEnvironment);
}
BOOL (WINAPI * Real_SetEnvironmentStringsW)( LPWCH NewEnvironment )
  = SetEnvironmentStringsW;

__declspec(dllexport) BOOL WINAPI Mine_SetEnvironmentStringsW( LPWCH NewEnvironment ){
  if(ChessWrapperSentry::Wrap("SetEnvironmentStringsW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetEnvironmentStringsW");
   }
  return Real_SetEnvironmentStringsW(NewEnvironment);
}
BOOL (WINAPI * Real_SetEnvironmentVariableA)( LPCSTR lpName, LPCSTR lpValue )
  = SetEnvironmentVariableA;

__declspec(dllexport) BOOL WINAPI Mine_SetEnvironmentVariableA( LPCSTR lpName, LPCSTR lpValue ){
  if(ChessWrapperSentry::Wrap("SetEnvironmentVariableA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetEnvironmentVariableA");
   }
  return Real_SetEnvironmentVariableA(lpName, lpValue);
}
BOOL (WINAPI * Real_SetEnvironmentVariableW)( LPCWSTR lpName, LPCWSTR lpValue )
  = SetEnvironmentVariableW;

__declspec(dllexport) BOOL WINAPI Mine_SetEnvironmentVariableW( LPCWSTR lpName, LPCWSTR lpValue ){
  if(ChessWrapperSentry::Wrap("SetEnvironmentVariableW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetEnvironmentVariableW");
   }
  return Real_SetEnvironmentVariableW(lpName, lpValue);
}
UINT (WINAPI * Real_SetErrorMode)( UINT uMode )
  = SetErrorMode;

__declspec(dllexport) UINT WINAPI Mine_SetErrorMode( UINT uMode ){
  if(ChessWrapperSentry::Wrap("SetErrorMode")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetErrorMode");
   }
  return Real_SetErrorMode(uMode);
}
BOOL (WINAPI * Real_SetEvent)( HANDLE hEvent )
   = SetEvent;

__declspec(dllexport) BOOL WINAPI Mine_SetEvent( HANDLE hEvent ){
#ifdef WRAP_SetEvent
  if(ChessWrapperSentry::Wrap("SetEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetEvent");
     BOOL res = __wrapper_SetEvent(hEvent);
     return res;
  }
#endif
  return Real_SetEvent(hEvent);
}
void (WINAPI * Real_SetFileApisToANSI)( void )
  = SetFileApisToANSI;

__declspec(dllexport) void WINAPI Mine_SetFileApisToANSI( void ){
  if(ChessWrapperSentry::Wrap("SetFileApisToANSI")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileApisToANSI");
   }
  return Real_SetFileApisToANSI();
}
void (WINAPI * Real_SetFileApisToOEM)( void )
  = SetFileApisToOEM;

__declspec(dllexport) void WINAPI Mine_SetFileApisToOEM( void ){
  if(ChessWrapperSentry::Wrap("SetFileApisToOEM")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileApisToOEM");
   }
  return Real_SetFileApisToOEM();
}
BOOL (WINAPI * Real_SetFileAttributesA)( LPCSTR lpFileName, DWORD dwFileAttributes )
  = SetFileAttributesA;

__declspec(dllexport) BOOL WINAPI Mine_SetFileAttributesA( LPCSTR lpFileName, DWORD dwFileAttributes ){
  if(ChessWrapperSentry::Wrap("SetFileAttributesA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileAttributesA");
   }
  return Real_SetFileAttributesA(lpFileName, dwFileAttributes);
}
BOOL (WINAPI * Real_SetFileAttributesW)( LPCWSTR lpFileName, DWORD dwFileAttributes )
  = SetFileAttributesW;

__declspec(dllexport) BOOL WINAPI Mine_SetFileAttributesW( LPCWSTR lpFileName, DWORD dwFileAttributes ){
  if(ChessWrapperSentry::Wrap("SetFileAttributesW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileAttributesW");
   }
  return Real_SetFileAttributesW(lpFileName, dwFileAttributes);
}
DWORD (WINAPI * Real_SetFilePointer)( HANDLE hFile, LONG lDistanceToMove, PLONG lpDistanceToMoveHigh, DWORD dwMoveMethod )
  = SetFilePointer;

__declspec(dllexport) DWORD WINAPI Mine_SetFilePointer( HANDLE hFile, LONG lDistanceToMove, PLONG lpDistanceToMoveHigh, DWORD dwMoveMethod ){
  if(ChessWrapperSentry::Wrap("SetFilePointer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFilePointer");
   }
  return Real_SetFilePointer(hFile, lDistanceToMove, lpDistanceToMoveHigh, dwMoveMethod);
}
BOOL (WINAPI * Real_SetFilePointerEx)( HANDLE hFile, LARGE_INTEGER liDistanceToMove, PLARGE_INTEGER lpNewFilePointer, DWORD dwMoveMethod )
  = SetFilePointerEx;

__declspec(dllexport) BOOL WINAPI Mine_SetFilePointerEx( HANDLE hFile, LARGE_INTEGER liDistanceToMove, PLARGE_INTEGER lpNewFilePointer, DWORD dwMoveMethod ){
  if(ChessWrapperSentry::Wrap("SetFilePointerEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFilePointerEx");
   }
  return Real_SetFilePointerEx(hFile, liDistanceToMove, lpNewFilePointer, dwMoveMethod);
}
BOOL (WINAPI * Real_SetFileShortNameA)( HANDLE hFile, LPCSTR lpShortName )
  = SetFileShortNameA;

__declspec(dllexport) BOOL WINAPI Mine_SetFileShortNameA( HANDLE hFile, LPCSTR lpShortName ){
  if(ChessWrapperSentry::Wrap("SetFileShortNameA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileShortNameA");
   }
  return Real_SetFileShortNameA(hFile, lpShortName);
}
BOOL (WINAPI * Real_SetFileShortNameW)( HANDLE hFile, LPCWSTR lpShortName )
  = SetFileShortNameW;

__declspec(dllexport) BOOL WINAPI Mine_SetFileShortNameW( HANDLE hFile, LPCWSTR lpShortName ){
  if(ChessWrapperSentry::Wrap("SetFileShortNameW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileShortNameW");
   }
  return Real_SetFileShortNameW(hFile, lpShortName);
}
BOOL (WINAPI * Real_SetFileTime)( HANDLE hFile, const FILETIME *lpCreationTime, const FILETIME *lpLastAccessTime, const FILETIME *lpLastWriteTime )
  = SetFileTime;

__declspec(dllexport) BOOL WINAPI Mine_SetFileTime( HANDLE hFile, const FILETIME *lpCreationTime, const FILETIME *lpLastAccessTime, const FILETIME *lpLastWriteTime ){
  if(ChessWrapperSentry::Wrap("SetFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileTime");
   }
  return Real_SetFileTime(hFile, lpCreationTime, lpLastAccessTime, lpLastWriteTime);
}
BOOL (WINAPI * Real_SetFileValidData)( HANDLE hFile, LONGLONG ValidDataLength )
  = SetFileValidData;

__declspec(dllexport) BOOL WINAPI Mine_SetFileValidData( HANDLE hFile, LONGLONG ValidDataLength ){
  if(ChessWrapperSentry::Wrap("SetFileValidData")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFileValidData");
   }
  return Real_SetFileValidData(hFile, ValidDataLength);
}
BOOL (WINAPI * Real_SetFirmwareEnvironmentVariableA)( LPCSTR lpName, LPCSTR lpGuid, PVOID pValue, DWORD nSize )
  = SetFirmwareEnvironmentVariableA;

__declspec(dllexport) BOOL WINAPI Mine_SetFirmwareEnvironmentVariableA( LPCSTR lpName, LPCSTR lpGuid, PVOID pValue, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("SetFirmwareEnvironmentVariableA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFirmwareEnvironmentVariableA");
   }
  return Real_SetFirmwareEnvironmentVariableA(lpName, lpGuid, pValue, nSize);
}
BOOL (WINAPI * Real_SetFirmwareEnvironmentVariableW)( LPCWSTR lpName, LPCWSTR lpGuid, PVOID pValue, DWORD nSize )
  = SetFirmwareEnvironmentVariableW;

__declspec(dllexport) BOOL WINAPI Mine_SetFirmwareEnvironmentVariableW( LPCWSTR lpName, LPCWSTR lpGuid, PVOID pValue, DWORD nSize ){
  if(ChessWrapperSentry::Wrap("SetFirmwareEnvironmentVariableW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetFirmwareEnvironmentVariableW");
   }
  return Real_SetFirmwareEnvironmentVariableW(lpName, lpGuid, pValue, nSize);
}
UINT (WINAPI * Real_SetHandleCount)( UINT uNumber )
  = SetHandleCount;

__declspec(dllexport) UINT WINAPI Mine_SetHandleCount( UINT uNumber ){
  if(ChessWrapperSentry::Wrap("SetHandleCount")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetHandleCount");
   }
  return Real_SetHandleCount(uNumber);
}
BOOL (WINAPI * Real_SetHandleInformation)( HANDLE hObject, DWORD dwMask, DWORD dwFlags )
  = SetHandleInformation;

__declspec(dllexport) BOOL WINAPI Mine_SetHandleInformation( HANDLE hObject, DWORD dwMask, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("SetHandleInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetHandleInformation");
   }
  return Real_SetHandleInformation(hObject, dwMask, dwFlags);
}
BOOL (WINAPI * Real_SetInformationJobObject)( HANDLE hJob, JOBOBJECTINFOCLASS JobObjectInformationClass, LPVOID lpJobObjectInformation, DWORD cbJobObjectInformationLength )
  = SetInformationJobObject;

__declspec(dllexport) BOOL WINAPI Mine_SetInformationJobObject( HANDLE hJob, JOBOBJECTINFOCLASS JobObjectInformationClass, LPVOID lpJobObjectInformation, DWORD cbJobObjectInformationLength ){
  if(ChessWrapperSentry::Wrap("SetInformationJobObject")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetInformationJobObject");
   }
  return Real_SetInformationJobObject(hJob, JobObjectInformationClass, lpJobObjectInformation, cbJobObjectInformationLength);
}
void (WINAPI * Real_SetLastError)( DWORD dwErrCode )
  = SetLastError;

__declspec(dllexport) void WINAPI Mine_SetLastError( DWORD dwErrCode ){
  if(ChessWrapperSentry::Wrap("SetLastError")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetLastError");
   }
  return Real_SetLastError(dwErrCode);
}
BOOL (WINAPI * Real_SetLocalTime)( const SYSTEMTIME *lpSystemTime )
  = SetLocalTime;

__declspec(dllexport) BOOL WINAPI Mine_SetLocalTime( const SYSTEMTIME *lpSystemTime ){
  if(ChessWrapperSentry::Wrap("SetLocalTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetLocalTime");
   }
  return Real_SetLocalTime(lpSystemTime);
}
BOOL (WINAPI * Real_SetLocaleInfoA)( LCID Locale, LCTYPE LCType, LPCSTR lpLCData)
  = SetLocaleInfoA;

__declspec(dllexport) BOOL WINAPI Mine_SetLocaleInfoA( LCID Locale, LCTYPE LCType, LPCSTR lpLCData){
  if(ChessWrapperSentry::Wrap("SetLocaleInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetLocaleInfoA");
   }
  return Real_SetLocaleInfoA(Locale, LCType, lpLCData);
}
BOOL (WINAPI * Real_SetLocaleInfoW)( LCID Locale, LCTYPE LCType, LPCWSTR lpLCData)
  = SetLocaleInfoW;

__declspec(dllexport) BOOL WINAPI Mine_SetLocaleInfoW( LCID Locale, LCTYPE LCType, LPCWSTR lpLCData){
  if(ChessWrapperSentry::Wrap("SetLocaleInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetLocaleInfoW");
   }
  return Real_SetLocaleInfoW(Locale, LCType, lpLCData);
}
BOOL (WINAPI * Real_SetMailslotInfo)( HANDLE hMailslot, DWORD lReadTimeout )
  = SetMailslotInfo;

__declspec(dllexport) BOOL WINAPI Mine_SetMailslotInfo( HANDLE hMailslot, DWORD lReadTimeout ){
  if(ChessWrapperSentry::Wrap("SetMailslotInfo")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetMailslotInfo");
   }
  return Real_SetMailslotInfo(hMailslot, lReadTimeout);
}
BOOL (WINAPI * Real_SetMessageWaitingIndicator)( HANDLE hMsgIndicator, ULONG ulMsgCount )
  = SetMessageWaitingIndicator;

__declspec(dllexport) BOOL WINAPI Mine_SetMessageWaitingIndicator( HANDLE hMsgIndicator, ULONG ulMsgCount ){
  if(ChessWrapperSentry::Wrap("SetMessageWaitingIndicator")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetMessageWaitingIndicator");
   }
  return Real_SetMessageWaitingIndicator(hMsgIndicator, ulMsgCount);
}
BOOL (WINAPI * Real_SetNamedPipeHandleState)( HANDLE hNamedPipe, LPDWORD lpMode, LPDWORD lpMaxCollectionCount, LPDWORD lpCollectDataTimeout )
  = SetNamedPipeHandleState;

__declspec(dllexport) BOOL WINAPI Mine_SetNamedPipeHandleState( HANDLE hNamedPipe, LPDWORD lpMode, LPDWORD lpMaxCollectionCount, LPDWORD lpCollectDataTimeout ){
  if(ChessWrapperSentry::Wrap("SetNamedPipeHandleState")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetNamedPipeHandleState");
   }
  return Real_SetNamedPipeHandleState(hNamedPipe, lpMode, lpMaxCollectionCount, lpCollectDataTimeout);
}
BOOL (WINAPI * Real_SetPriorityClass)( HANDLE hProcess, DWORD dwPriorityClass )
  = SetPriorityClass;

__declspec(dllexport) BOOL WINAPI Mine_SetPriorityClass( HANDLE hProcess, DWORD dwPriorityClass ){
  if(ChessWrapperSentry::Wrap("SetPriorityClass")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetPriorityClass");
   }
  return Real_SetPriorityClass(hProcess, dwPriorityClass);
}
BOOL (WINAPI * Real_SetProcessAffinityMask)( HANDLE hProcess, DWORD_PTR dwProcessAffinityMask )
  = SetProcessAffinityMask;

__declspec(dllexport) BOOL WINAPI Mine_SetProcessAffinityMask( HANDLE hProcess, DWORD_PTR dwProcessAffinityMask ){
  if(ChessWrapperSentry::Wrap("SetProcessAffinityMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetProcessAffinityMask");
   }
  return Real_SetProcessAffinityMask(hProcess, dwProcessAffinityMask);
}
BOOL (WINAPI * Real_SetProcessPriorityBoost)( HANDLE hProcess, BOOL bDisablePriorityBoost )
  = SetProcessPriorityBoost;

__declspec(dllexport) BOOL WINAPI Mine_SetProcessPriorityBoost( HANDLE hProcess, BOOL bDisablePriorityBoost ){
  if(ChessWrapperSentry::Wrap("SetProcessPriorityBoost")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetProcessPriorityBoost");
   }
  return Real_SetProcessPriorityBoost(hProcess, bDisablePriorityBoost);
}
BOOL (WINAPI * Real_SetProcessShutdownParameters)( DWORD dwLevel, DWORD dwFlags )
  = SetProcessShutdownParameters;

__declspec(dllexport) BOOL WINAPI Mine_SetProcessShutdownParameters( DWORD dwLevel, DWORD dwFlags ){
  if(ChessWrapperSentry::Wrap("SetProcessShutdownParameters")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetProcessShutdownParameters");
   }
  return Real_SetProcessShutdownParameters(dwLevel, dwFlags);
}
BOOL (WINAPI * Real_SetProcessWorkingSetSize)( HANDLE hProcess, SIZE_T dwMinimumWorkingSetSize, SIZE_T dwMaximumWorkingSetSize )
  = SetProcessWorkingSetSize;

__declspec(dllexport) BOOL WINAPI Mine_SetProcessWorkingSetSize( HANDLE hProcess, SIZE_T dwMinimumWorkingSetSize, SIZE_T dwMaximumWorkingSetSize ){
  if(ChessWrapperSentry::Wrap("SetProcessWorkingSetSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetProcessWorkingSetSize");
   }
  return Real_SetProcessWorkingSetSize(hProcess, dwMinimumWorkingSetSize, dwMaximumWorkingSetSize);
}
BOOL (WINAPI * Real_SetProcessWorkingSetSizeEx)( HANDLE hProcess, SIZE_T dwMinimumWorkingSetSize, SIZE_T dwMaximumWorkingSetSize, DWORD Flags )
  = SetProcessWorkingSetSizeEx;

__declspec(dllexport) BOOL WINAPI Mine_SetProcessWorkingSetSizeEx( HANDLE hProcess, SIZE_T dwMinimumWorkingSetSize, SIZE_T dwMaximumWorkingSetSize, DWORD Flags ){
  if(ChessWrapperSentry::Wrap("SetProcessWorkingSetSizeEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetProcessWorkingSetSizeEx");
   }
  return Real_SetProcessWorkingSetSizeEx(hProcess, dwMinimumWorkingSetSize, dwMaximumWorkingSetSize, Flags);
}
BOOL (WINAPI * Real_SetStdHandle)( DWORD nStdHandle, HANDLE hHandle )
  = SetStdHandle;

__declspec(dllexport) BOOL WINAPI Mine_SetStdHandle( DWORD nStdHandle, HANDLE hHandle ){
  if(ChessWrapperSentry::Wrap("SetStdHandle")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetStdHandle");
   }
  return Real_SetStdHandle(nStdHandle, hHandle);
}
BOOL (WINAPI * Real_SetSystemFileCacheSize)( SIZE_T MinimumFileCacheSize, SIZE_T MaximumFileCacheSize, DWORD Flags )
  = SetSystemFileCacheSize;

__declspec(dllexport) BOOL WINAPI Mine_SetSystemFileCacheSize( SIZE_T MinimumFileCacheSize, SIZE_T MaximumFileCacheSize, DWORD Flags ){
  if(ChessWrapperSentry::Wrap("SetSystemFileCacheSize")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetSystemFileCacheSize");
   }
  return Real_SetSystemFileCacheSize(MinimumFileCacheSize, MaximumFileCacheSize, Flags);
}
BOOL (WINAPI * Real_SetSystemTime)( const SYSTEMTIME *lpSystemTime )
  = SetSystemTime;

__declspec(dllexport) BOOL WINAPI Mine_SetSystemTime( const SYSTEMTIME *lpSystemTime ){
  if(ChessWrapperSentry::Wrap("SetSystemTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetSystemTime");
   }
  return Real_SetSystemTime(lpSystemTime);
}
BOOL (WINAPI * Real_SetSystemTimeAdjustment)( DWORD dwTimeAdjustment, BOOL bTimeAdjustmentDisabled )
  = SetSystemTimeAdjustment;

__declspec(dllexport) BOOL WINAPI Mine_SetSystemTimeAdjustment( DWORD dwTimeAdjustment, BOOL bTimeAdjustmentDisabled ){
  if(ChessWrapperSentry::Wrap("SetSystemTimeAdjustment")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetSystemTimeAdjustment");
   }
  return Real_SetSystemTimeAdjustment(dwTimeAdjustment, bTimeAdjustmentDisabled);
}
DWORD (WINAPI * Real_SetTapeParameters)( HANDLE hDevice, DWORD dwOperation, LPVOID lpTapeInformation )
  = SetTapeParameters;

__declspec(dllexport) DWORD WINAPI Mine_SetTapeParameters( HANDLE hDevice, DWORD dwOperation, LPVOID lpTapeInformation ){
  if(ChessWrapperSentry::Wrap("SetTapeParameters")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetTapeParameters");
   }
  return Real_SetTapeParameters(hDevice, dwOperation, lpTapeInformation);
}
DWORD (WINAPI * Real_SetTapePosition)( HANDLE hDevice, DWORD dwPositionMethod, DWORD dwPartition, DWORD dwOffsetLow, DWORD dwOffsetHigh, BOOL bImmediate )
  = SetTapePosition;

__declspec(dllexport) DWORD WINAPI Mine_SetTapePosition( HANDLE hDevice, DWORD dwPositionMethod, DWORD dwPartition, DWORD dwOffsetLow, DWORD dwOffsetHigh, BOOL bImmediate ){
  if(ChessWrapperSentry::Wrap("SetTapePosition")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetTapePosition");
   }
  return Real_SetTapePosition(hDevice, dwPositionMethod, dwPartition, dwOffsetLow, dwOffsetHigh, bImmediate);
}
DWORD_PTR (WINAPI * Real_SetThreadAffinityMask)( HANDLE hThread, DWORD_PTR dwThreadAffinityMask )
  = SetThreadAffinityMask;

__declspec(dllexport) DWORD_PTR WINAPI Mine_SetThreadAffinityMask( HANDLE hThread, DWORD_PTR dwThreadAffinityMask ){
  if(ChessWrapperSentry::Wrap("SetThreadAffinityMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadAffinityMask");
   }
  return Real_SetThreadAffinityMask(hThread, dwThreadAffinityMask);
}
BOOL (WINAPI * Real_SetThreadContext)( HANDLE hThread, const CONTEXT *lpContext )
  = SetThreadContext;

__declspec(dllexport) BOOL WINAPI Mine_SetThreadContext( HANDLE hThread, const CONTEXT *lpContext ){
  if(ChessWrapperSentry::Wrap("SetThreadContext")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadContext");
   }
  return Real_SetThreadContext(hThread, lpContext);
}
EXECUTION_STATE (WINAPI * Real_SetThreadExecutionState)( EXECUTION_STATE esFlags )
  = SetThreadExecutionState;

__declspec(dllexport) EXECUTION_STATE WINAPI Mine_SetThreadExecutionState( EXECUTION_STATE esFlags ){
  if(ChessWrapperSentry::Wrap("SetThreadExecutionState")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadExecutionState");
   }
  return Real_SetThreadExecutionState(esFlags);
}
DWORD (WINAPI * Real_SetThreadIdealProcessor)( HANDLE hThread, DWORD dwIdealProcessor )
  = SetThreadIdealProcessor;

__declspec(dllexport) DWORD WINAPI Mine_SetThreadIdealProcessor( HANDLE hThread, DWORD dwIdealProcessor ){
  if(ChessWrapperSentry::Wrap("SetThreadIdealProcessor")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadIdealProcessor");
   }
  return Real_SetThreadIdealProcessor(hThread, dwIdealProcessor);
}
BOOL (WINAPI * Real_SetThreadLocale)( LCID Locale )
  = SetThreadLocale;

__declspec(dllexport) BOOL WINAPI Mine_SetThreadLocale( LCID Locale ){
  if(ChessWrapperSentry::Wrap("SetThreadLocale")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadLocale");
   }
  return Real_SetThreadLocale(Locale);
}
BOOL (WINAPI * Real_SetThreadPriority)( HANDLE hThread, int nPriority )
  = SetThreadPriority;

__declspec(dllexport) BOOL WINAPI Mine_SetThreadPriority( HANDLE hThread, int nPriority ){
  if(ChessWrapperSentry::Wrap("SetThreadPriority")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadPriority");
   }
  return Real_SetThreadPriority(hThread, nPriority);
}
BOOL (WINAPI * Real_SetThreadPriorityBoost)( HANDLE hThread, BOOL bDisablePriorityBoost )
  = SetThreadPriorityBoost;

__declspec(dllexport) BOOL WINAPI Mine_SetThreadPriorityBoost( HANDLE hThread, BOOL bDisablePriorityBoost ){
  if(ChessWrapperSentry::Wrap("SetThreadPriorityBoost")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadPriorityBoost");
   }
  return Real_SetThreadPriorityBoost(hThread, bDisablePriorityBoost);
}
BOOL (WINAPI * Real_SetThreadStackGuarantee)( PULONG StackSizeInBytes )
  = SetThreadStackGuarantee;

__declspec(dllexport) BOOL WINAPI Mine_SetThreadStackGuarantee( PULONG StackSizeInBytes ){
  if(ChessWrapperSentry::Wrap("SetThreadStackGuarantee")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetThreadStackGuarantee");
   }
  return Real_SetThreadStackGuarantee(StackSizeInBytes);
}
BOOL (WINAPI * Real_SetTimeZoneInformation)( const TIME_ZONE_INFORMATION *lpTimeZoneInformation )
  = SetTimeZoneInformation;

__declspec(dllexport) BOOL WINAPI Mine_SetTimeZoneInformation( const TIME_ZONE_INFORMATION *lpTimeZoneInformation ){
  if(ChessWrapperSentry::Wrap("SetTimeZoneInformation")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetTimeZoneInformation");
   }
  return Real_SetTimeZoneInformation(lpTimeZoneInformation);
}
HANDLE (WINAPI * Real_SetTimerQueueTimer)( HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, BOOL PreferIo )
  = SetTimerQueueTimer;

__declspec(dllexport) HANDLE WINAPI Mine_SetTimerQueueTimer( HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, BOOL PreferIo ){
  if(ChessWrapperSentry::Wrap("SetTimerQueueTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetTimerQueueTimer");
   }
  return Real_SetTimerQueueTimer(TimerQueue, Callback, Parameter, DueTime, Period, PreferIo);
}
LPTOP_LEVEL_EXCEPTION_FILTER (WINAPI * Real_SetUnhandledExceptionFilter)( LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter )
  = SetUnhandledExceptionFilter;

__declspec(dllexport) LPTOP_LEVEL_EXCEPTION_FILTER WINAPI Mine_SetUnhandledExceptionFilter( LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter ){
  if(ChessWrapperSentry::Wrap("SetUnhandledExceptionFilter")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetUnhandledExceptionFilter");
   }
  return Real_SetUnhandledExceptionFilter(lpTopLevelExceptionFilter);
}
BOOL (WINAPI * Real_SetUserGeoID)( GEOID GeoId)
  = SetUserGeoID;

__declspec(dllexport) BOOL WINAPI Mine_SetUserGeoID( GEOID GeoId){
  if(ChessWrapperSentry::Wrap("SetUserGeoID")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetUserGeoID");
   }
  return Real_SetUserGeoID(GeoId);
}
BOOL (WINAPI * Real_SetVolumeLabelA)( LPCSTR lpRootPathName, LPCSTR lpVolumeName )
  = SetVolumeLabelA;

__declspec(dllexport) BOOL WINAPI Mine_SetVolumeLabelA( LPCSTR lpRootPathName, LPCSTR lpVolumeName ){
  if(ChessWrapperSentry::Wrap("SetVolumeLabelA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetVolumeLabelA");
   }
  return Real_SetVolumeLabelA(lpRootPathName, lpVolumeName);
}
BOOL (WINAPI * Real_SetVolumeLabelW)( LPCWSTR lpRootPathName, LPCWSTR lpVolumeName )
  = SetVolumeLabelW;

__declspec(dllexport) BOOL WINAPI Mine_SetVolumeLabelW( LPCWSTR lpRootPathName, LPCWSTR lpVolumeName ){
  if(ChessWrapperSentry::Wrap("SetVolumeLabelW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetVolumeLabelW");
   }
  return Real_SetVolumeLabelW(lpRootPathName, lpVolumeName);
}
BOOL (WINAPI * Real_SetVolumeMountPointA)( LPCSTR lpszVolumeMountPoint, LPCSTR lpszVolumeName )
  = SetVolumeMountPointA;

__declspec(dllexport) BOOL WINAPI Mine_SetVolumeMountPointA( LPCSTR lpszVolumeMountPoint, LPCSTR lpszVolumeName ){
  if(ChessWrapperSentry::Wrap("SetVolumeMountPointA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetVolumeMountPointA");
   }
  return Real_SetVolumeMountPointA(lpszVolumeMountPoint, lpszVolumeName);
}
BOOL (WINAPI * Real_SetVolumeMountPointW)( LPCWSTR lpszVolumeMountPoint, LPCWSTR lpszVolumeName )
  = SetVolumeMountPointW;

__declspec(dllexport) BOOL WINAPI Mine_SetVolumeMountPointW( LPCWSTR lpszVolumeMountPoint, LPCWSTR lpszVolumeName ){
  if(ChessWrapperSentry::Wrap("SetVolumeMountPointW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetVolumeMountPointW");
   }
  return Real_SetVolumeMountPointW(lpszVolumeMountPoint, lpszVolumeName);
}
BOOL (WINAPI * Real_SetWaitableTimer)( HANDLE hTimer, const LARGE_INTEGER *lpDueTime, LONG lPeriod, PTIMERAPCROUTINE pfnCompletionRoutine, LPVOID lpArgToCompletionRoutine, BOOL fResume )
  = SetWaitableTimer;

__declspec(dllexport) BOOL WINAPI Mine_SetWaitableTimer( HANDLE hTimer, const LARGE_INTEGER *lpDueTime, LONG lPeriod, PTIMERAPCROUTINE pfnCompletionRoutine, LPVOID lpArgToCompletionRoutine, BOOL fResume ){
  if(ChessWrapperSentry::Wrap("SetWaitableTimer")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetWaitableTimer");
   }
  return Real_SetWaitableTimer(hTimer, lpDueTime, lPeriod, pfnCompletionRoutine, lpArgToCompletionRoutine, fResume);
}
BOOL (WINAPI * Real_SetupComm)( HANDLE hFile, DWORD dwInQueue, DWORD dwOutQueue )
  = SetupComm;

__declspec(dllexport) BOOL WINAPI Mine_SetupComm( HANDLE hFile, DWORD dwInQueue, DWORD dwOutQueue ){
  if(ChessWrapperSentry::Wrap("SetupComm")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SetupComm");
   }
  return Real_SetupComm(hFile, dwInQueue, dwOutQueue);
}
DWORD (WINAPI * Real_SignalObjectAndWait)( HANDLE hObjectToSignal, HANDLE hObjectToWaitOn, DWORD dwMilliseconds, BOOL bAlertable )
   = SignalObjectAndWait;

__declspec(dllexport) DWORD WINAPI Mine_SignalObjectAndWait( HANDLE hObjectToSignal, HANDLE hObjectToWaitOn, DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_SignalObjectAndWait
  if(ChessWrapperSentry::Wrap("SignalObjectAndWait")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SignalObjectAndWait");
     DWORD res = __wrapper_SignalObjectAndWait(hObjectToSignal, hObjectToWaitOn, dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_SignalObjectAndWait(hObjectToSignal, hObjectToWaitOn, dwMilliseconds, bAlertable);
}
DWORD (WINAPI * Real_SizeofResource)( HMODULE hModule, HRSRC hResInfo )
  = SizeofResource;

__declspec(dllexport) DWORD WINAPI Mine_SizeofResource( HMODULE hModule, HRSRC hResInfo ){
  if(ChessWrapperSentry::Wrap("SizeofResource")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SizeofResource");
   }
  return Real_SizeofResource(hModule, hResInfo);
}
void (WINAPI * Real_Sleep)( DWORD dwMilliseconds )
   = Sleep;

__declspec(dllexport) void WINAPI Mine_Sleep( DWORD dwMilliseconds ){
#ifdef WRAP_Sleep
  if(ChessWrapperSentry::Wrap("Sleep")){
     ChessWrapperSentry sentry;
     Chess::LogCall("Sleep");
     __wrapper_Sleep(dwMilliseconds);
     return;
  }
#endif
  return Real_Sleep(dwMilliseconds);
}
DWORD (WINAPI * Real_SleepEx)( DWORD dwMilliseconds, BOOL bAlertable )
   = SleepEx;

__declspec(dllexport) DWORD WINAPI Mine_SleepEx( DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_SleepEx
  if(ChessWrapperSentry::Wrap("SleepEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SleepEx");
     DWORD res = __wrapper_SleepEx(dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_SleepEx(dwMilliseconds, bAlertable);
}
DWORD (WINAPI * Real_SuspendThread)( HANDLE hThread )
   = SuspendThread;

__declspec(dllexport) DWORD WINAPI Mine_SuspendThread( HANDLE hThread ){
#ifdef WRAP_SuspendThread
  if(ChessWrapperSentry::Wrap("SuspendThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SuspendThread");
     DWORD res = __wrapper_SuspendThread(hThread);
     return res;
  }
#endif
  return Real_SuspendThread(hThread);
}
void (WINAPI * Real_SwitchToFiber)( LPVOID lpFiber )
  = SwitchToFiber;

__declspec(dllexport) void WINAPI Mine_SwitchToFiber( LPVOID lpFiber ){
  if(ChessWrapperSentry::Wrap("SwitchToFiber")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SwitchToFiber");
   }
  return Real_SwitchToFiber(lpFiber);
}
BOOL (WINAPI * Real_SwitchToThread)( void )
   = SwitchToThread;

__declspec(dllexport) BOOL WINAPI Mine_SwitchToThread( void ){
#ifdef WRAP_SwitchToThread
  if(ChessWrapperSentry::Wrap("SwitchToThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SwitchToThread");
     BOOL res = __wrapper_SwitchToThread();
     return res;
  }
#endif
  return Real_SwitchToThread();
}
BOOL (WINAPI * Real_SystemTimeToFileTime)( const SYSTEMTIME *lpSystemTime, LPFILETIME lpFileTime )
  = SystemTimeToFileTime;

__declspec(dllexport) BOOL WINAPI Mine_SystemTimeToFileTime( const SYSTEMTIME *lpSystemTime, LPFILETIME lpFileTime ){
  if(ChessWrapperSentry::Wrap("SystemTimeToFileTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SystemTimeToFileTime");
   }
  return Real_SystemTimeToFileTime(lpSystemTime, lpFileTime);
}
BOOL (WINAPI * Real_SystemTimeToTzSpecificLocalTime)( LPTIME_ZONE_INFORMATION lpTimeZoneInformation, LPSYSTEMTIME lpUniversalTime, LPSYSTEMTIME lpLocalTime )
  = SystemTimeToTzSpecificLocalTime;

__declspec(dllexport) BOOL WINAPI Mine_SystemTimeToTzSpecificLocalTime( LPTIME_ZONE_INFORMATION lpTimeZoneInformation, LPSYSTEMTIME lpUniversalTime, LPSYSTEMTIME lpLocalTime ){
  if(ChessWrapperSentry::Wrap("SystemTimeToTzSpecificLocalTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("SystemTimeToTzSpecificLocalTime");
   }
  return Real_SystemTimeToTzSpecificLocalTime(lpTimeZoneInformation, lpUniversalTime, lpLocalTime);
}
BOOL (WINAPI * Real_TerminateJobObject)( HANDLE hJob, UINT uExitCode )
  = TerminateJobObject;

__declspec(dllexport) BOOL WINAPI Mine_TerminateJobObject( HANDLE hJob, UINT uExitCode ){
  if(ChessWrapperSentry::Wrap("TerminateJobObject")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TerminateJobObject");
   }
  return Real_TerminateJobObject(hJob, uExitCode);
}
BOOL (WINAPI * Real_TerminateProcess)( HANDLE hProcess, UINT uExitCode )
  = TerminateProcess;

__declspec(dllexport) BOOL WINAPI Mine_TerminateProcess( HANDLE hProcess, UINT uExitCode ){
  if(ChessWrapperSentry::Wrap("TerminateProcess")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TerminateProcess");
   }
  return Real_TerminateProcess(hProcess, uExitCode);
}
BOOL (WINAPI * Real_TerminateThread)( HANDLE hThread, DWORD dwExitCode )
  = TerminateThread;

__declspec(dllexport) BOOL WINAPI Mine_TerminateThread( HANDLE hThread, DWORD dwExitCode ){
  if(ChessWrapperSentry::Wrap("TerminateThread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TerminateThread");
   }
  return Real_TerminateThread(hThread, dwExitCode);
}
DWORD (WINAPI * Real_TlsAlloc)( void )
  = TlsAlloc;

__declspec(dllexport) DWORD WINAPI Mine_TlsAlloc( void ){
  if(ChessWrapperSentry::Wrap("TlsAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TlsAlloc");
   }
  return Real_TlsAlloc();
}
BOOL (WINAPI * Real_TlsFree)( DWORD dwTlsIndex )
  = TlsFree;

__declspec(dllexport) BOOL WINAPI Mine_TlsFree( DWORD dwTlsIndex ){
  if(ChessWrapperSentry::Wrap("TlsFree")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TlsFree");
   }
  return Real_TlsFree(dwTlsIndex);
}
LPVOID (WINAPI * Real_TlsGetValue)( DWORD dwTlsIndex )
  = TlsGetValue;

__declspec(dllexport) LPVOID WINAPI Mine_TlsGetValue( DWORD dwTlsIndex ){
  if(ChessWrapperSentry::Wrap("TlsGetValue")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TlsGetValue");
   }
  return Real_TlsGetValue(dwTlsIndex);
}
BOOL (WINAPI * Real_TlsSetValue)( DWORD dwTlsIndex, LPVOID lpTlsValue )
  = TlsSetValue;

__declspec(dllexport) BOOL WINAPI Mine_TlsSetValue( DWORD dwTlsIndex, LPVOID lpTlsValue ){
  if(ChessWrapperSentry::Wrap("TlsSetValue")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TlsSetValue");
   }
  return Real_TlsSetValue(dwTlsIndex, lpTlsValue);
}
BOOL (WINAPI * Real_TransactNamedPipe)( HANDLE hNamedPipe, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesRead, LPOVERLAPPED lpOverlapped )
  = TransactNamedPipe;

__declspec(dllexport) BOOL WINAPI Mine_TransactNamedPipe( HANDLE hNamedPipe, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesRead, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("TransactNamedPipe")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TransactNamedPipe");
   }
  return Real_TransactNamedPipe(hNamedPipe, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesRead, lpOverlapped);
}
BOOL (WINAPI * Real_TransmitCommChar)( HANDLE hFile, char cChar )
  = TransmitCommChar;

__declspec(dllexport) BOOL WINAPI Mine_TransmitCommChar( HANDLE hFile, char cChar ){
  if(ChessWrapperSentry::Wrap("TransmitCommChar")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TransmitCommChar");
   }
  return Real_TransmitCommChar(hFile, cChar);
}
BOOL (WINAPI * Real_TryEnterCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
   = TryEnterCriticalSection;

__declspec(dllexport) BOOL WINAPI Mine_TryEnterCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
#ifdef WRAP_TryEnterCriticalSection
  if(ChessWrapperSentry::Wrap("TryEnterCriticalSection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TryEnterCriticalSection");
     BOOL res = __wrapper_TryEnterCriticalSection(lpCriticalSection);
     return res;
  }
#endif
  return Real_TryEnterCriticalSection(lpCriticalSection);
}
BOOL (WINAPI * Real_TzSpecificLocalTimeToSystemTime)( LPTIME_ZONE_INFORMATION lpTimeZoneInformation, LPSYSTEMTIME lpLocalTime, LPSYSTEMTIME lpUniversalTime )
  = TzSpecificLocalTimeToSystemTime;

__declspec(dllexport) BOOL WINAPI Mine_TzSpecificLocalTimeToSystemTime( LPTIME_ZONE_INFORMATION lpTimeZoneInformation, LPSYSTEMTIME lpLocalTime, LPSYSTEMTIME lpUniversalTime ){
  if(ChessWrapperSentry::Wrap("TzSpecificLocalTimeToSystemTime")){
     ChessWrapperSentry sentry;
     Chess::LogCall("TzSpecificLocalTimeToSystemTime");
   }
  return Real_TzSpecificLocalTimeToSystemTime(lpTimeZoneInformation, lpLocalTime, lpUniversalTime);
}
LONG (WINAPI * Real_UnhandledExceptionFilter)( struct _EXCEPTION_POINTERS *ExceptionInfo )
  = UnhandledExceptionFilter;

__declspec(dllexport) LONG WINAPI Mine_UnhandledExceptionFilter( struct _EXCEPTION_POINTERS *ExceptionInfo ){
  if(ChessWrapperSentry::Wrap("UnhandledExceptionFilter")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UnhandledExceptionFilter");
   }
  return Real_UnhandledExceptionFilter(ExceptionInfo);
}
BOOL (WINAPI * Real_UnlockFile)( HANDLE hFile, DWORD dwFileOffsetLow, DWORD dwFileOffsetHigh, DWORD nNumberOfBytesToUnlockLow, DWORD nNumberOfBytesToUnlockHigh )
  = UnlockFile;

__declspec(dllexport) BOOL WINAPI Mine_UnlockFile( HANDLE hFile, DWORD dwFileOffsetLow, DWORD dwFileOffsetHigh, DWORD nNumberOfBytesToUnlockLow, DWORD nNumberOfBytesToUnlockHigh ){
  if(ChessWrapperSentry::Wrap("UnlockFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UnlockFile");
   }
  return Real_UnlockFile(hFile, dwFileOffsetLow, dwFileOffsetHigh, nNumberOfBytesToUnlockLow, nNumberOfBytesToUnlockHigh);
}
BOOL (WINAPI * Real_UnlockFileEx)( HANDLE hFile, DWORD dwReserved, DWORD nNumberOfBytesToUnlockLow, DWORD nNumberOfBytesToUnlockHigh, LPOVERLAPPED lpOverlapped )
  = UnlockFileEx;

__declspec(dllexport) BOOL WINAPI Mine_UnlockFileEx( HANDLE hFile, DWORD dwReserved, DWORD nNumberOfBytesToUnlockLow, DWORD nNumberOfBytesToUnlockHigh, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("UnlockFileEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UnlockFileEx");
   }
  return Real_UnlockFileEx(hFile, dwReserved, nNumberOfBytesToUnlockLow, nNumberOfBytesToUnlockHigh, lpOverlapped);
}
BOOL (WINAPI * Real_UnmapViewOfFile)( LPCVOID lpBaseAddress )
  = UnmapViewOfFile;

__declspec(dllexport) BOOL WINAPI Mine_UnmapViewOfFile( LPCVOID lpBaseAddress ){
  if(ChessWrapperSentry::Wrap("UnmapViewOfFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UnmapViewOfFile");
   }
  return Real_UnmapViewOfFile(lpBaseAddress);
}
BOOL (WINAPI * Real_UnregisterWait)( HANDLE WaitHandle )
  = UnregisterWait;

__declspec(dllexport) BOOL WINAPI Mine_UnregisterWait( HANDLE WaitHandle ){
  if(ChessWrapperSentry::Wrap("UnregisterWait")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UnregisterWait");
   }
  return Real_UnregisterWait(WaitHandle);
}
BOOL (WINAPI * Real_UnregisterWaitEx)( HANDLE WaitHandle, HANDLE CompletionEvent )
  = UnregisterWaitEx;

__declspec(dllexport) BOOL WINAPI Mine_UnregisterWaitEx( HANDLE WaitHandle, HANDLE CompletionEvent ){
  if(ChessWrapperSentry::Wrap("UnregisterWaitEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UnregisterWaitEx");
   }
  return Real_UnregisterWaitEx(WaitHandle, CompletionEvent);
}
BOOL (WINAPI * Real_UpdateResourceA)( HANDLE hUpdate, LPCSTR lpType, LPCSTR lpName, WORD wLanguage, LPVOID lpData, DWORD cb )
  = UpdateResourceA;

__declspec(dllexport) BOOL WINAPI Mine_UpdateResourceA( HANDLE hUpdate, LPCSTR lpType, LPCSTR lpName, WORD wLanguage, LPVOID lpData, DWORD cb ){
  if(ChessWrapperSentry::Wrap("UpdateResourceA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UpdateResourceA");
   }
  return Real_UpdateResourceA(hUpdate, lpType, lpName, wLanguage, lpData, cb);
}
BOOL (WINAPI * Real_UpdateResourceW)( HANDLE hUpdate, LPCWSTR lpType, LPCWSTR lpName, WORD wLanguage, LPVOID lpData, DWORD cb )
  = UpdateResourceW;

__declspec(dllexport) BOOL WINAPI Mine_UpdateResourceW( HANDLE hUpdate, LPCWSTR lpType, LPCWSTR lpName, WORD wLanguage, LPVOID lpData, DWORD cb ){
  if(ChessWrapperSentry::Wrap("UpdateResourceW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("UpdateResourceW");
   }
  return Real_UpdateResourceW(hUpdate, lpType, lpName, wLanguage, lpData, cb);
}
ULONGLONG (WINAPI * Real_VerSetConditionMask)( ULONGLONG ConditionMask, DWORD TypeMask, BYTE Condition )
  = VerSetConditionMask;

__declspec(dllexport) ULONGLONG WINAPI Mine_VerSetConditionMask( ULONGLONG ConditionMask, DWORD TypeMask, BYTE Condition ){
  if(ChessWrapperSentry::Wrap("VerSetConditionMask")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VerSetConditionMask");
   }
  return Real_VerSetConditionMask(ConditionMask, TypeMask, Condition);
}
BOOL (WINAPI * Real_VerifyVersionInfoA)( LPOSVERSIONINFOEXA lpVersionInformation, DWORD dwTypeMask, DWORDLONG dwlConditionMask )
  = VerifyVersionInfoA;

__declspec(dllexport) BOOL WINAPI Mine_VerifyVersionInfoA( LPOSVERSIONINFOEXA lpVersionInformation, DWORD dwTypeMask, DWORDLONG dwlConditionMask ){
  if(ChessWrapperSentry::Wrap("VerifyVersionInfoA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VerifyVersionInfoA");
   }
  return Real_VerifyVersionInfoA(lpVersionInformation, dwTypeMask, dwlConditionMask);
}
BOOL (WINAPI * Real_VerifyVersionInfoW)( LPOSVERSIONINFOEXW lpVersionInformation, DWORD dwTypeMask, DWORDLONG dwlConditionMask )
  = VerifyVersionInfoW;

__declspec(dllexport) BOOL WINAPI Mine_VerifyVersionInfoW( LPOSVERSIONINFOEXW lpVersionInformation, DWORD dwTypeMask, DWORDLONG dwlConditionMask ){
  if(ChessWrapperSentry::Wrap("VerifyVersionInfoW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VerifyVersionInfoW");
   }
  return Real_VerifyVersionInfoW(lpVersionInformation, dwTypeMask, dwlConditionMask);
}
LPVOID (WINAPI * Real_VirtualAlloc)( LPVOID lpAddress, SIZE_T dwSize, DWORD flAllocationType, DWORD flProtect )
  = VirtualAlloc;

__declspec(dllexport) LPVOID WINAPI Mine_VirtualAlloc( LPVOID lpAddress, SIZE_T dwSize, DWORD flAllocationType, DWORD flProtect ){
  if(ChessWrapperSentry::Wrap("VirtualAlloc")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualAlloc");
   }
  return Real_VirtualAlloc(lpAddress, dwSize, flAllocationType, flProtect);
}
LPVOID (WINAPI * Real_VirtualAllocEx)( HANDLE hProcess, LPVOID lpAddress, SIZE_T dwSize, DWORD flAllocationType, DWORD flProtect )
  = VirtualAllocEx;

__declspec(dllexport) LPVOID WINAPI Mine_VirtualAllocEx( HANDLE hProcess, LPVOID lpAddress, SIZE_T dwSize, DWORD flAllocationType, DWORD flProtect ){
  if(ChessWrapperSentry::Wrap("VirtualAllocEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualAllocEx");
   }
  return Real_VirtualAllocEx(hProcess, lpAddress, dwSize, flAllocationType, flProtect);
}
BOOL (WINAPI * Real_VirtualFree)( LPVOID lpAddress, SIZE_T dwSize, DWORD dwFreeType )
  = VirtualFree;

__declspec(dllexport) BOOL WINAPI Mine_VirtualFree( LPVOID lpAddress, SIZE_T dwSize, DWORD dwFreeType ){
  if(ChessWrapperSentry::Wrap("VirtualFree")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualFree");
   }
  return Real_VirtualFree(lpAddress, dwSize, dwFreeType);
}
BOOL (WINAPI * Real_VirtualFreeEx)( HANDLE hProcess, LPVOID lpAddress, SIZE_T dwSize, DWORD dwFreeType )
  = VirtualFreeEx;

__declspec(dllexport) BOOL WINAPI Mine_VirtualFreeEx( HANDLE hProcess, LPVOID lpAddress, SIZE_T dwSize, DWORD dwFreeType ){
  if(ChessWrapperSentry::Wrap("VirtualFreeEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualFreeEx");
   }
  return Real_VirtualFreeEx(hProcess, lpAddress, dwSize, dwFreeType);
}
BOOL (WINAPI * Real_VirtualLock)( LPVOID lpAddress, SIZE_T dwSize )
  = VirtualLock;

__declspec(dllexport) BOOL WINAPI Mine_VirtualLock( LPVOID lpAddress, SIZE_T dwSize ){
  if(ChessWrapperSentry::Wrap("VirtualLock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualLock");
   }
  return Real_VirtualLock(lpAddress, dwSize);
}
BOOL (WINAPI * Real_VirtualProtect)( LPVOID lpAddress, SIZE_T dwSize, DWORD flNewProtect, PDWORD lpflOldProtect )
  = VirtualProtect;

__declspec(dllexport) BOOL WINAPI Mine_VirtualProtect( LPVOID lpAddress, SIZE_T dwSize, DWORD flNewProtect, PDWORD lpflOldProtect ){
  if(ChessWrapperSentry::Wrap("VirtualProtect")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualProtect");
   }
  return Real_VirtualProtect(lpAddress, dwSize, flNewProtect, lpflOldProtect);
}
BOOL (WINAPI * Real_VirtualProtectEx)( HANDLE hProcess, LPVOID lpAddress, SIZE_T dwSize, DWORD flNewProtect, PDWORD lpflOldProtect )
  = VirtualProtectEx;

__declspec(dllexport) BOOL WINAPI Mine_VirtualProtectEx( HANDLE hProcess, LPVOID lpAddress, SIZE_T dwSize, DWORD flNewProtect, PDWORD lpflOldProtect ){
  if(ChessWrapperSentry::Wrap("VirtualProtectEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualProtectEx");
   }
  return Real_VirtualProtectEx(hProcess, lpAddress, dwSize, flNewProtect, lpflOldProtect);
}
SIZE_T (WINAPI * Real_VirtualQuery)( LPCVOID lpAddress, PMEMORY_BASIC_INFORMATION lpBuffer, SIZE_T dwLength )
  = VirtualQuery;

__declspec(dllexport) SIZE_T WINAPI Mine_VirtualQuery( LPCVOID lpAddress, PMEMORY_BASIC_INFORMATION lpBuffer, SIZE_T dwLength ){
  if(ChessWrapperSentry::Wrap("VirtualQuery")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualQuery");
   }
  return Real_VirtualQuery(lpAddress, lpBuffer, dwLength);
}
SIZE_T (WINAPI * Real_VirtualQueryEx)( HANDLE hProcess, LPCVOID lpAddress, PMEMORY_BASIC_INFORMATION lpBuffer, SIZE_T dwLength )
  = VirtualQueryEx;

__declspec(dllexport) SIZE_T WINAPI Mine_VirtualQueryEx( HANDLE hProcess, LPCVOID lpAddress, PMEMORY_BASIC_INFORMATION lpBuffer, SIZE_T dwLength ){
  if(ChessWrapperSentry::Wrap("VirtualQueryEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualQueryEx");
   }
  return Real_VirtualQueryEx(hProcess, lpAddress, lpBuffer, dwLength);
}
BOOL (WINAPI * Real_VirtualUnlock)( LPVOID lpAddress, SIZE_T dwSize )
  = VirtualUnlock;

__declspec(dllexport) BOOL WINAPI Mine_VirtualUnlock( LPVOID lpAddress, SIZE_T dwSize ){
  if(ChessWrapperSentry::Wrap("VirtualUnlock")){
     ChessWrapperSentry sentry;
     Chess::LogCall("VirtualUnlock");
   }
  return Real_VirtualUnlock(lpAddress, dwSize);
}
DWORD (WINAPI * Real_WTSGetActiveConsoleSessionId)()
  = WTSGetActiveConsoleSessionId;

__declspec(dllexport) DWORD WINAPI Mine_WTSGetActiveConsoleSessionId(){
  if(ChessWrapperSentry::Wrap("WTSGetActiveConsoleSessionId")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WTSGetActiveConsoleSessionId");
   }
  return Real_WTSGetActiveConsoleSessionId();
}
BOOL (WINAPI * Real_WaitCommEvent)( HANDLE hFile, LPDWORD lpEvtMask, LPOVERLAPPED lpOverlapped )
  = WaitCommEvent;

__declspec(dllexport) BOOL WINAPI Mine_WaitCommEvent( HANDLE hFile, LPDWORD lpEvtMask, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("WaitCommEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitCommEvent");
   }
  return Real_WaitCommEvent(hFile, lpEvtMask, lpOverlapped);
}
BOOL (WINAPI * Real_WaitForDebugEvent)( LPDEBUG_EVENT lpDebugEvent, DWORD dwMilliseconds )
  = WaitForDebugEvent;

__declspec(dllexport) BOOL WINAPI Mine_WaitForDebugEvent( LPDEBUG_EVENT lpDebugEvent, DWORD dwMilliseconds ){
  if(ChessWrapperSentry::Wrap("WaitForDebugEvent")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitForDebugEvent");
   }
  return Real_WaitForDebugEvent(lpDebugEvent, dwMilliseconds);
}
DWORD (WINAPI * Real_WaitForMultipleObjects)( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds )
   = WaitForMultipleObjects;

__declspec(dllexport) DWORD WINAPI Mine_WaitForMultipleObjects( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds ){
#ifdef WRAP_WaitForMultipleObjects
  if(ChessWrapperSentry::Wrap("WaitForMultipleObjects")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitForMultipleObjects");
     DWORD res = __wrapper_WaitForMultipleObjects(nCount, lpHandles, bWaitAll, dwMilliseconds);
     return res;
  }
#endif
  return Real_WaitForMultipleObjects(nCount, lpHandles, bWaitAll, dwMilliseconds);
}
DWORD (WINAPI * Real_WaitForMultipleObjectsEx)( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds, BOOL bAlertable )
   = WaitForMultipleObjectsEx;

__declspec(dllexport) DWORD WINAPI Mine_WaitForMultipleObjectsEx( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_WaitForMultipleObjectsEx
  if(ChessWrapperSentry::Wrap("WaitForMultipleObjectsEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitForMultipleObjectsEx");
     DWORD res = __wrapper_WaitForMultipleObjectsEx(nCount, lpHandles, bWaitAll, dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_WaitForMultipleObjectsEx(nCount, lpHandles, bWaitAll, dwMilliseconds, bAlertable);
}
DWORD (WINAPI * Real_WaitForSingleObject)( HANDLE hHandle, DWORD dwMilliseconds )
   = WaitForSingleObject;

__declspec(dllexport) DWORD WINAPI Mine_WaitForSingleObject( HANDLE hHandle, DWORD dwMilliseconds ){
#ifdef WRAP_WaitForSingleObject
  if(ChessWrapperSentry::Wrap("WaitForSingleObject")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitForSingleObject");
     DWORD res = __wrapper_WaitForSingleObject(hHandle, dwMilliseconds);
     return res;
  }
#endif
  return Real_WaitForSingleObject(hHandle, dwMilliseconds);
}
DWORD (WINAPI * Real_WaitForSingleObjectEx)( HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable )
   = WaitForSingleObjectEx;

__declspec(dllexport) DWORD WINAPI Mine_WaitForSingleObjectEx( HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_WaitForSingleObjectEx
  if(ChessWrapperSentry::Wrap("WaitForSingleObjectEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitForSingleObjectEx");
     DWORD res = __wrapper_WaitForSingleObjectEx(hHandle, dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_WaitForSingleObjectEx(hHandle, dwMilliseconds, bAlertable);
}
BOOL (WINAPI * Real_WaitNamedPipeA)( LPCSTR lpNamedPipeName, DWORD nTimeOut )
  = WaitNamedPipeA;

__declspec(dllexport) BOOL WINAPI Mine_WaitNamedPipeA( LPCSTR lpNamedPipeName, DWORD nTimeOut ){
  if(ChessWrapperSentry::Wrap("WaitNamedPipeA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitNamedPipeA");
   }
  return Real_WaitNamedPipeA(lpNamedPipeName, nTimeOut);
}
BOOL (WINAPI * Real_WaitNamedPipeW)( LPCWSTR lpNamedPipeName, DWORD nTimeOut )
  = WaitNamedPipeW;

__declspec(dllexport) BOOL WINAPI Mine_WaitNamedPipeW( LPCWSTR lpNamedPipeName, DWORD nTimeOut ){
  if(ChessWrapperSentry::Wrap("WaitNamedPipeW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WaitNamedPipeW");
   }
  return Real_WaitNamedPipeW(lpNamedPipeName, nTimeOut);
}
int (WINAPI * Real_WideCharToMultiByte)( UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar, LPSTR lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar, LPBOOL lpUsedDefaultChar)
  = WideCharToMultiByte;

__declspec(dllexport) int WINAPI Mine_WideCharToMultiByte( UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar, LPSTR lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar, LPBOOL lpUsedDefaultChar){
  if(ChessWrapperSentry::Wrap("WideCharToMultiByte")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WideCharToMultiByte");
   }
  return Real_WideCharToMultiByte(CodePage, dwFlags, lpWideCharStr, cchWideChar, lpMultiByteStr, cbMultiByte, lpDefaultChar, lpUsedDefaultChar);
}
UINT (WINAPI * Real_WinExec)( LPCSTR lpCmdLine, UINT uCmdShow )
  = WinExec;

__declspec(dllexport) UINT WINAPI Mine_WinExec( LPCSTR lpCmdLine, UINT uCmdShow ){
  if(ChessWrapperSentry::Wrap("WinExec")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WinExec");
   }
  return Real_WinExec(lpCmdLine, uCmdShow);
}
BOOL (WINAPI * Real_Wow64DisableWow64FsRedirection)( PVOID *OldValue )
  = Wow64DisableWow64FsRedirection;

__declspec(dllexport) BOOL WINAPI Mine_Wow64DisableWow64FsRedirection( PVOID *OldValue ){
  if(ChessWrapperSentry::Wrap("Wow64DisableWow64FsRedirection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("Wow64DisableWow64FsRedirection");
   }
  return Real_Wow64DisableWow64FsRedirection(OldValue);
}
BOOLEAN (WINAPI * Real_Wow64EnableWow64FsRedirection)( BOOLEAN Wow64FsEnableRedirection )
  = Wow64EnableWow64FsRedirection;

__declspec(dllexport) BOOLEAN WINAPI Mine_Wow64EnableWow64FsRedirection( BOOLEAN Wow64FsEnableRedirection ){
  if(ChessWrapperSentry::Wrap("Wow64EnableWow64FsRedirection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("Wow64EnableWow64FsRedirection");
   }
  return Real_Wow64EnableWow64FsRedirection(Wow64FsEnableRedirection);
}
BOOL (WINAPI * Real_Wow64RevertWow64FsRedirection)( PVOID OlValue )
  = Wow64RevertWow64FsRedirection;

__declspec(dllexport) BOOL WINAPI Mine_Wow64RevertWow64FsRedirection( PVOID OlValue ){
  if(ChessWrapperSentry::Wrap("Wow64RevertWow64FsRedirection")){
     ChessWrapperSentry sentry;
     Chess::LogCall("Wow64RevertWow64FsRedirection");
   }
  return Real_Wow64RevertWow64FsRedirection(OlValue);
}
BOOL (WINAPI * Real_WriteConsoleA)( HANDLE hConsoleOutput, const void *lpBuffer, DWORD nNumberOfCharsToWrite, LPDWORD lpNumberOfCharsWritten, LPVOID lpReserved )
  = WriteConsoleA;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleA( HANDLE hConsoleOutput, const void *lpBuffer, DWORD nNumberOfCharsToWrite, LPDWORD lpNumberOfCharsWritten, LPVOID lpReserved ){
  if(ChessWrapperSentry::Wrap("WriteConsoleA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleA");
   }
  return Real_WriteConsoleA(hConsoleOutput, lpBuffer, nNumberOfCharsToWrite, lpNumberOfCharsWritten, lpReserved);
}
BOOL (WINAPI * Real_WriteConsoleInputA)( HANDLE hConsoleInput, const INPUT_RECORD *lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsWritten )
  = WriteConsoleInputA;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleInputA( HANDLE hConsoleInput, const INPUT_RECORD *lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsWritten ){
  if(ChessWrapperSentry::Wrap("WriteConsoleInputA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleInputA");
   }
  return Real_WriteConsoleInputA(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsWritten);
}
BOOL (WINAPI * Real_WriteConsoleInputW)( HANDLE hConsoleInput, const INPUT_RECORD *lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsWritten )
  = WriteConsoleInputW;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleInputW( HANDLE hConsoleInput, const INPUT_RECORD *lpBuffer, DWORD nLength, LPDWORD lpNumberOfEventsWritten ){
  if(ChessWrapperSentry::Wrap("WriteConsoleInputW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleInputW");
   }
  return Real_WriteConsoleInputW(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsWritten);
}
BOOL (WINAPI * Real_WriteConsoleOutputA)( HANDLE hConsoleOutput, const CHAR_INFO *lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpWriteRegion )
  = WriteConsoleOutputA;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleOutputA( HANDLE hConsoleOutput, const CHAR_INFO *lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpWriteRegion ){
  if(ChessWrapperSentry::Wrap("WriteConsoleOutputA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleOutputA");
   }
  return Real_WriteConsoleOutputA(hConsoleOutput, lpBuffer, dwBufferSize, dwBufferCoord, lpWriteRegion);
}
BOOL (WINAPI * Real_WriteConsoleOutputAttribute)( HANDLE hConsoleOutput, const WORD *lpAttribute, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfAttrsWritten )
  = WriteConsoleOutputAttribute;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleOutputAttribute( HANDLE hConsoleOutput, const WORD *lpAttribute, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfAttrsWritten ){
  if(ChessWrapperSentry::Wrap("WriteConsoleOutputAttribute")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleOutputAttribute");
   }
  return Real_WriteConsoleOutputAttribute(hConsoleOutput, lpAttribute, nLength, dwWriteCoord, lpNumberOfAttrsWritten);
}
BOOL (WINAPI * Real_WriteConsoleOutputCharacterA)( HANDLE hConsoleOutput, LPCSTR lpCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten )
  = WriteConsoleOutputCharacterA;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleOutputCharacterA( HANDLE hConsoleOutput, LPCSTR lpCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten ){
  if(ChessWrapperSentry::Wrap("WriteConsoleOutputCharacterA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleOutputCharacterA");
   }
  return Real_WriteConsoleOutputCharacterA(hConsoleOutput, lpCharacter, nLength, dwWriteCoord, lpNumberOfCharsWritten);
}
BOOL (WINAPI * Real_WriteConsoleOutputCharacterW)( HANDLE hConsoleOutput, LPCWSTR lpCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten )
  = WriteConsoleOutputCharacterW;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleOutputCharacterW( HANDLE hConsoleOutput, LPCWSTR lpCharacter, DWORD nLength, COORD dwWriteCoord, LPDWORD lpNumberOfCharsWritten ){
  if(ChessWrapperSentry::Wrap("WriteConsoleOutputCharacterW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleOutputCharacterW");
   }
  return Real_WriteConsoleOutputCharacterW(hConsoleOutput, lpCharacter, nLength, dwWriteCoord, lpNumberOfCharsWritten);
}
BOOL (WINAPI * Real_WriteConsoleOutputW)( HANDLE hConsoleOutput, const CHAR_INFO *lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpWriteRegion )
  = WriteConsoleOutputW;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleOutputW( HANDLE hConsoleOutput, const CHAR_INFO *lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, PSMALL_RECT lpWriteRegion ){
  if(ChessWrapperSentry::Wrap("WriteConsoleOutputW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleOutputW");
   }
  return Real_WriteConsoleOutputW(hConsoleOutput, lpBuffer, dwBufferSize, dwBufferCoord, lpWriteRegion);
}
BOOL (WINAPI * Real_WriteConsoleW)( HANDLE hConsoleOutput, const void *lpBuffer, DWORD nNumberOfCharsToWrite, LPDWORD lpNumberOfCharsWritten, LPVOID lpReserved )
  = WriteConsoleW;

__declspec(dllexport) BOOL WINAPI Mine_WriteConsoleW( HANDLE hConsoleOutput, const void *lpBuffer, DWORD nNumberOfCharsToWrite, LPDWORD lpNumberOfCharsWritten, LPVOID lpReserved ){
  if(ChessWrapperSentry::Wrap("WriteConsoleW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteConsoleW");
   }
  return Real_WriteConsoleW(hConsoleOutput, lpBuffer, nNumberOfCharsToWrite, lpNumberOfCharsWritten, lpReserved);
}
BOOL (WINAPI * Real_WriteFile)( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, LPOVERLAPPED lpOverlapped )
   = WriteFile;

__declspec(dllexport) BOOL WINAPI Mine_WriteFile( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, LPOVERLAPPED lpOverlapped ){
#ifdef WRAP_WriteFile
  if(ChessWrapperSentry::Wrap("WriteFile")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteFile");
     BOOL res = __wrapper_WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);
     return res;
  }
#endif
  return Real_WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);
}
BOOL (WINAPI * Real_WriteFileEx)( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
   = WriteFileEx;

__declspec(dllexport) BOOL WINAPI Mine_WriteFileEx( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine ){
#ifdef WRAP_WriteFileEx
  if(ChessWrapperSentry::Wrap("WriteFileEx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteFileEx");
     BOOL res = __wrapper_WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, lpCompletionRoutine);
     return res;
  }
#endif
  return Real_WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, lpCompletionRoutine);
}
BOOL (WINAPI * Real_WriteFileGather)( HANDLE hFile, FILE_SEGMENT_ELEMENT aSegmentArray[], DWORD nNumberOfBytesToWrite, LPDWORD lpReserved, LPOVERLAPPED lpOverlapped )
  = WriteFileGather;

__declspec(dllexport) BOOL WINAPI Mine_WriteFileGather( HANDLE hFile, FILE_SEGMENT_ELEMENT aSegmentArray[], DWORD nNumberOfBytesToWrite, LPDWORD lpReserved, LPOVERLAPPED lpOverlapped ){
  if(ChessWrapperSentry::Wrap("WriteFileGather")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteFileGather");
   }
  return Real_WriteFileGather(hFile, aSegmentArray, nNumberOfBytesToWrite, lpReserved, lpOverlapped);
}
BOOL (WINAPI * Real_WritePrivateProfileSectionA)( LPCSTR lpAppName, LPCSTR lpString, LPCSTR lpFileName )
  = WritePrivateProfileSectionA;

__declspec(dllexport) BOOL WINAPI Mine_WritePrivateProfileSectionA( LPCSTR lpAppName, LPCSTR lpString, LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("WritePrivateProfileSectionA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WritePrivateProfileSectionA");
   }
  return Real_WritePrivateProfileSectionA(lpAppName, lpString, lpFileName);
}
BOOL (WINAPI * Real_WritePrivateProfileSectionW)( LPCWSTR lpAppName, LPCWSTR lpString, LPCWSTR lpFileName )
  = WritePrivateProfileSectionW;

__declspec(dllexport) BOOL WINAPI Mine_WritePrivateProfileSectionW( LPCWSTR lpAppName, LPCWSTR lpString, LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("WritePrivateProfileSectionW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WritePrivateProfileSectionW");
   }
  return Real_WritePrivateProfileSectionW(lpAppName, lpString, lpFileName);
}
BOOL (WINAPI * Real_WritePrivateProfileStringA)( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpString, LPCSTR lpFileName )
  = WritePrivateProfileStringA;

__declspec(dllexport) BOOL WINAPI Mine_WritePrivateProfileStringA( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpString, LPCSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("WritePrivateProfileStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WritePrivateProfileStringA");
   }
  return Real_WritePrivateProfileStringA(lpAppName, lpKeyName, lpString, lpFileName);
}
BOOL (WINAPI * Real_WritePrivateProfileStringW)( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpString, LPCWSTR lpFileName )
  = WritePrivateProfileStringW;

__declspec(dllexport) BOOL WINAPI Mine_WritePrivateProfileStringW( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpString, LPCWSTR lpFileName ){
  if(ChessWrapperSentry::Wrap("WritePrivateProfileStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WritePrivateProfileStringW");
   }
  return Real_WritePrivateProfileStringW(lpAppName, lpKeyName, lpString, lpFileName);
}
BOOL (WINAPI * Real_WritePrivateProfileStructA)( LPCSTR lpszSection, LPCSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCSTR szFile )
  = WritePrivateProfileStructA;

__declspec(dllexport) BOOL WINAPI Mine_WritePrivateProfileStructA( LPCSTR lpszSection, LPCSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCSTR szFile ){
  if(ChessWrapperSentry::Wrap("WritePrivateProfileStructA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WritePrivateProfileStructA");
   }
  return Real_WritePrivateProfileStructA(lpszSection, lpszKey, lpStruct, uSizeStruct, szFile);
}
BOOL (WINAPI * Real_WritePrivateProfileStructW)( LPCWSTR lpszSection, LPCWSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCWSTR szFile )
  = WritePrivateProfileStructW;

__declspec(dllexport) BOOL WINAPI Mine_WritePrivateProfileStructW( LPCWSTR lpszSection, LPCWSTR lpszKey, LPVOID lpStruct, UINT uSizeStruct, LPCWSTR szFile ){
  if(ChessWrapperSentry::Wrap("WritePrivateProfileStructW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WritePrivateProfileStructW");
   }
  return Real_WritePrivateProfileStructW(lpszSection, lpszKey, lpStruct, uSizeStruct, szFile);
}
BOOL (WINAPI * Real_WriteProcessMemory)( HANDLE hProcess, LPVOID lpBaseAddress, LPCVOID lpBuffer, SIZE_T nSize, SIZE_T * lpNumberOfBytesWritten )
  = WriteProcessMemory;

__declspec(dllexport) BOOL WINAPI Mine_WriteProcessMemory( HANDLE hProcess, LPVOID lpBaseAddress, LPCVOID lpBuffer, SIZE_T nSize, SIZE_T * lpNumberOfBytesWritten ){
  if(ChessWrapperSentry::Wrap("WriteProcessMemory")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteProcessMemory");
   }
  return Real_WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesWritten);
}
BOOL (WINAPI * Real_WriteProfileSectionA)( LPCSTR lpAppName, LPCSTR lpString )
  = WriteProfileSectionA;

__declspec(dllexport) BOOL WINAPI Mine_WriteProfileSectionA( LPCSTR lpAppName, LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("WriteProfileSectionA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteProfileSectionA");
   }
  return Real_WriteProfileSectionA(lpAppName, lpString);
}
BOOL (WINAPI * Real_WriteProfileSectionW)( LPCWSTR lpAppName, LPCWSTR lpString )
  = WriteProfileSectionW;

__declspec(dllexport) BOOL WINAPI Mine_WriteProfileSectionW( LPCWSTR lpAppName, LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("WriteProfileSectionW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteProfileSectionW");
   }
  return Real_WriteProfileSectionW(lpAppName, lpString);
}
BOOL (WINAPI * Real_WriteProfileStringA)( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpString )
  = WriteProfileStringA;

__declspec(dllexport) BOOL WINAPI Mine_WriteProfileStringA( LPCSTR lpAppName, LPCSTR lpKeyName, LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("WriteProfileStringA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteProfileStringA");
   }
  return Real_WriteProfileStringA(lpAppName, lpKeyName, lpString);
}
BOOL (WINAPI * Real_WriteProfileStringW)( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpString )
  = WriteProfileStringW;

__declspec(dllexport) BOOL WINAPI Mine_WriteProfileStringW( LPCWSTR lpAppName, LPCWSTR lpKeyName, LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("WriteProfileStringW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteProfileStringW");
   }
  return Real_WriteProfileStringW(lpAppName, lpKeyName, lpString);
}
DWORD (WINAPI * Real_WriteTapemark)( HANDLE hDevice, DWORD dwTapemarkType, DWORD dwTapemarkCount, BOOL bImmediate )
  = WriteTapemark;

__declspec(dllexport) DWORD WINAPI Mine_WriteTapemark( HANDLE hDevice, DWORD dwTapemarkType, DWORD dwTapemarkCount, BOOL bImmediate ){
  if(ChessWrapperSentry::Wrap("WriteTapemark")){
     ChessWrapperSentry sentry;
     Chess::LogCall("WriteTapemark");
   }
  return Real_WriteTapemark(hDevice, dwTapemarkType, dwTapemarkCount, bImmediate);
}
BOOL (WINAPI * Real_ZombifyActCtx)( HANDLE hActCtx )
  = ZombifyActCtx;

__declspec(dllexport) BOOL WINAPI Mine_ZombifyActCtx( HANDLE hActCtx ){
  if(ChessWrapperSentry::Wrap("ZombifyActCtx")){
     ChessWrapperSentry sentry;
     Chess::LogCall("ZombifyActCtx");
   }
  return Real_ZombifyActCtx(hActCtx);
}
long (WINAPI * Real__hread)( HFILE hFile, LPVOID lpBuffer, long lBytes )
  = _hread;

__declspec(dllexport) long WINAPI Mine__hread( HFILE hFile, LPVOID lpBuffer, long lBytes ){
  if(ChessWrapperSentry::Wrap("_hread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_hread");
   }
  return Real__hread(hFile, lpBuffer, lBytes);
}
long (WINAPI * Real__hwrite)( HFILE hFile, LPCCH lpBuffer, long lBytes )
  = _hwrite;

__declspec(dllexport) long WINAPI Mine__hwrite( HFILE hFile, LPCCH lpBuffer, long lBytes ){
  if(ChessWrapperSentry::Wrap("_hwrite")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_hwrite");
   }
  return Real__hwrite(hFile, lpBuffer, lBytes);
}
HFILE (WINAPI * Real__lclose)( HFILE hFile )
  = _lclose;

__declspec(dllexport) HFILE WINAPI Mine__lclose( HFILE hFile ){
  if(ChessWrapperSentry::Wrap("_lclose")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_lclose");
   }
  return Real__lclose(hFile);
}
HFILE (WINAPI * Real__lcreat)( LPCSTR lpPathName, int iAttribute )
  = _lcreat;

__declspec(dllexport) HFILE WINAPI Mine__lcreat( LPCSTR lpPathName, int iAttribute ){
  if(ChessWrapperSentry::Wrap("_lcreat")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_lcreat");
   }
  return Real__lcreat(lpPathName, iAttribute);
}
LONG (WINAPI * Real__llseek)( HFILE hFile, LONG lOffset, int iOrigin )
  = _llseek;

__declspec(dllexport) LONG WINAPI Mine__llseek( HFILE hFile, LONG lOffset, int iOrigin ){
  if(ChessWrapperSentry::Wrap("_llseek")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_llseek");
   }
  return Real__llseek(hFile, lOffset, iOrigin);
}
HFILE (WINAPI * Real__lopen)( LPCSTR lpPathName, int iReadWrite )
  = _lopen;

__declspec(dllexport) HFILE WINAPI Mine__lopen( LPCSTR lpPathName, int iReadWrite ){
  if(ChessWrapperSentry::Wrap("_lopen")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_lopen");
   }
  return Real__lopen(lpPathName, iReadWrite);
}
UINT (WINAPI * Real__lread)( HFILE hFile, LPVOID lpBuffer, UINT uBytes )
  = _lread;

__declspec(dllexport) UINT WINAPI Mine__lread( HFILE hFile, LPVOID lpBuffer, UINT uBytes ){
  if(ChessWrapperSentry::Wrap("_lread")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_lread");
   }
  return Real__lread(hFile, lpBuffer, uBytes);
}
UINT (WINAPI * Real__lwrite)( HFILE hFile, LPCCH lpBuffer, UINT uBytes )
  = _lwrite;

__declspec(dllexport) UINT WINAPI Mine__lwrite( HFILE hFile, LPCCH lpBuffer, UINT uBytes ){
  if(ChessWrapperSentry::Wrap("_lwrite")){
     ChessWrapperSentry sentry;
     Chess::LogCall("_lwrite");
   }
  return Real__lwrite(hFile, lpBuffer, uBytes);
}
LPSTR (WINAPI * Real_lstrcatA)( LPSTR lpString1, LPCSTR lpString2 )
  = lstrcatA;

__declspec(dllexport) LPSTR WINAPI Mine_lstrcatA( LPSTR lpString1, LPCSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcatA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcatA");
   }
  return Real_lstrcatA(lpString1, lpString2);
}
LPWSTR (WINAPI * Real_lstrcatW)( LPWSTR lpString1, LPCWSTR lpString2 )
  = lstrcatW;

__declspec(dllexport) LPWSTR WINAPI Mine_lstrcatW( LPWSTR lpString1, LPCWSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcatW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcatW");
   }
  return Real_lstrcatW(lpString1, lpString2);
}
int (WINAPI * Real_lstrcmpA)( LPCSTR lpString1, LPCSTR lpString2 )
  = lstrcmpA;

__declspec(dllexport) int WINAPI Mine_lstrcmpA( LPCSTR lpString1, LPCSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcmpA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcmpA");
   }
  return Real_lstrcmpA(lpString1, lpString2);
}
int (WINAPI * Real_lstrcmpW)( LPCWSTR lpString1, LPCWSTR lpString2 )
  = lstrcmpW;

__declspec(dllexport) int WINAPI Mine_lstrcmpW( LPCWSTR lpString1, LPCWSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcmpW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcmpW");
   }
  return Real_lstrcmpW(lpString1, lpString2);
}
int (WINAPI * Real_lstrcmpiA)( LPCSTR lpString1, LPCSTR lpString2 )
  = lstrcmpiA;

__declspec(dllexport) int WINAPI Mine_lstrcmpiA( LPCSTR lpString1, LPCSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcmpiA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcmpiA");
   }
  return Real_lstrcmpiA(lpString1, lpString2);
}
int (WINAPI * Real_lstrcmpiW)( LPCWSTR lpString1, LPCWSTR lpString2 )
  = lstrcmpiW;

__declspec(dllexport) int WINAPI Mine_lstrcmpiW( LPCWSTR lpString1, LPCWSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcmpiW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcmpiW");
   }
  return Real_lstrcmpiW(lpString1, lpString2);
}
LPSTR (WINAPI * Real_lstrcpyA)( LPSTR lpString1, LPCSTR lpString2 )
  = lstrcpyA;

__declspec(dllexport) LPSTR WINAPI Mine_lstrcpyA( LPSTR lpString1, LPCSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcpyA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcpyA");
   }
  return Real_lstrcpyA(lpString1, lpString2);
}
LPWSTR (WINAPI * Real_lstrcpyW)( LPWSTR lpString1, LPCWSTR lpString2 )
  = lstrcpyW;

__declspec(dllexport) LPWSTR WINAPI Mine_lstrcpyW( LPWSTR lpString1, LPCWSTR lpString2 ){
  if(ChessWrapperSentry::Wrap("lstrcpyW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcpyW");
   }
  return Real_lstrcpyW(lpString1, lpString2);
}
LPSTR (WINAPI * Real_lstrcpynA)( LPSTR lpString1, LPCSTR lpString2, int iMaxLength )
  = lstrcpynA;

__declspec(dllexport) LPSTR WINAPI Mine_lstrcpynA( LPSTR lpString1, LPCSTR lpString2, int iMaxLength ){
  if(ChessWrapperSentry::Wrap("lstrcpynA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcpynA");
   }
  return Real_lstrcpynA(lpString1, lpString2, iMaxLength);
}
LPWSTR (WINAPI * Real_lstrcpynW)( LPWSTR lpString1, LPCWSTR lpString2, int iMaxLength )
  = lstrcpynW;

__declspec(dllexport) LPWSTR WINAPI Mine_lstrcpynW( LPWSTR lpString1, LPCWSTR lpString2, int iMaxLength ){
  if(ChessWrapperSentry::Wrap("lstrcpynW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrcpynW");
   }
  return Real_lstrcpynW(lpString1, lpString2, iMaxLength);
}
int (WINAPI * Real_lstrlenA)( LPCSTR lpString )
  = lstrlenA;

__declspec(dllexport) int WINAPI Mine_lstrlenA( LPCSTR lpString ){
  if(ChessWrapperSentry::Wrap("lstrlenA")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrlenA");
   }
  return Real_lstrlenA(lpString);
}
int (WINAPI * Real_lstrlenW)( LPCWSTR lpString )
  = lstrlenW;

__declspec(dllexport) int WINAPI Mine_lstrlenW( LPCWSTR lpString ){
  if(ChessWrapperSentry::Wrap("lstrlenW")){
     ChessWrapperSentry sentry;
     Chess::LogCall("lstrlenW");
   }
  return Real_lstrlenW(lpString);
}
LONG AttachDetours(){
   DetourTransactionBegin();
   ChessDetourAttach(&(PVOID&)Real_CreateIoCompletionPort, Mine_CreateIoCompletionPort);
   ChessDetourAttach(&(PVOID&)Real_CreateThread, Mine_CreateThread);
   ChessDetourAttach(&(PVOID&)Real_CreateTimerQueue, Mine_CreateTimerQueue);
   ChessDetourAttach(&(PVOID&)Real_CreateTimerQueueTimer, Mine_CreateTimerQueueTimer);
   ChessDetourAttach(&(PVOID&)Real_DeleteTimerQueue, Mine_DeleteTimerQueue);
   ChessDetourAttach(&(PVOID&)Real_DeleteTimerQueueEx, Mine_DeleteTimerQueueEx);
   ChessDetourAttach(&(PVOID&)Real_DeleteTimerQueueTimer, Mine_DeleteTimerQueueTimer);
   ChessDetourAttach(&(PVOID&)Real_DuplicateHandle, Mine_DuplicateHandle);
   ChessDetourAttach(&(PVOID&)Real_EnterCriticalSection, Mine_EnterCriticalSection);
   ChessDetourAttach(&(PVOID&)Real_GetQueuedCompletionStatus, Mine_GetQueuedCompletionStatus);
   ChessDetourAttach(&(PVOID&)Real_InterlockedCompareExchange, Mine_InterlockedCompareExchange);
   ChessDetourAttach(&(PVOID&)Real_InterlockedCompareExchange64, Mine_InterlockedCompareExchange64);
   ChessDetourAttach(&(PVOID&)Real_InterlockedDecrement, Mine_InterlockedDecrement);
   ChessDetourAttach(&(PVOID&)Real_InterlockedExchange, Mine_InterlockedExchange);
   ChessDetourAttach(&(PVOID&)Real_InterlockedIncrement, Mine_InterlockedIncrement);
   ChessDetourAttach(&(PVOID&)Real_LeaveCriticalSection, Mine_LeaveCriticalSection);
   ChessDetourAttach(&(PVOID&)Real_PostQueuedCompletionStatus, Mine_PostQueuedCompletionStatus);
   ChessDetourAttach(&(PVOID&)Real_PulseEvent, Mine_PulseEvent);
   ChessDetourAttach(&(PVOID&)Real_QueueUserAPC, Mine_QueueUserAPC);
   ChessDetourAttach(&(PVOID&)Real_QueueUserWorkItem, Mine_QueueUserWorkItem);
   ChessDetourAttach(&(PVOID&)Real_ReadFile, Mine_ReadFile);
   ChessDetourAttach(&(PVOID&)Real_ReadFileEx, Mine_ReadFileEx);
   ChessDetourAttach(&(PVOID&)Real_ReleaseMutex, Mine_ReleaseMutex);
   ChessDetourAttach(&(PVOID&)Real_ReleaseSemaphore, Mine_ReleaseSemaphore);
   ChessDetourAttach(&(PVOID&)Real_ResetEvent, Mine_ResetEvent);
   ChessDetourAttach(&(PVOID&)Real_ResumeThread, Mine_ResumeThread);
   ChessDetourAttach(&(PVOID&)Real_SetEvent, Mine_SetEvent);
   ChessDetourAttach(&(PVOID&)Real_SignalObjectAndWait, Mine_SignalObjectAndWait);
   ChessDetourAttach(&(PVOID&)Real_Sleep, Mine_Sleep);
   ChessDetourAttach(&(PVOID&)Real_SleepEx, Mine_SleepEx);
   ChessDetourAttach(&(PVOID&)Real_SuspendThread, Mine_SuspendThread);
   ChessDetourAttach(&(PVOID&)Real_SwitchToThread, Mine_SwitchToThread);
   ChessDetourAttach(&(PVOID&)Real_TryEnterCriticalSection, Mine_TryEnterCriticalSection);
   ChessDetourAttach(&(PVOID&)Real_WaitForMultipleObjects, Mine_WaitForMultipleObjects);
   ChessDetourAttach(&(PVOID&)Real_WaitForMultipleObjectsEx, Mine_WaitForMultipleObjectsEx);
   ChessDetourAttach(&(PVOID&)Real_WaitForSingleObject, Mine_WaitForSingleObject);
   ChessDetourAttach(&(PVOID&)Real_WaitForSingleObjectEx, Mine_WaitForSingleObjectEx);
   ChessDetourAttach(&(PVOID&)Real_WriteFile, Mine_WriteFile);
   ChessDetourAttach(&(PVOID&)Real_WriteFileEx, Mine_WriteFileEx);
   return ChessDetourTransactionCommit();
}
LONG DetachDetours(){
   DetourTransactionBegin();
   ChessDetourDetach(&(PVOID&)Real_CreateIoCompletionPort, Mine_CreateIoCompletionPort);
   ChessDetourDetach(&(PVOID&)Real_CreateThread, Mine_CreateThread);
   ChessDetourDetach(&(PVOID&)Real_CreateTimerQueue, Mine_CreateTimerQueue);
   ChessDetourDetach(&(PVOID&)Real_CreateTimerQueueTimer, Mine_CreateTimerQueueTimer);
   ChessDetourDetach(&(PVOID&)Real_DeleteTimerQueue, Mine_DeleteTimerQueue);
   ChessDetourDetach(&(PVOID&)Real_DeleteTimerQueueEx, Mine_DeleteTimerQueueEx);
   ChessDetourDetach(&(PVOID&)Real_DeleteTimerQueueTimer, Mine_DeleteTimerQueueTimer);
   ChessDetourDetach(&(PVOID&)Real_DuplicateHandle, Mine_DuplicateHandle);
   ChessDetourDetach(&(PVOID&)Real_EnterCriticalSection, Mine_EnterCriticalSection);
   ChessDetourDetach(&(PVOID&)Real_GetQueuedCompletionStatus, Mine_GetQueuedCompletionStatus);
   ChessDetourDetach(&(PVOID&)Real_InterlockedCompareExchange, Mine_InterlockedCompareExchange);
   ChessDetourDetach(&(PVOID&)Real_InterlockedCompareExchange64, Mine_InterlockedCompareExchange64);
   ChessDetourDetach(&(PVOID&)Real_InterlockedDecrement, Mine_InterlockedDecrement);
   ChessDetourDetach(&(PVOID&)Real_InterlockedExchange, Mine_InterlockedExchange);
   ChessDetourDetach(&(PVOID&)Real_InterlockedIncrement, Mine_InterlockedIncrement);
   ChessDetourDetach(&(PVOID&)Real_LeaveCriticalSection, Mine_LeaveCriticalSection);
   ChessDetourDetach(&(PVOID&)Real_PostQueuedCompletionStatus, Mine_PostQueuedCompletionStatus);
   ChessDetourDetach(&(PVOID&)Real_PulseEvent, Mine_PulseEvent);
   ChessDetourDetach(&(PVOID&)Real_QueueUserAPC, Mine_QueueUserAPC);
   ChessDetourDetach(&(PVOID&)Real_QueueUserWorkItem, Mine_QueueUserWorkItem);
   ChessDetourDetach(&(PVOID&)Real_ReadFile, Mine_ReadFile);
   ChessDetourDetach(&(PVOID&)Real_ReadFileEx, Mine_ReadFileEx);
   ChessDetourDetach(&(PVOID&)Real_ReleaseMutex, Mine_ReleaseMutex);
   ChessDetourDetach(&(PVOID&)Real_ReleaseSemaphore, Mine_ReleaseSemaphore);
   ChessDetourDetach(&(PVOID&)Real_ResetEvent, Mine_ResetEvent);
   ChessDetourDetach(&(PVOID&)Real_ResumeThread, Mine_ResumeThread);
   ChessDetourDetach(&(PVOID&)Real_SetEvent, Mine_SetEvent);
   ChessDetourDetach(&(PVOID&)Real_SignalObjectAndWait, Mine_SignalObjectAndWait);
   ChessDetourDetach(&(PVOID&)Real_Sleep, Mine_Sleep);
   ChessDetourDetach(&(PVOID&)Real_SleepEx, Mine_SleepEx);
   ChessDetourDetach(&(PVOID&)Real_SuspendThread, Mine_SuspendThread);
   ChessDetourDetach(&(PVOID&)Real_SwitchToThread, Mine_SwitchToThread);
   ChessDetourDetach(&(PVOID&)Real_TryEnterCriticalSection, Mine_TryEnterCriticalSection);
   ChessDetourDetach(&(PVOID&)Real_WaitForMultipleObjects, Mine_WaitForMultipleObjects);
   ChessDetourDetach(&(PVOID&)Real_WaitForMultipleObjectsEx, Mine_WaitForMultipleObjectsEx);
   ChessDetourDetach(&(PVOID&)Real_WaitForSingleObject, Mine_WaitForSingleObject);
   ChessDetourDetach(&(PVOID&)Real_WaitForSingleObjectEx, Mine_WaitForSingleObjectEx);
   ChessDetourDetach(&(PVOID&)Real_WriteFile, Mine_WriteFile);
   ChessDetourDetach(&(PVOID&)Real_WriteFileEx, Mine_WriteFileEx);
   return ChessDetourTransactionCommit();
}
