/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessTransition.h"
#include "IQueryEnabled.h"
#include <ostream>

class IChessExecution {
public:
	virtual ~IChessExecution(){}

	virtual size_t GetInitStack() const {return 0;}

	virtual size_t NumTransitions() const {return 0;}

	virtual ChessTransition Transition(size_t n) const{return ChessTransition();}

	virtual IQueryEnabled* GetQueryEnabled() const{return 0;}

	virtual std::ostream& operator<<(std::ostream& o){return o;}

};

inline std::ostream& operator<<(std::ostream& o, IChessExecution& e){
	return e.operator<<(o);
}