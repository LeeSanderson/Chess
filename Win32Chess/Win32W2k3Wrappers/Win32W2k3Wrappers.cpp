/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Win32W2k3Wrappers.cpp : Defines the exported functions for the DLL application.
//

#include "Win32Base.h"
#include "Chess.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"
#include "IChessWrapper.h"

#if (_WIN32_WINNT >= 0x0502) // I really dont know what this should be

LONGLONG WINAPI __wrapper_InterlockedCompareExchange64(
  __inout_ecount(sizeof(LONGLONG)) LONGLONG volatile* Destination,
  LONGLONG Exchange,
  LONGLONG Comparand
  )
{
	LONG* top = (LONG *) Destination;
	LONG* bottom = top+1;
	SyncVar vars[2];
	vars[0] = GetWin32SyncManager()->GetSyncVarFromAddress((void *)top);
	vars[1] = GetWin32SyncManager()->GetSyncVarFromAddress((void *)bottom);
	//SyncVar agg(vars, 2);
	Chess::AggregateSyncVarAccess(vars, 2, SVOP::RWVAR_READWRITE);
	//Chess::ThreadReadWriteSyncVar(agg);
	LONGLONG ret = InterlockedCompareExchange64(Destination, Exchange, Comparand);
	Chess::CommitSyncVarAccess();
	return ret;
}

WrapperFunctionInfo W2k3WrapperFunctions[] = {
	WrapperFunctionInfo(InterlockedCompareExchange64, __wrapper_InterlockedCompareExchange64, "kernel32::InterlockedCompareExchange64"),
	WrapperFunctionInfo()
};

#else

WrapperFunctionInfo W2k3WrapperFunctions[] = {
	WrapperFunctionInfo()
};

#endif

class WrappersW2k3 : public IChessWrapper {
public:
	virtual WrapperFunctionInfo* GetWrapperFunctions(){
		return W2k3WrapperFunctions;
	}	
};

IChessWrapper* wrappers[8];

extern "C" __declspec(dllexport) IChessWrapper** GetChessWrappers(){
	int i = 0;
	wrappers[i] = new WrappersW2k3();
	i++;
	wrappers[i] = 0;
	return wrappers;
}