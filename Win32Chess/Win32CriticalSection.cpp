/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "ChessAssert.h"
#include "Chess.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"
#include "IChessWrapper.h"
#include "ChessProfilerTimer.h"

#ifdef MAINTAIN_MY_OWN_REENTRY_COUNT
stdext::hash_map<LPCRITICAL_SECTION, int> ReentryCount;
void Win32CriticalSectionReset()
{
	ReentryCount.clear();
}
void IncrementReentryCount(LPCRITICAL_SECTION lpCriticalSection)
{
	if (ReentryCount.find(lpCriticalSection) == ReentryCount.end())
		ReentryCount[lpCriticalSection] = 0;
	ReentryCount[lpCriticalSection]++;
}
void DecrementReentryCount(LPCRITICAL_SECTION lpCriticalSection)
{
	if (ReentryCount.find(lpCriticalSection) == ReentryCount.end())
		ReentryCount[lpCriticalSection] = 0;
	ReentryCount[lpCriticalSection]--;
}
int GetReentryCount(LPCRITICAL_SECTION lpCriticalSection)
{
	if (ReentryCount.find(lpCriticalSection) == ReentryCount.end())
		ReentryCount[lpCriticalSection] = 0;
	assert(lpCriticalSection->RecursionCount == ReentryCount[lpCriticalSection]);
	return ReentryCount[lpCriticalSection];
}


void ResetReentryCount(LPCRITICAL_SECTION lpCriticalSection){
	if (ReentryCount.find(lpCriticalSection) != ReentryCount.end())
		ReentryCount.erase(lpCriticalSection);
}

void WINAPI __wrapper_InitializeCriticalSection(
  __out  LPCRITICAL_SECTION lpCriticalSection)
{
	// report a CHESS warning if reentry count is not 0
	ResetReentryCount(lpCriticalSection);
	InitializeCriticalSection(lpCriticalSection);
}

BOOL WINAPI __wrapper_InitializeCriticalSectionAndSpinCount(
  __inout  LPCRITICAL_SECTION lpCriticalSection,
  __in     DWORD dwSpinCount)
{
	// report a CHESS warning if reentry count is not 0
	ResetReentryCount(lpCriticalSection);
	return InitializeCriticalSectionAndSpinCount(lpCriticalSection, dwSpinCount);
}

BOOL WINAPI __wrapper_InitializeCriticalSectionEx(
  __out  LPCRITICAL_SECTION lpCriticalSection,
  __in   DWORD dwSpinCount,
  __in   DWORD Flags)
{
	// report a CHESS warning if reentry count is not 0
	ResetReentryCount(lpCriticalSection);
	return InitializeCriticalSectionEx(lpCriticalSection, dwSpinCount, Flags);
}
void WINAPI __wrapper_DeleteCriticalSection(__inout  LPCRITICAL_SECTION lpCriticalSection)
{
	// report a CHESS warning if reentry count is not 0
	ResetReentryCount(lpCriticalSection);
	DeleteCriticalSection(lpCriticalSection);
}

#else
void Win32CriticalSectionReset(){}

inline void IncrementReentryCount(LPCRITICAL_SECTION lpCriticalSection){}

inline void DecrementReentryCount(LPCRITICAL_SECTION lpCriticalSection){}

#ifndef UNDER_CE
#pragma warning( push )
#pragma warning( disable: 25004) // even though lpCriticalSection is const, Prefix insists that it "could be const"
int GetReentryCount(const LPCRITICAL_SECTION lpCriticalSection)
{
	return lpCriticalSection->RecursionCount;
}
#pragma warning( pop )
#endif


//inline void ResetReentryCount(LPCRITICAL_SECTION lpCriticalSection){}
#endif



void WIN32CHESS_API WINAPI __wrapper_EnterCriticalSection(LPCRITICAL_SECTION lpCriticalSection)
{
	while (true) {
		Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress(lpCriticalSection), SVOP::LOCK_ACQUIRE);
		if(!TryEnterCriticalSection(lpCriticalSection)){
			Chess::LocalBacktrack();
			continue;
		}
		ChessErrorSentry sentry;
		IncrementReentryCount(lpCriticalSection);
		Chess::CommitSyncVarAccess();
		break;
	}
}

BOOL WINAPI __wrapper_TryEnterCriticalSection( LPCRITICAL_SECTION lpCriticalSection )
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress(lpCriticalSection), SVOP::LOCK_TRYACQUIRE);
	BOOL ret = TryEnterCriticalSection(lpCriticalSection);
	ChessErrorSentry sentry;
	if (ret)
		IncrementReentryCount(lpCriticalSection);
	Chess::CommitSyncVarAccess();
	return ret;	
}

void WIN32CHESS_API WINAPI __wrapper_LeaveCriticalSection(LPCRITICAL_SECTION lpCriticalSection)
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress(lpCriticalSection), SVOP::LOCK_RELEASE);
	LeaveCriticalSection(lpCriticalSection);
	ChessErrorSentry sentry;
	DecrementReentryCount(lpCriticalSection);
	Chess::CommitSyncVarAccess();
}
