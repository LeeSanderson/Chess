/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "SyncVarManager.h"
#include "ChessAssert.h"
#include "ChessImpl.h"

SyncVarManager::~SyncVarManager(){
	if(initState){
		delete initState;
		initState = 0;
	}
}

void SyncVarManager::Reset(){
	if(initState){
		nextSyncVarId = initState->nextSyncVarId;
		for(size_t i=initState->aggregateMap.size(); i<aggregateMap.size(); i++){
			delete aggregateMap[i].first;
		}
		aggregateMap.resize(initState->aggregateMap.size());
	}
	else{
		nextSyncVarId = FirstNonTaskSyncVar;
		for(size_t i=0; i<aggregateMap.size(); i++){
			delete aggregateMap[i].first;
		}
		aggregateMap.clear();
	}
}

void SyncVarManager::SetInitState()	{
	if(!initState)
		initState = new SyncVarManager();
	initState->nextSyncVarId = nextSyncVarId;
	initState->aggregateMap = aggregateMap;
}


SyncVar SyncVarManager::GetAggregateSyncVar(const SyncVar* vec, size_t n){
	assert(n >= 2);
	SyncVar ret = FirstAggregateSyncVar + aggregateMap.size();
	SyncVar* varvec = new SyncVar[n];
	for(size_t i=0; i<n; i++){
		varvec[i] = vec[i];
	}
	aggregateMap.push_back(std::pair<const SyncVar*, size_t>(varvec, n));
	return ret;
}


const SyncVar* SyncVarManager::GetAggregateVector(SyncVar v){
	assert(IsAggregate(v) && v-FirstAggregateSyncVar < aggregateMap.size());
	return aggregateMap[v-FirstAggregateSyncVar].first;
}

size_t SyncVarManager::GetAggregateVectorSize(SyncVar v){
	assert(IsAggregate(v) && v-FirstAggregateSyncVar < aggregateMap.size());
	return aggregateMap[v-FirstAggregateSyncVar].second;
}





std::ostream& SyncVarWriter::operator<<(std::ostream& o)const{
	if(!ChessImpl::GetSyncVarManager()->IsAggregate(var))
		return o << var;

	o << "[";
	const SyncVar* varvec = ChessImpl::GetSyncVarManager()->GetAggregateVector(var);
	size_t n = ChessImpl::GetSyncVarManager()->GetAggregateVectorSize(var);
	for(size_t i=0; i<n; i++){
		o << varvec[i] << ' ';
	}
	return o << "]";
}

// Reads a sequence of SyncVars and the trailing ']'
SyncVar* ReadSyncVarSeq(std::istream& i, int& n){
#if SINGULARITY
#else
	std::vector<SyncVar> input;
	SyncVar v;
	while(i >> SyncVarReader(v)){
		input.push_back(v);
	}
	if(i.bad()){
		// all bets are off
		return 0;
	}

	i.clear();
	char c = 0;
	i >> c;
	if(c == ']'){
		n = (int)input.size();
		SyncVar* ret = new SyncVar[input.size()];
		for(size_t j=0; j<input.size(); j++){
			ret[j] = input[j];
		}
		return ret;
	}
	if(input.size() == 0){
		i.putback(c);
		i.clear(std::ios_base::failbit);
	}
	else{
		// all bets are off
		i.clear(std::ios_base::badbit);
	}
#endif
	return 0;
}


std::istream& SyncVarReader::operator>>(std::istream& i){
	SyncVar r;
	i >> r;
	if(i.good()){
		var = r;
		return i;
	}
#if !SINGULARITY
	// attempt reading an aggregate 
	i.clear();
	char c = 0;
	i >> c;
	if(c != '['){
		i.putback(c);
		i.clear(std::ios_base::failbit);
		return i;
	}

	SyncVar* arr;
	int n;
	if(arr = ReadSyncVarSeq(i, n)){
		var = ChessImpl::GetSyncVarManager()->GetAggregateSyncVar(arr, n);
		delete[] arr;
	}
#endif
	return i;
}
