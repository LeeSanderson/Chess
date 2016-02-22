/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "SyncVar.h"
#include <iostream>

class ChessTransition{
public:
	Task tid;
	SyncVar var;
	SyncVarOp op;

	ChessTransition(){tid = 0; var=0; op=0;}
	ChessTransition(Task t, SyncVar v, SyncVarOp o)
		: tid(t), var(v), op(o){}

	std::ostream& operator<<(std::ostream& o) const;
	std::istream& operator>>(std::istream& i);
};

inline std::ostream& operator<<(std::ostream& o, const ChessTransition& tr){return tr.operator<<(o);}
inline std::istream& operator>>(std::istream& i, ChessTransition& tr){return tr.operator>>(i);}
