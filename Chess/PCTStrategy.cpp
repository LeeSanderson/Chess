
#include "PCTStrategy.h"
#include "ChessExecution.h"
#include "ChessImpl.h"

ChessExecution* PCTStrategy::InitialExecution(){
	return ChessImpl::EmptyExecution();
}

void PCTStrategy::CompletedExecution(ChessExecution* curr){
	return;
}

ChessExecution* PCTStrategy::NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound){
	num_of_runs--;

	if(num_of_runs > 0) {
		prev->ClearRecoveryPoint();
		prev->BacktrackToInitStack();
		return prev;
	}

	return 0;
}