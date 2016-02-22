/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessStl.h"
#include "ChessOptions.h"
#include "PriorityFunction.h"
#include "windows.h"

class StringOptions{
public:
	StringOptions(){
		InitFastOptions();
	}
	bool GetValue(const ChessOptions* opts, const char* opt, int& val){
		stdext::hash_map<std::string, std::string>::iterator i = optionMap.find(opt);
		if(i == optionMap.end()) return false;
		std::istringstream s(i->second);
		if(s >> val){
			return true;
		}
		return false;
	}

	bool GetValue(const ChessOptions* opts, const char* opt, bool& val){
		stdext::hash_map<std::string, std::string>::iterator i = optionMap.find(opt);
		if(i == optionMap.end()) return false;
		std::istringstream s(i->second);
		if(s >> val){
			return true;
		}
		return false;
	}

	void SetValue(ChessOptions* opts, const std::string& opt, const std::string& val){
		if(SetFastOption(opts, opt, val)){
			return;
		}
		optionMap[opt] = val;
	}

	void SetOptionsFromFile(ChessOptions* opts, const char* file){
		std::ifstream f(file);
		if(!f.good()) return;
		while(!f.eof()){
			std::string line;
			getline(f, line);
			if(line.length() <= 0) continue;
			if(line.at(0) == '#') continue; // skip comments
			if(line.find('=') == std::string::npos) continue; // need a '='
			std::istringstream linestr(line);
			std::string opt;
			std::string val;
			char eq;
			linestr >> std::skipws >> opt >> std::skipws >> eq >> std::skipws >> val;
			if(linestr.fail() || eq != '=')
				continue;
			SetValue(opts, opt, val);
		}
	}

	void Update(StringOptions* opts){
		stdext::hash_map<std::string, std::string>::iterator i;
		for(i=opts->optionMap.begin(); i !=opts->optionMap.end(); i++){
			optionMap[i->first] = i->second;
		}
	}

private:
	stdext::hash_map<std::string, std::string> optionMap;

	stdext::hash_map<std::string, int> fastIntOptionOffset;
	stdext::hash_map<std::string, int> fastBoolOptionOffset;

	void InitFastOptions(){
		ChessOptions fake_object;
#define CALC_OFFSET(field) (int)((size_t) &fake_object.field - (size_t) &fake_object)
		fastIntOptionOffset["bug_depth"] = CALC_OFFSET(bug_depth);
		fastIntOptionOffset["num_of_runs"] = CALC_OFFSET(num_of_runs);
		fastIntOptionOffset["num_var_bound"] = CALC_OFFSET(num_var_bound);
		fastIntOptionOffset["random_seed"] = CALC_OFFSET(random_seed);
		fastIntOptionOffset["delay_bound"] = CALC_OFFSET(delay_bound);
		fastIntOptionOffset["preemption_bound"] = CALC_OFFSET(preemption_bound);
		fastIntOptionOffset["max_preemptions"] = CALC_OFFSET(preemption_bound);
		fastIntOptionOffset["max_stack_size"] = CALC_OFFSET(max_stack_size);
		fastIntOptionOffset["max_exec_time"] = CALC_OFFSET(max_exec_time);
		fastIntOptionOffset["max_chess_time"] = CALC_OFFSET(max_chess_time);
		fastIntOptionOffset["max_executions"] = CALC_OFFSET(max_executions);
		fastIntOptionOffset["fairness_parameter"] = CALC_OFFSET(fairness_parameter);
		fastIntOptionOffset["depth_bound"] = CALC_OFFSET(depth_bound);
		fastIntOptionOffset["idfs_bound"] = CALC_OFFSET(idfs_bound);
		fastIntOptionOffset["sober_targetrace"] = CALC_OFFSET(idfs_bound);


		fastBoolOptionOffset["PCT"] = CALC_OFFSET(PCT);
		fastBoolOptionOffset["DeRandomizedPCT"] = CALC_OFFSET(DeRandomizedPCT);
		fastBoolOptionOffset["variable_bounding"] = CALC_OFFSET(variable_bounding);
		fastBoolOptionOffset["load_schedule"] = CALC_OFFSET(load_schedule);
		fastBoolOptionOffset["ls"] = CALC_OFFSET(load_schedule);
		fastBoolOptionOffset["break_on_assert"] = CALC_OFFSET(break_on_assert);
		fastBoolOptionOffset["break_on_deadlock"] = CALC_OFFSET(break_on_deadlock);
		fastBoolOptionOffset["break_on_timeout"] = CALC_OFFSET(break_on_timeout);
		fastBoolOptionOffset["break_on_preemptions"] = CALC_OFFSET(break_on_preemptions);
		fastBoolOptionOffset["break_on_context_switch"] = CALC_OFFSET(break_on_context_switch);
		fastBoolOptionOffset["break_on_race"] = CALC_OFFSET(break_on_race);
		fastBoolOptionOffset["break_on_task_resume"] = CALC_OFFSET(break_on_task_resume);
		fastBoolOptionOffset["handle_nondeterminism"] = CALC_OFFSET(handle_nondeterminism);
		fastBoolOptionOffset["do_sleep_sets"] = CALC_OFFSET(do_sleep_sets);
		// best-first search/DPOR additions (BFS)
		fastBoolOptionOffset["do_dpor"] = CALC_OFFSET(do_dpor);
		fastBoolOptionOffset["best_first"] = CALC_OFFSET(best_first);
		fastBoolOptionOffset["fair_por"] = CALC_OFFSET(fair_por);
		fastBoolOptionOffset["bounded"] = CALC_OFFSET(bounded);
		fastBoolOptionOffset["prioritized_var"] = CALC_OFFSET(prioritized_var);

		fastBoolOptionOffset["do_random"] = CALC_OFFSET(do_random);
		fastBoolOptionOffset["do_idfs"] = CALC_OFFSET(do_idfs);
		fastBoolOptionOffset["notime"] = CALC_OFFSET(notime);
		fastBoolOptionOffset["print_sched_on_error"] = CALC_OFFSET(print_sched_on_error);
		fastBoolOptionOffset["nopopups"] = CALC_OFFSET(nopopups);
		fastBoolOptionOffset["quiescence"] = CALC_OFFSET(quiescence);
		fastBoolOptionOffset["sober"] = CALC_OFFSET(sober);
		fastBoolOptionOffset["sober_dataracesonly"] = CALC_OFFSET(sober_dataracesonly);
		fastBoolOptionOffset["sober_targetrace"] = CALC_OFFSET(sober_targetrace);
		fastBoolOptionOffset["trace"] = CALC_OFFSET(trace);
		fastBoolOptionOffset["logging"] = CALC_OFFSET(logging);
		fastBoolOptionOffset["gui"] = CALC_OFFSET(gui);
		fastBoolOptionOffset["debug_output_flag"] = CALC_OFFSET(debug_output_flag);
		fastBoolOptionOffset["show_hbexecs"] = CALC_OFFSET(show_hbexecs);
		fastBoolOptionOffset["show_nlb"] = CALC_OFFSET(show_nlb);
		fastBoolOptionOffset["show_stacksplit"] = CALC_OFFSET(show_stacksplit);
		fastBoolOptionOffset["profile"] = CALC_OFFSET(profile);
	}

	bool SetFastOption(ChessOptions* opts, const std::string& opt, const std::string& val){
		stdext::hash_map<std::string, int>::iterator i;
		i=fastIntOptionOffset.find(opt);
		if(i != fastIntOptionOffset.end()){
			int intVal;
			std::istringstream s(val);
			if(s >> intVal){
				*(int*)(((char*)opts)+(i->second)) = intVal;
				return true;
			}
		}
		i=fastBoolOptionOffset.find(opt);
		if(i != fastBoolOptionOffset.end()){
			bool boolVal;
			if(val.compare("true") == 0){
				*(bool*)(((char*)opts)+(i->second)) = true;
				return true;
			}
			if(val.compare("false") == 0){
				*(bool*)(((char*)opts)+(i->second)) = true;
				return true;
			}
			std::istringstream s(val);
			if(s >> boolVal){
				*(bool*)(((char*)opts)+(i->second)) = boolVal;
				return true;
			}
		}
		return false;
	}
};



// set the default options
ChessOptions::ChessOptions(){

	//default options -- keep these in sync (same order) as in ChessOptions.h

	PCT = false;
	DeRandomizedPCT = false;
	num_of_runs = -1;
	variable_bounding = false;
	num_var_bound = -1;
	bug_depth = 1;
	pct_seed = -1;

	var_bound = -1;
	delay_bound = -1;
	preemption_bound = 2;
	recover_schedule = false;
	load_schedule = false;

#ifndef UNDER_CE
    output_prefix = "";
	schedule_file = "sched";
	load_schedule_file = "sched";
	recover_schedule_file = "sched";
#else
	output_prefix = "\\release\\";
    schedule_file = "\\release\\sched";
    load_schedule_file = "\\release\\sched";
    recover_schedule_file = "\\release\\sched";
#endif
    observation_mode = "";
    enumerate_observations = "";
    check_observations = "";
    xml_commandline = "";

#ifndef UNDER_CE
	break_on_assert = false;
	break_on_deadlock = false;
	break_on_timeout = false;
#else
	break_on_assert = true;
	break_on_deadlock = true;
	break_on_timeout = true;
#endif
	break_on_preemptions = false;
	break_on_context_switch = false;
	break_on_task_resume = false;
	break_on_race = false;
	break_after_preemptions = false;
	break_after_context_switch = false;

	die_on_nonidempotence = true;
	tolerate_deadlock = false;

	max_stack_size = 20000;
	max_exec_time = 10;
	max_chess_time = 0; //inf
	max_executions = 0;
	use_exec_printer = false;
	handle_nondeterminism = true;
    
 	use_remote_test_driver = false;
	fairness_parameter = 1;

	do_idfs = false;
	depth_bound = 0;
	idfs_bound = 10;

	do_random = false;
	random_seed = 0;

	do_sleep_sets = true;
	// best-first search options (BFS)
	do_dpor = false;
	best_first = false;
	fair_por = false;
	bounded = true;
	prioritized_var = 0;
	best_first_priority = PriorityFunction::WDPOR;
	bounded_priority = PriorityFunction::FEWER_PREEMPTIONS;

	notime = false;
	print_sched_on_error = false;
	nopopups = false;

	quiescence = true;

	sober = false;
	sober_dataracesonly = false;
	sober_targetrace = 0;
	trace = false;
	logging = false;
	gui = false;

	debug_output_flag = false;
	show_stacksplit = false;
	show_hbexecs = false;
	show_nlb = false;

	wrap_random = false;
	profile = false;

	show_progress = false;
	show_progress_start = 10000; 

	strOpts = 0;

	record_preempt_methods = false;

	infinite_timeout_bound = 0;

	unify_nonthreadhandles = false;
}

ChessOptions::~ChessOptions(){
	if(strOpts){
		delete strOpts;
		strOpts = 0;
	}
}

void ChessOptions::SetOptionsFromFile(const char* file){
	if(strOpts == 0)
		strOpts = new StringOptions();
	strOpts->SetOptionsFromFile(this, file);
}

bool ChessOptions::GetValue(const char* opt, int& val) const{
	if(strOpts == 0)
		((ChessOptions*)this)->strOpts = new StringOptions();
	return strOpts->GetValue(this, opt, val);
}

bool ChessOptions::GetValue(const char* opt, bool& val) const{
	if(strOpts == 0)
		((ChessOptions*)this)->strOpts = new StringOptions();
	return strOpts->GetValue(this, opt, val);
}

void ChessOptions::SetValue(const char* opt, const char* val){
	if(strOpts == 0)
		strOpts = new StringOptions();
	std::string sopt(opt);
	std::string sval(val);
	strOpts->SetValue(this, sopt, sval);
}

void ChessOptions::Update(ChessOptions& opt){
	StringOptions* old = strOpts;
	*this = opt; // overwrite all fields including strOpts
	strOpts = old;
	if(opt.strOpts)
		strOpts->Update(opt.strOpts);
}