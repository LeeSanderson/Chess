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

#include "DBStrategy.h"
#include "ChessImpl.h"
#include "ChessExecution.h"
#include "windows.h"
#include "SyncVar.h"
#include <map>
#include <hash_set>

ChessExecution* DBStrategy::InitialExecution(){
	return ChessImpl::EmptyExecution();
}

void DBStrategy::CompletedExecution(ChessExecution* curr){

}

ChessExecution* DBStrategy::NextExecution(ChessExecution *exec, IQueryEnabled *qEnabled, size_t depthBound) {
	size_t bstep = exec->NumTransitions();
	size_t eventIndex = exec->NumEvents() - 1;
	TaskVector<size_t> preemptionDisableCount = exec->GetPreemptionDisableCount();
	size_t numDelays = delays.size();

	assert (numDelays <= delayBound);

	if(ChessImpl::vbpruner != 0)
		assert(!ChessImpl::PrunetheSchedule(exec,qEnabled));

	while (bstep > 0){
		bool ret;
		
		while (numDelays > 0 && delays[numDelays-1] == bstep)
			numDelays--;
		delays.resize(numDelays);
		
		bstep--;

		if (numDelays == delayBound)
			continue;

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
		if (op == SVOP::TASK_FORK || op == SVOP::TASK_END 
			|| op == SVOP::TASK_RESUME || op == SVOP::TASK_FENCE)
			continue; // do not schedule before these operations

		SyncVar v = exec->Transition(bstep).var;
		Task curr = exec->Transition(bstep).tid;

		if (op == SVOP::CHOICE) {
			assert (v >= 0);
			if (v == 0)
				continue;
			ret = exec->Backtrack(bstep, curr);
			assert(ret);
			return exec;
		}

		if (bstep > 0 
			&& exec->Transition(bstep-1).tid == curr 
			&& !exec->TaskTimedOutAtStep(bstep, curr)
			&& preemptionDisableCount[curr] > 0)
			continue;

		Task next;
		ret = qEnabled->NextEnabledAtStep(bstep, curr, next);
		assert(ret);
		if (curr == next) continue;

		ChessExecution* copy = new ChessExecution(exec);
		ret = copy->Backtrack(bstep, next);
		assert(ret);

		if(ChessImpl::vbpruner != 0 && ChessImpl::PrunetheSchedule(copy,qEnabled)){
			delete copy;
			continue;
		} else {
			delete exec;
		}

		delays.push_back(bstep);
		numDelays++;
		return copy;
	}

	return 0;
}
