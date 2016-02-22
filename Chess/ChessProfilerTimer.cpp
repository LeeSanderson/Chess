/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessProfilerTimer.h"
#include "ChessStl.h"
#include "Chess.h"
#include "ChessAssert.h"
#include "ChessStream.h"

#define GLOBAL_TIMER_NAME "_GLOBAL_"

CHESS_API bool ChessProfilerTimer::profileEnabled = false;
ChessProfileGlobal* ChessProfilerTimer::global = 0;

struct TimerStats{
	int numCalls;
	__int64 numTicks;
	TimerStats(){
		numCalls = 0;
		numTicks = 0;
	}
};

class ChessProfileGlobal{
public:
	ChessProfileGlobal(){
		globalTimer = 0;
	}
	void Clear(){
		if(globalTimer)
			delete globalTimer;

		stdext::hash_map<std::string, TimerStats*>::iterator i;
		for(i=timers.begin(); i!=timers.end(); i++){
			//delete i->second;
			i->second->numCalls = 0;
			i->second->numTicks = 0;
		}
		//timers.clear();
	}

	void Print(){
		stdext::hash_map<std::string, TimerStats*>::iterator i;
		*GetChessOutputStream() << "================ Chess Profile Timers ==================\n";
		*GetChessOutputStream() 
			<< std::setw(30) << "Timer Name" << " "  
			<< std::setw(10) << " Total (ms)" << " "
			<< std::setw(10) << " #Count" <<  " "
			<< std::setw(10) << " Avg (ms)"
			<< "\n";
		for(i=timers.begin(); i!=timers.end(); i++){
			int totalMs = Chess::GetSyncManager()->ConvertTickCountToMs(i->second->numTicks);
			*GetChessOutputStream() 
				<< std::setw(30) << i->first << " "
				<< std::setw(10) << totalMs << " "
				<< std::setw(10) << i->second->numCalls << " "
				<< std::setw(10) << (totalMs*1.0)/(i->second->numCalls)
				<< '\n';
		}
		(*GetChessOutputStream()).flush();
		
	}
	stdext::hash_map<std::string, TimerStats*> timers;
	ChessProfilerTimer* globalTimer;
};



bool ChessProfilerTimer::IsProfileEnabled(){
	return profileEnabled;
}
void ChessProfilerTimer::EnableProfiling(){
	profileEnabled = true;
	if(global == 0)
		global = new ChessProfileGlobal();
	else
		global->Clear();
	global->globalTimer = new ChessProfilerTimer(GLOBAL_TIMER_NAME);
	global->globalTimer->Start();
}

void ChessProfilerTimer::DisableProfiling(){
	global->globalTimer->Stop();
	profileEnabled = false;
}

ChessProfilerTimer::ChessProfilerTimer(const char* n){
	startCount = 0;
	name = n;
	if(global == 0)
		global = new ChessProfileGlobal();
}

void ChessProfilerTimer::InternalStart(){
	if(startCount != 0)
		assert(!"ChessProfilerTimer started without being stopped or cancelled");

	startCount = Chess::GetSyncManager()->GetCurrentTickCount();
}

void ChessProfilerTimer::InternalCancel(){
	startCount = 0;
}

void ChessProfilerTimer::InternalStop(){
	if(startCount <= 100){
		assert(!"ChessProfilerTimer stopped without being started");
	}
	__int64 endCount = Chess::GetSyncManager()->GetCurrentTickCount();
	endCount -= startCount;
	startCount = 0;

	if(global->timers.find(name) == global->timers.end()){
		global->timers[name] = new TimerStats();
	}	
	global->timers[name]->numCalls++;
	global->timers[name]->numTicks += endCount;
}

void ChessProfilerTimer::PrintProfileTimers(){
	if(profileEnabled){
		DisableProfiling();
		global->Print();
	}
}