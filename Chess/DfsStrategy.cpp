/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#if SINGULARITY

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif

#endif

#include "DfsStrategy.h"
#include "ChessExecution.h"
#include "Chess.h"
#include "ChessImpl.h"
#include "CacheRaceMonitor.h"
#include "ResultsPrinter.h"
#include "Dpor.h"
#include <algorithm>

ChessExecution* DfsStrategy::InitialExecution(){
	// start timestamp recording if we're doing DPOR (Katie)
	if (ChessImpl::GetOptions().do_dpor) {
		// clear our the timestamps and restart recording
		ChessImpl::GetRaceMonitor()->start_hbstamp_recording();
	}
	return ChessImpl::EmptyExecution();
}

bool DfsStrategy::DoneSchedulingAllThreads(Task first, Task curr, Task next){
	//Should schedule in this order: first, first+1,...0,...first-1
	// We are done if next <= curr in this order
	if(first <= curr && first <= next) return next <= curr;
	if(first > curr && first > next) return next <= curr;
	if(first <= curr && first > next) return false;
	if(first > curr && first <= next) return true;
	assert(false);
	return true;
}

Task DfsStrategy::FirstScheduledTaskAtStep(size_t step, ChessExecution* exec){
	if(step == 0)
		return Task(0);

	Task taskAtLastStep = exec->Transition(step-1).tid;
	if(exec->GetQueryEnabled()->IsEnabledAtStep(step, taskAtLastStep))
		return taskAtLastStep;

	Task next;
	bool ret = exec->GetQueryEnabled()->NextEnabledAtStep(step, taskAtLastStep, next);
	assert(ret);
	return next;
}

// added the destructive parameter so we'd know whether this is a peek or a pop so to speak, if we're just
// peeking at the backtracking point then we can't clean up the backtracking point info but if it's popping
// it then we need to ensure we don't leave stale backtracking points around (Katie)
bool DfsStrategy::FindBacktrackingPoint(ChessExecution* exec, IQueryEnabled* qEnabled, size_t startStep, size_t startTask, size_t& endStep, Task& endTask, std::set<SyncVar>& skipVars, bool destructive){
	size_t bstep = startStep;
	size_t eventIndex = exec->NumEvents() - 1;
	TaskVector<size_t> preemptionDisableCount = exec->GetPreemptionDisableCount();

	while(bstep > 0){
		//backtrack from bstep
		bstep--;

		// at the end of this loop preemptionEnableCount reflects its value in state bstep
		while (eventIndex > 0)
		{
			ExecEvent e = exec->GetEvent(eventIndex);
			if (e.sid < bstep) break;
			// undo preemption enable/disable
			if (e.eid == ExecEvent::PREEMPTION_DISABLE)
				preemptionDisableCount[e.tid]--;
			if (e.eid == ExecEvent::PREEMPTION_ENABLE)
				preemptionDisableCount[e.tid]++;
			eventIndex--;
		}
		
		SyncVarOp op = exec->Transition(bstep).op;
		if(op == SVOP::TASK_FORK || op == SVOP::TASK_END 
			|| op == SVOP::TASK_RESUME || op == SVOP::TASK_FENCE)
			continue; // do not schedule before these operations

		SyncVar v = exec->Transition(bstep).var;
		if(skipVars.find(v) != skipVars.end()){
			continue;
		}

		Task curr = exec->Transition(bstep).tid;

		if (op == SVOP::CHOICE)
		{
			assert (v >= 0);
			if (v == 0) continue;
			endStep = bstep;
			endTask = curr;
			return true;
		}

		if (bstep > 0 
			&& exec->Transition(bstep-1).tid == curr 
			&& !exec->TaskTimedOutAtStep(bstep, curr) 
			&& preemptionDisableCount[curr] > 0)
			continue;

		if(startTask != Task(0) && bstep == startStep -1){
			curr = startTask;
		}
		Task next;
		bool ret = qEnabled->NextEnabledAtStep(bstep, curr, next);
		assert(ret); // atleast curr is enabled at bstep
		
		// Check whether DPOR will prune this execution, if needed (Katie)
		if (ChessImpl::GetOptions().do_dpor) {
			while (!DoneSchedulingAllThreads(FirstScheduledTaskAtStep(bstep, exec), curr, next)) {
				if (backtrackingPoints.find(bstep) != backtrackingPoints.end() && backtrackingPoints[bstep].Contains(next)) {
					endStep = bstep;
					endTask = next;
					// clean out any backtracking points below this one if we're doing DPOR (Katie)
					if (destructive) {
						// clean up, if necessary
						backtrackingPoints[bstep].Set(next, false);
						if (backtrackingPoints[bstep].IsEmpty()) {
							backtrackingPoints.erase(bstep);
						}
						backtrackingPoints.erase(backtrackingPoints.upper_bound(bstep), backtrackingPoints.end());
					}
					return true;
				}
				curr = next;
				qEnabled->NextEnabledAtStep(bstep, curr, next);
			}
		} else {
			if(! DoneSchedulingAllThreads(FirstScheduledTaskAtStep(bstep, exec), curr, next)){
				endStep = bstep;
				endTask = next;
				return true;
			}
		}
	}
	return false;
}

class DfsDataRaceCb : public OnDataRaceCb{
public:
	virtual void OnDataRace(EventId in1, EventId in2, void* loc){
		*GetChessOutputStream() << "Race " << ((size_t)loc/4) << std::endl;
		racedVars.insert(SyncVar(((size_t)loc)/4));
	}
	std::set<SyncVar>& GetRacedVars(){return racedVars;}
private:
	std::set<SyncVar> racedVars;
};

//void GetNonracyVolatiles(ChessExecution* exec, size_t depthBound, std::set<SyncVar>& result){
//	std::set<SyncVar> nonVolatileSyncVars;
//	std::set<SyncVar> allVars;
//
//	CacheRaceMonitor crm(false, true);
//	DfsDataRaceCb drcb;
//	crm.SetMultipleRacesMode(true);
//	crm.SetOnDataRaceCb(&drcb);
//
//	int eventId=0;
//	for(size_t i=0; i<depthBound; i++){
//		const ChessTransition& t = exec->Transition(i);
//		allVars.insert(t.var);
//
//		if(!t.var.IsAggregate() && (t.op == SVOP::RWVAR_READWRITE || t.op == SVOP::RWVAR_READ || t.op == SVOP::RWVAR_WRITE)){
//			*GetChessOutputStream() << t.tid << " Data " << t.var.Id() << std::endl;
//			crm.OnDataVarAccess(EventId(t.tid, eventId++), (void*)(t.var.Id()*4), 4, t.op != SVOP::RWVAR_READ, 0);
//		}
//		else{
//			nonVolatileSyncVars.insert(t.var);
//			*GetChessOutputStream() << t.tid << " Sync " << t.var << std::endl;
//			crm.OnSyncVarAccess(EventId(t.tid, eventId++), t.tid, t.var, t.op);
//		}
//	}
//	std::set<SyncVar> nonSkipVars;
//	std::insert_iterator<std::set<SyncVar> > ii1(nonSkipVars, nonSkipVars.begin());
//	set_union(nonVolatileSyncVars.begin(), nonVolatileSyncVars.end(), drcb.GetRacedVars().begin(), drcb.GetRacedVars().end(), ii1);
//
//	std::insert_iterator<std::set<SyncVar> > ii2(result, result.begin());
//	set_difference(allVars.begin(), allVars.end(), nonSkipVars.begin(), nonSkipVars.end(), ii2);
//	*GetChessOutputStream() << " NonRacy: " << result.size() << " total: " << allVars.size() << std::endl;
//	set_difference(allVars.begin(), allVars.end(), nonSkipVars.begin(), nonSkipVars.end(), std::ostream_iterator<SyncVar>(*GetChessOutputStream(), " "));
//	*GetChessOutputStream() << std::endl;
//}

ChessExecution* DfsStrategy::NextExecution(ChessExecution* exec, IQueryEnabled* qEnabled, size_t depthBound){
	

	if(ChessImpl::resultsPrinter->SearchIsPruned()){
		lastBacktrackSid = -1;
		return 0;
	}
	
	size_t bstep = exec->NumTransitions();
	
	// check for depth_bound
	if(depthBound && bstep > depthBound){
		bstep = depthBound;
	}

	// record whether this execution was successful, and only find new DPOR backtracking points if it was (Katie)
	bool replaySuccessful = true;

	// check for nonlocalbacktrack
	if(lastBacktrackSid != exec->GetRecordIndex()){
		// not doing DFS - probably due to nondeterminism
		lastBacktrackSid = -1;
		if (ChessImpl::GetStats()->GetNumExecutions() > 1) {
			replaySuccessful = false;
		}
	}

	Task startTask = Task(0);
	if (lastBacktrackSid != -1) {
		// See if we successfully executed the backtracking point we meant to, or if we local backtracked on it (Katie)
		for (size_t i = 0; i < exec->NumEvents(); i++) {
			if (exec->GetEvent(i).sid > lastBacktrackSid) {
				break;
			}
			if (exec->GetEvent(i).sid == lastBacktrackSid && exec->GetEvent(i).tid == lastBacktrackTask && exec->GetEvent(i).eid == ExecEvent::LOCALBACKTRACK) {
				// we local backtracked on the new transition we tried, so we need to skip over that one and try another (Katie)
				replaySuccessful = false;
				ChessImpl::GetStats()->OnNonlocalBacktrack(lastBacktrackOp);
				bstep = lastBacktrackSid+1;
				startTask = lastBacktrackTask;
			}
		}
	}

	std::set<SyncVar> nonracyVolatiles;
	//GetNonracyVolatiles(exec, bstep, nonracyVolatiles);

	// Update the DPOR information, if needed (Katie)
	if (ChessImpl::GetOptions().do_dpor && replaySuccessful) {
		Dpor::GetInstance()->CompletedExecution(exec, bstep);
		size_t numThreads = ChessImpl::NumThreads();
		for (size_t i = Dpor::GetInstance()->GetStartStep(); i < bstep; i++) {
			for (size_t j = 1; j < numThreads; j++) {
				if (Dpor::GetInstance()->Contains(i, j)) {
					backtrackingPoints[i].Set(j, true);
				}
			}
		}
	}

	Task next;
	if(FindBacktrackingPoint(exec, qEnabled, bstep, startTask, bstep, next, nonracyVolatiles, true /* destructive */)){
		// first get a recovery point
		exec->ClearRecoveryPoint();
		size_t rstep = bstep+1;
		Task rnext;
		if(FindBacktrackingPoint(exec, qEnabled, rstep, next, rstep, rnext, nonracyVolatiles, false /* not destructive */)){
			exec->SetRecoveryPoint(rstep, rnext);
		}

		// now do the backtrack
		// transitions t0, t1, ... t{bstep-1} stay on the stack
		//   t{bstep}.tid should be next
		if(exec->Backtrack(bstep, next)){
			lastBacktrackSid = exec->GetRecordIndex();
			lastBacktrackTask = next;
			ChessTransition trans;
			lastBacktrackOp = 0;
			if(exec->GetLookaheadAtStep(bstep, next, trans))
				lastBacktrackOp = trans.op;
			// restart the timestamp recording if we're doing DPOR (Katie)
			if (ChessImpl::GetOptions().do_dpor) {
				// clear our the timestamps and restart recording
				ChessImpl::GetRaceMonitor()->start_hbstamp_recording();
			}
			return exec;
		}
		// else fallthrough to failure
	}

	// clear our the timestamps and restart recording if we're doing DPOR (Katie)
	if (ChessImpl::GetOptions().do_dpor) {
		ChessImpl::GetRaceMonitor()->start_hbstamp_recording();
	}

	lastBacktrackSid = -1;
	return 0;
}

