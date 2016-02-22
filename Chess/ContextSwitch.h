/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessBase.h"
#include "SyncVar.h"
#include <map>
#include <vector>

// A ContextSwitch is a node in the execution tree.  It contains a step, tid and bool representing
// a context switch performed that is not the same as the default record mode action.  A context 
// switch points to its parent, which is the previous non-default context switch performed in that
// execution.  The root node has a tid/step of 0 and represents the initial default execution.  Its
// children represent backtracking points from that execution.  Each of their children represent
// backtracking points from that child execution, and so on.  A node does not keep track of its
// successors, so you can't travel down the tree.  It only keeps track of its parent, and the
// *number* of successors it has.  Once a node has no successors, it is safe to call its
// Detatch method, which will remove it from the tree, and recusively remove any parents from 
// the tree that, after having removed the child, no longer have any children.  This way, the
// tree cleans itself up as soon as the leaves are no longer necessary.  At the end of a run of
// CHESS, you should be left with only the root node with no children, if the tree has been
// maintained properly and you have called Detatch every time a node becomes unnecessary.
class ContextSwitch {
public:
	// accessors
	size_t GetStep() const { return m_step; }
	Task GetTask() const { return m_tid; }
	bool RequiresPreemption() const { return m_requiresPreemption; }
	ContextSwitch* GetParent() const { return m_parent; }

	// creates a new node for <s,t,r> as a child of this node
	virtual bool CreateSuccessor(size_t s, Task t, bool r, ContextSwitch*& child) { return false; }
	// detatches this node from the tree
	virtual void Detatch() {}
	// detatches all of the children of this node, and all of their children, from the tree
	virtual void DetatchSubtree() { assert(false); }
	virtual bool HasSuccessors() const { return false; }

#ifdef STATS
	static size_t NumCreated() { return ms_numCreated; }
	static size_t NumDestroyed() { return ms_numDestroyed; }
	static size_t NumDetatched() { return ms_numDetatched; }
#endif

protected:
	ContextSwitch() : m_step(0), 
		m_tid(0), 
		m_requiresPreemption(false), 
		m_parent(NULL) {}
	ContextSwitch(size_t s, Task t, bool r, ContextSwitch* parent);
	ContextSwitch* m_parent;
#ifdef STATS
	static size_t ms_numCreated;
	static size_t ms_numDestroyed;
	static size_t ms_numDetatched;
#endif

private:
	size_t m_step;
	Task m_tid;
	bool m_requiresPreemption;
};

// This is a context switch node where you can only query it for whether it has children or not;
// you cannot enumerate over its children.
class QueryableContextSwitch : public ContextSwitch {
public:
	QueryableContextSwitch() : ContextSwitch(), m_numSuccessors(0) {}
	virtual bool CreateSuccessor(size_t s, Task t, bool r, ContextSwitch*& child);
	virtual void Detatch();
	virtual bool HasSuccessors() const { return m_numSuccessors != 0; }
private:
	QueryableContextSwitch(size_t s, Task t, bool r, QueryableContextSwitch* parent) : ContextSwitch(s, t, r, parent), m_numSuccessors(0) {}
	size_t m_numSuccessors;
};

// This class provides a convenient mechanism to iterate over an EnumberableContextSwitch's children
class ChildIterator {
	friend class EnumerableContextSwitch;
public:
	bool HasNext() const { return m_itr != m_end; }
	ContextSwitch* Next();
private:
	ChildIterator(std::map<size_t, std::vector<ContextSwitch*>>::iterator begin, std::map<size_t, std::vector<ContextSwitch*>>::iterator end) : m_end(end) {
		m_itr = begin;
		m_index = 0;
	}
	std::map<size_t, std::vector<ContextSwitch*>>::iterator m_itr;
	const std::map<size_t, std::vector<ContextSwitch*>>::iterator m_end;
	size_t m_index;
};

// This is a context switch node that actually keeps track of its children, not just whether it
// has them or not, and allows you to enumerate over them.
class EnumerableContextSwitch : public ContextSwitch {
public:
	EnumerableContextSwitch() : ContextSwitch() {}
	virtual bool CreateSuccessor(size_t s, Task t, bool r, ContextSwitch*& child);
	virtual void Detatch();
	virtual bool HasSuccessors() const { return !m_children.empty(); }
	virtual void DetatchSubtree();
	virtual ChildIterator Iterator() { return ChildIterator(m_children.begin(), m_children.end()); }
private:
	EnumerableContextSwitch(size_t s, Task t, bool r, EnumerableContextSwitch* parent) : ContextSwitch(s, t, r, parent) {}
	bool RemoveChild(ContextSwitch* cs);
	std::map<size_t, std::vector<ContextSwitch*>> m_children;
};