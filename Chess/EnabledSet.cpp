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

//#define NO_FAIRNESS

#include "ChessImpl.h"
#include "SyncVarManager.h"
#include "EnabledSet.h"
#include <algorithm>
#include "ChessLog.h"

EnabledSet::EnabledSet(ChessExecution* exec){
	execution = exec;
	updateSid = 0;
	updateEid = 0;
}

EnabledSet::~EnabledSet(){
	execution = 0;
	enabledSets.clear();
	suspended.clear();
	watchList.clear();
	waitManyTasks.clear();
	//releasedVars.clear();
	fairness.Clear();
}


void EnabledSet::Reset(const std::set<Task>& initTasks){
	enabledSets.clear();
	suspended.clear();
	watchList.clear();
	waitManyTasks.clear();
	//releasedVars.clear();
	fairness.Clear();

	updateSid = 0;
	updateEid = 0;

	enabledSets.push_back(SortedTaskList());
	std::vector<SortedTaskList>::iterator enabled = (--enabledSets.end());
	std::set<Task>::const_iterator i;
	for(i = initTasks.begin(); i!= initTasks.end(); i++){
//		enabled->insert(enabled->end(), *i);
		enabled->insert(*i);
	}
}

const int NOT_SUSPENDED = 0;
const int SUSPENDED_AND_ENABLED = 1;
const int SUSPENDED_AND_DISABLED = 2;

void EnabledSet::DisableTask(size_t sid, const ChessTransition& transition, bool localbacktrack){
	//SortedTaskList::iterator i = enabledSets[sid].find(transition.tid);
	//if(i == enabledSets[sid].end()){
	//	return; // this can happen because of nondeterminism
	//}
	if(!enabledSets[sid].Contains(transition.tid)){
		return; // this can happen because of nondeterminism
	}
//	assert(suspended.find(transition.tid) == suspended.end());
	assert(suspended[transition.tid] == NOT_SUSPENDED);
	CHESS_LOG(ENABLED_SET, "Disable", transition.tid, sid);
    EventCounter &ec(*ChessImpl::GetEventCounter());
	if (! localbacktrack) // only create DISABLE attribute for non-local-backtrack causes
		ChessImpl::SetEventAttribute(ec.getEventId(sid-1), DISABLE, transition.tid);
//	enabledSets[sid].erase(i);
	enabledSets[sid].erase(transition.tid);
	
	InsertWatchList(transition.tid, transition.var, transition.op);
}

void EnabledSet::EnableTask(size_t sid, Task tid){
	SortedTaskList& enabled = enabledSets[sid];

//	if(suspended.find(tid) == suspended.end()){
	if(suspended[tid] == NOT_SUSPENDED){
		CHESS_LOG(ENABLED_SET, "Enable", tid, sid-1);
		if (sid > 0) // we're not sending the enabling of the initial thread
			ChessImpl::SetEventAttribute(ChessImpl::GetEventCounter()->getEventId(sid-1), ENABLE, tid);
		enabled.insert(tid);
	}
	else{
//		assert(suspended[tid] == false);
		assert(suspended[tid] == SUSPENDED_AND_DISABLED);
		suspended[tid] = SUSPENDED_AND_ENABLED; // enabled when later resumed
//		suspended[tid] = true; // enabled when later resumed
	}
}

void EnabledSet::InsertWatchList(Task tid, SyncVar var, SyncVarOp op)
{
	CHESS_LOG(ENABLED_SET, "InsertWatchList", tid, var, op);
	//releasedVars[var] = false;
	//if(releasedVars.find(var) != releasedVars.end()){
	//	releasedVars.erase(var);
	//}
	if(!ChessImpl::GetSyncVarManager()->IsAggregate(var)){
		//watchList[var].push_back(tid);
		watchList[var].insert(tid);
	}
	else{
		const SyncVar* varvec = ChessImpl::GetSyncVarManager()->GetAggregateVector(var);
		int n = ChessImpl::GetSyncVarManager()->GetAggregateVectorSize(var);
		if(op == SVOP::WAIT_ALL || op == SVOP::WAIT_ANY){
			waitManyTasks[tid] = var;
		}
		for(int i=0; i<n; i++){
			//watchList[varvec[i]].push_back(tid);
			watchList[varvec[i]].insert(tid);
		}
	}
}

bool EnabledSet::IsEnabledAtStep(size_t step, Task tid){
	if(step > execution->NumTransitions()){
		assert(step > execution->NumTransitions());
		return false;
	}
	UpdateEnabled(step);
	const SortedTaskList* enabled = &(enabledSets[step]);
//	return enabled->find(tid) != enabled->end() && !IsFairBlocked(step, tid);
	return enabled->Contains(tid) && !IsFairBlocked(step, tid);
}

bool EnabledSet::IsEnabledAtStepWithNoFairness(size_t step, Task tid){
	if(step > execution->NumTransitions()){
		assert(step > execution->NumTransitions());
		return false;
	}
	UpdateEnabled(step);
	const SortedTaskList* enabled = &(enabledSets[step]);
	return enabled->Contains(tid);
}

bool EnabledSet::NextEnabledAtStep(size_t step, Task curr, Task& next){
	if(step > execution->NumTransitions()){
		assert(step > execution->NumTransitions());
		return false;
	}

	UpdateEnabled(step);
	const SortedTaskList* enabled = &(enabledSets[step]);
		
	//if(enabled->size() == 0)
	//	return false; //deadlock
	if(enabled->IsEmpty())
		return false; //deadlock

	// find the next task > curr in enabled.
	// if no such task exists, then find the next task >= 0
	Task candidate = enabled->FindIndexLargerThan(curr);
	Task firstCandidate = candidate;
	while(IsFairBlocked(step, candidate)){
		candidate = enabled->FindIndexLargerThan(candidate);
		if(candidate == firstCandidate){
			assert(!"Priority Graph contains a cycle");
		}
	}
	next = candidate;

	//SortedTaskList::iterator start = enabled->upper_bound(curr);
	//SortedTaskList::iterator n = start;
	//while(true){
	//	if(n == enabled->end()){
	//		n = enabled->begin();
	//	}
	//	if(!IsFairBlocked(step, *n))
	//		break;

	//	n = enabled->upper_bound(*n);
	//	if(n == start){
	//		assert(!"Priority Graph contains a cycle");
	//	}
	//}
	//next = *n;

	return true;
}

bool EnabledSet::IsFairBlocked(size_t sid, const Task tid){
#ifdef NO_FAIRNESS
	return false;
#endif
	UpdateEnabled(sid);
	return fairness.IsFairBlocked(sid, tid, enabledSets[sid]);
}

// Return true if blocker blocks blocked at step sid (Katie)
bool EnabledSet::FairBlocks(size_t sid, const Task blocker, const Task blocked) {
#ifdef NO_FAIRNESS
	return false;
#endif
	UpdateEnabled(sid);
	return fairness.FairBlocks(sid, blocker, blocked, enabledSets[sid]);
}

// Update enabled for states [updateSid, ..., end] 
void EnabledSet::CatchupWithExecution(size_t endSid){
	assert(endSid <= execution->NumTransitions());
	assert(enabledSets.size()-1 == updateSid);

	while(true){
		// catchup with events at updateSid
		while(updateEid < execution->NumEvents() && execution->GetEvent(updateEid).sid <= updateSid){
			ProcessEvent(updateSid, execution->GetEvent(updateEid));
			updateEid ++;
		}
		if(updateSid == endSid)
			break;

		assert(updateSid < endSid);
		size_t trid = updateSid;
		// transition trid takes us from state updateSid to updateSid+1;
		assert(trid < execution->NumTransitions());
		updateSid++;
		SortedTaskList s;
		enabledSets[enabledSets.size()-1].Copy(s);
		enabledSets.push_back(s);
		ProcessTransition(updateSid, execution->Transition(trid));
	}
}

void EnabledSet::ProcessEvent(size_t sid, const ExecEvent& exevent){
	CHESS_LOG(ENABLED_SET, "EnabledSet::ProcessEvent", sid, exevent.eid, exevent.op, exevent.var);
	assert(sid < enabledSets.size());
	switch(exevent.eid){
		case ExecEvent::LOCALBACKTRACK : 
			{
				size_t esid = exevent.sid;
				Task etid = exevent.tid;
				ChessTransition trans;
				bool ret = execution->GetLookaheadAtStep(esid, etid, trans);
				CHESS_LOG(ENABLED_SET, "EnabledSet::ProcessEvent::LocalBacktrack", esid, trans);
				assert(ret);
				if(ret){
					// due to nondeterminism tid might be performing a different transition
					// than the one it backtracked on previously. So do this check
					// and only DisableTask when there is no nondeterminism
					// However, this check can also fail if the varId assignments are different
					//  currently, this just means we are inefficient - madan

					if(trans.op == exevent.op && trans.var == exevent.var){
						DisableTask(esid, trans, true);
					}
				}
				break;
			}
		case ExecEvent::QUIESCENT_WAKEUP :
			{
				size_t esid = exevent.sid;
				Task etid = exevent.tid;
				EnableTask(esid, etid);
				break;
			}
		case ExecEvent::PREEMPTION_DISABLE :
		case ExecEvent::PREEMPTION_ENABLE :
		case ExecEvent::TIMEOUT :
			{ 
				break;
			}
		default:
			assert(!"Invalid ExecEvent");
	}
}

void EnabledSet::ProcessTransition(size_t sid, const ChessTransition& trans){
	CHESS_LOG(ENABLED_SET, "EnabledSet::ProcessTransition", sid, trans);
	assert(sid > 0);
	assert(sid == enabledSets.size()-1);
	SortedTaskList& enabled = enabledSets[sid];
//	assert(enabledSets[sid-1].find(trans.tid) != enabledSets[sid-1].end());
	//assert(enabledSets[sid-1].Contains(trans.tid));	
	if(!enabledSets[sid-1].Contains(trans.tid)){
		// Due to nondeterminism, trans.tid need not be "enabled" even though it is currently executing the trans!
		enabledSets[sid-1].insert(trans.tid);
		EnableTask(sid, trans.tid);
	}
	size_t trid = sid-1; // the transition that we are processing

	switch(trans.op){
		case SVOP::TASK_FORK:
			{
				Task tid = trans.var/*.ToTask()*/;
				//assert(enabled.find(tid) == enabled.end());
				assert(!enabled.Contains(tid));
				suspended[tid] = SUSPENDED_AND_ENABLED; //insert into enabled when resumed
//				suspended[tid] = true; //insert into enabled when resumed
				break;
			}
		case SVOP::TASK_RESUME :
			{
				Task tid = trans.var/*.ToTask()*/;
//				assert(suspended.find(tid) != suspended.end());
				assert(suspended[tid] != NOT_SUSPENDED);
				if(suspended[tid] == SUSPENDED_AND_ENABLED){
//				if(suspended[tid]){
					CHESS_LOG(ENABLED_SET, "Enable", tid, trid);
		            ChessImpl::SetEventAttribute(ChessImpl::GetEventCounter()->getEventId(trid), ENABLE, tid);
					enabled.insert(tid);
				}
				suspended[tid] = NOT_SUSPENDED; 
//				suspended.erase(tid);
				break;
			}
		case SVOP::TASK_SUSPEND :
			{
				Task tid = trans.var/*.ToTask()*/;
				assert(suspended[tid] == NOT_SUSPENDED);
//				assert(suspended.find(tid) == suspended.end());
//				if(enabled.find(tid) != enabled.end()){
				if(enabled.Contains(tid)){
					CHESS_LOG(ENABLED_SET, "Disable", tid, trid);
		            ChessImpl::SetEventAttribute(ChessImpl::GetEventCounter()->getEventId(trid), DISABLE, tid);
					enabled.erase(tid);
					suspended[tid]= SUSPENDED_AND_ENABLED;
//					suspended[tid]= true;
				}
				else{
					suspended[tid]= SUSPENDED_AND_DISABLED;
//					suspended[tid]= false;
				}
				break;
			}
		case SVOP::TASK_END :
			{
//				assert(enabled.find(trans.tid) != enabled.end());
				assert(enabled.Contains(trans.tid));
				assert(suspended[trans.tid] == NOT_SUSPENDED);
//				assert(suspended.find(trans.tid) == suspended.end());
				CHESS_LOG(ENABLED_SET, "Disable", trans.tid, trid);
				ChessImpl::SetEventAttribute(ChessImpl::GetEventCounter()->getEventId(trid), DISABLE, trans.tid);
				enabled.erase(trans.tid);
				break;
			}
		case SVOP::LOCK_ACQUIRE:
			{
				// for t in enabled, if lookahead of t is a lock then t becomes disabled
				Task t = trans.tid;
				assert(enabled.Contains(trans.tid)); // rely on this for termination
				while(true){
					t = enabled.FindIndexLargerThan(t);
					if(t == trans.tid) 
						break;
					ChessTransition lookaheadTrans;
					if(execution->GetLookaheadAtStep(sid, t, lookaheadTrans)){
						if(lookaheadTrans.var == trans.var && lookaheadTrans.op == SVOP::LOCK_ACQUIRE){
							DisableTask(sid, ChessTransition(t, trans.var, SVOP::LOCK_ACQUIRE), false);
						}
					}
				};

				// Replace it later with a faster check
				//std::vector<Task> lockAcqs;
				//lockAcqs.reserve(enabled.size());
				//for(SortedTaskList::iterator ei = enabled.begin(); ei != enabled.end(); ei++){
				//	if(*ei == trans.tid)
				//		continue;
				//	ChessTransition lookaheadTrans;
				//	if(execution->GetLookaheadAtStep(sid, *ei, lookaheadTrans)){
				//		if(lookaheadTrans.var == trans.var && lookaheadTrans.op == SVOP::LOCK_ACQUIRE){
				//			lockAcqs.push_back(*ei);
				//		}
				//	}
				//}
				//for(std::vector<Task>::iterator li = lockAcqs.begin(); li != lockAcqs.end(); li++){
				//	DisableTask(sid, ChessTransition(*li, trans.var, SVOP::LOCK_ACQUIRE));
				//}
			}
			break;
		default:
			//nothing
			;
	}

	if(SVOP::IsReleaseOp(trans.op)){
		const SyncVar* varvec;
		int n;
		if(ChessImpl::GetSyncVarManager()->IsAggregate(trans.var)){
			varvec = ChessImpl::GetSyncVarManager()->GetAggregateVector(trans.var);
			n = ChessImpl::GetSyncVarManager()->GetAggregateVectorSize(trans.var);
		}
		else{
			varvec = &trans.var;
			n = 1;
		}
		for(int i=0; i<n; i++){
			SyncVar v = varvec[i];
			//releasedVars[v] = true;
			SortedTaskList& watchers = watchList[v];
			if(!watchers.IsEmpty()){
				Task first = watchers.FindIndexLargerThan(0);
				Task t = first;
				do{
					CHESS_LOG(ENABLED_SET, "Enable on Release", t, trans.var);
					EnableTask(sid, t);
					//remove t from all watchLists, this is needed because of wait_all or wait_any
					if(waitManyTasks[t] != 0){
						SyncVar agg = waitManyTasks[t];
						const SyncVar* aggVarVec = ChessImpl::GetSyncVarManager()->GetAggregateVector(agg);
						int aggVarLen = ChessImpl::GetSyncVarManager()->GetAggregateVectorSize(agg);
						for(int j = 0; j < aggVarLen; j++){
							if(aggVarVec[j] != v){
								watchList[aggVarVec[j]].erase(t);
							}
						}
					}
					waitManyTasks[t] = 0;
					

					t = watchers.FindIndexLargerThan(t);
				}while(t != first);
				watchers.clear();
			}
		}
	}

	// now that we have updated the enabled set for sid
	fairness.OnTransition(sid, trans, enabledSets[sid-1], enabledSets[sid]);
}

void EnabledSet::Backtrack(size_t step){
	enabledSets.resize(1);
	fairness.Clear();
	updateSid = 0;
	updateEid = 0;
	watchList.clear();
	waitManyTasks.clear();
	//releasedVars.clear();
}

std::ostream& EnabledSet::operator<<(std::ostream& o) const {
	int c = 0;
	for(size_t i = 0; i<enabledSets.size(); i++){
		o << "[" << c++ << "] :";
		//for(SortedTaskList::const_iterator ti = enabledSets[i].begin();
		//	ti != enabledSets[i].end(); ti++){
		//	o << *ti << ' ';
		//}
		const SortedTaskList& enabled = enabledSets[i];
		if(!enabled.IsEmpty()){
			Task first = enabled.FindIndexLargerThan(0);
			Task t = first;
			do{
				o << t << ' ';
				t = enabled.FindIndexLargerThan(t);
			}while(t != first);
		}
		o << "\n";
	}
	return o;
}

//OnTransition(ThreadId t){
//   Remove edges (x,t) from Q for all x
//   Remove edges (x,t) from P for all x
//   If t has yielded in the past
//                Add edges (t,x) to Q for all x that got disabled by this transition
//   If this transition is a yield {
//         For all x such that (t,x) is in Q add (t,x) to P
//         Remove edges (t,x) in Q for all x
//         Add edges (t,x) to Q for all x in enabled set
//   }
//}
//


void EnabledSet::FairnessAlgorithm::OnTransition(size_t sid, const ChessTransition& trans, 
										const SortedTaskList& oldEn, const SortedTaskList& newEn){
#ifdef NO_FAIRNESS
		return;
#endif
	if(priorityGraphs.Last().HasIncomingEdges(trans.tid)){
		PriorityGraph& pg = priorityGraphs.InsertLast(sid);
		pg.DelIncomingEdges(trans.tid);
	}
	if(pendingGraph.HasIncomingEdges(trans.tid)){
		pendingGraph.DelIncomingEdges(trans.tid);
	}

	int fb = Chess::GetOptions().fairness_parameter;
	assert(fb > 0);
	if(yieldCount[trans.tid] >= fb){  // have 'yielded' in the past
		BitVector disabledGuys;
		//XXX: Implement BitVector set difference
		Task t = trans.tid;
		assert(oldEn.Contains(trans.tid)); // rely on this for termination
		while(true){
			t = oldEn.FindIndexLargerThan(t);
			if(t == trans.tid)
				break;
			if(!newEn.Contains(t))
				disabledGuys.insert(t);
		}
		pendingGraph.AddEdges(trans.tid, disabledGuys);
	}

	if(trans.op == SVOP::TASK_YIELD){
		yieldCount[trans.tid]++;
		if(yieldCount[trans.tid] % fb == 0){
			// got an 'yield'
			if(pendingGraph.HasOutgoingEdges(trans.tid)){
				if(priorityGraphs.LastIndex() != sid){
					priorityGraphs.InsertLast(sid);
				}
				priorityGraphs.Last().AddEdges(trans.tid, pendingGraph.OutgoingEdges(trans.tid));
				pendingGraph.DelOutgoingEdges(trans.tid);
			}
			SortedTaskList en;
			newEn.Copy(en);
			en.erase(trans.tid);
			pendingGraph.AddEdges(trans.tid, en);
		}
	}

}

bool EnabledSet::FairnessAlgorithm::IsFairBlocked(size_t sid, const Task tid, const SortedTaskList& enabled){
#ifdef NO_FAIRNESS
	return false;
#endif
	const PriorityGraph& pg = priorityGraphs.Lookup(sid);
	if(!pg.HasOutgoingEdges(tid))
		return false;

	BitVector outedges;
	pg.OutgoingEdges(tid).Copy(outedges);
	outedges.Intersect(enabled);
	if(outedges.IsEmpty())
		return false;
	else
		return true;
//	PriorityGraph::EdgeIterator ei;
//	for(ei = pg.OutgoingEdgesBegin(tid); ei != pg.OutgoingEdgesEnd(tid); ei++){
////		if(enabled.find(*ei) != enabled.end()){
//		if(enabled.Contains(*ei)){
//			return true;
//		}
//	}

//	return false;
}

// Returns true if blocker blocks blocked at step sid (Katie)
bool EnabledSet::FairnessAlgorithm::FairBlocks(size_t sid, const Task blocker, const Task blocked, const SortedTaskList& enabled) {
#ifdef NO_FAIRNESS
	return false;
#endif
	const PriorityGraph& pg = priorityGraphs.Lookup(sid);
	BitVector outEdges = pg.OutgoingEdges(blocked);
	return outEdges.Contains(blocker);
}

void EnabledSet::FairnessAlgorithm::Clear(){
	priorityGraphs.clear();
	pendingGraph.Clear();
	yieldCount.clear();
}

