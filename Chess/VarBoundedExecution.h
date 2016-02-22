

#pragma once
#include "Chess.h"
#include "PCTExecution.h"


class VarBoundedExecution : public PCTExecution{
public:
	VarBoundedExecution(const Task initTasks[], int n,int bug_depth);
	VarBoundedExecution(ChessExecution* exec,int bug_depth);
	~VarBoundedExecution();


protected:
	

private:
	std::hash_map<GCAddress,int> variables_to_track;	
};