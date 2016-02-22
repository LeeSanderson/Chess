/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "NondeterminismHandler.h"
#include "ChessImpl.h"
#include "BestFirstExecution.h"

bool NondeterminismHandler::reported_nondeterminism_already = false;

ChessExecution* NondeterminismHandler::AccessNondeterminism(ChessExecution *exec, Task tid, SyncVar var, SyncVarOp op){
	bool firstTest = ChessImpl::InTestStartup();
	if(!Chess::GetOptions().handle_nondeterminism || firstTest){
		ChessTransition trans = ChessTransition(tid,var,op);
		ChessTransition exp = exec->ExpectedTransition(exec->NumTransitions());
		std::stringstream s;
		if(firstTest)
			s << "Nondeterminism occured during TestStartup, and CHESS is unable to recover";
		else
			s << "Detected nondeterminism outside the control of Chess\n";
		s << " Got an unexpected SyncVarAccess: " << trans << "\n";
		s << " Was expecting SyncVarAccess    : " << exp << "\n";
		s << " Current execution length       : " << exec->NumTransitions() << "\n";
		ChessImpl::ChessAssertion(s.str().c_str(), CHESS_EXIT_NONDET_ERROR);
		return exec;
	}

	return ProcessNondeterminism(exec);
}

ChessExecution* NondeterminismHandler::EnabledNondeterminism(ChessExecution *exec, Task tid){
	bool firstTest = ChessImpl::InTestStartup();
	if(!Chess::GetOptions().handle_nondeterminism || firstTest){
		std::stringstream s;
		if(firstTest)
			s << "Nondeterminism occured during TestStartup, and CHESS is unable to recover";
		else
			s << "Detected nondeterminism outside the control of Chess\n";
		s << " Didnt expect transition to be disabled : " << exec->Transition(exec->NumTransitions()-1) << "\n";
		s << " Current execution length       : " << exec->NumTransitions() << "\n";
		ChessImpl::ChessAssertion(s.str().c_str(), CHESS_EXIT_NONDET_ERROR);
		return exec;
	}

	return ProcessNondeterminism(exec);
}

	// simple strategy for recovering from lazy initialization:
	// First, define an occurrance of "lazy initialization" as
	//   Failure to execute a targetExecution numAttemptsAtTargetExecutioin times
	//   And the program executes the same execution in every attempt
	//
	// Recovery works as follows:
	//   Attempt targetExecution numAttemptsAtTargetExecution times
	//   if succeeded in any one: proceed as if nothing happened
	//   else
	//       if we saw the same exec E, backtrack from E
	//       else backtrack from common prefix of executions seen

ChessExecution* NondeterminismHandler::ProcessNondeterminism(ChessExecution* exec){
	nondeterminismSeen = true;
	if(targetExecution == 0){
		targetExecution = exec;
	}
	else{
		assert(targetExecution == exec); 
	}

	// clone the current execution from the repro execution
	// On backtrack(), we will do further nondeterminism processing
	exec = exec->Clone();

	// jump to record mode
	exec->PruneExecution(exec->NumTransitions());
	return exec;
}

void NondeterminismHandler::RecoverNondeterminismOnBacktrack(ChessExecution *exec, NondeterminismHandler::RecoveryStatus& ret){
	ret.action = RecoveryStatus::NO_RECOVERY;

	if(targetExecution == 0)
		return; // common case - havent seen nondeterminism yet
	
	if(!nondeterminismSeen){
		// succeeded in exploring targetExecution
		CleanupGlobalState();
		return; // continue as if nothing happened
	}

	exploredExecutions.push_back(exec);

	if(exploredExecutions.size() < (size_t)numAttemptsAtTargetExecution){
		CleanupPerExecutionState();
		// try executing targetExecution
		ret.action = RecoveryStatus::EXECUTE_EXECUTION;
		ret.execution = targetExecution;
		return;
	}

	// give up attempting to execute targetExecution
	size_t commonPrefix = GetCommonPrefixOfExploredExecutions();
	bool same = true;
	for(size_t i=0; i<exploredExecutions.size(); i++){
		if(exploredExecutions[i]->NumTransitions() != commonPrefix)
			same = false;
	}
	if(same){
		// detected lazy initialization
		ret.action = RecoveryStatus::BACKTRACK_FROM_EXECUTION;
		ret.execution = exec;
		ret.backtrackStep = 0; // no bound
	}
	else{
		if(commonPrefix == 0){
			// no common prefix, something bad is happening
			// shove your head in the sand and simply continue from exec
			// setting backtrackStep to 0 will do the "right" thing
		}
		ret.action = RecoveryStatus::BACKTRACK_FROM_EXECUTION;
		ret.execution = exec;
		ret.backtrackStep = commonPrefix;
		ChessImpl::SetExitCode(CHESS_EXIT_INCOMPLETE_INTERLEAVING_COVERAGE);
        if (! reported_nondeterminism_already)
		{
			Chess::ReportWarning("Detected Nondeterminism. Coverage may be incomplete.", 
				"<action name=\"Repro\"> <carg>/p:handle_nondeterminism=false</carg></action>", false);
			reported_nondeterminism_already = true;
		}
	}
	// If we're going to re-execute a half-completed BestFirstExecution we need to fix it up a bit (Katie)
	if (ChessImpl::GetOptions().best_first) {
		static_cast<BestFirstExecution*>(ret.execution)->Reenable(targetExecution->NumTransitions()-1);
	}
	assert(exploredExecutions[exploredExecutions.size()-1] == exec);
	exploredExecutions[exploredExecutions.size()-1] = targetExecution; // ensures we do not delete exec, and delete targetExecution
	CleanupGlobalState();
	return;
}

size_t NondeterminismHandler::GetCommonPrefixOfExploredExecutions(){
	size_t ret;
	for(ret = 0; ret < exploredExecutions[0]->NumTransitions(); ret++){
		const ChessTransition& t1 = exploredExecutions[0]->Transition(ret);

		for(size_t i=1; i<exploredExecutions.size(); i++){
			if(exploredExecutions[i]->NumTransitions() <= ret)
				return ret;
			const ChessTransition& t2 = exploredExecutions[i]->Transition(ret);
			
			// now do the same check that ChessExecution does to detect nondetermninism
			if(t1.tid != t2.tid || t1.op != t2.op){ // note: vars can change
				// different
				return ret;
			}
		}
	}
	return ret;
}

void NondeterminismHandler::CleanupGlobalState(){
	CleanupPerExecutionState();

	targetExecution = 0;
	for(size_t i=0; i<exploredExecutions.size(); i++){
		if(exploredExecutions[i])
			delete exploredExecutions[i];
	}
	exploredExecutions.resize(0);
}

void NondeterminismHandler::CleanupPerExecutionState(){
	nondeterminismSeen = false;
}