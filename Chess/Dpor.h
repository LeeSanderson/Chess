/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessImpl.h"
#include "ChessExecution.h"
#include "CacheRaceMonitor.h"
#include <map>

#define NOT_CONSERVATIVE 1
#define NO_MATCHING_ACQUIRE 2
#define FAIR_BLOCKS 4

class Dpor {
public:
	static Dpor* GetInstance() { return &m_instance; }
	void CompletedExecution(ChessExecution* exec, size_t depthBound);
	const BitVector& GetBacktrackingPoints(size_t step) { return m_backtrackingPoints[step]; }
	bool Contains(size_t step, Task tid) { return m_backtrackingPoints[step].Contains(tid); }
	bool NotConservative(size_t step, Task tid) { return (m_attributes[step][tid] & NOT_CONSERVATIVE) > 0; }
	bool NoMatchingAcquire(size_t step, Task tid) { return (m_attributes[step][tid] & NO_MATCHING_ACQUIRE) > 0; }
	bool FairBlocks(size_t step, Task tid) { return (m_attributes[step][tid] & FAIR_BLOCKS) > 0; }
	size_t GetStartStep() const;

private:
	Dpor() {}
	void HandleConflict(size_t bstep, size_t tid, SyncVar var, Timestamp& pts, SyncVarOp lookaheadOp);
	void FindConflict(SyncVar var, SyncVarOp op, Task tid, Timestamp& pts);
	size_t GetMatchingAcquireStep(size_t relStep);
	void ProcessTransition(size_t step, ChessTransition& trans);
	void PushTransition(size_t step);
	void AddBacktrackingPoint(size_t bstep, Task tid, bool hasMatchingAcq) {
		if (!hasMatchingAcq) {
			m_attributes[bstep][tid] = (NO_MATCHING_ACQUIRE | NOT_CONSERVATIVE);
		} else {
			m_attributes[bstep][tid] = NOT_CONSERVATIVE;
		}
		if (m_bounded && m_exec->RequiresPreemption(bstep, tid)) {
			AddBacktrackingPoint(m_lastNonPreemptionStep[tid], tid, hasMatchingAcq);
		}
		m_backtrackingPoints[bstep].Set(tid, true);
		if (bstep < m_minStep) {
			m_minStep = bstep;
		}
	}
	void AddConservativeBacktrackingPoint(size_t bstep, Task tid, bool fairBlocks, bool hasMatchingAcq) {
		size_t att = m_backtrackingPoints[bstep].Get(tid) ? m_attributes[bstep][tid] : 0;
		if (fairBlocks) {
			att |= FAIR_BLOCKS;
		}
		if (!hasMatchingAcq) {
			att |= NO_MATCHING_ACQUIRE;
		}
		m_attributes[bstep][tid] = att;
		if (m_bounded && m_exec->RequiresPreemption(bstep, tid)) {
			AddConservativeBacktrackingPoint(m_lastNonPreemptionStep[tid], tid, fairBlocks, hasMatchingAcq);
		}
		m_backtrackingPoints[bstep].Set(tid, true);
		if (bstep < m_minStep) {
			m_minStep = bstep;
		}
	}

	static Dpor m_instance;
	std::vector<BitVector> m_backtrackingPoints;
	std::vector<TaskVector<size_t>> m_attributes;
	SyncVarManager* m_svm;
	EnabledSet* m_enabled;
	// map varids to the most recent index at which they were read/written
	SyncVarVector<size_t> m_mostRecentWrite;
	SyncVarVector<TaskVector<size_t>> m_mostRecentAccess;
	size_t m_depthBound;
	const ChessExecution* m_exec;
	size_t m_minStep;
	bool m_bounded;
	TaskVector<size_t> m_lastNonPreemptionStep;
};