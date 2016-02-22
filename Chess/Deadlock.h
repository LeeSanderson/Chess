/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

class DeadlockMgmt {
private:
	enum State {NORMAL=0, TEMP_BLOCK, QUIESCENT, DEADLOCKED}; 

	static const unsigned QUIESCENCE_THRESHOLD = 5; // after QUIESCENCE_THRESHOLD we're quiescent
	static const unsigned DEADLOCK_THRESHOLD = 1000; // after DEADLOCK_THRESHOLD we cry deadlock

	State state;
	unsigned count;
	unsigned oldWatermark;
	unsigned newWatermark;

public:
	void reset() {
		state = NORMAL;
		count = 0;
		oldWatermark = 0;
		newWatermark = 0;
	};

	DeadlockMgmt () {
		reset();
	};
	void imWaiting() {
		switch(state) {

			case NORMAL:
				state = TEMP_BLOCK;
				break;

			case DEADLOCKED:
				break;

			case TEMP_BLOCK:
				if (madeProgress())
					reset();
				else {
					if (count == QUIESCENCE_THRESHOLD)
						state = QUIESCENT;
					count++;
				}
				break;

			case QUIESCENT:
				if (madeProgress())
					reset();
				else {
					if (count == DEADLOCK_THRESHOLD)
						state = DEADLOCKED;
					count++;
				}
				break;
		};
	};

	BOOL isDeadlocked() {
		return state == DEADLOCKED;
	};

	BOOL isQuiescent() {
		return state == QUIESCENT;
	};

	void setWatermark(unsigned w) {
		oldWatermark = newWatermark;
		newWatermark = w;
	};

	BOOL madeProgress() {
		return (newWatermark > oldWatermark);
	};
};
