/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "BestFirstExecution.h"
#include "ExecutionSummary.h"
#include "ContextSwitch.h"
#include "ChessTransition.h"
#include "EnabledSet.h"
#include "ChessProfilerTimer.h"
#include "ChessImpl.h"
#include <string>

// For some reason these are declared both in Chess.cpp and in ChessExecution.cpp. It seems like
// this should be handled differently but for now I'm just going to declare them as well so that
// I can use them.
const int NDKIND_VARACCESS = 0;
const int NDKIND_LB = 1;

BestFirstExecution::BestFirstExecution(Task tasks[], int n)
	: ChessExecution(tasks, n), m_endSwitch(NULL) {
}

BestFirstExecution::BestFirstExecution(BestFirstExecution* exec) : ChessExecution(exec) {
	m_endSwitch = exec->m_endSwitch;
	m_replaySummary = exec->m_replaySummary;
	m_replaySuccessful = exec->m_replaySuccessful;
	m_priority = exec->m_priority;
	m_prioritizedSteps = exec->m_prioritizedSteps;
}

// unlike with dfs, we'll do local backtracks even in replay mode
bool BestFirstExecution::LocalBacktrack(){
	assert (topIndex > 0);
	ChessTransition top = stack[topIndex-1];
	// see if we have backtracked on the last context switch specified by replay.  If so, we chose
	// to try out a new execution that isn't even possible, because the thread is not enabled.  In that
	// case, we need to try a different execution instead, or else repeat an old execution, because the
	// one specified by the backtracking point isn't possible.
	if (m_replaySummary.ContainsContextSwitchAtStep(topIndex-1) && top.tid == m_replaySummary.GetNextTask(topIndex-1)) {
		// Setting replay successful to false means we won't backtrack from this execution.  This may lose some executions
		// thanks to fairness, because executing this task, even though all it does is block, un-fair blocks some other
		// task and enables it.  The best first search can't take advantage of that anyway, though, because it does not
		// preserve the event informing us that this task local backtracked.  So just say forget it and lose some executions
		// due to the fairness issues.
		m_replaySuccessful = false;
		if (topIndex-1 < m_endSwitch->GetStep()) {
			// non-determinism
			return false;
		}
		assert(m_endSwitch->GetStep() == topIndex-1 && m_endSwitch->GetTask() == top.tid);
		// the backtracking point we chose was blocked.
		assert(topIndex-1 >= m_endSwitch->GetStep());
		// rebuild the replay summary with the end switch's parent as the new end switch and just do the
		// repeat execution.
		m_replaySummary.Reinitialize(m_endSwitch->GetParent());
		recordIndex = m_endSwitch->GetParent()->GetStep();
	}
	topIndex--;
	stack.resize(topIndex);
	AddLookahead(top);
	events.push_back(ExecEvent(top.tid, ExecEvent::LOCALBACKTRACK, topIndex, top.var, top.op));
	return true;
}

void BestFirstExecution::Backtrack(ContextSwitch* cs) {
	m_priority.clear();
	m_prioritizedSteps.clear();
	BacktrackToInitStack();
	m_endSwitch = cs;
	recordIndex = m_endSwitch->GetStep();
	m_replaySummary.Reinitialize(cs);
	m_replaySuccessful = true;
}

void BestFirstExecution::Serialize(std::ostream& f){
	f << "// Init Tasks: <num> <tk_1> ... <tk_num> \n";
	f << initTasks.size() << '\n';
	for(std::set<Task>::iterator ti = initTasks.begin(); ti != initTasks.end(); ti++){
		f << *ti << '\n';
	}

	m_replaySummary.Serialize(f);

	f << '\n';
}

void BestFirstExecution::Deserialize(std::istream& f)
{
	// get the init transitions
	int num;
	std::string junk;
	getline(f, junk); //read the comment for InitTasks
	f >> num;
	if(!f.good() || num < 0) goto fail;
	initTasks.clear();
	for(int i=0; i<num; i++){
		Task t;
		f >> t;
		initTasks.insert(t);
	}
	// and now just grab the replay summary
	if (!m_replaySummary.Deserialize(f)) {
		goto fail;
	}
	Backtrack(m_replaySummary.GetEndSwitch());

	if(!f.good()) goto fail;

	return;

fail:
	std::cerr << "Deserialize of BestFirstExecution failed" << std::endl;
	assert(!"Deserialize of BestFirstExecution Failed");
}

// we have to store the per thread step number even in replay mode
bool BestFirstExecution::CommitSyncVarAccess(int nr) {
	// check for nondeterminism
	if (topIndex > 1 && 
		m_replaySummary.ContainsContextSwitchAtStep(topIndex-1) && 
		m_replaySummary.GetNextTask(topIndex-1) != stack[topIndex-1].tid && 
		stack[topIndex-1].op != SVOP::CHOICE) {

		m_replaySuccessful = false;
		return false;
	}
	// if we're waiting for quiesence but we still have backtracking points then there's non-determinism
	if (stack[topIndex-1].op == SVOP::QUIESCENT_WAIT && topIndex < recordIndex) {
		m_replaySuccessful = false;
		return false;
	}
	assert(perThreadStepNum.size() == stack.size() - 1);
	perThreadStepNum.push_back(nr);
	if (m_priority[stack[topIndex-1].tid] > 0) {
		m_prioritizedSteps.Set(topIndex-1, true);
	}
	return true;
}

ContextSwitch* BestFirstExecution::GetEndSwitch() const {
	return m_endSwitch;
}

const ExecutionSummary* BestFirstExecution::GetReplaySummary() const {
	return &m_replaySummary;
}

int BestFirstExecution::SyncVarAccess(ChessTransition& trans){
	// We are executing transition with trid == topIndex
	// We are currently in the state with sid == topIndex

	// Eagerly update the Enabled state
	AddLookahead(trans);
	enabled->UpdateEnabled(topIndex);

	// We only want to do something different from the normal record mode here if the replay summary
	// says that we have a preemptive context switch at this point.  If we have a preemptive context
	// switch, then we will hit this twice, the first time with a different tid from next task
	// dictated by the replay summary, and the second time with the same task as the replay summary
	// dictates.  The second part of the if ensures that the second time we hit this point, after we've 
	// already been switched to the preemptive task, we won't just return false again and get into an 
	// infinite loop.  If the context switch is non-preemptive, then we'll just act like we're in 
	// record mode so the enabled info will get updated correctly, and then NextTaskToSchedule will 
	// return the correct Task to schedule next.
	if (m_replaySummary.ContainsContextSwitchAtStep(topIndex) && 
		trans.tid != m_replaySummary.GetNextTask(topIndex) &&
		m_replaySummary.RequiresPreemption(topIndex)) {

		AddLookahead(trans);
		return REQUIRE_CONTEXT_SWITCH;
	}
	else{
		if (m_replaySummary.ContainsContextSwitchAtStep(topIndex) &&
			(IsFairBlocked(trans.tid) && trans.tid == m_replaySummary.GetNextTask(topIndex))) {
			
			m_replaySuccessful = false;
			if (topIndex < m_endSwitch->GetStep() || trans.op == SVOP::QUIESCENT_WAIT || trans.var == SyncVarManager::QuiescenceVar) {
				return REQUIRE_NONDETERMINISM_PROCESSING;
			}
			assert(m_endSwitch->GetStep() == topIndex && m_endSwitch->GetTask() == trans.tid);
			// the backtracking point we chose was blocked.
			// rebuild the replay summary with the end switch's parent as the new end switch and just 
			// do the repeat execution.
			m_replaySummary.Reinitialize(m_endSwitch->GetParent());
			recordIndex = m_endSwitch->GetParent()->GetStep();
		}
		if (IsFairBlocked(trans.tid)) {
			AddLookahead(trans);
			return REQUIRE_CONTEXT_SWITCH;
		}
	}
	// I don't think we have a way to check for nondeterminism anymore now that
	// I'm not keeping every single transition around for replay :(
	// TODO:  Think about whether it's ok to lose the ability to detect non-determinism
	//   (i.e. timeofday and random) and deal with it if it's not ok
	stack.push_back(trans);
	topIndex++;
	assert(stack.size() == topIndex);

	return SUCCESS;
}

// in replay mode, we have to do something a bit more complex than the ChessExecution to figure out
// what task to schedule next
int BestFirstExecution::NextTaskToSchedule(Task& next) {
	if (m_replaySummary.ContainsContextSwitchAtStep(topIndex)) {
		next = m_replaySummary.GetNextTask(topIndex);
		assert (topIndex > 0);
		if ((stack[topIndex-1].tid == next && stack[topIndex-1].op != SVOP::CHOICE) || !enabled->IsEnabledAtStep(topIndex, next)) {
			m_replaySuccessful = false;
			return REQUIRE_NONDETERMINISM_PROCESSING;
		}
		assert(lookahead[next].tid != 0);
		ChessTransition access = lookahead[next];
		assert(access.tid == next);
		lookahead[next] = ChessTransition();
		return SUCCESS;
	}
	// record mode.  Remaining code copied from ChessExecution.cpp and should probably be refactored so
	// it's not copied, but I wanted to avoid changing the baseline code as much as possible (Katie)
	Task curr = Task(0); // default
	if(topIndex != 0){
		curr = stack[topIndex-1].tid;
		if(enabled->IsEnabledAtStep(topIndex, curr)){
			// due to fairness and local backtrack
			// curr might get reenabled
			next = curr;
			return SUCCESS;
		}
	}
	if (!enabled->NextEnabledAtStep(topIndex, curr, next)) {
		//deadlock
		// but not if there is a task waiting for quiescence
		Task taskwfq;
		if (IsTaskWaitingForQuiescence(taskwfq)) {
			// reached quiescence
			QuiescentWakeup(taskwfq);
			next = taskwfq;
			return SUCCESS;
		}
		return FAILURE; // deadlock
	}
#if !SINGULARITY
	if (Chess::GetOptions().do_random) {
		Task iter = next;
		std::vector<Task> en;
		do {
			en.push_back(iter);
			enabled->NextEnabledAtStep(topIndex, iter, iter);
		} while (next != iter);
		next = en[rand()%en.size()];
	}
#endif
	assert (lookahead[next].tid != 0); // not a null transition
	ChessTransition access = lookahead[next];
	assert (access.tid == next);
	lookahead[next] = ChessTransition();
	return SUCCESS;
}

// We have to do quiescent wakeup even in replay mode in the case of a replayed execution using ls
void BestFirstExecution::QuiescentWakeup(Task tid) {
	events.push_back(ExecEvent(tid, ExecEvent::QUIESCENT_WAKEUP, topIndex));	
}

int BestFirstExecution::Choose(Task tid, int numChoices)
{
	ChessProfilerSentry s("ChessExecution::Choose");
	if (InReplayMode() && m_replaySummary.ContainsContextSwitchAtStep(topIndex)) {
		ChessTransition trans(tid, m_replaySummary.GetNextTask(topIndex), SVOP::CHOICE);
		stack.push_back(trans);
		topIndex++;
		return trans.var;
	} else {
		ChessTransition trans(tid, numChoices-1, SVOP::CHOICE);
		stack.push_back(trans);
		topIndex++;
		return numChoices-1;
	}
}

ChessExecution* BestFirstExecution::Clone() {
	BestFirstExecution* ret = new BestFirstExecution(this);
	if (ret->initExecState == 0) {
		delete ret;
		return 0;
	}
	if (ret->enabled == NULL) {
		delete ret->initExecState;
		delete ret;
		return 0;
	}
	return ret;
}

void BestFirstExecution::PruneExecution(size_t index) {
	ChessExecution::PruneExecution(index);
	recordIndex = m_endSwitch->GetStep();
	ContextSwitch* endSwitch = m_endSwitch;
	assert (index > 0);
	while (endSwitch->GetStep() >= index-1) {
		endSwitch = endSwitch->GetParent();
	}
	m_replaySummary.Reinitialize(endSwitch);
	recordIndex = endSwitch->GetStep();
}

void BestFirstExecution::Reset(){
	ChessExecution::Reset();
	stack.resize(topIndex);
	perThreadStepNum.resize(topIndex);
}

void BestFirstExecution::Reinitialize() {
	Backtrack(m_endSwitch);
}

void BestFirstExecution::SetEndSwitch(ContextSwitch* cs) {
	m_endSwitch = cs;
	recordIndex = cs->GetStep();
	m_replaySummary.Reinitialize(cs);
}

void BestFirstExecution::PrioritizePreemptions(Task tid) {
	m_priority[tid]++;
}

void BestFirstExecution::UnprioritizePreemptions(Task tid) {
	m_priority[tid]--;
}

bool BestFirstExecution::IsPrioritizedStep(size_t step) const {
	return m_prioritizedSteps.Contains(step);
}

// nstep is the step at which nondeterminism first occurred.  We need to know this so that
// we can throw away any DPOR backtracking points below that step, as they refer to an
// execution that we have discarded.
void BestFirstExecution::Reenable(size_t nstep) {
	m_replaySuccessful = true;
	recordIndex = nstep;
}
