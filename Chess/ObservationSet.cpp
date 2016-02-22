/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ObservationSet.h"
#include "ObservationMonitor.h"
#include "Observation.h"
#include "Chess.h"
#include <sstream>

using namespace std;

void ObservationSet::Load(const char *filename)
{
	std::ifstream f(filename);
	if(!f.good()) 
		obsmon->ReportFileError("open", string(filename));
	string obs;
	bool in_callhistory = false;
	vector<Interleaving> interleavings;
	while(!f.eof()){ // This parsing is not clean... I am parsing my own file format, not general XML structure
		std::string line;
		getline(f, line);
		if (line.substr(0,18) == "    <interleaving>")
		{
			in_callhistory = false;
			interleavings.push_back(Interleaving());
			interleaving_parse(interleavings.back(), line, 18);
		}
		else if (line == "  </observation>")
		{
			in_callhistory = false;
			interleavings.swap(loadedspec[obs]);
			obs.clear();
		}
		if (in_callhistory)
			obs.append(line).append("\n");
		if (line == "  <observation>")
		{
			in_callhistory = true;
			continue;
		}
	}
	f.close();
}


void ObservationSet::interleaving_parse(Interleaving &itl, const std::string &str, unsigned pos)
{
	int val = 0;
	char c;
	do
	{ 
		c = str[pos++];
		if (c >= '0' && c <= '9')
			val = (10*val) + (c -'0');
		else 
		{
			if (c == '.')
                interleaving_add(itl, val, Observation::stall);
			else if (c == '[')
				interleaving_add(itl, val, Observation::call);
			else if (val != 0)
				interleaving_add(itl, val, Observation::ret);
			val = 0;
		}
	} while (c != '<' && pos < str.size());
}

void ObservationSet::interleaving_add(Interleaving &itl, int tid, ObservationSet::e_type evt)
{
	itl.seq.push_back(pair<int,e_type>(tid, evt));
    if (evt == Observation::call)
	{
		itl.serial = itl.serial && !itl.opactive;
		itl.opactive = true;
	} 
	else if (evt == Observation::ret)
	{
		itl.serial = itl.serial && itl.opactive;
        itl.opactive = false;
	}
}

bool ObservationSet::interleaving_compatible_with_ret_to_call_edges(Observation &obs, Interleaving &itl)
{
	int numops = obs.ops.size();
	vector<bool> done(numops, false);
	for (vector<pair<int, e_type> >::iterator it = itl.seq.begin(); it != itl.seq.end(); it++)
	{
		if (it->second == Observation::call)
		{
			for (int d = 1; d <= numops; d++)
				if (done[d-1] && obs.ret_to_call_edge(it->first, d))
					return false;
		}
		else if (it->second == Observation::ret)
		{
			done[it->first-1] = true;
		}
	}
	return true;
}



void ObservationSet::Check(Observation &obs, bool some_ops_suppressed)
{
	map<string,vector<Interleaving> >::iterator it = loadedspec.find(obs.get_key());
	if (it != loadedspec.end())
	{
		if (obsmon->obsmode == ObservationMonitor::om_SC)
			return;
		else if (obsmon->obsmode == ObservationMonitor::om_SC_s)
		{
			for(vector<Interleaving>::iterator it2 = it->second.begin(); it2 != it->second.end(); it2++)
				if (it2->serial)
					return;
		}
		else if (obsmon->obsmode == ObservationMonitor::om_lin)
		{
			for(vector<Interleaving>::iterator it2 = it->second.begin(); it2 != it->second.end(); it2++)
				if (interleaving_compatible_with_ret_to_call_edges(obs, *it2))
					return;
		}  
		else if (obsmon->obsmode == ObservationMonitor::om_lin_s)
		{
			for(vector<Interleaving>::iterator it2 = it->second.begin(); it2 != it->second.end(); it2++)
				if (it2->serial && interleaving_compatible_with_ret_to_call_edges(obs, *it2))
					return;
		}  
	}
	obsmon->ReportInvalidObservation(obs, some_ops_suppressed);
}

void ObservationSet::Add(Observation &obs)
{
	if (!obs.excluded())
	{
		ostringstream sstr;
		obs.serialize_interleaving(sstr);
		minedspec[obs.get_key()].insert(sstr.str());
	}
}

void ObservationSet::Save(const char *filename)
{
	// start the observation file
	ofstream stream;
	stream.open(filename);
	if (! stream.good())
		obsmon->ReportFileError("write to", string(filename));
	stream << "<observationset>" << endl;
	for (map<string,set<string> >::iterator it1 = minedspec.begin(); it1 != minedspec.end(); it1++)
	{
       stream << "  <observation>" << endl;
       stream << it1->first;
	   for (set<string>::iterator it2 = it1->second.begin(); it2 != it1->second.end(); it2++)
		   stream << *it2 << endl;
       stream << "  </observation>" << endl;
	}
	stream << "</observationset>" << endl;
	stream.close();
}

	