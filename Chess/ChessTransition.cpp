/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessTransition.h"
#include "SyncVarManager.h"

std::ostream& ChessTransition::operator<<(std::ostream& o) const{
	o << tid << ' ' << SyncVarWriter(var) << ' ' << op;
	return o;
}

std::istream& ChessTransition::operator>>(std::istream& i){
	i >> tid >> SyncVarReader(var) >> op;
	return i;
}
