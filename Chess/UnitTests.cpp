/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessApi.h"
#include "Chess.h"
#include "RepVector.h"
#include "PriorityGraph.h"
#include "BitVector.h"
#include <hash_map>
#include <iostream>

inline bool check_fn(bool val, const char* str){
	if(!val){
		*GetChessOutputStream() << str << " failed" << std::endl;
		Chess::GetSyncManager()->DebugBreak();
	}
	return val;
}

#define check(a) do{ if(!check_fn(a, #a)){return false;} } while(false)


bool RepVectorTests(){
	RepVector<int> test1;
	check(test1.Lookup(10) == 0);
	
	test1.Insert(5, 1);
	check(test1.Lookup(4) == 0);
	check(test1.Lookup(5) == 1);
	check(test1.Lookup(10) == 1);

	test1.Insert(10, 0);
	check(test1.Lookup(10) == 0);

	test1.Prune(7);
	check(test1.Lookup(10) == 1);
	check(test1.Lookup(5) == 1);
	check(test1.Lookup(4) == 0);

	test1.Prune(5);
	check(test1.Lookup(10) == 1);
	check(test1.Lookup(5) == 1);
	check(test1.Lookup(4) == 0);

	test1.Prune(4);
	check(test1.Lookup(10) == 0);
	check(test1.Lookup(5) == 0);
	check(test1.Lookup(4) == 0);

	test1.Prune(0);
	check(test1.Lookup(10) == 0);
	check(test1.Lookup(5) == 0);
	check(test1.Lookup(4) == 0);

	RepVector<int> test2(1);
	check(test2.Lookup(10) == 1);


	RepVector<stdext::hash_map<int,int> > test3;

	stdext::hash_map<int,int>& first = test3.Insert(1);
	first[0] = 1;
	first[1] = 2;

	check((test3.Lookup(5).find(1))->second == 2);

	stdext::hash_map<int,int>& second = test3.Insert(4);
	second[0] = 4;
	second[1] = 5;

	check((test3.Lookup(5).find(1))->second == 5);
	check((test3.Lookup(3).find(1))->second == 2);

	return true;
}


bool PriorityGraphTests(){
	PriorityGraph graph;
	BitVector e1;
	e1.insert(2);
	e1.insert(3);
	graph.AddEdges(1, e1);

	BitVector e2;
	e2.insert(1);
	e2.insert(3);
	graph.AddEdges(2, e2);

	check(graph.HasOutgoingEdges(1));
	check(graph.HasOutgoingEdges(2));
	check(!graph.HasOutgoingEdges(3));

	graph.DelIncomingEdges(3);

	check(graph.HasOutgoingEdges(1));
	check(graph.HasOutgoingEdges(2));
	check(!graph.HasOutgoingEdges(3));

	graph.DelIncomingEdges(1);

	check(graph.HasOutgoingEdges(1));
	check(!graph.HasOutgoingEdges(2));
	check(!graph.HasOutgoingEdges(3));
	return true;
}

bool TestBit(BitVector& b, size_t index){
	b.Set(index, 1);
	check(b.Get(index));
	b.Set(index, 0);
	check(!b.Get(index));
	return true;
}

bool BitTests(){
	BitVector b;
	check(b.IsEmpty());
	check(TestBit(b, 0));
	b.Set(0,1);
	b.Set(15,1);
	check(!b.IsEmpty());
	check(TestBit(b, 1));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 2));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 3));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 25));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 29));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 30));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 31));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 32));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 33));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 34));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 50));
	check(b.Get(0) && b.Get(15));
	check(TestBit(b, 100));
	check(b.Get(0) && b.Get(15));
	b.Set(0,0);
	b.Set(15,0);
	check(b.IsEmpty());

	b.insert(0);
	b.insert(5);
	b.insert(10);

	check(b.FindIndexLargerThan(0) == 5);
	check(b.FindIndexLargerThan(5) == 10);
	check(b.FindIndexLargerThan(10) == 0);

	b.insert(30);

	check(b.FindIndexLargerThan(0) == 5);
	check(b.FindIndexLargerThan(5) == 10);
	check(b.FindIndexLargerThan(10) == 30);
	check(b.FindIndexLargerThan(30) == 0);

	b.insert(31);

	check(b.FindIndexLargerThan(0) == 5);
	check(b.FindIndexLargerThan(5) == 10);
	check(b.FindIndexLargerThan(10) == 30);
	check(b.FindIndexLargerThan(30) == 31);	
	check(b.FindIndexLargerThan(31) == 0);

	b.erase(31);
	check(b.FindIndexLargerThan(0) == 5);
	check(b.FindIndexLargerThan(5) == 10);
	check(b.FindIndexLargerThan(10) == 30);
	check(b.FindIndexLargerThan(30) == 0);

	return true;
}

bool CHESS_API UnitTests(){
	check(RepVectorTests());
	check(PriorityGraphTests());
	check(BitTests());
	return true;
}

//void main(){
//	UnitTests();
//}