/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"

class WIN32CHESS_API StackWalkHelper{
public:
	static DWORD64 GetCallerPC(int frameNo);
	static void GetCallerPC(int frameStart, int numFrames, DWORD64* ret);
	
	static bool GetSymbolInfoForPC(DWORD64 pc, std::string& fn,std::string& filename, int& line);

};