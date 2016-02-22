/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessApi.h"
#include "SyncVarOp.h"

class CHESS_API IChessStats{
public:
	virtual void OnNonlocalBacktrack(SyncVarOp op){}
	virtual void OnNewHBExecution(){}
	virtual void OnNewState(){}
	virtual ~IChessStats(){}

	virtual int GetNumExecutions(){return 0;}
	virtual int GetMaxThreads(){return 0;}
	virtual int GetMaxSteps(){return 0;}
	virtual int GetTotalSteps(){return 0;}
	virtual int GetMaxHBExecutions(){return 0;}
	virtual int GetMaxContextSwitches(){return 0;}
	virtual int GetTotalContextSwitches(){return 0;}
	virtual int GetElapsedTimeMS(){return 0;}

};