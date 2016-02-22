/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessMonitorTest.h"
#include "ChessExecution.h"
#include "ChessAssert.h"
#include "Chess.h"
#include "ChessImpl.h"

ChessMonitorTest::ChessMonitorTest(){
	currStep = 0;
	state = INIT;
}

void ChessMonitorTest::OnExecutionBegin(IChessExecution *exec){
	//ChessExecution* exec = (ChessExecution*)iexec; // XXX: not using dynamic_cast
	assert(state == INIT); 
	currExecution.clear();
	for(size_t i=0; i<exec->GetInitStack(); i++){
		currExecution.push_back(exec->Transition(i));
	}
	currStep = exec->GetInitStack();

	if(exec->NumTransitions() > exec->GetInitStack()){
		// All transitions BUT the last one are to be replayed
		for(size_t i=exec->GetInitStack(); i<exec->NumTransitions()-1; i++){
			currExecution.push_back(exec->Transition(i));
		}
	}

	if(exec->NumTransitions() > exec->GetInitStack()){
		state = REPLAY;
	}
	else{
		state = RECORD;
	}
}

void ChessMonitorTest::OnExecutionEnd(IChessExecution *exec){
	//ChessExecution* exec = (ChessExecution*)iexec; // XXX: not using dynamic_cast
	assert(state == RECORD);

	bool sameExec = true;
	if(	currStep != exec->NumTransitions()){
		sameExec = false;
	}
	else{
		for(size_t i=0; i<currStep; i++){
			if(! ( currExecution[i].tid == exec->Transition(i).tid 
				//&& currExecution[i].var == exec->Transition(i).var 
				&& currExecution[i].op == exec->Transition(i).op)){
					sameExec = false;
					break;
			}
		}
	}
	if(!sameExec){
		*GetChessErrorStream() << "ChessMonitorTest failed OnExecutionEnd\n";
		*GetChessErrorStream() << " Execution (NumTrans = " << exec->NumTransitions() << ")\n";
		*GetChessErrorStream() << *exec << "\n";
		*GetChessErrorStream() << " OnSyncVarAccess() called for \n";
		for(size_t i=0; i<currStep; i++){
			*GetChessErrorStream() << "[" << i << "]: " 
				<< currExecution[i].tid << " " 
				<< currExecution[i].var << " " 
				<< SVOP::ToString(currExecution[i].op) << '\n';
		}
		*GetChessErrorStream() << std::endl;
		assert(false);
	}

	state = INIT;
}

void ChessMonitorTest::OnShutdown(){
	assert(state != SHUTDOWN);
	state = SHUTDOWN;
}

void ChessMonitorTest::OnDataVarAccess(EventId id, void *loc, int size, bool isWrite, size_t pcId){
	//assert(state == RECORD || state == REPLAY);
}

void ChessMonitorTest::OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid){
	assert(state == RECORD || state == REPLAY);
	if(state== REPLAY){
		if(! (currExecution[currStep].tid == tid 
				//&& currExecution[currStep].var == trans.var 
				&& currExecution[currStep].op == op))
		{
			ChessTransition exp = currExecution[currStep];
			*GetChessErrorStream() << "ChessMonitorTest failed OnSyncVarAccess at step " << currStep << "\n";
			*GetChessErrorStream() << " Expecting : " << exp.tid << " " << exp.var << " " << SVOP::ToString(exp.op) << "\n";
			*GetChessErrorStream() << "      Got  : " << tid << " " << var << " " << SVOP::ToString(op) << "\n";
			*GetChessErrorStream() << " While replaying \n";
			*GetChessErrorStream() << *ChessImpl::CurrExecution() << std::endl;
			assert(false);

		}
	}
	else{
		currExecution.push_back(ChessTransition(tid, var, op));
	}
	currStep++;
	if(state == REPLAY && currStep == currExecution.size()){
		state = RECORD;
	}
}

void ChessMonitorTest::OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid){
//SyncVar agg(var, n);
	//XXX: Not checking var id's anyway, so send in just var[0]
	OnSyncVarAccess(id, tid, var[0], op, sid);
}

void ChessMonitorTest::OnSchedulePoint(ChessMonitorTest::SchedulePointType type, Task tid, SyncVar var, SyncVarOp op, size_t sid){
	assert(state == RECORD || state == REPLAY);
}