/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessBase.h"
#include "IChessMonitor.h"
#include "IChessStats.h"
#include <hash_set>

class HBMonitor : public IChessMonitor{
public:
	HBMonitor(IChessStats* s)
		: stats(s) {}

	virtual void OnExecutionEnd(IChessExecution* exec);

private:
	IChessStats* stats;
	stdext::hash_set<size_t> hbsets;
};

