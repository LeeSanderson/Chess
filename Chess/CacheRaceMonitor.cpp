/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessStl.h"
#include "CacheRaceMonitor.h"
#include "Chess.h"
#include "ChessImpl.h"
#include "ResultsPrinter.h"

using namespace std;

// compile-time options

#define QUIT_ON_RACE 0 
#define TRACE_ACCESSES 0
#define TRACE_CALLS 0
#define TRACE_LOCATION 0
#define SET_HBSTAMP_ATTRIBUTE 1


//inline void error(const std::string &inInfo) {
//  *GetChessOutputStream() << inInfo << endl;
//  *GetChessOutputStream().flush();
 // if (QUIT_ON_RACE) {
//	  //Chess::SetErrorMessage(inInfo);
//	//  Chess::GetSyncManager()->Exit(CHESS_EXIT_RACE);
 // }
//}

bool CacheRaceMonitor::IsDuplicate(const std::string &location)
{
	if (duplicates.find(location) != duplicates.end())
		return true;
	else {
		duplicates.insert(location);
		return false;
	}
}

void CacheRaceMonitor::FoundRacingVariable(void* loc)
{
    static char buf[256];
    buf[0] = '\0';
    Chess::GetSyncManager()->GetDataVarLabel(loc, buf, 255);
    racing_variables.insert(buf);
}

void CacheRaceMonitor::PrintRacingVariables(ostream &stream)
{
	for(set<string>::iterator it = racing_variables.begin(); it != racing_variables.end(); it++)
	{	
		if (it != racing_variables.begin())
			stream << ",";
		stream << *it;
	}
}

void CacheRaceMonitor::OnShutdown()
{
	if (racing_variables.size() > 0 && ! (Chess::GetOptions().load_schedule && Chess::GetOptions().max_executions == 1))
	{
		ostringstream xmlstr;
		xmlstr << "    <preemptionvars>";
		PrintRacingVariables(xmlstr);
		xmlstr << "</preemptionvars>" << endl;
		ChessImpl::GetResultsPrinter()->CaptureResult('W', "Data races found. For full coverage, fix races or preempt on all accesses.", xmlstr.str(), false, NULL);
	}
}

/** capture PC location from stack frame */

#define MAX_FRAME_DEPTH 3
#define MAX_STRLEN 1024
#define MAX_BUFSIZE MAX_FRAME_DEPTH*MAX_STRLEN

std::string CacheRaceMonitor::getProgramLocation() {

	static char filenamebufs[MAX_FRAME_DEPTH][MAX_STRLEN];
	static char *filename[MAX_FRAME_DEPTH];
	static int lineno[MAX_FRAME_DEPTH];
	static char procbufs[MAX_FRAME_DEPTH][MAX_STRLEN];
	static char *proc[MAX_FRAME_DEPTH];

	for(int i=0; i<MAX_FRAME_DEPTH; i++){
		filename[i] = filenamebufs[i];
		proc[i] = procbufs[i];
		filenamebufs[i][0] = procbufs[i][0] = 0;
	}

	ostringstream sstr;
	if(Chess::GetSyncManager()->GetCurrentStackTrace(MAX_FRAME_DEPTH, MAX_STRLEN, proc, filename, lineno))
	{
		int pos = 0;
		for(int i=0; i<MAX_FRAME_DEPTH; i++){
			if(proc[i][0] == 0)
				continue;
			if(filename[i][0] == 0)
				continue;
			int prefixpos = 0;
			for (int j = 0; j < MAX_STRLEN; j++)
               if (filename[i][j] == 0)
				   break;
			   else if (filename[i][j] == '\\' || filename[i][j] == ':' )
				   prefixpos = j + 1;
			sstr << proc[i] << ':' << (filename[i] + prefixpos) << '(' << lineno[i] << ')';
			break;
		} 
	}
	return sstr.str();
}


void CacheRaceMonitor::reportCacheRace(EventId inS, EventId inL, int inStorePcId, int inLoadPcId, SyncVar var) {

	// find only one race per execution
	if (!multipleRaces && race_found)
		return;

	// notify listener
	if(soberRaceCb){
		soberRaceCb->OnSoberRace(inS, inL, inStorePcId, inLoadPcId, var);
		return;
	}

	racecounter++;
	string location = getProgramLocation();

	if (targetrace != 0) 
	{
		if (racecounter != targetrace)
			return;
	} else {
		if (IsDuplicate(location))
			return;
	}

	// o.k., now we go ahead and report this race.
	ostringstream sstr;
	sstr << "Found store buffer vulnerability at " << location;
	//if (inStorePcId || inLoadPcId)
	//	sstr << "  (S" << inStorePcId << "->L" << inLoadPcId << ")"; 
	ostringstream xmlstr;
	if (Chess::GetOptions().trace && ! race_found)
		xmlstr << "<action name=\"View\" race=\"true\"/>" << endl;
    xmlstr << "<action name=\"Repro\" race=\"" << racecounter << "\" />" << endl;
	ChessImpl::GetResultsPrinter()->CaptureResult('R', sstr.str(), xmlstr.str(), true, NULL);

	// mark first race in trace file
    if (!race_found)
    {
	  Chess::SetEventAttribute(inS, DISPLAY_BOXED, "Crimson");
	  Chess::SetEventAttribute(inL, DISPLAY_BOXED, "Crimson");
    }

	race_found = true;
	if(Chess::GetOptions().break_on_race){
		Chess::GetSyncManager()->DebugBreak();
	}
}

void CacheRaceMonitor::reportDataRace(EventId in1, EventId in2, void* loc) {

	// mark racing variable
	FoundRacingVariable(loc);

	// find only one race per execution
	if (!multipleRaces && race_found)
		return;

	// notify listener
	if(dataRaceCb){
		dataRaceCb->OnDataRace(in1, in2, loc);
		return;
	}

	racecounter++;
	string location = getProgramLocation();

	if (targetrace != 0) 
	{
		if (racecounter != targetrace)
			return;
	} else {
		if (IsDuplicate(location))
			return;
	}

	// o.k., now we go ahead and report this race.
	ostringstream sstr;
	//	sstr << "Found data race at location " << loc << " ";
	sstr << "Found data race at " << location;
	//if (loc)
	//	sstr << " (" << loc << ")";
	ostringstream actions;
	if (Chess::GetOptions().trace && !race_found)
		actions << "<action name=\"View\" race=\"true\"/>" << endl;
    actions << "<action name=\"Repro\" race=\"" << racecounter << "\" />" << endl;
	ChessImpl::GetResultsPrinter()->CaptureResult('R', sstr.str(), actions.str(), true, NULL);

	// mark race in trace file
	if (!race_found)
	{
	   Chess::SetEventAttribute(in1, DISPLAY_BOXED, "Orange");
	   Chess::SetEventAttribute(in2, DISPLAY_BOXED, "Orange");
	}

	race_found = true;
	if(Chess::GetOptions().break_on_race){
		Chess::GetSyncManager()->DebugBreak();
	}
}


void CacheRaceMonitor::OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId) {
	if(size > 4){
		while(size > 0){
			OnDataVarAccess(id, loc, size > 4 ? 4 : size, isWrite, pcId);
			loc = ((char*)loc)+4;
			size -= 4;
		}
		return;
	}
	loc = (void*)( ((int)loc) & (~0x3));

	if (TRACE_CALLS)
		*GetChessOutputStream() << "Data " << id.tid << "," << loc << "," << isWrite << "," << pcId << std::endl;
	Thread *thread = get_thread(id.tid);

	DLocation *location = get_normal_location(loc);
#if DO_DATARACE_DETECTION
	if (isWrite)
		record_dstore(thread, loc, location, id.nr);
	else
		record_dload(thread, loc, location, id.nr);
#else
	if (isWrite)
		record_store(thread, location, pcId, id.nr);
	else
		record_load(thread, location, pcId, id.nr);
#endif
	Flush();
}

void CacheRaceMonitor::OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid) {
	if (TRACE_CALLS)
		*GetChessOutputStream() << "Sync " << tid << "," << var << "," << SVOP::ToString(op)
		<< " " << (SVOP::IsWrite(op)? "W" : "") << (SVOP::IsRead(op) ? "R" : "" )
		<< std::endl;
	Thread *thread = get_thread(tid);
	SLocation *location = get_syncvar_location(var);
	if (SVOP::IsWrite(op) && SVOP::IsRead(op))
		record_interlocked(thread, var, location, id.nr);
	else if (SVOP::IsWrite(op))
		record_store(thread, var, location, 0, id.nr);
	else if (SVOP::IsRead(op))
		record_load(thread, var, location, 0, id.nr);
	else {
		// no-op on syncvar accesses that are neither write nor read
	}
  Flush();
}

void CacheRaceMonitor::OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid) {
	for (int i = 0; i < n; ++i)
		OnSyncVarAccess(id, tid, var[i], op, sid);
}

void CacheRaceMonitor::MergeSyncAndDataVar(SyncVar var, void *loc)
{
	SLocation *sloc = get_syncvar_location(var);
	DLocation *dloc = get_normal_location((void*)( ((int)loc) & (~0x3)));
	sloc->dloc = dloc;
	dloc->sloc = sloc;
}


CacheRaceMonitor::Thread *CacheRaceMonitor::get_thread(Task tid) {
  map<Task, Thread*>::iterator it = mThreads.find(tid); 
  if (it != mThreads.end())
    return it->second;
  else {
    Thread *t = new Thread();
    t->mTask = tid - 1;  // we use zero based, not 1 based
    mThreads[tid] = t;
    return t;
  }
}

CacheRaceMonitor::SLocation *CacheRaceMonitor::get_syncvar_location(SyncVar svar) {
	map<SyncVar, SLocation*>::iterator it = mSyncvarLocations.find(svar); 
	if (it != mSyncvarLocations.end())
		return it->second;
	else {
		SLocation *l = new SLocation();
		l->dloc = 0;
		l->mIndex = (int) (mSyncvarLocations.size() + mNormalLocations.size());
		mSyncvarLocations[svar] = l;
		return l;
	}
}

CacheRaceMonitor::DLocation *CacheRaceMonitor::get_normal_location(void *loc) {
  map<void *, DLocation*>::iterator it = mNormalLocations.find(loc); 
  if (it != mNormalLocations.end())
    return it->second;
  else {
    DLocation *l = new DLocation();
	l->sloc = 0;
    l->mIndex = (int) (mSyncvarLocations.size() + mNormalLocations.size());
    mNormalLocations[loc] = l;
    return l;
  }
}

void *CacheRaceMonitor::get_loc_from_dloc(CacheRaceMonitor::DLocation *dloc) {
	// slow (linear) search is appropriate as it is rare (only for reporting data races)
	for (map<void *, DLocation*>::iterator it = mNormalLocations.begin(); it != mNormalLocations.end(); it++)
		if (it->second == dloc)
			return it->first;
	return 0;
}

void CacheRaceMonitor::clear() {
  // remove the dynamically allocated structs
  for (map<Task, Thread*>::iterator it = mThreads.begin(); it != mThreads.end(); ++it)
    delete (it->second);
  mThreads.clear();
  for (map<SyncVar, SLocation*>::iterator it = mSyncvarLocations.begin(); it != mSyncvarLocations.end(); ++it)
    delete (it->second);
  mSyncvarLocations.clear();
  for (map<void*, DLocation*>::iterator it = mNormalLocations.begin(); it != mNormalLocations.end(); ++it)
    delete (it->second);
  mNormalLocations.clear();
  // clear flags
  race_found = false;
  racecounter = 0;
}



// record a load
//
void CacheRaceMonitor::record_load(Thread *inThread, SyncVar var, SLocation *inLocation, int inPcId, int nr) {
  
	Timestamp *trace_ts = 0;
	const Timestamp *trace_sts = 0;
	const TSOTimestamp *trace_rts = 0;

#if DO_SOBER_DETECTION
	if (!dataRacesOnly) {

//		for (int pos=0; pos<inLocation->num_writes(); pos++) {
		for (int pos=0; pos<SLocation::num_writes(); pos++) {
			const Write *w = &(inLocation->last_write(pos));
			if(w->mThread == 0)
				break;

			// if we reached a local store, we're done
			if (w->mThread == inThread)
				break;
			if (w->mRhbStamp.lessthanorequal(inThread->rhb_lc))
				break;
			if (w->mHbStamp.lessthanorequal(inThread->hb_c)) {
				reportCacheRace(EventId(w->mThread->mTask+1, w->mNr), EventId(inThread->mTask+1, nr), w->mPcId, inPcId, var);
				break;
			}
		}

		// (load) update hb vector clock and time stamps
		Timestamp &ts(inThread->hb_c);
		ts.merge(inLocation->hb_s);
		ts.tick(inThread->mTask, nr);
		inLocation->hb_l.merge(ts);
		trace_ts = &ts;
  
		// (load) update rhb vector clocks and time stamps
		TSOTimestamp &rts(inThread->rhb_lc);
		rts.merge(inLocation->rhb_mc1.get(inThread->mTask));
		rts.tick_load(inThread->mTask, nr);
		inLocation->rhb_mc2.merge(rts);
		trace_rts = &rts;
	}
#endif

#if DO_DATARACE_DETECTION
 
	// (load) update sb vector clock and time stamp
	Timestamp &sts(inThread->sb_c);
	sts.merge(inLocation->sb_s);
	sts.tick(inThread->mTask, nr);
	trace_sts = &sts;

	DLocation *dloc = inLocation->dloc;
	if (dloc != 0)
	{
		// (load) check for conflicting normal writes 
		size_t maxthread = dloc->lastwrites.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && dloc->lastwrites.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, dloc->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), get_loc_from_dloc(dloc));
	}

	// (load) update last access per thread
	inLocation->lastreads.set(inThread->mTask, nr);

#endif

#if SET_HBSTAMP_ATTRIBUTE
  if (traceDetails && trace_ts)
	  set_hbstamp_attr(inThread->mTask+1, nr, *trace_ts);
  if (storeStamps && trace_ts)
      store_hbstamp(inThread->mTask+1, nr, *trace_ts);
#endif

  if (storeStamps && trace_ts)
	  store_hbstamp(inThread->mTask+1, nr, *trace_ts);

#if TRACE_ACCESSES
  if (traceDetails)
	  trace_instr("load ", trace_ts, trace_sts, trace_rts, inThread, inLocation, nr);
#endif
}

// record a store 
//
void CacheRaceMonitor::record_store(Thread *inThread, SyncVar var, SLocation *inLocation, int inPcId, int nr) {

	Timestamp *trace_ts = 0;
	const Timestamp *trace_sts = 0;
	const TSOTimestamp *trace_rts = 0;

#if DO_SOBER_DETECTION
	if (!dataRacesOnly) {

		// record write
		inLocation->push_write(Write(inThread, inPcId, nr));

		//vector<Write> &writes(inLocation->mWrites);
		//writes.push_back(Write(inThread, inPcId));

		// (store) update hb vector clock and time stamps
		Timestamp &ts(inLocation->last_write(0).mHbStamp);
		ts.merge(inThread->hb_c);
		ts.merge(inLocation->hb_l);
		ts.tick(inThread->mTask, nr);
		inLocation->hb_l = inLocation->hb_s = inThread->hb_c = ts;
		trace_ts = &ts;

		// (store) update rhb vector clocks and time stamps
		TSOTimestamp &rts(inLocation->last_write(0).mRhbStamp);
		rts.merge(inThread->rhb_sc);
		rts.merge(inThread->rhb_lc);
		rts.merge(inLocation->rhb_mc2);
		rts.tick_store(inThread->mTask, nr);
		inLocation->rhb_mc1.assign_all_except_p(rts, inThread->mTask);
		inLocation->rhb_mc2 = inThread->rhb_sc = rts;
		trace_rts = &rts;
	}
#endif

#if DO_DATARACE_DETECTION

	// (store) update sb vector clock and time stamp
	Timestamp &sts(inThread->sb_c);
	sts.tick(inThread->mTask, nr);
	inLocation->sb_s.merge(sts);
	trace_sts = &sts;

	DLocation *dloc = inLocation->dloc;
	if (dloc != 0)
	{
		// (store) check for conflicting normal writes 
		size_t maxthread = dloc->lastwrites.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && dloc->lastwrites.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, dloc->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), get_loc_from_dloc(dloc));

		// (store) check for conflicting normal reads
		maxthread = dloc->lastreads.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && dloc->lastreads.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, dloc->lastreads.get(i)), EventId(inThread->mTask + 1, nr), get_loc_from_dloc(dloc));
	}

	// (store) update last access per thread
	inLocation->lastwrites.set(inThread->mTask, nr);

#endif

#if SET_HBSTAMP_ATTRIBUTE
	if (traceDetails && trace_ts)
		set_hbstamp_attr(inThread->mTask+1, nr, *trace_ts);
#endif

  if (storeStamps && trace_ts)
	  store_hbstamp(inThread->mTask+1, nr, *trace_ts);

#if TRACE_ACCESSES
  if (traceDetails)
	  trace_instr("store", trace_ts, trace_sts, trace_rts, inThread, inLocation, nr);
#endif
}

// record an interlocked operation 
//
void CacheRaceMonitor::record_interlocked(Thread *inThread, SyncVar var, SLocation *inLocation, int nr) {

	Timestamp *trace_ts = 0;
	const Timestamp *trace_sts = 0;
	const TSOTimestamp *trace_rts = 0;

#if DO_SOBER_DETECTION
	if (!dataRacesOnly) {

		// record write
		inLocation->push_write(Write(inThread, 0, nr));

		//vector<Write> &writes(inLocation->mWrites);
		//writes.push_back(Write(inThread, 0));

		// (interlocked) update hb vector clock and time stamps
		Timestamp &ts(inLocation->last_write(0).mHbStamp);
		ts.merge(inThread->hb_c);
		ts.merge(inLocation->hb_l);
		ts.tick(inThread->mTask, nr);
		inLocation->hb_l = inLocation->hb_s = inThread->hb_c = ts;
		trace_ts = &ts;

		// (interlocked) update rhb vector clocks and time stamps
		TSOTimestamp &rts(inLocation->last_write(0).mRhbStamp);
		rts.merge(inThread->rhb_sc);
		rts.merge(inThread->rhb_lc);
		rts.merge(inLocation->rhb_mc2);
		rts.tick_store(inThread->mTask, nr);
		inLocation->rhb_mc1.assign_all(rts);
		inLocation->rhb_mc2 = inThread->rhb_sc = inThread->rhb_lc = rts;
		trace_rts = &rts;
	}
#endif

#if DO_DATARACE_DETECTION

	// (interlocked) update sb vector clock and time stamp
	Timestamp &sts(inThread->sb_c);
	sts.merge(inLocation->sb_s);
	sts.tick(inThread->mTask, nr);
	inLocation->sb_s = sts;
	trace_sts = &sts;

	DLocation *dloc = inLocation->dloc;
	if (dloc != 0)
	{
		// (interlocked) check for conflicting normal writes 
		size_t maxthread = dloc->lastwrites.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && dloc->lastwrites.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, dloc->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), get_loc_from_dloc(dloc));

		// (interlocked) check for conflicting normal reads
		maxthread = dloc->lastreads.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && dloc->lastreads.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, dloc->lastreads.get(i)), EventId(inThread->mTask + 1, nr), get_loc_from_dloc(dloc));
	}

	// (interlocked) update last access per thread
	inLocation->lastwrites.set(inThread->mTask, nr);
	inLocation->lastreads.set(inThread->mTask, nr);

#endif

#if SET_HBSTAMP_ATTRIBUTE
  if (traceDetails && trace_ts)
	  set_hbstamp_attr(inThread->mTask+1, nr, *trace_ts);
#endif

  if (storeStamps && trace_ts)
	  store_hbstamp(inThread->mTask+1, nr, *trace_ts);

#if TRACE_ACCESSES 
  if (traceDetails)
	  trace_instr("intlk", trace_ts, trace_sts, trace_rts, inThread, inLocation, nr);
#endif
}

#if DO_DATARACE_DETECTION

void CacheRaceMonitor::record_dload(Thread *inThread, void* loc, DLocation *inLocation, int nr) {

	// (dload) check for conflicting normal writes
	size_t maxthread = inLocation->lastwrites.getMaxPos();
	for (size_t i = 0; i <= maxthread; ++i)
		if (i != inThread->mTask && inLocation->lastwrites.get(i) > inThread->sb_c.get(i))
			reportDataRace(EventId(i + 1, inLocation->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), loc);
	// (dload) check for conflicting sync writes
	SLocation *sloc = inLocation->sloc;
	if (sloc != 0)
	{
		maxthread = sloc->lastwrites.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && sloc->lastwrites.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, sloc->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), loc);
	}

	// (dload) update last access per thread
	inLocation->lastreads.set(inThread->mTask, nr);

	// TEMP too costly
#if SET_HBSTAMP_ATTRIBUTE
	if (traceDetails) {
		Timestamp &ts(
#if DO_SOBER_DETECTION
inThread->hb_c
#else
inThread->sb_c
#endif
        );
		ts.tick(inThread->mTask, nr);
		set_hbstamp_attr(inThread->mTask+1, nr, ts);
	}
#endif
}

void CacheRaceMonitor::record_dstore(Thread *inThread, void* loc, DLocation *inLocation, int nr) {

	// (dstore) check for conflicting normal writes
	size_t maxthread = inLocation->lastwrites.getMaxPos();
	for (size_t i = 0; i <= maxthread; ++i)
		if (i != inThread->mTask && inLocation->lastwrites.get(i) > inThread->sb_c.get(i))
			reportDataRace(EventId(i + 1, inLocation->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), loc);
	// (dstore) check for conflicting sync writes 
	SLocation *sloc = inLocation->sloc;
	if (sloc != 0)
	{
		maxthread = sloc->lastwrites.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && sloc->lastwrites.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, sloc->lastwrites.get(i)), EventId(inThread->mTask + 1, nr), loc);
	}

	// (dstore) check for conflicting normal reads
	maxthread = inLocation->lastreads.getMaxPos();
	for (size_t i = 0; i <= maxthread; ++i)
		if (i != inThread->mTask && inLocation->lastreads.get(i) > inThread->sb_c.get(i))
			reportDataRace(EventId(i + 1, inLocation->lastreads.get(i)), EventId(inThread->mTask + 1, nr), loc);
	// (dstore) check for conflicting sync reads
	if (sloc != 0)
	{
		maxthread = sloc->lastreads.getMaxPos();
		for (size_t i = 0; i <= maxthread; ++i)
			if (i != inThread->mTask && sloc->lastreads.get(i) > inThread->sb_c.get(i))
				reportDataRace(EventId(i + 1, sloc->lastreads.get(i)), EventId(inThread->mTask + 1, nr), loc);
	}

	// (dstore) update last access per thread
	inLocation->lastwrites.set(inThread->mTask, nr);

	// TEMP too costly
#if SET_HBSTAMP_ATTRIBUTE
	if (traceDetails) {
		Timestamp &ts(
#if DO_SOBER_DETECTION
inThread->hb_c
#else
inThread->sb_c
#endif
        );
		ts.tick(inThread->mTask, nr);
		set_hbstamp_attr(inThread->mTask+1, nr, ts);
	}
#endif
}


#endif

// tracing functions

void CacheRaceMonitor::trace_vclock(ostream &inStream, Timestamp &inClock) {
  inClock.print(inStream);
  inStream << " ";
}
void CacheRaceMonitor::trace_tsoclock(ostream &inStream, TSOTimestamp &inClock) {
  inClock.print(inStream);
  inStream << " ";
}

void CacheRaceMonitor::set_hbstamp_attr(size_t inTid, size_t inN, Timestamp &inHbStamp) {
	ostringstream sstr;
	inHbStamp.print(sstr);
	Chess::SetEventAttribute(EventId(inTid, inN), HBSTAMP, sstr.str().c_str());
}

// interface for querying hbstamps
void CacheRaceMonitor::store_hbstamp(size_t inTid, size_t inN, const Timestamp &inHbStamp) {
	stamps[EventId(inTid, inN)] = inHbStamp;
}
// turns on hbstamp recording, resets store
void CacheRaceMonitor::start_hbstamp_recording() {
   storeStamps = true;	
   stamps.clear();
}
// returns the stored hbstamp for the given id
Timestamp &CacheRaceMonitor::get_hbstamp(EventId id) {
	return stamps[id];
}
// returns the element in ts associated with tid (Katie)
size_t CacheRaceMonitor::get_hbstamp_element(Timestamp& ts, Task tid) {
	return ts.get(get_thread(tid)->mTask);
}

void CacheRaceMonitor::trace_instr(const string &inKind, Timestamp *inHbStamp, Timestamp *inSoStamp, TSOTimestamp *inRhbStamp,
								   const Thread *inThread, const SLocation *inLocation, size_t inIndex)
{
	if (inHbStamp) trace_vclock(*GetChessOutputStream(), *inHbStamp);
	if (inRhbStamp) trace_tsoclock(*GetChessOutputStream(), *inRhbStamp);
	if (inSoStamp) trace_vclock(*GetChessOutputStream(), *inSoStamp);
	*GetChessOutputStream() << "t" << (inThread->mTask+1) << "." << inIndex 
		<< setw((int) inThread->mTask * 10) << setfill(' ')
		<< " " << inKind << " A" << inLocation->mIndex;
#if TRACE_LOCATION 
	if (TRACE_LOCATION == inLocation->mIndex) {
		*GetChessOutputStream() << "         hb_s=";
		trace_vclock(*GetChessOutputStream(), inLocation->hb_s);
		*GetChessOutputStream() << " hb_l=";
		trace_vclock(*GetChessOutputStream(), inLocation->hb_l);
		*GetChessOutputStream() << " sb_c=";
		trace_vclock(*GetChessOutputStream(), inLocation->sb_s);
	}
#endif
	*GetChessOutputStream() << endl;
}

size_t POOR_MAN_FLUSH_LIMIT = 900000;
void CacheRaceMonitor::Flush(){
	if(mNormalLocations.size() > POOR_MAN_FLUSH_LIMIT ||
		mSyncvarLocations.size() > POOR_MAN_FLUSH_LIMIT  ||
		mThreads.size() > POOR_MAN_FLUSH_LIMIT){
			*GetChessOutputStream() << "Poor Man Flush @ " << mNormalLocations.size() << " " << mSyncvarLocations.size() << " " << mThreads.size() << std::endl;
			//POOR_MAN_FLUSH_LIMIT += 100000;
		// Emulate a new execution
			clear();
			return;
	}
}

CacheRaceMonitor* theMonitor = 0;
extern "C" __declspec(dllexport) IChessMonitor* GetMonitor(){
  if(!theMonitor)
    theMonitor = new CacheRaceMonitor(false, false, false, 0);
  return theMonitor;
}

