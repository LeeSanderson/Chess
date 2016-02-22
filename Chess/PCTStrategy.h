
#pragma once
#include "ChessBase.h"

#include "IChessStrategy.h"

class ChessExecution;

class PCTStrategy : public IChessStrategy{
public :
	PCTStrategy(int _num_of_runs) : num_of_runs(_num_of_runs) {}

	ChessExecution* InitialExecution();
	void CompletedExecution(ChessExecution* curr);

	ChessExecution* NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound);

private :
    int num_of_runs;
};