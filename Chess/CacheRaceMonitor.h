/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/


#pragma once
#include "ChessBase.h"
#include<vector>
#include<map>

#include "..\Chess\IChessMonitor.h"
#include "VClock.h"


// NOTE: the following compile switches are helpful when working with very large executions.

// if DO_SOBER_DETECTION is 0, we are not checking for borderline executions.
#define DO_SOBER_DETECTION 1

// if DO_DATARACE_DETECTION is 0, data accesses are automatically promoted to syncvar accesses.
#define DO_DATARACE_DETECTION 1

// the write buffer limits how many  writes PER ADDRESS we are buffering. Lowering this bound means
// we may miss more store buffer vulnerabilities.
#define WRITE_BUFFER_SIZE 4

class OnSoberRaceCb{
public:
	virtual ~OnSoberRaceCb(){}
	virtual void OnSoberRace(EventId inS, EventId inL, int inStorePcId, int inLoadPcId,SyncVar Var);
};

#if DO_DATARACE_DETECTION
class OnDataRaceCb{
public:
	virtual ~OnDataRaceCb(){}
	virtual void OnDataRace(EventId in1, EventId in2, void* loc){}
};
#endif

class CacheRaceMonitor : public IChessMonitor {

 public:

	 CacheRaceMonitor(bool singleexec, bool traceDetails, bool dataracesonly, int targetrace) { 
		 this->traceDetails = traceDetails; 
		 this->dataRacesOnly = (dataracesonly && DO_DATARACE_DETECTION);
		 this->multipleRaces = (targetrace == 0);
		 this->soberRaceCb = 0;
		 this->storeStamps = false;
		 this->targetrace = targetrace;
#if DO_DATARACE_DETECTION
		 this->dataRaceCb = 0;
#endif
	 }
	 virtual ~CacheRaceMonitor() { clear(); };
	 void clear(); // clear state

	 // public interface for recording and checking memory accesses

	 // Called right before a new execution of CHESS
	 // The argument is the partial execution that CHESS is about to execute
	 virtual void OnExecutionBegin(IChessExecution* exec){ clear(); }

	 // Called right after completing a new execution
	 virtual void OnExecutionEnd(IChessExecution* exec){}
	
	 virtual void OnShutdown();

	 // Called right before a Data Var access
	 virtual void OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId);

	 // Called right *after* a Sync Var access - Calling it after ensures that there is no danger of LocalBacktrack
	 virtual void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid);
	 virtual void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid);

	 // public interface for querying hbstamps
	 void start_hbstamp_recording();   // turns on hbstamp recording
	 Timestamp &get_hbstamp(EventId id);  // returns the stored hbstamp for the given id
	 size_t get_hbstamp_element(Timestamp& ts, Task tid); // returns the element in ts associated with tid (Katie)

	 // support for mixed-mode accesses
	 void MergeSyncAndDataVar(SyncVar var, void *loc);

	 virtual OnSoberRaceCb* SetSoberRaceCb(OnSoberRaceCb* cb){
		 OnSoberRaceCb* ret = soberRaceCb;
		 soberRaceCb = cb;
		 return ret;
	 }

#if DO_DATARACE_DETECTION
	 virtual OnDataRaceCb* SetOnDataRaceCb(OnDataRaceCb* cb){
		 OnDataRaceCb* ret = dataRaceCb;
		 dataRaceCb = cb;
		 return ret;
	 }
#endif

protected:
    friend class AtomicityMonitor; // grant access to local stuff

	struct Thread;

	struct Write {
		Thread *mThread;
        int mPcId;
		int mNr;
		Timestamp mHbStamp;
		TSOTimestamp mRhbStamp;
		Write(Thread *inThread, int inPcId, int inNr) : mThread(inThread), mPcId(inPcId), mNr(inNr) { }
		Write() : mThread(0){}
	};

	struct Location { 
		int mIndex;
	};

	struct DLocation; // forward declaration

	struct SLocation : Location {

#if DO_SOBER_DETECTION
		// record all writes to this location
		Write mWrites[WRITE_BUFFER_SIZE]; // a bounded circular buffer
		int mWritesIndex; // the tail of the buffer

		// hb timestamps
		Timestamp hb_s; // time stamp last store/interlocked
		Timestamp hb_l; // time stamp loads past last store (if any)
		
		// rhb timestamps
		TSOTimestampVector rhb_mc1;
		TSOTimestamp rhb_mc2;
#endif

#if DO_DATARACE_DETECTION
		// sb timestamp
		Timestamp sb_s;
        // corresponding DLocation (or null if this location is purely sync)
		DLocation *dloc;
		// last accesses per thread
		Timestamp lastreads;
		Timestamp lastwrites;
#endif

#if DO_SOBER_DETECTION
		SLocation(){
			mWritesIndex = WRITE_BUFFER_SIZE-1;
		}

		void push_write(const Write& w){
			mWritesIndex = (mWritesIndex + 1) % WRITE_BUFFER_SIZE;
			mWrites[mWritesIndex] = w;
		}

		Write& last_write(int index){
			assert(0 <= mWritesIndex && mWritesIndex < WRITE_BUFFER_SIZE);
			int pos = mWritesIndex-index;
			if(pos < 0)
				pos += WRITE_BUFFER_SIZE;
			assert(0 <= pos && pos < WRITE_BUFFER_SIZE);
			return mWrites[pos];
		}
		static int num_writes(){ return WRITE_BUFFER_SIZE;} // made static to make PreFix happy (madan)
#endif
	};
  
#if DO_DATARACE_DETECTION
	struct DLocation : Location {
		// last accesses per thread
		Timestamp lastreads;
		Timestamp lastwrites;
        // corresponding SLocation (or null if this location is purely data)
        SLocation *sloc;
	};
#else
	typedef SLocation DLocation; // data accesses are treated exactly like syncvar accesses
#endif

	struct Thread {
		Task mTask;
#if DO_SOBER_DETECTION
		// hb timestamps
		Timestamp hb_c;
		// rhb timestamps
		TSOTimestamp rhb_lc;
		TSOTimestamp rhb_sc;
#endif
#if DO_DATARACE_DETECTION
		// sb timestamp
		Timestamp sb_c;
#endif
	};

	// map between CHESS entitites and internal data structs

	std::map<Task, Thread*> mThreads;
	std::map<SyncVar, SLocation*> mSyncvarLocations;   
	std::map<void *, DLocation*> mNormalLocations;   

	Thread *get_thread(Task tid);
	SLocation *get_syncvar_location(SyncVar svar);
	DLocation *get_normal_location(void *loc);
	void *get_loc_from_dloc(DLocation *dloc);


	// record and check memory accesses
	void record_load(Thread *inThread, SyncVar var, SLocation *inLocation, int inPcId, int nr);
	void record_store(Thread *inThread, SyncVar var, SLocation *inLocation, int inPcId, int nr);
	void record_interlocked(Thread *inThread, SyncVar var, SLocation *inLocation, int nr);

#if DO_DATARACE_DETECTION
	// record and check data accesses
	void record_dload(Thread *inThread, void* addr, DLocation *inLocation, int nr);
	void record_dstore(Thread *inThread, void* addr, DLocation *inLocation, int nr);
#endif

	// switch between data race and store buffer detection
	bool dataRacesOnly;

	// options
	bool multipleRaces;

	// report violations
	void reportCacheRace(EventId inS, EventId inL, int inStorePcId, int inLoadPcId,SyncVar Var);
	void reportDataRace(EventId in1, EventId in2, void* loc);
	bool race_found;
	OnSoberRaceCb* soberRaceCb;
#if DO_DATARACE_DETECTION
	OnDataRaceCb* dataRaceCb;
#endif

	// set hbstamp
	static void set_hbstamp_attr(size_t inTid, size_t inNr, Timestamp &inHbStamp);
	void store_hbstamp(size_t inTid, size_t inNr, const Timestamp &inHbStamp);
    bool storeStamps;
	std::map<EventId, Timestamp> stamps;

	// tracing functions
	bool traceDetails;
	static void trace_vclock(std::ostream &inStream, Timestamp &inClock);
	static void trace_tsoclock(std::ostream &inStream, TSOTimestamp &inClock);
	void trace_instr(const std::string &inKind, Timestamp *inHbStamp, Timestamp *inSoStamp, TSOTimestamp *inRhbStamp,
		const Thread *inThread, const SLocation *inLocation, size_t inIndex);

	// duplicate filter
	std::set<std::string> duplicates;
	bool IsDuplicate(const std::string &location);

    // racing variables
	std::set<std::string> racing_variables;
    void FoundRacingVariable(void* loc);
	void PrintRacingVariables(std::ostream &stream);

	// utility
	static std::string getProgramLocation();

	// target race
	int targetrace;
	int racecounter;

	void Flush();
};
