/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/


#include "VClock.h"


// destructor
Timestamp::~Timestamp() {
	if (mSharedVec) {
		if (--mSharedVec->mRefcount == 0)
			delete mSharedVec;
		mSharedVec = 0;
	}
}

// copy constructor
Timestamp::Timestamp(const Timestamp &inTimestamp) :  
mSharedVec(inTimestamp.mSharedVec), mMod(inTimestamp.mMod), mModVal(inTimestamp.mModVal), mModPos(inTimestamp.mModPos) {
	if (mSharedVec)
		mSharedVec->mRefcount++;
}

// assignment operator
Timestamp &Timestamp::operator=(const Timestamp &inTimestamp) { 
	mMod = inTimestamp.mMod;
	mModVal = inTimestamp.mModVal;
	mModPos = inTimestamp.mModPos;
	if (mSharedVec) {
		if (--mSharedVec->mRefcount == 0)
			delete mSharedVec;
	}
	mSharedVec = inTimestamp.mSharedVec;
	if (mSharedVec)
		mSharedVec->mRefcount++;
   return *this;
}

// get max pos with nonzero content
size_t Timestamp::getMaxPos() const {
	size_t maxpos = (mSharedVec ? mSharedVec->mElements.size()-1 : 0);
	if (mMod && mModPos > maxpos)
		maxpos = mModPos;
	return maxpos;
}

// get element
size_t Timestamp::get(size_t inThread) const {
	if (mMod && inThread == mModPos)
		return mModVal;
	if (! mSharedVec || mSharedVec->mElements.size() <= inThread)
		return 0;
	return mSharedVec->mElements[inThread];
}

// create shared vector
Timestamp::SharedVec::SharedVec(size_t size) : mElements(size, 0), mRefcount(1) {} 

// copy shared vector
Timestamp::SharedVec::SharedVec(const Timestamp::SharedVec &inVec) : mElements(inVec.mElements), mRefcount(1) { }
							 
// delete shared vector
Timestamp::SharedVec::~SharedVec() { assert(!mRefcount); }

// merge mod into vector 
void Timestamp::applyMod() {
	assert(mMod);
	if (!mSharedVec)
		// create vector
		mSharedVec = new SharedVec(mModPos+1);
	else {
		// copy if shared
		if (mSharedVec->mRefcount > 1) {
			mSharedVec->mRefcount--;
			mSharedVec = new SharedVec(*mSharedVec);
		}
		// stretch if too short
		while (mModPos >= mSharedVec->mElements.size())
			mSharedVec->mElements.push_back(0);
	} 
	// update position
	mSharedVec->mElements[mModPos] = mModVal;
	// clear mod
	mMod = false;
	mModVal = 0;
	mModPos = 0;
}

void Timestamp::set(size_t inThread, size_t inVal) {
	if (inThread != mModPos) {  
		if (mMod) 
			applyMod();
		mModPos = inThread;
	}
	mModVal = inVal;
	mMod = true;
}

void Timestamp::tick(size_t inThread, size_t nr) {
	assert(nr >= get(inThread));  // may have same tick number on aggregate accesses
	set(inThread, nr);
}

bool Timestamp::lessthanorequal(const Timestamp &inTimestamp) const {
	if (mMod && mModVal > inTimestamp.get(mModPos))
		return false;
	if (! mSharedVec)
		return true;
	if (inTimestamp.mMod && get(inTimestamp.mModPos) > inTimestamp.mModVal)
		return false;
	if (mSharedVec == inTimestamp.mSharedVec)
		return true;
	for (size_t i = 0; i < mSharedVec->mElements.size(); ++i) 
		if ((mSharedVec->mElements[i] > inTimestamp.get(i))
			&& (i != mModPos || !mMod))
			return false;
	return true;
}

void Timestamp::merge(const Timestamp &inTimestamp) {
	if ((mSharedVec == 0 || inTimestamp.mSharedVec == 0 || inTimestamp.mSharedVec == mSharedVec)
		&& (!mMod || !inTimestamp.mMod || inTimestamp.mModPos == mModPos)) {
			// cheap case - no vector operations required
			if (!mSharedVec) {
				mSharedVec = inTimestamp.mSharedVec;
				if (mSharedVec)
					mSharedVec->mRefcount++;
			}
			if (!mMod) {
				mMod = inTimestamp.mMod;
				mModVal = inTimestamp.mModVal;
				mModPos = inTimestamp.mModPos;
			} else if (inTimestamp.mMod && inTimestamp.mModVal > mModVal) {
				mModVal = inTimestamp.mModVal;
			}
	} else {
		size_t maxpos = inTimestamp.getMaxPos();
		for (size_t i = 0; i <= maxpos; ++i) { 
			size_t val = inTimestamp.get(i);
			if (val > get(i)) {
				// need to actually modify this timestamp
				set(i, val);
			}
		}
	}
}

void Timestamp::print(std::ostream &outStream) {
	size_t maxpos = getMaxPos();
	outStream << '[';
	for (size_t i = 0; i <= maxpos; ++i) {
		if (i)
			outStream << ' ';
		outStream << get(i);
	}
	outStream << ']';
}

// destructor
TSOTimestamp::~TSOTimestamp() {
	if (mSharedVec) {
		if (--mSharedVec->mRefcount == 0)
			delete mSharedVec;
		mSharedVec = 0;
	}
}

// copy constructor
TSOTimestamp::TSOTimestamp(const TSOTimestamp &inTimestamp) :  
mSharedVec(inTimestamp.mSharedVec), mMod(inTimestamp.mMod), mModValL(inTimestamp.mModValL), 
mModValS(inTimestamp.mModValS), mModPos(inTimestamp.mModPos) {
	if (mSharedVec)
		mSharedVec->mRefcount++;
}

// assignment operator
TSOTimestamp &TSOTimestamp::operator=(const TSOTimestamp &inTimestamp) { 
	mMod = inTimestamp.mMod;
	mModValL = inTimestamp.mModValL;
	mModValS = inTimestamp.mModValS;
	mModPos = inTimestamp.mModPos;
	if (mSharedVec) {
		if (--mSharedVec->mRefcount == 0)
			delete mSharedVec;
	}
	mSharedVec = inTimestamp.mSharedVec;
	if (mSharedVec)
		mSharedVec->mRefcount++;
   return *this;
}

// get max pos with nonzero content
size_t TSOTimestamp::getMaxPos() const {
	size_t maxpos = (mSharedVec ? mSharedVec->mElements.size()/2-1 : 0);
	if (mMod && mModPos > maxpos)
		maxpos = mModPos;
	return maxpos;
}

// get element
size_t TSOTimestamp::get_load(size_t inThread) const {
	if (mMod && inThread == mModPos)
		return mModValL;
	if (! mSharedVec || mSharedVec->mElements.size()/2 <= inThread)
		return 0;
	return mSharedVec->mElements[2*inThread];
}
size_t TSOTimestamp::get_store(size_t inThread) const {
	if (mMod && inThread == mModPos)
		return mModValS;
	if (! mSharedVec || mSharedVec->mElements.size()/2 <= inThread)
		return 0;
	return mSharedVec->mElements[2*inThread+1];
}

// create shared vector
TSOTimestamp::SharedVec::SharedVec(size_t size) : mElements(size, 0), mRefcount(1) {} 

// copy shared vector
TSOTimestamp::SharedVec::SharedVec(const TSOTimestamp::SharedVec &inVec) : mElements(inVec.mElements), mRefcount(1) { }
							 
// delete shared vector
TSOTimestamp::SharedVec::~SharedVec() { assert(!mRefcount); }

// merge mod into vector 
void TSOTimestamp::applyMod() {
	assert(mMod);
	if (!mSharedVec)
		// create vector
		mSharedVec = new SharedVec(2*(mModPos+1));
	else {
		// copy if shared
		if (mSharedVec->mRefcount > 1) {
			mSharedVec->mRefcount--;
			mSharedVec = new SharedVec(*mSharedVec);
		}
		// stretch if too short
		while (mModPos >= mSharedVec->mElements.size()/2) 
			mSharedVec->mElements.push_back(0);
	} 
	// update position
	mSharedVec->mElements[2*mModPos] = mModValL;
	mSharedVec->mElements[2*mModPos+1] = mModValS;
	// clear mod
	mMod = false;
	mModValL = 0;
	mModValS = 0;
	mModPos = 0;
}

void TSOTimestamp::set(size_t inThread, size_t inValL, size_t inValS) {
	if (inThread != mModPos) {  
		if (mMod) 
			applyMod();
		mModPos = inThread;
	}
	mModValL = inValL;
	mModValS = inValS;
	mMod = true;
}

void TSOTimestamp::tick_load(size_t inThread, size_t nr) {
	assert(nr >= get_load(inThread));
	set(inThread, nr, get_store(inThread));
}

void TSOTimestamp::tick_store(size_t inThread, size_t nr) {
	assert(nr >= get_store(inThread));
	set(inThread, get_load(inThread), nr);
}

bool TSOTimestamp::lessthanorequal(const TSOTimestamp &inTimestamp) const {
	if (mMod && (mModValL > inTimestamp.get_load(mModPos) || mModValS > inTimestamp.get_store(mModPos)))
		return false;
	if (! mSharedVec)
		return true;
	if (inTimestamp.mMod && (get_load(inTimestamp.mModPos) > inTimestamp.mModValL || get_store(inTimestamp.mModPos) > inTimestamp.mModValS))
		return false;
	if (mSharedVec == inTimestamp.mSharedVec)
		return true;
	for (size_t i = 0; i < mSharedVec->mElements.size()/2; ++i) 
		if ((mSharedVec->mElements[2*i] > inTimestamp.get_load(i) || mSharedVec->mElements[2*i+1] > inTimestamp.get_store(i))
			&& (i != mModPos || !mMod))
			return false;
	return true;
}

void TSOTimestamp::merge(const TSOTimestamp &inTimestamp) {
	if ((mSharedVec == 0 || inTimestamp.mSharedVec == 0 || inTimestamp.mSharedVec == mSharedVec)
		&& (!mMod || !inTimestamp.mMod || inTimestamp.mModPos == mModPos)) {
			// cheap case - no vector operations required
			if (!mSharedVec) {
				mSharedVec = inTimestamp.mSharedVec;
				if (mSharedVec)
					mSharedVec->mRefcount++;
			}
			if (!mMod) {
				mMod = inTimestamp.mMod;
				mModValL = inTimestamp.mModValL;
				mModValS = inTimestamp.mModValS;
				mModPos = inTimestamp.mModPos;
			} else if (inTimestamp.mMod) {
				if (inTimestamp.mModValL > mModValL) 
					mModValL = inTimestamp.mModValL;
				if (inTimestamp.mModValS > mModValS) 
					mModValS = inTimestamp.mModValS;
			}
	} else {
		size_t maxpos = inTimestamp.getMaxPos();
		for (size_t i = 0; i <= maxpos; ++i) { 
			int valL = inTimestamp.get_load(i);
			int valS = inTimestamp.get_store(i);
			int curL = get_load(i);
			int curS = get_store(i);
			if (valL > curL || valS > curS) {
				// need to actually modify this TSOTimestamp
				set(i, (valL > curL ? valL : curL), (valS > curS ? valS : curS));
			}
		}
	}
}

void TSOTimestamp::print(std::ostream &outStream) {
	size_t maxpos = getMaxPos();
	outStream << '[';
	for (size_t i = 0; i <= maxpos; ++i) {
		if (i)
			outStream << ' ';
		outStream << get_load(i) << '/' << get_store(i);
	}
	outStream << ']';
}

TSOTimestamp &TSOTimestampVector::get(size_t inThread) {
	if (mHasException && inThread == mExceptionThread)
		return mExceptionTimestamp;
	else
		return mDefaultTimestamp;
}

void TSOTimestampVector::assign_all(const TSOTimestamp &inClock) {
	mDefaultTimestamp = inClock;
	mHasException = false;
}

void TSOTimestampVector::assign_all_except_p(const TSOTimestamp &inClock, size_t inThread) {
	assert(inThread >= 0);
	if (!mHasException || inThread != mExceptionThread)
		mExceptionTimestamp = mDefaultTimestamp;
	mHasException = true;
	mExceptionThread = inThread;
	mDefaultTimestamp = inClock;
}
