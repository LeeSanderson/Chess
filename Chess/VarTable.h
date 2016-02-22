/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

template<class ValueType>
class var_table{
public:
	var_table(int _logPageSize=7, int _logVarSize=2)
		:logVarSize(_logVarSize), logPageSize(_logPageSize), cache(0){}

	ValueType& operator[](const void* addr){
		int key;
		int offset;
		convert_addr(addr, key, offset);
		if(cache != 0 && cache_key == key){
			return (*cache)[offset];
		}
		if(map.find(key) == map.end()){
			map[key].insert(map[key].begin(), (1 << logPageSize), ValueType());
		}
		cache_key = key;
		cache = &(map[key]);
		return map[key][offset];
	}

	void clear(){map.clear();cache=0;}

private:
	stdext::hash_map<int, std::vector<ValueType> > map;
	int logVarSize;
	int logPageSize;
	int cache_key;
	std::vector<ValueType>* cache;

	void convert_addr(const void* addr, int& key, int& offset){
		int a = (((int)addr) >> logVarSize);
		key = (a >> logPageSize);
		offset = a - (key << logPageSize);
	}
};
