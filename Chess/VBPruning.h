
#pragma once
#include "Chess.h"
#include "IChessStrategy.h"
#include "ChessExecution.h"
#include <map>
#include "SyncVar.h"

class VBPruning {
public:
	VBPruning(int bound) {
		var_bound = bound;
	}
	bool ComputePremptionVars(ChessExecution* curr,IQueryEnabled *qEnabled);
private:
	std::hash_set<SyncVar> vars;
	size_t var_bound;
};