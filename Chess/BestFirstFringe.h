/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ExecutionSummary.h"
#include "ContextSwitch.h"
#include "PriorityFunction.h"
#include "HashFunction.h"
#include <deque>

class FringeBin {
public:
	virtual void Add(ContextSwitch* cs) {}
	virtual bool Contains(ContextSwitch* cs) { return false; }
	virtual void Remove(ContextSwitch* cs) {}
	virtual ContextSwitch* RemoveNext() { return NULL; }
	virtual bool IsEmpty() { return false; }
};

class StackFringeBin : public FringeBin {
public:
	virtual void Add(ContextSwitch* cs);
	virtual bool Contains(ContextSwitch* cs);
	virtual void Remove(ContextSwitch* cs);
	virtual ContextSwitch* RemoveNext();
	virtual bool IsEmpty();
private:
	std::deque<ContextSwitch*> m_bin;
};

class QueueFringeBin : public FringeBin {
public:
	virtual void Add(ContextSwitch* cs);
	virtual bool Contains(ContextSwitch* cs);
	virtual void Remove(ContextSwitch* cs);
	virtual ContextSwitch* RemoveNext();
	virtual bool IsEmpty();
private:
	std::deque<ContextSwitch*> m_bin;
};

class OrderedHashFringeBin : public FringeBin {
public:
	virtual void Add(ContextSwitch* cs);
	virtual bool Contains(ContextSwitch* cs);
	virtual void Remove(ContextSwitch* cs);
	virtual ContextSwitch* RemoveNext();
	virtual bool IsEmpty();
private:
	std::vector<ContextSwitch*> m_bin;
	stdext::hash_map<ContextSwitch*, size_t> m_map;
	void Cleanup();
};

class RandomHashFringeBin : public FringeBin {
public:
public:
	virtual void Add(ContextSwitch* cs);
	virtual bool Contains(ContextSwitch* cs);
	virtual void Remove(ContextSwitch* cs);
	virtual ContextSwitch* RemoveNext();
	virtual bool IsEmpty();
private:
	std::vector<ContextSwitch*> m_bin;
	stdext::hash_map<ContextSwitch*, size_t> m_map;
};

static enum BinType {
	QUEUE,
	STACK,
	ORDERED_HASH,
	RANDOM_HASH,
};

// I made it so you could pop from either end of a given priority bound.  Alternatively, you could just have a
// set instead since the order in which you choose among equal priority items theoretically doesn't matter.

class BestFirstFringe {
public:
	BestFirstFringe(BinType type);
	~BestFirstFringe();
	virtual bool Insert(ContextSwitch* cs, size_t priority);
	virtual ContextSwitch* RemoveNext();
	virtual size_t CurrentBound();
	virtual void Remove(ContextSwitch* cs);
	virtual size_t GetPriority(ContextSwitch* cs);
private:
	// I wasn't sure if the dequeue was the right data structure to use here.  I want the lowest
	// possible space overhead with constant time to insert or remove from either end.  I don't care
	// about accessing, inserting, or removing anywhere but the two ends.  I thought a doubly linked
	// list might have higher space overhead, but I have no idea how the deque is implemented underneath.
	std::map<size_t, FringeBin*> m_fringe;
	// true if we should remove the most recently added item with the minimum priority from
	// the fringe, false if we should remove the least recently added item with the minimum 
	// priority
	BinType m_binType;
	FringeBin* CreateBin();
};