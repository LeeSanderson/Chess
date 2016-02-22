/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "ChessWrapperSentry.h"

DWORD ChessWrapperSentry::tlsInChessFlag = 0;

bool ChessWrapperSentry::Init(){
	if(tlsInChessFlag != 0){
		return true;
	}
	DWORD index = ::TlsAlloc();
	if(index == TLS_OUT_OF_INDEXES){
		return false;
	}
	tlsInChessFlag = index;
	return true;
}
