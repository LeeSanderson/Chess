/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma unmanaged
#pragma warning(push)
#include <CodeAnalysis\Warnings.h>
#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings
#include <windows.h>
#pragma warning(pop)

extern "C" __declspec(dllexport)
DWORD CallThreadFunction(LPTHREAD_START_ROUTINE f, LPVOID a)
{
	return f(a);
}
