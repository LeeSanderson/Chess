/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Chess.h"
#include "TransitiveHappensBefore.h"

void TransitiveHappensBefore::PrintSyncVarStatistics()
{
	stdext::hash_map<SyncVar, int *>::const_iterator iter;
	iter = sync_vars.begin();
	while (iter != sync_vars.end()) 
	{
		std::cout << "Sync Var: " << iter->first << std::endl;
		std::cout << "# ACQUIRE operations: " << iter->second[ACQUIRE] << std::endl;
		std::cout << "# RELEASE operations: " << iter->second[RELEASE] << std::endl;
		std::cout << "# FORK operations: " << iter->second[FORK] << std::endl;
		std::cout << "# READ operations: " << iter->second[READ] << std::endl;
		std::cout << "# WRITE operations: " << iter->second[WRITE] << std::endl;
		std::cout << "# READWRITE operations: " << iter->second[READWRITE] << std::endl;
		iter++;
	}
}


bool TransitiveHappensBefore::CheckWriteAccess(MLOC loc, Task performing_tid, int inst_id) 
{
	bool retVal;
	OwnerInfo *info;

	if (owners.find(loc) == owners.end())
	{
		info = new OwnerInfo();
		info->owner_tid = performing_tid;
		info->write_index = latest;
		info->write_inst_id = inst_id;
		latest->ref_count++;
		info->read_indices.clear();
		info->read_inst_ids.clear();
		owners[loc] = info;
		return true;
	}

	info = owners[loc];
	if (info == NULL)
	{
		// a race on loc has already been reported
		return true;
	}

	SyncEvent *write_index = info->write_index;

	stdext::hash_map<Task, SyncEvent *>::const_iterator iter;
	if (info->read_indices.size() == 0)
	{
		assert(write_index != NULL);
		retVal = CheckOwnershipTransfer(info->owner_tid, performing_tid, write_index);
		if(!retVal){
			Chess::OnRace(loc, info->owner_tid, info->write_inst_id, performing_tid, inst_id, Chess::WRITE_WRITE);
		}
	}
	else
	{
		iter = info->read_indices.begin();
		retVal = true;
		while (iter != info->read_indices.end()) 
		{
			if (retVal){
				retVal = CheckOwnershipTransfer(iter->first, performing_tid, iter->second);
				if(!retVal){
					Chess::OnRace(loc, iter->first, info->read_inst_ids[iter->first], performing_tid, inst_id, Chess::READ_WRITE);
				}
			}
			assert(iter->second->ref_count >= 1);
			iter->second->ref_count--;
			iter++;
		}
	}

	if (write_index != NULL)
	{
		assert(write_index->ref_count >= 1);
		write_index->ref_count--;
	}

	if (retVal)
	{
		info->owner_tid = performing_tid;
		info->write_index = latest;
		info->write_inst_id = inst_id;
		latest->ref_count++;
		info->read_indices.clear();
		info->read_inst_ids.clear();
	}
	else
	{
		delete(info);
		owners[loc] = NULL;
	}

	return retVal;
}

bool TransitiveHappensBefore::CheckReadAccess(MLOC loc, Task performing_tid, int inst_id)
{
	bool retVal;
	OwnerInfo *info;

	if (owners.find(loc) == owners.end())
	{
		info = new OwnerInfo();
		info->write_index = NULL;
		info->write_inst_id = 0;
		info->read_indices.clear();
		info->read_indices[performing_tid] = latest;
		info->read_inst_ids[performing_tid] = inst_id;
		latest->ref_count++;
		owners[loc] = info;
		return true;
	}

	info = owners[loc];
	if (info == NULL)
	{
		// a race on loc has already been reported
		return true;
	}

	stdext::hash_map<Task, SyncEvent *>::iterator iter;
	iter = info->read_indices.find(performing_tid);
	if (iter != info->read_indices.end())
	{
		iter->second->ref_count--;
		info->read_indices[performing_tid] = latest;
		info->read_inst_ids[performing_tid] = inst_id;
		latest->ref_count++;
		return true;
	}

	size_t num = info->read_indices.size();
	SyncEvent *write_index = info->write_index;

	if (num != 1)
	{
		if (write_index == NULL)
			retVal = true;
		else{
			retVal = CheckOwnershipTransfer(info->owner_tid, performing_tid, write_index);
			if(!retVal){
				Chess::OnRace(loc, info->owner_tid, info->write_inst_id, performing_tid, inst_id, Chess::WRITE_READ);
			}
		}
	}
	else
	{
		iter = info->read_indices.begin();
		retVal = CheckOwnershipTransfer(iter->first, performing_tid, iter->second);
		if (retVal)
		{
			iter->second->ref_count--;
			info->read_indices.clear();
			info->read_inst_ids.clear();
		}
		else if (write_index == NULL)
			retVal = true;
		else{
			retVal = CheckOwnershipTransfer(info->owner_tid, performing_tid, write_index);
			if(!retVal){
				Chess::OnRace(loc, info->owner_tid, info->write_inst_id, performing_tid, inst_id, Chess::WRITE_READ);
			}
		}
	}

	if (retVal)
	{
		info->read_indices[performing_tid] = latest;
		info->read_inst_ids[performing_tid] = inst_id;
		latest->ref_count++;
	}
	else
	{
		if (write_index != NULL) 
		{
			assert(write_index->ref_count >= 1);
			write_index->ref_count--;
		}
		iter = info->read_indices.begin();
		while (iter != info->read_indices.end())
		{
			iter->second->ref_count--;
			iter++;
		}
		delete(info);
		owners[loc] = NULL;
	}

	return retVal;
}

#define TRANSITIVE 

#ifdef TRANSITIVE

bool TransitiveHappensBefore::CheckOwnershipTransfer(Task current_owner, Task next_owner, SyncEvent *starting_index)
{
	if (current_owner == next_owner)
		return true;

	stdext::hash_set<SyncVar> resource_set;
	SyncEvent *index = starting_index;
	bool retVal = false;
	resource_set.insert(current_owner);

	while (index != latest)
	{
		Task performing_tid = index->performing_tid;
		switch (index->action)
		{
		case ACQUIRE:
		case READ:
			if (resource_set.find(index->resource) != resource_set.end())
			{
				resource_set.insert(performing_tid);
				if (performing_tid == next_owner)
					retVal = true;
			}
			break;
		case RELEASE:
		case WRITE:
			if (resource_set.find(performing_tid) != resource_set.end())
				resource_set.insert(index->resource);
			break;
		case FORK:
			if (resource_set.find(performing_tid) != resource_set.end())
			{
				resource_set.insert(index->resource);
				if (index->resource == next_owner)
					retVal = true;
			}
			break;
		case READWRITE:
			if (resource_set.find(index->resource) != resource_set.end())
			{
				resource_set.insert(performing_tid);
				if (performing_tid == next_owner)
					retVal = true;
			}
			else if (resource_set.find(performing_tid) != resource_set.end())
				resource_set.insert(index->resource);
			break;
		}

		if (retVal)
			break;
		else
			index = index->next;
	}
	return retVal;
}

#else

bool TransitiveHappensBefore::CheckOwnershipTransfer(int current_owner, int next_owner, SyncEvent *starting_index)
{
	if (current_owner == next_owner)
		return true;

	hash_set<int> s;
	SyncEvent *index = starting_index;
	bool retVal = false;
	while (index != latest)
	{
		switch (index->action)
		{
		case ACQUIRE:
		case READ:
			if (index->performing_tid == next_owner && s.find(index->resource) != s.end())
				retVal = true;
			break;
		case RELEASE:
		case WRITE:
			if (index->performing_tid == current_owner)
				s.insert(index->resource);
			break;
		case FORK:
			if (index->performing_tid == current_owner && index->resource == next_owner)
				retVal = true;
			break;
		case READWRITE:
			if (index->performing_tid == next_owner && s.find(index->resource) != s.end())
				retVal = true;
			if (index->performing_tid == current_owner)
				s.insert(index->resource);
			break;
		}

		if (retVal)
			break;
		else
			index = index->next;
	}
	return retVal;
}

#endif