/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <vector>

#define F_RunTest 0
#define F_Stealer 1
#define F_Push 2
#define F_Pop 3
#define F_Steal 4
#define F_SyncPush 5
#define F_SyncPop 6
#define F_Acquire 7
#define F_Release 8

namespace THE
{
struct ActivationRecord {
public:
	int fname;
	int pc;
	std::vector<void *> lvars;
	ActivationRecord()
	{
		fname = 0;
		pc = 0;
	}

public: 
	void Update(int pc0)
	{
		pc = pc0;
	}

	void Update(int pc0, int id, void *val0)
	{
		pc = pc0;
		if (lvars.size() <= id)
			lvars.resize(id + 1);
		lvars[id] = val0;
	}
};

struct State 
{
public:
	std::vector<std::vector<ActivationRecord>> stacks;

	void Push(int tid, int fname)
	{
		ActivationRecord ar;
		ar.fname = fname;
		if (stacks.size() <= tid)
			stacks.resize(tid+1);
		stacks[tid].push_back(ar);
	}

	void Pop(int tid)
	{
		int size = stacks[tid].size();
		assert(size > 0);
		stacks[tid].resize(size-1);
	}

	ActivationRecord& Peek(int tid)
	{
		int size = stacks[tid].size();
		assert(size > 0);
		return stacks[tid][size-1];
	}

	void Clear()
	{
		stacks.clear();
	}
};

State state;

	// SpinLock
	//  
#ifndef USE_NON_BLOCKING_SYNC
	class SpinLock {

	protected:

		SpinLock(ULONG retries = 4000)
		{
			InitializeCriticalSection(&cs);
		}

		void Acquire(int tid) 
		{
			state.Push(tid, F_Acquire);
			ActivationRecord& ar = state.Peek(tid);
			
			// pc == 0
			state.Peek(tid).Update(0);
			EnterCriticalSection(&cs);

			state.Pop(tid);
		}

		void Release(int tid)
		{
			state.Push(tid, F_Release);
			ActivationRecord& ar = state.Peek(tid);
			
			// pc == 0
			state.Peek(tid).Update(0);
			LeaveCriticalSection(&cs);

			state.Pop(tid);
		}

		~SpinLock()
		{
			DeleteCriticalSection(&cs);
		}

	private:

		CRITICAL_SECTION cs;
	};
#else
#include <ChessApi.h>

	class SpinLock {
	protected:

		// On a single processor machine we yield the processor
		// straightaway.
		//
		SpinLock(ULONG retries = 4):
			 lock((PVOID)0),
				 maxRetriesBeforeSleep(retries)
			 {
				 SYSTEM_INFO sysInfo;
				 GetSystemInfo(&sysInfo);

				 if (sysInfo.dwNumberOfProcessors == 1) {
					 this->maxRetriesBeforeSleep = 0;
				 }
			 }

			 // Grabs the lock: spin waiting the lock to become free.
			 // If we are over the spin threshold give out the current thread's quanta.
			 //
			 void Acquire(int tid) {
				state.Push(tid, F_Acquire);
				ActivationRecord& ar = state.Peek(tid);
			
				 PVOID old;
				 ULONG retries = 0;

				 do {
					 // pc == 0
					 state.Peek(tid).Update(0, 0, (void *) retries);
					 old = InterlockedExchangePointer(&this->lock, (PVOID)1);

					 if (old == (PVOID)0) {
						 // If we got here, then we grabed the lock.
						 // old == 0 means that this thread "transitioned" the lock
						 // from an unlocked state (0) to a locked state (1)
						 //
						 break;
					 }

					 // Give up the current thread quanta if we did not aquire the
					 // lock and we exceeded the max retry count
					 //
					 if (++retries >= this->maxRetriesBeforeSleep) {
						 // pc == 1
						 state.Peek(tid).Update(1, 0, (void *) retries);
						 Sleep(0);
						 retries = 0;
					 }
				 }	while (1);

				 state.Pop(tid);
			 }

			 // Releases the lock
			 //
			 void Release(int tid)
			 {
				 state.Push(tid, F_Release);
				 ActivationRecord& ar = state.Peek(tid);
				 // Pointer assignment is atomic so there is no need to use any
				 // interlocking functions.
				 //

				 // pc == 0
				 state.Peek(tid).Update(0);
				 ChessSyncVarAccess((int)&this->lock);
				 this->lock = (PVOID)0;

				 state.Pop(tid);
			 }

	//private:
	public:
		// Lock variable:
		// 1 - lock acquired
		// 0 - lock free
		//
		PVOID volatile lock;

		// Tells how many times we should retry before giving up the
		// thread quanta. On single proc machines this should be 0
		// because the lock cannot be released while we are running so
		// there is no point in spinning...
		//
		ULONG maxRetriesBeforeSleep;

	}; // SpinLock
#endif // LOCK_CS


	//
	// A WorkStealQueue is a wait-free, lock-free structure associated with a single
	// thread that can Push and Pop elements. Other threads can do Take operations
	// on the other end of the WorkStealQueue with little contention.
	// </summary>
	//
	template <typename T>
	class WorkStealQueue : public SpinLock
	{
		// A 'WorkStealQueue' always runs its code in a single OS thread. We call this the
		// 'bound' thread. Only the code in the Take operation can be executed by
		// other 'foreign' threads that try to steal work.
		//
		// The queue is implemented as an array. The head and tail index this
		// array. To avoid copying elements, the head and tail index the array modulo
		// the size of the array. By making this a power of two, we can use a cheap
		// bit-and operation to take the modulus. The "mask" is always equal to the
		// size of the task array minus one (where the size is a power of two).
		//
		// The head and tail are volatile as they can be updated from different OS threads.
		// The "head" is only updated by foreign threads as they Take (steal) a task from
		// this queue. By putting a lock in Take, there is at most one foreign thread
		// changing head at a time. The tail is only updated by the bound thread.
		//
		// invariants:
		//   tasks.length is a power of 2
		//   mask == tasks.length-1
		//   head is only written to by foreign threads
		//   tail is only written to by the bound thread
		//   At most one foreign thread can do a Take
		//   All methods except Take are executed from a single bound thread
		//   tail points to the first unused location
		//
		static const LONG MaxSize     = 1024*1024;
		static const LONG InitialSize = 1024; // must be a power of 2

	// private:
	public:

		volatile LONG head;  // only updated by Take 
		volatile LONG tail;  // only updated by Push and Pop 
		T *  elems;         // the array of tasks 
		LONG mask;           // the mask for taking modulus 

		LONG readV(volatile LONG & v) { 
			LONG t = InterlockedCompareExchange(&v, 0, 0);
			return t;
		}
		void writeV(volatile LONG & v, LONG w) { 
			InterlockedExchange(&v, w); 
		}

	public:

		WorkStealQueue(LONG size = MaxSize)
		{      
			head = 0;
			tail = 0;
			mask = size - 1;
			elems = new T[size];
			for(LONG i=0; i<size; i++){
				elems[i] = T();
			}
		}

		~WorkStealQueue() 
		{
			delete elems;
		}

		// Push/Pop and Steal can be executed interleaved. In particular:
		// 1) A take and pop should be careful when there is just one element
		//    in the queue. This is done by first incrementing the head/decrementing the tail
		//    and than checking if it interleaved (head > tail).
		// 2) A push and take can interleave in the sense that a push can overwrite the
		//    value that is just taken. To account for this, we check conservatively in
		//    the push to assume that the size is one less than it actually is.
		//
		// See the CILK "THE" protocol for more information:
		//   "The implementation of the CILK-5 multi-threaded language"
		//   Matteo Frigo, Charles Leiserson, and Keith Randall.
		//

	public:
		LONG ComputeIndex(LONG l)
		{
			if (l >= head)
				return l - head;
			else
				return mask + 1 - (head - l);
		}

		bool Steal(T &result, int tid)
		{
			// id of h = 0
			// id of found = 1

			state.Push(tid, F_Steal);
			ActivationRecord& ar = state.Peek(tid);

			// pc == 0
			state.Peek(tid).Update(0);
			this->Acquire(tid);

			// ensure that at most one (foreign) thread writes to head
			// increment the head. Save in local h for efficiency
			
			// pc == 1
			state.Peek(tid).Update(1);
			LONG h = readV(head);

			// pc == 2
			state.Peek(tid).Update(2, 0, (void *) ComputeIndex(h));
			writeV(head, h + 1);

			bool found;
			// insert a memory fence here if memory is not sequentially consistent
			// pc == 3
			state.Peek(tid).Update(3);
			if (h < readV(tail)) {
				// == (h+1 <= tail) == (head <= tail)
				//
				// BUG: writeV(head, h + 1);
				result = elems[h & mask];
				found = true;
			}
			else {
				// failure: either empty or single element interleaving with pop
				// pc == 4
				state.Peek(tid).Update(4);
				writeV(head, h);              // restore the head
				found = false;
			}

			// pc == 5
			state.Peek(tid).Update(5, 1, (void *) found);
			this->Release(tid);

			state.Pop(tid);
			return found;
		}

		bool Pop(T & result, int tid)
		{
			// id of t == 0

			state.Push(tid, F_Pop);
			ActivationRecord& ar = state.Peek(tid);

			bool retVal;
			// decrement the tail. Use local t for efficiency.
			
			// pc == 0
			state.Peek(tid).Update(0);
			LONG t = readV(tail) - 1;

			// pc == 1
			state.Peek(tid).Update(1, 0, (void *) ComputeIndex(t));
			writeV(tail, t);

			// insert a memory fence here if memory is not sequentially consistent
			// pc == 2
			state.Peek(tid).Update(2);
			if (readV(head) <= t)
			{
				// BUG:  writeV(tail, t);

				// == (head <= tail)
				//
				result = elems[t & mask];
				retVal = true;
			}
			else
			{
				// failure: either empty or single element interleaving with take
				// pc == 3
				state.Peek(tid).Update(3);
				writeV(tail, t + 1);             // restore the tail 

				retVal = SyncPop(result, tid);   // do a single-threaded pop
			}

			state.Pop(tid);
			return retVal;
		}

	private:

		bool SyncPop(T & result, int tid)
		{
			// id of t == 0
			// id of found == 1

			state.Push(tid, F_SyncPop);
			ActivationRecord& ar = state.Peek(tid);

			// pc == 0
			state.Peek(tid).Update(0);
			this->Acquire(tid);

			// ensure that no Steal interleaves with this pop
			// pc == 1
			state.Peek(tid).Update(1);
			LONG t = readV(tail) - 1;

			// pc == 2
			state.Peek(tid).Update(2, 0, (void *) ComputeIndex(t));
			writeV(tail, t);

			bool found;

			// pc == 3
			state.Peek(tid).Update(3);
			if (readV(head) <= t)
			{
				// == (head <= tail)
				//
				result = elems[t & mask];
				found = true;
			}
			else
			{
				// pc == 4
				state.Peek(tid).Update(4);
				writeV(tail, t + 1);       // restore tail
				found = false;
			}

			// pc == 5
			state.Peek(tid).Update(5, 1, (void *) found);
			if (readV(head) > t)
			{
				// queue is empty: reset head and tail
				// pc == 6
				state.Peek(tid).Update(6);
				writeV(head, 0);

				// pc == 7
				state.Peek(tid).Update(7);
				writeV(tail, 0);
				found = false;
			}

			// pc == 8
			state.Peek(tid).Update(8, 1, (void *) found);
			this->Release(tid);

			state.Pop(tid);
			return found;
		}

	public:
		void Push(T elem, int tid)
		{
			// id of t == 0

			state.Push(tid, F_Push);
  			ActivationRecord& ar = state.Peek(tid);

			// pc == 0
			state.Peek(tid).Update(0);
			LONG t = readV(tail);
			// Careful here since we might interleave with Steal.
			// This is no problem since we just conservatively check if there is
			// enough space left (t < head + size). However, Steal might just have
			// incremented head and we could potentially overwrite the old head
			// entry, so we always leave at least one extra 'buffer' element and
			// check (tail < head + size - 1). This also plays nicely with our
			// initial mask of 0, where size is 2^0 == 1, but the tasks array is
			// still null.
			//

			// pc == 1
			state.Peek(tid).Update(1, 0, (void *) ComputeIndex(t));
			// Correct: if (t < readV(head) + mask && t < MaxSize)
			// BUG: if (t < readV(head) + mask + 1 && t < MaxSize)
			if (t < readV(head) + mask   // == t < head + size - 1
				&& t < MaxSize)
			{
				elems[t & mask] = elem;

				// pc == 2
				state.Peek(tid).Update(2);
				writeV(tail, t + 1);       // only increment once we have initialized the task entry.
			}
			else
			{
				// failure: we need to resize or re-index
				//
				// pc == 3
				state.Peek(tid).Update(3);
				SyncPush(elem, tid);
			}

			state.Pop(tid);
		}

	private:
		void SyncPush(T elem, int tid)
		{
			// id of h == 0
			// id of count == 1
			// id of t == 2

			state.Push(tid, F_SyncPush);
			ActivationRecord& ar = state.Peek(tid);

			// pc == 0
			state.Peek(tid).Update(0);
			this->Acquire(tid);
			// ensure that no Steal interleaves here
			// cache head, and calculate number of tasks
			
			// pc == 1
			state.Peek(tid).Update(1);
			LONG h = readV(head);

			// pc == 2
			state.Peek(tid).Update(2, 0, (void *) ComputeIndex(h));
			LONG count = readV(tail) - h;

			// normalize indices
			//
			h = h & mask;           // normalize head

			// pc == 3
			state.Peek(tid).Update(3, 0, (void *) ComputeIndex(h));
			state.Peek(tid).Update(3, 1, (void *) count);
			writeV(head, h);

			// pc == 4
			state.Peek(tid).Update(4);
			writeV(tail, h + count);

			// check if we need to enlarge the tasks
			//
			if (count >= mask)
			{
				// == (count >= size-1)
				//
				LONG newsize = (mask == 0 ? InitialSize : 2 * (mask + 1));

				assert(newsize < MaxSize);

				T * newtasks = new T[newsize];
				for(LONG i=0; i<newsize; i++){
					newtasks[i] = T();
				}
				for (LONG i = 0; i < count; i++)
				{
					newtasks[i] = elems[(h + i) & mask];
				}
				delete[] elems;
				elems = newtasks;
				mask = newsize - 1;

				// pc == 5
				state.Peek(tid).Update(5);
				//writeV(head, 0);
				head = 0;

				// pc == 6
				state.Peek(tid).Update(6);
				//writeV(tail, count);
				tail = count;
			}

			assert(count < mask);

			// push the element
			// pc == 7
			state.Peek(tid).Update(7);
			LONG t = readV(tail);
			elems[t & mask] = elem;

			// pc == 8
			state.Peek(tid).Update(8, 2, (void *) ComputeIndex(t));
			writeV(tail, t + 1);

			// pc == 9
			state.Peek(tid).Update(9);
			this->Release(tid);

			state.Pop(tid);
		}
	};
} // namespace THE
