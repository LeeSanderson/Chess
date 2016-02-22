/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "SyncVar.h"
#include "EventAttribute.h"
#include "ChessEvent.h"
#include "SyncVarVector.h"

class EventCounter {
private:
	TaskVector<int> mLast;
	std::vector<EventId> mEvts; // stack of event ids
    int watermark;
public:
	EventCounter() {}

	EventId getNext(Task tid, bool push) {
		EventId id(tid, ++mLast[tid]);
		if (push) {
			//*GetChessOutputStream() << "evts[" << mEvts.size() << "]=" << id << std::endl;
			mEvts.push_back(id);
		}
		return id;
	}

    EventId getEventId(size_t trid)
	{
        //assert(trid < mEvts.size());
		// temporary 
        if (trid >= mEvts.size())
			trid = mEvts.size() - 1;
        return mEvts[trid];
	}

	EventId peekNext(Task tid) {
		return EventId(tid, mLast[tid]+1);
	}

	void setInitState() {
		mLast.clear();
        watermark = mEvts.size();
	}

	void clear() { 
		mLast.clear(); 
		mEvts.clear();
    }

	void reset() {
       mLast.clear();
       mEvts.resize(watermark);
	}

    size_t getNumEvents() 
    {
       return mEvts.size();
    }
};
