/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "PriorityFunction.h"
#include "CacheRaceMonitor.h"
#include "ChessImpl.h"
#include "SleepSets.h"
#include <hash_set>
#include "EnabledSet.h"
#include "Dpor.h"

// the strings used to identify priority functions at the command line
const char* PriorityFunction::FEWER_PREEMPTIONS = "pb";
const char* PriorityFunction::DPOR = "dpor";
const char* PriorityFunction::WDPOR = "wdpor";
const char* PriorityFunction::BREADTH_FIRST = "bf";
const char* PriorityFunction::CONSTANT = "constant";
const char* PriorityFunction::DEPTH_FIRST = "df";
const char* PriorityFunction::MORE_PREEMPTIONS = "mp";
const char* PriorityFunction::PREEMPTIONS_FIRST = "preempts";
const char* PriorityFunction::RANDOM = "random";
const char* PriorityFunction::SLEEP_SETS = "ss";
const char* PriorityFunction::PRIORITIZE_METHODS = "method";
const char* PriorityFunction::PRIORITIZE_VARS = "var";
const char* PriorityFunction::OPTYPE = "optype";
const char* PriorityFunction::CLOCK_VECTOR = "cv";
const char* PriorityFunction::DATA = "data";
const char* PriorityFunction::DELIMITER = "_";

SleepSetPriorityFunction::SleepSetPriorityFunction() : m_sleepSets(SleepSets::GetInstance()) {}

size_t SleepSetPriorityFunction::GetPriority(BestFirstExecution* exec, size_t step, Task tid) {
	if (exec->Transition(step).tid == tid) {
		return LOWEST;
	}
	assert(m_sleepSets);
	return m_sleepSets->InSleepSet(exec, step, tid) ? LOWEST : HIGHEST;
}

size_t DPORPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	return (exec->Transition(bstep).tid == next || !Dpor::GetInstance()->Contains(bstep, next)) ? LOWEST : HIGHEST;
}

size_t WeightedDPORPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	if (exec->Transition(bstep).tid == next || !Dpor::GetInstance()->Contains(bstep, next)) {
		return LOWEST;
	}
	if (Dpor::GetInstance()->NotConservative(bstep, next)) {
		return Dpor::GetInstance()->NoMatchingAcquire(bstep, next) ? HIGHEST : MEDIUM;
	}
	return (Dpor::GetInstance()->NoMatchingAcquire(bstep, next) && Dpor::GetInstance()->FairBlocks(bstep, next)) ? HIGHEST : MEDIUM;
}

size_t NumPreemptionsPriorityFunction::NumPreemptionsAtStep(size_t step) {
	size_t size = m_preemptionSteps.size();
	if (size == 0) {
		return 0;
	}
	size_t i = size;
	do {
		i--;
		if (m_preemptionSteps[i] > step) {
			return size - (i + 1);
		}
	} while (i != 0);
	return size;
}

void NumPreemptionsPriorityFunction::CompletedExecution(BestFirstExecution* exec) {
	m_preemptionSteps.clear();
	ContextSwitch* cs = exec->GetEndSwitch();
	while (cs != NULL) {
		if (cs->RequiresPreemption()) {
			m_preemptionSteps.push_back(cs->GetStep());
		}
		cs = cs->GetParent();
	}
}

size_t FewerPreemptionsFirstPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) { 
	return NumPreemptionsPriorityFunction::NumPreemptionsAtStep(bstep-1) + (exec->RequiresPreemption(bstep, next) ? 1 : 0); 
}

size_t MorePreemptionsFirstPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	return m_maxValue - NumPreemptionsPriorityFunction::NumPreemptionsAtStep(bstep-1) - (exec->RequiresPreemption(bstep, next) ? 1 : 0);
}

size_t PreemptionsFirstPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	return exec->RequiresPreemption(bstep, next) ? HIGHEST : LOWEST;
}

HierarchicalPriorityFunction::HierarchicalPriorityFunction(size_t numLevels) : m_numLevels(numLevels) {
	size_t totalBits = 8*sizeof(size_t);
	m_bitsPerLevel = totalBits/numLevels;
	m_priorityMask = 0xFFFFFFFF >> (totalBits - m_bitsPerLevel);
	m_highestPrioPriorityMask = 0xFFFFFFFF << (m_bitsPerLevel*(numLevels-1));
}

HierarchicalPriorityFunction::~HierarchicalPriorityFunction() {
	assert(m_priorityFunctions.size() == m_numLevels);
	for (size_t i = 0; i < m_numLevels; i++) {
		delete m_priorityFunctions[i];
	}
	m_priorityFunctions.clear();
}

size_t HierarchicalPriorityFunction::GetInitialPriority() {
	size_t initPriority = 0;
	for (size_t i = 0; i < m_numLevels; i++) {
		size_t myInitPriority = m_priorityFunctions[i]->GetInitialPriority();
		assert((myInitPriority & m_priorityMask) == myInitPriority);
		initPriority |= (myInitPriority & m_priorityMask) << (m_bitsPerLevel * i);
	}
	return initPriority;
}

size_t HierarchicalPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	assert(m_priorityFunctions.size() == m_numLevels);
	size_t hierPriority = 0;
	for (size_t i = 0; i < m_numLevels; i++) {
		size_t priority = m_priorityFunctions[i]->GetPriority(exec, bstep, next);
		assert((priority & m_priorityMask) == priority);
		hierPriority |= (priority & m_priorityMask) << (m_bitsPerLevel * i);
	}
	return hierPriority;
}

bool HierarchicalPriorityFunction::RequiresHbTimestampRecording() const {
	assert(m_priorityFunctions.size() == m_numLevels);
	for (size_t i = 0; i < m_numLevels; i++) {
		if (m_priorityFunctions[i]->RequiresHbTimestampRecording()) {
			return true;
		}
	}
	return false;
}

bool HierarchicalPriorityFunction::IsMonotonic() const {
	assert(m_priorityFunctions.size() == m_numLevels);
	assert(m_priorityFunctions.size() > 0);
	return m_priorityFunctions[0]->IsMonotonic();
}

bool HierarchicalPriorityFunction::RequiresSleepSets() const {
	for (size_t i = 0; i < m_numLevels; i++) {
		if (m_priorityFunctions[i]->RequiresSleepSets()) {
			return true;
		}
	}
	return false;
}

bool HierarchicalPriorityFunction::UsesDpor() const {
	for (size_t i = 0; i < m_numLevels; i++) {
		if (m_priorityFunctions[i]->UsesDpor()) {
			return true;
		}
	}
	return false;
}

bool HierarchicalPriorityFunction::UsesWeightedDpor() const {
	for (size_t i = 0; i < m_numLevels; i++) {
		if (m_priorityFunctions[i]->UsesWeightedDpor()) {
			return true;
		}
	}
	return false;
}

// Insert them at the beginning so that the order in which you write them at the command
// line is the order of precedence.
void HierarchicalPriorityFunction::AddPriorityFunction(PriorityFunction* cf) {
	assert(m_priorityFunctions.size() < m_numLevels);
	m_priorityFunctions.push_front(cf);
}

void HierarchicalPriorityFunction::CompletedExecution(BestFirstExecution* exec) {
	for (size_t i = 0; i < m_numLevels; i++) {
		m_priorityFunctions[i]->CompletedExecution(exec);
	}
}

size_t PrioritizedMethodsPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	size_t step = bstep;
	bool bstepPrioritized = exec->IsPrioritizedStep(bstep);
	while (exec->Transition(step).tid != next) {
		step++;
		assert(step < exec->NumTransitions());
	}
	while (step < exec->NumTransitions() && exec->Transition(step).tid == next) {
		if (exec->IsPrioritizedStep(step)) {
			return bstepPrioritized ? HIGHEST : MEDIUM;
		}
		step++;
	}
	return bstepPrioritized ? MEDIUM : LOWEST;
}

size_t PrioritizedVarsPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	SyncVar var = exec->Transition(bstep).var;
	if (m_svm->IsAggregate(var)) {
		size_t size = m_svm->GetAggregateVectorSize(var);
		const SyncVar* varVec = m_svm->GetAggregateVector(var);
		for (size_t i = 0; i < size; i++) {
			if (m_var == varVec[i]) {
				return HIGHEST;
			}
		}
		return LOWEST;
	} else {
		return m_var == var ? HIGHEST : LOWEST;
	}
}

size_t OpTypePriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	switch (exec->Transition(bstep).op) {
		case SVOP::LOCK_ACQUIRE:
		case SVOP::LOCK_TRYACQUIRE:
		case SVOP::WAIT_ANY:
		case SVOP::RWVAR_READ:
		case SVOP::RWVAR_READWRITE:
		case SVOP::RWVAR_WRITE:
			return HIGHEST;
		case SVOP::RWEVENT:
		case SVOP::LOCK_RELEASE:
			return MEDIUM;
		default:
			return LOWEST;
	}
}

size_t DataAccessPriorityFunction::GetPriority(BestFirstExecution *exec, size_t bstep, Task next) {
	const SyncVarOp op = exec->Transition(bstep).op;
	return op == SVOP::DATA_READ || op == SVOP::DATA_WRITE;
}

void ClockVectorPriorityFunction::CompletedExecution(BestFirstExecution* exec) {
	m_backtrackingPoints.clear();
	m_backtrackingPoints.resize(exec->NumTransitions());
	CacheRaceMonitor* rm = ChessImpl::GetRaceMonitor();
	const size_t numTrans = exec->NumTransitions();
	TaskVector<int> prevValue;
	const size_t numThreads = ChessImpl::NumThreads();
	std::map<EventId, size_t> stepMap;
	SyncVarVector<TaskVector<size_t>> prevAcquire;
	size_t bstep = exec->GetRecordIndex();
	for (size_t i = 0; i < numTrans; i++) {
		const Task tid = exec->Transition(i).tid;
		const EventId id = EventId(tid, exec->GetPerThreadStepNum(i));
		stepMap[id] = i;
		Timestamp currTs = rm->get_hbstamp(id);
		for (size_t j = 1; j < numThreads; j++) {
			if (j == tid) {
				continue;
			}
			const size_t currElement = rm->get_hbstamp_element(currTs, j);
			if (prevValue[j] != currElement && currElement > 0) {
				const EventId jid = EventId(j, currElement);
				assert (stepMap.find(jid) != stepMap.end());
				size_t step = stepMap[jid];
				const SyncVarOp op = exec->Transition(step).op;
				if (op == SVOP::LOCK_RELEASE) {
					const size_t prevAcq = prevAcquire[exec->Transition(step).var][j];
					step = prevAcq ? prevAcq : step;
				}
				if (i >= bstep) {
					m_backtrackingPoints[step].Set(tid, true);
				}
				prevValue[j] = currElement;
			}
		}
		const SyncVarOp op = exec->Transition(i).op;
		if (op == SVOP::LOCK_ACQUIRE || op == SVOP::LOCK_TRYACQUIRE) {
			prevAcquire[exec->Transition(i).var][tid] = i;
		}
	}
	ContextSwitch* cs = exec->GetEndSwitch();
	// go up to the next to the root node, don't go all the way up to the root node because it's step is 0.
	while (cs->GetParent() != NULL) {
		const Task csTid = cs->GetTask();
		const size_t csStep = cs->GetStep();
		assert (csTid == exec->Transition(csStep).tid);
		BitVector varsTouched;
		const Task interruptedTask = exec->Transition(csStep-1).tid;
		for (size_t i = cs->GetStep(); exec->Transition(i).tid == csTid; i++) {
			const SyncVar var = exec->Transition(i).var;
			if (m_backtrackingPoints[i].Get(interruptedTask) && !varsTouched.Contains(var)) {
				m_backtrackingPoints[i].Set(interruptedTask, false);
				break;
			}
			if (!m_backtrackingPoints[i].IsEmpty()) {
				break;
			}
			varsTouched.insert(var);
		}
		cs = cs->GetParent();
	}
}

size_t ClockVectorPriorityFunction::GetPriority(BestFirstExecution* exec, size_t bstep, Task next) {
	return m_backtrackingPoints[bstep].Contains(next) ? HIGHEST : LOWEST;
}

PriorityFunction* PriorityFunction::CreatePriorityFunction(const char* priorityFunctionName) {
	PriorityFunction* priorityFunction;
	size_t len = strlen(priorityFunctionName);
	// find the number of priority functions
	size_t numLevels = 1;
	for (size_t i = 0; i < len; i++) {
		if (priorityFunctionName[i] == '_') {
			numLevels++;
		}
	}	
	if (numLevels > 1) {
		HierarchicalPriorityFunction* hcf = new HierarchicalPriorityFunction(numLevels);
		priorityFunction = hcf;
		// create each priority function and send it to the hierarchical priority function
		char* nextToken;
		char* cpy = new char[strlen(priorityFunctionName) + 1 /* +1 for the NULL character, I think */];
		strcpy_s(cpy, strlen(priorityFunctionName)+1, priorityFunctionName);
		char* cfn = strtok_s(cpy, DELIMITER, &nextToken);
		while (cfn) {
			hcf->AddPriorityFunction(CreatePriorityFunction(cfn, hcf->GetPriorityMask()));
			cfn = strtok_s(NULL, DELIMITER, &nextToken);
		}
	} else {
		priorityFunction = CreatePriorityFunction(priorityFunctionName, 0xFFFFFFFF /* mask for individual priority */);
	}
	return priorityFunction;
}

// If you feel like creating your own priority function, you should be able to just add an implementation of
// it to PriorityFunction.h/PriorityFunction.cpp, and then add an else here that instantiates the appropriate 
// priority function given whatever name you want people to specify at the command line -KEC
PriorityFunction* PriorityFunction::CreatePriorityFunction(const char* priorityFunctionName, size_t bitMask) {
	PriorityFunction* cf;
	if (!strcmp(priorityFunctionName, FEWER_PREEMPTIONS)) {
		cf = new FewerPreemptionsFirstPriorityFunction();
	} else if (!strcmp(priorityFunctionName, DPOR)) {
		cf = new DPORPriorityFunction();
	} else if (!strcmp(priorityFunctionName, WDPOR)) {
		cf = new WeightedDPORPriorityFunction();
	} else if (!strcmp(priorityFunctionName, BREADTH_FIRST)) {
		cf = new BreadthFirstPriorityFunction();
	} else if (!strcmp(priorityFunctionName, CONSTANT)) {
		cf = new ConstantPriorityFunction();
	} else if (!strcmp(priorityFunctionName, DEPTH_FIRST)) {
		cf = new DepthFirstPriorityFunction(bitMask);
	} else if (!strcmp(priorityFunctionName, MORE_PREEMPTIONS)) {
		cf = new MorePreemptionsFirstPriorityFunction(bitMask);
	} else if (!strcmp(priorityFunctionName, PREEMPTIONS_FIRST)) {
		cf = new PreemptionsFirstPriorityFunction();
	} else if (!strcmp(priorityFunctionName, RANDOM)) {
		cf = new RandomPriorityFunction(bitMask);
	} else if (!strcmp(priorityFunctionName, SLEEP_SETS)) {
		cf = new SleepSetPriorityFunction();
	} else if (!strcmp(priorityFunctionName, PRIORITIZE_METHODS)) {
		cf = new PrioritizedMethodsPriorityFunction();
	} else if (!strcmp(priorityFunctionName, PRIORITIZE_VARS)) {
		cf = new PrioritizedVarsPriorityFunction();
	} else if (!strcmp(priorityFunctionName, OPTYPE)) {
		cf = new OpTypePriorityFunction();
	} else if (!strcmp(priorityFunctionName, CLOCK_VECTOR)) {
		cf = new ClockVectorPriorityFunction();
	} else if (!strcmp(priorityFunctionName, DATA)) {
		cf = new DataAccessPriorityFunction();
	} else {
		cf = NULL;
		printf("Invalid priority function name: %s\n", priorityFunctionName);
		exit(1);
	}
	return cf;
}