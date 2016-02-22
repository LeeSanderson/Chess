/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessExecution.h"
#include "ContextSwitch.h"
#include "ExecutionSummary.h"
#include "BitVector.h"

#define MAX_FRAME_DEPTH 16
#define MAX_STRLEN 1024
#define MAX_BUFSIZE 16*1024

// Works with the BestFirstStrategy.  Unlike the ChessExecution, this class uses a compressed version
// of the execution for replay that consists of a map of <size_t, ContextSwitch*> indicating at what
// steps a transition other than the ChessExecution record mode default must be performed.  For each
// of those steps there is a ContextSwitch object, which indicates which task to schedule
// at that step, and a boolean indicating whether that action represents a preemption.  The bool may
// be unnecessary, but I couldn't think of a clean way to get that to work right while still
// making it easy to figure out whether a preemption has been done.  The problem is that the enabled
// set will not be updated right now unless you execute the task in question.  So, if a preemption
// occurs, you don't know it is a preemption until you have scheduled the task, and once you have
// scheduled the task it is too late to preempt it.
class BestFirstExecution : public ChessExecution {
public:
	BestFirstExecution(Task tasks[], int n);
	BestFirstExecution(BestFirstExecution* exec);

	// overrides
	bool LocalBacktrack();
	void Serialize(std::ostream& ostr);
	void Deserialize(std::istream& istr);
	void Reset();
	ChessExecution* Clone();
	int NextTaskToSchedule(Task& next);

	void Reinitialize();
	void Backtrack(ContextSwitch* cs);
	bool CommitSyncVarAccess(int nr);
	ContextSwitch* GetEndSwitch() const;
	const ExecutionSummary* GetReplaySummary() const;
	int Choose(Task tid, int numChoices);
	void PruneExecution(size_t index);
	void SetEndSwitch(ContextSwitch* cs);
	void PrioritizePreemptions(Task tid);
	void UnprioritizePreemptions(Task tid);
	bool IsPrioritizedStep(size_t step) const;
	bool ReplaySuccessful() const { return m_replaySuccessful; }
	void Reenable(size_t nstep);

protected:
	// overrides
	int SyncVarAccess(ChessTransition& trans);
	int ReplayTaskToSchedule(Task& next);
	void QuiescentWakeup(Task tid);
	
private:
	// This is just a fancy hash map from size_t to ContextSwitch* that makes it fast and easy to find out
	// whether we need to preempt or schedule a task other than the default at a given step.
	ExecutionSummary m_replaySummary;
	// The last context switch - the official backtracking point for this execution.  The point after which we
	// switch from replay to record mode.
	ContextSwitch* m_endSwitch;
	bool m_replaySuccessful;
	// To keep track of marked methods
	TaskVector<size_t> m_priority;
	BitVector m_prioritizedSteps;
};