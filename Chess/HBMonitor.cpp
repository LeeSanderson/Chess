/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#if SINGULARITY

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif

#endif

#include "HBMonitor.h"
#include "HashFunction.h"
#include "ChessExecution.h"
#include "HBExecution.h"

void HBMonitor::OnExecutionEnd(IChessExecution* exec){
	HBExecution hbexec(exec);
	HashValue h = hbexec.GetHash();
	stdext::hash_set<size_t>::iterator si = hbsets.find(h);
	if(si == hbsets.end()){
		stats->OnNewHBExecution();
		hbsets.insert(h);
	}
}