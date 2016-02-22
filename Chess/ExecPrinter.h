/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessStl.h"
#include "IChessMonitor.h"
#include "SyncVar.h"
#include "ChessExecution.h"
#include "HashFunction.h"
#include "ChessOptions.h"

class ExecPrinter : public IChessMonitor{
public:
	ExecPrinter(const ChessOptions *options);
	virtual void OnExecutionEnd(IChessExecution* exec);
	virtual void OnShutdown();

private:
	void VerbosePrint(IChessExecution* exec);
	void OnGraphBegin();
	void OnNewNode(size_t id, size_t step, IChessExecution* exec);
	void OnNewEdge(size_t src, size_t dst, size_t step, IChessExecution* exec);
	void OnGraphEnd();

	int numExecs;
	std::vector<stdext::hash_map<Task, size_t> > edges;
	std::ofstream allExecs;
	std::ofstream fileStream;
	stdext::hash_map<HashValue, int> exploredExecs;
};