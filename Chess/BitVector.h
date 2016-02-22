/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"
#include "ChessAssert.h"
#include <vector>
// fast implementation of a bitvector
// Stores upto 32 elements in an integer
// Stores the remaining in a vector<bool>

template<class T> class TaskVector;

class CHESS_API BitVector{
private:
	static const int INTEGER_SIZE=32;
	size_t smallbv;
	std::vector<bool>* overflow;
	static size_t GetMask(size_t index) {
		return (0x1 << index);
	}
	BitVector(size_t s){
		smallbv =s;
		overflow = 0;
	}

public:
	BitVector(){
		smallbv = 0;
		overflow = 0;
	}
	~BitVector(){
		if(overflow)
			delete overflow;
		overflow = 0;
	}

	BitVector(const BitVector& cp){
		smallbv = 0;
		overflow = 0;
		cp.Copy(*this);
	}

	const BitVector& operator=(const BitVector& other){
		if(this != &other){
			other.Copy(*this);
		}
		return *this;
	}

	void Copy(BitVector& res) const{
		res.smallbv = smallbv;
		if(!overflow && res.overflow){
			delete res.overflow;
			res.overflow = 0;
		}
		if(overflow){
			if(!res.overflow)
				res.overflow = new std::vector<bool>();
			*res.overflow = *overflow;
		}
	}

	void Set(size_t index, bool value){
		if(index < INTEGER_SIZE){
			size_t mask = GetMask(index);
			if(value)
				smallbv |= mask;
			else
				smallbv &= (~mask);
		}
		else{
			if(!value && !Get(index)){
				return; // no op
			}
			index -= INTEGER_SIZE;
			if(!overflow)
				overflow = new std::vector<bool>();
			if(index >= overflow->size())
				overflow->resize(index+1);
			(*overflow)[index] = value;
		}
	}

	bool Get(size_t index) const {
		if(index < INTEGER_SIZE){
			return (smallbv & GetMask(index)) != 0;
		}
		else{
			if(!overflow)
				return false;
			index -= INTEGER_SIZE;
			if(index >= overflow->size()) return false; 
			return (*overflow)[index];
		}
	}

	// BitVector as Set abstraction
	void Insert(size_t index){Set(index, true);}
	void Erase(size_t index) {Set(index, false);}
	// lower case macros for compatibility with sets
	void insert(size_t index){Set(index, true);}
	void erase(size_t index) {Set(index, false);}
	void clear(){
		smallbv = 0;
		if(overflow)
			delete overflow;
		overflow = 0;
	}

	bool Contains(size_t index) const {return Get(index);}
	bool IsEmpty() const{
		if(!overflow) return smallbv == 0;
		if(smallbv != 0) return false;
		for(size_t i=0; i<overflow->size(); i++){
			if((*overflow)[i]) return false;
		}
		return true;
	}

	void Intersect(const BitVector& other) {
		smallbv &= other.smallbv;
		if(overflow && other.overflow){
			for(size_t i=0; i<overflow->size() && i<other.overflow->size(); i++){
				(*overflow)[i] = (*overflow)[i] & (*other.overflow)[i];
			}
		}
	}

	void Union(const BitVector& other){
		smallbv |= other.smallbv;
		if(other.overflow){
			if(!overflow){
				overflow = new std::vector<bool>(other.overflow->size());
			}
			else{
				if(overflow->size() < other.overflow->size())
					overflow->resize(other.overflow->size());
			}
			for(size_t i=0; i<overflow->size() && i<other.overflow->size(); i++){
				(*overflow)[i] = (*overflow)[i] | (*other.overflow)[i];
			}
		}
	}

	bool operator==(const BitVector& other) const{
		if(smallbv == other.smallbv && overflow == other.overflow) return true;
		if(smallbv != other.smallbv) return false;
		size_t ub = INTEGER_SIZE;
		if(overflow && ub < overflow->size()) ub = overflow->size();
		if(other.overflow && ub < other.overflow->size()) ub = other.overflow->size();

		for(size_t i=INTEGER_SIZE; i<ub; i++){
			if(Get(i) != other.Get(i)) return false;
		}
		return true;
		
	}

	// find a k > index such that Get(k) is true
	// k wraps around to 0 if required
	// Should only be called when !IsEmpty()
	// This is a broken API
	size_t FindIndexLargerThan(size_t index)const{
		//assert(!IsEmpty());
		size_t ub = INTEGER_SIZE;
		if(overflow)
			ub = INTEGER_SIZE + overflow->size();
		for(size_t i=index+1; i<ub; i++){
			if(Get(i)) return i;
		}
		for(size_t i=0; i<=index; i++){
			if(Get(i)) return i;
		}
		//shouldnt reach here unless you are empty
		assert(/*false*/!IsEmpty());
		return 0;
	}

	std::ostream& operator<<(std::ostream&);
};

inline std::ostream& operator<<(std::ostream& o, BitVector& b){
	return b.operator<<(o);
}