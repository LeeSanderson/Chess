/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"
#include "Observation.h"

#include <string>
#include <map>
#include <vector>

class ObservationMonitor;

class ObservationSet {

public:
	ObservationSet(ObservationMonitor *omon) : obsmon(omon) { }

	void Load(const char *filename);
    void Save(const char *filename);

	void Add(Observation &observation);
	void Check(Observation &observation, bool some_ops_suppressed);

private:
	ObservationMonitor *obsmon;
	typedef enum Observation::e_type e_type;

	struct Interleaving 
	{
		Interleaving() : opactive(false), serial(true), seq() { }
		std::vector<pair<int, e_type> > seq;
		bool opactive;
		bool serial;
	};

	void interleaving_parse(Interleaving &interleaving, const std::string &str, unsigned pos);
	void interleaving_add(Interleaving &interleaving, int tid, e_type evt);
    bool interleaving_compatible_with_ret_to_call_edges(Observation &obs, Interleaving &itl);

	std::map<std::string,std::set<std::string> > minedspec;        // used for specmining
	std::map<std::string,std::vector<Interleaving> > loadedspec; // used for refinement checking    

};
