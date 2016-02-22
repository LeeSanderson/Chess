/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "SleepSets.h"
#include "HBExecution.h"

SleepSets* SleepSets::ms_instance = NULL;

SleepSets::SleepSets() : 
	m_priorityBound(NULL), 
	m_depthBound(0),
	m_prune(ChessImpl::GetOptions().do_sleep_sets) {
}

void SleepSets::Clear() {
	m_depthBound = 0;
	m_infoForState.clear();
}

bool SleepSets::InSleepSet(BestFirstExecution* exec, size_t bstep, Task next) {
	return bstep >= m_depthBound;
}

void SleepSets::CompletedExecution(BestFirstExecution* exec){
	HBExecution hbExec;
	size_t bstep = exec->GetRecordIndex();
	assert(bstep < exec->NumTransitions());
	for (size_t i = 0; i <= bstep; i++) {
		hbExec.PushTransition(exec->Transition(i));
	}
	HashValue oldHash = hbExec.GetHash();
	for(size_t i = bstep+1; i < exec->NumTransitions(); i++){
		HashValue h = hbExec.GetHash();
		if (h != oldHash || i == bstep+1) {
			std::map<HashValue, SleepSetInfo>::iterator mi = m_infoForState.find(h);
			size_t priority = (!m_priorityBound ? 0 : m_priorityBound->GetPriorityFunction()->GetPriority(exec, i, exec->Transition(i).tid));
			if(mi == m_infoForState.end()){
				// first time visiting the state
				m_infoForState[h] = SleepSetInfo(priority, IsVisitorTaskDisabled(exec, i));
			} else {
				// state hit
				size_t oldPriority = mi->second.m_priority;
				if (!m_priorityBound || (priority > oldPriority || (!m_prune && (priority == oldPriority)/* && (!m_preemptionBounded || mi->second.m_isVisitorTaskDisabled)*/))) {
					m_depthBound = i;
					return;
				}
				mi->second.m_priority = priority;
				mi->second.m_isVisitorTaskDisabled = IsVisitorTaskDisabled(exec, i);
			}
		}
		oldHash = h;
		hbExec.PushTransition(exec->Transition(i));
	}
	// Here if we didn't find a depth bound, we can't prune anything
	m_depthBound = exec->NumTransitions();
}

bool SleepSets::IsVisitorTaskDisabled(BestFirstExecution* exec, size_t step) {
	if (step == 0) {
		return true;
	}
	return !exec->GetQueryEnabled()->IsEnabledAtStep(step, exec->Transition(step-1).tid);
}