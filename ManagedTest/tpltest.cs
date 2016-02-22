using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class ChessTest {

    public static bool Run() {
        TPL_Case1();
        //TPL_Case2();
        //PLINQ_Case1();

        return true;
    }

    static int TPL_Case1() {
        int ctr = 0; 
        Task t1 = Task.Factory.StartNew(() => { Interlocked.Increment(ref ctr); });
        Task t2 = Task.Factory.StartNew(() => { Interlocked.Increment(ref ctr); });
        //t1.Wait();
        //t2.Wait();
        Task.WaitAny(t1);
        //WaitHandle wh = ((IAsyncResult)t2).AsyncWaitHandle;
        //wh.WaitOne();
        Task.WaitAny(t2);
        Assert.AreEqual(2, ctr);
        return ctr;
    }

    static void TPL_Case2()
    {
        int ctr = 0;
        Parallel.For(0, 10, (i) => { Interlocked.Increment(ref ctr); });
        Assert.AreEqual(10, ctr);
    }

    static void PLINQ_Case1()
    {
        var plinq_src = Enumerable.Range(0, 10).AsParallel().AsUnordered();
        var plinq = plinq_src.GroupBy(d => d, e => e).ToList();
    }


    public static int Main(string[] args) {
        Console.WriteLine("You should really run this with MCHESS.");
        return ChessTest.Run() ? 0 : -1;
    }

} // class

static class Assert {

    public static void AreEqual(int expected, int actual) {
        if (expected != actual) throw new Exception(string.Format("Assert.AreEaqual(FAIL, {0},{1})", expected, actual));
    }

} // class
