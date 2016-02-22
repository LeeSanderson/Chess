/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "SyncVarManager.h"
#include "ChessAssert.h"
#include "ChessVector.h"

template <class T>
class TaskVector{
public:
	size_t size() const{return vec.size();}
	void clear(){vec.clear();}
	const T& operator[](size_t i) const{return vec[i];}
	T& operator[](size_t i) {return vec[i];}
private:
	ChessVector<T, 16> vec;
};

//template<class T>
//class TaskVector{
//public:
//	TaskVector(){
//		overflow = 0;
//		sz = 0;
//	}
//
//	size_t size() const{
//		return sz;
//	}
//	void clear(){
//		//for(size_t i=0; i<sz && i<BaseVecSize; i++){
//		//	basevec[i] = T();
//		//}
//		if(overflow)
//			delete overflow;
//		overflow = 0;
//		sz = 0;
//	}
//
//	const T& operator[](size_t i) const{
//		return (const T&) ((TaskVector<T>*)this)->operator[](i);
//	}
//	T& operator[](size_t i) {
//		if(i >= size()){
//			Resize(i+1);
//		}
//		if(i < BaseVecSize)
//			return basevec[i];
//		return overflow[i-BaseVecSize];
//	}
//
//	//TaskVector(const TaskVector& v){
//	//	for(size_t i=0; i<BaseVecSize; i++){
//	//		basevec[i] = v.basevec[i];
//	//	}
//	//	overflow = v.overflow;
//	//}
//private:
//	static const int BaseVecSize=16;
//	T basevec[BaseVecSize];
//	T* overflow;
//	size_t sz;
//
//	void Resize(size_t s){
//		for(size_t i=sz; i<s && i < BaseVecSize; i++)
//			basevec[i] = T();
//		if(s > BaseVecSize){
//			if(!overflow){
//				overflow = new T[SyncVarManager::LastTask+1];
//			}
//			for(size_t i=BaseVecSize; i<s; i++){
//				overflow[i-BaseVecSize] = T();
//			}
//		}
//		sz = s;
//	}
//};

template<class T>
class SyncVarVector{
public:
	void clear(){
//		myHash.clear();
		taskvec.clear();
		syncVarVec.clear();
		aggSyncVarVec.clear();
	}
	const T& operator[](SyncVar i) const{
//		return myHash[i];
		return (const T&) ((SyncVarVector<T>*)this)->operator[](i);
	}
	T& operator[](SyncVar i) {
		if(i <= SyncVarManager::LastTask){
			return taskvec[i];
		}
		if(i < SyncVarManager::FirstAggregateSyncVar){
			return syncVarVec[i-SyncVarManager::LastTask-1];
		}
		return aggSyncVarVec[i-SyncVarManager::FirstAggregateSyncVar];
		//if(i <= SyncVarManager::LastTask){
		//	return taskvec[i];
		//}
		//else if(i < SyncVarManager::FirstAggregateSyncVar){
		//	size_t index = i-SyncVarManager::LastTask-1;
		//	if(index >= syncVarVec.size())
		//		syncVarVec.resize(index+1);
		//	return syncVarVec[index];
		//}
		//else{
		//	size_t index = i-SyncVarManager::FirstAggregateSyncVar;
		//	if(index >= aggSyncVarVec.size())
		//		aggSyncVarVec.resize(index+1);
		//	return aggSyncVarVec[index];
		//}
	}


private:
//	stdext::hash_map<SyncVar, T> myHash;
	TaskVector<T> taskvec;
	TaskVector<T> syncVarVec;
	TaskVector<T> aggSyncVarVec;
	//std::vector<T> syncVarVec;
	//std::vector<T> aggSyncVarVec;
};
