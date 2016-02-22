/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include <hash_map>
#include <set>
#include "BitVector.h"
#include "SyncVarVector.h"

//template<class T>
class PriorityGraph{
public:
	//void AddEdges(Task u, const std::set<Task>& succnodes){
	//	succ[u].insert(succnodes.begin(), succnodes.end());
	//}
	void AddEdges(Task u, const BitVector& b){
		succ[u].Union(b);
	}
	bool HasOutgoingEdges(Task u) const{
		return !succ[u].IsEmpty();
	}
	//size_t NumOutgoingEdges(Task u) const {
	//	stdext::hash_map<Task, std::set<Task> >::const_iterator i;
	//	i = succ.find(u);
	//	if(i == succ.end()) return 0;
	//	return i->second.size();
	//}
	const BitVector& OutgoingEdges(const Task u) const{
		return succ[u];
	}

	//typedef std::set<Task>::const_iterator EdgeIterator;

	//EdgeIterator OutgoingEdgesBegin(const Task u) const{
	//	assert(succ.find(u) != succ.end());
	//	return succ.find(u)->second.begin();
	//}
	//EdgeIterator OutgoingEdgesEnd(const Task u) const {
	//	assert(succ.find(u) != succ.end());
	//	return succ.find(u)->second.end();
	//}
	//const std::set<Task>& OutgoingEdges(const Task u) const {
	//	assert(succ.find(u) != succ.end());
	//	return succ.find(u)->second;
	//}

	void DelOutgoingEdges(Task v){
		succ[v].clear();
		//succ.erase(v);
	}

	bool HasIncomingEdges(Task v) const{
		for(size_t i=0; i<succ.size(); i++){
			if(succ[i].Contains(v))
				return true;
		}
		return false;
	}

	void DelIncomingEdges(Task v) {
		for(size_t i=0; i<succ.size(); i++){
			succ[i].erase(v);
		}
	}

	//size_t NumIncomingEdges(Task v) const {
	//	int ret = 0;
	//	stdext::hash_map<Task, std::set<Task> >::const_iterator i;
	//	for(i = succ.begin(); i!= succ.end(); i++){
	//		std::set<Task>::const_iterator vi = i->second.find(v);
	//		if(vi != i->second.end()){
	//			ret++;
	//		}
	//	}
	//	return ret;
	//}

	//void DelIncomingEdges(Task v){
	//	stdext::hash_map<Task, std::set<Task> >::iterator i;
	//	std::vector<Task> emptyKeys;
	//	for(i = succ.begin(); i!= succ.end(); i++){
	//		std::set<Task>::iterator vi = i->second.find(v);
	//		if(vi != i->second.end()){
	//			i->second.erase(vi);
	//		}
	//		if(i->second.size() == 0){
	//			//erasing outside loop - dont know if
	//			// erase() invalidates hash_map iterators
	//			emptyKeys.push_back(i->first);
	//		}
	//	}
	//	for(size_t i=0; i<emptyKeys.size(); i++){
	//		succ.erase(emptyKeys[i]);
	//	}
	//}
	
	void Clear(){
		succ.clear();
	}
private:
	TaskVector<BitVector> succ;
};