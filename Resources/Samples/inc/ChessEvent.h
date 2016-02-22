/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "SyncVar.h"
#include "EventAttribute.h"
#include <ostream>
struct EventId {

	Task tid;
	int nr;

	EventId(Task t, int n) : tid(t), nr(n) { }

	EventId() : tid(0), nr(0) { } // default constructor needed for some STL situations

	// lexicographic order
	bool operator==(const EventId& o) const { return tid==o.tid && nr == o.nr; }
	bool operator!=(const EventId& o) const{ return tid!=o.tid || nr != o.nr;  }
	bool operator<(const EventId& o) const { return (tid < o.tid || (tid == o.tid && nr < o.nr)); }
	bool operator>=(const EventId& o) const{ return !operator<(o); }
	bool operator>(const EventId& o) const { return (tid > o.tid || (tid == o.tid && nr > o.nr)); }
	bool operator<=(const EventId& o) const{ return !operator>(o); }

    // print
	std::ostream& operator<<(std::ostream& o) const { o << tid << '.' << nr; return o; }
};

inline std::ostream& operator<<(std::ostream& o, const EventId& id){
	return id.operator<<(o);
}
