/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessBase.h"
#include "ContextSwitch.h"
#include <hash_map>

// This is just a friendlier interface to access a given execution, which consists
// of a given leaf node and all of its parents up to the root node.
class ExecutionSummary {
public:
	void Reinitialize(ContextSwitch* endSwitch);
	ContextSwitch* GetEndSwitch() const;
	bool ContainsContextSwitchAtStep(size_t step) const;
	Task GetNextTask(size_t step) const;
	bool RequiresPreemption(size_t step) const;
	void Serialize(std::ostream& f) const;
	bool Deserialize(std::istream& f);

private:
	stdext::hash_map<size_t, ContextSwitch*> m_contextSwitches;
};