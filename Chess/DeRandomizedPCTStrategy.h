
#pragma once
#include "IChessStrategy.h"
#include "DeRandomizedPCTExecution.h"
#include <map>
#include "RepVector.h"
#include <utility>
#include <stack>

class DeRandomizedPCTStrategy : public IChessStrategy {
public:
	DeRandomizedPCTStrategy(int bugdepth);
	virtual ChessExecution* InitialExecution();
	virtual void CompletedExecution(ChessExecution* curr);
	virtual ChessExecution* NextExecution(ChessExecution* curr, IQueryEnabled* qEnabled, size_t depthBound);

private:
	int PreemptionPoint(ChessExecution* exec,size_t backtrackpoint);
	size_t bugdepth;
	size_t backTrackStep;
	std::map<Task,PRIORITY> PriorityStack;
	bool firsttime;
	size_t crnt_bugdepth;
	std::vector<int> depth;
	std::stack<std::pair<Task,PRIORITY>> stack;
	bool debug;
};