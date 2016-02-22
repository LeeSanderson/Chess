/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"
#include "IChessWrapper.h"

void Win32AsyncProcReset();	
void Win32IOCompletionReset();
void Win32TimersReset();
void Win32CriticalSectionReset();
//void Win32SRWLockReset();
//void Win32ConditionVariableReset();
void HeapStatusReset();

class Win32WrapperManager{
public:
	void LoadWrappers();

	void SetInitState(){
		for(size_t i=0; i<wrappers.size(); i++)
			wrappers[i]->SetInitState();
	}

	void Reset(){
		//Win32AsyncProcReset();
#ifndef UNDER_CE
		Win32IOCompletionReset();
		Win32TimersReset();
#endif
		Win32CriticalSectionReset();
		//Win32SRWLockReset();
		//Win32ConditionVariableReset();

		HeapStatusReset();
		for(size_t i=0; i<wrappers.size(); i++)
			wrappers[i]->Reset();
	}

	void TaskEnd(Task tid){
		for(size_t i=0; i<wrappers.size(); i++)
			wrappers[i]->TaskEnd(tid);
	}

	void RegisterWrapper(IChessWrapper* wrapper){
		wrappers.push_back(wrapper);
	}

	bool RegisterTestModule(HMODULE hModule);

private:
	void LoadWrapper(const wchar_t* wrapperDll);
	std::vector<IChessWrapper*> wrappers;
};