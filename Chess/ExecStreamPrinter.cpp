/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#if SINGULARITY

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif

#endif

#include "ExecStreamPrinter.h"
#include "ChessExecution.h"
#include "Chess.h"

ExecStreamPrinter::ExecStreamPrinter(std::ostream &out) :
	m_numExecs(0),
	m_out(out)
{
}

ExecStreamPrinter::~ExecStreamPrinter()
{
}

void ExecStreamPrinter::OnExecutionEnd(IChessExecution *exec)
{
	m_numExecs++;
	m_out << "==========Exec " << m_numExecs << "========================\n";
	m_out << *exec << std::endl;

}

void ExecStreamPrinter::OnShutdown()
{
}