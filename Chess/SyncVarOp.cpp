/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "SyncVarOp.h"

const char* SVOP::ToString(SyncVarOp op){
	switch(op){
		case NULL_OP : return "NULL_OP";
		case RWVAR_READWRITE : return "RWVAR_READWRITE";
		case LOCK_ACQUIRE : return "LOCK_ACQUIRE";
		case LOCK_TRYACQUIRE : return "LOCK_TRYACQUIRE";
		case LOCK_RELEASE : return "LOCK_RELEASE";
		case TASK_FORK : return "TASK_FORK";
		case TASK_BEGIN : return "TASK_BEGIN";
		case TASK_END : return "TASK_END";
		case TASK_JOIN : return "TASK_JOIN";
		case TASK_RESUME : return "TASK_RESUME";
		case TASK_SUSPEND : return "TASK_SUSPEND";
		case TASK_YIELD : return "TASK_YIELD";
		case TASK_FENCE : return "TASK_FENCE";
		case WAIT_ANY : return "WAIT_ANY";
		case WAIT_ALL : return "WAIT_ALL";
		case RWEVENT : return "RWEVENT";
		case RWAPCQ : return "RWAPCQ";
		case RWIOCP : return "RWIOCP";
		case RWVAR_READ: return "RWVAR_READ";
		case RWVAR_WRITE: return "RWVAR_WRITE";
		case FLT_RW : return "FLT_RW";
		case QUIESCENT_WAIT : return "QUIESCENT_WAIT";
		case MANUAL_SCHEDULE : return "MANUAL_SCHEDULE";
		case DIO_RECEIVE: return "DIO_RECEIVE";
		case DIO_SEND: return "DIO_SEND";

		case DATA_READ: return "DATA_READ";
		case DATA_WRITE: return "DATA_WRITE";

		case CHOICE: return "CHOICE";

		case TRACED_EVENT: return "TRACED_EVENT";

		case UNKNOWN : return "UNKNOWN";
		case INVALID : return "INVALID";

		default : return "XXX";
	}
}
