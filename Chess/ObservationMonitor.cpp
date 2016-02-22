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

#include "ObservationMonitor.h"
#include "Chess.h"
#include <map>
#include <sstream>

#include "ChessExecution.h"
#include "ChessOptions.h"
#include "ChessImpl.h"
#include "IQueryEnabled.h"
#include "CacheRaceMonitor.h"
#include "AtomicityMonitor.h"

#include "ObservationSet.h"
#include "Observation.h"

using namespace std;

ObservationMonitor::ObservationMonitor(const ChessOptions *options, CacheRaceMonitor *cacheracemonitor)
{
    this->options = options;
    this->cacheracemonitor = cacheracemonitor;

	specmining = (options->enumerate_observations != 0 && options->enumerate_observations[0] != '\0');

	// parse observation mode
	obsmode = (specmining) ? om_coarse : om_SC; // defaults
	if (options->observation_mode != NULL)
	{
		string omodestring(options->observation_mode);
		if (omodestring == "SC")
			obsmode = om_SC;
		else if (omodestring == "SC_s")
			obsmode = om_SC_s;
		else if (omodestring == "atom")
			obsmode = om_atom;
		else if (omodestring == "all")
			obsmode = om_all;
		else if (omodestring == "coarse")
			obsmode = om_coarse;
		else if (omodestring == "serial")
			obsmode = om_serial;
		else if (omodestring == "lin")
			obsmode = om_lin;
		else if (omodestring == "lin_s")
			obsmode = om_lin_s;
	}

	// check if mode is correct for this operation
	if (specmining != (obsmode == om_all || obsmode == om_coarse || obsmode == om_serial))
	{
		std::stringstream s;
		s << "Invalid observation mode: " << options->observation_mode << endl;
		ChessImpl::ChessAssertion(s.str().c_str(), CHESS_EXIT_INVALID_OBSERVATION);
	}

	// set up current observation record
	curobs = new Observation(this);

	if (obsmode == om_atom)
	{
		assert (!specmining);
		// create atomicity monitor
		atomicitymonitor = new AtomicityMonitor(cacheracemonitor, curobs);
		ChessImpl::RegisterMonitor(atomicitymonitor);
	}
	else
	{
		curset = new ObservationSet(this);

		if (!specmining)
		{
			assert(options->check_observations[0] != '\0');
			curset->Load(options->check_observations);
		}
	}
\
}


ObservationMonitor::~ObservationMonitor()
{
	if (obsmode != om_atom)
		delete curset;
	delete curobs;
}

void ObservationMonitor::OnShutdown() {
	if (specmining)
		curset->Save(options->enumerate_observations);
}

void ObservationMonitor::OnExecutionBegin(IChessExecution* exec)
{
	curobs->clear();
}

void ObservationMonitor::OnExecutionEnd(IChessExecution* exec)
{
	if (! curobs->normalize())
	{
		ostringstream s;
		s << "Invalid Observation - CHESS encountered an execution whose observation is not" << endl
			<< "well formed:" << endl;
		curobs->serialize_callhistory(s, false);
		curobs->serialize_interleaving(s);
		ChessImpl::ChessAssertion(s.str().c_str(), CHESS_EXIT_INVALID_OBSERVATION);
	}

	//curobs->serialize_callhistory(std::cout);
	//curobs->serialize_interleaving(std::cout);
	//cout << endl;

	
	// check/add interleaving
	if (obsmode != om_atom)
	{
		if (specmining)
			curset->Add(*curobs);
		else 
		{
			if (! ((obsmode == om_lin_s || obsmode == om_SC_s) && curobs->has_more_than_one_blocked()))
				curset->Check(*curobs, false);
			else 
			{   // check individual blocked ops
				int count = 0;
				while (curobs->suppress_next(count++))
				{
					curobs->unnormalize();
					curobs->normalize();
                    curset->Check(*curobs, true);

					//curobs->serialize_callhistory(std::cout);
					//curobs->serialize_interleaving(std::cout);
					//cout << endl;

				}
			}
		}
	}
}

void ObservationMonitor::ReportInvalidObservation(Observation &obs, bool some_ops_suppressed)
{
	std::stringstream ss;
	if (some_ops_suppressed)
	{
		ss << "The following blocking prefix can not be matched:" << endl
		<< obs.get_key();
		obs.serialize_interleaving(ss);

        // restore full observation
		obs.unnormalize();
		obs.unsuppress();
		obs.normalize();
	}

	// report error
	std::stringstream s;
	s << "Invalid Observation - CHESS encountered an execution that is not" << endl
		<< "consistent with the observations listed in " << options->check_observations << ":" << endl;
	obs.serialize_callhistory(s, false);
	obs.serialize_interleaving(s);
	if (some_ops_suppressed)
		s << endl << ss.str() << endl;
	ChessImpl::ChessAssertion(s.str().c_str(), CHESS_EXIT_INVALID_OBSERVATION);
}

void ObservationMonitor::ReportFileError(const std::string &action, const std::string &filename)
{
	// report error
	std::stringstream s;
	s << "Could not " << action << " observation file " << filename << "." << endl;
	ChessImpl::ChessAssertion(s.str().c_str(), CHESS_EXIT_INVALID_OBSERVATION);
}

void ObservationMonitor::Call(Task tid, void *object, const char *opname, bool iscallback)
{
	int depth = curobs->cur_depth(tid);
		
	EventId id(ChessImpl::TraceEvent(std::string(iscallback ? "CALLBACK " : "CALL ").append(opname)));

	if (specmining && (obsmode == om_coarse || obsmode == om_serial))
	{
		if (!iscallback)
			ChessImpl::PreemptionDisable();
		else
			ChessImpl::PreemptionEnable();
	}
	
	if (!curobs->add_call(id.tid, object, opname, iscallback))
		ChessImpl::ChessAssertion("Incorrect use of CHESS API: improper nesting of call/return/callback/callbackreturn.", CHESS_EXIT_INVALID_TEST);

	if (obsmode == om_atom && depth == 0)
		atomicitymonitor->opcall(id, object, opname);
}

void ObservationMonitor::Return(Task tid, bool iscallback)
{
	int depth = curobs->cur_depth(tid);

	if (specmining && (obsmode == om_coarse || obsmode == om_serial))
	{
		if (!iscallback)
			ChessImpl::PreemptionEnable();
		else
			ChessImpl::PreemptionDisable();
	}

	EventId id(ChessImpl::TraceEvent(std::string(iscallback ? "CALLBACKRETURN " : "RETURN ")));

	if (!curobs->add_return(id.tid, iscallback))
		ChessImpl::ChessAssertion("Incorrect use of CHESS API: improper nesting of call/return/callback/callbackreturn.", CHESS_EXIT_INVALID_TEST);

	if (obsmode == om_atom && depth == 1)
		atomicitymonitor->opreturn(id);
}

void ObservationMonitor::Deadlock(Task tid)
{
   curobs->deadlock(tid);
}

void ObservationMonitor::MarkTimeout(Task tid)
{
	if (curobs->enter_timeout(tid) && specmining)
		ChessImpl::PreemptionEnable();
}

void ObservationMonitor::SyncVarAccessCommitted(Task tid)
{
	if (curobs->exit_timeout(tid) && specmining)
		ChessImpl::PreemptionDisable();
}


void ObservationMonitor::CheckBlock(IQueryEnabled *queryenabled, int transition)
{
    bool blocked = true;
    Task tid = 0;
    if ((tid = curobs->get_next_active_thread(tid)) != 0)
	{
	    do
		{
          if (queryenabled->IsEnabledAtStepWithNoFairness(transition, tid))
		      return;
		}
		while ((tid = curobs->get_next_active_thread(tid)) != 0);
        // all active threads are blocked
		curobs->stalled();
		if (!specmining && (obsmode == om_lin_s || obsmode == om_SC_s))
		{   
			// check individual blocked ops
			int count = 0;
			while (curobs->suppress_next(count++))
			{
				curobs->normalize();
				curset->Check(*curobs, curobs->has_more_than_one_blocked());
				curobs->unnormalize();
			}
		}

	}
}


void ObservationMonitor::IntValue(const char *label, long long value)
{
	std::stringstream sstr;
	sstr << "OBSERVE " << label << "=" << value << " (int)";
	EventId id(ChessImpl::TraceEvent(sstr.str()));

	curobs->add_integer(id.tid, label, value);
}
 
void ObservationMonitor::PointerValue(const char *label, void *value)
{
	std::stringstream sstr;
	sstr << "OBSERVE " << label << "=" << value << " (ptr)";;
	EventId id(ChessImpl::TraceEvent(sstr.str()));
    curobs->add_pointer(id.tid, label, value);
}

void ObservationMonitor::StringValue(const char *label, const char *value)
{
	EventId id(ChessImpl::TraceEvent(std::string("OBSERVE ").append(label).append("=").append(value).append(" (str)")));
    curobs->add_string(id.tid, label, value);
}

