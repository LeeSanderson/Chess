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

#include "ChessExecution.h"
#include "EnabledSet.h"
#include "Chess.h"
#include "ChessImpl.h"
#include <sstream>
#include "ChessLog.h"
#include "ChessProfilerTimer.h"
#include "SyncVarManager.h"
#include "ObservationMonitor.h"
#include "windows.h"

using namespace std;

ChessExecution::ChessExecution(const Task tasks[], int n){
	enabled = new EnabledSet(this);
	assert(n > 0);
	for(int i=0; i<n; i++){
		initTasks.insert(tasks[i]);
	}
	recordIndex = 0;
	initStackSize = 0;
	initExecState =0;
	recoveryValid = false;
	Reset();
	enabled->Reset(initTasks);
	initExecState =0;
	taskWaitingForQuiescence = 0;
}

ChessExecution::~ChessExecution(){
	if(initExecState){
		delete initExecState;
		initExecState = 0;
	}
	delete enabled;
	enabled = 0;
}

void ChessExecution::Reset(){
	preemptionDisableCount.clear();
	if(initStackSize != 0){
		SetExecState(initExecState);
		return;
	}
	topIndex = 0;
	eventIndex = 0;
	lookahead.clear();
	std::set<Task>::iterator i;
	for(i = initTasks.begin(); i != initTasks.end(); i++){
		AddLookahead(ChessTransition(*i, *i, SVOP::TASK_BEGIN));
	}
}

// Moved most of clone method to copy constructor so the BestFirstExecution can make 
// use of it during its own clone method (BFS)
ChessExecution::ChessExecution(ChessExecution* exec) {
	std::set<Task>::const_iterator itr = exec->initTasks.begin();
	for (itr; itr != exec->initTasks.end(); itr++) {
		initTasks.insert(*itr);
	}
	enabled = new EnabledSet(this);
	initStackSize = 0;
	Reset();
	taskWaitingForQuiescence = exec->taskWaitingForQuiescence;
	initStackSize = exec->initStackSize;
	stack = exec->stack;
	topIndex = exec->topIndex;
	events = exec->events;
	eventIndex = exec->eventIndex;
	initExecState = new (std::nothrow) ChessExecState(exec->initExecState);
	recordIndex = exec->recordIndex;
	enabled = new (std::nothrow) EnabledSet(this);
	if (enabled != NULL) {
		enabled->Reset(initTasks);
	}
	lookahead = exec->lookahead;
	recoveryValid = exec->recoveryValid;
	recoveryStep = exec->recoveryStep;
	recoveryTask = exec->recoveryTask;
	perThreadStepNum = exec->perThreadStepNum;
	preemptionDisableCount = exec->preemptionDisableCount;
}

// moved most of this method's contents to a copy constructor (BFS)
ChessExecution* ChessExecution::Clone(){
	ChessExecution* ret = new ChessExecution(this);
	if(ret->initExecState == 0){
		delete ret;
		return 0;
	}
	if(ret->enabled == NULL){
		delete ret->initExecState;
		delete ret;
		return 0;
	}

	return ret;
}

ChessExecState* ChessExecution::GetExecState(){
	ChessExecState* ret = new ChessExecState();
	ret->topIndex = topIndex;
	ret->eventIndex = eventIndex;
	ret->lookahead = lookahead;
	return ret;
}

void ChessExecution::SetExecState(const ChessExecState* state){
	topIndex = state->topIndex;
	eventIndex = state->eventIndex;
	lookahead = state->lookahead;
	assert(topIndex <= stack.size());
}

void ChessExecution::SetInitStack(){
	initStackSize = topIndex;
	initExecState = GetExecState();
	recordIndex = topIndex;
}

int ChessExecution::Choose(Task tid, int numChoices)
{
	ChessProfilerSentry s("ChessExecution::Choose");
	if(InReplayMode()){
		ChessTransition& top = stack[topIndex];
		assert (top.tid == tid && top.op == SVOP::CHOICE);
		topIndex++;
		return top.var;
	}
	else {
		ChessTransition trans(tid, numChoices-1, SVOP::CHOICE);
		stack.push_back(trans);
		topIndex++;
		return numChoices-1;
	}
}

void ChessExecution::PreemptionDisable(Task tid){
	if (InRecordMode()) {
		size_t sid = stack.size()-1;
		assert (sid >= 0);
		events.push_back(ExecEvent(tid, ExecEvent::PREEMPTION_DISABLE, sid));
	}
	preemptionDisableCount[tid]++;
}

void ChessExecution::PreemptionEnable(Task tid){
	if (InRecordMode()) {
		size_t sid = stack.size()-1;
		assert (sid >= 0);
		events.push_back(ExecEvent(tid, ExecEvent::PREEMPTION_ENABLE, sid));
	}
	assert (preemptionDisableCount[tid] > 0);
	preemptionDisableCount[tid]--;
}

int ChessExecution::SyncVarAccess(Task tid, SyncVar var, SyncVarOp op){
	ChessTransition trans(tid, var, op);
#if !SINGULARITY
	if(Chess::GetOptions().do_random && InRecordMode() && (op < SVOP::TASK_FORK || op > SVOP::TASK_YIELD) && rand()%10 == 0){
		AddLookahead(trans);
		return false;
	}
#endif
	return SyncVarAccess(trans);
}

void ChessExecution::ToString() {
	for(size_t i = 0 ; i<stack.size() ; i++ ) {
		std::cout << "Schedule " << i << " "<< stack[i].tid << " " << SyncVarWriter(stack[i].var) << " " << SVOP::ToString(stack[i].op) << std::endl;
	}
	std::cout<<std::endl;
}


// An opportunity to update state/detect non-determinism (for best-first search) upon commit (BFS)
bool ChessExecution::CommitSyncVarAccess(int nr) {
	if (InRecordMode()) {
		perThreadStepNum.push_back(nr);
	}
	return true;
}

void ChessExecution::PruneExecution(size_t index){
	stack.resize(index);
	// prune the per thread step numbers, as well (BFS)
	if (perThreadStepNum.size() > index) {
	  perThreadStepNum.resize(index);
	}
	for(size_t i = 0; i<events.size(); i++){
		if(events[i].sid >= stack.size()){
			events.resize(i);
			break;
		}
	}
	recordIndex = stack.size();
	topIndex = index;
}

int ChessExecution::SyncVarAccess(ChessTransition& trans){
	ChessProfilerSentry s("ChessExecution::SyncVarAccess");
	// We are executing transition with trid == topIndex
	// We are currently in the state with sid == topIndex

	// Eagerly update the Enabled state
	AddLookahead(trans);
	enabled->UpdateEnabled(topIndex);

	if(InReplayMode()){
		assert(!ChessImpl::GetOptions().PCT);

		ChessTransition& top = stack[topIndex];
		if(top.tid != trans.tid){
			// expect a context switch
			AddLookahead(trans);
			// if we're replaying a local backtrack, tell monitors about the event so
			// that blocks will show up in the ConcurrencyExplorer even in replay
			// mode (BFS)
			for (size_t i = 0; i < events.size(); i++) {
				if (events[i].sid == topIndex && events[i].eid == ExecEvent::LOCALBACKTRACK && events[i].tid == stack[topIndex-1].tid) {
					{
						ChessImpl::SetNextEventAttribute(STATUS, "b");
					}
				}
			}
			return REQUIRE_CONTEXT_SWITCH;
		}
		// check for nondeterminism
		// XXX: When changing the logic for detecting nondeterminism
		//  make sure the check in NondeterminismHandler.cpp is consistent
		if(top.op != trans.op){ // vars can change
			return REQUIRE_NONDETERMINISM_PROCESSING;
		}
		if(top.var != trans.var){
			top.var = trans.var;
		}
		topIndex++;
		return SUCCESS;
	} else {
		if(IsFairBlocked(trans.tid)){
			AddLookahead(trans);
			return REQUIRE_CONTEXT_SWITCH;
		}
		stack.push_back(trans);
		topIndex++;
		return SUCCESS;
	}
}


bool ChessExecution::LocalBacktrack(){
	if(!InRecordMode()){
		// Nondeterminism
		return false;
	}
	topIndex--;
	ChessTransition top = stack[topIndex];
	stack.resize(topIndex);
	AddLookahead(top);
	events.push_back(ExecEvent(top.tid, ExecEvent::LOCALBACKTRACK, topIndex, top.var, top.op));
	assert(InRecordMode());
	return true;
}

void ChessExecution::MarkTimeout(){
	if(InRecordMode()){
		// <state sid> -- trans --> <state sid+1>
		// MarkTimeout during trans means that trans.tid incurred a timeout in state sid
		// topIndex points to sid+1 
		assert(topIndex > 0);
		size_t sid = topIndex-1;
		ChessTransition trans = stack[sid];
		assert(events[events.size()-1].sid <= sid); // make sure events is still ordered by sid
		events.push_back(ExecEvent(trans.tid, ExecEvent::TIMEOUT, sid));
	}
	else{
		// Check for nondeterminism
	}
}

bool ChessExecution::TaskTimedOutAtStep(size_t step, Task tid){
	// need a faster way
	for(size_t i=0; i<events.size(); i++){
		if(events[i].sid == step && events[i].tid == tid && events[i].eid == ExecEvent::TIMEOUT)
			return true;
	}
	return false;
}

bool ChessExecution::IsFairBlocked(Task tid){
	return enabled->IsFairBlocked(topIndex, tid);
}

// return an int rather than a bool so we can detect non-determinism during this method in the
// best-first search case (BFS)
int ChessExecution::NextTaskToSchedule(Task& next){
	if(InReplayMode()){
		next = stack[topIndex].tid;
	}
	else{
		Task curr = Task(0); //default
		if(topIndex != 0){
			curr = stack[topIndex-1].tid;
			if(enabled->IsEnabledAtStep(topIndex, curr)){
				// due to fairness and local backtrack
				// curr might get reenabled
				next = curr;
				return SUCCESS;
			}
		}
		if(!enabled->NextEnabledAtStep(topIndex, curr, next)){
			//deadlock
			// but not if there is a task waiting for quiescence
			Task taskwfq;
			if(IsTaskWaitingForQuiescence(taskwfq)){
				// reached quiescence
				QuiescentWakeup(taskwfq);
				next = taskwfq;
				return SUCCESS;
			}

			return FAILURE; // deadlock
		}
#if !SINGULARITY
		if(Chess::GetOptions().do_random){
			Task iter = next;
			std::vector<Task> en;
			do{
				en.push_back(iter);
				enabled->NextEnabledAtStep(topIndex, iter, iter);
			}while(next != iter);
			next = en[rand()%en.size()];
		}
#endif
	}
//	assert(lookahead.find(next) != lookahead.end());
	assert(lookahead[next].tid != 0); // not a null transition
	ChessTransition access = lookahead[next];
	assert(access.tid == next);
//	lookahead.erase(next);
	lookahead[next] = ChessTransition();
	//bool ret = SyncVarAccess(access);
	//assert(ret);
	return SUCCESS;
}

bool ChessExecution::Backtrack(size_t step, Task next){
	assert(stack.size() >= step);
	assert(stack.size() == topIndex);
	if(initStackSize && step < initStackSize){
		return false;
	}
	while(topIndex > step){
		topIndex--;
		const ChessTransition& top = stack[topIndex];
		AddLookahead(top);
	}
	// Update the per thread step indexes as well (BFS)
	if (next != Task(0) && stack[step].op == SVOP::CHOICE) {
		stack.resize(step+1);
		perThreadStepNum.resize(step+1);
	} else {
		stack.resize(step);
		perThreadStepNum.resize(step);
	}

	size_t i;
	for(i = 0; i<events.size(); i++){
		if(events[i].sid > stack.size()){
			events.resize(i);
			break;
		} else if (events[i].sid == stack.size() && events[i].eid == ExecEvent::PREEMPTION_DISABLE) {
			events.resize(i);
			break;
		} else if (events[i].sid == stack.size() && events[i].eid == ExecEvent::PREEMPTION_ENABLE) {
			events.resize(i);
			break;
		}
	}

	recordIndex = stack.size();

	// enabled->Backtrack(step);
	enabled->Backtrack(stack.size());

	if(next == Task(0))
		return true;

	if (stack.size() == step+1)
	{
		assert (stack[step].tid == next);
		assert (stack[step].op == SVOP::CHOICE);
		stack[step].var--; // var encodes the choice number; we start with numChoices-1 and go down to 0
		recordIndex--; // because we are not adding one more transition on in this case (BFS)
		perThreadStepNum.resize(recordIndex); // per thread step index should be one behind the size of stack in replay.
		return true;
	}

	// To schedule next at step
	assert(lookahead[next].tid != 0);
	ChessTransition access = lookahead[next];
	assert(access.tid == next);
	lookahead[next] = ChessTransition();

	//Push the access into the stack
	stack.push_back(access);
	topIndex++;

	//Note: we are not incrementing recordIndex
	//  This is because the new transition pushed into the stack has not been explored yet
	
	lookahead.clear();
	return true;
}

bool ChessExecution::Recover(){
	if(!recoveryValid)
		return false;
	*GetChessOutputStream() << "Recover To " << recoveryStep << ' ' << recoveryTask << "\n";
	bool ret = Backtrack(recoveryStep, recoveryTask); 
	recoveryValid = false;
	return ret;
}

void ChessExecution::BacktrackToInitStack(){
	topIndex = stack.size(); // because Reset is called previously on Backtrack()
	bool ret = Backtrack(initStackSize, Task(0));
	assert(ret);
	//Reset();
	//assert(topIndex == initStackSize);
	//enabled->Backtrack(topIndex);
	//stack.resize(topIndex);
}

void ChessExecution::QuiescentWakeup(Task tid){
	if(InRecordMode()){
		events.push_back(ExecEvent(tid, ExecEvent::QUIESCENT_WAKEUP, topIndex));
	}
}

void ChessExecution::EnterQuiescence(){
	assert(!"EnterQuiescence not yet implemented");
}
bool ChessExecution::ReachedQuiescence(){
	// Reached quiescence in a state only if QUIESCENT_WAKUP event is
	// scheduled in the state. We want to see if we have reached quiescence
	// in the state before the current transition QUIESCENT_WAIT
	assert(topIndex > 0);
	assert(stack[topIndex-1].op == SVOP::QUIESCENT_WAIT);
	for(size_t i = 0; i<events.size(); i++){
		if(events[i].sid == topIndex-1 && events[i].eid == ExecEvent::QUIESCENT_WAKEUP){
			return true;
		}
	}			
	return false;
}

bool ChessExecution::IsTaskWaitingForQuiescence(Task& taskwfq) const{
	if(taskWaitingForQuiescence == Task(0))
		return false;
	taskwfq = taskWaitingForQuiescence;
	return true;
}


std::ostream& ChessExecution::operator<<(std::ostream& o){
	int trans = 0;
	for(size_t i=0; i<stack.size(); i++){
		// print enabled
		o << "\t\t\t en = {";
		Task first;
		if(enabled->NextEnabledAtStep(i, 0, first)){
			Task t = first;
			do{
				o << t << ' ';
				enabled->NextEnabledAtStep(i, t, t);
			}while(t != first);
		}
		o << "}\n";

		const ChessTransition& t = stack[i];
		o << "[" << trans++ << "]: " 
			<< t.tid << ' ' << SyncVarWriter(t.var) << ' ' << SVOP::ToString(t.op) << '\n';
	}
	return o;
}

IQueryEnabled* ChessExecution::GetQueryEnabled() const {
	return enabled;
}


bool ChessExecution::GetLookaheadAtStep(size_t step, Task tid, ChessTransition& result){
	assert(0 <= step && step <= stack.size());

	// make sure this only looks in the stack from 0 to topIndex. 
	// the lookahead might not be accurate beyond topIndex as the vars might be different

	//easy case 
	if(step < topIndex && stack[step].tid == tid){
		result = stack[step];
		return true;
	}

	//linear and slow check
	for(size_t i=step; i<topIndex; i++){
		if(stack[i].tid == tid){
			result = stack[i];
			return true;
		}
	}

	if(lookahead[tid].tid != 0){
//	if(lookahead.find(tid) != lookahead.end()){
		result = lookahead[tid];
		return true;
	}
	return false;
}


bool ChessExecution::PrevTransitionOfTask(size_t step, Task tid, size_t& index){
	assert(0 <= step && step <= stack.size());
	while (step > 0)
	{
		step--;
		if(stack[step].tid == tid){
			index = step;
			return true;
		}
	}
	return false;
}

// For computing the correct timestamp at a step.  I meant to check and see if this is really necessary
// or if I am crazy but I never got around to it.  I think the problem was that when a step neither read
// nor wrote, the timestamp you got would be all zeros and thus it would look like it happened-before
// everything and that messed up DPOR (BFS)
bool ChessExecution::PrevHbTransitionOfTask(size_t step, Task tid, size_t& index) const {
	assert(step <= stack.size());
	if (step == 0) return false;
	for (size_t i = step-1; i >= 0; i--) {
		ChessTransition trans = stack[i];
		if (trans.tid == tid &&
			((SVOP::IsRead(trans.op) || SVOP::IsWrite(trans.op)) && trans.var != 0)) {
			index = i;
			return true;
		}
		if (i == 0) {
			return false;
		}
	}
	return false;
}

void ChessExecution::Serialize(std::ostream& f){		
	//f << "// Init Tasks: <num> <tk_1> ... <tk_num> \n";

	{
	size_t initTasksSize = initTasks.size();
	f.write((char*)&initTasksSize, sizeof(size_t));


	for(std::set<Task>::iterator ti = initTasks.begin(); ti != initTasks.end(); ti++){
		size_t task = *ti;
		f.write((char*)&task, sizeof(size_t));
	}

//	f << "// Transitions:  <num> <tr_1> <tr_2> ... <tr_num> \n" ;
	size_t stackSize = stack.size();
	f.write((char*)&stackSize, sizeof(size_t));

	for(size_t i =0; i<stack.size(); i++){
		ChessTransition tr = stack[i];
		f.write((char*)&tr, sizeof(ChessTransition));
	}

	//f << "// Exec Events: <num> <tid_1> <eid_1> <step_1> ... <tid_num> <eid_num> <step_num> \n";
	size_t eventsSize = events.size();
	f.write((char*)&eventsSize, sizeof(size_t));

	for(size_t i=0; i<events.size(); i++){
		ExecEvent execEvent = events[i];
		f.write((char*)&execEvent, sizeof(ExecEvent));
	}
	}
	// XXX
	// This is bad only in the case we try to read this sched file back
	//if(f.bad()){
	//	Chess::AbnormalExit(-1, "Cannot serialize schedule");
	//}

}

void ChessExecution::Deserialize(std::istream& f)
{
	size_t initTasksSize = 0;
	f.read((char*)&initTasksSize, sizeof(size_t));
	initTasks.clear();
	for(size_t i=0; i<initTasksSize; i++){
		Task t;
		f.read((char*)&t, sizeof(size_t));
		if(f.fail()) goto failure;
		initTasks.insert(t);
	}

	size_t stackSize = 0;
	f.read((char*)&stackSize, sizeof(size_t));
	if(f.fail()) goto failure;

	stack.clear();
	stack.reserve(stackSize);

	for(size_t i=0; i<stackSize; i++){
		ChessTransition t;
		f.read((char*)&t, sizeof(ChessTransition));
		if(f.fail())goto failure;
		stack.push_back(t);
		perThreadStepNum.push_back(0); // include per thread step number for init stack (BFS)
	}
	topIndex = stack.size();
	// record index is where we first transition into previously unexplored territory (BFS)
	recordIndex = stack.size()-1;
	perThreadStepNum.resize(recordIndex);

	size_t eventsSize = 0;
	f.read((char*)&eventsSize, sizeof(size_t));
	if(f.fail()) goto failure;

	events.clear();
	for(size_t i=0; i<eventsSize; i++){
		ExecEvent execEvent;
		f.read((char*)&execEvent, sizeof(ExecEvent));
		if(f.fail()) goto failure;
		events.push_back(execEvent);
	}
	recoveryValid = false; // not impl

	// catchup enabled
	enabled->Reset(initTasks);

	return;

failure:
	Chess::AbnormalExit(-1, "Invalid schedule file");
	return;

}

// Returns true if scheduling next at step requires a preemption for this execution (BFS)
bool ChessExecution::RequiresPreemption(size_t step, Task next) const {
	if (step == 0) {
		return false;
	}
	Task curr = stack[step-1].tid;
	return (curr != next) && enabled->IsEnabledAtStep(step, curr);
}

//void ChessExecution::SerializeText(std::ostream& f){
//	//assert(topIndex == stack.size());
//
//	f << "// Init Tasks: <num> <tk_1> ... <tk_num> \n";
//	f << initTasks.size() << '\n';
//	for(std::set<Task>::iterator ti = initTasks.begin(); ti != initTasks.end(); ti++){
//		f << *ti << '\n';
//	}
//	//f << '\n';
//
//	f << "// Transitions:  <num> <tr_1> <tr_2> ... <tr_num> \n" ;
//	f << stack.size() << '\n';
//	for(size_t i =0; i<stack.size(); i++){
//		f << stack[i] << '\n';
//	}
//
//	f << "// Exec Events: <num> <tid_1> <eid_1> <step_1> ... <tid_num> <eid_num> <step_num> \n";
//	f << events.size() << '\n';
//	for(size_t i=0; i<events.size(); i++){
//		f << events[i].tid << ' ' << events[i].eid << ' ' << events[i].sid << ' ' << SyncVarWriter(events[i].var) << ' ' << events[i].op << '\n';
//	}
//
//	f << "// Recovery Point: <valid?> <step> <task> \n";
//	f << recoveryValid << ' ' << (recoveryValid?recoveryStep:0) << ' ' << (recoveryValid?recoveryTask:0) << '\n';
//
//	f << std::endl;
//	f << '\n';
//	
//	f << "// Lookahead: <num> <tk_1> <tr_1> ... <tk_num> <tr_num>\n"; 
//	f << lookahead.size() << '\n';
//	for(size_t i=0; i<lookahead.size(); i++){
//		f << i << ' ' << lookahead[i] << '\n';
//	}
//	//stdext::hash_map<Task, ChessTransition>::iterator li;
//	//for(li = lookahead.begin(); li != lookahead.end(); li++){
//	//	f << li->first << ' ' << li->second << '\n';
//	//}
//	f << '\n';
//}
//
//void ChessExecution::Deserialize(std::istream& f)
//{
//	int num;
//	std::string junk;
//	getline(f, junk); //read the comment for InitTasks
//	f >> num;
//	if(!f.good() || num < 0) goto fail;
//	initTasks.clear();
//	for(int i=0; i<num; i++){
//		Task t;
//		f >> t;
//		initTasks.insert(t);
//	}
//
//	getline(f, junk);
//	getline(f, junk);
//	f >> num;
//	if(!f.good() || num < 0) goto fail;
//	stack.clear();
//	stack.reserve(num);
//	for(int i=0; i<num; i++){
//		ChessTransition t;
//		f >> t;
//		stack.push_back(t);
//	}
//	topIndex = stack.size();
//
//	//getline(f,junk);
//	//getline(f,junk);
//	//f >> num;
//	//if(!f.good() || num < 0) goto fail;
//	//lookahead.clear();
//	//for(int i=0; i<num; i++){
//	//	Task t;
//	//	ChessTransition tr;
//	//	f >> t;
//	//	f >> tr;
//	//	lookahead[t] = tr;
//	//}
//
//	getline(f,junk);
//	getline(f,junk);
//	f >> num;
//	if(!f.good() || num < 0) goto fail;
//	events.clear();
//	for(int i=0; i<num; i++){
//		Task tid;
//		size_t sid;
//		int eid;
//		SyncVar var;
//		SyncVarOp op;
//		f >> tid;
//		f >> eid;
//		f >> sid;
//		f >> SyncVarReader(var);
//		f >> op;
//		events.push_back(ExecEvent(tid, (ExecEvent::EventId)eid, sid, var, op));
//	}
//
//	getline(f, junk);
//	getline(f, junk);
//	f >> recoveryValid;
//	if(recoveryValid){
//		f >> recoveryStep;
//		f >> recoveryTask;
//	}
//
//	if(!f.good()) goto fail;
//
//	// catchup enabled
//	enabled->Reset(initTasks);
//	//for(size_t i=0; i<events.size(); i++){
//	//	switch(events[i].eid){
//	//		case ExecEvent::LOCALBACKTRACK : 
//	//			{
//	//				size_t sid = events[i].sid;
//	//				Task tid = events[i].tid;
//	//				const ChessTransition& trans = GetLookaheadAtStep(sid, tid);
//	//				enabled->DisableTask(sid, trans);
//	//				break;
//	//			}
//	//		case ExecEvent::YIELD :
//	//			{
//	//				size_t sid = events[i].sid;
//	//				Task tid = events[i].tid;
//	//				enabled->TaskYield(sid, tid);
//	//				break;
//	//			}
//	//		default:
//	//			goto fail;
//	//	}
//	//}
//
//	return;
//
//fail:
//	*GetChessErrorStream() << "Deserialize of ChessExecution failed" << std::endl;
//	assert(!"Deserialize of ChessExecution Failed");
//}
//
//
//
