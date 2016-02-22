/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Win32MFCWrappers.cpp : Defines the exported functions for the DLL application.
//
#define _AFXDLL
#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>  

#include "Win32Base.h"
#include "Chess.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"
#include "IChessWrapper.h"
#include <iostream>
//#if (_WIN32_WINNT >= 0x0502) // I really dont know what this should be
//#define USE_MFC

#ifdef USE_MFC
CWinThread* __wrapper_AfxBeginThread(
   AFX_THREADPROC pfnThreadProc,
   LPVOID pParam,
   int nPriority,
   UINT nStackSize,
   DWORD dwCreateFlags,
   LPSECURITY_ATTRIBUTES lpSecurityAttrs
   )
{
	return AfxBeginThread(pfnThreadProc, pParam, nPriority, nStackSize, dwCreateFlags, lpSecurityAttrs);
}

CWinThread* (WINAPI *__real_AfxBeginThread)(
   AFX_THREADPROC pfnThreadProc,
   LPVOID pParam,
   int nPriority,
   UINT nStackSize,
   DWORD dwCreateFlags,
   LPSECURITY_ATTRIBUTES lpSecurityAttrs
   ) = AfxBeginThread;

WrapperFunctionInfo MFCWrapperFunctions[] = {
	WrapperFunctionInfo(__real_AfxBeginThread, __wrapper_AfxBeginThread, "mfc::AfxBeginThread"),
	WrapperFunctionInfo()
};

#else

WrapperFunctionInfo MFCWrapperFunctions[] = {
	WrapperFunctionInfo()
};

#endif

class WrappersMFC : public IChessWrapper {
public:
	virtual WrapperFunctionInfo* GetWrapperFunctions(){
		return MFCWrapperFunctions;
	}	
};

IChessWrapper* wrappers[8];

extern "C" __declspec(dllexport) IChessWrapper** GetChessWrappers(){
	int i = 0;
	wrappers[i] = new WrappersMFC();
	i++;
	wrappers[i] = 0;
	return wrappers;
}