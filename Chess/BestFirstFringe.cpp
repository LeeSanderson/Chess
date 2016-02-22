/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "BestFirstFringe.h"

void StackFringeBin::Add(ContextSwitch* cs)
{
	m_bin.push_back(cs);
}

ContextSwitch* StackFringeBin::RemoveNext()
{
	ContextSwitch* ret = m_bin.back();
	m_bin.pop_back();
	return ret;
}

void StackFringeBin::Remove(ContextSwitch* cs)
{
	assert(false);
}

bool StackFringeBin::Contains(ContextSwitch* cs)
{
	assert(false);
	return false;
}

bool StackFringeBin::IsEmpty()
{
	return m_bin.empty();
}

void QueueFringeBin::Add(ContextSwitch* cs)
{
	m_bin.push_back(cs);
}

ContextSwitch* QueueFringeBin::RemoveNext()
{
	ContextSwitch* ret = m_bin.front();
	m_bin.pop_front();
	return ret;
}

void QueueFringeBin::Remove(ContextSwitch *cs)
{
	assert(false);
}

bool QueueFringeBin::Contains(ContextSwitch *cs)
{
	assert(false);
	return false;
}

bool QueueFringeBin::IsEmpty()
{
	return m_bin.empty();
}

void OrderedHashFringeBin::Add(ContextSwitch* cs)
{
	assert (m_bin.empty() || m_bin.back() != NULL);
	m_bin.push_back(cs);
	assert (m_bin[m_bin.size()-1] == cs);
	m_map[cs] = m_bin.size()-1;
}

ContextSwitch* OrderedHashFringeBin::RemoveNext()
{
	ContextSwitch* ret = m_bin.back();
	assert (ret != NULL);
	m_bin.pop_back();
	m_map.erase(ret);
	Cleanup();
	return ret;
}

void OrderedHashFringeBin::Cleanup()
{
	while (!m_bin.empty() && m_bin.back() == NULL) {
		m_bin.pop_back();
	}
	assert (m_bin.empty() || m_bin.back() != NULL);
}

void OrderedHashFringeBin::Remove(ContextSwitch* cs)
{
	assert (m_map.find(cs) != m_map.end());
	m_bin[m_map[cs]] = NULL;
	Cleanup();
	m_map.erase(cs);
}

bool OrderedHashFringeBin::Contains(ContextSwitch* cs)
{
	return m_map.find(cs) != m_map.end();
}

bool OrderedHashFringeBin::IsEmpty()
{
	assert (m_bin.empty() || m_bin.back() != NULL);
	return m_bin.empty();
}

void RandomHashFringeBin::Add(ContextSwitch* cs)
{
	m_bin.push_back(cs);
	assert (m_bin[m_bin.size()-1] == cs);
	m_map[cs] = m_bin.size()-1;
}

ContextSwitch* RandomHashFringeBin::RemoveNext()
{
	size_t randIndex = rand()%m_bin.size();
	assert (randIndex < m_bin.size());
	ContextSwitch* ret = m_bin[randIndex];
	assert (ret != NULL);
	ContextSwitch* replacement = m_bin[m_bin.size()-1];
	m_bin[randIndex] = replacement;
	m_map[replacement] = randIndex;
	m_map.erase(ret);
	m_bin.pop_back();
	return ret;
}

void RandomHashFringeBin::Remove(ContextSwitch* cs)
{
	assert(m_map.find(cs) != m_map.end());
	size_t csIndex = m_map[cs];
	assert (csIndex < m_bin.size());
	assert (m_bin[csIndex] == cs);
	ContextSwitch* replacement = m_bin[m_bin.size()-1];
	m_bin[csIndex] = replacement;
	m_map[replacement] = csIndex;
	m_map.erase(cs);
	m_bin.pop_back();
}

bool RandomHashFringeBin::Contains(ContextSwitch* cs)
{
	return m_map.find(cs) != m_map.end();
}

bool RandomHashFringeBin::IsEmpty()
{
	assert (m_bin.size() == m_map.size());
	return m_bin.empty();
}

BestFirstFringe::BestFirstFringe(BinType binType) :
	m_binType(binType) {
}

BestFirstFringe::~BestFirstFringe() {
	while (RemoveNext());
}

bool BestFirstFringe::Insert(ContextSwitch* cs, size_t priority) {
	assert(!cs->HasSuccessors());
	if (m_fringe.find(priority) == m_fringe.end()) {
		m_fringe[priority] = CreateBin();
	}
	m_fringe[priority]->Add(cs);
	// I used to keep track of visited info here and return false if the insert failed, but
	// I changed that.  So now there's really no point in returning anything.  Insert will
	// always succeed.
	return true;
}


ContextSwitch* BestFirstFringe::RemoveNext() {
	if (m_fringe.empty()) {
		// we're done with the search
		return NULL;
	}
	// return and remove an element from the lowest bin
	assert(!m_fringe.begin()->second->IsEmpty());
	ContextSwitch* ret = m_fringe.begin()->second->RemoveNext();
	// see if we have finished up that priority bound
	if (m_fringe.begin()->second->IsEmpty()) {
		FringeBin* bin = m_fringe.begin()->second;
		m_fringe.erase(m_fringe.begin());
		delete bin;
	}
	return ret;
}

// Originally, this was for the strategy to know when the priority bound had changed so it could
// flush its visited set, but I got rid of visited sets by using the suicidal tree.  There's
// no longer really any purpose for it, but it may be a useful piece of info to know.  I.e.
// if you wanted to print out the priority of each execution performed for debugging purposes.
size_t BestFirstFringe::CurrentBound() {
	return m_fringe.empty() ? 0 : m_fringe.begin()->first;
}

FringeBin* BestFirstFringe::CreateBin() {
	switch (m_binType) {
		case STACK:
			return new StackFringeBin();
		case QUEUE:
			return new QueueFringeBin();
		case ORDERED_HASH:
			return new OrderedHashFringeBin();
		case RANDOM_HASH:
			return new RandomHashFringeBin();
		default:
			assert(false);
			return NULL;
	}
}

void BestFirstFringe::Remove(ContextSwitch *cs) {
	assert(m_binType == RANDOM_HASH || m_binType == ORDERED_HASH);
	std::map<size_t, FringeBin*>::iterator itr = m_fringe.begin();
	while (itr != m_fringe.end()) {
		if (itr->second->Contains(cs)) {
			itr->second->Remove(cs);
			if (itr->second->IsEmpty()) {
				FringeBin* bin = itr->second;
				m_fringe.erase(itr);
				delete bin;
			}
			break;
		}
		itr++;
	}
}

size_t BestFirstFringe::GetPriority(ContextSwitch *cs) {
	assert(m_binType == RANDOM_HASH || m_binType == ORDERED_HASH);
	std::map<size_t, FringeBin*>::iterator itr = m_fringe.begin();
	while (itr != m_fringe.end()) {
		if (itr->second->Contains(cs)) {
			return itr->first;
		}
		itr++;
	}
	assert (false);
	return UINT_MAX;
}