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

BOOL WINAPI __wrapper_ReadConsoleInputW(
    IN HANDLE hConsoleInput,
    OUT PINPUT_RECORD lpBuffer,
    IN DWORD nLength,
    OUT LPDWORD lpNumberOfEventsRead
	)
{
	if(!ChessWrapperSentry::Wrap("ReadConsoleInputW")){
		return ReadConsoleInputW(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);
	}
	ChessWrapperSentry sentry;

	while(true){
		Chess::SyncVarAccess(-2, SVOP::WAIT_ANY);
		DWORD ret = WaitForSingleObject(hConsoleInput, 0);
		switch(ret){
			case WAIT_OBJECT_0:
				return ReadConsoleInputW(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);

			case WAIT_TIMEOUT :
				Chess::LocalBacktrack();
				break;

			default:
				// dont know what else to do
				return ReadConsoleInputW(hConsoleInput, lpBuffer, nLength, lpNumberOfEventsRead);
		}
	}
}

void GetWrappersConsoleTable(stdext::hash_map<void*, void*>& wrapperTable){
	wrapperTable[&ReadConsoleInputW] = __wrapper_ReadConsoleInputW;
}


