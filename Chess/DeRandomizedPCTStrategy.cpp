
#pragma once
#include "DeRandomizedPCTStrategy.h"
#include "ChessImpl.h"
#include "ChessTransition.h"
#include "DeRandomizedPCTExecution.h"
#include <map>
#include <stack>
#include <utility>
#include "windows.h"

DeRandomizedPCTStrategy::DeRandomizedPCTStrategy(int bugdepth) : bugdepth(bugdepth) {
	backTrackStep = 0;
	firsttime = true;
	debug = false;
	crnt_bugdepth = 1;
}

ChessExecution* DeRandomizedPCTStrategy::InitialExecution() {
	return ChessImpl::EmptyExecution();
}

void DeRandomizedPCTStrategy::CompletedExecution(ChessExecution* curr){
	return;
}

int DeRandomizedPCTStrategy::PreemptionPoint(ChessExecution* exec,size_t backtrackpoint) {
	
	int i;
	assert(crnt_bugdepth == depth.size());
	for( i = backtrackpoint; i > depth[crnt_bugdepth - 2]; i--) {
		ChessTransition trans = exec->Transition(i);
		SyncVarOp op = trans.op;
		if (op == SVOP::TASK_FORK || op == SVOP::TASK_END 
			|| op == SVOP::TASK_RESUME || op == SVOP::TASK_FENCE)
			continue;

		if(i < 1)
			continue;

		if(exec->Transition(i - 1).tid != trans.tid)
			continue;

		return i;
	}
	return -1;

}

ChessExecution* DeRandomizedPCTStrategy::NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound) {

	DeRandomizedPCTExecution* pctexec = (DeRandomizedPCTExecution*)prev;
	std::map<Task,PRIORITY>* priorityMap = pctexec->GetPriorityMap();
	size_t Nsteps = prev->NumTransitions();

	if(Nsteps < backTrackStep)
		return 0;

	while(true) {
		if( crnt_bugdepth == 1 ) {
			if(backTrackStep != 0) {
				assert(prev->Transition(backTrackStep - 1).op == SVOP::TASK_FORK);
				Task prev_tid = prev->Transition(backTrackStep - 1).var;
				assert(PriorityStack[prev_tid] == (prev_tid + TaskInitPriority));		// its earlier priority should be the init priority
				(*priorityMap)[prev_tid] = (TaskInitPriority - prev_tid);			// restore the previous priority
				PriorityStack.erase(prev_tid);
				assert(PriorityStack.find(prev_tid) == PriorityStack.end());
			} else if(firsttime) {
				firsttime = false;
				
				depth.push_back(prev->GetInitStack());
				assert(depth.size() == crnt_bugdepth);

				if(bugdepth > crnt_bugdepth) {
					crnt_bugdepth++;
				}

				continue;
			}

			for(size_t i = backTrackStep ; i < Nsteps ; i++) {
				ChessTransition trans = prev->Transition(i);
				if(trans.op == SVOP::TASK_FORK ) {
					//log the previous priority
					assert(priorityMap->find(trans.var) != priorityMap->end());
					PriorityStack.insert(std::pair<Task,PRIORITY>(trans.var,priorityMap->find(trans.var)->second));
					(*priorityMap)[trans.var] = TaskInitPriority + trans.op - OFFSET;
					Task next_task = prev->Transition(i + 1).tid;
					prev->Backtrack(i,next_task);
					backTrackStep = i + 1;
					if(bugdepth > crnt_bugdepth) {
						crnt_bugdepth++;	// jump to the next bug-depth
					}

					return prev;
				}
			}
			return 0;
		} else {

			int prev_backtrackpoint;
			assert(crnt_bugdepth >= depth.size());
			if(crnt_bugdepth > depth.size()) {
				depth.push_back(Nsteps);
				ChessTransition prev_trans = prev->Transition(Nsteps - 1);
				stack.push(std::pair<Task,PRIORITY>(prev_trans.tid,(*priorityMap)[prev_trans.tid]));
			}

			assert(crnt_bugdepth == depth.size());
			prev_backtrackpoint = depth[depth.size() - 1];
			int new_backtrackpoint = PreemptionPoint(prev,prev_backtrackpoint - 1);
			depth.pop_back();
			depth.push_back(new_backtrackpoint);
			if(new_backtrackpoint == -1) {
				ChessTransition prev_trans = prev->Transition(prev_backtrackpoint - 1);
				std::pair<Task,PRIORITY> tuple = stack.top();
				stack.pop();
				assert(prev_trans.tid == tuple.first);
				(*priorityMap)[prev_trans.tid] = tuple.second;	// restore the previous priority
				// now run the program again with bug_depth = 1 so that we can decide the scheduling for the next run
				prev->Backtrack(prev_backtrackpoint - 1,prev->Transition(prev_backtrackpoint - 1).tid);

				crnt_bugdepth--;		// backtrack to the previous level
				depth.pop_back();
				return prev;
			} else {
				ChessTransition prev_trans = prev->Transition(prev_backtrackpoint - 1);
				ChessTransition curr_trans = prev->Transition(new_backtrackpoint);
				std::pair<Task,PRIORITY> tuple = stack.top();
				stack.pop();
				assert(prev_trans.tid == tuple.first);
				(*priorityMap)[prev_trans.tid] = tuple.second;	// restore the previous priority
				stack.push(std::pair<Task,PRIORITY>(curr_trans.tid,(*priorityMap)[curr_trans.tid]));
				(*priorityMap)[curr_trans.tid] = 5000 - crnt_bugdepth;// update the new pririty
				prev->Backtrack(new_backtrackpoint - 1,prev->Transition(new_backtrackpoint - 1).tid);
				if(bugdepth > crnt_bugdepth) {
					crnt_bugdepth++;
				}
				return prev;
			}
		}
	}
	return 0;
}
