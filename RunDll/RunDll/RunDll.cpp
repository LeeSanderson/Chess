/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// RunDll.cpp : Defines the entry point for the console application.
//


#pragma warning(push)

#include <CodeAnalysis\Warnings.h>

#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25032 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings
#include "stdafx.h"

#include <windows.h>
#include <iostream>
#include <string>
#include <hash_map>

#define CHESS_IMPORT
#include "IChessMonitor.h"
#include "Chess.h"
#include "Win32SyncManager.h"
//#include "IATDetours.h"
#include "..\Win32Chess\IATDetours.cpp"
#include "TracePrinter.h"
#include "EventCounter.h"

TracePrinter* tracePrinter;
#pragma warning(pop)

typedef int (*CHESS_TEST_RUN_TYPE)();
bool attached = false;

CHESS_TEST_RUN_TYPE LoadEntryPoint(HMODULE handle, const char* fnName){
	CHESS_TEST_RUN_TYPE ret = (CHESS_TEST_RUN_TYPE) GetProcAddress(handle, fnName); 

	if(ret == NULL){
		// try if we can find _fnName@4
		std::string fnNameMangled = "_"+std::string(fnName)+"@4";
		ret = (CHESS_TEST_RUN_TYPE) GetProcAddress(handle, fnNameMangled.c_str()); 
	}
	return ret;
}

CRITICAL_SECTION RunDllCS;
class AutoRunDllCS{
public:
	AutoRunDllCS(){EnterCriticalSection(&RunDllCS);}
	~AutoRunDllCS(){LeaveCriticalSection(&RunDllCS);}
};

stdext::hash_map<DWORD, size_t> ThreadIdToVar;
stdext::hash_map<void*, size_t> AddrToVar;
stdext::hash_map<HANDLE, size_t> HandleToVar;
size_t nextTid = 1;
size_t nextVar = 512;

EventCounter eventCounter;

bool randomMode = false;
bool stressMode = false;
int stressParam = 5;

size_t GetTid(DWORD threadId){
	AutoRunDllCS sentry;
	if(ThreadIdToVar.find(threadId) == ThreadIdToVar.end()){
		ThreadIdToVar[threadId] = nextTid++;
	}
	return ThreadIdToVar[threadId];
}
size_t GetTid(){
	AutoRunDllCS sentry;
	DWORD threadId = GetCurrentThreadId();
	return GetTid(threadId);
}

size_t GetVar(void* addr){
	AutoRunDllCS sentry;
	if(AddrToVar.find(addr) == AddrToVar.end()){
		AddrToVar[addr] = nextVar++;
	}
	return AddrToVar[addr];
}
size_t GetVarFromHandle(HANDLE h){
	AutoRunDllCS sentry;
	if(HandleToVar.find(h) == HandleToVar.end()){
		HandleToVar[h] = nextVar++;
	}
	return HandleToVar[h];
}
void RegisterHandle(HANDLE h, size_t v){
	AutoRunDllCS sentry;
	HandleToVar[h] = v;
}
void Reset(){
	AutoRunDllCS sentry;
	ThreadIdToVar.clear();
	AddrToVar.clear();
	nextTid = 1;
	nextVar = 512;
	GetTid();
	eventCounter.clear();
}


std::string GetLastErrorString(){
	char errorstr[256];
	DWORD error = GetLastError();
#pragma warning (push)
#pragma warning (disable: 25068)
	FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM, NULL, error, 0, errorstr, 256, NULL);
#pragma warning (pop)
	return errorstr;
}

CRITICAL_SECTION guiCs;
void SyncVarAccess(Task tid, size_t var, SyncVarOp op){	
	EnterCriticalSection(&guiCs);
	EventId id = eventCounter.getNext(tid, true);
	tracePrinter->OnSchedulePoint(IChessMonitor::SVACCESS, id, var, op, 0);
	//tracePrinter->OnSyncVarAccess(eventCounter.getNext(tid), tid, var, op);
	tracePrinter->OnEventAttributeUpdate(id, STATUS, "c");
	LeaveCriticalSection(&guiCs);
}

struct ThreadWrapperStruct{
	LPTHREAD_START_ROUTINE lpStartAddress;
	LPVOID lpParameter;
};
DWORD WINAPI ThreadWrapper(LPVOID arg){
	ThreadWrapperStruct* wr = (ThreadWrapperStruct*)arg;
//	SyncVarAccess(GetTid(), GetTid(), SVOP::TASK_BEGIN);
	DWORD ret = wr->lpStartAddress(wr->lpParameter);
	SyncVarAccess(GetTid(), GetTid(), SVOP::TASK_END);
	delete wr;
	return ret;
}

HANDLE WINAPI MyCreateThread(
									 LPSECURITY_ATTRIBUTES lpThreadAttributes,
									 SIZE_T dwStackSize,
									 LPTHREAD_START_ROUTINE lpStartAddress,
									 LPVOID lpParameter,
									 DWORD dwCreationFlags,
									 LPDWORD lpThreadId)
{
	if(!attached)
		return CreateThread(lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);

	DWORD tid;
	if(lpThreadId == 0){
		lpThreadId = &tid;
	}
	ThreadWrapperStruct* wr = new ThreadWrapperStruct();
	wr->lpStartAddress = lpStartAddress;
	wr->lpParameter = lpParameter;
	HANDLE ret = CreateThread(lpThreadAttributes, dwStackSize, ThreadWrapper, wr, CREATE_SUSPENDED, lpThreadId);
	if(ret != NULL){
		SyncVarAccess(GetTid(), GetTid(*lpThreadId), SVOP::TASK_FORK);
		SyncVarAccess(GetTid(), GetTid(*lpThreadId), SVOP::TASK_RESUME);
		RegisterHandle(ret, GetTid(*lpThreadId));
		if ((dwCreationFlags & CREATE_SUSPENDED) == 0){
			ResumeThread(ret);
		}
	}
	if(stressMode && rand()%5 == 0) Sleep(1);
	return ret;

}

void WINAPI MyEnterCriticalSection(LPCRITICAL_SECTION lpCriticalSection)
{
	if(randomMode && (rand()%2 == 0)) Sleep(0);
	EnterCriticalSection(lpCriticalSection);	
	if(attached) SyncVarAccess(GetTid(), GetVar(lpCriticalSection), SVOP::LOCK_ACQUIRE);
}

void WINAPI MyLeaveCriticalSection(LPCRITICAL_SECTION lpCriticalSection)
{
	if(attached) SyncVarAccess(GetTid(), GetVar(lpCriticalSection), SVOP::LOCK_RELEASE);
	LeaveCriticalSection(lpCriticalSection);
	if(randomMode && rand()%5 == 0) Sleep(rand()%100);
}


DWORD WINAPI MyWaitForSingleObject(
	HANDLE hHandle,
	DWORD dwMilliseconds)
{
	DWORD ret = WaitForSingleObject(hHandle, dwMilliseconds);
	if(attached) SyncVarAccess(GetTid(), GetVarFromHandle(hHandle), SVOP::WAIT_ALL); 
	return ret;
}

LONG WINAPI MyInterlockedIncrement(__inout LONG volatile *lpAddend)
{
	if(attached) SyncVarAccess(GetTid(), GetVar((void*)lpAddend), SVOP::RWVAR_READWRITE);
	return InterlockedIncrement(lpAddend);
}

LONG WINAPI MyInterlockedDecrement(__inout LONG volatile *lpAddend)
{
	if(attached) SyncVarAccess(GetTid(), GetVar((void*)lpAddend), SVOP::RWVAR_READWRITE);
	return InterlockedDecrement(lpAddend);
}

LONG WINAPI MyInterlockedExchange(__inout LONG volatile *Target, __in LONG Value)
{
	if(attached) SyncVarAccess(GetTid(), GetVar((void*)Target), SVOP::RWVAR_READWRITE);
	return InterlockedExchange(Target, Value);
}

LONG WINAPI MyInterlockedCompareExchange(
	__inout LONG volatile *Destination, 
	__in LONG Exchange, 
	__in LONG Comperand)
{
	if(attached) SyncVarAccess(GetTid(), GetVar((void*)Destination), SVOP::RWVAR_READWRITE);
	return InterlockedCompareExchange(Destination, Exchange, Comperand);
}



//DWORD WINAPI MyWaitForMultipleObjects(
//	DWORD nCount, 
//	CONST HANDLE* lpHandles, 
//	BOOL fWaitAll, 
//	DWORD dwMilliseconds) 
//{
//	DWORD ret = MyWaitForMultipleObjects(nCount, lpHandles, fWaitAll, dwMilliseconds);
//	SyncVarAccess(ChessTransition
//	return ret;
//}

volatile bool stressDone = false;
#pragma warning(push)
#pragma warning(disable: 25004) // thread functions have to be LPVOID (and not const LPVOID)
DWORD WINAPI StressFunction(LPVOID param){
#pragma warning(pop)
	int recCount = (int) param;
	HANDLE child = NULL;
	for(int k=0; k<2; k++){
		if(recCount > 0){
			child = CreateThread(NULL, 0, StressFunction, (LPVOID)(recCount-1), NULL, NULL);
		}
		while(!stressDone){
			// do crap
			for(int i=0; i<(stressParam*100); i++){
				int len = 100*i + 43;
				char* buff = (char*)malloc(len);
                if (buff) 
				{
					for(int j=0; j<len; j++){
						buff[j] = (char)(i*i*1.0/j + j*j*1.0/17);
					}
					free(buff);
				}
				if(i%45 == 0) Sleep(1);
			}
		}

		if(child){
			WaitForSingleObject(child, INFINITE);
		}
	}
	return 0;
}

void SplitArgument(const std::string& str, std::vector<std::string>& ret, char c = ':')
{
	std::string::size_type pos = str.find(c);
	if(pos == std::string::npos){
        ret.push_back(str);
    }
    else{
        if(pos == 0 || pos == str.length()-1){
            std::cout << "Argument invalid: " << "\"" << str << "\"" << std::endl;
            exit(-1);
        }
        ret.push_back(str.substr(0, pos));
		SplitArgument(str.substr(pos+1, str.length()-pos-1), ret, c);
        //ret.push_back(str.substr(pos+1, str.length()-pos-1));
    }
}

int main(int argc, char* argv[])
{
	bool guiMode = true;
	bool wrappersOn = true;

	Win32SyncManager* sm = new Win32SyncManager();
	ChessOptions opts;
	ChessInit(opts, sm);

	const char* dllName = 0;
	int numIter = 100; //default
	stressMode = false;
	randomMode = false;

	InitializeCriticalSection(&RunDllCS);

	for (int i = 1; i < argc; i++) {
        if (argv[i][0] == '-' || argv[i][0] == '/')
        {
			if(argv[i][0] == '/'){
				argv[i][0] = '-';
			}
			std::vector<std::string> split_argv;
			std::string argstr = argv[i];
			for(std::string::size_type p=0; p<argstr.length(); p++){
				if(argstr.at(p) == '='){
					argstr.at(p) = ':'; // replace all '=' with ':'
				}
			}
			SplitArgument(argstr, split_argv);
			if(split_argv[0] == "-iter"){
				numIter = atoi(split_argv[1].c_str());
				if(numIter <= 0){
					std::cerr<< "Invalid parameter to -iter" << std::endl;
					exit(-1);
				}
			}
			else if(split_argv[0] == "-seed"){
				int seed = atoi(split_argv[1].c_str());
				if(seed < 0){
					std::cerr<< "Invalid parameter to -iter" << std::endl;
					exit(-1);
				}
				srand(seed);
			}			
			else if(split_argv[0] == "-stress"){
				stressMode = true;
				if(split_argv.size() > 1){
					int sp = atoi(split_argv[1].c_str());
					if(sp > 0){
						stressParam = sp;
					}
				}
			}
			else if(split_argv[0] == "-nogui"){
				guiMode = false;
			}
			else if(split_argv[0] == "-nowrap"){
				wrappersOn = false;
			}
			else if(split_argv[0] == "-contest"){
				randomMode = true;
			}
			else{
				std::cerr<< "Skipping unknown argument " << argv[i] << std::endl;
			}
		}
		else{
			dllName = argv[i];
		}
	}
	
	if(!dllName){
		std::cout << argv[0] << " needs a dll as input " << std::endl;
		exit(-1);
	}

	if(guiMode)
	{
		opts.gui = true;
		tracePrinter = new TracePrinter(&opts);
	}

	InitializeCriticalSection(&guiCs);
#pragma warning (push)
#pragma warning (disable: 25068)
	HMODULE handle = LoadLibraryA(dllName);
#pragma warning (pop)
	if (handle == NULL)
	{
		std::cout << "Load Library of " << dllName << " failed : " << GetLastErrorString() << std::endl;
		exit(-1);
	}

	if(stressMode){
		CreateThread(NULL, 0, StressFunction, (LPVOID)(stressParam*10), NULL, NULL);
	}
	stdext::hash_map<void*, void*> fnTable;
	if(wrappersOn){
		fnTable[EnterCriticalSection] = MyEnterCriticalSection;
		fnTable[LeaveCriticalSection] = MyLeaveCriticalSection;
		fnTable[CreateThread] = MyCreateThread;
		fnTable[WaitForSingleObject] = MyWaitForSingleObject;
#ifdef _X86_
		fnTable[(LONG (__stdcall *)(volatile LONG *)) &InterlockedIncrement] = MyInterlockedIncrement;
		fnTable[(LONG (__stdcall *)(volatile LONG *)) &InterlockedDecrement] = MyInterlockedDecrement;
		fnTable[(LONG (__stdcall *)(volatile LONG *, LONG)) &InterlockedExchange] = MyInterlockedExchange;
		fnTable[(LONG (__stdcall *)(volatile LONG *, LONG, LONG)) &InterlockedCompareExchange] = MyInterlockedCompareExchange;
#endif
	}

	if(!IATDetour(handle, fnTable)){
		std::cerr<< "Cannot detour Win32 functions" << std::endl;
		return false;
	}

	const char* fnName = "ChessTestRun";
	CHESS_TEST_RUN_TYPE runTest = LoadEntryPoint(handle, fnName);
	if (runTest == NULL)
	{
		std::cout << "GetProcAddress failed (couldn't find symbol '" << fnName << "' in the .DLL)" << std::endl;
		exit(-1);
	}
	
	attached = true;

	Reset();
	DWORD start = GetTickCount();
	for(int i=0; i<numIter; i++){
		if(guiMode)
			tracePrinter->OnExecutionBegin(0);
		runTest();
		Reset();
	}
	DWORD elapsed = GetTickCount()-start;
	//std::cout << numIter << " iterations in " << elapsed << " ms" << std::endl;

	attached = false;
	return 0;
}

