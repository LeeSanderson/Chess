/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessAssert.h"
#include "Chess.h"
#include <sstream>


bool inChessAssert = false;

void ChessAssert::ChessAssertFn(const char * _Message, const char *_File, unsigned int _Line){
	if(inChessAssert){
		// Asserting in Assert - really screwed up
		exit(-1);
	}
	inChessAssert = true;
	std::ostringstream str;
	str << "Chess Internal Assertion \"" << _Message << "\" failed at " << _File << ":" << _Line << std::endl;
	if(!Chess::GetOptions().nopopups)
		Chess::GetSyncManager()->DebugBreak();
	Chess::AbnormalExit(CHESS_EXIT_INTERNAL_ERROR, str.str().c_str());
	inChessAssert = false;
}
