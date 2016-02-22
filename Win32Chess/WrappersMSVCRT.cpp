/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "IChessWrapper.h"
//#include <process.h>
//#include <hash_map>
#include "IATDetours.h"
#include "Win32SyncManager.h"
#include "Win32Wrappers.h"


uintptr_t (__cdecl *Real_beginthread)(void (__cdecl * _StartAddress) (void *),
									unsigned _StackSize, 
									void * _ArgList) = _beginthread;

uintptr_t __cdecl __wrapper_beginthread(void (__cdecl * _StartAddress) (void *),
									unsigned _StackSize, 
									void * _ArgList);

uintptr_t (__cdecl * Real_beginthreadex)(void * _Security,
									 unsigned _StackSize,
									 unsigned (__stdcall * _StartAddress) (void *),
									 void * _ArgList, 
									 unsigned _InitFlag,
									 unsigned * _ThrdAddr) = _beginthreadex;

uintptr_t __cdecl __wrapper_beginthreadex(void * _Security,
									 unsigned _StackSize,
									 unsigned (__stdcall * _StartAddress) (void *),
									 void * _ArgList, 
									 unsigned _InitFlag,
									 unsigned * _ThrdAddr);

void (__cdecl * Real_endthread)( void ) = _endthread;
void __wrapper_endthread( void );

void (__cdecl * Real_endthreadex)( unsigned retval ) = _endthreadex;
void __wrapper_endthreadex( unsigned retval );

int (* Real_rand)(void) = rand;
int __wrapper_rand(void);

//bool WrappersMSVCRT::RegisterTestModule(HMODULE hModule){
//	// wrap _beginthreadex
//	stdext::hash_map<void*, void*> wrapperTable;
//
//#ifndef _DLL
//#error "CHESS should be dynamically linked to the C RunTime"
//	*((void**)&Real_beginthread) = GetFunctionAddressByName(hModule, "MSVCR80D.dll", "_beginthread");
//	*((void**)&Real_beginthreadex) = GetFunctionAddressByName(hModule, "MSVCR80D.dll", "_beginthreadex");
//	*((void**)&Real_endthread) = GetFunctionAddressByName(hModule, "MSVCR80D.dll", "_endthread");
//	*((void**)&Real_endthreadex) = GetFunctionAddressByName(hModule, "MSVCR80D.dll", "_endthreadex");
//	*((void**)&Real_rand) = GetFunctionAddressByName(hModule, "MSVCR80D.dll", "rand");
//#else
//	wrapperTable[Real_beginthread] = __wrapper_beginthread;
//	wrapperTable[Real_beginthreadex] = __wrapper_beginthreadex;
//	wrapperTable[Real_endthread] = __wrapper_endthread;
//	wrapperTable[Real_endthreadex] = __wrapper_endthreadex;
//	if(Chess::GetOptions().wrap_random)
//		wrapperTable[Real_rand] = __wrapper_rand;
//#endif
//
//	return IATDetour(hModule, wrapperTable);
//}


uintptr_t __cdecl __wrapper_beginthreadex(void * _Security,
									 unsigned _StackSize,
									 unsigned (__stdcall * _StartAddress) (void *),
									 void * _ArgList, 
									 unsigned _InitFlag,
									 unsigned * _ThrdAddr)
{
	if(!ChessWrapperSentry::Wrap("beginthreadex")){
		return Real_beginthreadex(_Security, _StackSize, _StartAddress, _ArgList, _InitFlag, _ThrdAddr);
	}
	ChessWrapperSentry sentry;

	uintptr_t retVal;

	struct ThreadRoutineArg* threadArgs = (struct ThreadRoutineArg*) malloc(sizeof(struct ThreadRoutineArg));
	if (!threadArgs) {
		Chess::AbnormalExit(-1, "out of memory");
        return 0; //should never reach this
	}
	threadArgs->Function = (LPTHREAD_START_ROUTINE)_StartAddress;
	threadArgs->Context = _ArgList;

	Semaphore childSem;
	childSem.Init();

	threadArgs->selfSemaphore = childSem;

	retVal = _beginthreadex(_Security, _StackSize, (unsigned (__stdcall*)(void*))ThreadCreateWrapper, threadArgs, _InitFlag, _ThrdAddr);

	if (retVal != 0)
	{
		Task child;
		Chess::TaskFork(child);
		GetWin32SyncManager()->RegisterThreadSemaphore(child, childSem, TRUE);
		GetWin32SyncManager()->AddChildHandle(child, (HANDLE)retVal);
		if (_InitFlag != CREATE_SUSPENDED){
			Chess::ResumeTask(child);
		}
		else{
			// Chess::TaskFork() by default creates a suspended task
		}
	}
	else{
		threadArgs->selfSemaphore.Clear();
		free(threadArgs);
	}
	return retVal;
}

uintptr_t __cdecl __wrapper_beginthread(void (__cdecl * _StartAddress) (void *),
									unsigned _StackSize, 
									void * _ArgList)
{
	if(!ChessWrapperSentry::Wrap("beginthread")){
		return Real_beginthread(_StartAddress, _StackSize, _ArgList);
	}
	ChessWrapperSentry sentry;

	fprintf(stderr, "Chess does not implement _beginthread (yet)");
	return 0;
}

void __wrapper_endthread( void )
{
	if(!ChessWrapperSentry::Wrap("endthread")){
		_endthread();
	}
	ChessWrapperSentry sentry;

	Chess::EnterChess();
	GetWin32SyncManager()->ThreadEnd();
	_endthread();
}

void __wrapper_endthreadex( unsigned retval )
{
	if(!ChessWrapperSentry::Wrap("endthreadex")){
		_endthreadex(retval);
	}
	ChessWrapperSentry sentry;

	Chess::EnterChess();
	GetWin32SyncManager()->ThreadEnd();
	_endthreadex(retval);
}

int __wrapper_rand(void){
	if(!ChessWrapperSentry::Wrap("rand")){
		return Real_rand();
	}
	ChessWrapperSentry sentry;
	
	switch(Chess::Choose(3)){
		case 0: return 0;
		case 1: return RAND_MAX/2;
		default: return RAND_MAX;
	}
}


WrapperFunctionInfo MSVCRTWrapperFunctions[] = {
	WrapperFunctionInfo(_beginthread, __wrapper_beginthread, "MSVCRT::_beginthread"),
	WrapperFunctionInfo(_beginthreadex, __wrapper_beginthreadex, "MSVCRT::_beginthreadex"),
	WrapperFunctionInfo(_endthread, __wrapper_endthread, "MSVCRT::_endthread"),
	WrapperFunctionInfo(_endthreadex, __wrapper_endthreadex, "MSVCRT::_endthreadex"),
	WrapperFunctionInfo(rand, __wrapper_rand, "MSVCRT::rand"),
	WrapperFunctionInfo()
};

void RemoveWrapperFunction(const void* f){
	WrapperFunctionInfo* p = MSVCRTWrapperFunctions;
	WrapperFunctionInfo* findex = MSVCRTWrapperFunctions;
	WrapperFunctionInfo* lastindex = MSVCRTWrapperFunctions;
	while(p->origFunctionAddr){
		if(p->origFunctionAddr == f)
			findex = p;
		lastindex = p;
		p++;
	}
	*findex = *lastindex;
	*lastindex = WrapperFunctionInfo();
}

class WrappersMSVCRT : public IChessWrapper {
public:
	virtual WrapperFunctionInfo* GetWrapperFunctions(){
		if(!Chess::GetOptions().wrap_random){
			RemoveWrapperFunction(rand);
		}
		return MSVCRTWrapperFunctions;
	}

};

IChessWrapper* GetWrappersMSVCRT(){
	return new WrappersMSVCRT();
}
