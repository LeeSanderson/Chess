
#pragma once
#include "DeRandomizedPCTExecution.h"
#include "EnabledSet.h"
#include "ChessProfilerTimer.h"
#include "ChessImpl.h"

DeRandomizedPCTExecution::DeRandomizedPCTExecution(const Task tasks[], int n) : ChessExecution(tasks,n) {
	for(int i=0; i<n; i++) {
		priorityMap.insert(std::pair<Task,PRIORITY>(tasks[i],TaskInitPriority + tasks[i]));
	}
}

DeRandomizedPCTExecution::DeRandomizedPCTExecution(ChessExecution* exec) : ChessExecution(exec) {
	
	std::set<Task>::const_iterator itr = exec->GetInitTasks().begin();
	for (itr; itr != exec->GetInitTasks().end(); itr++) {
		priorityMap.insert(std::pair<Task,PRIORITY>(*itr,TaskInitPriority + *itr));
	}

}

DeRandomizedPCTExecution::~DeRandomizedPCTExecution() {
// todo : Gives some strange Error ???
//	ChessExecution::~ChessExecution();
}


void DeRandomizedPCTExecution::Reset() {
	ChessExecution::Reset();
	if(GetInitStack() == 0) {
		priorityMap.clear();
	}
	
	std::set<Task>::const_iterator itr = initTasks.begin();
	for (itr; itr != initTasks.end(); itr++) {
		priorityMap.insert(std::pair<Task,PRIORITY>(*itr,TaskInitPriority + *itr));
	}

}



void DeRandomizedPCTExecution::AddNewTask(Task child) {
	ChessExecution::AddNewTask(child);
	if(priorityMap.find(child) == priorityMap.end()) {
		// assign a priority to the child task
		// priority = taskinitpriority + threadid
		priorityMap.insert(std::pair<Task,PRIORITY>(child,TaskInitPriority + child));
	}

}

int DeRandomizedPCTExecution::SyncVarAccess(ChessTransition& trans){
	ChessProfilerSentry s("ChessExecution::SyncVarAccess");
	// We are executing transition with trid == topIndex
	// We are currently in the state with sid == topIndex

	// Eagerly update the Enabled state
	AddLookahead(trans);
	enabled->UpdateEnabled(topIndex);

	if(InReplayMode()){
		ChessTransition& top = stack[topIndex];
		if(top.tid != trans.tid){
			// expect a context switch
			AddLookahead(trans);
			// if we're replaying a local backtrack, tell monitors about the event so
			// that blocks will show up in the ConcurrencyExplorer even in replay
			// mode (BFS)
			for (size_t i = 0; i < events.size(); i++) {
				if (events[i].sid == topIndex && events[i].eid == ExecEvent::LOCALBACKTRACK && events[i].tid == stack[topIndex-1].tid) {
					{
						ChessImpl::SetNextEventAttribute(STATUS, "b");
					}
				}
			}
			return REQUIRE_CONTEXT_SWITCH;
		}
		// check for nondeterminism
		// XXX: When changing the logic for detecting nondeterminism
		//  make sure the check in NondeterminismHandler.cpp is consistent
		if(top.op != trans.op){ // vars can change
			return REQUIRE_NONDETERMINISM_PROCESSING;
		}
		if(top.var != trans.var){
			top.var = trans.var;
		}
		topIndex++;
		return SUCCESS;
	} else {
		// again run the highest priority thread
		Task running_tid = trans.tid;
		Task max_pri_task = HighestPriorityTask(topIndex,trans.tid);
		if(IsFairBlocked(trans.tid) || max_pri_task != trans.tid){
			AddLookahead(trans);
			return REQUIRE_CONTEXT_SWITCH;
		}
		stack.push_back(trans);
		topIndex++;
		return SUCCESS;
	}
}

Task DeRandomizedPCTExecution::HighestPriorityTask(size_t index,Task tid) {
	std::map<Task,PRIORITY>::iterator p;
	PRIORITY max_priority = 0;
	Task task_max_priority = 0;
	for(p = priorityMap.begin() ; p != priorityMap.end();p++) {
		if(enabled->IsEnabledAtStep(index, p->first)) {
			if(max_priority <= p->second) {
				max_priority = p->second;
				task_max_priority = p->first;
			}
		}
	}
	return task_max_priority;
}


int DeRandomizedPCTExecution::NextTaskToSchedule(Task& next){
	if(ChessExecution::InReplayMode()){
		next = stack[topIndex].tid;
		//	assert(lookahead.find(next) != lookahead.end());
		assert(lookahead[next].tid != 0); // not a null transition
		ChessTransition access = lookahead[next];
		assert(access.tid == next);
		//	lookahead.erase(next);
		lookahead[next] = ChessTransition();
		//bool ret = SyncVarAccess(access);
		//assert(ret);
		return SUCCESS;
	}
	else {
		// else run the highest priority thread
		Task curr = Task(0);
		Task max_priority_task = HighestPriorityTask(topIndex,curr);
		if(topIndex != 0){
			curr = stack[topIndex-1].tid;
			if( max_priority_task == curr){
				next = curr;
				return SUCCESS;
			}
		}

		if(!enabled->NextEnabledAtStep(topIndex, curr, next)){
			//deadlock
			// but not if there is a task waiting for quiescence
			Task taskwfq;
			if(IsTaskWaitingForQuiescence(taskwfq)){
				// reached quiescence
				QuiescentWakeup(taskwfq);
				next = taskwfq;
				return SUCCESS;
			}
			return FAILURE; // deadlock
			}
		next = max_priority_task;
		assert(lookahead[next].tid != 0); // not a null transition
		ChessTransition access = lookahead[next];
		assert(access.tid == next);
		lookahead[next] = ChessTransition();
		//bool ret = SyncVarAccess(access);
		//assert(ret);
		return SUCCESS;
	}
}