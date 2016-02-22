/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"

#if (_WIN32_WINNT >= 0x0600) 
#include "ChessAssert.h"
#include "Chess.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"
#include "IChessWrapper.h"
#include "SyncVarVector.h"

#pragma warning (push) 
#pragma warning (disable: 25004)
int GetReentryCount(const LPCRITICAL_SECTION lpCriticalSection)
{
	return lpCriticalSection->RecursionCount;
}
#pragma warning (pop)

inline void IncrementReentryCount(LPCRITICAL_SECTION lpCriticalSection){}
inline void DecrementReentryCount(LPCRITICAL_SECTION lpCriticalSection){}

SyncVarVector<std::deque<Task>> ConditionVariableQueues;
void Win32ConditionVariableReset()
{
	ConditionVariableQueues.clear();
}
void Add(SyncVar ConditionVariable)
{
	ConditionVariableQueues[ConditionVariable].push_back(Chess::GetCurrentTid());
}
void Remove(SyncVar ConditionVariable)
{
	std::deque<Task> lq = ConditionVariableQueues[ConditionVariable];
	if (!lq.empty())
		lq.pop_front();
}
void RemoveAll(SyncVar ConditionVariable)
{
	ConditionVariableQueues[ConditionVariable].clear();
}

bool IsMember(SyncVar ConditionVariable)
{
	Task currentTid = Chess::GetCurrentTid();
	std::deque<Task>& q = ConditionVariableQueues[ConditionVariable];
	for (std::deque<Task>::const_iterator i = q.begin(); i != q.end(); ++i)
	{ 
		if (*i == currentTid)
			return true;
	}
	return false;
}

VOID WINAPI __wrapper_InitializeConditionVariable(
  __out  PCONDITION_VARIABLE ConditionVariable)
{
	if(!ChessWrapperSentry::Wrap("InitializeConditionVariable")){
		InitializeConditionVariable(ConditionVariable);
	}
	ChessWrapperSentry sentry;

	// report a warning if the queue corresponding to the condition variable is nonempty
	SyncVar ConditionVariableSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(ConditionVariable);
	ConditionVariableQueues[ConditionVariableSyncVar].clear();
	InitializeConditionVariable(ConditionVariable);
}

BOOL WINAPI __wrapper_SleepConditionVariableCS(
  __inout  PCONDITION_VARIABLE ConditionVariable,
  __inout  PCRITICAL_SECTION CriticalSection,
  __in     DWORD dwMilliseconds
  )
{
	if(!ChessWrapperSentry::Wrap("SleepConditionVariableCS")){
		return SleepConditionVariableCS(ConditionVariable, CriticalSection, dwMilliseconds);
	}
	ChessWrapperSentry wrappersentry;

	SyncVar ConditionVariableSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(ConditionVariable);
	SyncVar CriticalSectionSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(CriticalSection);
	SyncVar vars[2];
	int n = 2;
	vars[0] = ConditionVariableSyncVar;
	vars[1] = CriticalSectionSyncVar;
	//SyncVar agg(vars, n);
	//Chess::SyncVarAccess(agg, SVOP::RWVAR_READWRITE);
	Chess::AggregateSyncVarAccess(vars, n, SVOP::RWVAR_READWRITE);
	if (dwMilliseconds == INFINITE)
		Add(ConditionVariableSyncVar);
	int ReentryCount = GetReentryCount(CriticalSection);
	for (int i = 0; i < ReentryCount; i++)
	{
		DecrementReentryCount(CriticalSection);
		LeaveCriticalSection(CriticalSection);
	}
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();

	Chess::TaskYield();

	if (dwMilliseconds == INFINITE)
	{
		while (true) {
			Chess::SyncVarAccess(ConditionVariableSyncVar, SVOP::RWVAR_READWRITE);
			if (IsMember(ConditionVariableSyncVar)){
				Chess::LocalBacktrack();
				continue;
			}
			ChessErrorSentry sentry2;
			Chess::CommitSyncVarAccess();
			break;
		}
	}

	__wrapper_EnterCriticalSection(CriticalSection);
	for (int i = 1; i < ReentryCount; i++)
	{
		IncrementReentryCount(CriticalSection);
		EnterCriticalSection(CriticalSection);
	}
	return TRUE;
}

VOID WINAPI __wrapper_AcquireSRWLockExclusive(PSRWLOCK SRWLock);

VOID WINAPI __wrapper_AcquireSRWLockShared(PSRWLOCK SRWLock);

BOOL WINAPI __wrapper_SleepConditionVariableSRW(
  __inout  PCONDITION_VARIABLE ConditionVariable,
  __inout  PSRWLOCK SRWLock,
  __in     DWORD dwMilliseconds,
  __in     ULONG Flags
)
{
	if(!ChessWrapperSentry::Wrap("SleepConditionVariableSRW")){
		return SleepConditionVariableSRW(ConditionVariable, SRWLock, dwMilliseconds, Flags);
	}
	ChessWrapperSentry wrappersentry;

	SyncVar ConditionVariableSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(ConditionVariable);
	SyncVar SRWLockSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(SRWLock);
	SyncVar vars[2];
	int n = 2;
	vars[0] = ConditionVariableSyncVar;
	vars[1] = SRWLockSyncVar;
	//SyncVar agg(vars, n);
	//Chess::SyncVarAccess(agg, SVOP::RWVAR_READWRITE);
	Chess::AggregateSyncVarAccess(vars, n, SVOP::RWVAR_READWRITE);
	if (dwMilliseconds == INFINITE)
		Add(ConditionVariableSyncVar);
	if (Flags == CONDITION_VARIABLE_LOCKMODE_SHARED)
		ReleaseSRWLockShared(SRWLock);
	else
		ReleaseSRWLockExclusive(SRWLock);
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();

	Chess::TaskYield();

	if (dwMilliseconds == INFINITE)
	{
		while (true) {
			Chess::SyncVarAccess(ConditionVariableSyncVar, SVOP::RWVAR_READWRITE);
			if (IsMember(ConditionVariableSyncVar)){
				Chess::LocalBacktrack();
				continue;
			}
			ChessErrorSentry sentry2;
			Chess::CommitSyncVarAccess();
			break;
		}
	}

	if (Flags == CONDITION_VARIABLE_LOCKMODE_SHARED)
		__wrapper_AcquireSRWLockShared(SRWLock);
	else
		__wrapper_AcquireSRWLockExclusive(SRWLock);
	return TRUE;
}

VOID WINAPI __wrapper_WakeAllConditionVariable(
  __inout  PCONDITION_VARIABLE ConditionVariable
)
{
	if(!ChessWrapperSentry::Wrap("WakeAllConditionVariable")){
		return WakeAllConditionVariable(ConditionVariable);
	}
	ChessWrapperSentry wrappersentry;

	SyncVar ConditionVariableSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(ConditionVariable);
	Chess::SyncVarAccess(ConditionVariableSyncVar, SVOP::RWVAR_READWRITE);
	RemoveAll(ConditionVariableSyncVar);
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();
}

VOID WINAPI __wrapper_WakeConditionVariable(
  __inout  PCONDITION_VARIABLE ConditionVariable
)
{
	if(!ChessWrapperSentry::Wrap("WakeConditionVariable")){
		return WakeConditionVariable(ConditionVariable);
	}
	ChessWrapperSentry wrappersentry;

	SyncVar ConditionVariableSyncVar = GetWin32SyncManager()->GetSyncVarFromAddress(ConditionVariable);
	Chess::SyncVarAccess(ConditionVariableSyncVar, SVOP::RWVAR_READWRITE);
	Remove(ConditionVariableSyncVar);
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();
}

	//// Condition variables
	//wrapperTable[&InitializeConditionVariable] = Mine_InitializeConditionVariable;
	//wrapperTable[&SleepConditionVariableCS] = Mine_SleepConditionVariableCS;
	//wrapperTable[&SleepConditionVariableSRW] = Mine_SleepConditionVariableSRW;
	//wrapperTable[&WakeConditionVariable] = Mine_WakeConditionVariable;
	//wrapperTable[&WakeAllConditionVariable] = Mine_WakeAllConditionVariable;

WrapperFunctionInfo ConditionVariableWrapperFunctions[] = {
	WrapperFunctionInfo(InitializeConditionVariable, __wrapper_InitializeConditionVariable, "kernel32::InitializeConditionVariable"),
	WrapperFunctionInfo(SleepConditionVariableCS, __wrapper_SleepConditionVariableCS, "kernel32::SleepConditionVariableCS"),
	WrapperFunctionInfo(SleepConditionVariableSRW, __wrapper_SleepConditionVariableSRW, "kernel32::SleepConditionVariableSRW"),
	WrapperFunctionInfo(WakeConditionVariable, __wrapper_WakeConditionVariable, "kernel32::WakeConditionVariable"),
	WrapperFunctionInfo(WakeAllConditionVariable, __wrapper_WakeAllConditionVariable, "kernel32::WakeAllConditionVariable"),
	WrapperFunctionInfo()
};

class WrappersConditionVariable : public IChessWrapper {
public:
	virtual WrapperFunctionInfo* GetWrapperFunctions(){
		return ConditionVariableWrapperFunctions;
	}
	
	virtual void Reset(){
		Win32ConditionVariableReset();
	}
};

IChessWrapper* GetConditionVariableWrappers(){
	return new WrappersConditionVariable();
}

#else

IChessWrapper* GetConditionVariableWrappers(){
	return null;
}

#endif
