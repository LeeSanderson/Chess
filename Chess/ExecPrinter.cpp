/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ExecPrinter.h"
#include "ChessExecution.h"
#include "Chess.h"
#include "HBExecution.h"
#include <iomanip>

ExecPrinter::ExecPrinter(const ChessOptions *options){
	allExecs.open(std::string(options->output_prefix).append("allexecs.txt").c_str());
	numExecs = 0;
}

void ExecPrinter::OnExecutionEnd(IChessExecution *exec){
	numExecs++;
	allExecs << std::setw(3) << numExecs << '|';

	HBExecution hb(exec);
	if(exploredExecs.find(hb.GetHash()) == exploredExecs.end()){
		exploredExecs[hb.GetHash()] = numExecs;
		allExecs << '*' << std::setw(3) << exploredExecs.size();
	}
	else{
		allExecs << '~' << std::setw(3) << exploredExecs[hb.GetHash()];
	}
	allExecs << '|' << std::setw(11) << hb.GetHash() << '|';
	for(size_t i=0; i<exec->NumTransitions(); i++){
		allExecs << exec->Transition(i) << '|';
	}
	allExecs << std::endl;
}

void ExecPrinter::VerbosePrint(IChessExecution* exec){
	allExecs << "==========Exec " << numExecs << "========================\n";
	allExecs << *exec << std::endl;

	//allExecs << "Strategy States \n";
	//for(size_t sid = 0; sid <= exec->NumTransitions(); sid++){
	//	allExecs << "[" << sid << "]" << Chess::GetStrategy()->DebugState(sid) << "\n";
	//}
	allExecs.flush();

	if(false){
		size_t n = 0;
		if(edges.size() == 0){
			edges.push_back(stdext::hash_map<Task, size_t>());
			OnNewNode(n, 0, exec);
		}

		for(size_t i = 0; i<exec->NumTransitions(); i++){
			ChessTransition curr = exec->Transition(i);
			if(edges[n].find(curr.tid) == edges[n].end()){
				size_t s = edges.size();
				edges.push_back(stdext::hash_map<Task, size_t>());
				OnNewNode(s, i+1, exec);
				edges[n][curr.tid] = s;
				OnNewEdge(n,s,i,exec);
			}
			n = edges[n][curr.tid];
		}
		fileStream << n << "->\"ex"<<numExecs << "\";" << std::endl;
		fileStream.flush();
	}
}

void ExecPrinter::OnNewNode(size_t id, size_t step, IChessExecution* exec){
	fileStream << id << " [label = \"en=[";
	IQueryEnabled* qEnabled = exec->GetQueryEnabled();
	Task firstEnabled;
	if(qEnabled->NextEnabledAtStep(step, Task(0), firstEnabled)){
		Task enTask = firstEnabled;
		do{
			fileStream << enTask << " ";
			qEnabled->NextEnabledAtStep(step, enTask, enTask);
		}while(enTask != firstEnabled);
	}
	fileStream << "] " 
		//<< Chess::GetStrategy()->DebugState(step) << "\"];" 
		<< std::endl;
}

void ExecPrinter::OnNewEdge(size_t src, size_t dst, size_t step, IChessExecution* exec){
	fileStream << src << "->" << dst 
		<< " [label= \"(" 
		<< exec->Transition(step).tid << "," 
		<< exec->Transition(step).var << "," 
		<< SVOP::ToString(exec->Transition(step).op) << ")\"];" << std::endl;
}

void ExecPrinter::OnGraphBegin(){
	fileStream << "digraph {" << std::endl;
}

void ExecPrinter::OnGraphEnd(){
	fileStream << "}" << std::endl;
	fileStream.close();
}

void ExecPrinter::OnShutdown(){
//	OnGraphEnd();
	allExecs.close();
}