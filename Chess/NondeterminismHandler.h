/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessExecution.h"
#include <vector>

class NondeterminismHandler{
public:
	NondeterminismHandler()
		: nondeterminismSeen(false), targetExecution(0){
		numAttemptsAtTargetExecution = 2;
	}

	void SetNumAttemptsAtTargetExecution(int i){
		if(i > 0){
			numAttemptsAtTargetExecution = i;
		}
	}

	ChessExecution* AccessNondeterminism(ChessExecution* exec, Task tid, SyncVar var, SyncVarOp op);
	ChessExecution* EnabledNondeterminism(ChessExecution* exec, Task tid);
	
	struct RecoveryStatus{
		enum RecoveryAction{
			NO_RECOVERY,
			EXECUTE_EXECUTION,
			BACKTRACK_FROM_EXECUTION
		} action;

		ChessExecution* execution; // when action > NO_RECOVERY
		int backtrackStep; // when action == BACKTRACK_FROM_EXECUTION
	};

	void RecoverNondeterminismOnBacktrack(ChessExecution* exec, RecoveryStatus& out);

private:

	// per execution state
	bool nondeterminismSeen;

	void CleanupPerExecutionState();

	// global state maintained across executions

	ChessExecution* targetExecution;
	// targetExecution == null implies that we havent detected any nondeterminism yet
	// otherwise, targetExecution is the execution for which nondeterminism was detected

	std::vector<ChessExecution*> exploredExecutions;
	// when targetExecution != null, exploredExecutions is the executions explored when 
	// attempting to execute the targetExecution

	void CleanupGlobalState();

	int numAttemptsAtTargetExecution;

	ChessExecution* ProcessNondeterminism(ChessExecution* exec);
	size_t GetCommonPrefixOfExploredExecutions();
	
	static bool reported_nondeterminism_already;
};