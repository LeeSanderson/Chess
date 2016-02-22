/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "Win32Base.h"
#include "ChessAssert.h"
#include "Chess.h"

class WIN32CHESS_API ChessWrapperSentry{
public:
	ChessWrapperSentry() {
		Chess::EnterChess();
	}

	~ChessWrapperSentry() {
		Chess::LeaveChess();
	}

	static void EnterChessImpl() {
		BOOL ret = ::TlsSetValue(tlsInChessFlag, (LPVOID)(((size_t)TlsGetValue(tlsInChessFlag))+1));
		assert(ret);
	}

	static void LeaveChessImpl() {
		DWORD errorCode = ::GetLastError();
		BOOL ret = ::TlsSetValue(tlsInChessFlag, (LPVOID)(((size_t)TlsGetValue(tlsInChessFlag))-1));
//		BOOL ret = ::TlsSetValue(tlsInChessFlag, (LPVOID)0);
		::SetLastError(errorCode);
		assert(ret);
	}

	static bool Init();

	static bool Wrap(const char* str) {
		return Chess::IsInitialized() && TlsGetValue(tlsInChessFlag) == (LPVOID)0;
	}

	static DWORD tlsInChessFlag;
};

