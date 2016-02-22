/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "BitVector.h"
#include <deque>
#include <time.h>
#include <map>
#include "HBExecution.h"
#include "ChessImpl.h"

class SleepSets;
class BestFirstExecution;

// This is the interface for a priority function for a best first search.  The GetPriority method
// returns the priority for executing Task next at bstep in exec.
class PriorityFunction {
public:
	virtual ~PriorityFunction() {}
	virtual size_t GetInitialPriority() { return 0; }
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {return 0;}
	virtual bool RequiresHbTimestampRecording() const { return false; }
	virtual bool RequiresSleepSets() const { return false; }
	virtual bool IsMonotonic() const { return true; }
	virtual void CompletedExecution(BestFirstExecution* exec) {}
	virtual bool UsesDpor() const { return false; }
	virtual bool UsesWeightedDpor() const { return false; }
	static PriorityFunction* CreatePriorityFunction(const char* priorityFunctionName);
	static PriorityFunction* CreatePriorityFunction(const char* priorityFunctionName, size_t numBits);

	// the strings used to identify priority functions at the command line
	static const char* FEWER_PREEMPTIONS;
	static const char* DPOR;
	static const char* WDPOR;
	static const char* BREADTH_FIRST;
	static const char* CONSTANT;
	static const char* DEPTH_FIRST;
	static const char* MORE_PREEMPTIONS;
	static const char* PREEMPTIONS_FIRST;
	static const char* RANDOM;
	static const char* SLEEP_SETS;
	static const char* PRIORITIZE_METHODS;
	static const char* PRIORITIZE_VARS;
	static const char* OPTYPE;
	static const char* CLOCK_VECTOR;
	static const char* DATA;
	static const char* DELIMITER;

	enum Priority {
		HIGHEST,
		HIGH,
		MEDIUM,
		LOW,
		LOWEST,
	};
};

class ConstantPriorityFunction : public PriorityFunction {};

class RandomPriorityFunction : public PriorityFunction {
public:
	RandomPriorityFunction(size_t mask) {
		srand((int)time(NULL));
		m_mask = mask;
	}
	virtual size_t GetInitialPriority() { return rand() & m_mask; }
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next) { return rand() & m_mask; }
	virtual bool IsMonotonic() const { return false; }
private:
	size_t m_mask;
};

class BreadthFirstPriorityFunction : public PriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next) { return bstep; }
};

class NumPreemptionsPriorityFunction : public PriorityFunction {
public:
	void CompletedExecution(BestFirstExecution* exec);
protected:
	size_t NumPreemptionsAtStep(size_t step);
private:
	std::vector<size_t> m_preemptionSteps;
};

class FewerPreemptionsFirstPriorityFunction : public NumPreemptionsPriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
};

class MorePreemptionsFirstPriorityFunction : public NumPreemptionsPriorityFunction {
public:
	MorePreemptionsFirstPriorityFunction(size_t maxValue) : m_maxValue(maxValue) {}
	virtual size_t GetInitialPriority() { return m_maxValue; }
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
private:
	size_t m_maxValue;
};

class PreemptionsFirstPriorityFunction : public PriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
};

class DepthFirstPriorityFunction : public PriorityFunction {
public:
	DepthFirstPriorityFunction(size_t maxValue) : m_maxValue(maxValue) {}
	virtual size_t GetInitialPriority() { return m_maxValue; }
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next) { return m_maxValue - bstep; }
	virtual bool IsMonotonic() const { return false; }

private:
	size_t m_maxValue;
};

class SleepSetPriorityFunction : public PriorityFunction {
public:
	SleepSetPriorityFunction();
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
	virtual bool RequiresSleepSets() const { return true; }

protected:
	SleepSets* m_sleepSets;
};

class DPORPriorityFunction : public PriorityFunction {
public:
	DPORPriorityFunction() {}
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
	virtual bool RequiresHbTimestampRecording() const { return true; }
	virtual bool UsesDpor() const { return true; }
};

class WeightedDPORPriorityFunction : public PriorityFunction {
public:
	WeightedDPORPriorityFunction() {}
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
	virtual bool RequiresHbTimestampRecording() const { return true; }
	virtual bool UsesDpor() const { return true; }
	virtual bool UsesWeightedDpor() const { return true; }
};

class OpTypePriorityFunction : public PriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
};

// This priority function returns 1 if the call stack at a preemption point includes a method specified
// by the user at the command line.
class PrioritizedMethodsPriorityFunction : public PriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
};

class PrioritizedVarsPriorityFunction : public PriorityFunction {
public:
	PrioritizedVarsPriorityFunction() : m_svm(ChessImpl::GetSyncVarManager()), m_var(ChessImpl::GetOptions().prioritized_var) {}
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
private:
	SyncVarManager* m_svm;
	const SyncVar m_var;
};

class DataAccessPriorityFunction : public PriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool IsMonotonic() const { return false; }
};

class ClockVectorPriorityFunction : public PriorityFunction {
public:
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool RequiresHbTimestampRecording() const { return true; }
	virtual bool IsMonotonic() const { return false; }
	virtual void CompletedExecution(BestFirstExecution* exec);
private:
	std::vector<BitVector> m_backtrackingPoints;
};

// This allows you to combine any of the other priority functions.  Each priority function gets an equal share
// of 32 bits.  The ones furthest to the left have the highest priority.
class HierarchicalPriorityFunction : public PriorityFunction {
public:
	HierarchicalPriorityFunction(size_t numLevels);
	~HierarchicalPriorityFunction();
	// overrides
	virtual size_t GetInitialPriority();
	virtual size_t GetPriority(BestFirstExecution* exec, size_t bstep, Task next);
	virtual bool RequiresHbTimestampRecording() const;
	virtual bool IsMonotonic() const;
	virtual bool RequiresSleepSets() const;
	virtual void CompletedExecution(BestFirstExecution* exec);
	virtual bool UsesDpor() const;
	virtual bool UsesWeightedDpor() const;

	void AddPriorityFunction(PriorityFunction* cf);
	size_t GetHighestPrioPriorityMask() const { return m_highestPrioPriorityMask; }
	size_t GetPriorityMask() const { return m_priorityMask; }

private:
	size_t m_numLevels;
	size_t m_priorityMask;
	size_t m_highestPrioPriorityMask;
	size_t m_bitsPerLevel;
	std::deque<PriorityFunction*> m_priorityFunctions;
};

// for bounding a search based on a priority function
class PriorityBound {
public:
	PriorityBound(PriorityFunction* priorityFunction, size_t bound) 
		: m_priorityFunction(priorityFunction), m_bound(bound) {}
	PriorityFunction* GetPriorityFunction() const { return m_priorityFunction; }
	bool IsPrunable(BestFirstExecution* exec, size_t step, Task tid) { return m_priorityFunction->GetPriority(exec, step, tid) > m_bound; }
	
private:
	size_t m_bound;
	PriorityFunction* m_priorityFunction;
};