/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessExecution.h"
#include "Chess.h"
#include "ChessImpl.h"

class SSEstimator{
public:
	void OnExecutionBegin(const IChessExecution* exec);
	void OnExecutionEnd(const IChessExecution* exec, int numExecs);
	
	void SSEstimator::DisplayEstimate(std::ostream& o, const IChessExecution* exec, int numExecs);
	
private:
	struct Info{
		size_t min;
		size_t max;
		int	num;
		size_t sum;
		Info(){
			min = max = num = sum = 0;
		}
	};

	std::vector<Info> stats;

	std::vector<size_t> stackSplit;
	std::vector<size_t> numBacktracksAtLevel;
	std::vector<Task> prevExec;
	std::vector<size_t> whenPushed;

};


