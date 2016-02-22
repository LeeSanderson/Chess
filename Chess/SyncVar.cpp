/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#if SINGULARITY

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif

#endif

#include "SyncVar.h"
#include <cassert>
#include "HashFunction.h"
#include <vector>

	class AggregateSyncVar{
	public:
		AggregateSyncVar(SyncVar v[], int n);
		AggregateSyncVar(const AggregateSyncVar& cp);
		~AggregateSyncVar();
		int NumVars() { return num;}
		SyncVar Var(int i);
		bool operator==(AggregateSyncVar& other);
		size_t Hash(){ return hash; }
		bool operator<(AggregateSyncVar& other);

		void IncRef();
		void DecRef();
	//private:
		int num;
		size_t hash;
		SyncVar* var;
		int numRefs;
	};

SyncVar::SyncVar(SyncVar vars[], int n){
	if(n <= 1){
		isAggregate = false;
		u.id = n > 0 ? vars[0].u.id : 0;
	}
	else{
		isAggregate = true;
		u.aggregate = new AggregateSyncVar(vars, n);
		u.aggregate->IncRef();
	}
}

void SyncVar::AggregateCopyConstruct(const SyncVar& cp){
	isAggregate = true;
	cp.u.aggregate->IncRef();
	u.aggregate = cp.u.aggregate;	
}

void SyncVar::AggregateDestruct(){
	u.aggregate->DecRef();
	u.aggregate = 0;
}

int SyncVar::AggregateNumVars() const {
	return u.aggregate->NumVars();
}

SyncVar SyncVar::AggregateVar(int i) const{
	return u.aggregate->Var(i);
}

bool SyncVar::AggregateEq(const SyncVar& other) const {
	return *this->u.aggregate == *other.u.aggregate;
}

size_t SyncVar::AggregateHash() const{
	return u.aggregate->Hash();
}

void SyncVar::AggregateAssignment(const SyncVar& cp){
	if(this != &cp){
		if(this->IsAggregate()){
			this->u.aggregate->DecRef();
			this->u.aggregate = 0;
		}
		if(!cp.IsAggregate()){
			this->isAggregate = false;
			this->u.id = cp.u.id;
		}
		else{
			this->isAggregate = true;
			cp.u.aggregate->IncRef();
			this->u.aggregate = cp.u.aggregate;
		}
	}
}



AggregateSyncVar::AggregateSyncVar(SyncVar v[], int n){
	assert(n > 1);
	num = n;
	var = new SyncVar[n];
	for(int i=0; i<n; i++){
		var[i] = v[i];
	}
	hash = ComputeHash(v, n);
	numRefs = 0;
}

AggregateSyncVar::AggregateSyncVar(const AggregateSyncVar& cp){
	num = cp.num;
	var = new SyncVar[num];
	for(int i=0; i<num; i++){
		var[i] = cp.var[i];
	}
	hash = cp.hash;
}

void AggregateSyncVar::IncRef(){
	numRefs++;
}

void AggregateSyncVar::DecRef(){
	numRefs--;
	if(numRefs == 0)
		delete this;
}

AggregateSyncVar::~AggregateSyncVar(){
	delete[] var;
	var = 0;
}

SyncVar AggregateSyncVar::Var(int i){
	if(i < 0 || i >= num) return SyncVar(); // Null
	return var[i];
}

bool AggregateSyncVar::operator==(AggregateSyncVar& other){
	if(this->num != other.num) return false;
	for(int i=0; i<this->num; i++){
		if(this->var[i] != other.var[i]) 
			return false;
	}
	return true;
}

bool SyncVar::operator<(const SyncVar &v) const {
	if(!this->IsAggregate()){
		if(v.IsAggregate())
			return true;
		return this->Id() < v.Id();
	}
	else{
		if(!v.IsAggregate())
			return false;
		return this->u.aggregate->operator<(*v.u.aggregate);
	}
}

bool AggregateSyncVar::operator<(AggregateSyncVar &other){
	if(this->NumVars() < other.NumVars())
		return true;
	if(this->NumVars() > other.NumVars())
		return false;
	for(int i=0; i<this->NumVars(); i++){
		if(this->Var(i) == other.Var(i))
			continue;
		return this->Var(i) < other.Var(i);
	}
	// *this == other
	return false;
}

std::ostream& SyncVar::operator<<(std::ostream& o) const{
	if(!IsAggregate())
		return o << Id();
	o << "[";
	for(int i=0; i<u.aggregate->NumVars(); i++){
		o << u.aggregate->Var(i) << ' ';
	}
	return o << "]";
}

std::istream& SyncVar::operator>>(std::istream& i){
	size_t varid=0;
	i >> varid;
	if(i.good()){
		*this = SyncVar(varid);
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
		*this = SyncVar(arr, n);
		delete[] arr;
	}
#endif
	return i;
}

// Reads a sequence of SyncVars and the trailing ']'
SyncVar* SyncVar::ReadSyncVarSeq(std::istream& i, int& n){
#if SINGULARITY
#else
	std::vector<SyncVar> input;
	SyncVar v;
	while(i >> v){
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
		for(size_t i=0; i<input.size(); i++){
			ret[i] = input[i];
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
