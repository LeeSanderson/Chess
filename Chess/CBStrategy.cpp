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

#include "CBStrategy.h"
#include "ChessExecution.h"
//#include <sstream>

ChessExecution* CBStrategy::InitialExecution(){
	return dfs.InitialExecution();
}

class CBQueryEnabled : public IQueryEnabled{
public:
	CBQueryEnabled(CBStrategy* p, ChessExecution* e, IQueryEnabled* qEn)
		:parent(p), exec(e), qEnabled(qEn){}

	bool NextEnabledAtStep(size_t step, Task curr, Task& next){
		if(parent->IsCBRestricted(exec, step)){
			next = parent->CBRestrictedTask(exec, step);
			return true;
		}
		return qEnabled->NextEnabledAtStep(step, curr, next);
	}

	bool IsEnabledAtStep(size_t step, Task tid){
		if(parent->IsCBRestricted(exec, step)){
			return tid == parent->CBRestrictedTask(exec, step);
		}
		return qEnabled->IsEnabledAtStep(step, tid);
	}
private:
	CBStrategy* parent;
	ChessExecution* exec;
	IQueryEnabled* qEnabled;
};


void CBStrategy::CompletedExecution(ChessExecution* curr){
	UpdateCB(curr); 
}

ChessExecution* CBStrategy::NextExecution(ChessExecution *prev, IQueryEnabled *qEnabled, size_t depthBound){
	CBQueryEnabled cbQenabled(this, prev, qEnabled);
	ChessExecution* next = dfs.NextExecution(prev, &cbQenabled, depthBound);
	if(next != 0){
		numContextSwitches.resize(next->NumTransitions());
	}
	return next;
}


void CBStrategy::UpdateCB(ChessExecution* exec){
	// assuming that exec is an extension of an execution for which
	// numContextSwitches was calculated and cached
	numContextSwitches.resize(exec->GetRecordIndex());
	//numContextSwitches.clear();
	if(numContextSwitches.size() > exec->NumTransitions()){
		numContextSwitches.resize(exec->NumTransitions());
	}

	// numContextSwitches[i] depicts the number of context switches _before_ the ith
	// transition in exec
	int numCs = numContextSwitches.size() == 0 ? 0 :
		numContextSwitches[numContextSwitches.size()-1];

	for(size_t i = numContextSwitches.size(); i <= exec->NumTransitions(); i++){
		if(i >= 2){
			// (state i-2) <Trans i-2> (state i-1) <Trans i-1> (state i)
			// cb for state i increases if all of the following
				// <Trans i-2>.tid was enabled in state i-1 
				// <Trans i-2>.tid did not timeout in state i-1
				// <Trans i-2>.tid != <Trans i-1>.tid
	
			
			const ChessTransition& curr = exec->Transition(i-1);
			const ChessTransition& prev = exec->Transition(i-2);
			// did executing curr increase the context switch?
			if(prev.tid != curr.tid 
				&& exec->GetQueryEnabled()->IsEnabledAtStep(i-1, prev.tid)
				&& !exec->TaskTimedOutAtStep(i-1, prev.tid)){
			
				numCs++;
				//if(numCs > contextBound) {
				//	fprintf(stderr, "Error: numCs: %d, contextBound: %d\n", numCs, contextBound);
				//	fflush(stderr);
				//}
				//assert(numCs <= contextBound);
			}
		}
		numContextSwitches.push_back(numCs);
	}
	assert(numContextSwitches.size() == exec->NumTransitions()+1);
}

bool CBStrategy::IsCBRestricted(ChessExecution* exec, size_t step){
	if(step == 0) return false;
	const Task prev = exec->Transition(step-1).tid;
	return numContextSwitches[step] == contextBound && 
		exec->GetQueryEnabled()->IsEnabledAtStep(step, prev);
}

Task CBStrategy::CBRestrictedTask(ChessExecution* exec, size_t step){
	assert(IsCBRestricted(exec, step));
	assert(step > 0);
	return exec->Transition(step-1).tid;
}

//std::string CBStrategy::DebugState(size_t step){
//	if(numContextSwitches.size() <= step)
//		return "";
//	std::ostringstream o;
//	o << "cb=" << numContextSwitches[step];
//	o.flush();
//	return o.str();
//}
