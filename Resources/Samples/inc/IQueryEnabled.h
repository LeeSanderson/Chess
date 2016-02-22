/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "SyncVar.h"

class IQueryEnabled{
public:
	// Before the execution of the ith transition (starting from 0)
	//  return the next task such that
	//     * next is enabled
	//     * next > curr in the order (curr < curr+1 ... < last < first ... curr-1)
	virtual bool NextEnabledAtStep(size_t step, Task curr, Task& next)
	{return false;}

	virtual bool IsEnabledAtStep(size_t step, Task tid){return false;}

	virtual int NumEnalbedAtStep(size_t step){
		int numEnabled = 0;
		size_t first;
		if(!NextEnabledAtStep(step, 0, first))
			return 0;

		size_t next = first;
		do{
			numEnabled++;
			NextEnabledAtStep(step, next, next);
		}while(next != first);
		return numEnabled;
	}

	virtual ~IQueryEnabled(){}
};