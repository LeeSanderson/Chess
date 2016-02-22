/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

#include "SyncVar.h"
#include "ChessExecution.h"
//#include <iostream>
#include "IQueryEnabled.h"
#include "PriorityGraph.h"
#include "RepVector.h"
#include "BitVector.h"
#include "SyncVarVector.h"

// EnabledSet maintains the set of enabled tasks at every step
// in a ChessExecution. The implementation does this lazily by 
// catching up with the execution whenever necessary. Caller can
// explicitly do this by called UpdateEnabled


// This class also abstracts from the rest of Chess the logic for
// maintaining the abstract states of synchronization variables

class EnabledSet : public IQueryEnabled{
public:
	//EnabledSet exists for an execution
	EnabledSet(ChessExecution* exec);
    ~EnabledSet();
	// Reset() should be called whenever the execution is reset
	void Reset(const std::set<Task>& initTasks);

	// return the next enabled task > curr, where tasks are ordered
	// with a wraparound: curr < curr+1 < ... last < first < first+1 < curr-1
	// bool NextEnabled(Task curr, Task& next);	
	virtual bool NextEnabledAtStep(size_t step, Task curr, Task& next);

	virtual bool IsEnabledAtStep(size_t step, Task tid);
	virtual bool IsEnabledAtStepWithNoFairness(size_t step, Task tid);
	
	bool IsFairBlocked(size_t sid, const Task tid);

	// Returns true if blocker blocks blocked at step sid (Katie)
	bool FairBlocks(size_t sid, const Task blocker, const Task blocked);

	void Backtrack(size_t step);

	std::ostream& operator<<(std::ostream& o) const;

	void UpdateEnabled(size_t sid){
		if(updateSid > sid)
			return;
		if(updateSid == sid){
			if(updateEid >= execution->NumEvents() ||
			execution->GetEvent(updateEid).sid > sid)
			return;
		}

		CatchupWithExecution(sid);
	}

private:
	ChessExecution* execution;

//	typedef std::set<Task> SortedTaskList; 
	typedef BitVector SortedTaskList; 
	std::vector<SortedTaskList> enabledSets;

	//typedef stdext::hash_map<SyncVar, std::vector<Task> > WatchListType;
	typedef SyncVarVector<SortedTaskList> WatchListType;
	WatchListType watchList;
	TaskVector<SyncVar> waitManyTasks;
	//SyncVarVector<char> releasedVars;
	void InsertWatchList(Task tid, SyncVar varInt, SyncVarOp op);

	TaskVector<char> suspended;
//	stdext::hash_map<Task, bool> suspended;

	size_t updateSid;
	size_t updateEid;
	void CatchupWithExecution(size_t sid);
	void ProcessEvent(size_t sid, const ExecEvent& exevent);
	void ProcessTransition(size_t sid, const ChessTransition& trans);

	//// Notifies that tid is disabled in state sid
	void DisableTask(size_t sid, const ChessTransition& transition, bool localbacktrack);

	void EnableTask(size_t sid, Task tid);

	class FairnessAlgorithm {
	public:
		void Clear();
		bool IsFairBlocked(size_t sid, const Task tid, const SortedTaskList& enabled);
		// Returns true if blocker blocks blocked at step sid (Katie)
		bool FairBlocks(size_t sid, const Task blocker, const Task blocked, const SortedTaskList& enabled);
		void OnTransition(size_t sid, const ChessTransition& trans, 
			const SortedTaskList& oldEn, const SortedTaskList& newEn);

	private:
		// an edge t->u indicates that t has a lower priority than u
		RepVector<PriorityGraph > priorityGraphs;
		// an edge t->u means that t gets a lower priority than u at t's next yield
		PriorityGraph pendingGraph;
		TaskVector<int> yieldCount;
		//stdext::hash_map<Task, int> yieldCount;
	};
	FairnessAlgorithm fairness;

};

inline std::ostream& operator<<(std::ostream& o, const EnabledSet& en){
	return en.operator<<(o);
}