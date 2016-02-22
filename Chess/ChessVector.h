/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

template<class T, int BaseVecSize>
class ChessVector{
public:
	ChessVector(){
		overflow = 0;
		sz = 0;
		for(size_t i=0; i<BaseVecSize; i++){
			basevec[i] = T();
		}
	}

	~ChessVector(){
		if(overflow) delete overflow;
		overflow = 0;
		sz = 0;
	}

	ChessVector(const ChessVector& cp){
		overflow = 0;
		sz = 0;
		Resize(cp.sz);
		assert(sz == cp.sz);
		for(size_t i=0; i<sz; i++){
			(*this)[i] = cp[i];
		}

	}

	ChessVector& operator=(const ChessVector& other){
		if(this != &other){
			overflow = 0;
			sz = 0;
			Resize(other.sz);
			assert(sz == other.sz);
			for(size_t i=0; i<sz; i++){
				(*this)[i] = other[i];
			}
		}
		return *this;
	}

	size_t size() const{
		return sz;
	}

	void clear(){
		sz = 0;
		if (overflow) {
			overflow->clear();
			delete overflow;
			overflow = NULL;
		}
	}

	const T& operator[](size_t i) const{
		return (const T&) ((ChessVector<T,BaseVecSize>*)this)->operator[](i);
	}
	T& operator[](size_t i) {
		if(i >= size()){
			Resize(i+1);
		}
		if(i < BaseVecSize)
			return basevec[i];
		return (*overflow)[i-BaseVecSize];
	}

private:
	T basevec[BaseVecSize];
	std::vector<T>* overflow;
	size_t sz;

	void Resize(size_t s){
		for(size_t i=sz; i<s && i<BaseVecSize; i++){
			basevec[i] = T();
		}
		if(s > BaseVecSize){
			if(!overflow){
				overflow = new std::vector<T>(s-BaseVecSize);
			}
			overflow->resize(s-BaseVecSize);
		}
		sz = s;
	}
};
