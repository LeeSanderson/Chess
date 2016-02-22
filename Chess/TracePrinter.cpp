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

#include "TracePrinter.h"
#include "Chess.h"
#include <sstream>
#include "ChessOptions.h"
#include "ChessStream.h"

/** construct/destruct */

TracePrinter::TracePrinter(const ChessOptions *opts)
{
	mRecordDetails = opts->load_schedule || opts->logging;
	num_execs = 0;
#ifndef UNDER_CE
	mGui = opts->gui;
#else
	// Don't connect to Concurrency Explorer GUI under CE
	mGui = false;
#endif
	mStream.open(std::string(opts->output_prefix).append("trace").c_str());
	if (mGui) {
		ConnectGui();
	}
	PrintHeader(opts->load_schedule ? "s" : "m");
}
TracePrinter::~TracePrinter()
{
}

/** IChess interface */

void TracePrinter::OnExecutionBegin(IChessExecution* exec){

	num_execs++;

	// guard against multiple executions in repro file (caused by nondet handling)
	if (num_execs > 1 && !mGui && Chess::GetOptions().load_schedule && Chess::GetOptions().max_executions == 1)
	{
		mStream.close();
		mStream.open(std::string(Chess::GetOptions().output_prefix).append("trace").c_str());
		PrintHeader("s");
		num_execs = 0;
	}
 
	PrintExecutionBegin();
}
void TracePrinter::OnSchedulePoint(SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid) {
	PrintSchedulePoint(type, id, var, op, sid);
}
void TracePrinter::OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid) {
	PrintVarAccess(id, op, var, false, false, sid);
}
void TracePrinter::OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid){
	for (int i = 0; i < n; ++i)
		PrintVarAccess(id, op, var[i], false, true, sid);
}
void TracePrinter::OnDataVarAccess(EventId id, void *loc, int size, bool isWrite, size_t pcId) {
	PrintVarAccess(id, isWrite ? SVOP::DATA_WRITE : SVOP::DATA_READ, (int) loc / 4, true, false, 0);
}
void TracePrinter::OnShutdown() {
	PrintEnd();
}
void TracePrinter::OnEventAttributeUpdate(EventId id, EventAttribute attr, const char *val) {
	if (val != NULL && (*val == 'p' || *val == 'b')) {
		int showPreemptions = 1;
		Chess::GetOptions().GetValue("ConcurrencyExplorer::showPremptionsInGui", showPreemptions);
		if(showPreemptions)
			PrintTuple((int) id.tid, id.nr, attr, val);
	} else {
		PrintTuple((int) id.tid, id.nr, attr, val);
	}
}

void TracePrinter::OnTraceEvent(EventId id, const std::string &info) {
    PrintTracedEvent(id, info);
}

/** utility printing functions */

#define MAX_FRAME_DEPTH 16
#define MAX_STRLEN 1024
#define MAX_BUFSIZE 16*1024

static const char *getStackTraceString(int minFrame, int maxFrame, char** procedures, char** fileNames, const int* lineNumbers){
	static char buf[MAX_BUFSIZE];
	int pos = 0;
	for(int i=minFrame; i<maxFrame; i++){
		if(procedures[i][0] == 0)
			continue;
		if(fileNames[i][0] == 0)
			continue;
		pos += sprintf_s(buf+pos, MAX_BUFSIZE-pos,
			"%s|%s|%d|", procedures[i], fileNames[i], lineNumbers[i]);
	} 
	return buf;
}

static const char *printint(int i) {
	static char buf[MAX_STRLEN];
	sprintf_s(buf, MAX_STRLEN, "%d", i);
	return buf;
}

/** the core printing functions */

void TracePrinter::PrintSchedulePoint(IChessMonitor::SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid) {
	//if (type == IChessMonitor::SVACCESS) 
	{
		// send VAR_OP attribute
		PrintTuple(id.tid, id.nr, VAR_OP, SVOP::ToString(op));
		// send VAR_ID attribute
		int varid = var;
		if (varid)
			PrintTuple(id.tid, id.nr, VAR_ID, printint(varid));
		// send STACKTRACE attribute
		if(mRecordDetails) {
			PrintStackTrace(id);
		} 
	}
}

void TracePrinter::PrintTracedEvent(EventId id, const std::string& info)
{
    // send VAR_OP and VAR_ID (reserved values)
	PrintTuple((int) id.tid, id.nr, VAR_OP, SVOP::ToString(SVOP::TRACED_EVENT));
	PrintTuple((int) id.tid, id.nr, VAR_ID, "0");
	// send info
    PrintTuple((int) id.tid, id.nr, INSTR_METHOD, info.c_str());
	// send STACKTRACE attribute
	if(mRecordDetails) {
		PrintStackTrace(id);
	} 
}

void TracePrinter::PrintVarAccess(EventId id, int transop, int transvar, bool isData, bool isAggregate, size_t sid) {

	if (isData) {
		// send VAR_OP attribute
		PrintTuple(id.tid, id.nr, VAR_OP, SVOP::ToString(transop));
	}
	if (isData || isAggregate) {
		// send VAR_ID attribute
		PrintTuple(id.tid, id.nr, VAR_ID, printint(transvar));
	}
	if (isData && mRecordDetails) {
		// send STACKTRACE attribute
		PrintStackTrace(id);
	}

	// send SID attribute for non-data accesses
	if (!isData)
		PrintTuple(id.tid, id.nr, EVT_SID, printint(sid));
	
	// send THREADNAME attribute
	if (mRecordDetails && !isAggregate && !isData) {
		static char title[MAX_STRLEN];
		title[0] = '\0';
		Chess::GetSyncManager()->GetTaskName((int)id.tid,title,MAX_STRLEN);
		if (title[0])
			PrintTuple((int)id.tid, id.nr, THREADNAME, title);
	}

}

void TracePrinter::PrintStackTrace(EventId id) {
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
	if(Chess::GetSyncManager()->GetCurrentStackTrace(MAX_FRAME_DEPTH, MAX_STRLEN, proc, filename, lineno)) {
		PrintTuple((int)id.tid, id.nr, STACKTRACE, getStackTraceString(0, MAX_FRAME_DEPTH, proc, filename, lineno));
	}
}


/** code to set up the child process */

// THIS FUNCTIONALITY IS CURRENTLY DISABLED -- we are not allowing GUI to run "live"
 
#include <windows.h> 
#include <tchar.h>
#include <stdio.h> 
 
HANDLE hChildStdinRd, hChildStdinWr, hChildStdoutRd, hChildStdoutWr, hInputFile, hStdout;
 
void TracePrinter::ConnectGui()
{
#ifndef UNDER_CE
	SECURITY_ATTRIBUTES saAttr; 
	bool fSuccess; 
 
	// Set the bInheritHandle flag so pipe handles are inherited. 
 
	saAttr.nLength = sizeof(SECURITY_ATTRIBUTES); 
	saAttr.bInheritHandle = TRUE; 
	saAttr.lpSecurityDescriptor = NULL; 

	// Get the handle to the current STDOUT. 
 
	hStdout = GetStdHandle(STD_OUTPUT_HANDLE); 
 
	// Create a pipe for the child process's STDOUT. 
 
	if (! CreatePipe(&hChildStdoutRd, &hChildStdoutWr, &saAttr, 0)) 
		ErrorExit("Stdout pipe creation failed\n"); 

	// Ensure that the read handle to the child process's pipe for STDOUT is not inherited.

	SetHandleInformation( hChildStdoutRd, HANDLE_FLAG_INHERIT, 0);

	// Create a pipe for the child process's STDIN. 
 
	if (! CreatePipe(&hChildStdinRd, &hChildStdinWr, &saAttr, 0)) 
		ErrorExit("Stdin pipe creation failed\n"); 

	// Ensure that the write handle to the child process's pipe for STDIN is not inherited. 
 
	SetHandleInformation( hChildStdinWr, HANDLE_FLAG_INHERIT, 0);
 
	//*GetChessOutputStream() << "Creating..." << std::endl;

	// Now create the child process. 
   
	fSuccess = CreateChildProcess();
	if (! fSuccess) 
		ErrorExit("Create process failed with"); 
#endif
} 
 
bool TracePrinter::CreateChildProcess() 
{
#ifndef UNDER_CE
    TCHAR buf[MAX_STRLEN];
	sprintf_s(buf, MAX_STRLEN, "ConcurrencyExplorer.exe");

	PROCESS_INFORMATION piProcInfo; 
	STARTUPINFO siStartInfo;
	BOOL bFuncRetn = FALSE; 
 
	// Set up members of the PROCESS_INFORMATION structure. 
 
	ZeroMemory( &piProcInfo, sizeof(PROCESS_INFORMATION) );
 
	// Set up members of the STARTUPINFO structure. 
 
	ZeroMemory( &siStartInfo, sizeof(STARTUPINFO) );
	siStartInfo.cb = sizeof(STARTUPINFO); 
	siStartInfo.hStdError = hChildStdoutWr;
	siStartInfo.hStdOutput = hChildStdoutWr;
	siStartInfo.hStdInput = hChildStdinRd;
	siStartInfo.dwFlags |= STARTF_USESTDHANDLES;
 
	// Create the child process. 
    
	bFuncRetn = CreateProcess(NULL, 
		buf,           // command line 
		NULL,          // process security attributes 
		NULL,          // primary thread security attributes 
		TRUE,          // handles are inherited 
		0,             // creation flags 
		NULL,          // use parent's environment 
		NULL,          // use parent's current directory 
		&siStartInfo,  // STARTUPINFO pointer 
		&piProcInfo);  // receives PROCESS_INFORMATION 
   
	if (bFuncRetn == 0) {
		ErrorExit("Could not launch ConcurrencyExplorer.exe... not in path?\n");
		return false;
	}
	else 
	{
		CloseHandle(piProcInfo.hProcess);
		CloseHandle(piProcInfo.hThread);
		return (bFuncRetn != 0);
	}
#else
	// Don't connect to Concurrency Explorer GUI under CE
	return true;
#endif
}
VOID TracePrinter::ErrorExit (const char *msg) 
{ 
	fprintf(stderr, "%s\n", msg); 
	ExitProcess(0); 
}



/** the low-level printing to the file stream and GUI pipe */

#define BUFSIZE 3000
static char chBuf[BUFSIZE+1]; 
static int bufpos = 0;

void TracePrinter::PrintHeader(const char *str) {
	bufpos += sprintf_s(chBuf + bufpos, BUFSIZE - bufpos, "%s\n", str);
    flush();
}

void TracePrinter::PrintExecutionBegin() {
	// flush if the buffer is too full
	if (bufpos + 4 >= BUFSIZE)
		flush();
	bufpos += sprintf_s(chBuf + bufpos, BUFSIZE - bufpos, "#\n");
}


void TracePrinter::PrintTuple(int tid, int nr, int attr, const char* valstr) {
	int len = strlen(valstr);
	int bound = 60 + len;
	if (bufpos + bound >= BUFSIZE)
		flush();
	bufpos += sprintf_s(chBuf + bufpos, BUFSIZE - bufpos, "%d %d %d %d %s\n", tid, nr, attr, len, valstr);
	// flush after each status event
	if (attr == STATUS)
		flush();
}

void TracePrinter::PrintEnd() {
	flush();
	mStream.close();
	if (mGui) {
		// Close the pipe handle so the child process stops reading. 
		if (! CloseHandle(hChildStdinWr)) 
			ErrorExit("Close pipe failed\n"); 
	}
}

void TracePrinter::flush() {
	if (mGui) {
		DWORD byteswritten;
		if (! WriteFile(hChildStdinWr, chBuf, bufpos, &byteswritten, NULL)) 
		{
			// nothing - tolerate broken pipe (e.g. closed GUI)
			*GetChessErrorStream() << "broken pipe to ConcurrrencyExplorer (code=" << GetLastError() << ")" << std::endl;
		} 
	}
	chBuf[bufpos++] = '\0';
	mStream << chBuf;
	bufpos = 0;
}
