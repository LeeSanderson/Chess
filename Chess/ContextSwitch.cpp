/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ContextSwitch.h"

#ifdef STATS
size_t ContextSwitch::ms_numCreated = 0;
size_t ContextSwitch::ms_numDestroyed = 0;
size_t ContextSwitch::ms_numDetatched = 0;
#endif


ContextSwitch::ContextSwitch(size_t step, Task tid, bool requiresPreemption, ContextSwitch* parent) 
	: m_step(step), 
	m_tid(tid), 
	m_requiresPreemption(requiresPreemption), 
	m_parent(parent) {
# ifdef STATS
	ms_numCreated++;
#endif
}

ContextSwitch* ChildIterator::Next() {
	assert (m_itr != m_end);
	assert (m_index < m_itr->second.size());
	ContextSwitch* ret = m_itr->second[m_index];
	m_index++;
	if (m_index >= m_itr->second.size()) {
		m_index = 0;
		m_itr++;
	}
	if (m_itr != m_end && m_itr->second[m_index]->GetParent() == NULL) {
		Next();
	}
	return ret;
}

bool QueryableContextSwitch::CreateSuccessor(size_t step, Task tid, bool preempts, ContextSwitch*& child) {
	child = new QueryableContextSwitch(step, tid, preempts, this);
	m_numSuccessors++;
	return true;
}

void QueryableContextSwitch::Detatch() {
	assert (m_numSuccessors == 0);
	if (m_parent != NULL) {
		static_cast<QueryableContextSwitch*>(m_parent)->m_numSuccessors--;
		if (!m_parent->HasSuccessors()) {
			m_parent->Detatch();
		}
		delete this;
	}
}

bool EnumerableContextSwitch::RemoveChild(ContextSwitch* cs) {
	size_t step = cs->GetStep();
	if (m_children.upper_bound(step) == m_children.end()) {
		// this is the largest step so we may be able to delete stuff
		std::map<size_t, std::vector<ContextSwitch*>>::iterator itr = m_children.lower_bound(step);
		assert (itr->first == step);
		size_t size = itr->second.size();
		for (size_t i = 0; i < size; i++) {
			if (itr->second[i] != cs && itr->second[i]->GetParent() != NULL) {
				// There's a live sibling, so we can't delete yet, as executing that sibling may
				// convince us to execute this execution again and thus create a duplicate node
				return false;
			}
		}
		// ok to delete this entire step's backtracking points
		while (!itr->second.empty()) {
			ContextSwitch* back = itr->second.back();
			itr->second.pop_back();
			if (back->GetParent() == NULL) {
				delete back;
			}
		}
		assert (itr->second.empty());
		m_children.erase(itr);
		// erase more children, if we can
		while (m_children.end() != m_children.begin()) {
			itr = m_children.end();
			itr--;
			size_t size = itr->second.size();
			for (size_t i = 0; i < size; i++) {
				if (itr->second[i]->GetParent() != NULL) {
					return true;
				}
			}
			while (!itr->second.empty()) {
				ContextSwitch* back = itr->second.back();
				itr->second.pop_back();
				delete back;
			}
			assert (itr->second.empty());
			m_children.erase(itr);
		}
		return true;
	}
	// this is not the largest step, which means a larger step may execute in the future and recreate
	// this node, and we can't let that happen or we'll repeat executions (possibly forever).  So, not
	// ok to delete this node yet.
	return false;
}

// detatches the entire subtree under cs, but leaves cs in place
void EnumerableContextSwitch::DetatchSubtree() {
	while (HasSuccessors()) {
		std::map<size_t, std::vector<ContextSwitch*>>::iterator itr = m_children.end();
		itr--;
		while (!itr->second.empty()) {
			ContextSwitch* back = itr->second.back();
			itr->second.pop_back();
			if (back->HasSuccessors()) {
				back->DetatchSubtree();
			}
			assert (!back->HasSuccessors());
			delete back;
		}
		m_children.erase(itr);
	}
}

void EnumerableContextSwitch::Detatch() {
#ifdef STATS
	ms_numDetatched++;
#endif
	assert (m_children.empty());
	if (m_parent && static_cast<EnumerableContextSwitch*>(m_parent)->RemoveChild(this)) {
		if (!m_parent->HasSuccessors()) {
			m_parent->Detatch();
		}
#ifdef STATS
		ms_numDestroyed++;
#endif
		delete this;
	} else {
		m_parent = NULL;
	}
}

bool EnumerableContextSwitch::CreateSuccessor(size_t step, Task tid, bool preempts, ContextSwitch*& child) {
	std::map<size_t, std::vector<ContextSwitch*>>::iterator lb = m_children.lower_bound(step);
	if (lb == m_children.end()) {
		child = new EnumerableContextSwitch(step, tid, preempts, this);
		m_children[step].push_back(child);
		return true;
	}
	if (lb->first == step) {
		size_t size = lb->second.size();
		for (size_t i = 0; i < size; i++) {
			if (lb->second[i]->GetTask() == tid) {
				// duplicate node!
				child = lb->second[i];
				return false;
			}
		}
	}
	// either creating a new step/list, or adding a new entry to the list at step
	child = new EnumerableContextSwitch(step, tid, preempts, this);
	m_children[step].push_back(child);
	return true;
}