/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessStl.h"
#include "AtomicityMonitor.h"
#include "CacheRaceMonitor.h"
#include "Chess.h"
#include "ChessImpl.h"
#include "ResultsPrinter.h"
#include "Observation.h"

using namespace std;

// compile-time constants
#define TRACE_CALLS 0
#define PRINT_SETS 0

// the algorithm is encoded in this class
class AtomicityMonitor::ConflictGraph
{
public:
	ConflictGraph(AtomicityMonitor *am) { amonitor = am; }
	void clear() { nodes.clear(); }
	void begin(EventId id);
	void end(EventId id);
	void access(EventId id, CacheRaceMonitor::Location*, bool isWrite);
    
private:
	AtomicityMonitor *amonitor;

	// witness classes used for tracking which events participate in cycles
	struct Witness 
    {
		Witness() { refcount = 0; }
		Witness *inc() { refcount++; return this; };
		bool dec() { return --refcount == 0; }
        int refcount;
		virtual void getEvents(set<EventId> &acc) {}
		set<EventId> getEvents() { set<EventId> s; getEvents(s); return s; }
	};
	struct WitnessPtr
	{
        Witness *w;
		WitnessPtr() : w(0) {}
		WitnessPtr(Witness *wit) : w(wit->inc()) {}
		bool bound() { return w != 0; }
		void clear() { if (w && w->dec()) delete w; w = 0; }
		~WitnessPtr() { if (w && w->dec()) delete w; }
		WitnessPtr(const WitnessPtr &p) { w = p.w; if (w) w->inc(); }
		WitnessPtr &operator=(const WitnessPtr &p) { if (w && w->dec()) delete w; w = p.w; if (w) w->inc(); return *this; }
		Witness *operator->() { return w; }
		bool operator==(const WitnessPtr &p2) { return w == p2.w; }
	};
	struct WitnessVertex : Witness
	{
		EventId evt;
		WitnessVertex(EventId evnt) : evt(evnt) {}
		virtual void getEvents(set<EventId> &acc) { acc.insert(evt); }

	};
	struct WitnessEdge : Witness
	{
		WitnessPtr w1;
		WitnessPtr w2;
		WitnessEdge(WitnessPtr wit1, WitnessPtr wit2) : w1(wit1), w2(wit2) {}
		virtual void getEvents(set<EventId> &acc) { w1->getEvents(acc); w2->getEvents(acc);}
	};
	struct ActionInfo
	{
		ActionInfo() : isWrite(false), witness() { }
		ActionInfo(bool iw, EventId evtid) : isWrite(iw), witness(new WitnessVertex(evtid)) {}
		ActionInfo(bool iw, WitnessPtr w1, WitnessPtr w2) : isWrite(iw), witness(new WitnessEdge(w1,w2)) {}
        bool isWrite;
        WitnessPtr witness;
	};

	typedef map<CacheRaceMonitor::Location*,ActionInfo> actionset;

	struct Node
	{
        bool active;
		vector<WitnessPtr> pred;
		vector<WitnessPtr> succ;
		actionset setS;
		actionset setC;
        
		Node() : active(false) { }
		void resize(unsigned n) { pred.resize(n); succ.resize(n); }
	};
    vector<Node> nodes;

	Node &getNode(unsigned tid)
	{
		assert(tid > 0);  // 0 is not used
		if (tid >= nodes.size())
		{
			// enlarge matrix
			nodes.resize(tid + 1);
			for (unsigned i = 0; i <= tid; i++)
				nodes[i].resize(tid + 1);
		}
		return nodes[tid];
	}

	// adds edge (if not already there). If cycle results, returns witness. Otherwise, returns unbound WitnessPtr
	WitnessPtr add_edge(unsigned from, unsigned to, WitnessPtr &wfrom, WitnessPtr &wto) 
	{ 
		Node &fromnode(getNode(from));
		Node &tonode(getNode(to));
		assert(fromnode.succ[to] == tonode.pred[from]);
		if (! fromnode.succ[to].bound())
		{
			// add edge
            WitnessPtr wit(new WitnessEdge(wfrom, wto));
			fromnode.succ[to] = wit;
			tonode.pred[from] = wit;

			// check for cycle
			if (from == to)
				return wit;

			// add transitive edges (not part of original alg, but we want to catch cycles ASAP
			else
			{
				bool createdcycle = false;
				int size = fromnode.pred.size();
				for(int i = 1; i < size; i++)
				{
					if (fromnode.pred[i].bound())
					{  
						WitnessPtr p(add_edge(i, to, fromnode.pred[i], wit));
						if (p.bound()) return p;
					}
					if (tonode.succ[i].bound())
					{  
						WitnessPtr p(add_edge(from, i, wit, tonode.succ[i]));
						if (p.bound()) return p;
					}
				}
			}
		}
		return WitnessPtr();
	}
	void clear_edge(unsigned from, unsigned to) 
	{ 
		Node &fromnode(getNode(from));
		Node &tonode(getNode(to));
		assert(fromnode.succ[to] == tonode.pred[from]);
		fromnode.succ[to].clear();
		tonode.pred[from].clear();
	}
	void insert(actionset &target, CacheRaceMonitor::Location* loc, ActionInfo a)
	{
		map<CacheRaceMonitor::Location*,ActionInfo>::iterator itf = target.find(loc);
		if (itf == target.end())
			target[loc] = a;
		else if (a.isWrite && ! itf->second.isWrite)
			itf->second = a;
	}
	void insert_all(actionset &target, actionset &source, WitnessPtr &through)
	{
		for(map<CacheRaceMonitor::Location*,ActionInfo>::iterator it = source.begin(); it != source.end(); it++)
			insert(target, it->first, ActionInfo(it->second.isWrite, it->second.witness, through));
	}
};
// methods for conflict graph

void AtomicityMonitor::ConflictGraph::begin(EventId id)
{
     getNode(id.tid).active = true;
}


void AtomicityMonitor::ConflictGraph::end(EventId id)
{
    Node &node(getNode(id.tid));
	int size = node.pred.size();

	if (PRINT_SETS)
	{
		*GetChessOutputStream() << "- pred:";
		for (int i = 1; i < size; i++)
			if (node.pred[i].bound())
				*GetChessOutputStream() << " " << i;
		*GetChessOutputStream() << endl << "- succ:";
		for (int i = 1; i < size; i++)
			if (node.succ[i].bound())
				*GetChessOutputStream() << " " << i;
		*GetChessOutputStream() << endl << "- set S:";
		for(map<CacheRaceMonitor::Location*,ActionInfo>::iterator it = node.setS.begin(); it != node.setS.end(); it++)
			*GetChessOutputStream() << " " << it->first << (it->second.isWrite ? "W" : "R");
		*GetChessOutputStream() << endl << "- set C:";
		for(map<CacheRaceMonitor::Location*,ActionInfo>::iterator it = node.setC.begin(); it != node.setC.end(); it++)
			*GetChessOutputStream() << " " << it->first << (it->second.isWrite ? "W" : "R");
		*GetChessOutputStream() << endl;
	}

	for (int pr = 1; pr < size; pr++)
	{
		if (node.pred[pr].bound())
		{
			Node &pred(getNode(pr));
			// connect to all successors of node
			for(int sc = 1; sc < size; sc++)
				if (node.succ[sc].bound())
				{ 
					WitnessPtr p(add_edge(pr,sc,node.pred[pr],node.succ[sc]));
					if (p.bound())
						amonitor->reportAtomicityViolation(id, p->getEvents());
				}
			// augment set C
			insert_all(pred.setC, node.setS, node.pred[pr]);
			insert_all(pred.setC, node.setC, node.pred[pr]);
			// disconnect
			clear_edge(pr, id.tid);
		}
	}
	// disconnect all successors
	for(int sc = 1; sc < size; sc++)
		clear_edge(id.tid, sc);
	// this node is done
	node.setS.clear();
	node.setC.clear();
   	node.active = false;     
}

void AtomicityMonitor::ConflictGraph::access(EventId id, CacheRaceMonitor::Location* loc, bool isWrite)
{
    Node &node(getNode(id.tid));
	int size = node.pred.size();

    ActionInfo action(isWrite,id);

    // add to set S
	insert(node.setS, loc, action);

	// check for predecessors
	for (int vk = 1; vk < size; vk++)
	{
		Node &pred(getNode(vk));
		if (pred.active)
		{
		   map<CacheRaceMonitor::Location*,ActionInfo>::iterator it;
           // check S set for conflicts (if vk not node)
		   if (vk != id.tid)
		   {
			   it = pred.setS.find(loc);
			   if (it != pred.setS.end() && (isWrite || it->second.isWrite))
			   {
				   WitnessPtr p(add_edge(vk, id.tid, it->second.witness, action.witness));
				   if (p.bound())
					   amonitor->reportAtomicityViolation(id, p->getEvents());
			   }
		   }
		   // check set C for conflicts
           it = pred.setC.find(loc);
		   if (it != pred.setC.end() && (isWrite || it->second.isWrite))
		   {
			   WitnessPtr p(add_edge(vk, id.tid, it->second.witness, action.witness));
			   if (p.bound())
				   amonitor->reportAtomicityViolation(id, p->getEvents());
		   }
		}
	}

}

// atomicity monitor methods

AtomicityMonitor::AtomicityMonitor(CacheRaceMonitor *cacheracemonitor, Observation *curobservation) { 
	crmonitor = cacheracemonitor;
	cg = new ConflictGraph(this);
	atomicityviolation_found = false;
    curobs = curobservation;
}


void AtomicityMonitor::OnShutdown()
{
	delete cg;
	cg = 0;
}

void AtomicityMonitor::clear() 
{
	cg->clear();
}


void AtomicityMonitor::record_load(EventId id, SyncVar var, CacheRaceMonitor::Location *inLocation, int inPcId, int nr) {
	if (TRACE_CALLS)
		*GetChessOutputStream() << "Read  " << id << "," << inLocation << ",  " << var << std::endl;
	cg->access(id, inLocation, false);
}

void AtomicityMonitor::record_store(EventId id, SyncVar var, CacheRaceMonitor::Location *inLocation, int inPcId, int nr) {
	if (TRACE_CALLS)
		*GetChessOutputStream() << "Write " << id << "," << inLocation << ",  " << var << std::endl;
	cg->access(id, inLocation, true);
}

void AtomicityMonitor::record_interlocked(EventId id, SyncVar var, CacheRaceMonitor::Location *inLocation, int nr) {
	if (TRACE_CALLS)
		*GetChessOutputStream() << "Write " << id << "," << inLocation << ",  " << var  << std::endl;
	cg->access(id, inLocation, true);
}

// record a transaction begin
void AtomicityMonitor::opcall(EventId id, void *object, const char *name)
{
	if (TRACE_CALLS)
		*GetChessOutputStream() << "Begin " << id << "," << object << "," << name << std::endl;
    cg->begin(id);
}

// record a transaction end
void AtomicityMonitor::opreturn(EventId id)
{
	if (TRACE_CALLS)
		*GetChessOutputStream() << "End   " << id << std::endl;
    cg->end(id);
}

void AtomicityMonitor::OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId) {

    // do not track accesses outside transactions (we assume isolation)
    if (! curobs->is_in_operation(id.tid))
       return;

	if(size > 4){
		while(size > 0){
			OnDataVarAccess(id, loc, size > 4 ? 4 : size, isWrite, pcId);
			loc = ((char*)loc)+4;
			size -= 4;
		}
		return;
	}
	loc = (void*)( ((int)loc) & (~0x3));

	CacheRaceMonitor::DLocation *location = crmonitor->get_normal_location(loc);
	if (isWrite)
		record_store(id, 0, location, pcId, id.nr);
	else
		record_load(id, 0, location, pcId, id.nr);
}

void AtomicityMonitor::OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid) {

    // do not track accesses outside transactions (we assume isolation)
    if (! curobs->is_in_operation(id.tid))
       return;

	CacheRaceMonitor::SLocation *location = crmonitor->get_syncvar_location(var);
	if (SVOP::IsWrite(op) && SVOP::IsRead(op))
		record_interlocked(id, var, location, id.nr);
	else if (SVOP::IsWrite(op))
		record_store(id, var, location, 0, id.nr);
	else if (SVOP::IsRead(op))
		record_load(id, var, location, 0, id.nr);
	else {
		// no-op on syncvar accesses that are neither write nor read
	}
}

void AtomicityMonitor::OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid) {
	for (int i = 0; i < n; ++i)
		OnSyncVarAccess(id, tid, var[i], op, sid);
}





void AtomicityMonitor::reportAtomicityViolation(EventId id, set<EventId> events) {

	string location = crmonitor->getProgramLocation();

	ostringstream sstr;
	sstr << "Found atomicity violation at " << location << endl;
	for(set<EventId>::iterator it = events.begin(); it != events.end(); it++)
	{
		Chess::SetEventAttribute(*it, DISPLAY_BOXED, "Crimson");
	}

	errorstring = sstr.str();
}

void AtomicityMonitor::OnExecutionEnd(IChessExecution* exec) {

	if (errorstring.size() > 0)
		ChessImpl::ChessAssertion(errorstring.c_str(), CHESS_EXIT_ATOMICITY_VIOLATION);

}
