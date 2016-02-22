/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Checker.cpp : Defines the entry point for the console application.
//
#pragma warning (push)
#include <CodeAnalysis\Warnings.h>
#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings

#include <windows.h>
#include "ChessAssert.h"
#include <iostream>
#include <vector>
#include <string>
#include <crtdbg.h>
#include <sstream>
#pragma warning (pop)
using namespace std;

#define CHESS_IMPORT
#include "Chess.h"
#include "SyncVar.h"
#include "ChessStream.h"

bool CHESS_API UnitTests();

#ifdef UNDER_CE

#define LoadLibraryA   My_LoadLibrary

HINSTANCE My_LoadLibrary(__in LPCSTR lpLibFileName)
{
    WCHAR wLibFileName[MAX_PATH];

    int res = ::MultiByteToWideChar(CP_ACP, 0, lpLibFileName, -1, wLibFileName, sizeof(wLibFileName)/sizeof(wLibFileName[0]));
    if ( !res )
    {
        return NULL;
    }

    return LoadLibraryW(wLibFileName);
}

#endif

void SplitArgument(const string &str, vector<string>& ret, char c = ':')
{
    string::size_type pos = str.find(c);
    if(pos == string::npos){
        ret.push_back(str);
    }
    else{
        if(pos == 0 || pos == str.length()-1){
            *GetChessErrorStream() << "Argument invalid: " << "\"" << str << "\"" << std::endl;
            exit(-1);
        }
        ret.push_back(str.substr(0, pos));
		SplitArgument(str.substr(pos+1, str.length()-pos-1), ret, c);
        //ret.push_back(str.substr(pos+1, str.length()-pos-1));
    }
}
void usage()
{
	printf("Usage: wchess.exe [/maxpreemptions:<bound>] [/repro]"
		         " [/break:[s][c][d][f]]"
//				 " [-monitors:dll[:dll]*]"
//				 " [/dep:dll[:dll]*]" 
				 " <model.dll[:function]>\n");
	//printf("Usage: wchess.exe [-ls:<sched file>] [-ss:<sched file>]"
	//			 " [-mp:<bound>] [-tr:<trace file>] [-gg:<graph file>]"
	//			 " [-setbreak] [-setbreakonrace] [-setbreakondeadlock] [-fd]"
	//			 " <model.dll[:function]>\n");
}

//typedef void (WINAPI *RUN_TEST_TYPE) (int argc, char** argv);
//#define ARGS 0,0

//typedef void (*RUN_TEST_TYPE) ();
//#define ARGS

typedef LPTHREAD_START_ROUTINE RUN_TEST_TYPE;
typedef int (*CHESS_TEST_STARTUP_TYPE)(int argc, char** argv);
typedef int (*CHESS_TEST_RUN_TYPE)();
typedef void (*CHESS_TEST_SHUTDOWN_TYPE)();


typedef IChessMonitor* (*GET_MONITOR_TYPE)();

struct ProgramData{
	RUN_TEST_TYPE modelStartup;
	RUN_TEST_TYPE modelShutdown;
	RUN_TEST_TYPE runTest;

	//new interface
	CHESS_TEST_STARTUP_TYPE chessTestStartup;
	CHESS_TEST_RUN_TYPE chessTestRun;
	CHESS_TEST_SHUTDOWN_TYPE chessTestShutdown;

	CHESS_ON_ERROR_CALLBACK onErrorCallback;
	GetStateCallback getStateCallback;
	struct{
		int argc;
		char** argv;
	}runTestArgs;

	HMODULE hModule;
	std::vector<std::string> depModules;
	std::vector<std::string> monitorModules;
	int numWarmupRuntests;
	bool useDeprecatedFunctionNames;

	ProgramData(){
		modelStartup = modelShutdown = runTest = NULL;
		chessTestStartup = NULL;
		chessTestRun = NULL;
		chessTestShutdown = NULL;

		hModule = NULL;
		numWarmupRuntests = 0;
		useDeprecatedFunctionNames = false;
	}
};

std::string GetLastErrorString(){
	char errorstr[256];
	DWORD error = GetLastError();
#pragma warning(push)
#pragma warning(disable:25068)
#ifndef UNDER_CE
	FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM, NULL, error, 0, errorstr, 256, NULL);
#else
    WCHAR wErrorStr[256];
    DWORD dwRet;

    errorstr[0] = '\0';
    dwRet = FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, NULL, error, 0, wErrorStr, sizeof(wErrorStr)/sizeof(wErrorStr[0]), NULL);
    if ( dwRet )
    {
        ::WideCharToMultiByte(CP_ACP, 0, wErrorStr, -1, errorstr, sizeof(errorstr)/sizeof(errorstr[0]), NULL, NULL);
    }
#endif
#pragma warning(pop)
	return string(errorstr);
}

#ifdef UNDER_CE
#ifdef GetProcAddress
#undef GetProcAddress
#endif
#define GetProcAddress GetProcAddressA
#endif

RUN_TEST_TYPE LoadEntryPoint(HMODULE handle, const char* fnName, int numArgs){
	RUN_TEST_TYPE ret = (RUN_TEST_TYPE) GetProcAddress(handle, fnName); 

	if(ret == NULL){
		// try if we can find _fnName
		string fnNameMangled = "_"+std::string(fnName);
		ret = (RUN_TEST_TYPE) GetProcAddress(handle, fnNameMangled.c_str()); 
	}

	if(ret == NULL){
		// try if we can find _fnName@(4*numArgs)
		std::stringstream s;
		s << "_" << fnName << "@" << 4*numArgs;
		string fnNameMangled;
		s >> fnNameMangled;
//		string fnNameMangled = "_"+fnName+"@"+(4*numArgs);
		ret = (RUN_TEST_TYPE) GetProcAddress(handle, fnNameMangled.c_str()); 
	}
	return ret;
}

void ParseArguments(int argc, char * argv[], ChessOptions& opts, ProgramData& progData)
{
    bool parsingError = false;
	RUN_TEST_TYPE ret = NULL;

	if(argc == 1)
		parsingError = true;

#ifdef CHESS_INIT_IN_DLL
    for (int i = 0; i < argc; i++) {
#else
    for (int i = 1; i < argc; i++) {
#endif // CHESS_INIT_IN_DLL
        if (argv[i][0] == '-' || argv[i][0] == '/')
        {
			if(argv[i][0] == '/'){
				argv[i][0] = '-';
			}
            vector<string> split_argv;
			std::string argstr = argv[i];
			for(std::string::size_type j=0; j<argstr.length(); j++){
				if(argstr.at(j) == '='){
					argstr.at(j) = ':'; // replace all '=' with ':'
				}
			}
			SplitArgument(argstr, split_argv);
            // We assume that the array split_argv is of length at most 2
            if (split_argv[0] == "-ls" || split_argv[0] == "-repro")
            {
				opts.load_schedule = true;
				opts.max_executions = 1;
                if (split_argv.size() > 1)
                {
					opts.load_schedule_file = new char[split_argv[1].length()+1];
					strcpy_s(opts.load_schedule_file, split_argv[1].length()+1, split_argv[1].c_str());
                }
            }
            else if (split_argv[0] == "-continue")
            {
				opts.load_schedule = true;
				//opts.max_executions = 1;
                if (split_argv.size() > 1)
                {
					opts.load_schedule_file = new char[split_argv[1].length()+1];
					strcpy_s(opts.load_schedule_file, split_argv[1].length()+1, split_argv[1].c_str());
                }
            }
            else if (split_argv[0] == "-p")
            {
                if (split_argv.size() >= 3)
                {
					opts.SetValue(split_argv[1].c_str(), split_argv[2].c_str());
                }
            }
            else if (split_argv[0] == "-recover")
            {
				opts.recover_schedule = true;
                if (split_argv.size() > 1)
                {
					opts.recover_schedule_file = new char[split_argv[1].length()+1];
					strcpy_s(opts.recover_schedule_file, split_argv[1].length()+1, split_argv[1].c_str());
                }
            }
			else if (split_argv[0] == "-showhbexecs")
            {
				opts.show_hbexecs = true;
            }
			else if (split_argv[0] == "-remote")
            {
				opts.use_remote_test_driver = true;
            }
			else if (split_argv[0] == "-noremote")
            {
				opts.use_remote_test_driver = false;
            }
			else if (split_argv[0] == "-md" || split_argv[0] == "-maxdelays")
            {
                int n = atoi(split_argv[1].c_str());
                if (n >= 0)
					opts.delay_bound = n;
                else
                {
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
                }
            }
			else if (split_argv[0] == "-pct") {
				opts.PCT = true;
				opts.num_of_runs = atoi(split_argv[1].c_str());
				opts.bug_depth = atoi(split_argv[2].c_str());
			}
			else if(split_argv[0] == "-vb") {
				opts.var_bound = atoi(split_argv[1].c_str());
			}
            else if (split_argv[0] == "-mp" || split_argv[0] == "-maxpreemptions")
            {
                int n = atoi(split_argv[1].c_str());
                if (n >= 0)
					opts.preemption_bound = n;
                else
                {
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
                }
            }
            else if (split_argv[0] == "-dfs")
            {
				opts.preemption_bound = (1<<30);
            }
            else if (split_argv[0] == "-profile")
            {
				opts.profile = true;
            }
            else if (split_argv[0] == "-gui" || split_argv[0] == "-guitrace")
            {
				opts.trace = true;
				opts.gui = true;
            }
            else if (split_argv[0] == "-trace")
            {
				opts.trace = true; 
				opts.load_schedule = true; //-trace implies -repro
            }
			else if (split_argv[0] == "-fb")
			{
                int n = atoi(split_argv[1].c_str());
                if (n > 0)
					opts.fairness_parameter = n;
                else
                {
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
                }
			}
			else if(split_argv[0] == "-maxstack"){
				int n = atoi(split_argv[1].c_str());
				if(n > 0){
					opts.max_stack_size = n;
				}
				else{
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
				}
			}
			else if(split_argv[0] == "-maxchesstime"){
				int n = atoi(split_argv[1].c_str());
				if(n > 0){
					opts.max_chess_time = n;
				}
				else{
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
				}
			}
			else if(split_argv[0] == "-maxexectime"){
				int n = atoi(split_argv[1].c_str());
				if(n > 0){
					opts.max_exec_time = n;
				}
				else{
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
				}
			}
			else if(split_argv[0] == "-depthbound"){
				int n = atoi(split_argv[1].c_str());
				if(n >= 0){
					opts.depth_bound = n;
				}
				else{
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
				}
			}
			else if(split_argv[0] == "-useexecprinter"){
				opts.use_exec_printer = true;
			}
			else if(split_argv[0] == "-debugoutput"){
				opts.debug_output_flag = true;
			}
			else if(split_argv[0] == "-idfs"){
				opts.do_idfs = true;
				opts.idfs_bound = atoi(split_argv[1].c_str());
			}
			else if(split_argv[0] == "-deprecatedfunctions"){
				progData.useDeprecatedFunctionNames = true;
			}
            else if(split_argv[0] == "-setbreak"){
                DebugBreak();
            }
			else if(split_argv[0] == "-brk" || split_argv[0] == "-break"){
				std::string arg = split_argv[1];
				for(unsigned int j=0; j<arg.length(); j++){
					switch(arg.at(j)){
						case 's' : DebugBreak(); break;
						case 'c' : opts.break_on_context_switch = true; break; 
						case 'p' : opts.break_on_preemptions = true; break; 
						case 'C' : opts.break_after_context_switch = true; break; 
						case 'P' : opts.break_after_preemptions = true; break; 
						case 'd' : opts.break_on_deadlock = true; break; 
						case 't' : opts.break_on_timeout = true; break;
						case 'f' : opts.break_on_task_resume = true; break; 
					}
				}
			}
			else if(split_argv[0] == "-showProgress" || split_argv[0] == "-showprogress" ){
				opts.show_progress = true;
			}
			else if(split_argv[0] == "-showProgressStart"){
				int n = atoi(split_argv[1].c_str());
				if(n > 0){
					opts.show_progress_start = n;
				}
				else{
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
				}
			}
			else if(split_argv[0] == "-infiniteTimeoutBound"){
				int n = atoi(split_argv[1].c_str());
				if(n > 0){
					opts.infinite_timeout_bound = n;
				}
				else{
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
				}
			}
			else if(split_argv[0] == "-seed"){
				opts.random_seed = atoi(split_argv[1].c_str());
			}
			else if(split_argv[0] == "-random"){
				opts.do_random = true;
			}
			else if(split_argv[0] == "-sleepsets"){
				opts.do_sleep_sets = true;
			}
			else if(split_argv[0] == "-nosleepsets"){
				opts.do_sleep_sets = false;
			}
			else if(split_argv[0] == "-handlenondeterminism"){
				opts.handle_nondeterminism = true;
			}
			else if(split_argv[0] == "-nohandlenondeterminism"){
				opts.handle_nondeterminism = false;
			}
			else if(split_argv[0] == "-exec" || split_argv[0] == "-maxexecs"){
                int n = atoi(split_argv[1].c_str());
                if (n >= 0)
					opts.max_executions = n;
                else
                {
                    *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << ":" << split_argv[1] << "\"" << std::endl;
                    parsingError = true;
                }
			}
			else if(split_argv[0] == "-dep" || split_argv[0] == "-includeassembly" || split_argv[0] == "-ia"){
				if(split_argv.size() <= 1){
					*GetChessErrorStream() << "Need at least one argument for " << split_argv[0] << std::endl;
					parsingError = true;
				}
				else{
					for(size_t j=1; j<split_argv.size(); j++){
						progData.depModules.push_back(split_argv[j]);
					}
				}
			}
			else if(split_argv[0] == "-monitors"){
				if(split_argv.size() <= 1){
					*GetChessErrorStream() << "Need at least one argument for -monitors" << std::endl;
					parsingError = true;
				}
				else{
					for(size_t j=1; j<split_argv.size(); j++){
						progData.monitorModules.push_back(split_argv[j]);
					}
				}
			}
			else if(split_argv[0] == "-unifynonthreadhandles"){
				opts.unify_nonthreadhandles = true;
			}
			else if(split_argv[0] == "-setbreakondeadlock" || split_argv[0] == "-bd"){
				opts.break_on_deadlock = true;
			}
			else if(split_argv[0] == "-setbreakontimeout" || split_argv[0] == "-bt"){
				opts.break_on_timeout = true;
			}
			else if(split_argv[0] == "-setbreakonpreemptions" || split_argv[0] == "-bp"){
				opts.break_on_preemptions = true;
			}
			else if(split_argv[0] == "-setbreakoncontextswitch" || split_argv[0] == "-bc"){
				opts.break_on_context_switch = true;
			}
			else if(split_argv[0] == "-quiescence"){
				if(split_argv.size() != 2){
					*GetChessErrorStream() << "Need one argument for -quiescence" << std::endl;
					parsingError = true;
				}
				int n = atoi(split_argv[1].c_str());
				opts.quiescence = (n != 0);
			}
			else if(split_argv[0] == "-notime"){
				opts.notime = true;
			}
			else if(split_argv[0] == "-nopopups"){
				opts.nopopups = true;
			}
			else if(split_argv[0] == "-printschedonerror"){
				opts.print_sched_on_error = true;
			}
			else if(split_argv[0] == "-rununit"){
				UnitTests();
			}
			else if(split_argv[0] == "-displayfreq"){
				if(split_argv.size() != 2){
					*GetChessErrorStream() << "Need at one argument for -displayfreq" << std::endl;
					parsingError = true;
				}
				else{
					opts.SetValue("StatsMonitor::initDisplayFrequency", split_argv[1].c_str());
				}
			}
			else if(split_argv[0] == "-nofinalstats"){
				opts.SetValue("StatsMonitor::displayStatsAtEnd", "0");
			}
			else if(split_argv[0] == "-nopreemptionsingui"){
				opts.SetValue("ConcurrencyExplorer::showPremptionsInGui", "0");
			}
			else if(split_argv[0] == "-warmupruns"){
				if(split_argv.size() != 2){
					*GetChessErrorStream() << "Need at one argument for -warmupruns" << std::endl;
					parsingError = true;
				}
				else{
					progData.numWarmupRuntests = atoi(split_argv[1].c_str());
				}
			}
			else if(split_argv[0] == "-drpct" || split_argv[0] == "-DRPCT" || split_argv[0] == "-derandomizedpct") {
				opts.DeRandomizedPCT = true;
				opts.bug_depth = atoi(split_argv[1].c_str());
			}
			else if(split_argv[0] == "-PCT") {
				opts.PCT = true;
				opts.num_of_runs = atoi(split_argv[1].c_str());
				opts.bug_depth = atoi(split_argv[2].c_str());
				if(split_argv.size() == 4) opts.pct_seed = atoi(split_argv[3].c_str());
				else opts.pct_seed = -1;
			}
			else if(split_argv[0] == "-var_bounding" || split_argv[0] == "-vb"){
				opts.variable_bounding = true;
				opts.num_var_bound = atoi(split_argv[1].c_str());
			}
            else {
                *GetChessErrorStream() << "Argument not recognized" << "\"" << split_argv[0] << "\"" << std::endl;
                parsingError = true;
            }
        }
        else
        {
#ifndef CHESS_INIT_IN_DLL
            if (ret != NULL)
            {
                parsingError = true;
            }
            else
            {
	            vector<string> split_argv;
				SplitArgument(argv[i], split_argv, '!');
#pragma warning(push)
#pragma warning(disable: 25068)
                HMODULE handle = LoadLibraryA(split_argv[0].c_str());
#pragma warning(pop)
                if (handle == NULL)
                {
					*GetChessErrorStream() << "Load Library of " << argv[i] << " failed : " << GetLastErrorString() << std::endl;
                    exit(-1);
                }

				if(!progData.useDeprecatedFunctionNames){
					string fnName = "ChessTestRun";
					if(split_argv.size() > 1){
						fnName = split_argv[1];
					}

					progData.chessTestRun = (CHESS_TEST_RUN_TYPE) LoadEntryPoint(handle, fnName.c_str(), 0);
					if (progData.chessTestRun == NULL)
					{
						*GetChessErrorStream() << "GetProcAddress failed: couldn't find symbol '" << fnName << "' in " << split_argv[0] << std::endl;
						exit(-1);
					}
					progData.chessTestStartup = (CHESS_TEST_STARTUP_TYPE)LoadEntryPoint(handle, "ChessTestStartup", 2); 
					progData.chessTestShutdown = (CHESS_TEST_SHUTDOWN_TYPE)LoadEntryPoint(handle, "ChessTestShutdown", 0);
				}
				else{
					string fnName = "RunTest";
					if(split_argv.size() > 1){
						fnName = split_argv[1];
					}

					progData.runTest = LoadEntryPoint(handle, fnName.c_str(), 0);
					if (progData.runTest == NULL)
					{
						*GetChessErrorStream() << "GetProcAddress failed (couldn't find symbol '" << fnName << "' in the .DLL)" << std::endl;
						exit(-1);
					}
					progData.modelStartup = LoadEntryPoint(handle, "TestStartup", 2); 
					progData.modelShutdown = LoadEntryPoint(handle, "TestShutdown", 0);
				}


				progData.onErrorCallback = (CHESS_ON_ERROR_CALLBACK)GetProcAddress(handle, "_ChessOnErrorCallback@8");
				progData.hModule = handle;
				progData.getStateCallback = (GetStateCallback)LoadEntryPoint(handle, "GetState", 1);
				// argv[i] is the dll name
				//  argv[i] ... argv[argc-1] are arguments to runTest
				progData.runTestArgs.argc = argc - i;
				progData.runTestArgs.argv = &argv[i];
				break;
			}
#endif //CHESS_INIT_IN_DLL
		}
    }

	if (parsingError){
         usage();
        exit(-1);
	}
}

#include <Win32SyncManager.h>

#ifdef UNDER_CE

void WriteStringToDebuggerOutput(const char* fmt, ...)
{
    va_list argptr;

    char szBuf[512];
    WCHAR wszBuf[512];

    szBuf[0] = '\0';
    wszBuf[0] = L'\0';
    va_start(argptr, fmt);
    ::StringCchVPrintfA(szBuf, sizeof(szBuf)/sizeof(szBuf[0]), fmt, argptr);
    ::MultiByteToWideChar(CP_ACP, 0, szBuf, -1, wszBuf, sizeof(wszBuf)/sizeof(wszBuf[0]));
    ::OutputDebugString(wszBuf);
    va_end(argptr);
}

#endif

int main(int argc, char* argv[])
{
	ChessOptions opts;
	ProgramData progData;
	ParseArguments(argc, argv, opts, progData);
	//for(int iter = 0; iter<200; iter++){
	Win32SyncManager* sm = new Win32SyncManager();

	if(!sm->RegisterTestModule(progData.hModule)){
		*GetChessErrorStream()<< "Cannot register test module" << std::endl;
		exit(-1);
	}
	if(progData.onErrorCallback){
		ChessQueueOnErrorCallback(progData.onErrorCallback);
	}
	if(progData.getStateCallback){
		ChessRegisterGetStateCallback(progData.getStateCallback);
	}

	Chess::SetOptions(opts);
	for(size_t i=0; i<progData.depModules.size(); i++){

		//HMODULE hModule = GetModuleHandleA(progData.depModules[i].c_str());
#pragma warning(push)
#pragma warning(disable: 25068)
		HMODULE hModule = LoadLibraryA(progData.depModules[i].c_str());
#pragma warning(pop)
		if(hModule == NULL){
			*GetChessErrorStream() << "Cannot find dependent module " << progData.depModules[i] << std::endl; 
			continue;
		}
		if(!sm->RegisterTestModule(hModule)){
			*GetChessErrorStream()<< "Cannot register dependent module " << progData.depModules[i] << std::endl;
			exit(-1);
		}
	}

	if(Chess::GetOptions().quiescence){
		ChessInit(opts, sm);
	}

	for(size_t i=0; i<progData.monitorModules.size(); i++){
#pragma warning(push)
#pragma warning(disable: 25068)
		HMODULE hModule = LoadLibraryA(progData.monitorModules[i].c_str());
#pragma warning(pop)
		if(hModule == NULL){
			*GetChessErrorStream() << "Cannot find monitor module " << progData.monitorModules[i] << std::endl; 
			continue;
		}
		GET_MONITOR_TYPE getMonitor = (GET_MONITOR_TYPE)GetProcAddress(hModule, "GetMonitor");
		if(getMonitor == NULL){
			*GetChessErrorStream() << "Cannot find GetMonitor function in " << progData.monitorModules[i] << std::endl; 
			continue;
		}
		IChessMonitor* monitor = getMonitor();
		ChessRegisterMonitor(monitor);
	}

	if(progData.useDeprecatedFunctionNames && progData.modelStartup){
		DWORD ret = (*progData.modelStartup)(&progData.runTestArgs);
		if(ret < 0)
			return CHESS_EXIT_TEST_FAILURE;
	}
	if(!progData.useDeprecatedFunctionNames && progData.chessTestStartup){
		int ret = (*progData.chessTestStartup)(progData.runTestArgs.argc, progData.runTestArgs.argv);
		if(ret < 0)
			return CHESS_EXIT_TEST_FAILURE;
	}

	for(int i=0; i<progData.numWarmupRuntests; i++){
		if(progData.useDeprecatedFunctionNames){
			long ret = (long) progData.runTest(0);
			if(ret < 0){
				*GetChessErrorStream()<< "Warmup RunTests returned with a failure (value = " << ret << ")" << std::endl;
#ifdef UNDER_CE
                WriteStringToDebuggerOutput("Warmup RunTests returned with a failure (value = %ld)\n", ret);
#endif
				return CHESS_EXIT_TEST_FAILURE;
			}
		}
		else{
			int ret = progData.chessTestRun();
			if(ret < 0){
				*GetChessErrorStream()<< "Warmup ChessTestRun returned with a failure (value = " << ret << ")" << std::endl;
#ifdef UNDER_CE
                WriteStringToDebuggerOutput("Warmup RunTests returned with a failure (value = %ld)\n", ret);
#endif
				return CHESS_EXIT_TEST_FAILURE;
			}
		}
	}

	if(!Chess::GetOptions().quiescence){
		ChessInit(opts, sm);
	}

	int exitCode = 0;
	bool moreToTest = true;
	while(moreToTest){
		ChessStartTest();
		if(progData.useDeprecatedFunctionNames){
			long ret = (long) progData.runTest(0);
			if(ret < 0){
				*GetChessErrorStream()<< "RunTest returned with a failure (value = " << ret << ")" << std::endl;
#ifdef UNDER_CE
                WriteStringToDebuggerOutput("RunTest returned with a failure (value = %ld)\n", ret);
#endif
				exitCode = CHESS_EXIT_TEST_FAILURE;
				break;
			}
		}
		else{
			int ret = progData.chessTestRun();
			if(ret < 0){
				*GetChessErrorStream()<< "ChessTestRun returned with a failure (value = " << ret << ")" << std::endl;
#ifdef UNDER_CE
                WriteStringToDebuggerOutput("ChessTestRun returned with a failure (value = %ld)\n", ret);
#endif
				exitCode = CHESS_EXIT_TEST_FAILURE;
				break;
			}
		}
		moreToTest = ChessEndTest();

	}

	//IChessStats* st = Chess::GetStats();
	//*GetChessErrorStream()<< "TotalCS: " << st->GetTotalContextSwitches() << " MaxCS: " << st->GetMaxContextSwitches() << std::endl;

	if(!exitCode){
		if (progData.useDeprecatedFunctionNames && progData.modelShutdown != NULL)
			(*progData.modelShutdown)(0);
		if (!progData.useDeprecatedFunctionNames && progData.chessTestShutdown)
			(*progData.chessTestShutdown)();
	}

	ChessDone();
	sm->ShutDown();
	FreeLibrary(GetModuleHandle("Win32Chess.dll"));
	//}
#ifdef UNDER_CE
    WriteStringToDebuggerOutput("Chess: Exiting with code = %d\n", exitCode ? exitCode : Chess::GetExitCode());
#endif
	return exitCode ? exitCode : Chess::GetExitCode();
}

