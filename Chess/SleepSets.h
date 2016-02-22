/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "BitVector.h"
#include "HashFunction.h"
#include "BestFirstExecution.h"
#include "PriorityFunction.h"
#include <map>

class SleepSets {
public:
	static SleepSets* GetInstance() {
		if (ms_instance == NULL) {
			ms_instance = new SleepSets();
		}
		assert (ms_instance != NULL);
		return ms_instance;
	}
	bool InSleepSet(BestFirstExecution* exec, size_t step, Task tid);
	void SetPriorityBound(PriorityBound* priorityBound) { m_priorityBound = priorityBound; }
	void CompletedExecution(BestFirstExecution* curr);
	void Clear();

private:
	static SleepSets* ms_instance;
	SleepSets();
	bool IsVisitorTaskDisabled(BestFirstExecution* exec, size_t step);

	struct SleepSetInfo {
		SleepSetInfo() : m_priority(0) {}
		SleepSetInfo(size_t priority, bool isVisitorTaskDisabled) : m_priority(priority), m_isVisitorTaskDisabled(isVisitorTaskDisabled) {}

		size_t m_priority;
		bool m_isVisitorTaskDisabled;
	};
	std::map<HashValue, SleepSetInfo> m_infoForState;
	PriorityBound* m_priorityBound;
	size_t m_depthBound;
	bool m_prune;
};