/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include <hash_map>
#include "SyncVar.h"
#include "ChessExecution.h"
#include "ChessImpl.h"
#include "SyncVarManager.h"
#include "HashFunction.h"

class HBExecution{
public:
	HBExecution(){
		Reset();
	}

	HBExecution(IChessExecution* exec){
		Reset();
		if(exec == 0) return;
		for(size_t i=0; i<exec->NumTransitions(); i++){
			PushTransition(exec->Transition(i));
		}
	}

	void PushTransition(const ChessTransition& t){
		// Don't push these transitions, they don't affect the happens-before graph (Katie)
		if (!SVOP::IsWrite(t.op) && !SVOP::IsRead(t.op)) {
			return;
		}
		SyncVarManager* svm = ChessImpl::GetSyncVarManager();
		if(svm->IsAggregate(t.var)){
			const SyncVar* varvec = svm->GetAggregateVector(t.var);
			size_t n = svm->GetAggregateVectorSize(t.var);
			for(size_t i=0; i<n; i++){
				PushTransition(ChessTransition(t.tid, varvec[i], t.op));
			}
			return;
		}

		//if(stepTable.size() <= t.tid){
		//	stepTable.resize(t.tid+1);
		//}
		size_t step = stepTable[t.tid]++;

		HashValue v = 0;
		if (t.op == SVOP::CHOICE)
		{
			v = ComputeHash(t.tid, step, t.var);
		}
		else
		{
			//if(versionTable.size() <= t.var){
			//	versionTable.resize(t.var+1);
			//}
			// Only update the version number on a write (Katie)
			size_t version = versionTable[t.var];
			if (SVOP::IsWrite(t.op)) {
				versionTable[t.var]++;
			}

			//if(canonSyncVar.size() <= t.var){
			//	canonSyncVar.resize(t.var+1);
			//}
			if(canonSyncVar[t.var] == 0){
				// generate a new canonical sync var for t.var.
				// use the current canon sync var id from t.tid
				//if(canonSyncVarId.size() <= t.tid){
				//	canonSyncVarId.resize(t.tid+1);
				//}
				canonSyncVar[t.var] = canonSyncVarId[t.tid]++;
			}
			size_t canon = canonSyncVar[t.var];		
			v = ComputeHash(t.tid, step, canon, version);
		}
		//*GetChessOutputStream() << t.tid << "," << step << "," << canon << "," << version << " = " << v << std::endl;
		hash ^= v;
	}

	HashValue GetHash() const {return hash;}



	void Reset(){
		hash = 0;
		canonSyncVarId.clear();
		canonSyncVar.clear();
		versionTable.clear();
		stepTable.clear();
	}

private:
	HashValue hash;
	SyncVarVector<size_t> canonSyncVar;
	SyncVarVector<size_t> canonSyncVarId;
	SyncVarVector<size_t> versionTable;
	TaskVector<size_t> stepTable;
	//std::vector<size_t> canonSyncVar;
	//std::vector<size_t> canonSyncVarId;
	//std::vector<size_t> versionTable;
	//std::vector<size_t> stepTable;

	//stdext::hash_map<SyncVar, size_t> canonSyncVar;
	//stdext::hash_map<Task, size_t> canonSyncVarId;
	//stdext::hash_map<SyncVar, size_t> versionTable;
	//stdext::hash_map<Task, size_t> stepTable;

};