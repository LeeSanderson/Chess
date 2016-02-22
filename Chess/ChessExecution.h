/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"
#include "SyncVar.h"
#include "ChessAssert.h"
#include "ChessTransition.h"
#include "IChessExecution.h"
#include "IQueryEnabled.h"
#include "SyncVarVector.h"

class EnabledSet;

// A ChessTransition is an access of a SyncVar by a Task denoted by <tid, var, op>
// A ChessExecution is a sequence of transitions

struct ExecEvent{
	enum EventId{ LOCALBACKTRACK, QUIESCENT_WAKEUP, PREEMPTION_DISABLE, PREEMPTION_ENABLE, TIMEOUT};
	Task tid;
	EventId eid;
	size_t sid; 
	// Task tid executed event eid at state sid
	SyncVar var;   // the var and op are required for LOCALBACKTRACK
	SyncVarOp op;

	ExecEvent(){}
	ExecEvent(Task t, EventId e, size_t s)
		: tid(t), eid(e), sid(s), var(SyncVar()), op(0){}
	ExecEvent(Task t, EventId e, size_t s, SyncVar v, SyncVarOp o)
		: tid(t), eid(e), sid(s), var(v), op(o) {}
};

// moved this class to the .h file rather than the .cpp file so it would be defined (Katie)
class ChessExecState{
public:
//	stdext::hash_map<Task, ChessTransition> lookahead;
	TaskVector<ChessTransition> lookahead;
	size_t topIndex;
	size_t eventIndex;
private:
	ChessExecState(){
	}
	ChessExecState(ChessExecState* c){
		lookahead = c->lookahead;
		topIndex = c->topIndex;
		eventIndex = c->eventIndex;
	}
	friend class ChessExecution;
};

class ChessExecution : public IChessExecution{
public:
	ChessExecution(const Task initTasks[], int n);
	// Added copy constructor so the BestFirstExecution can make use of it during clone (Katie)
	ChessExecution(ChessExecution* exec);
	~ChessExecution();

	void SetInitStack();
	size_t GetInitStack() const {return initStackSize;}
	// virtual so the best-first search can override it (Katie)
	virtual void Reset();

	static const int SUCCESS = 0;
	static const int REQUIRE_CONTEXT_SWITCH = 1;
	static const int REQUIRE_NONDETERMINISM_PROCESSING = 2;
	// for the case where we've detected deadlock when returning the next task to schedule (Katie)
	static const int FAILURE = 3;

	// Task tid is about to perform operation op on var
	// SyncVarAccess permits tid to perform the operation by returning SUCCESS
	// Failure indicates either a need for a context switch or nondeterminism processing
	int SyncVarAccess(Task tid, SyncVar var, SyncVarOp op);
	// An opportunity to perform various operations upon commit, and for the best-first execution
	// to detect non-determinism/update state (Katie)
	virtual bool CommitSyncVarAccess(int nr);

	int Choose(Task tid, int numChoices);

	void PreemptionDisable(Task tid);
	void PreemptionEnable(Task tid);
	TaskVector<size_t> GetPreemptionDisableCount() {
		return preemptionDisableCount;
	}

	void ToString();

	// virtual so the best-first search can override it (Katie)
	virtual bool LocalBacktrack();

	virtual void MarkTimeout();

	virtual bool TaskTimedOutAtStep(size_t step, Task tid);

	// Can be called only after construction or after a call to 
	//  SyncVarAccess (that returned false) and LocalBacktrack
	// virtual so the best-first search can override it.  Also made it return
	// an int rather than a bool because the best-first search may find
	// non-determinism during this method (Katie)
	virtual int NextTaskToSchedule(Task& next);

	IQueryEnabled* GetQueryEnabled() const;
	//void SetQueryEnabled(IQueryEnabled* qen);

	// Backtrack to step:
	//    Keep transitions 0,1,...step-1 in the stack
	//    This reaches the step'th state
	//    Now, schedule next as the step'th transition
	// Can fail if the backtrack step is smaller than the initState length
	bool Backtrack(size_t step, Task next);

	// Next bactrack point, in case of a crash/bug
	void SetRecoveryPoint(size_t step, Task next){
		recoveryValid = true;
		recoveryStep = step;
		recoveryTask = next;
	}

	void ClearRecoveryPoint(){
		recoveryValid = false;
	}

	bool Recover();

	void BacktrackToInitStack();

	// Make this virtual so the BestFirstExecution can override it (Katie)
	virtual void PruneExecution(size_t index);

	// The index from which 'new' transitions were explored
	// can be very useful from non-dfs strategies to know where the new state space starts from
	size_t GetRecordIndex() const {return recordIndex;}

	// virtual so the best-first search can override it (Katie)
	virtual ChessExecution* Clone();

	// Accessing ChessExecution as a sequence of Transitions
	size_t NumTransitions() const {return topIndex;}
	ChessTransition Transition(size_t n) const{
		assert(0 <= n && n < NumTransitions());
		return stack[n];
	}

	const ChessTransition& ExpectedTransition(size_t n) const{
		assert(0 <= n && n < stack.size());
		return stack[n];
	}

	size_t NumEvents() const {return events.size();}
	const ExecEvent& GetEvent(size_t n){
		assert(0 <= n && n < NumEvents());
		return events[n];
	}

	std::ostream& operator<<(std::ostream& o);
	
	// returns false if no lookahead is present
	bool GetLookaheadAtStep(size_t step, Task tid, ChessTransition& result);

	bool PrevTransitionOfTask(size_t step, Task tid, size_t& index);
	// For computing a legitimate timestamp at a step that neither reads nor writes (Katie)
	bool PrevHbTransitionOfTask(size_t step, Task tid, size_t& index) const;

	////internal used by EnabledSet only
	//const stdext::hash_map<Task, ChessTransition>& GetLookahead(){
	//	return lookahead;
	//}

	// virtual so the best-first search can override it (Katie)
	virtual void Serialize(std::ostream& ostr);
	virtual void Deserialize(std::istream& istr);

	virtual void AddNewTask(Task child){
		AddLookahead(ChessTransition(child, child, SVOP::TASK_BEGIN));
	}

	bool InReplayMode() const { return topIndex < stack.size();}
	bool InRecordMode() const { return topIndex == stack.size();}

	// Returns true if scheduling next at step requires a preemption for this execution (Katie)
	bool RequiresPreemption(size_t step, Task tid) const;

	// made this public for implementing quiescence - XXX
	void AddLookahead(const ChessTransition& trans){
		//assert(lookahead.find(tid) == lookahead.end());
		lookahead[trans.tid] = trans;
	}

	// Return the per thread step number corresponding to step (Katie)
	size_t GetPerThreadStepNum(size_t step) const {
		assert(step < perThreadStepNum.size());
		return perThreadStepNum[step];
	}

#ifdef DEBUG_SLEEPSETS
	std::vector<ChessTransition> Stack() { return stack; }
#endif

	// Moved WaitForQuiescence to Chess.cpp and added this so it can change
	// the task waiting for quiescence (Katie)
	void SetTaskWaitingForQuiescence(Task tid) { 
		taskWaitingForQuiescence = tid; 
	}
	// Made this method public so it can be accessed from Chess.cpp (Katie)
	bool ReachedQuiescence();

	static void EnterQuiescence();
	static void PulseQuiescence(){}

	static const int stackTraceSize = 8;
	struct StackTrace{
		__int64 trace[stackTraceSize];
	};

	std::set<Task> GetInitTasks() {
		return initTasks;
	}

// made some variables protected so BestFirstExecution can use them (Katie)
protected:
	// made this method both virtual and protected so the BestFirstExecution can override/access it (Katie)
	virtual int SyncVarAccess(ChessTransition& trans);
	// core state: this is the state of the execution that is serialized and deserialized
	// Rest of the state is a function of the core state
	std::set<Task> initTasks;
	ChessExecState* initExecState;
	bool IsTaskWaitingForQuiescence(Task& taskwfq) const;
	std::vector<ChessTransition> stack;
	size_t topIndex;
	std::vector<ExecEvent> events;
	size_t recordIndex; // the index at which record starts
	EnabledSet* enabled;
	TaskVector<ChessTransition> lookahead;
	bool IsFairBlocked(Task tid);
	std::vector<int> perThreadStepNum; // per thread index at each step (Katie)
	bool replaySuccessful; // false if non-determinism, or the backtracking point was not enabled (Katie)

	// moved from private to protected so that PCTExceution can access it (Sandeep)
	void QuiescentWakeup(Task tid);

private:
	size_t eventIndex;

	size_t initStackSize;

	// Core state ends

	ChessExecState* GetExecState();
	void SetExecState(const ChessExecState* state);

	Task taskWaitingForQuiescence;

	bool CheckInvariants(){
		assert(0<=topIndex && topIndex <= stack.size());
	}

	bool recoveryValid;
	size_t recoveryStep;
	Task recoveryTask;

	TaskVector<size_t> preemptionDisableCount;
};

inline std::ostream& operator<<(std::ostream& o, ChessExecution& e){
	return e.operator<<(o);
}