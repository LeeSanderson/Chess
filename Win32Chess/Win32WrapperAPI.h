/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#ifndef CHESSEXPORT
#define CHESSEXPORT WIN32CHESS_API
#endif

/// This file contains the wrappers for all the Win32 functions that CHESS traps

CHESSEXPORT DWORD WINAPI __wrapper_WaitForSingleObject(
  HANDLE hHandle,
  DWORD dwMilliseconds);

CHESSEXPORT DWORD WINAPI __wrapper_WaitForMultipleObjects(
  DWORD nCount, 
  CONST HANDLE* lpHandles, 
  BOOL fWaitAll, 
  DWORD dwMilliseconds);
 
CHESSEXPORT HANDLE WINAPI __wrapper_CreateThread(
  LPSECURITY_ATTRIBUTES lpThreadAttributes,
  SIZE_T dwStackSize,
  LPTHREAD_START_ROUTINE lpStartAddress,
  LPVOID lpParameter,
  DWORD dwCreationFlags,
  LPDWORD lpThreadId);

CHESSEXPORT VOID WINAPI __wrapper_ExitThread(
  DWORD dwExitCode);

CHESSEXPORT VOID WINAPI __wrapper_FreeLibraryAndExitThread(
    HMODULE hModule,
    DWORD dwExitCode
    );


#ifndef UNDER_CE
CHESSEXPORT BOOL WINAPI __wrapper_QueueUserWorkItem(
  LPTHREAD_START_ROUTINE Function,
  PVOID Context,
  ULONG Flags);
#endif

CHESSEXPORT DWORD WINAPI __wrapper_SuspendThread(HANDLE hThread);

CHESSEXPORT DWORD WINAPI __wrapper_ResumeThread(HANDLE hThread);

CHESSEXPORT void WINAPI __wrapper_InitializeCriticalSection(LPCRITICAL_SECTION lpCriticalSection);

CHESSEXPORT BOOL WINAPI __wrapper_InitializeCriticalSectionAndSpinCount(LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount);

CHESSEXPORT BOOL WINAPI __wrapper_InitializeCriticalSectionEx(LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount, DWORD Flags);

CHESSEXPORT void WINAPI __wrapper_EnterCriticalSection(LPCRITICAL_SECTION lpCriticalSection);

CHESSEXPORT BOOL WINAPI __wrapper_TryEnterCriticalSection( LPCRITICAL_SECTION lpCriticalSection );

CHESSEXPORT void WINAPI __wrapper_LeaveCriticalSection(LPCRITICAL_SECTION lpCriticalSection);

CHESSEXPORT void WINAPI __wrapper_DeleteCriticalSection(__inout  LPCRITICAL_SECTION lpCriticalSection);

//CHESSEXPORT VOID WINAPI __wrapper_InitializeSRWLock(PSRWLOCK SRWLock);

//CHESSEXPORT VOID WINAPI __wrapper_AcquireSRWLockExclusive(PSRWLOCK SRWLock);
//
//CHESSEXPORT VOID WINAPI __wrapper_ReleaseSRWLockExclusive(PSRWLOCK SRWLock);
//
//CHESSEXPORT VOID WINAPI __wrapper_AcquireSRWLockShared(PSRWLOCK SRWLock);
//
//CHESSEXPORT VOID WINAPI __wrapper_ReleaseSRWLockShared(PSRWLOCK SRWLock);
//
//CHESSEXPORT VOID WINAPI __wrapper_InitializeConditionVariable(__out  PCONDITION_VARIABLE ConditionVariable);
//
//CHESSEXPORT BOOL WINAPI __wrapper_SleepConditionVariableCS(
//  __inout  PCONDITION_VARIABLE ConditionVariable,
//  __inout  PCRITICAL_SECTION CriticalSection,
//  __in     DWORD dwMilliseconds);
//
//CHESSEXPORT BOOL WINAPI __wrapper_SleepConditionVariableSRW(
//  __inout PCONDITION_VARIABLE ConditionVariable,
//  __inout PSRWLOCK SRWLock,
//  __in DWORD dwMilliseconds,
//  __in ULONG Flags);
//
//CHESSEXPORT VOID WINAPI __wrapper_WakeAllConditionVariable(__inout PCONDITION_VARIABLE ConditionVariable);
//
//CHESSEXPORT VOID WINAPI __wrapper_WakeConditionVariable(__inout PCONDITION_VARIABLE ConditionVariable);

CHESSEXPORT void WINAPI __wrapper_Sleep(DWORD dwMilliseconds);

CHESSEXPORT BOOL WINAPI __wrapper_SetEvent(HANDLE hEvent);

CHESSEXPORT LONG WINAPI __wrapper_InterlockedIncrement(LONG volatile *lpAddend);

CHESSEXPORT LONG WINAPI __wrapper_InterlockedDecrement(LONG volatile *lpAddend);

CHESSEXPORT LONG WINAPI __wrapper_InterlockedExchange(LONG volatile *Target, LONG Value);

CHESSEXPORT LONG WINAPI __wrapper_InterlockedCompareExchange(LONG volatile *Destination, LONG Exchange, LONG Comperand);

CHESSEXPORT BOOL WINAPI __wrapper_ResetEvent( HANDLE hEvent );

CHESSEXPORT HANDLE WINAPI __wrapper_CreateEventA(LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCSTR lpName);
CHESSEXPORT HANDLE WINAPI __wrapper_CreateEventW(LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCWSTR lpName);

CHESSEXPORT HANDLE WINAPI __wrapper_CreateMutexA(LPSECURITY_ATTRIBUTES lpMutexAttributes, BOOL bInitialOwner, LPCSTR lpName);
CHESSEXPORT HANDLE WINAPI __wrapper_CreateMutexW(LPSECURITY_ATTRIBUTES lpMutexAttributes, BOOL bInitialOwner, LPCWSTR lpName);

CHESSEXPORT HANDLE WINAPI __wrapper_OpenMutexA(DWORD dwDesiredAccess, BOOL bInheritHandle, LPCSTR lpName);
CHESSEXPORT HANDLE WINAPI __wrapper_OpenMutexW(DWORD dwDesiredAccess, BOOL bInheritHandle, LPCWSTR lpName);

CHESSEXPORT BOOL WINAPI __wrapper_PulseEvent( HANDLE hEvent );

CHESSEXPORT BOOL WINAPI __wrapper_ReleaseMutex( HANDLE hMutex );

CHESSEXPORT BOOL WINAPI __wrapper_ReleaseSemaphore( HANDLE hSemaphore, LONG lReleaseCount, LPLONG lpPreviousCount );

CHESSEXPORT DWORD WINAPI __wrapper_SleepEx( DWORD dwMilliseconds, BOOL bAlertable );

CHESSEXPORT BOOL WINAPI __wrapper_SwitchToThread( void );

CHESSEXPORT DWORD WINAPI __wrapper_WaitForSingleObjectEx( HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable );

CHESSEXPORT DWORD WINAPI __wrapper_WaitForMultipleObjectsEx(
  DWORD nCount, 
  CONST HANDLE* lpHandles, 
  BOOL bWaitAll, 
  DWORD dwMilliseconds,
  BOOL bAlertable);

#ifndef UNDER_CE
CHESSEXPORT HANDLE WINAPI __wrapper_CreateTimerQueue( void );

CHESSEXPORT BOOL WINAPI __wrapper_CreateTimerQueueTimer( PHANDLE phNewTimer, HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, ULONG Flags );

CHESSEXPORT BOOL WINAPI __wrapper_DeleteTimerQueueTimer( HANDLE TimerQueue, HANDLE Timer, HANDLE CompletionEvent );

CHESSEXPORT BOOL WINAPI __wrapper_DeleteTimerQueueEx( HANDLE TimerQueue, HANDLE CompletionEvent );

CHESSEXPORT BOOL WINAPI __wrapper_DeleteTimerQueue( HANDLE TimerQueue );

CHESSEXPORT BOOL WINAPI __wrapper_GetQueuedCompletionStatus( HANDLE CompletionPort, LPDWORD lpNumberOfBytesTransferred, PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds );

CHESSEXPORT BOOL WINAPI __wrapper_PostQueuedCompletionStatus( HANDLE CompletionPort, DWORD dwNumberOfBytesTransferred, ULONG_PTR dwCompletionKey, LPOVERLAPPED lpOverlapped );

CHESSEXPORT DWORD WINAPI __wrapper_SignalObjectAndWait( HANDLE hObjectToSignal, HANDLE hObjectToWaitOn, DWORD dwMilliseconds, BOOL bAlertable );

CHESSEXPORT DWORD WINAPI __wrapper_QueueUserAPC( PAPCFUNC pfnAPC, HANDLE hThread, ULONG_PTR dwData );

CHESSEXPORT BOOL WINAPI __wrapper_ReadFileEx( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

CHESSEXPORT BOOL WINAPI __wrapper_WriteFileEx( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

CHESSEXPORT HANDLE WINAPI __wrapper_CreateIoCompletionPort( HANDLE FileHandle, HANDLE ExistingCompletionPort, ULONG_PTR CompletionKey, DWORD NumberOfConcurrentThreads );

CHESSEXPORT BOOL WINAPI __wrapper_ReadFile( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, LPOVERLAPPED lpOverlapped );

CHESSEXPORT BOOL WINAPI __wrapper_WriteFile( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, LPOVERLAPPED lpOverlapped );
#endif

CHESSEXPORT BOOL WINAPI __wrapper_DuplicateHandle( HANDLE hSourceProcessHandle, HANDLE hSourceHandle, HANDLE hTargetProcessHandle, LPHANDLE lpTargetHandle, DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwOptions );
 
#ifndef UNDER_CE
CHESSEXPORT BOOL WINAPI __wrapper_DeviceIoControl(HANDLE hDevice, DWORD dwIoControlCode, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD BufferSize, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped);
#endif

CHESSEXPORT void *__wrapper_malloc(size_t _Size);

CHESSEXPORT void *__wrapper_realloc(void *_Memory, size_t _Size);

CHESSEXPORT void __wrapper_free(void *_Memory);

CHESSEXPORT void *__wrapper_HeapAlloc(HANDLE hHeap, DWORD dwFlags, SIZE_T _Size);

CHESSEXPORT void *__wrapper_HeapReAlloc(HANDLE hHeap, DWORD dwFlags, void *_Memory, SIZE_T _Size);

CHESSEXPORT BOOL __wrapper_HeapFree(HANDLE hHeap, DWORD dwFlags, void *_Memory);