/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"
#include "..\Chess\SyncVar.h"
//#include "Error.h"

class Timestamp  {

public:
	// default constructor
	Timestamp() : mSharedVec(0), mMod(false), mModVal(0), mModPos(0) {};

	// destruct, copy, assign (needed for correct ref counting)
	~Timestamp();
	Timestamp(const Timestamp &inCopiedTimestamp);
	Timestamp &operator=(const Timestamp &inAssignedTimestamp);

	// timestamp operations
	void tick(size_t inThread, size_t nr);
	void merge(const Timestamp &inClock);
	bool lessthanorequal(const Timestamp &inClock) const;
	size_t get(size_t inThread) const;
	void set(size_t inThread, size_t inValue);
	size_t getMaxPos() const;

	// print
	void print(std::ostream &outStream);

private:
	struct SharedVec {
		std::vector<size_t> mElements;
		int mRefcount;
		SharedVec(size_t size);
		SharedVec(const SharedVec& inVec);
		~SharedVec();
	};
	SharedVec *mSharedVec; // underlying vector; null indicates zero-vec
    bool mMod;             // true if modified
	size_t mModVal;        // modified element value
	size_t mModPos;        // position of modified element

	void applyMod();  // flatten mod into vec
};

class TSOTimestamp  {

public:
	// default constructor
	TSOTimestamp() : mSharedVec(0), mMod(false), mModValL(0), mModValS(0), mModPos(0) {};

	// destruct, copy, assign (needed for correct ref counting)
	~TSOTimestamp();
	TSOTimestamp(const TSOTimestamp &inCopiedTimestamp);
	TSOTimestamp &operator=(const TSOTimestamp &inAssignedTimestamp);

	// timestamp operations
	void tick_load(size_t inThread, size_t nr);
	void tick_store(size_t inThread, size_t nr);
	void merge(const TSOTimestamp &inClock);
	bool lessthanorequal(const TSOTimestamp &inClock) const;
	size_t get_load(size_t inThread) const;
	size_t get_store(size_t inThread) const;
	void set(size_t inThread, size_t inLoadValue, size_t inStoreValue);
	size_t getMaxPos() const;

	// print
	void print(std::ostream &outStream);

private:
	struct SharedVec {
		std::vector<size_t> mElements;
		int mRefcount;
		SharedVec(size_t size);
		SharedVec(const SharedVec& inVec);
		~SharedVec();
	};
	SharedVec *mSharedVec; // underlying vector; null indicates zero-vec
    bool mMod;             // true if modified
	size_t mModValL;           // modified element value (L)
	size_t mModValS;           // modified element value (S)
	size_t mModPos;        // position of modified element

	void applyMod();  // flatten mod into vec
};


class TSOTimestampVector {

public:
	TSOTimestamp &get(size_t inThread);
	void assign_all(const TSOTimestamp &inClock);
	void assign_all_except_p(const TSOTimestamp &inTSOTimestamp, size_t inThread);

private:
	TSOTimestamp mDefaultTimestamp;
    bool mHasException;
    size_t mExceptionThread;
	TSOTimestamp mExceptionTimestamp;
};



