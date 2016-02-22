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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;


namespace Microsoft.ConcurrencyExplorer
{
    delegate bool RR();
    delegate void CloseForm();
    
    // simple grid visualization of all chess tests run so far
    internal class StateBrowser : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        // private Container components = null;

        internal StateBrowser(DisplayLocationDel loc_del, IThreadView t_viz, Formatter f, GuiController controller)
        {
            tviz = t_viz;
            formatter = f;
            converter = f.GetIndexConverter();
            display_location = loc_del;
            table = new List<List<TaskVarOp>>();
            max_tid = new Dictionary<int,int>();
            focus = -1;
            replay = false;
            current_column = -1;
            current_row = -1;
            mouse_x = mouse_y = -1;
            col_sz = 20;
            row_sz = 20;
            row_max = 0;
            prev_col = -1;
            prev_row = -1;
            ResizeRedraw = true;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            OnResize(System.EventArgs.Empty);
            AutoScroll = true;
            line = new Pen(Color.Black);
            white_pen = new Pen(Color.White, 2);
            this.controller = controller;
            // InitializeComponent();
        }

        private Brush selbrush = new SolidBrush(Palette.getSelectionColor()); 
        private Brush preemptbrush = new SolidBrush(Color.Orange);
        // mark repeat executions (BFS)
        private Brush repeatbrush = new SolidBrush(Color.Orchid);

        private void draw_item(int col, int row, Graphics g, bool isSelected)
        {
            if (table == null || row < 0 ||
                (focus == -1 && (col < 0 || col >= table.Count || row >= table[col].Count)) ||
                (focus != -1 && (focus < 0 || focus >= table.Count || row >= table[focus].Count)))
                return;
            TaskVarOp t = table[(focus == -1) ? col : focus][row];

            Point pt = AutoScrollPosition;

            if (focus == -1)
            {
                draw_entry(t, g, col, row, pt, isSelected);
            }
            else
            {
                for (int tid = 0; tid < max_tid[focus]; tid++)
                    if (tid != t.tid - 1)
                        draw_enabled(t, g, tid, row, pt);
                draw_entry(t, g, t.tid - 1, row, pt, isSelected);
            }
        }

        private static int enabledlevel = 230;
        private static int disabledlevel = 205;
        private static Color enabledcolor = Color.FromArgb(enabledlevel, enabledlevel, enabledlevel);
        private static Color disabledcolor = Color.FromArgb(disabledlevel, disabledlevel, disabledlevel);


        private void draw_enabled(TaskVarOp t, Graphics g, int col, int row, Point pt)
        {
            //int exec = focus + 1;
            //int thread = col + 1;
            EnableInfo info = EnableInfo.Error;
            lock (t.entry)
            {
                info = t.entry.enableinfo[col];
            }
            int left = pt.X + col * col_sz;
            int top = pt.Y + 1 + row * row_sz;
            int right = left + col_sz;
            int bottom = top + row_sz - 1;
            int mid = bottom - (bottom-top)/4;

           if (info == EnableInfo.Disabled)
            {
                g.FillRectangle(new SolidBrush(disabledcolor),
                   left, top, right-left, bottom-top);
            }
            else if (info == EnableInfo.Enabled)
            {
               g.FillRectangle(new SolidBrush(enabledcolor),
                   left, top, right-left, bottom-top);
             }
            else if (info == EnableInfo.Enable)
            {
                // ALT 1 : smooth
                //Brush b = new LinearGradientBrush(new Point(left, top), new Point(left, bottom),
                //                                                      disabledcolor, enabledcolor);
                //g.FillRectangle(b,
                //   left, top, right-left, bottom-top);

                // ALT2 : triangles
                //g.FillPolygon(disabledbrush, new Point[] { 
                //    new Point(left, top), new Point (right, top), new Point(left, bottom)
                //}); 
                //g.FillPolygon(enabledbrush, new Point[] { 
                //    new Point(left, bottom), new Point (right, top), new Point(right, bottom)
                //}); 

                // ALT3 : middle
                g.FillRectangle(new SolidBrush(disabledcolor),
                   left, top, right-left, mid-top);
                g.FillRectangle(Brushes.DarkGray,
                   left, mid, right-left, bottom-mid);
                
            }
            else if (info == EnableInfo.Disable)
            {
                g.FillRectangle(new SolidBrush(enabledcolor),
                   left, top, right - left, mid - top);
                g.FillRectangle(Brushes.DarkGray,
                   left, mid, right - left, bottom - mid);
            }
           else if (info == EnableInfo.Error)
            {
                g.FillRectangle(Brushes.Crimson,
                   left, top, right - left, bottom - top);

            }
        }
        
        private void draw_entry(TaskVarOp t, Graphics g, int col, int row, Point pt, bool isSelected)
        {
            if (t.entry.status == "p")
            {
                // draw preemption marker
                g.FillEllipse(isSelected ? selbrush : preemptbrush, new Rectangle(pt.X + col * col_sz, pt.Y + row * row_sz, col_sz, row_sz));
                return;
            }

            // Draw a pink circle at the bottom of repeat executions (BFS)
            if (t.entry.status.Contains("r"))
            {
                // draw repeat execution marker
                g.FillEllipse(isSelected ? selbrush : repeatbrush, new Rectangle(pt.X + col * col_sz, pt.Y + row * row_sz, col_sz, row_sz));
                return;
            }

            Format format = formatter.GetFormat(t.entry, focus != -1, isSelected);
            Pen line_pen = new Pen(format.lineColor, format.fatBorder ? 3 : 1);
            Brush text_brush = new SolidBrush(format.textColor);

            float div = format.fillBrushes.Length;
            for (int i = format.fillBrushes.Length - 1; i >= 0; --i)
            {
                g.FillRectangle(format.fillBrushes[i],
                   pt.X + (col + i / div) * col_sz, pt.Y + row * row_sz, col_sz / div, row_sz);
            }
            if (focus == -1 || formatter.useOldStyle)
                g.DrawRectangle(line_pen, pt.X + col * col_sz, pt.Y + row * row_sz, col_sz, row_sz);

            if (isSelected && formatter.useOldStyle)
            {
                g.DrawRectangle(white_pen, pt.X + col * col_sz + 3, pt.Y + row * row_sz + 3, col_sz - 5, row_sz - 5);
            }

            if (!System.String.IsNullOrEmpty(format.text))
                g.DrawString(format.text, format.font, text_brush, pt.X + 10 + col * col_sz, pt.Y + row * row_sz + 8);

            if (format.cross)
            {
                g.DrawLine(line_pen, pt.X + col * col_sz, pt.Y + row * row_sz, pt.X + (col + 1) * col_sz, pt.Y + (row + 1) * row_sz);
                g.DrawLine(line_pen, pt.X + col * col_sz, pt.Y + (row + 1) * row_sz, pt.X + (col + 1) * col_sz, pt.Y + row * row_sz);
            }

            if (t.entry.marked)
            {
                int diam = 10;
                if (row_sz < diam) diam = row_sz;
                if (col_sz < diam) diam = col_sz;
                g.FillEllipse(new SolidBrush(Color.Red), new Rectangle(pt.X + col * col_sz, pt.Y + row * row_sz, diam, diam));
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (current_column < 0)
            {
                // no entries yet
                return;
            }
            Point pt = AutoScrollPosition;
            Graphics g = e.Graphics;
            if (focus == -1)
            {
                int col, row, curr_row, curr_col;
                get_col_row_from_scroll(out col, out row);
                curr_col = col;

                // heading
                //while (curr_col <= current_column && (curr_col - col) * col_sz < ClientSize.Width)
                //{
                 //   g.FillRectangle(new SolidBrush(Color.AliceBlue), pt.X + curr_col * col_sz, pt.Y, col_sz, row_sz);
                //    g.DrawRectangle(line, pt.X + curr_col * col_sz, pt.Y, col_sz, row_sz);
                //    Brush wsb = new SolidBrush(Color.Black);
                //    Font f = new Font("Courier New", 8);
                //    g.DrawString("T"+(System.Convert.ToString(curr_col)), f, wsb, pt.X + curr_col * col_sz, pt.Y + 8);
                //    curr_col++;
                //}
                // curr_col = col;

                // table
                while (curr_col <= current_column && (curr_col - col) * col_sz < ClientSize.Width)
                {
                    curr_row = row;
                    while (curr_row < table[curr_col].Count && (curr_row - row) * row_sz < ClientSize.Height)
                    {
                        draw_item(curr_col, curr_row, g, false);
                        curr_row++;
                    }
                    curr_col++;
                }
                draw_selection(g);
                this.AutoScrollMinSize = new Size(current_column * col_sz, row_max * row_sz);
            }
            else
            {
                int row, curr_row;
                get_row_from_scroll(out row);
                curr_row = row;
                while (curr_row < table[focus].Count && (curr_row - row)* row_sz < ClientSize.Height)
                {
                    TaskVarOp elem = table[focus][curr_row];
                    draw_item(elem.tid - 1, curr_row, g, false);
                    curr_row++;
                }
                if (prev_row != -1)
                  draw_item(table[focus][prev_row].tid - 1, prev_row, g, true);
                this.AutoScrollMinSize = new Size(ClientSize.Width, table[focus].Count * row_sz);
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            if (current_column < 0)
            {
                // no entries yet
                return;
            }
            base.OnResize(e);
            if (focus != -1 && max_tid[focus] != 0)
                col_sz = ClientSize.Width / max_tid[focus];
            Invalidate();
        }


        private void get_helper(ref int col, ref int row)
        {
            // put it in bounds
            if (col <= 0)
            {
                col = 0;
            }
            else if (col > current_column)
            {
                col = current_column;
            }
            List<TaskVarOp> r = table[(focus == -1) ? col : focus];
            if (row <= 0)
            {
                row = 0;
            }
            else if (row >= r.Count)
            {
                row = r.Count - 1;
            }
        }

        private void propagate_selection()
        {
            int col, row;
            if (focus != -1)
            {
                col = focus;
                row = prev_row;
            }
            else
            {
                col = prev_col;
                row = prev_row;
            }
            if (col != -1 && row >= 0 && row < table[col].Count)
                controller.SetSelection(table[col][row].entry.seqno);
        }

        private void get_col_row(int x, int y, out int col, out int row)
        {
            Point pt = AutoScrollPosition;
            col = (int)((x - pt.X) / col_sz);
            row = (int)((y - pt.Y) / row_sz);
            get_helper(ref col, ref row);
        }

        private void get_col_row_from_scroll(out int col, out int row)
        {
            Point pt = AutoScrollPosition;
            col = (int)((-pt.X) / col_sz);
            row = (int)((-pt.Y) / row_sz);
            // put it in bounds
            get_helper(ref col, ref row);
        }

        private void get_row_from_scroll(out int row)
        {
            int col_fake = 0;
            Point pt = AutoScrollPosition;
            row = (int)((-pt.Y) / row_sz);
            // put it in bounds
            get_helper(ref col_fake, ref row);
        }

        internal int SetFocus(int f, bool animate)
        {
            if (0 <= f && f <= current_column)
            {
                if (animate)
                {
                    // TODO: animate for smooth transition
                    // 1. grey out non-focus columns.
                    Point pt = AutoScrollPosition;
                    Graphics g = CreateGraphics();
                    int col, row;
                    get_col_row_from_scroll(out col, out row);
                    g.FillRectangle(new SolidBrush(Color.White), pt.X, pt.Y, (f - col) * col_sz, ClientSize.Height);
                    g.FillRectangle(new SolidBrush(Color.White), pt.X + (f - col + 1) * col_sz + 1, pt.Y, ClientSize.Width, ClientSize.Height);
                    System.Threading.Thread.Sleep(500);
                }
                focus = f;
                // compute new width
                if (max_tid[f] != 0)
                    col_sz = ClientSize.Width / max_tid[f];
                //AutoScrollPosition = new Point(0, 0);
                if (prev_row < 0)
                    prev_row = 0;
                else if (prev_row >= table[f].Count)
                    prev_row = table[f].Count - 1;
                this.AutoScrollMinSize = new Size(ClientSize.Width, row_sz * table[f].Count);
                Refresh();
                return max_tid[f];
            }
            return 0;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            if (current_column < 0)
            {
                // no entries yet
                return;
            }
            base.OnScroll(se);
            Refresh();
        }

        private bool off_screen(int col, int row)
        {
            Point pt = AutoScrollPosition;
            int x = pt.X + col*col_sz;
            int y = pt.Y + row*row_sz;
            return (x < 0 || x >= ClientSize.Width || y < 0 || y >= ClientSize.Height);
        }

        private void draw_selection(Graphics g)
        {
            Point pt = AutoScrollPosition;
            if (prev_col != -1 && prev_row != -1)
            {
                draw_item(prev_col, prev_row, g, true);
                g.DrawRectangle(new Pen(Palette.getSelectionColor()), pt.X + prev_col * col_sz, pt.Y, col_sz, row_sz * table[prev_col].Count);
            }
        }

        private void move_selection(int col, int row)
        {
            Point pt = AutoScrollPosition;
            Graphics g = CreateGraphics();
            if (prev_col != -1)
            {
                // erase
                draw_item(prev_col, prev_row, g, false);
                g.DrawRectangle(line, pt.X + prev_col * col_sz, pt.Y, col_sz, row_sz * table[prev_col].Count);
            }
            prev_col = col;
            prev_row = row;
            draw_selection(g);
            // is the selection off the screen?
            if (off_screen(col, row)) {
                // center it
                AutoScrollPosition =
                    new Point(col * col_sz - (ClientSize.Width / (2 * col_sz)) * col_sz,
                              row * row_sz - (ClientSize.Height / (2 * row_sz)) * row_sz);
                Invalidate();
            }
        }

        private void move_selection_focus(int row)
        {
            Point pt = AutoScrollPosition;
            Graphics g = CreateGraphics();
            if (prev_row != -1)
            {
                draw_item(table[focus][prev_row].tid - 1, prev_row, g, false);
            }
            prev_row = row;
            draw_item(table[focus][row].tid-1, row, g, true);

            if (replay)
            {
                // algorithm: go forward to find next event for each task
                // TODO: make this increment for the common case of stepping forward/backward by one row
                Dictionary<int, TaskVarOp> global_state = new Dictionary<int, TaskVarOp>();
                global_state.Add(table[focus][row].tid, table[focus][row]);
                int curr = row + 1;
                int width;
                max_tid.TryGetValue(focus, out width);
                while (curr < table[focus].Count && global_state.Count < width)
                {
                    int tid = table[focus][curr].tid;
                    if (!global_state.ContainsKey(tid))
                    {
                        global_state.Add(tid, table[focus][curr]);
                    }
                    curr++;
                }
                // now display the consistent global state
                foreach (TaskVarOp t in global_state.Values)
                {
                    display_location(t);
                }
            }

            if (off_screen(0,row)) {
                  AutoScrollPosition =
                    new Point(pt.X,
                              row * row_sz - (ClientSize.Height / (2 * row_sz)) * row_sz);
                Invalidate();
            }
        }

        internal void SetSelection(int entry)
        {
            Coord c;
            if (entries.TryGetValue(entry, out c) && c != null)
            {
                if (focus != -1)
                {
                    if (c.col != prev_col)
                        SetFocus(c.col, false);
                    if (c.row != prev_row)
                        move_selection_focus(c.row);
                }
                else
                {
                    if (c.col != prev_col || c.row != prev_row)
                    move_selection(c.col, c.row);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (current_column < 0)
            {
                // no entries yet
                return;
            }
            Focus();
            int x = mouse_x, y = mouse_y;
            mouse_x = e.X;
            mouse_y = e.Y;
            if (x != mouse_x || y != mouse_y)
            {
                int col, row;
                get_col_row(e.X, e.Y, out col, out row);
                if (focus == -1)
                {
                    move_selection(col, row);
                    propagate_selection();
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (current_column < 0)
            {
                // no entries yet
                return;
            }

            mouse_x = e.X;
            mouse_y = e.Y;

            int col, row;
            get_col_row(e.X, e.Y, out col, out row);
            if (e.Button == MouseButtons.Left)
            {
                if (focus == -1)
                {
                    SetFocus(col,true);
                    propagate_selection();
                }
                else
                {
                    move_selection_focus(row);
                    propagate_selection();
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }


        protected override void OnKeyDown(KeyEventArgs kea)
        {
            if (current_column < 0)
            {
                // no entries yet
                return;
            }
            switch (kea.KeyCode)
            {
                case Keys.Q:
                    controller.QueueCommand(new GuiController.Command(delegate()
                    {
                        controller.ShutDown();
                    }));
                    return;
                case Keys.F:
                    //ConcurrencyExplorer newview = new ConcurrencyExplorer(false);
                    //new IdentityTransformation().process(ce.getEventDB(), newview);
                    return;
                case Keys.I:
                    {
                        int col = (focus == -1) ? prev_col : focus;
                        int row = prev_row;
                        if (col >= 0 && col < table.Count && row >= 0 && row < table[col].Count)
                        {
                            System.Console.WriteLine(table[col][row].entry.ToString());
                            Refresh();
                        }
                        return;
                    }
                case Keys.Space:
                    {
                        int col = (focus == -1) ? prev_col : focus;
                        int row = prev_row;
                        if (col >= 0 && col < table.Count && row >= 0 && row < table[col].Count)
                        {
                            controller.ToggleMark(table[col][row].entry.seqno);
                            Refresh();
                        }
                    }
                    return;
                case Keys.Return:
                    if (focus == -1 && prev_col != -1)
                    {
                        SetFocus(prev_col, true);
                        propagate_selection();
                    }
                    return;
                case Keys.Back:
                    {
                        if (focus != -1)
                        {
                            int col = focus;
                            focus = -1;
                            col_sz = 20; row_sz = 20;
                            move_selection(col, 0);
                            propagate_selection();
                            Refresh();
                        }
                        return;
                    }
                case Keys.End:
                    return;
                case Keys.Down:
                    if (focus == -1)
                    {
                        propagate_selection();
                    }
                    else
                    {
                        move_selection_focus(prev_row == table[focus].Count-1 ? prev_row : prev_row + 1);
                        propagate_selection();
                    }
                    return;
                case Keys.PageDown:
                    return;
                case Keys.Up:
                    if (focus == -1)
                    {
                        int col = prev_col, row = prev_row - 1;
                        get_helper(ref col, ref row);
                        move_selection(col, row);
                        propagate_selection();
                    }
                    else
                    {
                        move_selection_focus(prev_row != 0 ? prev_row - 1 : 0);
                        propagate_selection();
                    }
                    return;
                case Keys.PageUp:
                    return;
                case Keys.Right:
                    if (focus == -1 && prev_col < current_column)
                    {
                        int col = prev_col+1, row = prev_row;
                        get_helper(ref col, ref row);
                        move_selection(col, row);
                        propagate_selection();
                    }
                    else if (focus != -1 && focus < current_column)
                    {
                        SetFocus(focus + 1, false);
                        propagate_selection();
                    }
                    return;
                case Keys.Left:
                    if (focus == -1)
                    {
                        int col = prev_col-1, row = prev_row;
                        get_helper(ref col, ref row);
                        move_selection(col, row);
                        propagate_selection();
                    }
                    else if (focus > 0)
                    {
                        SetFocus(focus - 1, false);
                        propagate_selection();
                    }
                    return;
                case Keys.S:
                    if (focus == -1)
                    {
                        if (col_sz >= 4 && row_sz >= 4)
                        {
                            col_sz -= 2;
                            row_sz -= 2;
                            Invalidate();
                        }
                    }
                    return;
                case Keys.A:
                    if (focus == -1 && col_sz <= 100)
                    {
                        col_sz += 2;
                        row_sz += 2;
                        Invalidate();
                    }
                    return;
            }
        }

        internal void SetReplay()
        {
            replay = true;
            current_row = -1;
        }

        internal void NewColumn()
        {
            current_row = -1;
            if (!replay)
            {
                current_column++;
                table.Add(new List<TaskVarOp>());
                max_tid[current_column] = 0;
            }
            else
            {
               // TODO: should check for consistency
            }
            //AutoScrollPosition = new Point(-current_column * col_sz, 0);
        }

        internal bool NewColumnEntry(TaskVarOp tvo)
        {
            current_row++;
            entries[tvo.entry.seqno] = new Coord(current_column, current_row);
            if (replay && focus != -1)
            {
                // TODO: should check for consistency
                // highlight current place
                // draw_item(new SolidBrush(Color.Yellow), tid - 1, current_row, CreateGraphics());
                // check to see if we have a set a break point at current entry
                // System.Threading.Thread.Sleep(500);
                //return selected[current_row];
                return false;
            }
            else
            {
                // we are expanding the test
		// swap these two lines for the two below if you use the repeat execution marks (BFS)
                //if (current_row + 1 > row_max)
                //    row_max = current_row + 1;
                if (current_row > row_max)
                    row_max = current_row;
                table[current_column].Add(tvo);
                if (tvo.tid > max_tid[current_column])
                    max_tid[current_column] = tvo.tid;

                // paint the current one
                draw_item(current_column, current_row, CreateGraphics(), false);
                // make sure we are visible (scroll to focus area)
                return false;
            }
        }

        internal void InvisibleEntry(int tid)
        {
            if (replay && focus != -1)
            {
            }
            else
            {
                if (tid > max_tid[current_column])
                    max_tid[current_column] = tid;
            }
        }
        
        internal IThreadView tviz;
        internal Formatter formatter;
        internal IIndexConverter converter;
        internal GuiController controller;

        internal int col_sz, row_sz;
        internal int mouse_x, mouse_y;
        internal int current_column;
        internal int current_row;
        internal int row_max;
        internal List<List<TaskVarOp>> table;
        internal Dictionary<int,int> max_tid;
        internal Pen line;
        internal Pen white_pen;
        internal int prev_col, prev_row;

        internal int focus;
        internal bool replay;

        internal DisplayLocationDel display_location;

        // map entry ids to col, row
        private class Coord { internal int col, row; internal Coord(int c, int r) { col = c; row = r; } }
        private Dictionary<int, Coord> entries = new Dictionary<int,Coord>();

    }
}