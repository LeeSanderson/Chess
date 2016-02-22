/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
owners is a map from memory addresses to the type OwnerInfo.
owners[loc] is defined if and only if loc has been accessed at least
once.  When a race is reported on loc, owners[loc] is set to null.
The fields in OwnerInfo relevant to the computation of the
happens-before relation are owner_tid, write_index, and read_indices.
The fields write_inst_id and read_inst_ids are used to keep track of
the access prior to the current access that participates in the
data-race. 

The algorithm maintains a singly-linked global list of SyncEvent
objects.  The head of this list is pointed to by earliest and the tail
by latest.  The field write_index points to an object in the list of
synchronization events.  The field read_indices is a map from thread
ids to objects in the list of synchronization events.  Each object in
this list corresponds to a thread action that is either the source or
a sink of a happens-before edge.  There is a union in the SyncEvent
structure.  For a SyncEvent e, if e.action \in {ACQUIRE, RELEASE} then
the lock field of the union is meaningful.  Otherwise, the tid field
of the union is meaningful.  The variable earliest points to the head
of the list and the variable latest points to an empty element at the
tailx of the list where the next synchronization event will be stored.

owners[loc]->owner_tid is the id of the last thread that wrote loc.
owners[loc]->write_index is an object in the list of synchronization
events.  The synchronization events from this reference (inclusive) to
the tail of the list (exclusive) happened since the last write access
to loc. Similarly, if owners[loc]->read_indices[tid] is defined, then
its value is an object in the list of synchronization events.  The
synchronization events from this reference (inclusive) to the tail of
the list (exclusive) happened since the last read access to loc by
thread tid.

We now describe how to determine whether the current access to loc
creates a data-race with the previous access.  Let the current access
be performed by the thread performing_tid.  There are two cases, one
for write accesses and one for read accesses.

(1) Suppose the access is a write access. In this case, if the map
read_indices is empty then we check for ownership transfer from
owners[loc]->owner_tid to performing_tid starting from
owners[loc]->write_index.  If the map read_indices is nonempty, then
for all tid such that owners[loc]->read_indices[tid] is defined we
check for ownership transfer from tid to performing_tid starting from
owners[loc]->read_indices[tid]. After the check, we set read_indices
to the empty map, set owner_tid to performing_tid, and set write_index
to the tail of the list of the synchronization events.

(2) Suppose the access is a read access.  In this case, if the map
read_indices contains a single entry mapping tid to event e in the
list of synchronization events, then we check for ownership transfer
from tid to performing_tid starting from e.  If the check fails or if
read_indices contains more than one entry, we check for ownership
transfer from owners[loc]->owner_tid to peforming_tid starting from
owners[loc]->write_index.  In either case, we add an entry to
read_indices mapping performing_tid to the tail of the list of
synchronization events.

The field ref_count is used to reclaim objects in the list of
synchronization events.  Whenever we add a reference to an event e in
the list from owners[loc]->write_index or from
owners[loc]->read_indices[tid] for some loc and tid, we increment
e->ref_count.  When we remove such a reference to e, we decrement
e->ref_count.  After each access, all the event objects at the
beginning of the list (starting from earliest) whose ref_count is 0
are freed and earliest is set to the first event e such that e->
ref_count is different from 0.
*/
#pragma once
#include "ChessStl.h"
//#include <assert.h>
//#include <iostream>
#include "SyncVar.h"

using namespace std;
//int GetSyncVarForThread(int i);

class Chess;

class TransitiveHappensBefore {
private:
	enum SyncAction {ACQUIRE, RELEASE, FORK, READ, WRITE, READWRITE};
	typedef void* MLOC;

#define NUM_SYNC_ACTIONS 6

	struct SyncEvent {
		SyncAction action;
		Task performing_tid;
		SyncVar resource;
		int ref_count;
		SyncEvent *next;
	};

	class OwnerInfo {
	public:
		Task owner_tid;
		SyncEvent *write_index;
		int write_inst_id;

		stdext::hash_map<Task, SyncEvent *> read_indices;
		stdext::hash_map<Task, int> read_inst_ids;
	};

	stdext::hash_map<MLOC, OwnerInfo *> owners;
	SyncEvent *earliest;
	SyncEvent *latest;

	stdext::hash_map<SyncVar, int *> sync_vars;

public:
	TransitiveHappensBefore() {
		earliest = latest = (SyncEvent *) malloc(sizeof(SyncEvent));
		latest->ref_count = 0;
	}

	~TransitiveHappensBefore(void) {
		free(earliest);
	}

	void RecordSyncVarStatistics(SyncVar var, SyncAction action)
	{
		if (sync_vars.find(var) == sync_vars.end())
		{
			sync_vars[var] = (int *) malloc(NUM_SYNC_ACTIONS * sizeof(int));
			for (int i = 0; i < NUM_SYNC_ACTIONS; i++)
				sync_vars[var][i] = 0;
		}
		sync_vars[var][action]++;
	}

	void PrintSyncVarStatistics();

	void ResetSyncVarStatistics()
	{
		stdext::hash_map<SyncVar, int *>::const_iterator iter;
		iter = sync_vars.begin();
		while (iter != sync_vars.end()) 
		{
			free(iter->second);
			iter++;
		}
		sync_vars.clear();
	}

	void Reset() {
		stdext::hash_map<MLOC, OwnerInfo *>::const_iterator iter;
		iter = owners.begin();
		while (iter != owners.end())
		{
			if (iter->second != NULL)
				delete(iter->second);
			iter++;
		}
		owners.clear();
        int count = 0;
		while (earliest != latest)
		{
			SyncEvent *temp = earliest;
			earliest = earliest->next;
			free(temp);
            count++;
		}
		latest->ref_count = 0;
		ResetSyncVarStatistics();
	}

	void Lock(int lock, Task performing_tid) {
		latest->action = ACQUIRE;
		latest->performing_tid = performing_tid;
		latest->resource = lock;
		SyncEvent *newEvent = (SyncEvent *) malloc(sizeof(SyncEvent)); 
		latest->next = newEvent;
		latest = newEvent;
		latest->ref_count = 0;

		RecordSyncVarStatistics(lock, ACQUIRE);
	}

	void Unlock(int lock, Task performing_tid) {
		latest->action = RELEASE;
		latest->performing_tid = performing_tid;
		latest->resource = lock;
		SyncEvent *newEvent = (SyncEvent *) malloc(sizeof(SyncEvent)); 
		latest->next = newEvent;
		latest = newEvent;
		latest->ref_count = 0;

		RecordSyncVarStatistics(lock, RELEASE);
	}

	void Fork(Task forked_tid, Task performing_tid) {
		latest->action = FORK;
		latest->performing_tid = performing_tid;
		latest->resource = forked_tid;
		SyncEvent *newEvent = (SyncEvent *) malloc(sizeof(SyncEvent)); 
		latest->next = newEvent;
		latest = newEvent;
		latest->ref_count = 0;

		RecordSyncVarStatistics(forked_tid, FORK);
	}

	void ReadSyncVar(SyncVar lock, Task performing_tid) {
		latest->action = READ;
		latest->performing_tid = performing_tid;
		latest->resource = lock;
		SyncEvent *newEvent = (SyncEvent *) malloc(sizeof(SyncEvent)); 
		latest->next = newEvent;
		latest = newEvent;
		latest->ref_count = 0;

		RecordSyncVarStatistics(lock, READ);
	}

	void WriteSyncVar(SyncVar lock, Task performing_tid) {
		latest->action = WRITE;
		latest->performing_tid = performing_tid;
		latest->resource = lock;
		SyncEvent *newEvent = (SyncEvent *) malloc(sizeof(SyncEvent)); 
		latest->next = newEvent;
		latest = newEvent;
		latest->ref_count = 0;

		RecordSyncVarStatistics(lock, WRITE);
	}

	void ReadWriteSyncVar(SyncVar lock, Task performing_tid) {
		latest->action = READWRITE;
		latest->performing_tid = performing_tid;
		latest->resource = lock;
		SyncEvent *newEvent = (SyncEvent *) malloc(sizeof(SyncEvent)); 
		latest->next = newEvent;
		latest = newEvent;
		latest->ref_count = 0;

		RecordSyncVarStatistics(lock, READWRITE);
	}

	void Access(MLOC loc, Task performing_tid, bool isWrite, int inst_id) {
		bool retVal;

		if (isWrite)
			retVal = CheckWriteAccess(loc, performing_tid, inst_id);
		else
			retVal = CheckReadAccess(loc, performing_tid, inst_id);


		while (earliest->ref_count == 0)
		{
			SyncEvent *temp;
			temp = earliest;
			earliest = earliest->next;
			free(temp);
		}

		/*
		* Race is now reported by the code that detects the conflict (CheckReadAccess and CheckWriteAccess)
		if (!retVal){
		OnRace(loc);
		//raceReporter.ReportRace(loc);
		//*GetChessErrorStream() << "Race Found for Loc 0x" << loc << std::endl;
		}
		*/
	}

private:
	bool CheckWriteAccess(MLOC loc, Task performing_tid, int inst_id);
	bool CheckReadAccess(MLOC loc, Task performing_tid, int inst_id);
	bool CheckOwnershipTransfer(Task current_owner, Task next_owner, SyncEvent *starting_index);

};
