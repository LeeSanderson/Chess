
#pragma once
#include "PCTExecution.h"
#include "SyncVar.h"

#include <map>

class PCTwithVBExecution : public PCTExecution {
public:
	PCTwithVBExecution(const Task tasks[], int n, int _bug_depth, int num_of_vars, int seed);
	PCTwithVBExecution(ChessExecution* exec,int bug_depth, int num_of_vars, int seed);
	void Reset();

protected:
	virtual bool IsPriorityChangePoint(ChessTransition& trans);

private:
	std::hash_set<SyncVar> variables_to_track;
	size_t Total_num_of_vars;
	size_t Num_of_vars_to_track;
	std::hash_map<SyncVar,int> var_to_k_map;
};