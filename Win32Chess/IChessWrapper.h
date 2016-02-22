/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"
#include "Chess.h"

struct WrapperFunctionInfo{
	void* origFunctionAddr;
	void* wrapperFunctionAddr;
	char* functionName;

	WrapperFunctionInfo()
		: origFunctionAddr(0), wrapperFunctionAddr(0), functionName(0) {}
	WrapperFunctionInfo(void* a, void* b, char* c)
		: origFunctionAddr(a), wrapperFunctionAddr(b), functionName(c) {}
};


class WIN32CHESS_API IChessWrapper {
public:
	// return "null terminated" array of WrapperFunctionInfos. A null WrapperFunctionInfo has a null origFunctionAddr
	virtual WrapperFunctionInfo* GetWrapperFunctions(){return 0;}

	virtual void SetInitState(){}
	virtual void Reset(){}
	virtual void TaskEnd(Task tid){}

	virtual ~IChessWrapper(){}
};

class ChessErrorSentry{
public:
	ChessErrorSentry(){error = GetLastError(); valid = true;}
	void Clear(){valid = false;}
	~ChessErrorSentry(){if(valid) SetLastError(error);}
private:
	DWORD error;
	bool valid;
};