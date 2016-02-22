/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/****************************   System Calls Wrappers *********************************/

#include "Win32Base.h"

//#if _MSC_VER >= 1300
//#include <winsock2.h>
//#endif
#include <windows.h>
#include "ChessAssert.h"
#include <stdio.h>

#include "Chess.h"
#include "Win32SyncManager.h"
#include "Win32WrapperAPI.h"
#include "IChessWrapper.h"
#ifndef UNDER_CE
#include <process.h>
#endif

#include <map>

extern "C" void NTSYSAPI NTAPI RtlUnwind(PVOID endframe, PVOID eip, PEXCEPTION_RECORD rec, PVOID retval);

#define WRAP_AcquireSRWLockExclusive
#define WRAP_AcquireSRWLockShared
#define WRAP_CreateIoCompletionPort
#define WRAP_CreateThread
#define WRAP_CreateTimerQueue
#define WRAP_CreateTimerQueueTimer
#define WRAP_DeleteCriticalSection
#define WRAP_DeleteTimerQueue
#define WRAP_DeleteTimerQueueEx
#define WRAP_DeleteTimerQueueTimer
#define WRAP_DuplicateHandle
#define WRAP_ExitThread
#define WRAP_EnterCriticalSection
#define WRAP_GetQueuedCompletionStatus
#define WRAP_InitializeConditionVariable
#define WRAP_InitializeCriticalSection
#define WRAP_InitializeCriticalSectionAndSpinCount
#define WRAP_InitializeCriticalSectionEx
#define WRAP_InitializeSRWLock
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
#define WRAP_ReleaseSRWLockExclusive
#define WRAP_ReleaseSRWLockShared
#define WRAP_ResetEvent
#define WRAP_ResumeThread
#define WRAP_SetEvent
#define WRAP_CreateEvent
#define WRAP_CreateEventEx
#define WRAP_CreateMutex
#define WRAP_CreateMutexEx
#define WRAP_OpenMutex
#define WRAP_CreateSemaphore
#define WRAP_CreateSemaphoreEx
#define WRAP_SignalObjectAndWait
#define WRAP_Sleep
#define WRAP_SleepConditionVariableCS
#define WRAP_SleepConditionVariableSRW
#define WRAP_SleepEx
#define WRAP_SuspendThread
#define WRAP_SwitchToThread
#define WRAP_TryEnterCriticalSection
#define WRAP_WaitForMultipleObjects
#define WRAP_WaitForMultipleObjectsEx
#define WRAP_WaitForSingleObject
#define WRAP_WaitForSingleObjectEx
#define WRAP_WakeAllConditionVariable
#define WRAP_WakeConditionVariable
#define WRAP_WriteFile
#define WRAP_WriteFileEx

#define WRAP_DeviceIoControl
#define WRAP_CreateFileW

extern "C"{

HANDLE (WINAPI * Real_CreateIoCompletionPort)( HANDLE FileHandle, HANDLE ExistingCompletionPort, ULONG_PTR CompletionKey, DWORD NumberOfConcurrentThreads )
   = CreateIoCompletionPort;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateIoCompletionPort( HANDLE FileHandle, HANDLE ExistingCompletionPort, ULONG_PTR CompletionKey, DWORD NumberOfConcurrentThreads ){
#ifdef WRAP_CreateIoCompletionPort
  if(ChessWrapperSentry::Wrap("CreateIoCompletionPort")){
     ChessWrapperSentry sentry;
     HANDLE res = __wrapper_CreateIoCompletionPort(FileHandle, ExistingCompletionPort, CompletionKey, NumberOfConcurrentThreads);
     return res;
  }
#endif
  return Real_CreateIoCompletionPort(FileHandle, ExistingCompletionPort, CompletionKey, NumberOfConcurrentThreads);
}

HANDLE (WINAPI * Real_CreateThread)( LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, LPVOID lpParameter, DWORD dwCreationFlags, LPDWORD lpThreadId )
   = CreateThread;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateThread( LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, LPVOID lpParameter, DWORD dwCreationFlags, LPDWORD lpThreadId ){
#ifdef WRAP_CreateThread
  if(ChessWrapperSentry::Wrap("CreateThread")){
     ChessWrapperSentry sentry;
     HANDLE res = __wrapper_CreateThread(lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);
     return res;
  }
#endif
  return Real_CreateThread(lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);
}

VOID (WINAPI * Real_ExitThread)( DWORD dwExitCode )
   = ExitThread;

__declspec(dllexport) VOID WINAPI __Chess_ExitThread( DWORD dwExitCode ){
#ifdef WRAP_ExitThread
  if(ChessWrapperSentry::Wrap("ExitThread")){
     ChessWrapperSentry sentry;
     __wrapper_ExitThread(dwExitCode);
     return;
  }
#endif
  return Real_ExitThread(dwExitCode);
}


HANDLE (WINAPI * Real_CreateTimerQueue)( void )
   = CreateTimerQueue;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateTimerQueue( void ){
#ifdef WRAP_CreateTimerQueue
  if(ChessWrapperSentry::Wrap("CreateTimerQueue")){
     ChessWrapperSentry sentry;
     HANDLE res = __wrapper_CreateTimerQueue();
     return res;
  }
#endif
  return Real_CreateTimerQueue();
}
BOOL (WINAPI * Real_CreateTimerQueueTimer)( PHANDLE phNewTimer, HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, ULONG Flags )
   = CreateTimerQueueTimer;

__declspec(dllexport) BOOL WINAPI __Chess_CreateTimerQueueTimer( PHANDLE phNewTimer, HANDLE TimerQueue, WAITORTIMERCALLBACK Callback, PVOID Parameter, DWORD DueTime, DWORD Period, ULONG Flags ){
#ifdef WRAP_CreateTimerQueueTimer
  if(ChessWrapperSentry::Wrap("CreateTimerQueueTimer")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_CreateTimerQueueTimer(phNewTimer, TimerQueue, Callback, Parameter, DueTime, Period, Flags);
     return res;
  }
#endif
  return Real_CreateTimerQueueTimer(phNewTimer, TimerQueue, Callback, Parameter, DueTime, Period, Flags);
}

//void (WINAPI * Real_DeleteCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
//   = DeleteCriticalSection;
//
//__declspec(dllexport) void WINAPI __Chess_DeleteCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
//#ifdef WRAP_DeleteCriticalSection
//  if(ChessWrapperSentry::Wrap("DeleteCriticalSection")){
//     ChessWrapperSentry sentry;
//     __wrapper_DeleteCriticalSection(lpCriticalSection);
//     return;
//  }
//#endif
//  return Real_DeleteCriticalSection(lpCriticalSection);
//}

BOOL (WINAPI * Real_DeleteTimerQueue)( HANDLE TimerQueue )
   = DeleteTimerQueue;

__declspec(dllexport) BOOL WINAPI __Chess_DeleteTimerQueue( HANDLE TimerQueue ){
#ifdef WRAP_DeleteTimerQueue
  if(ChessWrapperSentry::Wrap("DeleteTimerQueue")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_DeleteTimerQueue(TimerQueue);
     return res;
  }
#endif
  return Real_DeleteTimerQueue(TimerQueue);
}
BOOL (WINAPI * Real_DeleteTimerQueueEx)( HANDLE TimerQueue, HANDLE CompletionEvent )
   = DeleteTimerQueueEx;

__declspec(dllexport) BOOL WINAPI __Chess_DeleteTimerQueueEx( HANDLE TimerQueue, HANDLE CompletionEvent ){
#ifdef WRAP_DeleteTimerQueueEx
  if(ChessWrapperSentry::Wrap("DeleteTimerQueueEx")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_DeleteTimerQueueEx(TimerQueue, CompletionEvent);
     return res;
  }
#endif
  return Real_DeleteTimerQueueEx(TimerQueue, CompletionEvent);
}
BOOL (WINAPI * Real_DeleteTimerQueueTimer)( HANDLE TimerQueue, HANDLE Timer, HANDLE CompletionEvent )
   = DeleteTimerQueueTimer;

__declspec(dllexport) BOOL WINAPI __Chess_DeleteTimerQueueTimer( HANDLE TimerQueue, HANDLE Timer, HANDLE CompletionEvent ){
#ifdef WRAP_DeleteTimerQueueTimer
  if(ChessWrapperSentry::Wrap("DeleteTimerQueueTimer")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_DeleteTimerQueueTimer(TimerQueue, Timer, CompletionEvent);
     return res;
  }
#endif
  return Real_DeleteTimerQueueTimer(TimerQueue, Timer, CompletionEvent);
}
BOOL (WINAPI * Real_DuplicateHandle)( HANDLE hSourceProcessHandle, HANDLE hSourceHandle, HANDLE hTargetProcessHandle, LPHANDLE lpTargetHandle, DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwOptions )
   = DuplicateHandle;

__declspec(dllexport) BOOL WINAPI __Chess_DuplicateHandle( HANDLE hSourceProcessHandle, HANDLE hSourceHandle, HANDLE hTargetProcessHandle, LPHANDLE lpTargetHandle, DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwOptions ){
#ifdef WRAP_DuplicateHandle
  if(ChessWrapperSentry::Wrap("DuplicateHandle")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, lpTargetHandle, dwDesiredAccess, bInheritHandle, dwOptions);
     return res;
  }
#endif
  BOOL res = Real_DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, lpTargetHandle, dwDesiredAccess, bInheritHandle, dwOptions);
  return res;
}

void (WINAPI * Real_EnterCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
   = EnterCriticalSection;

__declspec(dllexport) void WINAPI __Chess_EnterCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
#ifdef WRAP_EnterCriticalSection
  if(ChessWrapperSentry::Wrap("EnterCriticalSection")){
     ChessWrapperSentry sentry;
     __wrapper_EnterCriticalSection(lpCriticalSection);
     return;
  }
#endif
  return Real_EnterCriticalSection(lpCriticalSection);
}

BOOL (WINAPI * Real_GetQueuedCompletionStatus)( HANDLE CompletionPort, LPDWORD lpNumberOfBytesTransferred, PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds )
   = GetQueuedCompletionStatus;

__declspec(dllexport) BOOL WINAPI __Chess_GetQueuedCompletionStatus( HANDLE CompletionPort, LPDWORD lpNumberOfBytesTransferred, PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds ){
#ifdef WRAP_GetQueuedCompletionStatus
  if(ChessWrapperSentry::Wrap("GetQueuedCompletionStatus")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_GetQueuedCompletionStatus(CompletionPort, lpNumberOfBytesTransferred, lpCompletionKey, lpOverlapped, dwMilliseconds);
     return res;
  }
#endif
  return Real_GetQueuedCompletionStatus(CompletionPort, lpNumberOfBytesTransferred, lpCompletionKey, lpOverlapped, dwMilliseconds);
}

//VOID (WINAPI *Real_InitializeConditionVariable)(PCONDITION_VARIABLE ConditionVariable) = InitializeConditionVariable;
//
//VOID WINAPI __Chess_InitializeConditionVariable(PCONDITION_VARIABLE ConditionVariable) {
//#ifdef WRAP_InitializeConditionVariable
//	if(ChessWrapperSentry::Wrap("InitializeConditionVariable")) {
//		ChessWrapperSentry sentry;
//		__wrapper_InitializeConditionVariable(ConditionVariable);
//		return;
//	}
//#endif
//	Real_InitializeConditionVariable(ConditionVariable);
//}
//
//void (WINAPI * Real_InitializeCriticalSection)(LPCRITICAL_SECTION lpCriticalSection) = InitializeCriticalSection;
//
//void WINAPI __Chess_InitializeCriticalSection(LPCRITICAL_SECTION lpCriticalSection) {
//#ifdef WRAP_InitializeCriticalSection
//	if(ChessWrapperSentry::Wrap("InitializeCriticalSection")) {
//		ChessWrapperSentry sentry;
//		__wrapper_InitializeCriticalSection(lpCriticalSection);
//		return;
//	}
//#endif
//	Real_InitializeCriticalSection(lpCriticalSection);
//}
//
//BOOL (WINAPI * Real_InitializeCriticalSectionAndSpinCount)(LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount) = InitializeCriticalSectionAndSpinCount;
//
//BOOL WINAPI __Chess_InitializeCriticalSectionAndSpinCount(LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount) {
//#ifdef WRAP_InitializeCriticalSectionAndSpinCount
//	if(ChessWrapperSentry::Wrap("InitializeCriticalSectionAndSpinCount")) {
//		ChessWrapperSentry sentry;
//		return __wrapper_InitializeCriticalSectionAndSpinCount(lpCriticalSection, dwSpinCount);
//	}
//#endif
//	return Real_InitializeCriticalSectionAndSpinCount(lpCriticalSection, dwSpinCount);
//}
//
//BOOL (WINAPI * Real_InitializeCriticalSectionEx)(LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount, DWORD Flags) = InitializeCriticalSectionEx;
//
//BOOL WINAPI __Chess_InitializeCriticalSectionEx(LPCRITICAL_SECTION lpCriticalSection, DWORD dwSpinCount, DWORD Flags) {
//#ifdef WRAP_InitializeCriticalSectionEx
//	if(ChessWrapperSentry::Wrap("InitializeCriticalSectionEx")) {
//		ChessWrapperSentry sentry;
//		return __wrapper_InitializeCriticalSectionEx(lpCriticalSection, dwSpinCount, Flags);
//	}
//#endif
//	return Real_InitializeCriticalSectionEx(lpCriticalSection, dwSpinCount, Flags);
//}
//
//VOID (WINAPI *Real_InitializeSRWLock)(PSRWLOCK SRWLock) = InitializeSRWLock;
//
//VOID WINAPI __Chess_InitializeSRWLock(PSRWLOCK SRWLock) {
//#ifdef WRAP_InitializeSRWLock
//	if(ChessWrapperSentry::Wrap("InitializeSRWLock")) {
//		ChessWrapperSentry sentry;
//		__wrapper_InitializeSRWLock(SRWLock);
//		return;
//	}
//#endif
//	Real_InitializeSRWLock(SRWLock);
//}

LONG (WINAPI * Real_InterlockedCompareExchange)( LONG volatile *Destination, LONG Exchange, LONG Comperand )
   = InterlockedCompareExchange;

__declspec(dllexport) LONG WINAPI __Chess_InterlockedCompareExchange( LONG volatile *Destination, LONG Exchange, LONG Comperand ){
#ifdef WRAP_InterlockedCompareExchange
  if(ChessWrapperSentry::Wrap("InterlockedCompareExchange")){
     ChessWrapperSentry sentry;
     LONG res = __wrapper_InterlockedCompareExchange(Destination, Exchange, Comperand);
     return res;
  }
#endif
  return Real_InterlockedCompareExchange(Destination, Exchange, Comperand);
}
LONG (WINAPI * Real_InterlockedDecrement)( LONG volatile *lpAddend )
   = InterlockedDecrement;

__declspec(dllexport) LONG WINAPI __Chess_InterlockedDecrement( LONG volatile *lpAddend ){
#ifdef WRAP_InterlockedDecrement
  if(ChessWrapperSentry::Wrap("InterlockedDecrement")){
     ChessWrapperSentry sentry;
     LONG res = __wrapper_InterlockedDecrement(lpAddend);
     return res;
  }
#endif
  return Real_InterlockedDecrement(lpAddend);
}
LONG (WINAPI * Real_InterlockedExchange)( LONG volatile *Target, LONG Value )
   = InterlockedExchange;

__declspec(dllexport) LONG WINAPI __Chess_InterlockedExchange( LONG volatile *Target, LONG Value ){
#ifdef WRAP_InterlockedExchange
  if(ChessWrapperSentry::Wrap("InterlockedExchange")){
     ChessWrapperSentry sentry;
     LONG res = __wrapper_InterlockedExchange(Target, Value);
     return res;
  }
#endif
  return Real_InterlockedExchange(Target, Value);
}
LONG (WINAPI * Real_InterlockedExchangeAdd)( LONG volatile *Addend, LONG Value )
  = InterlockedExchangeAdd;

__declspec(dllexport) LONG WINAPI __Chess_InterlockedExchangeAdd( LONG volatile *Addend, LONG Value ){
  if(ChessWrapperSentry::Wrap("InterlockedExchangeAdd")){
     ChessWrapperSentry sentry;
   }
  return Real_InterlockedExchangeAdd(Addend, Value);
}
PSINGLE_LIST_ENTRY (WINAPI * Real_InterlockedFlushSList)( PSLIST_HEADER ListHead )
  = InterlockedFlushSList;

__declspec(dllexport) PSINGLE_LIST_ENTRY WINAPI __Chess_InterlockedFlushSList( PSLIST_HEADER ListHead ){
  if(ChessWrapperSentry::Wrap("InterlockedFlushSList")){
     ChessWrapperSentry sentry;
   }
  return Real_InterlockedFlushSList(ListHead);
}
LONG (WINAPI * Real_InterlockedIncrement)( LONG volatile *lpAddend )
   = InterlockedIncrement;

__declspec(dllexport) LONG WINAPI __Chess_InterlockedIncrement( LONG volatile *lpAddend ){
#ifdef WRAP_InterlockedIncrement
  if(ChessWrapperSentry::Wrap("InterlockedIncrement")){
     ChessWrapperSentry sentry;
     LONG res = __wrapper_InterlockedIncrement(lpAddend);
     return res;
  }
#endif
  return Real_InterlockedIncrement(lpAddend);
}
void (WINAPI * Real_LeaveCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
   = LeaveCriticalSection;

__declspec(dllexport) void WINAPI __Chess_LeaveCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
#ifdef WRAP_LeaveCriticalSection
  if(ChessWrapperSentry::Wrap("LeaveCriticalSection")){
     ChessWrapperSentry sentry;
     __wrapper_LeaveCriticalSection(lpCriticalSection);
     return;
  }
#endif
  return Real_LeaveCriticalSection(lpCriticalSection);
}
BOOL (WINAPI * Real_PostQueuedCompletionStatus)( HANDLE CompletionPort, DWORD dwNumberOfBytesTransferred, ULONG_PTR dwCompletionKey, LPOVERLAPPED lpOverlapped )
   = PostQueuedCompletionStatus;

__declspec(dllexport) BOOL WINAPI __Chess_PostQueuedCompletionStatus( HANDLE CompletionPort, DWORD dwNumberOfBytesTransferred, ULONG_PTR dwCompletionKey, LPOVERLAPPED lpOverlapped ){
#ifdef WRAP_PostQueuedCompletionStatus
  if(ChessWrapperSentry::Wrap("PostQueuedCompletionStatus")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_PostQueuedCompletionStatus(CompletionPort, dwNumberOfBytesTransferred, dwCompletionKey, lpOverlapped);
     return res;
  }
#endif
  return Real_PostQueuedCompletionStatus(CompletionPort, dwNumberOfBytesTransferred, dwCompletionKey, lpOverlapped);
}
BOOL (WINAPI * Real_PulseEvent)( HANDLE hEvent )
   = PulseEvent;

__declspec(dllexport) BOOL WINAPI __Chess_PulseEvent( HANDLE hEvent ){
#ifdef WRAP_PulseEvent
  if(ChessWrapperSentry::Wrap("PulseEvent")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_PulseEvent(hEvent);
     return res;
  }
#endif
  return Real_PulseEvent(hEvent);
}
DWORD (WINAPI * Real_QueueUserAPC)( PAPCFUNC pfnAPC, HANDLE hThread, ULONG_PTR dwData )
   = QueueUserAPC;

__declspec(dllexport) DWORD WINAPI __Chess_QueueUserAPC( PAPCFUNC pfnAPC, HANDLE hThread, ULONG_PTR dwData ){
#ifdef WRAP_QueueUserAPC
  if(ChessWrapperSentry::Wrap("QueueUserAPC")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_QueueUserAPC(pfnAPC, hThread, dwData);
     return res;
  }
#endif
  return Real_QueueUserAPC(pfnAPC, hThread, dwData);
}
BOOL (WINAPI * Real_QueueUserWorkItem)( LPTHREAD_START_ROUTINE Function, PVOID Context, ULONG Flags )
   = QueueUserWorkItem;

__declspec(dllexport) BOOL WINAPI __Chess_QueueUserWorkItem( LPTHREAD_START_ROUTINE Function, PVOID Context, ULONG Flags ){
#ifdef WRAP_QueueUserWorkItem
  if(ChessWrapperSentry::Wrap("QueueUserWorkItem")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_QueueUserWorkItem(Function, Context, Flags);
     return res;
  }
#endif
  return Real_QueueUserWorkItem(Function, Context, Flags);
}
BOOL (WINAPI * Real_ReadFile)( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, LPOVERLAPPED lpOverlapped )
   = ReadFile;

__declspec(dllexport) BOOL WINAPI __Chess_ReadFile( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, LPOVERLAPPED lpOverlapped ){
#ifdef WRAP_ReadFile
  if(ChessWrapperSentry::Wrap("ReadFile")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);
     return res;
  }
#endif
  return Real_ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);
}
BOOL (WINAPI * Real_ReadFileEx)( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
   = ReadFileEx;

__declspec(dllexport) BOOL WINAPI __Chess_ReadFileEx( HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine ){
#ifdef WRAP_ReadFileEx
  if(ChessWrapperSentry::Wrap("ReadFileEx")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_ReadFileEx(hFile, lpBuffer, nNumberOfBytesToRead, lpOverlapped, lpCompletionRoutine);
     return res;
  }
#endif
  return Real_ReadFileEx(hFile, lpBuffer, nNumberOfBytesToRead, lpOverlapped, lpCompletionRoutine);
}
BOOL (WINAPI * Real_ReleaseMutex)( HANDLE hMutex )
   = ReleaseMutex;

__declspec(dllexport) BOOL WINAPI __Chess_ReleaseMutex( HANDLE hMutex ){
#ifdef WRAP_ReleaseMutex
  if(ChessWrapperSentry::Wrap("ReleaseMutex")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_ReleaseMutex(hMutex);
     return res;
  }
#endif
  return Real_ReleaseMutex(hMutex);
}
BOOL (WINAPI * Real_ReleaseSemaphore)( HANDLE hSemaphore, LONG lReleaseCount, LPLONG lpPreviousCount )
   = ReleaseSemaphore;

__declspec(dllexport) BOOL WINAPI __Chess_ReleaseSemaphore( HANDLE hSemaphore, LONG lReleaseCount, LPLONG lpPreviousCount ){
#ifdef WRAP_ReleaseSemaphore
  if(ChessWrapperSentry::Wrap("ReleaseSemaphore")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_ReleaseSemaphore(hSemaphore, lReleaseCount, lpPreviousCount);
     return res;
  }
#endif
  return Real_ReleaseSemaphore(hSemaphore, lReleaseCount, lpPreviousCount);
}

//VOID (WINAPI * Real_ReleaseSRWLockExclusive)( PSRWLOCK SRWLock )
//   = ReleaseSRWLockExclusive;
//
//__declspec(dllexport) VOID WINAPI __Chess_ReleaseSRWLockExclusive( PSRWLOCK SRWLock ){
//#ifdef WRAP_ReleaseSRWLockExclusive
//  if(ChessWrapperSentry::Wrap("ReleaseSRWLockExclusive")){
//     ChessWrapperSentry sentry;
//     __wrapper_ReleaseSRWLockExclusive(SRWLock);
//     return;
//  }
//#endif
//  Real_ReleaseSRWLockExclusive(SRWLock);
//}
//
//VOID (WINAPI * Real_ReleaseSRWLockShared)( PSRWLOCK SRWLock )
//   = ReleaseSRWLockShared;
//
//__declspec(dllexport) VOID WINAPI __Chess_ReleaseSRWLockShared( PSRWLOCK SRWLock ){
//#ifdef WRAP_ReleaseSRWLockShared
//  if(ChessWrapperSentry::Wrap("ReleaseSRWLockShared")){
//     ChessWrapperSentry sentry;
//     __wrapper_ReleaseSRWLockShared(SRWLock);
//     return;
//  }
//#endif
//  Real_ReleaseSRWLockShared(SRWLock);
//}

HANDLE (WINAPI * Real_CreateEventA)(
  __in_opt  LPSECURITY_ATTRIBUTES lpEventAttributes,
  __in      BOOL bManualReset,
  __in      BOOL bInitialState,
  __in_opt  LPCSTR lpName
) = CreateEventA;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateEventA(
  __in_opt  LPSECURITY_ATTRIBUTES lpEventAttributes,
  __in      BOOL bManualReset,
  __in      BOOL bInitialState,
  __in_opt  LPCSTR lpName
  ){
#ifdef WRAP_CreateEvent
  if(ChessWrapperSentry::Wrap("CreateEvent")){
     ChessWrapperSentry sentry;
     return __wrapper_CreateEventA(lpEventAttributes, bManualReset, bInitialState, lpName);
  }
#endif
	  return Real_CreateEventA(lpEventAttributes, bManualReset, bInitialState, lpName);
}

HANDLE (WINAPI * Real_CreateEventW)(
  __in_opt  LPSECURITY_ATTRIBUTES lpEventAttributes,
  __in      BOOL bManualReset,
  __in      BOOL bInitialState,
  __in_opt  LPCWSTR lpName
) = CreateEventW;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateEventW(
  __in_opt  LPSECURITY_ATTRIBUTES lpEventAttributes,
  __in      BOOL bManualReset,
  __in      BOOL bInitialState,
  __in_opt  LPCWSTR lpName
  ){
#ifdef WRAP_CreateEvent
  if(ChessWrapperSentry::Wrap("CreateEvent")){
     ChessWrapperSentry sentry;
     return __wrapper_CreateEventW(lpEventAttributes, bManualReset, bInitialState, lpName);
  }
#endif
	  return Real_CreateEventW(lpEventAttributes, bManualReset, bInitialState, lpName);
}

HANDLE (WINAPI * Real_CreateMutexA)(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bInitialOwner,
  LPCSTR lpName
) = CreateMutexA;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateMutexA(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bInitialOwner,
  LPCSTR lpName
  ){
#ifdef WRAP_CreateMutex
  if(ChessWrapperSentry::Wrap("CreateMutex")){
     ChessWrapperSentry sentry;
     return __wrapper_CreateMutexA(lpEventAttributes, bInitialOwner, lpName);
  }
#endif
  return Real_CreateMutexA(lpEventAttributes, bInitialOwner, lpName);
}

HANDLE (WINAPI * Real_CreateMutexW)(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bInitialOwner,
  LPCWSTR lpName
) = CreateMutexW;

__declspec(dllexport) HANDLE WINAPI __Chess_CreateMutexW(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bInitialOwner,
  LPCWSTR lpName
  ){
#ifdef WRAP_CreateMutex
  if(ChessWrapperSentry::Wrap("CreateMutex")){
     ChessWrapperSentry sentry;
     return __wrapper_CreateMutexW(lpEventAttributes, bInitialOwner, lpName);
  }
#endif
	  return Real_CreateMutexW(lpEventAttributes, bInitialOwner, lpName);
}

HANDLE (WINAPI * Real_OpenMutexA)(
	DWORD dwDesiredAccess,
	BOOL bInheritHandle,
	LPCSTR lpName
) = OpenMutexA;

__declspec(dllexport) HANDLE WINAPI __Chess_OpenMutexA(
	DWORD dwDesiredAccess,
	BOOL bInheritHandle,
	LPCSTR lpName
  ){
#ifdef WRAP_OpenMutex
  if(ChessWrapperSentry::Wrap("OpenMutex")){
     ChessWrapperSentry sentry;
     return __wrapper_OpenMutexA(dwDesiredAccess, bInheritHandle, lpName);
  }
#endif
  return Real_OpenMutexA(dwDesiredAccess, bInheritHandle, lpName);
}

HANDLE (WINAPI * Real_OpenMutexW)(
	DWORD dwDesiredAccess,
	BOOL bInheritHandle,
	LPCWSTR lpName
) = OpenMutexW;

__declspec(dllexport) HANDLE WINAPI __Chess_OpenMutexW(
	DWORD dwDesiredAccess,
	BOOL bInheritHandle,
	LPCWSTR lpName
  ){
#ifdef WRAP_OpenMutex
  if(ChessWrapperSentry::Wrap("OpenMutex")){
     ChessWrapperSentry sentry;
     return __wrapper_OpenMutexW(dwDesiredAccess, bInheritHandle, lpName);
  }
#endif
	  return Real_OpenMutexW(dwDesiredAccess, bInheritHandle, lpName);
}

BOOL (WINAPI * Real_ResetEvent)( HANDLE hEvent )
   = ResetEvent;

__declspec(dllexport) BOOL WINAPI __Chess_ResetEvent( HANDLE hEvent ){
#ifdef WRAP_ResetEvent
  if(ChessWrapperSentry::Wrap("ResetEvent")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_ResetEvent(hEvent);
     return res;
  }
#endif
  return Real_ResetEvent(hEvent);
}
DWORD (WINAPI * Real_ResumeThread)( HANDLE hThread )
   = ResumeThread;

__declspec(dllexport) DWORD WINAPI __Chess_ResumeThread( HANDLE hThread ){
#ifdef WRAP_ResumeThread
  if(ChessWrapperSentry::Wrap("ResumeThread")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_ResumeThread(hThread);
     return res;
  }
#endif
  return Real_ResumeThread(hThread);
}
BOOL (WINAPI * Real_SetEvent)( HANDLE hEvent )
   = SetEvent;

__declspec(dllexport) BOOL WINAPI __Chess_SetEvent( HANDLE hEvent ){
#ifdef WRAP_SetEvent
  if(ChessWrapperSentry::Wrap("SetEvent")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_SetEvent(hEvent);
     return res;
  }
#endif
  return Real_SetEvent(hEvent);
}
DWORD (WINAPI * Real_SignalObjectAndWait)( HANDLE hObjectToSignal, HANDLE hObjectToWaitOn, DWORD dwMilliseconds, BOOL bAlertable )
   = SignalObjectAndWait;

__declspec(dllexport) DWORD WINAPI __Chess_SignalObjectAndWait( HANDLE hObjectToSignal, HANDLE hObjectToWaitOn, DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_SignalObjectAndWait
  if(ChessWrapperSentry::Wrap("SignalObjectAndWait")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_SignalObjectAndWait(hObjectToSignal, hObjectToWaitOn, dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_SignalObjectAndWait(hObjectToSignal, hObjectToWaitOn, dwMilliseconds, bAlertable);
}
void (WINAPI * Real_Sleep)( DWORD dwMilliseconds )
   = Sleep;

__declspec(dllexport) void WINAPI __Chess_Sleep( DWORD dwMilliseconds ){
#ifdef WRAP_Sleep
  if(ChessWrapperSentry::Wrap("Sleep")){
     ChessWrapperSentry sentry;
     __wrapper_Sleep(dwMilliseconds);
     return;
  }
#endif
  return Real_Sleep(dwMilliseconds);
}

//BOOL (WINAPI * Real_SleepConditionVariableCS)( PCONDITION_VARIABLE ConditionVariable, PCRITICAL_SECTION CriticalSection, DWORD dwMilliseconds )
//	= SleepConditionVariableCS;
//
//__declspec(dllexport) BOOL WINAPI __Chess_SleepConditionVariableCS( 
//	PCONDITION_VARIABLE ConditionVariable, 
//	PCRITICAL_SECTION CriticalSection, 
//	DWORD dwMilliseconds
//	)
//{
//#ifdef WRAP_SleepConditionVariableCS
//	if(ChessWrapperSentry::Wrap("SleepConditionVariableCS")){
//		ChessWrapperSentry sentry;
//		return __wrapper_SleepConditionVariableCS( ConditionVariable, CriticalSection, dwMilliseconds );
//	}
//#endif
//	return Real_SleepConditionVariableCS( ConditionVariable, CriticalSection, dwMilliseconds );
//}
//
//BOOL (WINAPI * Real_SleepConditionVariableSRW)( PCONDITION_VARIABLE ConditionVariable, PSRWLOCK SRWLock, DWORD dwMilliseconds, ULONG Flags )
//	= SleepConditionVariableSRW;
//
//__declspec(dllexport) BOOL WINAPI __Chess_SleepConditionVariableSRW( 
//	PCONDITION_VARIABLE ConditionVariable, 
//	PSRWLOCK SRWLock, 
//	DWORD dwMilliseconds, 
//	ULONG Flags 
//	)
//{
//#ifdef WRAP_SleepConditionVariableSRW
//	if(ChessWrapperSentry::Wrap("SleepConditionVariableSRW")){
//		ChessWrapperSentry sentry;
//		return __wrapper_SleepConditionVariableSRW( ConditionVariable, SRWLock, dwMilliseconds, Flags );
//	}
//#endif
//	return Real_SleepConditionVariableSRW( ConditionVariable, SRWLock, dwMilliseconds, Flags );
//}

DWORD (WINAPI * Real_SleepEx)( DWORD dwMilliseconds, BOOL bAlertable )
	= SleepEx;

__declspec(dllexport) DWORD WINAPI __Chess_SleepEx( DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_SleepEx
  if(ChessWrapperSentry::Wrap("SleepEx")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_SleepEx(dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_SleepEx(dwMilliseconds, bAlertable);
}
BOOL (WINAPI * Real_SwitchToThread)( void )
   = SwitchToThread;

__declspec(dllexport) BOOL WINAPI __Chess_SwitchToThread( void ){
#ifdef WRAP_SwitchToThread
  if(ChessWrapperSentry::Wrap("SwitchToThread")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_SwitchToThread();
     return res;
  }
#endif
  return Real_SwitchToThread();
}
DWORD (WINAPI * Real_SuspendThread)( HANDLE hThread )
   = SuspendThread;

__declspec(dllexport) DWORD WINAPI __Chess_SuspendThread( HANDLE hThread ){
#ifdef WRAP_SuspendThread
  if(ChessWrapperSentry::Wrap("SuspendThread")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_SuspendThread(hThread);
     return res;
  }
#endif
  return Real_SuspendThread(hThread);
}
BOOL (WINAPI * Real_TryEnterCriticalSection)( LPCRITICAL_SECTION lpCriticalSection )
   = TryEnterCriticalSection;

__declspec(dllexport) BOOL WINAPI __Chess_TryEnterCriticalSection( LPCRITICAL_SECTION lpCriticalSection ){
#ifdef WRAP_TryEnterCriticalSection
  if(ChessWrapperSentry::Wrap("TryEnterCriticalSection")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_TryEnterCriticalSection(lpCriticalSection);
     return res;
  }
#endif
  return Real_TryEnterCriticalSection(lpCriticalSection);
}
DWORD (WINAPI * Real_WaitForMultipleObjects)( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds )
   = WaitForMultipleObjects;

__declspec(dllexport) DWORD WINAPI __Chess_WaitForMultipleObjects( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds ){
#ifdef WRAP_WaitForMultipleObjects
  if(ChessWrapperSentry::Wrap("WaitForMultipleObjects")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_WaitForMultipleObjects(nCount, lpHandles, bWaitAll, dwMilliseconds);
     return res;
  }
#endif
  return Real_WaitForMultipleObjects(nCount, lpHandles, bWaitAll, dwMilliseconds);
}
DWORD (WINAPI * Real_WaitForMultipleObjectsEx)( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds, BOOL bAlertable )
   = WaitForMultipleObjectsEx;

__declspec(dllexport) DWORD WINAPI __Chess_WaitForMultipleObjectsEx( DWORD nCount, const HANDLE *lpHandles, BOOL bWaitAll, DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_WaitForMultipleObjectsEx
  if(ChessWrapperSentry::Wrap("WaitForMultipleObjectsEx")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_WaitForMultipleObjectsEx(nCount, lpHandles, bWaitAll, dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_WaitForMultipleObjectsEx(nCount, lpHandles, bWaitAll, dwMilliseconds, bAlertable);
}
DWORD (WINAPI * Real_WaitForSingleObject)( HANDLE hHandle, DWORD dwMilliseconds )
   = WaitForSingleObject;

__declspec(dllexport) DWORD WINAPI __Chess_WaitForSingleObject( HANDLE hHandle, DWORD dwMilliseconds ){
#ifdef WRAP_WaitForSingleObject
  if(ChessWrapperSentry::Wrap("WaitForSingleObject")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_WaitForSingleObject(hHandle, dwMilliseconds);
     return res;
  }
#endif
  return Real_WaitForSingleObject(hHandle, dwMilliseconds);
}
DWORD (WINAPI * Real_WaitForSingleObjectEx)( HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable )
   = WaitForSingleObjectEx;

__declspec(dllexport) DWORD WINAPI __Chess_WaitForSingleObjectEx( HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable ){
#ifdef WRAP_WaitForSingleObjectEx
  if(ChessWrapperSentry::Wrap("WaitForSingleObjectEx")){
     ChessWrapperSentry sentry;
     DWORD res = __wrapper_WaitForSingleObjectEx(hHandle, dwMilliseconds, bAlertable);
     return res;
  }
#endif
  return Real_WaitForSingleObjectEx(hHandle, dwMilliseconds, bAlertable);
}

//VOID (WINAPI * Real_WakeAllConditionVariable)( __inout  PCONDITION_VARIABLE ConditionVariable )
//	= WakeAllConditionVariable;
//
//__declspec(dllexport) VOID WINAPI __Chess_WakeAllConditionVariable( __inout  PCONDITION_VARIABLE ConditionVariable )
//{
//#ifdef WRAP_WakeAllConditionVariable
//  if(ChessWrapperSentry::Wrap("WakeAllConditionVariable")){
//     ChessWrapperSentry sentry;
//     __wrapper_WakeAllConditionVariable( ConditionVariable );
//     return;
//  }
//#endif
//  Real_WakeAllConditionVariable( ConditionVariable );
//}
//
//VOID (WINAPI * Real_WakeConditionVariable)( __inout  PCONDITION_VARIABLE ConditionVariable )
//	= WakeConditionVariable;
//
//__declspec(dllexport) VOID WINAPI __Chess_WakeConditionVariable( __inout  PCONDITION_VARIABLE ConditionVariable )
//{
//#ifdef WRAP_WakeConditionVariable
//  if(ChessWrapperSentry::Wrap("WakeConditionVariable")){
//     ChessWrapperSentry sentry;
//     __wrapper_WakeConditionVariable( ConditionVariable );
//     return;
//  }
//#endif
//  Real_WakeConditionVariable( ConditionVariable );
//}

BOOL (WINAPI * Real_WriteFile)( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, LPOVERLAPPED lpOverlapped )
   = WriteFile;

__declspec(dllexport) BOOL WINAPI __Chess_WriteFile( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPDWORD lpNumberOfBytesWritten, LPOVERLAPPED lpOverlapped ){
#ifdef WRAP_WriteFile
  if(ChessWrapperSentry::Wrap("WriteFile")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);
     return res;
  }
#endif
  return Real_WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);
}
BOOL (WINAPI * Real_WriteFileEx)( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
   = WriteFileEx;

__declspec(dllexport) BOOL WINAPI __Chess_WriteFileEx( HANDLE hFile, LPCVOID lpBuffer, DWORD nNumberOfBytesToWrite, LPOVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine ){
#ifdef WRAP_WriteFileEx
  if(ChessWrapperSentry::Wrap("WriteFileEx")){
     ChessWrapperSentry sentry;
     BOOL res = __wrapper_WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, lpCompletionRoutine);
     return res;
  }
#endif
  return Real_WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, lpCompletionRoutine);
}

// UMDF-Checker specific
#if 0
BOOL (WINAPI *Real_DeviceIoControl)(HANDLE hDevice, DWORD dwIoControlCode, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped)
	= DeviceIoControl;

__declspec(dllexport) BOOL WINAPI __Chess_DeviceIoControl(HANDLE hDevice, DWORD dwIoControlCode, LPVOID lpInBuffer, DWORD nInBufferSize, LPVOID lpOutBuffer, DWORD nOutBufferSize, LPDWORD lpBytesReturned, LPOVERLAPPED lpOverlapped) {
#ifdef WRAP_DeviceIoControl	
	if(ChessWrapperSentry::Wrap("DeviceIoControl")) {
		ChessWrapperSentry sentry;
		BOOL res = __wrapper_DeviceIoControl(hDevice, dwIoControlCode, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesReturned, lpOverlapped);
		DWORD errorCode = GetLastError();
		//fprintf(stderr, "(%d) DeviceIoControl return to Driver: %p (in %d, out %d)\n", Chess::GetCurrentTid(), hDevice, nInBufferSize, nOutBufferSize);
		fflush(stderr);
		SetLastError(errorCode);
		return res;
	}
#endif
	/*fprintf(stderr, "(%d) DeviceIoControl called: %p (in %d, out %d)\n", GetCurrentThreadId(), hDevice, nInBufferSize, nOutBufferSize);
	fflush(stderr);*/
	return Real_DeviceIoControl(hDevice, dwIoControlCode, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesReturned, lpOverlapped);
}
#endif

HANDLE (WINAPI *Real_CreateFileW)(LPCWSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile)
	= CreateFileW;

HANDLE WINAPI __Chess_CreateFileW(LPCWSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile)
{
	HANDLE res = Real_CreateFileW(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
#ifdef WRAP_CreateFileW
	//fprintf(stderr, "Handle %p: FILE_FLAGGED_OVERLAPPED? %d\n", res, dwFlagsAndAttributes & FILE_FLAG_OVERLAPPED);
#endif
	return res;
}

HMODULE (WINAPI *Real_LoadLibraryExA)(LPCSTR lpFileName, HANDLE hFlags, DWORD dwFlags) = LoadLibraryExA;

HMODULE WINAPI __Chess_LoadLibraryExA(LPCSTR lpFileName, HANDLE hFlags, DWORD dwFlags) {
#ifdef WRAP_LoadLibraryExA
	if(ChessWrapperSentry::Wrap("LoadLibraryExA")) {
		ChessWrapperSentry sentry;
		HMODULE res = Real_LoadLibraryExA(lpFileName, hFlags, dwFlags);
		((Win32SyncManager *) Chess::GetSyncManager())->RegisterTestModule(res);
		return res;
	}
#endif
	return Real_LoadLibraryExA(lpFileName, hFlags, dwFlags);
}

HMODULE (WINAPI *Real_LoadLibraryExW)(LPCWSTR lpFileName, HANDLE hFlags, DWORD dwFlags) = LoadLibraryExW;

HMODULE WINAPI __Chess_LoadLibraryExW(LPCWSTR lpFileName, HANDLE hFlags, DWORD dwFlags) {
#ifdef WRAP_LoadLibraryExW
	if(ChessWrapperSentry::Wrap("LoadLibraryExW")) {
		ChessWrapperSentry sentry;
		HMODULE res = Real_LoadLibraryExW(lpFileName, hFlags, dwFlags);
		((Win32SyncManager *) Chess::GetSyncManager())->RegisterTestModule(res);
		return res;
	}
#endif
	return Real_LoadLibraryExW(lpFileName, hFlags, dwFlags);
}

HMODULE (WINAPI *Real_LoadLibraryA)(LPCSTR lpLibFileName) = LoadLibraryA;

HMODULE WINAPI __Chess_LoadLibraryA(LPCSTR lpLibFileName) {
#ifdef WRAP_LoadLibraryA
	if(ChessWrapperSentry::Wrap("LoadLibraryA")) {
		ChessWrapperSentry sentry;
		HMODULE res = Real_LoadLibraryA(lpLibFileName);
		((Win32SyncManager *) Chess::GetSyncManager())->RegisterTestModule(res);
		return res;
	}
#endif
	return Real_LoadLibraryA(lpLibFileName);
}

HMODULE (WINAPI *Real_LoadLibraryW)(LPCWSTR lpLibFileName) = LoadLibraryW;

HMODULE WINAPI __Chess_LoadLibraryW(LPCWSTR lpLibFileName) {
#ifdef WRAP_LoadLibraryW
	if(ChessWrapperSentry::Wrap("LoadLibraryW")) {
		ChessWrapperSentry sentry;
		HMODULE res = Real_LoadLibraryW(lpLibFileName);
		((Win32SyncManager *) Chess::GetSyncManager())->RegisterTestModule(res);
		return res;
	}
#endif
	return Real_LoadLibraryW(lpLibFileName);
}

#define WRAP_malloc
#define WRAP_realloc
#define WRAP_free
#define WRAP_HeapAlloc
#define WRAP_HeapReAlloc
#define WRAP_HeapFree

void *(*Real_malloc)(size_t _Size) = malloc;

void *__Chess_malloc(size_t _Size) {
#ifdef WRAP_malloc
	if(ChessWrapperSentry::Wrap("malloc")) {
		ChessWrapperSentry sentry;
		return __wrapper_malloc(_Size);
	}
#endif
	return Real_malloc(_Size);
}

void *(*Real_realloc)(void *_Memory, size_t _Size) = realloc;

void *__Chess_realloc(void *_Memory, size_t _Size) {
#ifdef WRAP_realloc
	if(ChessWrapperSentry::Wrap("realloc")) {
		ChessWrapperSentry sentry;
		return __wrapper_realloc(_Memory, _Size);
	}
#endif
	return Real_realloc(_Memory, _Size);
}

void (*Real_free)(void *_Memory) = free;

void __Chess_free(void *_Memory) {
#ifdef WRAP_free
	if(ChessWrapperSentry::Wrap("free")) {
		ChessWrapperSentry sentry;
		return __wrapper_free(_Memory);
	}
#endif
	return Real_free(_Memory);
}

void *(WINAPI *Real_HeapAlloc)(HANDLE hHeap, DWORD dwFlags, SIZE_T _Size) = HeapAlloc;

void * WINAPI __Chess_HeapAlloc(HANDLE hHeap, DWORD dwFlags, SIZE_T _Size) {
#ifdef WRAP_HeapAlloc
	if(ChessWrapperSentry::Wrap("HeapAlloc")) {
		ChessWrapperSentry sentry;
		return __wrapper_HeapAlloc(hHeap, dwFlags, _Size);
	}
#endif
	void *res = Real_HeapAlloc(hHeap, dwFlags, _Size);
	return res;
}

void *(WINAPI *Real_HeapReAlloc)(HANDLE hHeap, DWORD dwFlags, void *_Memory, SIZE_T _Size) = HeapReAlloc;

void * WINAPI __Chess_HeapReAlloc(HANDLE hHeap, DWORD dwFlags, void *_Memory, SIZE_T _Size) {
#ifdef WRAP_HeapReAlloc
	if(ChessWrapperSentry::Wrap("HeapReAlloc")) {
		ChessWrapperSentry sentry;
		return __wrapper_HeapReAlloc(hHeap, dwFlags, _Memory, _Size);
	}
#endif
	return Real_HeapReAlloc(hHeap, dwFlags, _Memory, _Size);
}

BOOL (WINAPI *Real_HeapFree)(HANDLE hHeap, DWORD dwFlags, void *_Memory) = HeapFree;

BOOL WINAPI __Chess_HeapFree(HANDLE hHeap, DWORD dwFlags, void *_Memory) {
#ifdef WRAP_HeapFree
	if(ChessWrapperSentry::Wrap("HeapFree")) {
		ChessWrapperSentry sentry;
		return __wrapper_HeapFree(hHeap, dwFlags, _Memory);
	}
#endif
	return Real_HeapFree(hHeap, dwFlags, _Memory);
}
}

BOOL
WINAPI
__wrapper_RegisterWaitForSingleObject(
    __deref_out PHANDLE phNewWaitObject,
    __in        HANDLE hObject,
    __in        WAITORTIMERCALLBACK Callback,
    __in_opt    PVOID Context,
    __in        ULONG dwMilliseconds,
    __in        ULONG dwFlags
    );
BOOL WINAPI __wrapper_UnregisterWait(
  HANDLE WaitHandle
);
BOOL WINAPI __wrapper_SetWaitableTimer(
  HANDLE hTimer,
  const LARGE_INTEGER* pDueTime,
  LONG lPeriod,
  PTIMERAPCROUTINE pfnCompletionRoutine,
  LPVOID lpArgToCompletionRoutine,
  BOOL fResume
);

VOID WINAPI __wrapper_FreeLibraryAndExitThread(
  HMODULE hModule,
  DWORD dwExitCode
  );

///XXX: 
///XXX: 
///XXX: 
///XXX: Sometime in the future, get rid of the extra level of indirection
///XXX: between __Chess_XXX and __wrapper_XXX
///XXX: 
///XXX: 
///XXX: 



void __declspec(dllexport) GetWin32Wrappers(stdext::hash_map<void*, void*>& wrapperTable){
	// Critical Section
	//wrapperTable[&InitializeCriticalSection] = __Chess_InitializeCriticalSection;
	//wrapperTable[&InitializeCriticalSectionAndSpinCount] = __Chess_InitializeCriticalSectionAndSpinCount;
	//wrapperTable[&InitializeCriticalSectionEx] = __Chess_InitializeCriticalSectionEx;
	wrapperTable[&EnterCriticalSection] = __Chess_EnterCriticalSection;
	wrapperTable[&TryEnterCriticalSection] = __Chess_TryEnterCriticalSection;
	wrapperTable[&LeaveCriticalSection] = __Chess_LeaveCriticalSection;
//	wrapperTable[&DeleteCriticalSection] = __Chess_DeleteCriticalSection;

	// Interlocked
#ifdef _X86_
    wrapperTable[(LONG (__stdcall *)(volatile LONG *)) &InterlockedIncrement] = __Chess_InterlockedIncrement;
    wrapperTable[(LONG (__stdcall *)(volatile LONG *)) &InterlockedDecrement] = __Chess_InterlockedDecrement;
    wrapperTable[(LONG (__stdcall *)(volatile LONG *, LONG)) &InterlockedExchange] = __Chess_InterlockedExchange;
    wrapperTable[(LONG (__stdcall *)(volatile LONG *, LONG, LONG)) &InterlockedCompareExchange] = __Chess_InterlockedCompareExchange;
#endif

	// Threading
	wrapperTable[&CreateThread] = __Chess_CreateThread;
	wrapperTable[&ExitThread] = __Chess_ExitThread;
	wrapperTable[&FreeLibraryAndExitThread] = __wrapper_FreeLibraryAndExitThread;
	wrapperTable[&QueueUserWorkItem] = __Chess_QueueUserWorkItem;
	wrapperTable[&SuspendThread] = __Chess_SuspendThread;
	wrapperTable[&ResumeThread] = __Chess_ResumeThread;

	//Events
	wrapperTable[&SetEvent] = __Chess_SetEvent;
	wrapperTable[&ResetEvent] = __Chess_ResetEvent;
	wrapperTable[&PulseEvent] = __Chess_PulseEvent;

	wrapperTable[&CreateEventA] = __Chess_CreateEventA;
	wrapperTable[&CreateEventW] = __Chess_CreateEventW;
	wrapperTable[&CreateMutexA] = __Chess_CreateMutexA;
	wrapperTable[&CreateMutexW] = __Chess_CreateMutexW;
	wrapperTable[&OpenMutexA] = __Chess_OpenMutexA;
	wrapperTable[&OpenMutexW] = __Chess_OpenMutexW;

	// Mutex & Semaphore
	wrapperTable[&ReleaseMutex] = __Chess_ReleaseMutex;
	wrapperTable[&ReleaseSemaphore] = __Chess_ReleaseSemaphore;

	// Sleep and Yield
	wrapperTable[&Sleep] = __Chess_Sleep;
	wrapperTable[&SleepEx] = __Chess_SleepEx;
	wrapperTable[&SwitchToThread] = __Chess_SwitchToThread;

	// Waits
	wrapperTable[&WaitForSingleObjectEx] = __Chess_WaitForSingleObjectEx;
	wrapperTable[&WaitForSingleObject] = __Chess_WaitForSingleObject;
	wrapperTable[&WaitForMultipleObjectsEx] = __Chess_WaitForMultipleObjectsEx;
	wrapperTable[&WaitForMultipleObjects] = __Chess_WaitForMultipleObjects;
	wrapperTable[&SignalObjectAndWait] = __Chess_SignalObjectAndWait;

	// moved to Win32AsyncProc.cpp
	//	wrapperTable[&QueueUserAPC] = __Chess_QueueUserAPC;
	//	wrapperTable[&ReadFileEx] = __Chess_ReadFileEx;
	//	wrapperTable[&WriteFileEx] = __Chess_WriteFileEx;

	// IoCompletions
	wrapperTable[&ReadFile] = __Chess_ReadFile;
	wrapperTable[&WriteFile] = __Chess_WriteFile;

	wrapperTable[&CreateIoCompletionPort] = __Chess_CreateIoCompletionPort;
	wrapperTable[&PostQueuedCompletionStatus] = __Chess_PostQueuedCompletionStatus;
	wrapperTable[&GetQueuedCompletionStatus] = __Chess_GetQueuedCompletionStatus;

	// Timers
	wrapperTable[&CreateTimerQueue] = __Chess_CreateTimerQueue;
	wrapperTable[&CreateTimerQueueTimer] = __Chess_CreateTimerQueueTimer;
	wrapperTable[&DeleteTimerQueueTimer] = __Chess_DeleteTimerQueueTimer;
	wrapperTable[&DeleteTimerQueueEx] = __Chess_DeleteTimerQueueEx;
	wrapperTable[&DeleteTimerQueue] = __Chess_DeleteTimerQueue;
	wrapperTable[&SetWaitableTimer] = __wrapper_SetWaitableTimer;
	wrapperTable[&RegisterWaitForSingleObject] = __wrapper_RegisterWaitForSingleObject;
	wrapperTable[&UnregisterWait] = __wrapper_UnregisterWait;

	//// SRWLocks moved to Win32VistaWrappers
	//wrapperTable[&InitializeSRWLock] = __Chess_InitializeSRWLock;
	//wrapperTable[&AcquireSRWLockExclusive] = __Chess_AcquireSRWLockExclusive;
	//wrapperTable[&ReleaseSRWLockExclusive] = __Chess_ReleaseSRWLockExclusive;
	//wrapperTable[&AcquireSRWLockShared] = __Chess_AcquireSRWLockShared;
	//wrapperTable[&ReleaseSRWLockShared] = __Chess_ReleaseSRWLockShared;

	//// Condition variables
	//wrapperTable[&InitializeConditionVariable] = __Chess_InitializeConditionVariable;
	//wrapperTable[&SleepConditionVariableCS] = __Chess_SleepConditionVariableCS;
	//wrapperTable[&SleepConditionVariableSRW] = __Chess_SleepConditionVariableSRW;
	//wrapperTable[&WakeConditionVariable] = __Chess_WakeConditionVariable;
	//wrapperTable[&WakeAllConditionVariable] = __Chess_WakeAllConditionVariable;

	wrapperTable[&DuplicateHandle] = __Chess_DuplicateHandle;

	// UMDF-Checker specific
	//wrapperTable[&DeviceIoControl] = __Chess_DeviceIoControl;
	wrapperTable[&CreateFileW] = __Chess_CreateFileW;
	
	//wrapperTable[&LoadLibraryA] = __Chess_LoadLibraryA;
	//wrapperTable[&LoadLibraryW] = __Chess_LoadLibraryW;

	////wrapperTable[&LoadLibraryEx] = __Chess_LoadLibraryEx;
	//wrapperTable[&LoadLibraryExA] = __Chess_LoadLibraryExA;
	//wrapperTable[&LoadLibraryExW] = __Chess_LoadLibraryExW;

	//wrapperTable[&malloc] = __Chess_malloc;
	//wrapperTable[&realloc] = __Chess_realloc;
	//wrapperTable[&free] = __Chess_free;

	//wrapperTable[&HeapAlloc] = __Chess_HeapAlloc;
	//wrapperTable[&HeapReAlloc] = __Chess_HeapReAlloc;
	//wrapperTable[&HeapFree] = __Chess_HeapFree;
}