/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessStl.h"
#include "ExecutionSummary.h"
#include <map>

// This is just a faster access, convenient way to represent one particular path from the root of the tree to
// a leaf, which represents a given execution.  Only one of these exists at a time and it is maintained by
// the BestFirstExecution and used for fast easy access to the context switches in the execution it is 
// executing.  This is also the class that really does the Serialize/Deserialize work for replay for
// BestFirstExecutions.

void ExecutionSummary::Reinitialize(ContextSwitch* endSwitch) {
	m_contextSwitches.clear();
	ContextSwitch* curr = endSwitch;
	while (curr != NULL) {
		m_contextSwitches[curr->GetStep()] = curr;
		curr = curr->GetParent();
	}
}

bool ExecutionSummary::ContainsContextSwitchAtStep(size_t step) const { 
	return m_contextSwitches.find(step) != m_contextSwitches.end(); 
}

Task ExecutionSummary::GetNextTask(size_t step) const {
	assert(ContainsContextSwitchAtStep(step));
	return m_contextSwitches.find(step)->second->GetTask();
}

bool ExecutionSummary::RequiresPreemption(size_t step) const {
	assert(ContainsContextSwitchAtStep(step));
	return m_contextSwitches.find(step)->second->RequiresPreemption();
}

ContextSwitch* ExecutionSummary::GetEndSwitch() const {
	assert(!m_contextSwitches.empty());
	stdext::hash_map<size_t, ContextSwitch*>::const_iterator itr = m_contextSwitches.begin();
	size_t max = itr->first;
	ContextSwitch* endSwitch = itr->second;
	for (itr; itr != m_contextSwitches.end(); itr++) {
		if (itr->first > max) {
			endSwitch = itr->second;
			max = itr->first;
		}
	}
	assert(endSwitch);
	return endSwitch;
}

void ExecutionSummary::Serialize(std::ostream& f) const{
	f << "// Context Switches:  <num> <cs_1> <cs_2> ... <cs_num> \n" ;
	f << m_contextSwitches.size() << '\n';
	stdext::hash_map<size_t, ContextSwitch*>::const_iterator itr = m_contextSwitches.begin();
	for (itr; itr != m_contextSwitches.end(); itr++) {
		f << itr->first << ' ' << itr->second->GetTask() << ' ' << itr->second->RequiresPreemption() << '\n';
	}
}

bool ExecutionSummary::Deserialize(std::istream& f)
{
	std::string junk;
	int num;
	getline(f, junk);
	getline(f, junk);
	f >> num;
	if(!f.good() || num < 0) return false;
	m_contextSwitches.clear();
	assert(m_contextSwitches.size() == 0);
	// this is a memory leak.  I should fix that.  Small one though.
	std::map<size_t, std::pair<Task, bool> > tempMap;
	for(int i=0; i<num; i++){
		size_t step;
		Task tid;
		bool preempts;
		f >> step;
		f >> tid;
		f >> preempts;
		tempMap.insert(std::pair<size_t, std::pair<Task, bool> >(step, std::pair<Task, bool>(tid, preempts)));
	}
	std::map<size_t, std::pair<Task, bool> >::const_iterator itr = tempMap.begin();
	ContextSwitch* parent = new QueryableContextSwitch();
	for (itr; itr != tempMap.end(); itr++) {
		if (itr != tempMap.begin()) {
			parent->CreateSuccessor(itr->first, itr->second.first, itr->second.second, parent);
		}
		assert(parent);
		m_contextSwitches.insert(std::pair<size_t, ContextSwitch*>(itr->first, parent));
	}

	if(!f.good()) return false;

	return true;
}