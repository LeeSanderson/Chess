/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

// define basic type
typedef unsigned EventAttribute;

// define enumeration
#ifdef MCHESS
namespace MChess {
  public enum class EventAttributeEnum : int { 
#else
enum EventAttributeEnum {
#endif

	VAR_OP = 1,         // sync var op (as assigned by chess)
    VAR_ID,             // sync var id (as assigned by chess)

	STATUS,             // set to "b" (blocked), "p" (preempted), or "c" (committed)

	INSTR_METHOD,       // name of instrumented method, or general info about event

	THREADNAME,         // name of the thread
	STACKTRACE,         // stack trace, stringified

    DISPLAY_BOXED,      // set to "XXX" for frame of color XXX
    HBSTAMP,            // hbstamp in standard format

    EVT_SID,            // stack id

    ENABLE,             // enable a thread
    DISABLE,            // disable a thread

	LAST_ATTR           // last in list.. I use this for iterating over all
	
#ifdef MCHESS
  }; 
}
#else
};
#endif


