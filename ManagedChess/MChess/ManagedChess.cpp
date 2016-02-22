/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

//managed interface for CHESS

#if (_MANAGED == 1) || (_M_CEE == 1)

#define MCHESS
#include "Chess.h"
#include "SyncManager.h"
#include "SyncVar.h"
#include "SyncVarOp.h"
#include "EventAttribute.h"
#include "ErrorInfo.h"

#pragma warning(push)
#include <CodeAnalysis\Warnings.h>
#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25032 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings
#include <windows.h>
#include <hash_map>
#include <string>
#include <vcclr.h>
#include <iostream>
#pragma warning(pop)

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;

#include <msclr/marshal.h>    
using namespace msclr::interop;

namespace Microsoft {
 namespace ManagedChess {

public ref class MErrorInfo {
public:
	MErrorInfo(){
	}
	MErrorInfo(String^ msg){
		Message = msg;
	}
	MErrorInfo(Exception^ ex){
		Message = ex->Message;
		ExType = ex->GetType()->FullName;
		StackTrace = ex->StackTrace;

		// This logic mirrors the XConcurrencyNames.CreateXError method
		// Recursively append any inner exceptions
		// For AggregateException instances, add an XError element per exception
		AggregateException^ aggEx = dynamic_cast<AggregateException^>(ex);
		if(aggEx)
		{
			System::Collections::Generic::List<MErrorInfo^> errors;
			for each(Exception^ innerEx in aggEx->InnerExceptions)
				errors.Add(gcnew MErrorInfo(innerEx));
			InnerErrors = errors.ToArray();
		}
		else if(ex->InnerException)
		{
			InnerErrors = gcnew array<MErrorInfo^>(1);
			InnerErrors[0] = gcnew MErrorInfo(ex->InnerException);
		}
	}

	String^ Message;
	String^ ExType;
	String^ StackTrace;

	array<MErrorInfo^>^ InnerErrors;

};

public ref class MChessOptions {
public:
	int  delayBound;
	int  PCT_num_of_runs;
	int  PCT_bug_depth;
	int  PCT_seed;
	int  var_bound;
	int  preemptionBound;
	bool PCT;
	bool PCT_VB;
	bool DeRandomizedPCT;
	bool loadSchedule;
	bool recoverSchedule;
	String^ xmlCommandline;
	String^ outputPrefix;
	String^ loadScheduleFile;
	String^ observationMode;
	String^ enumerateObservations;
	String^ checkObservations;
	String^ varLabels;
	bool breakOnAssert;
	bool breakOnDeadlock;
	bool breakOnTimeout;
	bool breakOnPreemptions;
	bool breakOnContextSwitch;
	bool breakAfterPreemptions;
	bool breakAfterContextSwitch;
	bool breakOnTaskResume;
	bool breakOnRace;

	bool dieOnNonidempotence;

	int maxStackSize;
	int maxExecTime;
	int maxChessTime;
	bool debugOutputFlag;
	int maxExecutions;
	bool useExecPrinter;
	bool handleNondeterminism;

	int fairnessParameter;	// parameterized fairness

	bool doIdfs;			// iterative depth-first search
	int depthBound;			// stop execution after this number of sync ops
	int idfsBound;

	bool doRandom;			// random search
	int randomSeed;
	
	bool doSleepSets;
	// best first search options
	bool doDpor;
	bool bestFirst;
	bool fairPor;
	bool bounded;
	int prioritizedVar;
	String^ boundedPriority;
	String^ bestFirstPriority;

	bool notime;
	bool printSchedOnError;
	bool nopopups;

	bool quiescence;

	bool sober;
	bool soberDataRacesOnly;
	int soberTargetrace;

	bool gui;
	bool logging;
	bool trace;

	bool showNlb;
	bool showHbexecs;
	bool showStackSplit;
	bool showProgress;
	int showProgressStart;

	bool loopAtEnd;
	int warmupRuns;
	int processorCount;
	bool tolerateDeadlock;

	// preemption conversion options
	bool recordPreemptMethods;
	bool preemptAccesses;
	String^ preemptionVars;


	// ER specific
	bool monitorVolatiles;
	bool gcTrack;
	bool monitorAccesses;
	bool monitorCctors;
	bool finesse;

	MChessOptions() {
		// default options
		delayBound = -1;
		PCT_num_of_runs = -1;
		PCT_bug_depth = -1;
		PCT_seed = -1;
		PCT = false;
		PCT_VB = false;
		DeRandomizedPCT = false;
		var_bound = -1;
		varLabels = "";
		preemptionBound = 2;
		xmlCommandline = "";
		observationMode = "";
		checkObservations = "";		
		enumerateObservations = "";
		outputPrefix = "";
		loadScheduleFile = "sched";
		preemptionVars = "";
		monitorVolatiles = true;
		gcTrack = true;

		monitorAccesses = false;
		preemptAccesses = false;
		recordPreemptMethods = false;
		finesse = false;
		idfsBound = 10;
		doSleepSets = true;
		// Best first search options (BFS)
		doDpor = false;
		bestFirst = false;
		fairPor = false;
		bounded = true;
		prioritizedVar = 0;
		boundedPriority = "pb";
		bestFirstPriority = "wdpor_pb";
		fairnessParameter = 1;
		maxStackSize = 20000;
		maxExecTime = 10;
		dieOnNonidempotence = true;
		handleNondeterminism = true;
		processorCount = 1;
		showProgressStart = 10000;
		tolerateDeadlock = false;
	}
};

	// this interface is used by CHESS to control the CLR scheduler
	// this interface represents what we need to implement on the managed side

	public ref class MSyncManager
	{
	public:
		// associated initTask with the main CLR thread
		virtual void Init(int ctid) {}
		virtual void SetInitState() {}
		// enable the CLR thread associated with the ChessTask
		virtual void ScheduleTask(int ctid, bool atTermination) {}
		virtual void Reset() {}
		virtual void Shutdown(int code) {}
		virtual void Shutdown(System::Exception ^e) {}
		virtual void TaskEnd(int ctid) {}
		virtual void EnterChess() {}
		virtual void LeaveChess() {}
		virtual void DebugBreak() {}
		virtual bool IsDebuggerPresent() { return false; }
		virtual bool GetCurrentStackTrace(int n, List<String^>^ procs, List<String^>^ files, List<int>^ lineNos){
			return false;
		}
		virtual String^ GetFullyQualifiedTopProcedure(){
			return gcnew System::String("");
		}
		virtual String^ GetTaskName(int ctid) {
			return System::String::Concat("ChessTask", ctid.ToString());
		}
		virtual String^ GetDataVarLabel(int datavar) {
			return System::String::Concat("Datavar", datavar.ToString());
		}
	};

	static void MarshalString ( String ^ s, int& len, char* os, int maxlen ) {
	   using namespace System::Runtime::InteropServices;
	   const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();

	   len = strlen(chars)+1;
	   strncpy_s(os, len, chars, maxlen);

	   Marshal::FreeHGlobal(IntPtr((void*)chars));
	}

// this code is a shim in order for CHESS to use a SyncVarManager written in managed code
// TODO: we should assert that we are in the correct app domain before each reverse pinvoke

	class ShimSyncManager : SyncManager {
	public:
		ShimSyncManager(MSyncManager^ manager) { 
			m_handle = static_cast<IntPtr>(GCHandle::Alloc(manager));
			timerHandle = NULL;
		}
		~ShimSyncManager() {
			static_cast<GCHandle>(m_handle).Free();
		}
		virtual void Init(Task initTask){
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->Init(initTask);
		}
		virtual void SetInitState(){
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->SetInitState();
		}
		virtual void ScheduleTask(Task next, bool atTermination){
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->ScheduleTask(next,atTermination);
		}
		virtual void Reset() {
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->Reset();
		}
		virtual void ShutDown() {
		}
		virtual void TaskEnd(Task curr){
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->TaskEnd(curr);
		}
		virtual void EnterChess() {
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->EnterChess();
		}
		virtual void LeaveChess() {
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->LeaveChess();
		}
		virtual void DebugBreak() {
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->DebugBreak();
		}

		virtual bool IsDebuggerPresent(){
			// GOTCHA: since IsDebuggerPresent is called from a win32 timer callback by (unmanaged CHESS), 
			// (see void ChessImpl::ProgressTrackerFn) the AppDomain isn't correct
			// so don't do a reverse pinvoke here, instead use the Win32 API directly
			if (::IsDebuggerPresent()) return true; else return false;
		}

		virtual void Exit(int code) {
			canceled = true;
			// cancel timer before exiting
			if (timerHandle != NULL)
				#pragma warning( suppress: 6031 )  // Suppress warning about ignored return code... we're out of here anyway
				DeleteTimerQueueTimer(NULL, timerHandle, NULL);
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			msm->Shutdown(code);
		}

		static void CALLBACK ChessWaitOrTimerCallback(PVOID lpParameter, BOOLEAN TimerOrWaitFired)
		{
			void (*f)() = (void (*)())lpParameter;
			if (!canceled && Chess::IsInitialized())
				f();
		}

		virtual bool QueuePeriodicTimer(int period, void (*timerFn)())
		{
			if(timerHandle != NULL)
				return false; // can only have one timer right now
			
			// initialize flag
			canceled = false;

			BOOL ret = CreateTimerQueueTimer(&timerHandle, NULL, ChessWaitOrTimerCallback, timerFn, 
				period, period, WT_EXECUTELONGFUNCTION);
			if(!ret){
				return false;
			}
			return true;
		}

		virtual bool GetCurrentStackTrace(int n, int m, char* procedure[], char* filename[], int lineno[]){
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			List<String^ >^ procs = gcnew List<String^ >();
			List<String^ >^ files = gcnew List<String^ >();
			List<int>^ lineNos = gcnew List<int>();
			if(msm->GetCurrentStackTrace(n, procs, files, lineNos)){
				if(n > procs->Count)
					n = procs->Count;
				for(int i=0; i<n; i++){
					pin_ptr<const wchar_t> p;
					size_t r;
					p = PtrToStringChars(procs[i]);
					wcstombs_s(&r, procedure[i], m, p, _TRUNCATE);
					p = PtrToStringChars(files[i]);
					wcstombs_s(&r, filename[i], m, p, _TRUNCATE);
					lineno[i] = lineNos[i];
				}
				return true;
			}
			return false;
		}

		virtual bool GetFullyQualifiedTopProcedure(int &len, char* name, int maxlen){
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			String^ ret = msm->GetFullyQualifiedTopProcedure();
			MarshalString(ret,len,name,maxlen);
			return true;
		}

		virtual void GetTaskName(Task tid, char* c, int len) {
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			const String ^ res = msm->GetTaskName(tid);
			pin_ptr<const wchar_t> p = PtrToStringChars(res);
			size_t r;
			wcstombs_s(&r, c, len, p, _TRUNCATE);
		}

		virtual void GetDataVarLabel(void *loc, char* c, int len) {
			MSyncManager^ msm = safe_cast< MSyncManager ^ >(static_cast<GCHandle>(m_handle).Target);
			const String ^ res = msm->GetDataVarLabel((int) loc);
			pin_ptr<const wchar_t> p = PtrToStringChars(res);
			size_t r;
			wcstombs_s(&r, c, len, p, _TRUNCATE);
		}

	private:
		private:   IntPtr m_handle;
		private:   HANDLE timerHandle;
		private:   static volatile bool canceled; 
		};

		volatile bool ShimSyncManager::canceled;

public ref class MEventId {
public:
	int tid;
	int nr;
	MEventId(int t, int n) : tid(t), nr(n) { }
	MEventId(EventId id) : tid(id.tid), nr(id.nr) { }
};

public ref class MChessInt {
public:
	int i;
};




public ref class MChessChess {
private:
	static ErrorInfo* ConvertToUnmanaged(marshal_context% mc, MErrorInfo^ errorInfo) {
		ErrorInfo* c_errorInfo = NULL;

		if(errorInfo)
		{
			c_errorInfo = new ErrorInfo();

			if(errorInfo->Message && errorInfo->Message->Length != 0)
				c_errorInfo->Message = (char *) mc.marshal_as<const char *>(errorInfo->Message);
			if(errorInfo->ExType && errorInfo->ExType->Length != 0)
				c_errorInfo->ExType = (char *) mc.marshal_as<const char *>(errorInfo->ExType);
			if(errorInfo->StackTrace && errorInfo->StackTrace->Length != 0)
				c_errorInfo->StackTrace = (char *) mc.marshal_as<const char *>(errorInfo->StackTrace);
			if(errorInfo->InnerErrors && errorInfo->InnerErrors->Length != 0)
			{
				int cnt = errorInfo->InnerErrors->Length;
				c_errorInfo->InnerErrorsCount = cnt;
				c_errorInfo->InnerErrors = new ErrorInfo*[cnt];
				for(int i = 0; i < cnt; i++)
					c_errorInfo->InnerErrors[i] = ConvertToUnmanaged(mc, errorInfo->InnerErrors[i]);
			}
		}

		return c_errorInfo;
	}


public:
	// for Chess Main Loop
	static void Init(MSyncManager^ sm, const MChessOptions^ mco) {
		ChessOptions o;
		static_mc = gcnew marshal_context();
		String ^managedstring;
		
		o.recover_schedule = mco->recoverSchedule;
		
		o.delay_bound = mco->delayBound;
		o.preemption_bound = mco->preemptionBound;
		o.PCT = mco->PCT;
		o.DeRandomizedPCT = mco->DeRandomizedPCT;
		o.num_of_runs = mco->PCT_num_of_runs;
		o.bug_depth = mco->PCT_bug_depth;
		o.pct_seed = mco->PCT_seed;
		o.var_bound = mco->var_bound;
		o.load_schedule = mco->loadSchedule;
		managedstring=mco->outputPrefix;
		o.output_prefix = (char *) static_mc->marshal_as<const char *>(managedstring);
		managedstring=String::Concat(mco->outputPrefix, "sched");
		o.schedule_file = o.recover_schedule_file = (char *) static_mc->marshal_as<const char *>(managedstring);
		managedstring=mco->loadScheduleFile;
		o.load_schedule_file = (char *) static_mc->marshal_as<const char *>(managedstring);
		managedstring=mco->xmlCommandline;
		o.xml_commandline = (char *) static_mc->marshal_as<const char *>(managedstring);
		managedstring=mco->observationMode;
		o.observation_mode = (char *) static_mc->marshal_as<const char *>(managedstring);
		managedstring=mco->enumerateObservations;
		o.enumerate_observations = (char *) static_mc->marshal_as<const char *>(managedstring);
		managedstring=mco->checkObservations;
		o.check_observations = (char *) static_mc->marshal_as<const char *>(managedstring);

		o.break_on_assert = mco->breakOnAssert;
		o.break_on_deadlock = mco->breakOnDeadlock;
		o.break_on_timeout = mco->breakOnTimeout;
		o.break_on_preemptions = mco->breakOnPreemptions;
		o.break_on_context_switch = mco->breakOnContextSwitch;
		o.break_after_preemptions = mco->breakAfterPreemptions;
		o.break_after_context_switch = mco->breakAfterContextSwitch;
		o.break_on_task_resume = mco->breakOnTaskResume;
		o.break_on_race = mco->breakOnRace;

		o.max_stack_size = mco->maxStackSize;
		o.max_exec_time = mco->maxExecTime;
		o.max_chess_time = mco->maxChessTime;
		o.max_executions = mco->maxExecutions;
		o.use_exec_printer = mco->useExecPrinter;
		o.handle_nondeterminism = mco->handleNondeterminism;

		o.fairness_parameter = mco->fairnessParameter;

		o.do_idfs = mco->doIdfs;
		o.depth_bound = mco->depthBound;
		o.idfs_bound = mco->idfsBound;

		o.do_random = mco->doRandom;
		o.random_seed = mco->randomSeed;

		o.do_sleep_sets = mco->doSleepSets;
		// Best first search options (BFS)
		o.do_dpor = mco->doDpor;
		o.best_first = mco->bestFirst;
		o.fair_por = mco->fairPor;
		o.bounded = mco->bounded;
		o.prioritized_var = mco->prioritizedVar;
		o.best_first_priority = (char*)(Marshal::StringToHGlobalAnsi(mco->bestFirstPriority)).ToPointer();
		o.bounded_priority = (char*)(Marshal::StringToHGlobalAnsi(mco->boundedPriority)).ToPointer();

		o.notime = mco->notime;
		o.print_sched_on_error = mco->printSchedOnError;
		o.nopopups = mco->nopopups;

		o.quiescence = mco->quiescence;
		o.sober = mco->sober;
		o.sober_dataracesonly = mco->soberDataRacesOnly;
		o.sober_targetrace = mco->soberTargetrace;
		o.gui = mco->gui;
		o.trace = mco->trace;
		o.gui = mco->gui;
		o.logging = mco->logging;
		o.die_on_nonidempotence = mco->dieOnNonidempotence;
		o.tolerate_deadlock = mco->tolerateDeadlock;

		o.debug_output_flag = mco->debugOutputFlag;
		o.show_nlb = mco->showNlb;
		o.show_hbexecs = mco->showHbexecs;
		o.show_stacksplit = mco->showStackSplit;
		o.show_progress = mco->showProgress;
		o.show_progress_start = mco->showProgressStart;

		o.record_preempt_methods = mco->recordPreemptMethods;

		Chess::SetOptions(o);
		Chess::Init((SyncManager *)new ShimSyncManager(sm));
	}
private:
	// static marshal context; use for conversions (String -> char*) with unlimited lifetime
	static marshal_context ^static_mc;

public:
	static void Done(bool enter) { Chess::Done(enter); }
	static int  GetCurrentTid() { return Chess::GetCurrentTid(); }
	static int  Choose(int numChoices) { return Chess::Choose(numChoices); }
	static void PreemptionDisable() { return Chess::PreemptionDisable(); }
	static void PreemptionEnable() { return Chess::PreemptionEnable(); }
	// prioritize/unprioritize preemptions for best-first search (BFS)
	static void PrioritizePreemptions() { return Chess::PrioritizePreemptions(); }
	static void UnprioritizePreemptions() { return Chess::UnprioritizePreemptions(); }
	static bool StartTest() { return Chess::StartTest(); }
	static bool EndTest() { return Chess::EndTest(); }
	static bool EnterChess() { return Chess::EnterChess();}
	static void LeaveChess() {return Chess::LeaveChess();}
	static bool LocalBacktrack() { return Chess::LocalBacktrack(); }
	static bool TaskResume(int tid) { return Chess::ResumeTask((Task)tid); }
	static bool TaskSuspend(int tid) { return Chess::SuspendTask((Task)tid); }
	static bool TaskBegin() { return Chess::TaskBegin(); }
	static bool TaskEnd() { return Chess::TaskEnd(); }
	static bool TaskYield() { return Chess::TaskYield(); }
	static bool TaskFork(MChessInt^ tid) {
		Task i;
		if (Chess::TaskFork(i)) {
			tid->i = i;
			return true;
		} else
			return false;
	}
	static bool CommitSyncVarAccess() { return Chess::CommitSyncVarAccess(); }
	static void MarkTimeout() { return Chess::MarkTimeout(); }
	static void WakeNextDeadlockedThread(bool isContinuation, bool isDeadlockedThread) { Chess::WakeNextDeadlockedThread(isContinuation, isDeadlockedThread); }

#pragma warning (suppress: 25057) 
	static bool AggregateSyncVarAccess(array<int> ^vars, MSyncVarOp op) {
		if (vars->Length > 0) {
			SyncVar* mvars = new SyncVar[vars->Length];
			for(int i=0; i<vars->Length; i++) {
				mvars[i] = SyncVar(vars[i]);
			}
			bool ret = Chess::AggregateSyncVarAccess(mvars,vars->Length,static_cast<SyncVarOp>(op));
			// IMPORTANT: [] before mvars, otherwise bad assert
			delete [] mvars;
			return ret;
		}
		return true;
	}
	static bool SyncVarAccess(int var, MSyncVarOp op) { 
		return Chess::SyncVarAccess(SyncVar(var),static_cast<SyncVarOp>(op)); 
	}
	static SyncVar GetNextSyncVar(){
		return Chess::GetNextSyncVar();
	}
	static MEventId ^DataVarAccess(int varnum, bool isWrite) {
		return gcnew MEventId(Chess::DataVarAccess((void *) varnum,1,isWrite,0)); 
	}

	static void MergeSyncAndDataVar(int svar, int dvar)
	{
		Chess::MergeSyncAndDataVar(SyncVar(svar), (void *) dvar);
	}

	static void SetNextEventAttribute(EventAttribute attr, String ^value) {
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::SetNextEventAttribute(attr, mc.marshal_as<const char*>(value));
	}

	static void TraceEvent(String ^info) { 
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::TraceEvent(mc.marshal_as<const char*>(info));
	}


	delegate bool OnErrorCallbackDelegate(int exitCode, System::IntPtr details);
	static void QueueOnErrorCallback(OnErrorCallbackDelegate^ del){
		using namespace System::Runtime::InteropServices;
		CHESS_ON_ERROR_CALLBACK cb = (CHESS_ON_ERROR_CALLBACK)Marshal::GetFunctionPointerForDelegate(del).ToPointer();
		Chess::QueueOnErrorCallback(cb);
	}

	static int GetExitCode() {
		return Chess::GetExitCode();
	}

	static void ReportWarning(String^ description, String ^action, bool includeschedule) { 
		marshal_context mc;
		char* c_description = (char *) mc.marshal_as<const char *>(description);
		char* c_action = (char *) mc.marshal_as<const char *>(action);
		Chess::ReportWarning(c_description, c_action, includeschedule);
	}

	static void ReportError(String^ description, String ^action, MErrorInfo^ errorInfo) { 
		marshal_context mc;
		char* c_description = (char*) mc.marshal_as<const char*>(description);
		char* c_action = (char*) mc.marshal_as<const char*>(action);
		ErrorInfo* c_errorInfo = ConvertToUnmanaged(mc, errorInfo);
		Chess::ReportError(c_description, c_action, c_errorInfo);
		if(c_errorInfo != NULL)
			delete c_errorInfo;
	}
	static void ReportFinalStatistics(int exitCode) { 
		Chess::ReportFinalStatistics(exitCode);
	}
	static void CloseResults() { 
		Chess::CloseResults();
	}


	static void ObserveOperationCall(long obj, String ^opname)
	{
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::ObserveOperationCall((void *)obj, mc.marshal_as<const char*>(opname));
	}
	static void ObserveOperationReturn() { Chess::ObserveOperationReturn(); }
	static void ObserveCallback(long obj, String ^opname)
	{
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::ObserveCallback((void *)obj, mc.marshal_as<const char*>(opname));
	}
	static void ObserveCallbackReturn() { Chess::ObserveCallbackReturn(); }
	static void ObserveIntValue(String ^label, long long intvalue)
	{
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::ObserveIntValue(mc.marshal_as<const char*>(label), intvalue);
	}
	static void ObservePointerValue(String ^label, long ptrvalue)
	{
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::ObservePointerValue(mc.marshal_as<const char*>(label), (void *)ptrvalue);
	}
	static void ObserveStringValue(String ^label, String ^strvalue)
	{
		marshal_context mc; // goes out of scope and releases char* at end of call
		Chess::ObserveStringValue(mc.marshal_as<const char*>(label), mc.marshal_as<const char*>(strvalue));
	}
	static bool IsBreakingDeadlock()
	{
		return Chess::IsBreakingDeadlock();
	}

	static List<System::Byte>^ GetChessSchedule() {
		char *buf;
		const int buflen=1000;
		buf = new char[buflen];
		int ret = Chess::GetChessSchedule(buf, buflen);
		if (ret >= buflen) {
			delete[] buf;
			buf = new char[ret];
			Chess::GetChessSchedule(buf,ret);
		}
		List<System::Byte>^ result = gcnew List<System::Byte>();
		result->Capacity = ret;
		for(int i=0; i<ret; i++) {
			result->Add(buf[i]);
		}
		delete[] buf;
		return result;
	}

	static bool SetChessSchedule(List<System::Byte>^ buf) {
		char *my_sched = new char[buf->Count];
		int i;
		for(i=0; i<buf->Count; i++) {
			my_sched[i] = (unsigned char)buf[i];
		}
		return Chess::SetChessSchedule(my_sched,i);
	}
};

public ref class MChessStats {
private:
	IChessStats* stats;
public:	
	 MChessStats() {
		stats = Chess::GetStats();
	 } 
	 int GetNumExecutions(){
		 return stats->GetNumExecutions();
	 }
	 int GetMaxThreads(){
		 return stats->GetMaxThreads();
	 }
	 int GetMaxSteps() {
		 return stats->GetMaxSteps();
	 }
	 int GetTotalSteps(){
		 return stats->GetTotalSteps();
	 }
	 int GetMaxHBExecutions(){
		 return stats->GetMaxHBExecutions();		 
	 }
	 int GetMaxContextSwitches(){
		 return stats->GetMaxContextSwitches();
	 }
	 int GetTotalContextSwitches(){
		 return stats->GetTotalContextSwitches();
	 }
	 int GetElapsedTimeMS(){
		 return stats->GetElapsedTimeMS();
	 }
};

}
}

#endif
