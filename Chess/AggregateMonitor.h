/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"
#include "IChessMonitor.h"
#include "ChessProfilerTimer.h"

class AggregateMonitor : IChessMonitor{
public:
	AggregateMonitor()
		:monitorEnabled(false){}
	~AggregateMonitor(){
		for(std::vector<IChessMonitor*>::iterator it = monitors.begin(); it != monitors.end(); ++it) {
			delete *it;
		}
		std::vector<IChessMonitor*>().swap( monitors );
	}
	void OnExecutionBegin(IChessExecution* exec){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnExecutionBegin(exec);
		}
	}

	void OnExecutionEnd(IChessExecution* exec){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnExecutionEnd(exec);
		}
	}

	void OnShutdown(){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++) {
				if (monitors[i]->GetCallOnShutdown()) {
					monitors[i]->OnShutdown();
				}
			}
		}
	}

	void OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnDataVarAccess(id, loc, size, isWrite, pcId);
		}
	}

	void OnTraceEvent(EventId id, const std::string &evtype) {
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnTraceEvent(id, evtype);
		}
	}

	void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnSyncVarAccess(id, tid, var, op, sid);
		}
	}

	void OnAggregateSyncVarAccess(EventId id, Task tid, const SyncVar* var, int n, SyncVarOp op, size_t sid){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnAggregateSyncVarAccess(id, tid, (SyncVar*)var, n, op, sid);
		}
	}

	void OnSchedulePoint(SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid){
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnSchedulePoint(type, id, var, op, sid);
		}
	}

	void OnEventAttributeUpdate(EventId id, EventAttribute attr, const char *val) {
		if(monitorEnabled){
			ChessProfilerSentry sentry("ChessMonitors");
			for(size_t i=0; i<monitors.size(); i++)
				monitors[i]->OnEventAttributeUpdate(id, attr, val);
		}
	}

	void EnabledMonitors(){
		monitorEnabled = true;
	}

	void DisableMonitors(){
		monitorEnabled = false;
	}

	void RegisterMonitor(IChessMonitor *mon, bool atHead){
		if(atHead){
			monitors.insert(monitors.begin(), mon);
		}
		else{
			monitors.push_back(mon);
		}
	}



private:
	bool monitorEnabled;
	std::vector<IChessMonitor*> monitors;

};