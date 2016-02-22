
#pragma once
#include "IChessExecution.h"
#include "ChessExecution.h"

#include <map>

typedef size_t PRIORITY;
#define TaskInitPriority 20000
#define OFFSET 10000

class DeRandomizedPCTExecution : public ChessExecution{
public :
	DeRandomizedPCTExecution(const Task initTasks[], int n);
	DeRandomizedPCTExecution(ChessExecution* exec);
	~DeRandomizedPCTExecution();
	Task HighestPriorityTask(size_t index,Task tid);
	void AddNewTask(Task child);
	std::map<Task,PRIORITY>* GetPriorityMap() {
		return &priorityMap;
	}
	int NextTaskToSchedule(Task& next);
	void Reset();

protected :
	virtual int SyncVarAccess(ChessTransition& trans);

private :
	std::map<Task,PRIORITY> priorityMap;
	
};