
#pragma once
#include "PCTExecution.h"

#include "ChessExecution.h"
#include "windows.h"
#include "EnabledSet.h"
#include "ChessProfilerTimer.h"
#include "Chess.h"
#include "ChessAssert.h"
#include "ChessImpl.h"

#include <time.h>
#include <cstdlib>

#define DBG(a) \
	do { } while(false);

// numbers from 4000 to 7999 are reserved for the priorities to the tasks 

PCTExecution::PCTExecution(const Task tasks[], int n, int _bug_depth, int _seed) : ChessExecution(tasks,n) {

	time_t seconds;
	time(&seconds);
	seed = _seed;
	if(seed == -1) {
		seed = (int) seconds;
	}
	std::cout << "Seed: " << seed << "\n";
	srand((unsigned int)seed);

	bug_depth = _bug_depth;
	for(int i=0; i<n; i++) {
		AssignPriority(tasks[i]);
	}

	crnt_priority = 1;
	prev_k = 10;
	AllocatePriorityInversionPoints();
	crnt_k = 0;
	debug = false;
}

PCTExecution::~PCTExecution() {
// todo : Gives some strange Error ???
//	ChessExecution::~ChessExecution();
}

void PCTExecution::AssignPriority(Task t) {
	int priority = (rand() % 4000) + 4000;
	while( usedpriorities.find(priority) != usedpriorities.end() ) {
		priority = (rand() % 4000) + 4000;
	}
	usedpriorities.insert(priority);
	assert(usedpriorities.find(priority) != usedpriorities.end());
	//priorityMap.insert(std::pair<Task,PRIORITY>(t,priority));
	priorityMap[t] = priority;
    DBG(std::cout << "PCT: Assigned " << t << "\t" << priority << "\n";);

}


void PCTExecution::InvertPriority(Task t){
	int priority = (rand() % 4000);

	while(usedpriorities.find(priority) != usedpriorities.end()) {
		priority = (rand() % 4000);
	}

	priorityMap[t] = priority;
	DBG(std::cout << "PCT: Assigned " << t << "\t" << priority << "\n";);
	assert(usedpriorities.find(priority) == usedpriorities.end());
	usedpriorities.insert(priority);
}

PRIORITY PCTExecution::GetPriority(Task t) {
	return priorityMap.find(t)->second;
}

PCTExecution::PCTExecution(ChessExecution* exec, int _bug_depth, int _seed) : ChessExecution(exec) {
	time_t seconds;
	time(&seconds);
	seed = _seed;
	if(seed == -1) {
		seed = (int) seconds;
	}
	std::cout << "Seed: " << seed << "\n";
	srand((unsigned int)seed);

		
	bug_depth = _bug_depth;
	std::set<Task>::const_iterator itr = exec->GetInitTasks().begin();
	for (itr; itr != exec->GetInitTasks().end(); itr++) {
		AssignPriority(*itr);
	}

	crnt_priority = 1;
	prev_k = 10;
	AllocatePriorityInversionPoints();
	crnt_k = 0;
	debug = false;
}

void PCTExecution::AddNewTask(Task child) {
	ChessExecution::AddNewTask(child);
	AssignPriority(child);
}

bool PCTExecution::IsPriorityChangePoint(ChessTransition& trans) {

	// the only pre-emption points are Lock Acquire Lock Release, RwVar Read/Write 
	//if(trans.op != SVOP::LOCK_RELEASE && trans.op != SVOP::LOCK_ACQUIRE && trans.op != SVOP::RWVAR_READ && trans.op != SVOP::RWVAR_WRITE) {
	//	return false;
	//}

	// donot pre-empt before these operations
	SyncVarOp op = trans.op;
	if((op == SVOP::TASK_FORK || op == SVOP::TASK_END 
			|| op == SVOP::TASK_RESUME || op == SVOP::TASK_FENCE))
		return false;

	if(priorities_inversion_points.find(crnt_k) == priorities_inversion_points.end()) {
		crnt_k++;
		return false;
	}
	crnt_k++;
	return true;
}

Task PCTExecution::HighestPriorityTask(size_t index) {
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

void PCTExecution::printPriorities(size_t index) {
	std::cout << "PCT: ";
    std::map<Task,PRIORITY>::iterator p;
	for(p = priorityMap.begin() ; p != priorityMap.end();p++) {
		if(enabled->IsEnabledAtStep(index, p->first)) {
			std::cout << "(" << p->first << "," << p->second << ") ";
		} else {
			std::cout << "(" << p->first << "d," << p->second << ") ";
		}
	}
	std::cout << "\n";
}

void PCTExecution::SwapPriority(size_t index, Task t) {
	std::hash_set<Task> enabledTasks;

    std::map<Task,PRIORITY>::iterator p;
	for(p = priorityMap.begin() ; p != priorityMap.end();p++) {
		if(enabled->IsEnabledAtStep(index, p->first) && t != p->first) {
			enabledTasks.insert(p->first);
		}
	}

	if(enabledTasks.empty()) return;
	int num = (rand() % enabledTasks.size());

	std::hash_set<Task>::iterator p2;
	for(p2 = enabledTasks.begin(); p2 != enabledTasks.end(); p2++, num--) {
		if(num == 0) {
			int priority1 = priorityMap[t];
			int priority2 = priorityMap[*p2];
			priorityMap[t] = priority2;
			priorityMap[*p2] = priority1;
			DBG(std::cout << "PCT: Swapped " << t << "\t" << priority2 << "\n";);
			DBG(std::cout << "PCT: Swapped " << *p2 << "\t" << priority1 << "\n";);
			return;
		}
	}
}

void PCTExecution::Reset(){
	ChessExecution::Reset();
	usedpriorities.clear();
	priorityMap.clear();
	
	std::set<Task>::const_iterator itr = initTasks.begin();
	for (itr; itr != initTasks.end(); itr++) {
		AssignPriority(*itr);
	}
	crnt_priority = 1;
	if(crnt_k != 0)
		prev_k = crnt_k;

	AllocatePriorityInversionPoints();
	crnt_k = 0;

}

void PCTExecution::AllocatePriorityInversionPoints(){
	priorities_inversion_points.clear();
	if(bug_depth >= prev_k) {
		prev_k = bug_depth + 1;
	}
	assert(bug_depth < prev_k);
	for(int i = 0 ; i < bug_depth ; i++ ) {
		int point = rand()% prev_k;
		while(priorities_inversion_points.find(point) != priorities_inversion_points.end()) {
			point = rand()% prev_k;
		}
		priorities_inversion_points.insert(point);
	}
}


int PCTExecution::NextTaskToSchedule(Task& next){
	assert(!InReplayMode());
	Task curr = Task(0);
	Task max_priority_task = HighestPriorityTask(topIndex);
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

int PCTExecution::SyncVarAccess(ChessTransition& trans){
	ChessProfilerSentry s("ChessExecution::SyncVarAccess");

	DBG({
		printPriorities(topIndex);
	    std::cout << "PCT: Running " << trans.tid << "\n";
	});

	// We are executing transition with trid == topIndex
	// We are currently in the state with sid == topIndex

	// Eagerly update the Enabled state
	AddLookahead(trans);
	enabled->UpdateEnabled(topIndex);

	if(InReplayMode()) {		
		
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
	}
	
	Task running_tid = trans.tid;
	bool fb = IsFairBlocked(trans.tid);
	bool en = enabled->IsEnabledAtStepWithNoFairness(topIndex, trans.tid);
	if(fb && en && false) {
		//int p1 = priorityMap.find(running_tid)->second;
		DBG(std::cout << "PCT: === fb ===\n";);
		DBG(printPriorities(topIndex););
		SwapPriority(topIndex, running_tid);
		DBG(printPriorities(topIndex););
		DBG(std::cout << "PCT: +++ fb +++\n";);
		//int p2 = priorityMap.find(running_tid)->second;
		//std::cout << "PCT: Changed from " << p1 << " to " << p2 << std::endl;
	} else if(IsPriorityChangePoint(trans)) {	
		//std::cout << "PCT: " << PCTExecution::getProgramLocation() << "\n";
		DBG(std::cout << "PCT: Changing priority\n";);
		InvertPriority(running_tid);
		DBG(printPriorities(topIndex););
	}

	Task max_pri_task = HighestPriorityTask(topIndex);
	if(fb || max_pri_task != trans.tid){
		DBG(std::cout << "PCT: Need CS\n";);
		return REQUIRE_CONTEXT_SWITCH;
	}
	DBG(std::cout << "PCT: Continuing\n";);
	stack.push_back(trans);
	topIndex++;
	return SUCCESS;
}

#define PCT_MAX_FRAME_DEPTH 3
#define PCT_MAX_STRLEN 1024
#define PCT_MAX_BUFSIZE MAX_FRAME_DEPTH*MAX_STRLEN

std::string PCTExecution::getProgramLocation() {

	static char filenamebufs[PCT_MAX_FRAME_DEPTH][PCT_MAX_STRLEN];
	static char *filename[PCT_MAX_FRAME_DEPTH];
	static int lineno[PCT_MAX_FRAME_DEPTH];
	static char procbufs[PCT_MAX_FRAME_DEPTH][PCT_MAX_STRLEN];
	static char *proc[PCT_MAX_FRAME_DEPTH];

	for(int i=0; i<PCT_MAX_FRAME_DEPTH; i++){
		filename[i] = filenamebufs[i];
		proc[i] = procbufs[i];
		filenamebufs[i][0] = procbufs[i][0] = 0;
	}

	std::ostringstream sstr;
	if(Chess::GetSyncManager()->GetCurrentStackTrace(PCT_MAX_FRAME_DEPTH, PCT_MAX_STRLEN, proc, filename, lineno))
	{
		int pos = 0;
		for(int i=0; i<PCT_MAX_FRAME_DEPTH; i++){
			if(proc[i][0] == 0)
				continue;
			if(filename[i][0] == 0)
				continue;
			int prefixpos = 0;
			for (int j = 0; j <PCT_MAX_STRLEN; j++)
               if (filename[i][j] == 0)
				   break;
			   else if (filename[i][j] == '\\' || filename[i][j] == ':' )
				   prefixpos = j + 1;
			sstr << proc[i] << ':' << (filename[i] + prefixpos) << '(' << lineno[i] << ')';
			break;
		} 
	}
	return sstr.str();
}
