/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <algorithm>
#include "BestFirstStrategy.h"
#include "StatsMonitor.h"
#include "Chess.h"
#include "ChessImpl.h"
#include "Dpor.h"
#include "DfsStrategy.h"
#include "ResultsPrinter.h"

// This Strategy performs a best first search using an PriorityFunction object and a BestFirstFringe to
// prioritize the possible executions.

BestFirstStrategy::BestFirstStrategy() 
	: m_doDpor(ChessImpl::GetOptions().do_dpor),
	  m_doSleepSets(ChessImpl::GetOptions().do_sleep_sets),
	  m_dpor(NULL),
	  m_sleepSets(NULL),
	  m_priorityBound(NULL),
	  m_warned(false)
#ifdef STATS
	, m_nodesUpgraded(0)
#endif
{
	assert(ChessImpl::GetOptions().best_first_priority);
	m_priorityFunction = PriorityFunction::CreatePriorityFunction(ChessImpl::GetOptions().best_first_priority);
	if (ChessImpl::GetOptions().bounded) {
		assert (ChessImpl::GetOptions().bounded_priority);
		PriorityFunction* priorityFunction = PriorityFunction::CreatePriorityFunction(ChessImpl::GetOptions().bounded_priority);
		if (!priorityFunction->IsMonotonic()) {
			fprintf(stderr, "WARNING: Your priority bound is not monotonic.  You are not guaranteed full coverage within the bound.\n");
		}
		m_priorityBound = new PriorityBound(priorityFunction, ChessImpl::GetOptions().preemption_bound);
		m_recordHbStamps = priorityFunction->RequiresHbTimestampRecording();
	}
	if (m_doSleepSets || m_priorityFunction->RequiresSleepSets()) {
		m_sleepSets = SleepSets::GetInstance();
		m_sleepSets->SetPriorityBound(m_priorityBound);
	}
	assert(m_priorityFunction);
	m_recordHbStamps = m_recordHbStamps || m_priorityFunction->RequiresHbTimestampRecording();
	if (m_recordHbStamps) {
		// clear our the timestamps and restart recording
		ChessImpl::GetRaceMonitor()->start_hbstamp_recording();
	}
	if (m_priorityFunction->UsesDpor() || m_doDpor) {
		m_dpor = Dpor::GetInstance();
	}
	// if we're going to handle non-determinism then we need to be able to delete specific nodes from the fringe, and
	// the hash will let us do that.  Hashing the nodes in the fringe is also useful if we want to be able to upgrade the
	// priority of a node after it has been added to the fringe, which can be useful with the DPOR cost functions.
	const BinType binType = ChessImpl::GetOptions().handle_nondeterminism ? ORDERED_HASH : STACK;
	m_fringe = new BestFirstFringe(binType);
	m_currExecution = NULL;
}

BestFirstStrategy::~BestFirstStrategy() {
	ContextSwitch* cs = m_fringe->RemoveNext();
	while (cs != NULL) {
		cs->Detatch();
		cs = m_fringe->RemoveNext();
	}
	if (m_currExecution) {
		m_currExecution->Detatch();
#ifdef STATS
		PrintStats();
#endif
	}
	delete m_fringe;
	delete m_priorityFunction;
	assert(!m_rootNode->HasSuccessors());
	delete m_rootNode;
}

ChessExecution* BestFirstStrategy::InitialExecution() {
	ChessExecution* exec = ChessImpl::EmptyExecution();
	// If we're doing DPOR, then keeping track of child nodes as well as parent nodes is useful
	// for both upgrading priorities and keeping track of which children have already been visited.
	// If we're handling nondeterminism, then keeping track of children is useful so we can delete
	// all of the invalid children that were generated for an execution that contained non-determinism.
	if (m_dpor || ChessImpl::GetOptions().handle_nondeterminism) {
		m_rootNode = new EnumerableContextSwitch();
	} else {
		m_rootNode = new QueryableContextSwitch();
	}
	if (!ChessImpl::GetOptions().load_schedule) {
		static_cast<BestFirstExecution*>(exec)->Backtrack(m_rootNode);
	}
	return exec;
}

#ifdef STATS
void BestFirstStrategy::PrintStats() const {
	printf("Created\t%d\tDestroyed\t%d\tDetatched\t%d\tUpgraded\t%d\n", ContextSwitch::NumCreated(), ContextSwitch::NumDestroyed(), ContextSwitch::NumDetatched(), m_nodesUpgraded);
}
#endif

ChessExecution* BestFirstStrategy::NextExecution(ChessExecution* exec, IQueryEnabled* qEnabled, size_t depthBound) {

	if(ChessImpl::resultsPrinter->SearchIsPruned()){
#ifdef STATS
		PrintStats();
#endif
		return NULL;
	}

	m_eventIndex = exec->NumEvents() - 1;
	m_disabledCount = exec->GetPreemptionDisableCount();
	BestFirstExecution* bfsExec = static_cast<BestFirstExecution*>(exec);
	ContextSwitch* endSwitch = bfsExec->GetEndSwitch();
	size_t endStep = exec->NumTransitions();
	if (depthBound && endStep > depthBound) {
		endStep = depthBound;
	}
	// update as necessary with the new completed execution
	m_priorityFunction->CompletedExecution(bfsExec);
	if (m_priorityBound) {
		m_priorityBound->GetPriorityFunction()->CompletedExecution(bfsExec);
	}
	if (m_sleepSets) {
		m_sleepSets->CompletedExecution(bfsExec);
	}

	// If we weren't successful in executing the backtracking point, which can happen if we were wrong
	// about its enabled information, then there's no point in backtracking from it, certainly.  With
	// DFS, there actually is, because it's possible that executing that one step that did nothing but
	// block could modify the priority graph for fairness and enable a thread.  But, since the best
	// first search doesn't preserve that local backtrack anyways, we'll lose that enabled information
	// and think it's non-determinism if we schedule that execution.  So, just prune it.
	if (bfsExec->ReplaySuccessful()) {
		// add any new executions to the fringe that we need to try as a result of this execution.
		FindBacktrackingPoints(bfsExec, exec->GetRecordIndex(), endStep);
	}

	// Detatch this endSwitch from the execution tree if we didn't find any children.  Upon calling
	// Detatch() the endSwitch is a dangling pointer (and is thus immediately set to NULL) - it has 
	// committed suicide, and any parents that had zero children after its being detatched have 
	// recursively committed suicide as well.  Fortunately, the outside world (i.e. the fringe) 
	// only has pointers to the leaf nodes.  If things ever change such that anyone other than an
	// internal node's child nodes has pointers to that internal node at a time when this method can
	// be called, then those will be dangling pointers and that is bad so be careful :)
	if (!endSwitch->HasSuccessors()) {
		endSwitch->Detatch();
	}

	// the concept of a nonlocal backtrack doesn't really make sense for best first search I don't think...
	// TODO: Check to make sure I'm right about that.

	m_currExecution = m_fringe->RemoveNext();
#ifdef STATS
	if (m_currExecution == NULL || (m_numExecs < 1000 && m_numExecs%100 == 0) || m_numExecs % 1000 == 0) {
		PrintStats();
	}
#endif
	if (m_currExecution == NULL) {
		// we're done!
		return false;
	}
	assert(!m_currExecution->HasSuccessors());
	bfsExec->Backtrack(m_currExecution);

	if (m_recordHbStamps) {
		// clear our the timestamps and restart recording
		ChessImpl::GetRaceMonitor()->start_hbstamp_recording();
	}
	return exec;
}

void BestFirstStrategy::AddBacktrackingPoint(BestFirstExecution* exec, size_t bstep, Task next) {
	assert(next != exec->Transition(bstep).tid || exec->Transition(bstep).op == SVOP::CHOICE);
	assert(exec->GetQueryEnabled()->IsEnabledAtStep(bstep, next));
	ContextSwitch* parent = static_cast<BestFirstExecution*>(exec)->GetEndSwitch();
	// We may not just add backtracking points below parent in the execution tree with dpor -
	// we may add them to its parent nodes.  Search up the tree until we find the first node
	// whose step is less than bstep.  Also, if we are not doing dpor and this execution had issues
	// with nondeterminism, then we may be adding a backtracking point further up.
	while (parent->GetStep() >= bstep) {
		parent = parent->GetParent();
	}
	ContextSwitch* cs;
	if (exec->Transition(bstep).op == SVOP::CHOICE) {
		parent->CreateSuccessor(bstep, exec->Transition(bstep).var-1, exec->RequiresPreemption(bstep, next), cs);
	} else {
		if (!parent->CreateSuccessor(bstep, next, exec->RequiresPreemption(bstep, next), cs)) {
			// duplicate, see if it is a leaf node
			if (!cs->HasSuccessors()) {
				// duplicate leaf node, see if its priority has changed
				// TODO: See if its value has changed
				return;
			} else {
				return;
			}
		}
	}
	// Figure out what the priority will be after executing this so we know where to put it in the fringe
	size_t bpriority = m_priorityFunction->GetPriority(exec, bstep, next);
	assert(parent->HasSuccessors());
	assert(!cs->HasSuccessors());
	assert(cs->GetStep() < exec->NumTransitions()-1);
	// Now, finally, actually insert the thing into the fringe.
	bool ret = m_fringe->Insert(cs, bpriority);
	assert(ret); // should be impossible to insert the same thing into the fringe twice
}

void BestFirstStrategy::ReplaceExecution(ChessExecution* oldExec, ChessExecution* newExec) {
	BestFirstExecution* bfsOld = static_cast<BestFirstExecution*>(oldExec);
	size_t bstep = bfsOld->GetRecordIndex();
	ContextSwitch* oldSwitch = bfsOld->GetEndSwitch();
	ContextSwitch* cs = oldSwitch;
	assert (bstep > 0);
	while (cs->GetStep() >= bstep) {
		assert (cs != NULL);
		cs = cs->GetParent();
	}
	// now cs should be the first parent higher than the point of non-determinism.  This run
	// now counts as the run of that backtracking point.  So, replace that context switch, which
	// is now doomed, with a new copy of the same backtracking point that does not yet have any 
	// children.
	if (!m_warned) {
		printf("WARNING: Program contains non-determinism that may be difficult to detect with best-first search.\n");
		m_warned = true;
	}
	if (cs == m_rootNode) {
		oldSwitch->Detatch();
		// special case for replacing the root node... empty everything out of the fringe and start over.
		ContextSwitch* next = m_fringe->RemoveNext();
		while (next != NULL) {
			next->Detatch();
			next = m_fringe->RemoveNext();
		}
		BestFirstExecution* bfsNew = static_cast<BestFirstExecution*>(newExec);
		bfsNew->SetEndSwitch(m_rootNode);
	} else {
		// erase all of this backtracking point's children
		RemoveSubtreeFromFringe(cs);
		cs->DetatchSubtree();
		BestFirstExecution* bfsNew = static_cast<BestFirstExecution*>(newExec);
		bfsNew->SetEndSwitch(cs);
	}
	// Our sleep sets are no longer valid!
	if (m_sleepSets) {
		m_sleepSets->Clear();
	}
}

// does nothing to protect against stack overflow if recursion depth is really large...
void BestFirstStrategy::RemoveSubtreeFromFringe(ContextSwitch* const cs) {
	if (!cs->HasSuccessors()) {
		m_fringe->Remove(cs);
	} else {
		ChildIterator itr = static_cast<EnumerableContextSwitch*>(cs)->Iterator();
		while (itr.HasNext()) {
			RemoveSubtreeFromFringe(itr.Next());
		}
	}
}

void BestFirstStrategy::FindBacktrackingPoints(BestFirstExecution* exec, size_t startStep, size_t depthBound) {
	size_t start = startStep;
	if (m_dpor) {
		m_dpor->CompletedExecution(exec, depthBound);
		start = std::min(startStep, m_dpor->GetStartStep());
	}
	IQueryEnabled* qEnabled = exec->GetQueryEnabled();
	for (size_t i = start; i < depthBound; i++) {
		Task curr = exec->Transition(i).tid;
		Task next;
		bool ret = qEnabled->NextEnabledAtStep(i, curr, next);
		assert(ret); // atleast curr is enabled at bstep
		if (exec->Transition(i).op == SVOP::CHOICE && exec->Transition(i).var > 0) {
			AddBacktrackingPoint(exec, i, curr);
		}
		while (!DfsStrategy::DoneSchedulingAllThreads(DfsStrategy::FirstScheduledTaskAtStep(i, exec), curr, next)){
			assert(i > 0);
			assert(qEnabled->IsEnabledAtStep(i, next));
			// Prune by priority bound, sleep sets, persistent sets, etc.
			if (!IsPrunableExecution(exec, i, next)) {
				AddBacktrackingPoint(exec, i, next);
			}
			qEnabled->NextEnabledAtStep(i, next, next);
		}
	}
}

bool BestFirstStrategy::IsPrunableExecution(BestFirstExecution* exec, size_t bstep, Task next) {
	assert(bstep < exec->NumTransitions());
	const SyncVarOp op = exec->Transition(bstep).op;
	if(op == SVOP::TASK_FORK || 
		op == SVOP::TASK_END || 
		op == SVOP::TASK_FENCE ||
		op == SVOP::TASK_RESUME) {
		return true; // do not schedule before these operations
	}
	if (m_doDpor && !m_dpor->Contains(bstep, next)) {
		return true;
	}
	if (m_skipVars.find(exec->Transition(bstep).var) != m_skipVars.end()) {
		return true;
	}
	if (IsPreemptionDisabled(exec, bstep, next)) {
		return true;
	}
	if (m_priorityBound && m_priorityBound->IsPrunable(exec, bstep, next)) {
		return true;
	}
	if (m_doSleepSets && m_sleepSets->InSleepSet(exec, bstep, next)) {
		return true;
	}
	return false;
}

bool BestFirstStrategy::IsPreemptionDisabled(BestFirstExecution* exec, size_t bstep, Task next) {
	while (m_eventIndex > 0) {
		ExecEvent e = exec->GetEvent(m_eventIndex);
		if (e.sid < bstep) return m_disabledCount[next] > 0;
		// undo preemption enable/disable
		if (e.eid == ExecEvent::PREEMPTION_DISABLE)
			m_disabledCount[e.tid]--;
		if (e.eid == ExecEvent::PREEMPTION_ENABLE)
			m_disabledCount[e.tid]++;
		m_eventIndex--;
	}
	return m_disabledCount[next] > 0;
}