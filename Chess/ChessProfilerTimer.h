/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessApi.h"
class ChessProfileGlobal;

class CHESS_API ChessProfilerTimer{
public:
	static void EnableProfiling();
	static void DisableProfiling();
	static bool IsProfileEnabled();

	static void PrintProfileTimers();
	
	ChessProfilerTimer(const char* name);


	void Start(){if(IsProfileEnabled()) InternalStart();}
	void Stop(){if(IsProfileEnabled()) InternalStop();}
	void Cancel(){if(IsProfileEnabled()) InternalCancel();}

	
private:
	static bool profileEnabled;
	static ChessProfileGlobal* global;

	void InternalStart();
	void InternalStop();
	void InternalCancel();
	__int64 startCount;
	const char* name;
};

class ChessProfilerSentry{
public:
	ChessProfilerSentry(const char* n)
		: t(n){
			t.Start();
	}
	~ChessProfilerSentry(){
		t.Stop();
	}
	void Start(){ t.Start();}
	void Stop() { t.Stop(); }
private:
	ChessProfilerTimer t;
};