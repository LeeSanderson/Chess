/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// SlimManualResetEvent.cs
//
// An manual-reset event that mixes a little spinning with a true Win32 event.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;

namespace Foo
{
    internal class SpinWait
    {
        public const int YIELD_THRESHOLD = 2;

        public void SpinOnce()
        {
        }

    }

    internal class Platform
    {
        public const bool IsSingleProcessor = true;
    }

    internal class Error
    {
        public static Exception ArgumentOutOfRange(string foo)
        {
            return new Exception("foo");
        }
    }

    internal class TraceHelpers
    {
        public static void Assert(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition);
        }
        public static void Assert(bool condition, string foo)
        {
            System.Diagnostics.Debug.Assert(condition, foo);
        }
    }

    /// <summary>
    /// ManualResetEventSlim wraps a manual-reset event internally with a little bit of
    /// spinning. When an event will be set imminently, it is often advantageous to avoid
    /// a 4k+ cycle context switch in favor of briefly spinning. Therefore we layer on to
    /// a brief amount of spinning that should, on the average, make using the slim event
    /// cheaper than using Win32 events directly. This can be reset manually, much like
    /// a Win32 manual-reset would be.
    ///
    /// Notes:
    ///     We lazily allocate the Win32 event internally. Therefore, the caller should
    ///     always call Dispose to clean it up, just in case. This API is a no-op of the
    ///     event wasn't allocated, but if it was, ensures that the event goes away
    ///     eagerly, instead of waiting for finalization.
    /// </summary>
    [DebuggerDisplay("Set = {IsSet}")]
    [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
    public class ManualResetEventSlim : IDisposable
    {

        // These are the default spin counts we use on single-proc and MP machines.
        private const int DEFAULT_SPIN_SP = 1;
        private const int DEFAULT_SPIN_MP = SpinWait.YIELD_THRESHOLD / 2;
        private const int MAX_DYNAMIC_SPIN = SpinWait.YIELD_THRESHOLD * 2;
        private const int USE_DYNAMIC_SPIN_MASK = unchecked((int)0x80000000);

        // State to represent signaled versus unsignaled.
        private const int EVENT_SIGNALED = 1;
        private const int EVENT_UNSIGNALED = 0;

        private volatile int m_state; // The state of our event.
        private volatile bool m_disposed; // Whether the event has been disposed.
        private ManualResetEvent m_eventObj; // A true Win32 event used for waiting.
        private int m_spinCount; // # of spins before waiting.

#if DEBUG
        private static int s_nextId; // The next id that will be given out.
        private int m_id = Interlocked.Increment(ref s_nextId); // A unique id for debugging purposes only.
        private long m_lastSetTime;
        private long m_lastResetTime;
#endif

        //-----------------------------------------------------------------------------------
        // Constructs a new event, optionally specifying the initial state and spin count.
        // The defaults are that the event is unsignaled and some reasonable default spin.
        //

        /// <summary>
        /// Constructs a new unset event object with a default spin count.
        /// </summary>
        public ManualResetEventSlim() : this(false)
        {
        }

        /// <summary>
        /// Constructs a new event object with a default spin count.
        /// </summary>
        /// <param name="initialState">Whether the event should be initially set (true) or not (false).</param>
        public ManualResetEventSlim(bool initialState)
        {
            // Specify the defualt spin count, and use dynamic spin adjustment if we're
            // on a multi-processor machine. Otherwise, we won't.
            Initialize(initialState, DEFAULT_SPIN_MP, !Platform.IsSingleProcessor);
        }

        /// <summary>
        /// Constructs a new event object.
        /// </summary>
        /// <param name="initialState">Whether the event should be initially set (true) or not (false).</param>
        /// <param name="spinCount">The number of spins before falling back to a true wait.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If the spinCount is less than 0.</exception>
        public ManualResetEventSlim(bool initialState, int spinCount)
        {
            if (spinCount < 0)
            {
                throw Error.ArgumentOutOfRange("spinCount");
            }

            // We will suppress dynamic spin adjustment because the user specified a count.
            Initialize(initialState, spinCount, false);
        }

        /// <summary>
        /// Whether the event has been signaled.
        /// </summary>
        public bool IsSet
        {
            get { return (m_state == EVENT_SIGNALED); }
        }

        /// <summary>
        /// Retrieves the underlying Win32 event object. Accessing this property forces
        /// initialization if the event hasn't been created already.
        /// </summary>
        public WaitHandle WaitHandle
        {
            get
            {
                ThrowIfDisposed();

                if (m_eventObj == null)
                {
                    // Lazily initialize the event object if needed.
                    LazyInitializeEvent();
                }

                return m_eventObj;
            }
        }

        /// <summary>
        /// Whether we should use dynamic spin count adjustment.
        /// </summary>
        private bool UseDynamicSpinAdjustment
        {
            get { return (m_spinCount & USE_DYNAMIC_SPIN_MASK) == USE_DYNAMIC_SPIN_MASK; }
        }

        /// <summary>
        /// Retrieves the current spin count.
        /// </summary>
        public int SpinCount
        {
            get { return (m_spinCount & ~USE_DYNAMIC_SPIN_MASK); }
        }

        /// <summary>
        /// Initializes the internal state of the event.
        /// </summary>
        /// <param name="initialState">Whether the event is set initially or not.</param>
        /// <param name="spinCount">The spin count that decides when the event will block.</param>
        /// <param name="useDynamicSpinAdjustment">Whether to use dynamic spin count adjustment.</param>
        private void Initialize(bool initialState, int spinCount, bool useDynamicSpinAdjustment)
        {
            m_state = initialState ? EVENT_SIGNALED : EVENT_UNSIGNALED;
            m_spinCount = /*Environment.ProcessorCount == 1 ? DEFAULT_SPIN_SP :*/ spinCount;

            if (useDynamicSpinAdjustment)
            {
                m_spinCount |= USE_DYNAMIC_SPIN_MASK;
            }

#if DEBUG
            TraceHelpers.TraceInfo("{0} - tid {1}: ManualResetEventSlim::ctor - created a new event,set={2}",
                m_id, Thread.CurrentThread.ManagedThreadId, initialState);
#endif
        }

        /// <summary>
        /// Throws an exception if the object has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException("SlimManualResetEvent");
            }
        }

        /// <summary>
        /// This method lazily initializes the event object. It uses CAS to guarantee that
        /// many threads racing to call this at once don't result in more than one event
        /// being stored and used. The event will be signaled or unsignaled depending on
        /// the state of the thin-event itself, with synchronization taken into account.
        /// </summary>
        /// <returns>True if a new event was created and stored, false otherwise.</returns>
        private bool LazyInitializeEvent()
        {
            int preInitializeState = m_state;
            ManualResetEvent newEventObj = new ManualResetEvent(preInitializeState == EVENT_SIGNALED);

            // We have to CAS this in case we are racing with another thread. We must
            // guarantee only one event is actually stored in this field.
            if (Interlocked.CompareExchange(ref m_eventObj, newEventObj, null) != null)
            {
                // We raced with someone else and lost. Destroy the garbage event.
                newEventObj.Close();

                return false;
            }
            else
            {
#if DEBUG
                TraceHelpers.TraceInfo("{0} - tid {1}: ManualResetEventSlim::LazyInitializeEvent - created a real event", m_id, Thread.CurrentThread.ManagedThreadId);
#endif

                // Now that the event is published, verify that the state hasn't changed since
                // we snapped the preInitializeState. Another thread could have done that
                // between our initial observation above and here. The barrier incurred from
                // the CAS above (in addition to m_state being volatile) prevents this read
                // from moving earlier and being collapsed with our original one.
                if (m_state != preInitializeState)
                {
                    TraceHelpers.Assert(IsSet,
                        "the only safe concurrent transition is from unset->set: detected set->unset");

                    // We saw it as unsignaled, but it has since become set.
                    m_eventObj.Set();
                }

                return true;
            }
        }

        /// <summary>
        /// Sets the event, possibly waking up any waiting threads.
        /// </summary>
        public void Set()
        {
            ThrowIfDisposed();

#if DEBUG
            TraceHelpers.TraceInfo("{0} - tid {1}: ManualResetEventSlim::Set - setting the event", m_id, Thread.CurrentThread.ManagedThreadId);
#endif

            // We need a memory barrier here to ensure the next read of the event object
            // doesn't occur before the write of the state. This would be a legal movement
            // according to the .NET memory model. We use an interlocked operation to
            // acheive this instead of an explicit memory barrier (since it is more
            // efficient currently on the CLR).
#pragma warning disable 0420
            Interlocked.Exchange(ref m_state, EVENT_SIGNALED);
#pragma warning restore 0420
            ManualResetEvent eventObj = m_eventObj;
            if (eventObj != null)
            {
                // We must surround this call to Set in a lock.  The reason is fairly subtle.
                // Sometimes a thread will issue a Wait and wake up after we have set m_state,
                // but before we have gotten around to setting m_eventObj (just below). That's
                // because Wait first checks m_state and will only access the event if absolutely
                // necessary.  However, the coding pattern { event.Wait(); event.Dispose() } is
                // quite common, and we must support it.  If the waiter woke up and disposed of
                // the event object before the setter has finished, however, we would try to set a
                // now-disposed Win32 event.  Crash!  To deal with this race, we use a lock to
                // protect access to the event object when setting and disposing of it.  We also
                // double-check that the event has not become null in the meantime when in the lock.
                lock (eventObj)
                {
                    if (m_eventObj != null)
                    {
                        // If somebody is waiting, we must set the event.
                        m_eventObj.Set();
                    }
                }
            }

#if DEBUG
            m_lastSetTime = DateTime.Now.Ticks;
#endif
        }

        /// <summary>
        /// Resets the event to the unsignaled state.
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();

#if DEBUG
            TraceHelpers.TraceInfo("{0} - tid {1}: ManualResetEventSlim::Reset - resetting the event", m_id, Thread.CurrentThread.ManagedThreadId);
#endif

            // If there's an event, reset it.
            if (m_eventObj != null)
            {
                m_eventObj.Reset();
            }

            // There is a race here. If another thread Sets the event, we will get into a state
            // where m_state will be unsignaled, yet the Win32 event object will have been signaled.
            // This could cause waiting threads to wake up even though the event is in an
            // unsignaled state. This is fine -- those that are calling Reset concurrently are
            // responsible for doing "the right thing" -- e.g. rechecking the condition and
            // resetting the event manually.

            // And finally set our state back to unsignaled.
            m_state = EVENT_UNSIGNALED;

#if DEBUG
            m_lastResetTime = DateTime.Now.Ticks;
#endif
        }

        /// <summary>
        /// Waits for the event to become set. We will spin briefly if the event isn't set
        /// (assuming a non-0 timeout), before falling back to a true wait.
        /// </summary>
        public void Wait()
        {
            Wait(Timeout.Infinite);
        }

        /// <summary>
        /// Waits for the event to become set. We will spin briefly if the event isn't set
        /// (assuming a non-0 timeout), before falling back to a true wait.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait.</param>
        /// <returns>True if the wait succeeded, false if the timeout expired.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">If the timeout is not within range.</exception>
        public bool Wait(TimeSpan timeout)
        {
            long totalMilliseconds = (long)timeout.TotalMilliseconds;
            if (totalMilliseconds < -1 || totalMilliseconds > int.MaxValue)
            {
                throw Error.ArgumentOutOfRange("timeout");
            }

            return Wait((int)totalMilliseconds);
        }

        /// <summary>
        /// Waits for the event to become set. We will spin briefly if the event isn't set
        /// (assuming a non-0 timeout), before falling back to a true wait.
        /// </summary>
        /// <param name="millisecondsTimeout">The maximum amount of time to wait.</param>
        /// <returns>True if the wait succeeded, false if the timeout expired.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">If the timeout is not within range.</exception>
        public bool Wait(int millisecondsTimeout)
        {
            ThrowIfDisposed();

            if (millisecondsTimeout < -1)
            {
                throw Error.ArgumentOutOfRange("millisecondsTimeout");
            }

            if (!IsSet)
            {
                if (millisecondsTimeout == 0)
                {
                    // For 0-timeouts, we just return immediately.
                    return false;
                }
                else
                {
#if DEBUG
                    TraceHelpers.TraceInfo("{0} - tid {1}: ManualResetEventSlim::Wait - doing a spin wait for {2} spins",
                        m_id, Thread.CurrentThread.ManagedThreadId, SpinCount);
#endif

                    // We spin briefly before falling back to allocating and/or waiting on a true event.
                    SpinWait s = new SpinWait();
                    Stopwatch sw = null;

                    if (SpinCount > 0)
                    {
                        if (millisecondsTimeout != Timeout.Infinite)
                        {
                            // We will account for time spent spinning, so that we can decrement it from our
                            // timeout.  In most cases the time spent in this section will be negligible.  But
                            // we can't discount the possibility of our thread being switched out for a lengthy
                            // period of time.  The timeout adjustments only take effect when and if we actually
                            // decide to block in the kernel below.
                            sw = Stopwatch.StartNew();
                        }

                        for (int i = 0; i < SpinCount; i++)
                        {
                            s.SpinOnce();

                            // Keep rechecking the state. If we've become signaled while we spun above,
                            // return.  This is done with a volatile read to prevent reordering and hoisting
                            // out of the loop.
                            if (IsSet)
                            {
                                return true;
                            }
                        }
                    }

                    if (m_eventObj == null)
                    {
                        // Lazily allocate the event. This method internally handles races w/ other threads.
                        LazyInitializeEvent();

                        if (IsSet)
                        {
                            // If it has since become signaled, there's no need to wait. This is strictly
                            // unnecessary, but does avoid a kernel transition.  Since the allocation could
                            // have taken some time, this might be worth the extra check in some cases.
                            return true;
                        }
                    }

#if DEBUG
                    TraceHelpers.TraceInfo("{0} - tid {1}: ManualResetEventSlim::Wait - entering a real wait",
                        m_id, Thread.CurrentThread.ManagedThreadId);
#endif

                    // Do our dynamic spin count accounting.  We don't worry about thread safety here.
                    // It's OK for reads and writes to be carried out non-atomically -- we might miss one
                    // here or there, but because it's a 4-byte aligned value we are guaranteed no tears.
                    if (UseDynamicSpinAdjustment)
                    {
                        int spinCount = SpinCount;

                        if (spinCount != 0 && spinCount < MAX_DYNAMIC_SPIN)
                        {
                            // We use an algorithm similar to the OS critical section. If our
                            // spinning didn't work out, we increment the counter so that we increase
                            // the chances of spinning working the next time. 
                            m_spinCount = ((spinCount + 1) % int.MaxValue) | USE_DYNAMIC_SPIN_MASK;
                        }
                        else
                        {
                            // If we reached the dynamic spin maximum, and it's still not working, bail.
                            // We don't want to spin any longer since it's not buying us anything (in fact,
                            // it's just wasting time). This forces us to go straight to the wait next time.
                            m_spinCount = 0;
                        }
                    }

                    // If we spun at all, we'll take into account the time elapsed by adjusting
                    // the timeout value before blocking in the kernel.
                    int realMillisecondsTimeout = millisecondsTimeout;
                    if (sw != null)
                    {
                        long elapsedMilliseconds = sw.ElapsedMilliseconds;
                        if (elapsedMilliseconds > int.MaxValue)
                        {
                            return false;
                        }

                        TraceHelpers.Assert(elapsedMilliseconds >= 0);

                        realMillisecondsTimeout -= (int)elapsedMilliseconds;
                        if (realMillisecondsTimeout <= 0)
                        {
                            return false;
                        }
                    }

                    return m_eventObj.WaitOne(realMillisecondsTimeout, false);
                }
            }

            return true;
        }

        /// <summary>
        /// WaitAny simulates a Win32-style WaitAny on the set of thin-events.
        /// </summary>
        /// <param name="events">An array of thin-events (null elements permitted)</param>
        /// <returns>The index of the specific event in events that caused us to wake up.</returns>
        internal static int WaitAny(ManualResetEventSlim[] events)
        {
            TraceHelpers.Assert(events != null);

#if DEBUG
            string eventList = "";
            foreach (ManualResetEventSlim e in events)
            {
                if (e != null)
                {
                    eventList = eventList + e.m_id + ", ";
                }
            }
            TraceHelpers.TraceInfo("{0}: ManualResetEventSlim::WaitAny - waiting on {1} events ({2})",
                Thread.CurrentThread.ManagedThreadId, events.Length, eventList);
#endif

            // First, ensure all the events have been set up.
            int nullEvents = 0;
            for (int i = 0; i < events.Length; i++)
            {
                // We permit nulls in the array -- we just skip them.
                if (events[i] == null)
                {
                    nullEvents++;
                    continue;
                }

                // Throw an exception if the event has been disposed.
                events[i].ThrowIfDisposed();

                // If this event has been set, avoid waiting altogether.
                if (events[i].IsSet)
                {
                    return i;
                }

                // Otherwise, lazily initialize the event in preparation for waiting.
                if (events[i].m_eventObj == null)
                {
                    events[i].LazyInitializeEvent();
                }
            }

            // Lastly, accumulate the events in preparation for a true wait.
            WaitHandle[] waitHandles = new WaitHandle[events.Length - nullEvents];
            TraceHelpers.Assert(waitHandles.Length > 0);

            for (int i = 0, j = 0; i < events.Length; i++)
            {
                if (events[i] == null)
                {
                    continue;
                }

                waitHandles[j] = events[i].WaitHandle;
                j++;
            }

            // And finally, issue the real wait.
            int index = WaitHandle.WaitAny(waitHandles);

            // Translate this back into the events array index. The 'waitHandles' array
            // will effectively have the non-null elements "slid down" into the positions
            // from 'events' that contain nulls. We count the number of null handles before
            // the index and add that to get our real position.
            for (int i = 0, j = -1; i < events.Length; i++)
            {
                // If the current event is non-null, increment our translation index.
                if (events[i] != null)
                {
                    j++;

                    // If we found the element, adjust our index and break.
                    if (j == index)
                    {
                        index = i;
                        break;
                    }
                }

                TraceHelpers.Assert(i != events.Length - 1, "didn't find a non-null event");
            }
            TraceHelpers.Assert(events[index] != null, "expected non-null event");

            return index;
        }

        /// <summary>
        /// Gets rid of the internal Win32 event, if one got allocated.
        /// </summary>
        public void Dispose()
        {
            // We will dispose of the event object.  We do this under a lock to protect
            // against the race condition outlined in the Set method above.
            ManualResetEvent eventObj = m_eventObj;
            if (eventObj != null)
            {
                lock (eventObj)
                {
                    eventObj.Close();
                    m_eventObj = null;
                }
            }

            m_disposed = true;
        }

    }


    public class ChessTest
    {
        public static bool Run()
        {
            ManualResetEventSlim s = new ManualResetEventSlim();

            Thread t1 = new Thread(new ThreadStart(() => 
                {
                    s.Set();
                    s.Dispose();
                }));
            Thread t2 = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        s.Wait();
                    }
                    catch (ObjectDisposedException e)
                    {
                        ;
                    }
                }));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            return true;

        }

        public static void Main()
        {
            Run();
        }
    }
}