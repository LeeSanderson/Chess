/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Collections.Generic;


namespace Test34 {
public class ChessTest {
   public static bool Run() {
        RWLockSlimTest.Main(null);
	return true;
   }
}

public class RWLockSlimTest {
	public static void Main(string[] args) {
		SynchronizedCache cache = new SynchronizedCache();
		Thread myThread = new Thread(MyThreadProc);
		myThread.Start(cache);
		cache.AccessProperties();
		cache.Add(3, "foo");
		cache.AddWithTimeout(6, "bar", 10);
		cache.AddOrUpdate(3, "bar");
		myThread.Join();
	}

	public static void MyThreadProc(object o) {
		SynchronizedCache cache = (SynchronizedCache) o;
		cache.Add(4, "bar");
		cache.AddWithTimeout(5, "bar", 10);
		cache.Delete(3);
	}
}

public class SynchronizedCache
{
    private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
    private Dictionary<int, string> innerCache = new Dictionary<int, string>();

    public int AccessProperties()
    {
	int t;
	t = cacheLock.CurrentReadCount + cacheLock.RecursiveReadCount + cacheLock.RecursiveUpgradeCount + cacheLock.RecursiveWriteCount;
	t = t + cacheLock.WaitingReadCount + cacheLock.WaitingWriteCount + cacheLock.WaitingUpgradeCount;
	
	bool b;
	b = cacheLock.IsReadLockHeld && cacheLock.IsUpgradeableReadLockHeld && cacheLock.IsWriteLockHeld;

	return b ? t : 0;
    }

    public string Read(int key)
    {
        cacheLock.EnterReadLock();
        try
        {
            return innerCache[key];
        }
        finally
        {
            cacheLock.ExitReadLock();
        }
    }

    public void Add(int key, string value)
    {
        cacheLock.EnterWriteLock();
        try
        {
            innerCache.Add(key, value);
        }
        finally
        {
            cacheLock.ExitWriteLock();
        }
    }

    public bool AddWithTimeout(int key, string value, int timeout)
    {
        if (cacheLock.TryEnterWriteLock(timeout))
        {
            try
            {
                innerCache.Add(key, value);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public AddOrUpdateStatus AddOrUpdate(int key, string value)
    {
        cacheLock.EnterUpgradeableReadLock();
        try
        {
            string result = null;
            if (innerCache.TryGetValue(key, out result))
            {
                if (result == value)
                {
                    return AddOrUpdateStatus.Unchanged;
                }
                else
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        innerCache[key] = value;
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                    return AddOrUpdateStatus.Updated;
                }
            }
            else
            {
                cacheLock.EnterWriteLock();
                try
                {
                    innerCache.Add(key, value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
                return AddOrUpdateStatus.Added;
            }
        }
        finally
        {
            cacheLock.ExitUpgradeableReadLock();
        }
    }

    public void Delete(int key)
    {
        cacheLock.EnterWriteLock();
        try
        {
            innerCache.Remove(key);
        }
        finally
        {
            cacheLock.ExitWriteLock();
        }
    }

    public enum AddOrUpdateStatus
    {
        Added,
        Updated,
        Unchanged
    };
}
}