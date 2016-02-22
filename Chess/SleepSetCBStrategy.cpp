/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "SleepSetCBStrategy.h"
#include "HBExecution.h"


//class SleepSetCBQueryEnabled : public IQueryEnabled{
//public:
//	SleepSetCBQueryEnabled(SleepSetCBStrategy* p, ChessExecution* e, IQueryEnabled* qEn)
//		:parent(p), exec(e), qEnabled(qEn){}
//
//	bool NextEnabledAtStep(size_t step, Task curr, Task& next){
//		if(parent->IsCBRestricted(exec, step)){
//			next = parent->CBRestrictedTask(exec, step);
//			return true;
//		}
//		return qEnabled->NextEnabledAtStep(step, curr, next);
//	}
//
//	bool IsEnabledAtStep(size_t step, Task tid){
//		if(parent->IsCBRestricted(exec, step)){
//			return tid == parent->CBRestrictedTask(exec, step);
//		}
//		return qEnabled->IsEnabledAtStep(step, tid);
//	}
//private:
//	SleepSetCBStrategy* parent;
//	ChessExecution* exec;
//	IQueryEnabled* qEnabled;
//};


ChessExecution* SleepSetCBStrategy::InitialExecution(){
	return cbStrategy.InitialExecution();
}

void SleepSetCBStrategy::CompletedExecution(ChessExecution* curr){
	execCnt ++;
	cbStrategy.CompletedExecution(curr);
	UpdateSleepSets(curr);
}

void SleepSetCBStrategy::UpdateSleepSets(ChessExecution* curr){
	HBExecution hbExec;
	if(startStateAtCompletedExecution > curr->GetRecordIndex()+1){
		// Non DFS - may be due to nondeterminism
		startStateAtCompletedExecution = curr->GetRecordIndex()+1;
	}

	assert(startStateAtCompletedExecution <= curr->NumTransitions());
	for(size_t i=0; i<startStateAtCompletedExecution; i++){
		hbExec.PushTransition(curr->Transition(i));
	}
	for(size_t i=startStateAtCompletedExecution; i<curr->NumTransitions(); i++){
		hbExec.PushTransition(curr->Transition(i));
		// now we are in sid i+1, 
		HashValue h = hbExec.GetHash();
		int cb = cbStrategy.GetContextBoundAtStep(i+1);
		//stdext::hash_map<HashValue, int>::iterator mi = minCBForState.find(h);
		std::map<HashValue, SleepSetInfo>::iterator mi = minCBForState.find(h);
		if(mi == minCBForState.end()){
			// first time visiting the state
			SleepSetInfo sinfo;
			sinfo.cb = cb;
			sinfo.isVisitorThreadDisabled = !curr->GetQueryEnabled()->IsEnabledAtStep(i+1, curr->Transition(i).tid);
#ifdef DEBUG_SLEEPSETS
			sinfo.depth = i;
			sinfo.execCnt = execCnt;
			sinfo.stack = curr->Stack();
#endif
			minCBForState[h] = sinfo;
		}
		else{
			// state hit

			if(mi->second.cb < cb /*||   need to comment this out for PreFast
				(false && mi->second.cb == cb && mi->second.isVisitorThreadDisabled) */){
					// if the current trace has a strictly higher cb then no point revisiting the state
					// However, if the current trace has the same cb as before, we have to prune only 
					// if any one of the previous visitors was disabled in this state. Otherwise, threads
					// would have been explored from that state only at the cost of a preemption - madan
	
					// Prune the execution after transition i
					depthBoundAtNextExecution = i+1;
#ifdef DEBUG_SLEEPSETS
					*GetChessErrorStream() << execCnt << " DepthBoundAtNext " << i+1 << " " << cb << " " << mi->second.cb << " " << mi->second.depth << " " << mi->second.execCnt << std::endl;
#endif
					break;
			}
			mi->second.cb = cb;
			mi->second.isVisitorThreadDisabled = !curr->GetQueryEnabled()->IsEnabledAtStep(i+1, curr->Transition(i).tid);
#ifdef DEBUG_SLEEPSETS
			mi->second.depth = i;
			mi->second.execCnt = execCnt;
#endif
		}
	}
}


ChessExecution* SleepSetCBStrategy::NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound){
//	SleepSetCBQueryEnabled ssqen(this, prev, qEnabled);

	if(depthBoundAtNextExecution && (depthBound == 0|| depthBound > depthBoundAtNextExecution)){
		depthBound = depthBoundAtNextExecution;
	}
	depthBoundAtNextExecution = 0;
	ChessExecution* ret = cbStrategy.NextExecution(prev, qEnabled, depthBound);
	if(ret){
		size_t t = ret->NumTransitions();
		assert(t > 0);
		// transitions go from 0..t-1
		// states go from 0...t
		// TODO: Madan said there was a reason why this started at t rather than t-1 and
		// was thus conservative by one step with sleep sets.  Maybe it has to do with
		// fairness?  I have no idea.  But someone should recall why, if that's the case,
		// and change it back if necessary (Katie)
		startStateAtCompletedExecution = t-1;
	}
	return ret;
}

