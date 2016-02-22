/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessApi.h"
#include "ChessStl.h"
#include <sstream>

class CHESS_API ErrorInfo
{
public:
	ErrorInfo();
	~ErrorInfo();

	char* Message;
	char* ExType;
	char* StackTrace;

	ErrorInfo** InnerErrors;
	int InnerErrorsCount;

	void WriteXml(std::ostream& writer) const;

};