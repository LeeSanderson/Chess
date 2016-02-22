/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

class ChessExecution;

#include "IQueryEnabled.h"

// Interface for implementing search strategies

class IChessStrategy{
public:
	virtual ~IChessStrategy(){}

	// Return the initial partial execution
	virtual ChessExecution* InitialExecution(){return 0;}

	// CHESS notifies that the previously provided execution has been completed
	virtual void CompletedExecution(ChessExecution* curr){}

	// Return the next partial execution for the current completed execution
	// qEnabled provides the enabled tasks at each step of the execution
	// depthBound indicates that CHESS is not interested in exploring beyond the depth. 0 indicates no depth bound
	virtual ChessExecution* NextExecution(ChessExecution* curr, IQueryEnabled* qEnabled, size_t depthBound){return 0;}
};