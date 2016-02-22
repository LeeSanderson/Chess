/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "StatsMonitor.h"
#include "ChessExecution.h"
#include "Chess.h"
#include "ChessImpl.h"
#include "ResultsPrinter.h"

#if SINGULARITY
#include <chess_clib.h>

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif
#else

#pragma warning( push )  // Push the existing state of all warnings
#pragma warning( disable: 25004 25025) // lets make Prefix happy  

#include <windows.h>

#pragma warning( pop )  // Restore all warnings to their previous state

#endif
#include "SSEstimator.h"

using namespace std;


StatsMonitor::StatsMonitor(int freq){
	numExecs = 0;
	displayFreq = freq;
	maxStackDepth = 0;
	totalStackDepth = 0;
	maxContextSwitches = 0;
	totalContextSwitches = 0;
	numNlb = 0;
	maxNumThreads = 0;
	numHbExecs = 0;
	numStates = 0;
	numNonterminatingExecs = 0;
	startTimeValid = false;
	ssEstimator = new SSEstimator();
	//*GetChessErrorStream() << "Chess engine started..." << std::endl;
}

StatsMonitor::~StatsMonitor(){
	delete ssEstimator;
	ssEstimator = 0;
}

int StatsMonitor::GetElapsedTimeMS(){
	return GetTickCount()-startTime;
}


void StatsMonitor::OnExecutionEnd(IChessExecution *iexec){
	//ChessExecution* exec = (ChessExecution*)iexec; // XXX: not using dynamic_cast
	numExecs++;
	
	if(Chess::GetOptions().depth_bound && iexec->NumTransitions() > 32){
		numNonterminatingExecs++;
	}

	size_t depth = iexec->NumTransitions()-iexec->GetInitStack();
	if(depth > maxStackDepth)
		maxStackDepth = depth;
	totalStackDepth += depth;
	
	int numThreads = 1;
	int numContextSwitches = 0;
	for(size_t i = iexec->GetInitStack(); i<iexec->NumTransitions(); i++){
		if(iexec->Transition(i).op == SVOP::TASK_BEGIN){
			numThreads ++;
		}
		if(i > 0){
			if(iexec->Transition(i-1).tid != iexec->Transition(i).tid)
				numContextSwitches ++;
		}
	}

	if(numThreads > maxNumThreads)
		maxNumThreads = numThreads;

	if(numContextSwitches > maxContextSwitches)
		maxContextSwitches = numContextSwitches;
	totalContextSwitches += numContextSwitches;

	ssEstimator->OnExecutionEnd(iexec, numExecs);

	if(displayFreq == 0){
		displayFreq = 1; //default
		Chess::GetOptions().GetValue("StatsMonitor::initDisplayFrequency", displayFreq);
		if(displayFreq <= 0)
			displayFreq = 1;
	}

	if(numExecs > displayFreq * 10){
		int maxDisplayFrequency = 1000;
		Chess::GetOptions().GetValue("StatsMonitor::maxDisplayFrequency", maxDisplayFrequency);
		if(displayFreq < maxDisplayFrequency){
			displayFreq = 10 * displayFreq;
		}
	}

	if(numExecs % displayFreq == 0){
		PrintStats(iexec);
	}

	// prune search if appropriate

	if (Chess::GetOptions().max_executions && numExecs >= Chess::GetOptions().max_executions)
		ChessImpl::resultsPrinter->PruneBySchedules(numExecs);

	clock_t elapsed = GetTickCount() - startTime;
#if SINGULARITY
	elapsed /= 1000000;
#endif
	if(Chess::GetOptions().max_chess_time &&
		Chess::GetOptions().max_chess_time <= elapsed/1000)
			ChessImpl::resultsPrinter->PruneByTime(Chess::GetOptions().max_chess_time);
}

void StatsMonitor::OnShutdown(){
	int displayFinalStats = 1;
	Chess::GetOptions().GetValue("StatsMonitor::displayStatsAtEnd", displayFinalStats);
	if(displayFinalStats == 0)
		return;

	if(displayFreq && numExecs % displayFreq != 0)
		PrintStats(0, true);
}

void StatsMonitor::OnNonlocalBacktrack(SyncVarOp op){
	numNlb++;
	if(nlbOps.find(op) == nlbOps.end()){
		nlbOps[op] = 0;
	}
	nlbOps[op]++;
}

void StatsMonitor::OnExecutionBegin(IChessExecution* exec){
	//const ChessExecution* exec = (ChessExecution*)iexec; // XXX: not using dynamic_cast
	if(!startTimeValid){
		startTimeValid = true;
		startTime = GetTickCount();
	}

	ssEstimator->OnExecutionBegin(exec);	
}

void StatsMonitor::OnNewHBExecution(){
	numHbExecs++;
}

void StatsMonitor::OnNewState(){
	numStates ++;
}


void StatsMonitor::PrintStats(IChessExecution* exec, bool isFinal){
	*GetChessErrorStream() 
		<< "Tests: " << numExecs << ' '
		<< "Threads: " << maxNumThreads << ' '
		<< "ExecSteps: " << maxStackDepth << ' ';
		;
	if(numStates != 0){
		*GetChessErrorStream() 
			<< "NumStates: " << numStates << ' ';
	}

	if(numNonterminatingExecs != 0){
		*GetChessErrorStream() 
			<< "NumNonterm: " << numNonterminatingExecs << ' ';
	}

	if(Chess::GetOptions().show_nlb || Chess::GetOptions().debug_output_flag){
		*GetChessErrorStream()
			<< "Nlb: " << numNlb;
		for(stdext::hash_map<SyncVarOp, int>::iterator i = nlbOps.begin();
			i != nlbOps.end(); i++){
				if(i->second > 10){
					*GetChessErrorStream() << "(" << SVOP::ToString(i->first) << ' ' << i->second << ")";
				}
		}
		*GetChessErrorStream() << ' ';
	}

	if(exec && Chess::GetOptions().show_progress && numExecs > Chess::GetOptions().show_progress_start){
		ssEstimator->DisplayEstimate(*GetChessErrorStream(), exec, numExecs);
	}

	//if(Chess::GetOptions().show_stacksplit || Chess::GetOptions().debug_output_flag){
	//	*GetChessErrorStream() 
	//		<< "StackSplit: "
	//		<< "[" ;
	//	for(size_t i = 0; i<stackSplit.size(); i++){
	//		if(i != 0) *GetChessErrorStream()<< "+";
	//		*GetChessErrorStream() << stackSplit[i];
	//	}
	//	*GetChessErrorStream() << "] ";
	//}

	if(Chess::GetOptions().show_hbexecs || Chess::GetOptions().debug_output_flag){
		*GetChessErrorStream() << "HBExecs: " << numHbExecs << ' ';
	}	

	if(!Chess::GetOptions().notime){
		clock_t elapsed = GetTickCount() - startTime;
#if SINGULARITY
		elapsed /= 1000000;
#endif
		*GetChessErrorStream() 
			<< "Time: "
			<< (elapsed/1000)
			<< "."
			<< (elapsed%1000);
	}
	*GetChessErrorStream() << std::endl;
}
