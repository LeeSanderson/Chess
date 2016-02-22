/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

#include "SyncVar.h"

class SyncVarManager{
public:
	static const SyncVar NullSyncVar = 0;
	
	static const SyncVar FirstTask = 1;
	// Tasks from 1...509
	static const SyncVar LastTask = 508;

	static const SyncVar OperationVar = 509;
	static const SyncVar AnonymousVar = 510;
	static const SyncVar QuiescenceVar = 511;
	
	static const SyncVar FirstNonTaskSyncVar = 512;
	// Regular SyncVars from 512...2^16
	static const SyncVar LastNonAggregateSyncVar = (0x10000-1);
	static const SyncVar FirstAggregateSyncVar = 0x10000;
	

	SyncVarManager(){
		initState = 0;
		Reset();
	}
	void SetInitState();
	void Reset();
	~SyncVarManager();

	SyncVar GetNextSyncVar(){
		return nextSyncVarId++;
	}

	SyncVar GetAggregateSyncVar(const SyncVar* vec, size_t n);

	static bool IsAggregate(SyncVar v){return v >= FirstAggregateSyncVar;}

	const SyncVar* GetAggregateVector(SyncVar v);
	size_t GetAggregateVectorSize(SyncVar v);

private:
	int nextSyncVarId;
	std::vector<std::pair<const SyncVar*, size_t> > aggregateMap;

	SyncVarManager* initState;
};



// helper class to read and write SyncVars
class SyncVarWriter{
public:
	SyncVarWriter(const SyncVar& v):var(v){}
	std::ostream& operator<<(std::ostream& o) const;
private:
	const SyncVar& var;
};

inline std::ostream& operator<<(std::ostream& o, const SyncVarWriter& tr){return tr.operator<<(o);}

class SyncVarReader{
public:
	SyncVarReader(SyncVar& v):var(v){}
	std::istream& operator>>(std::istream& i);
private:
	SyncVar& var;
};

inline std::istream& operator>>(std::istream& i, SyncVarReader& tr){return tr.operator>>(i);}
