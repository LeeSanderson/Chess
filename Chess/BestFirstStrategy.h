/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "IChessStrategy.h"
#include "BestFirstFringe.h"
#include "BestFirstExecution.h"
#include "Dpor.h"
#include "SleepSets.h"

// Implements best first search strategy using the BestFirstFontier data structure to keep track
// of and prioritize the interleavings that still need to be executed.  An PriorityFunction object
// determines the particular priority metric.
class BestFirstStrategy : public IChessStrategy {
public:
	BestFirstStrategy();
	~BestFirstStrategy();

	// overrides
	ChessExecution* InitialExecution();
	ChessExecution* NextExecution(ChessExecution* exec, IQueryEnabled* qEnabled, size_t depthBound);

	// we have to clean stuff up if non-determinism has been detected
	void ReplaceExecution(ChessExecution* oldExec, ChessExecution* newExec);

private:
	void FindBacktrackingPoints(BestFirstExecution* exec, const size_t startStep, const size_t depthBound);
	void AddBacktrackingPoint(BestFirstExecution* exec, size_t step, Task tid);
	void Detatch(ContextSwitch* const cs);
	bool IsPrunableExecution(BestFirstExecution* exec, size_t bstep, Task next);
	bool IsPreemptionDisabled(BestFirstExecution* exec, size_t bstep, Task next);
	void RemoveSubtreeFromFringe(ContextSwitch* const cs);
#ifdef STATS
	void PrintStats() const;
#endif
	
	// used to calculate the priority of a backtracking point
	PriorityFunction* m_priorityFunction;
	// the set of backtracking points we still need to try
	BestFirstFringe* m_fringe;
	// the root of a tree of backtracking points.  The fringe consists of pointers to leaf nodes in the tree
	// that has this as its root node.  The root represents the very first, default execution.  All
	// children represent a backtracking point from that default execution.  Each of their children
	// represents a backtracking point from them, etc.  See ExecutionSummary.h.
	ContextSwitch* m_rootNode;
	// so we can delete it on a crash.
	ContextSwitch* m_currExecution;
	// Ways to prune the search
	SleepSets* m_sleepSets;
	PriorityBound* m_priorityBound;
	Dpor* m_dpor;
	std::set<SyncVar> m_skipVars;
	// deal with preemption disable, which has a horribly ugly interface.
	TaskVector<size_t> m_disabledCount;
	size_t m_eventIndex;
	// so we don't have to recompute all the time
	bool m_recordHbStamps;
	const bool m_doDpor; // true if we actually prune based on DPOR
	const bool m_doSleepSets; // true if we actually prune using sleep sets
	bool m_warned; // true if we've seen non-determinism and warned the user about it already

#ifdef STATS
	size_t m_nodesUpgraded;
#endif
};