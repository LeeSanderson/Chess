/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"
#include <windows.h>

class Semaphore{
public:
	Semaphore();
	void Up();
	void Down();
	void AtomicUpDown(Semaphore* downSem);
	void Init();
	void Clear();
	bool IsNull() const;
	bool TryDown();
	static bool InternalStateValid();
//private:
	HANDLE sem;
//	int signature;
public:
//	int val;// this is here only for debugging purposes
};

