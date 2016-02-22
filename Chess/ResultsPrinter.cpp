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

#include <windows.h>
#include <winnls.h>
#include "ResultsPrinter.h"
#include "ChessApi.h"	// To get access to the Exit Codes
#include "Chess.h"
#include <sstream>

#include "ChessExecution.h"
#include "ChessOptions.h"
#include "ChessImpl.h"
#include "StatsMonitor.h"

#include <ctime>
using namespace std;

// printing utility functions


/* This function uses _time64 to get the current time 
 * and then uses localtime64() to convert this time to a structure 
 * representing the local time. The program converts the result 
 * from a 24-hour clock to a 12-hour clock and determines the 
 * proper extension (AM or PM).
 */
static void timestamp(ostream &stream)
{
	struct tm newtime;
	__time64_t long_time;
	char timebuf[26];
#ifdef UNDER_CE
    WCHAR wtimebuf[26];
#endif

	errno_t err;

	// Get time as 64-bit integer.
	_time64( &long_time ); 
	// Convert to local time.
	err = _localtime64_s( &newtime, &long_time ); 
	if (err)
	{
		printf("Invalid argument to _localtime64_s.");
		exit(1);
	}

	bool isPM = ( newtime.tm_hour > 12 );        // Set up extension. 
	if( newtime.tm_hour > 12 )        // Convert from 24-hour 
		newtime.tm_hour -= 12;    // to 12-hour clock. 
	if( newtime.tm_hour == 0 )        // Set hour to 12 if midnight.
		newtime.tm_hour = 12;

	// Convert to an ASCII representation. 
#ifndef UNDER_CE	
	err = asctime_s(timebuf, 26, &newtime);
    if (err)
    {
        printf("Invalid argument to asctime_s.");
        exit(1);
    }
#else
    err = wcsftime(wtimebuf, sizeof(wtimebuf)/sizeof(wtimebuf[0]), L"%a %b %d %I:%M:%S %Y\n", &newtime);
    if (!err)
    {
        printf("Invalid argument to wcsftime.");
        exit(1);
    }
#endif
#ifdef UNDER_CE
    err = ::WideCharToMultiByte(CP_ACP, 0, wtimebuf, sizeof(wtimebuf)/sizeof(wtimebuf[0]), timebuf, sizeof(timebuf)/sizeof(timebuf[0]), NULL, NULL);
	if ( !err )
	{
	    printf("WideCharToMultiByte failed.");
		exit(1);
	}
#endif
	timebuf[24] = '\0'; // remove newline
	
	stream << timebuf << " " << (isPM ? "PM" : "AM");
}

/** construct/destruct */

ResultsPrinter::ResultsPrinter(const ChessOptions *options, StatsMonitor *stm) : IChessMonitor(false)
{
    num_results = 0;
    this->options = options;
	this->stm = stm;
	race_count = 0;
	warning_count = 0;
    final = false;
	pruned_schedules = 0;
	pruned_time = 0;
	shutdown_called = 0;
	areFinalStatsReported = false;

    // start the results file
	mStream.open(std::string(options->output_prefix).append("results.xml").c_str());
	mStream << "<?xml version=\"1.0\" encoding=\"utf-8\"?>" << endl
		<< "<results xmlns=\"http://research.microsoft.com/chess\">" << endl
		<< "  <starttime>";
	timestamp(mStream);
	mStream << "</starttime>" << endl
		<< "  <commandline>" << endl
		<< options->xml_commandline;
	if (options->load_schedule)
		EncodeSchedule();
	mStream << "  </commandline>" << endl;
}

ResultsPrinter::~ResultsPrinter()
{
}

// begin{IChessMonitor}

void ResultsPrinter::OnExecutionBegin(IChessExecution* exec){
	this->preemptionMethods.clear();
}

void ResultsPrinter::OnEventAttributeUpdate(EventId id, EventAttribute attr, const char *val) 
{
	if (Chess::GetOptions().record_preempt_methods) {
		if (val != NULL && (*val == 'p')) 
		{
			char name[512];
			int len = 0;
			if (Chess::GetSyncManager()->GetFullyQualifiedTopProcedure(len,name,512) && name!=NULL) {
				this->preemptionMethods.push_back(name);
			}
		}
	}
}


// end{IChessMonitor}

void ResultsPrinter::EncodeSchedule()
{
	ostringstream sstr;
	ChessImpl::CurrExecution()->Serialize(sstr);
	string &str(sstr.str());

	static const char *hexchars = "0123456789ABCDEF";

	mStream << "    <schedule format=\"hex\">";
	for (unsigned pos = 0; pos < str.size(); pos++) {
		if (pos % 64 == 0)
			mStream << endl << "    ";
		if (pos % 4 == 0)
			mStream << " ";
		mStream << hexchars[(str[pos] >> 4) & 0xF] << hexchars[str[pos] & 0xF] ;
	}
	mStream << endl << "    </schedule>" << endl;
}

static void escape(std::ostream &stream, const std::string &str)
{
   for (unsigned pos = 0; pos < str.length(); pos++)
   {
      switch(str[pos])
      {
	     case '<': stream << "&lt;"; break;
	     case '>': stream << "&gt;"; break;
	     case '&': stream << "&amp;"; break;
		 default: stream << str[pos];
      }
   }
}

void ResultsPrinter::CaptureResult(char category, 
                                   const std::string &description, 
								   const std::string &actions,
								   bool includeschedule,
								   const ErrorInfo* errorInfo
								   )
{
	int index = ++num_results;

	//*GetChessOutputStream() << "[" << category << index << "] " << description << endl;
	//*GetChessOutputStream().flush();

     // update counts
	if (category == 'W')
		warning_count++;
	else if (category == 'R')
		race_count++;
	else if ((category == 'E') || (category == 'N'))
	{
		if (final)
			return; // can not report more than one error or notification
		final = true;
	}

	mStream << "  <result>" << endl;
	mStream << "    <label>" << category ;
	if (!final)
		mStream << index;
	mStream << "</label>" << endl;
	mStream << "    <description>";
	escape(mStream, description);
	mStream << "</description>" << endl;

	if(errorInfo != NULL)
		errorInfo->WriteXml(mStream);

	mStream << actions;

	if (includeschedule)
		EncodeSchedule();
	if (!this->preemptionMethods.empty()) 
	{
		for(std::list<const string>::iterator it = this->preemptionMethods.begin(); 
			it != preemptionMethods.end(); ++it) {
			mStream << "    <preemptionmethod>";
			escape(mStream, *it);
			mStream << "</preemptionmethod>" << endl;
		}
	}
	mStream << "  </result>" << endl;
}


void ResultsPrinter::AddError(const char* description, const char* action, const ErrorInfo* errorInfo)
{
	ostringstream actions;
	actions << action;
	if (options->trace)
		actions << "    <action name=\"View\" />" << endl;
	actions << "    <action name=\"Repro\" />" << endl;
	CaptureResult('E', description, actions.str(), true, errorInfo);
}

void ResultsPrinter::AddWarning(const char* description, const char* action, bool withrepro)
{
	ostringstream actions;
	actions << action;
	if (withrepro)
		actions << "    <action name=\"Repro\" />" << endl;
	CaptureResult('W', description, actions.str(), withrepro, NULL);
}

void ResultsPrinter::ReportFinalStatistics(int exitCode)
{
	areFinalStatsReported = true;

	// If we detected races, then we need to report them
	if(exitCode == 0 && race_count > 0)
		exitCode = CHESS_EXIT_RACE;

	// Add the final statistics as we end
	mStream << "<finalStats exitCode=\"" << exitCode << 
				"\" raceCount=\"" << race_count << 
				"\" warningCount=\"" << warning_count << 
				"\" schedulesRan=\"" << stm->GetNumExecutions() << 
				"\" lastThreadCount=\"" << stm->GetMaxThreads() <<
				"\" lastExecSteps=\"" << stm->GetMaxSteps() <<
				"\" lastHBExecSteps=\"" << stm->GetNumHBExecutions() <<
				"\" />" << endl;
}

void ResultsPrinter::CloseResults()
{
	// finish results.xml file
	mStream << "  <endtime>";
	timestamp(mStream);
	mStream << "</endtime>" << endl;

	mStream << "</results>" << endl;
	mStream.close();

	shutdown_called = true;	// Cause calling OnShutdown after closing the stream would be bad.
}

void ResultsPrinter::PruneBySchedules(int s)
{
	pruned_schedules = s;
}

void ResultsPrinter::PruneByTime(int t)
{
	pruned_time = t;
}

bool ResultsPrinter::SearchIsPruned()
{
	return (pruned_time || pruned_schedules);
}

void ResultsPrinter::OnShutdown(int exitCode) {
	// this gets called twice sometimes... guard against that here
    if (shutdown_called)
		return;

	shutdown_called = true;

	bool is_repro = (options->max_executions == 1);

	// if there were no errors and this is not a repro, print notification that search has completed
	if (! final && ! is_repro)
	{
		ostringstream completionmessage;
		if (pruned_time)
			completionmessage << "Search Interrupted After " << pruned_time << " Seconds";
        else if (pruned_schedules)
			completionmessage << "Search Interrupted After " << pruned_schedules << " Schedules";
		else
			completionmessage << "Search Completed";

		string action = (pruned_schedules || pruned_time) ?  "<action name=\"Continue\"/>" : "";

		CaptureResult('N', completionmessage.str(), action, false, NULL);
		
		//print notification to standard error
		*GetChessErrorStream()<< completionmessage.str() << " (" << warning_count << " Warning" << ((warning_count != 1) ? "s" : "");
		if (options->sober)
			*GetChessErrorStream()<< ", " << race_count << " Race"<< ((race_count != 1) ? "s" : "");
		*GetChessErrorStream()<< ")." << endl;
	}

	// add continue action
	if (!is_repro && (pruned_schedules || pruned_time))
		mStream << "    <action name=\"Continue\"/>" << endl;

	if(!areFinalStatsReported)
		ReportFinalStatistics(exitCode);

	CloseResults();
}

