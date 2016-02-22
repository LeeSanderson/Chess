/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessApi.h"

class StringOptions;

class CHESS_API ChessOptions{
public:

	// IMPORTANT:  keep this list in sync with initialization (defaults) in ChessOptions.cpp

	//start
	bool PCT;
	bool DeRandomizedPCT;
	int num_of_runs;
	bool variable_bounding;
	int num_var_bound;
	int bug_depth;
	int var_bound;
	int pct_seed;
	//end (sandeep)

	int delay_bound;
	int preemption_bound;
	bool recover_schedule;
	bool load_schedule;

    char* output_prefix;          // prefix for all output files
	char* schedule_file;          // output schedule file (incl. prefix)
	char* load_schedule_file;     // input schedule file (incl. prefix)
	char* recover_schedule_file;  // recover schedule file (incl. prefix)
	char* xml_commandline;        // full command line in xml format
    char *observation_mode;       // specifies observation mode to use
    char *enumerate_observations; // file to write enumerated observation set to (incl. prefix)
    char *check_observations;     // file to read observations set from(incl. prefix)

	bool break_on_assert;
	bool break_on_deadlock;
	bool break_on_timeout;
	bool break_on_preemptions;
	bool break_on_context_switch;
	bool break_on_task_resume;
	bool break_on_race;
	bool break_after_preemptions;
	bool break_after_context_switch;

	bool die_on_nonidempotence;
	bool tolerate_deadlock;

	int max_stack_size;
	int max_exec_time;
	int max_chess_time;
	int max_executions;
	bool use_exec_printer;
	bool handle_nondeterminism;

	bool use_remote_test_driver;
	int fairness_parameter;

	bool do_idfs;
	int depth_bound;
	int idfs_bound;

	bool do_random;
	int random_seed;

	bool do_sleep_sets;
	// best first search/DPOR options (Katie)
	bool do_dpor;
	bool best_first;
	bool fair_por;
	bool bounded;
	int prioritized_var;
	const char* best_first_priority;
	const char* bounded_priority;

	bool notime;
	bool print_sched_on_error;
	bool nopopups;

	bool quiescence;

	bool sober;
	bool sober_dataracesonly;
    int sober_targetrace;
    bool trace;
	bool logging;
    bool gui;

	bool debug_output_flag;
	bool show_hbexecs;
	bool show_nlb;
	bool show_stacksplit;

	bool wrap_random;
	bool profile;

	bool show_progress;
	int show_progress_start;

	bool record_preempt_methods;

	bool unify_nonthreadhandles;

	int infinite_timeout_bound;  // treat timeouts(in ms) > infinite_timeout_bound as infinite timeouts

	ChessOptions();
	~ChessOptions();

	void Update(ChessOptions& opt);

	static void SaveOptionsToFile(const char* file) {} /* made static for OACR, even though it is nonsense */
	void SetOptionsFromFile(const char* file);

	void SetValue(const char* opt, const char* val);

	bool GetValue(const char* opt, int& val) const; // Currently works only for slow options
	bool GetValue(const char* opt, bool& val) const; // Currently works only for slow options

private:
	// copy constructor is private, use Update instead
	ChessOptions(ChessOptions& o){}

	StringOptions* strOpts;
};
