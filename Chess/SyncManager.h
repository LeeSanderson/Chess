/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessApi.h"
#include "SyncVar.h"

class CHESS_API SyncManager{
public:
	virtual ~SyncManager(){}
	virtual void Init(Task initTask){}

	virtual void ScheduleTask(Task next, bool atTermination){}

	virtual void TaskEnd(Task tid){}

	virtual void Reset(){}
	virtual void ShutDown(){}
	virtual void SetInitState(){}

	virtual void EnterChess() {}

	virtual void LeaveChess() {}

	virtual bool RenameSymmetricTasks() {return true;}

	virtual void DebugBreak(){}
	virtual bool IsDebuggerPresent(){return false;}
	
	virtual void Sleep(int timeInMs){}

	virtual void Exit(int _Code){}


	virtual bool QueuePeriodicTimer(int timerPeriodInMilliseconds, void (*timerFn)()){return false;}

	virtual bool GetCurrentStackTrace(int n, int m, char* procedure[], char* filename[], int lineno[]){return false;}
	virtual bool GetCurrentStackTrace(int start, int num, __int64 pcs[]){return false;}
	virtual bool GetStackTraceSymbols(__int64 pc, int n, char* procedure, char* filename, int* lineno){return false;}
	virtual bool GetFullyQualifiedTopProcedure(int &n, char* procedure, int maxlen){return false;}

	virtual void GetTaskName(Task tid, char* name, int len) { return; }
	virtual void GetDataVarLabel(void *loc, char *name, int len) {return;}


	virtual __int64 GetCurrentTickCount(){return 0;}
	virtual int ConvertTickCountToMs(__int64 tickCount){return 0;}
};