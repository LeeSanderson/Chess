/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

#include <map>

class ObservationMonitor;

using namespace std;

class Observation {

friend class ObservationSet;

public:
	Observation(ObservationMonitor *omon) : obsmon(omon) { clear(); }
	void clear();

	bool add_call(size_t tid, void *ptr, const char *opname, bool iscallback);
	bool add_return(size_t tid, bool iscallback);
	//void add_stall(size_t tid);
	void add_integer(size_t tid, const char *label, long long value);
	void add_string(size_t tid, const char *label, const char *value);
	void add_pointer(size_t tid, const char *label, void *value);

	int get_pointer_index(void *ptr);
	int get_string_index(const char *str);
	int get_pointer_seqnum(int *cur_index, int index);

    bool is_in_operation(size_t tid);
	bool is_deadlocked() { return deadlocked; }
	bool has_more_than_one_blocked() { return active_operations > 1; }
	size_t get_next_active_thread(size_t tid);

	void deadlock(size_t tid);
	void stalled();

	bool enter_timeout(size_t tid);
    bool exit_timeout(size_t tid);

	bool normalize();
	void unnormalize();
    const string &get_key();
	void serialize_callhistory(ostream &stream, bool skip_labels=true);
	void serialize_interleaving(ostream &stream);
	bool excluded();
	bool suppress_next(int count);
	void unsuppress();

	bool ret_to_call_edge(int op1, int op2);

	int cur_depth(unsigned tid);
	int num_ops(unsigned tid);

private:
	ObservationMonitor *obsmon;

	enum e_type { call, ret, stall, obs_int, obs_str, obs_ptr };

	struct Entry
	{
		enum e_type type;
		int label;
		long long val;
		int depth;

		Entry(enum e_type t, long l, long long v, int d) { type = t; label = l; val = v; depth = d; }
	};

    struct Threadinfo
	{
        vector<Entry> sequence;
        vector<int> operations_active;
		int num_ops;
        bool intimeout;

		Threadinfo() { num_ops = 0; intimeout = 0;}
	};


    struct Opinfo
	{
		unsigned tid;
		int startindex;
		int endindex;
		int depth;
		int name;
		int obj;
		int nid;
		bool suppressed;
		Opinfo(unsigned t, int s, int d, int n, int o) : 
                tid(t), startindex(s), endindex(0), depth(d), name(n), obj(o), nid(0), suppressed(false){}
	};

    vector<Threadinfo> threads;
	vector<Opinfo> ops;
	vector<pair<int, enum e_type> > interleaving;

	Threadinfo &getThread(unsigned tid);

	void check_seriality_before_call(unsigned tid);

	// formatted output
	string key;

    // schedule info
	bool serial;
	bool deadlocked;
	int active_operations;
	int last_call_op;
	int maxdepth;

	// linearization order
    vector<bool> rettocall;
	void calc_ret_to_call_edges();

    // indexed information
	vector<string> strings;                  // (string)
	vector<pair<void *, int> > pointers;    // (value, normalizedindex)

	bool skip_label(int label);

    // normalization maps
	vector<int> nops;                // list of operations (in normalized order)
	map<void *, int> pointermap;     // map pointer to normalized pointerindex

};


