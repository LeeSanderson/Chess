using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.TaskoMeter
{
    internal class Display
    {
        // back pointers
        internal Stats owner;
        internal TaskMeter meter;

        internal Display refdisplay;

        internal class Box
        {
            public long start;
            public long end;
            public int tid;
            public long vpos;
            public Box(long s, long e, int t) { start = s; end = e; tid = t; }
        }

        // calculated fields
        internal List<Box> allboxes = new List<Box>();
        internal List<Box> visibleboxes = new List<Box>();
        internal Brush brush;
        internal int divisions = 1;
        internal double calculatedduration = 0.0;

        // displayed columns
        internal string task = "";
        internal string number = "";
        internal string duration = "";
        internal string start = "";
        internal string end = "";
        internal string speedup = "";

        internal object[] Format()
        {
            return new object[] { task, number, duration, start, end, this };
        }
        public override string ToString()
        {
            return ""; // we are using this with text box grid view cell, where no text should be displ.
        }

        public Display(Stats owner, TaskMeter meter)
        {
            this.owner = owner;
            this.meter = meter;
            task = meter.Name;

            KnownColor c = (KnownColor)  ((int) meter.VisualColor);
            brush = new SolidBrush(Color.FromKnownColor(c));
        }

        public void ClearData()
        {
            meter.ClearData();
        }

        public void RefreshData()
        {
            allboxes.Clear();
            foreach (TaskMeter.Interval iv in meter.GetIntervals())
                allboxes.Add(new Box(iv.Start, iv.End, iv.Tid));
            Recalculate();
        }


        private string FormatTime(double val)
        {
            return val.ToString();
        }

        public void Recalculate()
        {
            visibleboxes.Clear();
            List<long> filledto = new List<long>();


            SortedList<int, int> tidmap = new SortedList<int, int>();

            foreach (Box box in allboxes)
                if (box.end > owner.leftticks && box.start < owner.leftticks + owner.widthticks)
                {
                    visibleboxes.Add(box);
                    int vpos;
                    if (!tidmap.TryGetValue(box.tid, out vpos))
                    {
                        vpos = tidmap.Count;
                        tidmap.Add(box.tid, vpos);
                    }
                    box.vpos = vpos;
                }

            divisions = tidmap.Count;

            number = visibleboxes.Count.ToString();
            calculatedduration = 0.0;
            duration = "";
            start = "";
            end = "";

            if (visibleboxes.Count > 0)
            {
                if (visibleboxes.Count == 1)
                {
                    start = FormatTime(owner.scale * visibleboxes[0].start);
                    end = FormatTime(owner.scale * visibleboxes[0].end);
                    calculatedduration = owner.scale * (visibleboxes[0].end - visibleboxes[0].start);
                    duration = FormatTime(calculatedduration);
                }
                else
                {
                    long sum = 0;
                    foreach (Box iv in visibleboxes)
                        sum += iv.end - iv.start;
                    calculatedduration = (owner.scale * sum) / visibleboxes.Count;
                    duration = FormatTime(calculatedduration);
                }
            }
        }

        public void DrawAllBoxes(Stats.DataGridViewCustomCell cell)
        {
            foreach (Box iv in visibleboxes)
            {
                double top = iv.vpos * (1.0 / divisions);
                double bottom = (iv.vpos + 1) * (1.0 / divisions);
                cell.DrawBox(iv.start, iv.end, top, bottom, Pens.Black, brush);
            }
        }

    }
}
