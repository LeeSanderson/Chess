/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessStl.h"
#include "IChessMonitor.h"

#include "SyncVar.h"

class ExecStreamPrinter : public IChessMonitor
{
public:
	ExecStreamPrinter(std::ostream &out);
	virtual ~ExecStreamPrinter();

	virtual void OnExecutionEnd(IChessExecution* exec);
	virtual void OnShutdown();

private:
	int m_numExecs;
	std::ostream &m_out;
};