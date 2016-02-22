/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessApi.h"

typedef size_t SyncVarOp;

#ifdef MCHESS
// define a managed enumeration class
public enum class MSyncVarOp : int { 

#else
// define a class SVOP containing an enumeration and helper methods
class CHESS_API SVOP {
public:
	static enum SVOPEnum {
#endif

		NULL_OP = 0,
		RWVAR_READWRITE = 1,
		LOCK_ACQUIRE    = 2,
		LOCK_TRYACQUIRE = 3,
		LOCK_RELEASE    = 4,

		TASK_FORK       = 5,
		TASK_BEGIN      = 6,
		TASK_END        = 7,
		TASK_JOIN       = 8,
		TASK_RESUME     = 9,
		TASK_SUSPEND    = 10,

		TASK_YIELD      = 11,
		WAIT_ANY        = 12,
		WAIT_ALL        = 13,

		RWAPCQ          = 14,
		RWIOCP          = 15,
		RWEVENT         = 16,
		FLT_RW          = 17,

		QUIESCENT_WAIT  = 18,

		RWVAR_READ = 19,
		RWVAR_WRITE = 20,

		SRWLOCK_ACQUIRE_EXCLUSIVE = 21,
		SRWLOCK_RELEASE_EXCLUSIVE = 22,
		SRWLOCK_ACQUIRE_SHARED = 23,
		SRWLOCK_RELEASE_SHARED = 24,

		DIO_RECEIVE,
		DIO_SEND,

		TASK_FENCE,

		DATA_READ,
		DATA_WRITE,
		CHOICE,

        TRACED_EVENT,

		MANUAL_SCHEDULE = 255,
		UNKNOWN = 256,
		INVALID = 257

#ifdef MCHESS
	};
#else
	};
	static bool IsReleaseOp(SyncVarOp op){
		return op == RWVAR_READWRITE 
			|| op == RWVAR_WRITE
			|| op == LOCK_RELEASE
			|| op == TASK_END
			//|| op == TASK_RESUME
			|| op == RWAPCQ
			|| op == RWIOCP
			|| op == RWEVENT
			|| op == FLT_RW
			|| op == SRWLOCK_RELEASE_EXCLUSIVE
			|| op == SRWLOCK_RELEASE_SHARED
			|| op == DIO_SEND
			|| op == UNKNOWN;
	}

	// by default everything is a read and a write unless specified otherwise

	static bool IsWrite(SyncVarOp op){
		return ! OpsThatDontWrite(op);
	}

	static bool IsRead(SyncVarOp op){
		return ! OpsThatDontRead(op);
	}

	static const char* ToString(SyncVarOp op);

private:
	static bool OpsThatDontWrite(SyncVarOp op){
		return op == TASK_BEGIN
			|| op == TASK_JOIN
			|| op == TASK_YIELD
			|| op == WAIT_ANY // Note: CHESS' WAIT_ANY does not write, even though Win32 WAIT_ANY does
			|| op == WAIT_ALL 
			|| op == RWVAR_READ
			;
	}

	static bool OpsThatDontRead(SyncVarOp op){
		return op == TASK_FORK
			|| op == TASK_END
			|| op == TASK_YIELD
			|| op == RWVAR_WRITE
			;
	}

};
#endif
