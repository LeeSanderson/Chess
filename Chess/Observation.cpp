/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Observation.h"
#include "ObservationMonitor.h"
#include "Chess.h"
#include "ObservationSet.h"

#include <map>
#include <sstream>


void Observation::clear()
{
	threads.clear();
	ops.clear();
	interleaving.clear();
	strings.clear();
	pointers.clear();
	nops.clear();
	key.clear();
	rettocall.clear();
	pointermap.clear();
	get_pointer_index(0);  // null pointer has index 0, always
	serial = true;
	deadlocked = false;
	active_operations = 0;
	last_call_op = 0;
	maxdepth = 0;
}

Observation::Threadinfo &Observation::getThread(unsigned tid)
{
	while (tid >= threads.size())
		threads.push_back(Threadinfo());
	return threads[tid];
}

int Observation::cur_depth(unsigned int tid)
{
   	Threadinfo &thread(getThread(tid));
	return thread.operations_active.size();
}
int Observation::num_ops(unsigned int tid)
{
   	Threadinfo &thread(getThread(tid));
	return thread.num_ops;
}

int Observation::get_pointer_index(void *ptr)
{
	map<void*,int>::iterator it = pointermap.find(ptr);
	if (it != pointermap.end())
		return it->second;
	else
	{
		pointers.push_back(pair<void*,int>(ptr, -1));
		return pointermap[ptr] = pointers.size()-1;
	}
}

int Observation::get_string_index(const char *str)
{
	strings.push_back(string(str));  // we're not currently compressing
	return strings.size()-1;
}

bool Observation::is_in_operation(size_t tid)
{
	Threadinfo &thread(getThread(tid));
    return thread.operations_active.size() > 0;
}
bool Observation::enter_timeout(size_t tid)
{
	Threadinfo &thread(getThread(tid));
	if (! thread.intimeout)
	{
		thread.intimeout = true;
		return true;
	}
	return false;
}
bool Observation::exit_timeout(size_t tid)
{
	Threadinfo &thread(getThread(tid));
    if (thread.intimeout)
	{
		thread.intimeout = false;
		return true;
	}
	return false;
}

bool Observation::add_call(size_t tid, void *ptr, const char *opname, bool iscallback)
{
	check_seriality_before_call(tid);
	active_operations++;

	Threadinfo &thread(getThread(tid));
	int depth = thread.operations_active.size();
	// if this is a callback-only thread, push fake op
	if (depth == 0 && iscallback)
	{
		thread.operations_active.push_back(-1);
		depth++;
	}
	maxdepth = (depth > maxdepth) ? depth : maxdepth;
	if (iscallback != (depth % 2 == 1))
		return false;
	int name = get_string_index(opname);
	int obj = get_pointer_index(ptr);
	int id = ops.size() + 1;
	ops.push_back(Opinfo(tid,thread.sequence.size(), depth, name, obj));
	thread.sequence.push_back(Entry(call, id, 0, depth));
	thread.operations_active.push_back(id);
	thread.num_ops++;
	interleaving.push_back(pair<int,enum e_type>(id,call));

	last_call_op = id;
	return true;
}


void Observation::check_seriality_before_call(size_t tid)
{
	if (active_operations > 0)
	{
		if (serial)
		{
			// sanity check: when doing coarse enumeration, must have detected stall at this point
			assert(!obsmon->specmining || (obsmon->obsmode == ObservationMonitor::om_all || interleaving.back().second == stall));

			// generate observation corresponding to blocking serial ex.
			if (obsmon->obsmode == ObservationMonitor::om_serial)
			{
				normalize();
				obsmon->curset->Add(*this);
				unnormalize();
			}
		}
		serial = false;
	}
}

bool Observation::add_return(size_t tid, bool iscallback)
{
	active_operations--;

	Threadinfo &thread(getThread(tid));
	int depth = thread.operations_active.size();
	if (depth == 0 || (iscallback != (depth % 2 == 0)))
		return false;
	int op = thread.operations_active.back();
	thread.sequence.push_back(Entry(ret, op, 0, depth - 1));
	ops[op-1].endindex = thread.sequence.size();
	thread.operations_active.pop_back();
	interleaving.push_back(pair<int, enum e_type>(op, ret));
	return true;
}

void Observation::deadlock(size_t tid)
{
	stalled();
	deadlocked = true;
}

void Observation::stalled()
{
	if (interleaving.size() == 0 || interleaving.back().second != stall)
	{
		interleaving.push_back(pair<int, enum e_type>(0, stall));

	}
}

size_t Observation::get_next_active_thread(size_t tid)
{
	tid++;
	while (tid < threads.size() && (threads[tid].operations_active.size() % 2 == 0))
		tid++;
	return (tid >= threads.size()) ? 0 : tid;
}

void Observation::add_integer(size_t tid, const char *label, long long value)
{
	Threadinfo &thread(getThread(tid));
	int depth = thread.operations_active.size();
	thread.sequence.push_back(Entry(obs_int, get_string_index(label), value, depth));
}

void Observation::add_string(size_t tid, const char *label, const char *value)
{
	Threadinfo &thread(getThread(tid));
	int depth = thread.operations_active.size();
	thread.sequence.push_back(Entry(obs_str, get_string_index(label), get_string_index(value), depth));
}

void Observation::add_pointer(size_t tid, const char *label, void *value)
{
	Threadinfo &thread(getThread(tid));
	int depth = thread.operations_active.size();
	thread.sequence.push_back(Entry(obs_ptr, get_string_index(label), get_pointer_index(value), depth));
}

int Observation::get_pointer_seqnum(int *cur_index, int index)
{
	if (pointers[index].second == -1)
		pointers[index].second = (*cur_index)++;
	return pointers[index].second;
}    

bool Observation::skip_label(int label){
	string labelstr = strings[label];
	string obs_skip = "_observation_skip";
	return labelstr.compare(0, obs_skip.length(), obs_skip) == 0;
}

static void escape(ostream &stream, const string &str)
{
   for (unsigned pos = 0; pos < str.length(); pos++)
   {
      switch(str[pos])
      {
	     case '&': stream << "&amp;"; break;
	     case '<': stream << "&lt;"; break;
	     case '>': stream << "&gt;"; break;
	     case '"': stream << "&quot;"; break;
	     case '\'': stream << "&apos;"; break;
		 default: stream << str[pos];
      }
   }
}

bool Observation::normalize() 
{
	// determine canonical operation ids
	for (int depth = 0; depth <= maxdepth; depth++)
	{
		for (unsigned thr = 1; thr < threads.size(); thr++)
		{
			vector<Entry> &sequence(threads[thr].sequence);
			for (vector<Entry>::iterator it = sequence.begin(); it != sequence.end(); it++)
				if (it->type == call && it->depth == depth)
				{
					Opinfo &info(ops[it->label-1]);
					if (info.nid == 0 && ! info.suppressed)
					{
						info.nid = nops.size() + 1;
						nops.push_back(it->label);
					}
				}
		}
	}

	// format key string
	get_key();

	return true;  // may do more validation at some point, and return false if not well-formed
}

void Observation::unnormalize()
{
	for(vector<Opinfo>::iterator it = ops.begin(); it != ops.end(); it++)
		it->nid = 0;
	nops.clear();
	key.clear();
	rettocall.clear();
}

bool Observation::suppress_next(int count) 
{
	bool found = false;
	int countseen = 0;

	for(unsigned i = 0; i < ops.size(); i++)
	{
		if (ops[i].endindex == 0)  // incomplete op - blocked
		{
			if (countseen++ == count)
			{
				ops[i].suppressed = false;
				found = true;
			}
			else ops[i].suppressed = true;
		}
	}

	// on last call to suppress_next, undo suppression
	if (!found)
		unsuppress();

	return found;   
}

void Observation::unsuppress() 
{
	for(unsigned i = 0; i < ops.size(); i++)
	{
		ops[i].suppressed = false;
	}
}


bool Observation::ret_to_call_edge(int op1, int op2)
{
	if (! rettocall.size())
		calc_ret_to_call_edges();
	int numops = ops.size();
	return rettocall[(op1-1)*numops + (op2-1)];
}

void Observation::calc_ret_to_call_edges()
{
	int numops = ops.size();
	vector<bool> done(numops,false);
	rettocall.resize(numops*numops,false);
	for(vector<pair<int, enum Observation::e_type> >::iterator it = interleaving.begin(); it != interleaving.end(); it++)
	{
		if (it->second == Observation::call && !ops[it->first-1].suppressed)
		{
			for (int d = 1; d <= numops; d++)
				if (done[d-1])
					rettocall[(d-1)*numops + ops[it->first-1].nid - 1] = true;
		}
		else if (it->second == Observation::ret)
		{
			done[ops[it->first-1].nid - 1] = true;
		}
	}
}


const std::string &Observation::get_key() 
{
  if (key.size() == 0)
  {
	  ostringstream sstr;
	  serialize_callhistory(sstr);
	  key = sstr.str();
  }
  return key;
}

/*    
             <observation>
               <thread tid="1"> 1 2(6 7(10) 8) 3 </thread>
               <thread tid="2"> 4 5 </thread>
               <thread tid="3"> (9) </thread>
               <op id="1" name="prepare" obj="1"> arg="x" ret="true"</op>
               <op id="2" name="parallelfor" obj="1"tasks="3"</op>
               <op id="3" name="finish" obj="1"> arg="x" ret="true"</op>
               <op id="4" name="pop" obj="1"> arg="x" ret="true"</op>
               <op id="5" name="pop" obj="1"> arg="x" ret="true"</op>
               <cb id="6" name="task1" obj="1"></cb>
               <cb id="7" name="task2" obj="1"></cb>
               <cb id="8" name="task3" obj="1"></cb>
               <cb id="9" name="anothercallback" obj="1"></cb>
               <op id="10" name="isdone" obj="1"> ret="false"</op>
               <interleaving> 1[ ]1 4[ 2[ 6[ ]2 3[ ]3 ]4 </interleaving>
               <interleaving> 1[ ]1 2[ 4[ 6[ ]2 3[ ]4 ]3 </interleaving>
             </observation>
             <observation>
*/

void Observation::serialize_callhistory(ostream &stream, bool skip_labels) 
{
	// shortcut - already serialized earlier
	if (key.size() > 0)
	{
		stream << key;
		return;
	}

	int ptrseqnum = 0;
	get_pointer_seqnum(&ptrseqnum, 0);  // null pointer gets sequence number 0

	for (unsigned thr = 1; thr < threads.size(); thr++)
	{
		vector<Entry> &sequence(threads[thr].sequence);
		if ((sequence.size() == 0)
			|| (threads[thr].num_ops == 1 
                && sequence[0].type == call 
                && ops[sequence[0].label-1].suppressed))
			continue;
		stream << "    <thread>";
		int depth = 0;
		for (vector<Entry>::iterator it = sequence.begin(); it != sequence.end(); it++)
		{
			if (it->type == call && it->depth > depth)
			{
				stream << "(";
				depth++;
			}
			if (it->depth < depth)
			{
				stream << " )";
				depth--;
			}
			if (it->type == call)
			{ 
				if (ops[it->label-1].endindex != 0)
					stream << " " << ops[it->label-1].nid;
				else if ( ! ops[it->label-1].suppressed)
					stream << " " << ops[it->label-1].nid << "B";
			}
		}
		while (depth > 0)
		{
			stream << " )";
			depth--;
		}
		stream << "</thread>" << endl;
	}

	for (unsigned nop = 0; nop < nops.size(); nop++)
	{
		int nid = nop + 1;
		Opinfo &info(ops[nops[nop]-1]);
		stream << "    <" << (info.depth % 2 == 0 ? "op" : "cb");
		stream << " id=\"" << nid << "\" name=\"";
		escape(stream, strings[info.name]);
		stream << "\"";
		if (info.obj != 0)
			stream << " obj=\"" << get_pointer_seqnum(&ptrseqnum, info.obj) << "\"";
		stream << ">";
		vector<Entry> &sequence(threads[info.tid].sequence);
		for (int pos = info.startindex; pos < info.endindex; pos++)
		{
			Entry &entry(sequence[pos]);
			if (entry.depth == info.depth + 1)
				switch (entry.type)
			    {
				case call:
				case ret:
					break;
				case obs_int:
					if(skip_labels && skip_label(entry.label))
						break;
					stream << " ";
					escape(stream, strings[entry.label]);
					stream << "=\"" << entry.val << "\"";
					break;
				case obs_str:
					if(skip_labels && skip_label(entry.label))
						break;
					stream << " ";
					escape(stream, strings[entry.label]);
					stream << "=\"";
					escape(stream, strings[(unsigned) entry.val]);
					stream << "\"";
					break;
				case obs_ptr:
					if(skip_labels && skip_label(entry.label))
						break;
					stream << " ";
					escape(stream, strings[entry.label]);
					stream << "=\"" << get_pointer_seqnum(&ptrseqnum, (int) entry.val) << "\"";
					break;
			   }
		}
		stream << "</" << (info.depth % 2 == 0 ? "op" : "cb") << ">" << endl;
	}
}

bool Observation::excluded()
{
	return (obsmon->obsmode == ObservationMonitor::om_serial && !serial);
}

void Observation::serialize_interleaving(ostream &stream) 
{
	stream << "    <interleaving>";
	bool last_was_stall = true;
	for (vector<pair<int,enum e_type> >::iterator it = interleaving.begin(); it != interleaving.end(); it++)
	{
		if (it->second == call && (!ops[it->first-1].suppressed))
		{
			stream << " " << ops[it->first-1].nid << "[";
			last_was_stall = false;
		}
		else if (it->second == ret && (!ops[it->first-1].suppressed))
		{
			stream << " ]" << ops[it->first-1].nid;
			last_was_stall = false;
		}
		else if (it->second == stall && ! last_was_stall) // suppress multiple stalls, and initial stall
		{
			stream <<  " " << ".";
			last_was_stall = true;
		}
 	//	else
	//		stream <<  " " << ".";
	}
	stream << "</interleaving>";
}
