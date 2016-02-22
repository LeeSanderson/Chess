/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/


#ifdef STACK_TRACE_PER_TRANSITION
#include "Win32Base.h"
#include <windows.h>
#include <DbgHelp.h>
#include <string>
//#include <iostream>
#include "StackWalkHelper.h"
#include <iostream>
DWORD64 StackWalkHelper::GetCallerPC(int frameNo){
	CONTEXT c;
	STACKFRAME64 s;
	memset(&c, 0, sizeof(c));
	memset(&s, 0, sizeof(s));

	RtlCaptureContext(&c);
	s.AddrPC.Offset = c.Eip;
	s.AddrPC.Mode = AddrModeFlat;
	s.AddrFrame.Offset = c.Ebp;
	s.AddrFrame.Mode = AddrModeFlat;
	s.AddrStack.Offset = c.Esp;
	s.AddrStack.Mode = AddrModeFlat;

	// Since GetCallerPC is implemented as a function (as opposed to inlined), we increment by one stack frame
	frameNo++;

	for(int i=0; i<=frameNo; i++){
		if(!StackWalk64(IMAGE_FILE_MACHINE_I386, GetCurrentProcess(), GetCurrentThread(), 
			&s, &c, NULL, SymFunctionTableAccess64, SymGetModuleBase64, NULL)){
				return 0;
		}
	}
	return s.AddrPC.Offset;
}

#pragma warning(push)
#pragma warning(disable: 25057) //I gave it the ecount annotation, prefix is still complaining - dont know why, madan
void StackWalkHelper::GetCallerPC(int frameStart, int numFrames, __inout_ecount(numFrames) DWORD64 ret[]){
#pragma warning(pop)

	CONTEXT c;
	STACKFRAME64 s;
	memset(&c, 0, sizeof(c));
	memset(&s, 0, sizeof(s));

	RtlCaptureContext(&c);
	s.AddrPC.Offset = c.Eip;
	s.AddrPC.Mode = AddrModeFlat;
	s.AddrFrame.Offset = c.Ebp;
	s.AddrFrame.Mode = AddrModeFlat;
	s.AddrStack.Offset = c.Esp;
	s.AddrStack.Mode = AddrModeFlat;

	// Since GetCallerPC is implemented as a function (as opposed to inlined), we increment by one stack frame
	//frameStart++; This increment is a bug. StackWalk64 gets the 'next' stack frame!!

	for(int i=0; i<numFrames; i++){
		ret[i] = 0;
	}

	for(int i=0; i< frameStart+numFrames; i++){
		if(!StackWalk64(IMAGE_FILE_MACHINE_I386, GetCurrentProcess(), GetCurrentThread(), 
			&s, &c, NULL, SymFunctionTableAccess64, SymGetModuleBase64, NULL)){
				return;
		}
		if(i >= frameStart){
			ret[i-frameStart] = s.AddrPC.Offset;
		}
	}
}

bool StackWalkSymInitialized = false;
bool StackWalkSymInitializeFailed = false;

bool StackWalkHelper::GetSymbolInfoForPC(DWORD64 pc, std::string& fn, std::string& fileName, int& lineNumber){
	HANDLE hProcess = GetCurrentProcess();
	if(!StackWalkSymInitialized){
		if(!SymInitialize(hProcess, NULL, true)){
			*GetChessErrorStream()<< "SymInitialize Failed with " << GetLastError() << std::endl;
			StackWalkSymInitializeFailed = true;
		}
		StackWalkSymInitialized = true;
	}

	if(StackWalkSymInitializeFailed){
		fn = "";
		fileName = "";
		lineNumber = 0;
		return false;
	}

	DWORD64  dwDisplacement;

	ULONG64 buffer[(sizeof(SYMBOL_INFO) +
		MAX_SYM_NAME*sizeof(TCHAR) +
		sizeof(ULONG64) - 1) /
		sizeof(ULONG64)];
	PSYMBOL_INFO pSymbol = (PSYMBOL_INFO)buffer;

	pSymbol->SizeOfStruct = sizeof(SYMBOL_INFO);
	pSymbol->MaxNameLen = MAX_SYM_NAME;

	if (!SymFromAddr(hProcess, pc, &dwDisplacement, pSymbol))
	{
		// SymFromAddr failed
		return false;
		//*GetChessErrorStream()<< "SymFromAddr64 failed with " << GetLastError() << std::endl;
		//fn = "";
	}
	else{
		fn = pSymbol->Name;
	}

	DWORD  dwDisplacement2;
	IMAGEHLP_LINE64 line;

	SymSetOptions(SYMOPT_LOAD_LINES);

	line.SizeOfStruct = sizeof(IMAGEHLP_LINE64);

	if (!SymGetLineFromAddr64(hProcess, pc, &dwDisplacement2, &line))
	{
		return false;
		/**GetChessErrorStream()<< "SymGetLineFromAddr64 failed with " << GetLastError() << std::endl;
		fileName = "";
		lineNumber = 0;*/
	}
	else{
		fileName = line.FileName;
		lineNumber = line.LineNumber;
	}
	return true;
}

#endif