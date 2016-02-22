/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
// ideally, we should make a Win32Wrappers class
#include "Win32Base.h"
#include "Win32SyncManager.h"

struct ThreadRoutineArg{
	LPTHREAD_START_ROUTINE Function; 
	PVOID Context;
	Semaphore selfSemaphore;
};
DWORD WINAPI ThreadCreateWrapper(LPVOID arg);

//void WrappersReset();
void WrappersOnAlertableState(Task tid);

