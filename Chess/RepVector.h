/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessStl.h"
#include "ChessAssert.h"
// RepVector can concisely represent repeated data at successive
// locations in a Vector.

// Inserting value v at index i, inserts v at locations i, i+1, i+2, ...
// A subsequent insert of v' at j > i, causes the array to have v at 
// locations i, i+1, ... j-1 and v' at locations j, j+1, ...

// Elements can only be inserted at an index beyond the previous insert

// See UnitTest.cpp::RepVector for usage

template <class T>
class RepVector{
public:
	RepVector(){
		index.push_back(0);
		data.push_back(T());
		//assert(CheckInvariant());
	}
	RepVector(T init){
		index.push_back(0);
		data.push_back(init);
		//assert(CheckInvariant());
	}

	const T& Lookup(size_t pos) const{
		//assert(CheckInvariant());
		return data[FindIndex(pos)];
	}

	T& Last(){
		//assert(CheckInvariant());
		return data[data.size()-1];
	}

	size_t LastIndex() const{
		//assert(CheckInvariant());
		return index[index.size()-1];
	}

	// define index pos and set RepVector[pos] = val
	void Insert(size_t pos, T val){
		//assert(CheckInvariant());
		assert(index[index.size()-1] < pos);
		index.push_back(pos);
		data.push_back(val);
	}

	// a version of Insert for 'inplace' update
	T& Insert(size_t pos){
		//assert(CheckInvariant());
		assert(index[index.size()-1] < pos);
		index.push_back(pos);
		data.push_back(T());
		return Last();
	}

	// insert a copy of the Last element at pos
	T& InsertLast(size_t pos){
		//assert(CheckInvariant());
		assert(index[index.size()-1] < pos);
		T l = Last();
		index.push_back(pos);
		data.push_back(l);
		return Last();
	}

	//Remove all defined indices > pos
	void Prune(size_t pos){
		//assert(CheckInvariant());
		size_t i = FindIndex(pos);
		assert(index[i] <= pos && (i+1 == index.size() || index[i+1] > pos));
		data.resize(i+1);
		index.resize(i+1);
	}

	void clear(){Prune(0);}

private:
	std::vector<size_t> index;
	std::vector<T> data;

	bool CheckInvariant() const {
		return 	data.size() == index.size() && index.size() > 0 && index[0] == 0;
	}
	
	size_t FindIndex(size_t pos) const{
		//first implementation is linear
		for(size_t i = index.size(); i>0; i--){
			if(index[i-1] <= pos){
				return i-1;
			}
		}
		assert(false);
		return 0;
	}
};