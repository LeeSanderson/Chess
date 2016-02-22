/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "Chess.h"
#include "IATDetours.h"
#include "Win32Wrappers.h"
#include "Win32SyncManager.h"
#include "IChessWrapper.h"
#ifdef USE_WRAPPERS_FLTLIB

#define PFILTER_MESSAGE_HEADER PVOID
//extern "C"{
//__declspec(dllimport)
//HRESULT
//WINAPI  
//FilterSendMessage(
//				  IN HANDLE  hPort,    
//				  IN LPVOID  lpInBuffer OPTIONAL,    
//				  IN DWORD  dwInBufferSize,    
//				  IN OUT LPVOID  lpOutBuffer OPTIONAL,    
//				  IN DWORD  dwOutBufferSize,    
//				  OUT LPDWORD  lpBytesReturned    ); 
//HRESULT
//WINAPI  
//FilterGetMessage(    
//				 IN HANDLE  hPort,    
//				 IN OUT PFILTER_MESSAGE_HEADER  lpMessageBuffer,    
//				 IN DWORD  dwMessageBufferSize,    
//				 IN LPOVERLAPPED  lpOverlapped OPTIONAL    );
//}

HRESULT (WINAPI * Real_FilterSendMessage)( 
	HANDLE  hPort,    
	LPVOID  lpInBuffer,    
	DWORD  dwInBufferSize,    
	LPVOID  lpOutBuffer,    
	DWORD  dwOutBufferSize,    
	LPDWORD  lpBytesReturned    )
	= 0;

HRESULT (WINAPI * Real_FilterGetMessage)(    
	HANDLE  hPort,    
	PFILTER_MESSAGE_HEADER  lpMessageBuffer,    
	DWORD  dwMessageBufferSize,    
	LPOVERLAPPED  lpOverlapped )
	= 0;

HRESULT __wrapper_FilterSendMessage( 
				  HANDLE  hPort,    
				  LPVOID  lpInBuffer,    
				  DWORD  dwInBufferSize,    
				  LPVOID  lpOutBuffer,    
				  DWORD  dwOutBufferSize,    
				  LPDWORD  lpBytesReturned    );

HRESULT __wrapper_FilterGetMessage(    
	HANDLE  hPort,    
	PFILTER_MESSAGE_HEADER  lpMessageBuffer,    
	DWORD  dwMessageBufferSize,    
	LPOVERLAPPED  lpOverlapped );

__declspec(dllexport) HRESULT WINAPI Mine_FilterSendMessage( 
				  HANDLE  hPort,    
				  LPVOID  lpInBuffer,    
				  DWORD  dwInBufferSize,    
				  LPVOID  lpOutBuffer,    
				  DWORD  dwOutBufferSize,    
				  LPDWORD  lpBytesReturned    ){
  if(ChessWrapperSentry::Wrap("FilterSendMessage")){
     ChessWrapperSentry sentry;
	 HRESULT ret = __wrapper_FilterSendMessage(hPort, lpInBuffer, dwInBufferSize, lpOutBuffer, dwOutBufferSize, lpBytesReturned);
     return ret;
  }
  return Real_FilterSendMessage(hPort, lpInBuffer, dwInBufferSize, lpOutBuffer, dwOutBufferSize, lpBytesReturned);
}

__declspec(dllexport) HRESULT WINAPI Mine_FilterGetMessage(    
	HANDLE  hPort,    
	PFILTER_MESSAGE_HEADER  lpMessageBuffer,    
	DWORD  dwMessageBufferSize,    
	LPOVERLAPPED  lpOverlapped ){
  if(ChessWrapperSentry::Wrap("FilterGetMessage")){
     ChessWrapperSentry sentry;
	 HRESULT ret = __wrapper_FilterGetMessage(hPort, lpMessageBuffer, dwMessageBufferSize, lpOverlapped);
     return ret;
  }
  return Real_FilterGetMessage(hPort, lpMessageBuffer, dwMessageBufferSize, lpOverlapped);
}

void GetFltTable(stdext::hash_map<void*, void*>& wrapperTable){
	HMODULE fltModule = LoadLibrary("fltlib.dll");
	if(fltModule == NULL)
		return;

	*((void**)&Real_FilterSendMessage) = GetProcAddress(fltModule, "FilterSendMessage");
	if(!Real_FilterSendMessage){
		*GetChessErrorStream()<< "Cannot GetProcAddress for FilterSendMessage" << std::endl;
		return;
	}
	wrapperTable[Real_FilterSendMessage] = Mine_FilterSendMessage; 

	*((void**)&Real_FilterGetMessage) = GetProcAddress(fltModule, "FilterGetMessage");
	if(!Real_FilterGetMessage){
		*GetChessErrorStream()<< "Cannot GetProcAddress for FilterGetMessage" << std::endl;
		return;
	}
	wrapperTable[Real_FilterGetMessage] = Mine_FilterGetMessage; 
}

/// Wrappers
stdext::hash_map<HANDLE, int> qsize;
HANDLE hPorte = (HANDLE)1110;

HRESULT __wrapper_FilterSendMessage( 
				  HANDLE  hPort,    
				  LPVOID  lpInBuffer,    
				  DWORD  dwInBufferSize,    
				  LPVOID  lpOutBuffer,    
				  DWORD  dwOutBufferSize,    
				  LPDWORD  lpBytesReturned    )
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hPorte), SVOP::FLT_RW);
	if(qsize.find(hPorte) == qsize.end())
		qsize[hPorte] = 0;
	while(qsize[hPorte] == 0){
		Chess::LocalBacktrack();
	}
	qsize[hPorte]--;
	return Real_FilterSendMessage(hPort, lpInBuffer, dwInBufferSize, lpOutBuffer, dwOutBufferSize, lpBytesReturned);
}

HRESULT __wrapper_FilterGetMessage(    
	HANDLE  hPort,    
	PFILTER_MESSAGE_HEADER  lpMessageBuffer,    
	DWORD  dwMessageBufferSize,    
	LPOVERLAPPED  lpOverlapped )
{	
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hPorte), SVOP::FLT_RW);
	if(qsize.find(hPorte) == qsize.end())
		qsize[hPorte] = 0;
	if(lpOverlapped != NULL){
		HRESULT ret = Real_FilterGetMessage(hPort, lpMessageBuffer, dwMessageBufferSize, lpOverlapped);
		if(ret == ERROR_IO_PENDING)
			qsize[hPorte]++;
		return ret;
	}

	LPOVERLAPPED myOverlapped;
	OVERLAPPED tempOverlapped;
	memset(&tempOverlapped, 0, sizeof(OVERLAPPED));
	myOverlapped = &tempOverlapped;
	myOverlapped->hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

	HRESULT ret = Real_FilterGetMessage(hPort, lpMessageBuffer, dwMessageBufferSize, myOverlapped);
	//if(ret != S_OK || ret != ERROR_IO_PENDING)
	//	return ret;
	
	assert(ret != S_OK);
	qsize[hPorte]++;

	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hPorte), SVOP::FLT_RW);
	while(true){
		HRESULT retVal = WaitForSingleObject(myOverlapped->hEvent, 0);
		switch(retVal){
			case WAIT_OBJECT_0 :
				CloseHandle(myOverlapped->hEvent);
				return S_OK;

			case WAIT_TIMEOUT :
				Chess::LocalBacktrack();
				break;	
			default:
				fprintf(stderr, "Waiting for event failed in Wrapper FilterGetMessage");
				return retVal;
		}
	}
	assert(false);
	return S_OK;
}

#endif
