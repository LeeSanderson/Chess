/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "IChessMonitor.h"
#include <iostream>

class PrintMonitor : public IChessMonitor {
public:
	void OnExecutionBegin(IChessExecution* exec){
		std::cout << "\n==================\nOnExecutionBegin : " << exec->NumTransitions() << " transitions \n";
	}
	
	void OnExecutionEnd(IChessExecution* exec){
		std::cout << "OnExecutionEnd   : " << exec->NumTransitions() << " transitions\n";
	}
	
	void OnShutdown(){
		std::cout << "CHESS shutting down\n";
	}
	
	// Called right *after* a Sync Var access - Calling it after ensures that there is no danger of LocalBacktrack
	void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid){}
	void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid){}
	
	void OnSchedulePoint(SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid){
		std::cout << "CHESS Schedule   : " << "step=" << sid << " var= " << var << " op = " << SVOP::ToString(op) << "\n";
	}

};

PrintMonitor* theMonitor = 0;
extern "C" __declspec(dllexport) IChessMonitor* GetMonitor(){
  if(!theMonitor)
    theMonitor = new PrintMonitor();
  return theMonitor;
}

