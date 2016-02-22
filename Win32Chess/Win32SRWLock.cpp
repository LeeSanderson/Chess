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

struct SRWState {
private:
	int n;
public:
	SRWState() { n = 0; }
	bool IsAvailable() const { return n == 0; } 
	bool IsShared() const { return n > 0; }
	bool IsExclusive() const { return n == -1; }
	void AcquireExclusive() { assert(n == 0); n = -1; }
	void ReleaseExclusive() { assert(n == -1); n = 0; }
	void AcquireShared() { assert(n >= 0); n++; }
	void ReleaseShared() { assert(n > 0); n--; }
};

SyncVarVector<SRWState> SRWLocks;
bool IsAvailable(SyncVar SRWLock)
{
	return SRWLocks[SRWLock].IsAvailable();
}

bool IsShared(SyncVar SRWLock)
{
	return SRWLocks[SRWLock].IsShared();
}

bool IsExclusive(SyncVar SRWLock)
{
	return SRWLocks[SRWLock].IsExclusive();
}
void AcquireExclusive(SyncVar SRWLock)
{
	SRWLocks[SRWLock].AcquireExclusive();
}
void ReleaseExclusive(SyncVar SRWLock)
{
	SRWLocks[SRWLock].ReleaseExclusive();
}
void AcquireShared(SyncVar SRWLock)
{
	SRWLocks[SRWLock].AcquireShared();
}
void ReleaseShared(SyncVar SRWLock)
{
	SRWLocks[SRWLock].ReleaseShared();
}

void Win32SRWLockReset()
{
	SRWLocks.clear();
}

VOID WINAPI __wrapper_InitializeSRWLock(
  __out  PSRWLOCK SRWLock)
{
	// generate a warning if the lock is not in the AVAILABLE state
		SyncVar var = GetWin32SyncManager()->GetSyncVarFromAddress(SRWLock);	
		SRWLocks[var] = SRWState();
	InitializeSRWLock(SRWLock);
}

VOID WINAPI __wrapper_AcquireSRWLockExclusive(__inout  PSRWLOCK SRWLock)
{
	while (true) {
		SyncVar var = GetWin32SyncManager()->GetSyncVarFromAddress(SRWLock);
		Chess::SyncVarAccess(var, SVOP::SRWLOCK_ACQUIRE_EXCLUSIVE);
		if (!IsAvailable(var)) {
			Chess::LocalBacktrack();
			continue;
		}
		AcquireSRWLockExclusive(SRWLock);
		ChessErrorSentry sentry;
		AcquireExclusive(var);
		Chess::CommitSyncVarAccess();
		break;
	}
}

VOID WINAPI __wrapper_ReleaseSRWLockExclusive(__inout  PSRWLOCK SRWLock)
{
	SyncVar var = GetWin32SyncManager()->GetSyncVarFromAddress(SRWLock);
	Chess::SyncVarAccess(var, SVOP::SRWLOCK_RELEASE_EXCLUSIVE);
	ReleaseSRWLockExclusive(SRWLock);
	ChessErrorSentry sentry;
	ReleaseExclusive(var);
	Chess::CommitSyncVarAccess();
}

VOID WINAPI __wrapper_AcquireSRWLockShared(__inout  PSRWLOCK SRWLock)
{
	SyncVar var = GetWin32SyncManager()->GetSyncVarFromAddress(SRWLock);
	while (true) {
		Chess::SyncVarAccess(var, SVOP::SRWLOCK_ACQUIRE_SHARED);
		if (IsExclusive(var)) {
			Chess::LocalBacktrack();
			continue;
		}
		AcquireSRWLockShared(SRWLock);
		ChessErrorSentry sentry;
		AcquireShared(var);
		Chess::CommitSyncVarAccess();
		break;
	}
}


VOID WINAPI __wrapper_ReleaseSRWLockShared(__inout  PSRWLOCK SRWLock)
{
	SyncVar var = GetWin32SyncManager()->GetSyncVarFromAddress(SRWLock);
	Chess::SyncVarAccess(var, SVOP::SRWLOCK_RELEASE_SHARED);
	ReleaseSRWLockShared(SRWLock);
	ChessErrorSentry sentry;
	ReleaseShared(var);
	Chess::CommitSyncVarAccess();
}

	//// SRWLocks
	//wrapperTable[&InitializeSRWLock] = Mine_InitializeSRWLock;
	//wrapperTable[&AcquireSRWLockExclusive] = Mine_AcquireSRWLockExclusive;
	//wrapperTable[&ReleaseSRWLockExclusive] = Mine_ReleaseSRWLockExclusive;
	//wrapperTable[&AcquireSRWLockShared] = Mine_AcquireSRWLockShared;
	//wrapperTable[&ReleaseSRWLockShared] = Mine_ReleaseSRWLockShared;


WrapperFunctionInfo SRWLockWrapperFunctions[] = {
	WrapperFunctionInfo(InitializeSRWLock, __wrapper_InitializeSRWLock, "kernel32::InitializeSRWLock"),
	WrapperFunctionInfo(AcquireSRWLockExclusive, __wrapper_AcquireSRWLockExclusive, "kernel32::AcquireSRWLockExclusive"),
	WrapperFunctionInfo(ReleaseSRWLockExclusive, __wrapper_ReleaseSRWLockExclusive, "kernel32::ReleaseSRWLockExclusive"),
	WrapperFunctionInfo(AcquireSRWLockShared, __wrapper_AcquireSRWLockShared, "kernel32::AcquireSRWLockShared"),
	WrapperFunctionInfo(ReleaseSRWLockShared, __wrapper_ReleaseSRWLockShared, "kernel32::ReleaseSRWLockShared"),
	WrapperFunctionInfo()
};

class WrappersSRWLock : public IChessWrapper {
public:
	virtual WrapperFunctionInfo* GetWrapperFunctions(){
		return SRWLockWrapperFunctions;
	}
	
	virtual void Reset(){
		Win32SRWLockReset();
	}
};

IChessWrapper* GetSRWLockWrappers(){
	return new WrappersSRWLock();
}

#else

IChessWrapper* GetSRWLockWrappers(){
	return null;
}

#endif