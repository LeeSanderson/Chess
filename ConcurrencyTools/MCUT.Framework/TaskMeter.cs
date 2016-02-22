using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    public class TaskMeter
    {

        /// <summary>
        /// Represents an individual interval.
        /// </summary>
        public class Interval
        {
            public long Start;
            public long End;
            public int Tid;
            internal Interval Next;

            public Interval(long start, long end, int tid)
            {
                this.Start = start;
                this.End = end;
                this.Tid = tid;
                this.Next = null;
            }
        }


        // lock-free map from tid to start
        struct Entry
        {
            public volatile int thread;
            public long start;
        }

        // static data
        public static List<TaskMeter> _Tasks = new List<TaskMeter>();
        public static volatile System.Diagnostics.Stopwatch _StopWatch;

        // Instance Variables
        public Interval Intervals = null;
        volatile Entry[][] entrylistlist = new Entry[][] { new Entry[64] };
        public int seqnum;

        #region Constructors

        /// <summary>
        /// Creates a new instance with a specified color to use in visual displays.
        /// </summary>
        /// <param name="name">The display name of this meter.</param>
        /// <param name="color">
        /// A predefined color from the enumeration TaskMeter.Color
        /// </param>
        public TaskMeter(string name, Color color)
        {
            Name = name;
            Enabled = true;
            VisualColor = color;

            if (MeasurementIsUnderway())
                throw new Exception("Must not create TaskMeter while measuring.");

            lock (_Tasks)
            {
                _Tasks.Add(this);
                seqnum = _Tasks.Count;
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">The display name of this meter.</param>
        public TaskMeter(string name)
            // Compute a 'random' color based on the name of the meter.
            // The hash code will assert that the same color is used for each meter w/the same name.
            : this(name, (Color)(Math.Abs(name.GetHashCode()) % (lastcolor - firstcolor + 1)))
        { }

        #endregion

        /// <summary>
        /// The display name of this instance.
        /// </summary>
        public string Name { get; private set; }


        public Color VisualColor { get; private set; }
        public TaskMeter ReferenceMeter { get; set; }
        public bool Enabled { get; set; }

        /// <summary>
        /// Checks if a measurement is in progress.
        /// </summary>        
        public static bool MeasurementIsUnderway()
        {
            return _StopWatch != null;
        }


        /// <summary>
        /// Starts a new interval.
        /// </summary>
        public void Start()
        {
            if (!Enabled || _StopWatch == null)
                return;

            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Entry[] chunk;
            int pos = find(tid, out chunk);
            if (chunk[pos].start != 0)
                throw new InvalidOperationException("Start() called twice on same thread before calling End().");
            chunk[pos].start = _StopWatch.ElapsedTicks;
            if (chunk[pos].start == 0)  // Fix just in case no ticks have elapsed in the StopWatch
                chunk[pos].start = 1;
            System.Diagnostics.Contracts.Contract.Assert(chunk[pos].start > 0);
        }

        /// <summary>
        /// Ends an interval previously started with a call to <see cref="Start"/>.
        /// </summary>
        /// <returns>The number of ticks elapsed since the call to <see cref="Start"/>.</returns>
        public long End()
        {
            if (!Enabled || _StopWatch == null)
                return 0;

            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Entry[] chunk;
            int pos = find(tid, out chunk);
            if (chunk[pos].start == 0)
                throw new InvalidOperationException("End() called with no preceding call to Start() on same thread.");

            long elapsedTicks = _StopWatch.ElapsedTicks;
            Interval i = new Interval(chunk[pos].start, elapsedTicks, tid);
            while (true)
            {

                i.Next = Intervals;
                if (i.Next == System.Threading.Interlocked.CompareExchange(ref Intervals, i, i.Next))
                    break;
            }
            chunk[pos].start = 0;

            return elapsedTicks;
        }

        public void ClearData()
        {
            Intervals = null;
        }

        #region Wrappers
        public void Measure(Action a)
        {
            if (Enabled)
            {
                Start();
                try
                {
                    a();
                }
                finally
                {
                    End();
                }
            }
        }
        public void Measure(int repetitions, Action a)
        {
            if (Enabled)
                for (int i = 0; i < repetitions; i++)
                    Measure(a);
        }
        public Action WrapAction(Action a)
        {
            if (!Enabled)
                return a;
            else
                return delegate() {
                    Start();
                    try
                    {
                        a();
                    }
                    finally
                    {
                        End();
                    }
                };
        }
        public Action<T> WrapAction<T>(Action<T> a)
        {
            if (!Enabled)
                return a;
            else
                return delegate(T t) {
                    Start();
                    try
                    {
                        a(t);
                    }
                    finally
                    {
                        End();
                    }
                };
        }
        public Func<T> WrapFunc<T>(Func<T> a)
        {
            if (!Enabled)
                return a;
            else
                return delegate() {
                    Start();
                    try
                    {
                        return a();
                    }
                    finally
                    {
                        End();
                    }
                };
        }
        public Func<A, T> WrapFunc<A, T>(Func<A, T> a)
        {
            if (!Enabled)
                return a;
            else
                return delegate(A arg) {
                    Start();
                    try
                    {
                        return a(arg);
                    }
                    finally
                    {
                        End();
                    }
                };
        }
        public Func<A, B, T> WrapFunc<A, B, T>(Func<A, B, T> a)
        {
            if (!Enabled)
                return a;
            else
                return delegate(A arg, B arg2) {
                    Start();
                    try
                    {
                        return a(arg, arg2);
                    }
                    finally
                    {
                        End();
                    }
                };
        }
        public Func<A, B, C, T> WrapFunc<A, B, C, T>(Func<A, B, C, T> a)
        {
            if (!Enabled)
                return a;
            else
                return delegate(A arg, B arg2, C arg3) {
                    Start();
                    try
                    {
                        return a(arg, arg2, arg3);
                    }
                    finally
                    {
                        End();
                    }
                };
        }
        #endregion

        /// <summary>
        /// Gets an enumeration of all the intervals captured by this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Interval> GetIntervals()
        {
            Interval cur = Intervals;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Next;
            }
        }

        int find(int tid, out Entry[] outchunk)
        {
            while (true)
            {
                for (int list = 0; list < entrylistlist.Length; list++)
                {
                    Entry[] chunk = entrylistlist[list];
                    for (int pos = 0; pos < chunk.Length; pos++)
                    {
                        if (chunk[pos].thread == tid)
                        {
                            outchunk = chunk;
                            return pos;
                        }
                        else if (chunk[pos].thread == 0)
                        {
                            chunk[pos].thread = tid;
                            outchunk = chunk;
                            return pos;
                        }
                    }
                }
                Expand();
            }
        }
        void Expand()
        {
            lock (this)
            {
                Entry[][] newlistlist = new Entry[entrylistlist.Length + 1][];
                for (int i = 0; i < entrylistlist.Length; i++)
                    newlistlist[i] = entrylistlist[i];
                newlistlist[entrylistlist.Length] = new Entry[entrylistlist[entrylistlist.Length - 1].Length * 2];
                entrylistlist = newlistlist;
            }
        }

        private const int firstcolor = 28;
        private const int lastcolor = 167;

        public enum Color // enumeration codes match System.Drawing.KnownColor
        {
            AliceBlue = 28,
            AntiqueWhite = 29,
            Aqua = 30,
            Aquamarine = 31,
            Azure = 32,
            Beige = 33,
            Bisque = 34,
            Black = 35,
            BlanchedAlmond = 36,
            Blue = 37,
            BlueViolet = 38,
            Brown = 39,
            BurlyWood = 40,
            CadetBlue = 41,
            Chartreuse = 42,
            Chocolate = 43,
            Coral = 44,
            CornflowerBlue = 45,
            Cornsilk = 46,
            Crimson = 47,
            Cyan = 48,
            DarkBlue = 49,
            DarkCyan = 50,
            DarkGoldenrod = 51,
            DarkGray = 52,
            DarkGreen = 53,
            DarkKhaki = 54,
            DarkMagenta = 55,
            DarkOliveGreen = 56,
            DarkOrange = 57,
            DarkOrchid = 58,
            DarkRed = 59,
            DarkSalmon = 60,
            DarkSeaGreen = 61,
            DarkSlateBlue = 62,
            DarkSlateGray = 63,
            DarkTurquoise = 64,
            DarkViolet = 65,
            DeepPink = 66,
            DeepSkyBlue = 67,
            DimGray = 68,
            DodgerBlue = 69,
            Firebrick = 70,
            FloralWhite = 71,
            ForestGreen = 72,
            Fuchsia = 73,
            Gainsboro = 74,
            GhostWhite = 75,
            Gold = 76,
            Goldenrod = 77,
            Gray = 78,
            Green = 79,
            GreenYellow = 80,
            Honeydew = 81,
            HotPink = 82,
            IndianRed = 83,
            Indigo = 84,
            Ivory = 85,
            Khaki = 86,
            Lavender = 87,
            LavenderBlush = 88,
            LawnGreen = 89,
            LemonChiffon = 90,
            LightBlue = 91,
            LightCoral = 92,
            LightCyan = 93,
            LightGoldenrodYellow = 94,
            LightGray = 95,
            LightGreen = 96,
            LightPink = 97,
            LightSalmon = 98,
            LightSeaGreen = 99,
            LightSkyBlue = 100,
            LightSlateGray = 101,
            LightSteelBlue = 102,
            LightYellow = 103,
            Lime = 104,
            LimeGreen = 105,
            Linen = 106,
            Magenta = 107,
            Maroon = 108,
            MediumAquamarine = 109,
            MediumBlue = 110,
            MediumOrchid = 111,
            MediumPurple = 112,
            MediumSeaGreen = 113,
            MediumSlateBlue = 114,
            MediumSpringGreen = 115,
            MediumTurquoise = 116,
            MediumVioletRed = 117,
            MidnightBlue = 118,
            MintCream = 119,
            MistyRose = 120,
            Moccasin = 121,
            NavajoWhite = 122,
            Navy = 123,
            OldLace = 124,
            Olive = 125,
            OliveDrab = 126,
            Orange = 127,
            OrangeRed = 128,
            Orchid = 129,
            PaleGoldenrod = 130,
            PaleGreen = 131,
            PaleTurquoise = 132,
            PaleVioletRed = 133,
            PapayaWhip = 134,
            PeachPuff = 135,
            Peru = 136,
            Pink = 137,
            Plum = 138,
            PowderBlue = 139,
            Purple = 140,
            Red = 141,
            RosyBrown = 142,
            RoyalBlue = 143,
            SaddleBrown = 144,
            Salmon = 145,
            SandyBrown = 146,
            SeaGreen = 147,
            SeaShell = 148,
            Sienna = 149,
            Silver = 150,
            SkyBlue = 151,
            SlateBlue = 152,
            SlateGray = 153,
            Snow = 154,
            SpringGreen = 155,
            SteelBlue = 156,
            Tan = 157,
            Teal = 158,
            Thistle = 159,
            Tomato = 160,
            Turquoise = 161,
            Violet = 162,
            Wheat = 163,
            White = 164,
            WhiteSmoke = 165,
            Yellow = 166,
            YellowGreen = 167,
        }

    }
}
