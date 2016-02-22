/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "IChessMonitor.h"
#include "ChessStl.h"
#include <iostream>
#include <fstream>

#include "ErrorInfo.h"

class ChessOptions;
class ChessExecution;
class StatsMonitor;

class ResultsPrinter : public IChessMonitor
{
public:
	CHESS_API ResultsPrinter(const ChessOptions *options, StatsMonitor *stm);
	virtual CHESS_API ~ResultsPrinter();
	
	virtual CHESS_API void OnExecutionBegin(IChessExecution* exec);
	virtual CHESS_API void OnEventAttributeUpdate(EventId id, EventAttribute attr, const char *val);

	// called by ChessImpl to report warnings or errors
    void CHESS_API AddWarning(const char *description, const char* action, bool includeschedule);
    void CHESS_API AddError(const char* description, const char* action, const ErrorInfo* errorInfo);
    void CHESS_API ReportFinalStatistics(int exitCode);
    void CHESS_API CloseResults();

    // called by components that prune the search, so we know what happened at the end
	void CHESS_API PruneBySchedules(int numschedules);
	void CHESS_API PruneByTime(int milliseconds);
	bool CHESS_API SearchIsPruned();

	// the most general form
	void CHESS_API CaptureResult(char category, 
                       const std::string &description, 
					   const std::string &actions,
					   bool includeschedule,
					   const ErrorInfo* errorInfo
					   );


    void CHESS_API OnShutdown(int exitCode);

private:
	const ChessOptions *options;
	ChessExecution *currExecution;
	StatsMonitor *stm;
	std::ofstream mStream;

	int num_results;

	int race_count;
	int warning_count;
	bool final;

	bool shutdown_called;
	// The last exit code, otherwise Success.
	bool areFinalStatsReported;

    int pruned_schedules;
    int pruned_time;

	void EncodeSchedule();
	void GetTopOfStack();

	// for preemption sealing on method boundaries
	// this gets reset to empty at the beginning of each execution
	std::list<const std::string> preemptionMethods;
};