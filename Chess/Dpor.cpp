/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Dpor.h"
#include "ChessImpl.h"
#include "EnabledSet.h"
#include "SyncVarOp.h"

Dpor Dpor::m_instance;

size_t Dpor::GetStartStep() const {
	return m_minStep;
}

// I tried to make this pretty closely match what is described in Godefroid and Cormac's POPL paper.  I
// used the stack traversal optimizations and unlike what they describe in their paper I differentiated
// reads and writes.  I'm very tentative about my use of happens-before here, and to some extent about
// the whole algorithm so it could use some double checking. -Katie
void Dpor::CompletedExecution(ChessExecution* exec, size_t depthBound) {
	m_minStep = exec->NumTransitions();
	m_svm = ChessImpl::GetSyncVarManager();
	m_exec = exec;
	m_enabled = static_cast<EnabledSet*>(exec->GetQueryEnabled());
	m_backtrackingPoints.clear();
	m_depthBound = depthBound;
	m_bounded = ChessImpl::GetOptions().bounded;
	const size_t numThreads = ChessImpl::NumThreads();
	const size_t numTrans = exec->NumTransitions();
	m_backtrackingPoints.resize(numTrans);
	m_attributes.clear();
	m_attributes.resize(numTrans);
	m_lastNonPreemptionStep.clear();
	const size_t oldBstep = exec->GetRecordIndex();
	for (size_t i = 0; i < numTrans; i++) {
		const ChessTransition trans = exec->Transition(i);
		if (m_bounded) {
			for (size_t tid = 1; tid < numThreads; tid++) {
				if (trans.tid != tid && m_enabled->IsEnabledAtStep(i, tid) && !exec->RequiresPreemption(i, tid)) {
					m_lastNonPreemptionStep[tid] = i;
				}
			}
		}
		// We don't need to bother actually searching for backtracking points until we get beyond this
		// execution's backtracking point into its record mode transitions - the replay mode ones were
		// already found during a previous execution.
		if (i < oldBstep) {
			PushTransition(i);
			continue;
		}
		// handle choose operations that still have more choices
		if (i < depthBound && trans.op == SVOP::CHOICE && trans.var > 0) {
			AddBacktrackingPoint(i, trans.tid, false /* no matching acquire */);
		}
		// Iterate through every process in the system
		for (size_t tid = 1; tid < numThreads; tid++) {
			ChessTransition lookahead;
			// Find the next task to be performed by tid.
			if (!exec->GetLookaheadAtStep(i, tid, lookahead)) {
				continue;
			}
			assert (lookahead.tid == tid);
			ProcessTransition(i, lookahead);
		}
		PushTransition(i);
	}
	m_mostRecentWrite.clear();
	m_mostRecentAccess.clear();
}

void Dpor::ProcessTransition(size_t step, ChessTransition& trans) {
	const Task tid = trans.tid;
	// Find tid's timestamp prior to performing lookahead.  Not quite sure if I did this
	// correctly, it's hard to keep track of which timestamp I've got.  Someone should double
	// check this.  -Katie
	// Dealing with timestamps being equal to zero for an access that neither reads nor
	// writes is veeery annoying.  It would be nice if there were a way to clean that up.  Also,
	// be careful about var 0, which is used for things like memory barriers that don't actually
	// involve any communication between threads.  Those aren't *actually* happens before.  -Katie
	Timestamp pts;
	size_t index;
	if (m_exec->PrevHbTransitionOfTask(step, tid, index)) {
		pts = ChessImpl::GetRaceMonitor()->get_hbstamp(EventId(tid, m_exec->GetPerThreadStepNum(index)));
	}
	// find the step of the most recent conflicting access to var
	const size_t bstep = 0;
	const SyncVar var = trans.var;
	const SyncVarOp op = trans.op;
	if (SVOP::IsRead(op) || SVOP::IsWrite(op)) {
		if (m_svm->IsAggregate(var)) {
			const size_t size = m_svm->GetAggregateVectorSize(var);
			const SyncVar* aggVarVec = m_svm->GetAggregateVector(var);
			for (size_t i = 0; i < size; i++) {
				FindConflict(aggVarVec[i], op, tid, pts);
			}
		} else {
			FindConflict(var, op, tid, pts);
		}
	} else if (ChessImpl::GetOptions().fair_por && op == SVOP::TASK_YIELD) {
		for (size_t i = 1; i < ChessImpl::NumThreads(); i++) {
			if (i != tid && m_enabled->IsEnabledAtStep(step, i)) {
				AddBacktrackingPoint(step, i, false);
			}
		}
	}
}

void Dpor::FindConflict(SyncVar var, SyncVarOp op, Task tid, Timestamp& pts) {
	size_t conflictStep = 0;
	if (SVOP::IsWrite(op)) {
		const size_t numAccesses = m_mostRecentAccess[var].size();
		for (size_t j = 1; j < numAccesses; j++) {
			if (j != tid) {
				conflictStep = std::max(conflictStep, m_mostRecentAccess[var][j]);
			}
		}
	} else if (SVOP::IsRead(op)) {
		conflictStep = m_mostRecentWrite[var];
	}
	// if the most recent write was by this thread, then we don't need to handle it again, it was already handled
	// when we analyzed that access.
	if (conflictStep > 0 && m_exec->Transition(conflictStep).tid != tid) {
		HandleConflict(conflictStep, tid, var, pts, op);
	}
}
void Dpor::HandleConflict(size_t bstep, size_t tid, SyncVar var, Timestamp& pts, SyncVarOp lookaheadOp) {
	const ChessTransition btrans = m_exec->Transition(bstep);
	const SyncVarOp bop = btrans.op;
	bool hasMatchingAcq = false;
	if (bop == SVOP::LOCK_RELEASE && (lookaheadOp == SVOP::LOCK_ACQUIRE || lookaheadOp == SVOP::LOCK_TRYACQUIRE)) {
		// if we're moving an acquire before a release, then hunt down the matching acquire and schedule before that,
		// as well.
		size_t acqStep = GetMatchingAcquireStep(bstep);
		if (acqStep > 0 && m_exec->Transition(acqStep).tid != tid) {
			assert (m_exec->Transition(acqStep).op == SVOP::LOCK_ACQUIRE || m_exec->Transition(acqStep).op == SVOP::LOCK_TRYACQUIRE);
			hasMatchingAcq = true;
			HandleConflict(acqStep, tid, var, pts, lookaheadOp);
		}
	}
	
	const Task btid = btrans.tid;
	assert (tid != btid);
	// can't check against the depth bound until after we handle lock acq/rel, because even though bstep may
	// be beyond the depth bound, it's possible the matching lock acquire is not.
	if (bstep > m_depthBound) {
		return;
	}
	// No backtracking points necessary if something subsequent to bstep already has
	// happens before with next prior to executing lookahead
	if (m_exec->GetPerThreadStepNum(bstep) <= ChessImpl::GetRaceMonitor()->get_hbstamp_element(pts, btid)) {
		return;
	}
	SyncVar bvar = btrans.var;
	assert((SVOP::IsWrite(bop) || SVOP::IsRead(bop)) && bvar != 0);
	// if tid is enabled, it's sufficient to just add a backtracking point for it
	if (m_enabled->IsEnabledAtStep(bstep, tid)) {
		AddBacktrackingPoint(bstep, tid, hasMatchingAcq);
	} else {
		// tid isn't enabled.  Some other enabled process may have happens before
		// with tid though, and thus be able to enable it.  We have two choices:
		// (1) look at all enabled tasks at step and see if any of them could possibly 
		// enable tid.  If one could, schedule that.  If not, conservatively
		// schedule all tasks.  Or, (2) conservatively schedule all tasks.  For now, do 
		// (2).  Maybe try (1) later.
		size_t numThreads = ChessImpl::NumThreads();
		for (size_t next = 1; next < numThreads; next++) {
			if (next != btid && m_enabled->IsEnabledAtStep(bstep, next)) {
				assert (next != tid);
				AddConservativeBacktrackingPoint(bstep, next, m_enabled->FairBlocks(bstep, next, tid), hasMatchingAcq);
			}
		}
	}
}

void Dpor::PushTransition(size_t step) {
	const SyncVar var = m_exec->Transition(step).var;
	if (var == 0) {
		return;
	}
	const SyncVarOp op = m_exec->Transition(step).op;
	const Task tid = m_exec->Transition(step).tid;
	if (op != SVOP::TASK_FORK && op != SVOP::TASK_END && op != SVOP::TASK_RESUME) {
		if (m_svm->IsAggregate(var)) {
			const size_t size = m_svm->GetAggregateVectorSize(var);
			const SyncVar* aggVarVec = m_svm->GetAggregateVector(var);
			const bool isRead = SVOP::IsRead(op);
			const bool isWrite = SVOP::IsWrite(op);
			for (size_t i = 0; i < size; i++) {
				if (isWrite) {
					m_mostRecentAccess[aggVarVec[i]][tid] = step;
					m_mostRecentWrite[aggVarVec[i]] = step;
				} else if (isRead) {
					m_mostRecentAccess[aggVarVec[i]][tid] = step;
				}
			}
		} else {
			if (SVOP::IsWrite(op)) {
				m_mostRecentAccess[var][tid] = step;
				m_mostRecentWrite[var] = step;
			} else if (SVOP::IsRead(op)) {
				m_mostRecentAccess[var][tid] = step;
			}
		}
	}
}

size_t Dpor::GetMatchingAcquireStep(size_t relStep) {
	assert (m_exec->Transition(relStep).op == SVOP::LOCK_RELEASE);
	const SyncVar lockVar = m_exec->Transition(relStep).var;
	size_t step = relStep;
	while (step > 0) {
		step--;
		assert (step >= 0);
		const SyncVarOp op = m_exec->Transition(step).op;
		if (m_exec->Transition(step).var == lockVar && (op == SVOP::LOCK_ACQUIRE || op == SVOP::LOCK_TRYACQUIRE)) {
			return step;
		}
	}
	return step;
}