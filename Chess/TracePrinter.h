/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "IChessMonitor.h"
#include <iostream>
#include <fstream>

class ChessOptions;

class TracePrinter : public IChessMonitor
{
public:
	CHESS_API TracePrinter(const ChessOptions *options);
	virtual CHESS_API ~TracePrinter();

	virtual CHESS_API void OnExecutionBegin(IChessExecution* exec);
	virtual CHESS_API void OnSchedulePoint(SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid);
	virtual CHESS_API void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid);
	virtual CHESS_API void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid);
	virtual CHESS_API void OnDataVarAccess(EventId id, void *loc, int size, bool isWrite, size_t pcId);
	virtual CHESS_API void OnTraceEvent(EventId id, const std::string &info);
	virtual CHESS_API void OnShutdown();
	virtual CHESS_API void OnEventAttributeUpdate(EventId id, EventAttribute attr, const char *val);

private:
	std::ofstream mStream;
	bool mRecordDetails;
	bool mGui;
	int num_execs;

	/** connect to gui process */
	void ConnectGui();
	bool CreateChildProcess();
	void ErrorExit(const char *msg);
	void WriteToPipe();
	void WaitForGui();

	/** the core printing functions */
	void PrintSchedulePoint(IChessMonitor::SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid);
	void PrintVarAccess(EventId id, int transop, int transvar, bool isData, bool isAggregate, size_t sid);
	void PrintTracedEvent(EventId id, const std::string& info);
	void PrintStackTrace(EventId id);

	/** the low-level printing to the file stream */
	void PrintHeader(const char *str);
	void PrintExecutionBegin();
	void PrintTuple(int tid, int nr, int attr, const char* valstr);
	void PrintEnd();

	/** buffer management */
	void flush();
};