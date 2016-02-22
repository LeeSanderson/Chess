/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/


namespace THE
{
	// SpinLock
	//  
#ifndef USE_NON_BLOCKING_SYNC
	class SpinLock {

	protected:

		SpinLock(ULONG retries = 4000)
		{
			InitializeCriticalSection(&cs);
		}

		void Acquire() 
		{
			EnterCriticalSection(&cs);
		}

		void Release()
		{
			LeaveCriticalSection(&cs);
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
			 void Acquire() {
				 PVOID old;
				 ULONG retries = 0;

				 do {
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
						 Sleep(0);
						 retries = 0;
					 }
				 }	while (1);
			 }

			 // Releases the lock
			 //
			 void Release()
			 {
				 // Pointer assignment is atomic so there is no need to use any
				 // interlocking functions.
				 //
				 ChessSyncVarAccess((int)&this->lock);
				 this->lock = (PVOID)0;
			 }

	private:

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

	private:

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
			writeV(head,0);
			writeV(tail,0);
			mask = size - 1;
			elems = new T[size];
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

		bool Steal(T &result)
		{
			bool found;
			this->Acquire();

			// ensure that at most one (foreign) thread writes to head
			// increment the head. Save in local h for efficiency
			//
			LONG h = readV(head);
			writeV(head, h + 1);

			// insert a memory fence here if memory is not sequentially consistent
			//
			if (h < readV(tail)) {
				// == (h+1 <= tail) == (head <= tail)
				//
				// BUG: writeV(head, h + 1);
				result = elems[h & mask];
				found = true;
			}
			else {
				// failure: either empty or single element interleaving with pop
				//
				writeV(head, h);              // restore the head
				found = false;
			}
			this->Release();
			return found;
		}

		bool Pop(T & result)
		{
			// decrement the tail. Use local t for efficiency.
			//
			LONG t = readV(tail) - 1;
			writeV(tail, t);

			// insert a memory fence here if memory is not sequentially consistent
			//
			if (readV(head) <= t)
			{
				// BUG:  writeV(tail, t);

				// == (head <= tail)
				//
				result = elems[t & mask];
				return true;
			}
			else
			{
				// failure: either empty or single element interleaving with take
				//
				writeV(tail, t + 1);             // restore the tail 
				return SyncPop(result);   // do a single-threaded pop
			}
		}

	private:

		bool SyncPop(T & result)
		{
			bool found;

			this->Acquire();

			// ensure that no Steal interleaves with this pop
			//
			LONG t = readV(tail) - 1;
			writeV(tail, t);
			if (readV(head) <= t)
			{
				// == (head <= tail)
				//
				result = elems[t & mask];
				found = true;
			}
			else
			{
				writeV(tail, t + 1);       // restore tail
				found = false;
			}
			if (readV(head) > t)
			{
				// queue is empty: reset head and tail
				//
				writeV(head, 0);
				writeV(tail, 0);
				found = false;
			}
			this->Release();
			return found;
		}

	public:
		void Push(T elem)
		{
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
			// Correct: if (t < readV(head) + mask && t < MaxSize)
#define BUG3
#ifdef BUG3
			if (t < readV(head) + mask + 1 && t < MaxSize)
#else
			if (t < readV(head) + mask   // == t < head + size - 1
				&& t < MaxSize)
#endif
			{
				elems[t & mask] = elem;
				writeV(tail, t + 1);       // only increment once we have initialized the task entry.
			}
			else
			{
				// failure: we need to resize or re-index
				//
				SyncPush(elem);
			}
		}

	private:
		void SyncPush(T elem)
		{
			this->Acquire();
			// ensure that no Steal interleaves here
			// cache head, and calculate number of tasks
			//
			LONG h = readV(head);
			LONG count = readV(tail) - h;

			// normalize indices
			//
			h = h & mask;           // normalize head
			writeV(head, h);
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
				for (LONG i = 0; i < count; i++)
				{
					newtasks[i] = elems[(h + i) & mask];
				}
				delete[] elems;
				elems = newtasks;
				mask = newsize - 1;
				writeV(head, 0);
				writeV(tail, count);
			}

			assert(count < mask);

			// push the element
			//
			LONG t = readV(tail);
			elems[t & mask] = elem;
			writeV(tail, t + 1);
			this->Release();
		}
	};
} // namespace THE
