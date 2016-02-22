
#pragma once
#include "ChessExecution.h"
#include <ctime>

#include <map>

typedef size_t PRIORITY;
typedef size_t ERROR_PRIORITY;

class PCTExecution : public ChessExecution {
public :
	PCTExecution(const Task initTasks[], int n,int bug_depth, int seed);
	PCTExecution(ChessExecution* exec,int bug_depth, int seed);
	~PCTExecution();
	void AddNewTask(Task child);
	Task HighestPriorityTask(size_t index);
	int NextTaskToSchedule(Task& next);
	void Reset();
	static std::string getProgramLocation();
protected :
	virtual int SyncVarAccess(ChessTransition& trans);
	virtual bool IsPriorityChangePoint(ChessTransition& trans);
	void SwapPriority(size_t, Task t);
	void printPriorities(size_t index);
	std::hash_set<PRIORITY> priorities_inversion_points;
	bool debug;
	int bug_depth;
	int seed;
	int prev_k;
	int crnt_k;

private :
	void InvertPriority(Task t);
	void AssignPriority(Task t);
	PRIORITY GetPriority(Task t);
	void AllocatePriorityInversionPoints();
	std::map<Task,PRIORITY> priorityMap;
	std::hash_set<PRIORITY> usedpriorities;

	int crnt_priority;
	int init_priority;
};
