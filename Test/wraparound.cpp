/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

__declspec(dllimport) DWORD WINAPI RunTestOrig(LPVOID Args);

extern "C" 
__declspec(dllexport) int ChessTestRun(){
	return RunTestOrig(args);
}