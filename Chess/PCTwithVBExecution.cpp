
#pragma once
#include "PCTwithVBExecution.h"
#include "PCTExecution.h"
#include "SyncVarManager.h"
#include "ChessAssert.h"
#include "Chess.h"

PCTwithVBExecution::PCTwithVBExecution(const Task tasks[], int n, int _bug_depth, int num_of_vars, int seed) : PCTExecution(tasks,n,_bug_depth, seed) {
	for(size_t i = SyncVarManager::FirstNonTaskSyncVar ; i < (SyncVarManager::FirstNonTaskSyncVar + num_of_vars) ; i++ ) {
		variables_to_track.insert(i);
		var_to_k_map.insert(std::pair<SyncVar,int>(i,0));
	}
	Total_num_of_vars = 5;
	Num_of_vars_to_track = num_of_vars;
}

PCTwithVBExecution::PCTwithVBExecution(ChessExecution* exec,int bug_depth, int num_of_vars, int seed) : PCTExecution(exec,bug_depth, seed){
	for(size_t i = SyncVarManager::FirstNonTaskSyncVar ; i < (SyncVarManager::FirstNonTaskSyncVar + num_of_vars) ; i++ ) {
		variables_to_track.insert(i);
		var_to_k_map.insert(std::pair<SyncVar,int>(i,0));
	}
	Total_num_of_vars = 5;
	Num_of_vars_to_track = num_of_vars;
}

bool PCTwithVBExecution::IsPriorityChangePoint(ChessTransition& trans) {

	SyncVarOp op = trans.op;
	if((op == SVOP::TASK_FORK || op == SVOP::TASK_END 
			|| op == SVOP::TASK_RESUME || op == SVOP::TASK_FENCE))
		return false;

	if(var_to_k_map.find(trans.var) == var_to_k_map.end()) {
		var_to_k_map.insert(std::pair<SyncVar,int>(trans.var,1));
	} else {
		var_to_k_map.find(trans.var)->second = var_to_k_map.find(trans.var)->second + 1;
	}

	if(variables_to_track.find(trans.var) == variables_to_track.end()) {
		return false;
	}

	crnt_k++;

	if(priorities_inversion_points.find(crnt_k) == priorities_inversion_points.end()) {
		return false;
	}
	return true;
}

void PCTwithVBExecution::Reset() {
	variables_to_track.clear();
	int act_k = crnt_k;
	crnt_k = 0;
	if(var_to_k_map.size() != 0) {
		Total_num_of_vars = var_to_k_map.size();
	}
	assert(Num_of_vars_to_track <= Total_num_of_vars);
	while(variables_to_track.size() < Num_of_vars_to_track) {
		SyncVar syncvar = ((rand() % Total_num_of_vars) + SyncVarManager::FirstNonTaskSyncVar);
		if(variables_to_track.find(syncvar) == variables_to_track.end()) {
			variables_to_track.insert(syncvar);
			if(var_to_k_map.find(syncvar) != var_to_k_map.end()) {
				crnt_k += var_to_k_map.find(syncvar)->second;
			}
			else {
				crnt_k += 2;
			}
		} else {
			continue;
		}
	}

	std::hash_map<SyncVar,int>::iterator itr;
	for(itr = var_to_k_map.begin() ; itr != var_to_k_map.end(); itr++) {
		itr->second = 0;
	}
	PCTExecution::Reset();
}
