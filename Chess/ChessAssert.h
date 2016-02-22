/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessApi.h"

#define USE_CHESS_ASSERT

class CHESS_API ChessAssert{
public:
	static void ChessAssertFn(const char * _Message, const char *_File, unsigned _Line);
};


#ifdef USE_CHESS_ASSERT

#ifdef  NDEBUG

#define assert(_Expression)     ((void)0)

#else

#define assert(_Expression) (void)( (!!(_Expression)) || (ChessAssert::ChessAssertFn(#_Expression, __FILE__, __LINE__), 0) )

#endif  /* NDEBUG */

#else
//#include <assert.h>
#endif /* USE_CHESS_ASSERT */