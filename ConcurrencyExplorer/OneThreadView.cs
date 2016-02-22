/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Microsoft.ConcurrencyExplorer
{
    internal partial class OneThreadView : UserControl
    {
        private string title;
        private int last_file = -1;

        internal OneThreadView()
        {
            InitializeComponent();
            rtb_init(this.richTextBox1);
        }

        internal void SetTitle(string s)
        {
            title = s;
            this.groupBox1.Text = s;
            this.listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBox1.DrawItem += new DrawItemEventHandler(ListBox1_DrawItem);
        }

        private void ListBox1_DrawItem(object sender,
            System.Windows.Forms.DrawItemEventArgs e)
        {
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // Determine the color of the brush to draw each item based 
            // on whether or not we have a source file
            /*
            switch (e.Index)
            {
                case 0:
                    myBrush = Brushes.Red;
                    break;
                case 1:
                    myBrush = Brushes.Orange;
                    break;
                case 2:
                    myBrush = Brushes.Purple;
                    break;
            }
            */

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(this.listBox1.Items[e.Index].ToString(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        internal static void rtb_init(RichTextBox rtb)
        {
            rtb.HideSelection = false;
            rtb.WordWrap = false;
            rtb.Multiline = true;
            rtb.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            rtb.DetectUrls = false;
            rtb.Dock = System.Windows.Forms.DockStyle.Fill;
            rtb.Location = new System.Drawing.Point(0, 0);
            rtb.Name = "richTextBox1";
            rtb.ReadOnly = true;
            rtb.Size = new System.Drawing.Size(486, 482);
            rtb.TabIndex = 0;
            rtb.Text = "(no information available)";
        }

        /*
        private void color_line(string line, int start)
        {
            var text = this.richTextBox1;
            // Backup the users current selection point.
            // Split the line into tokens.
            Regex r = new Regex("([ \\t{}();])");
            string[] tokens = r.Split(line);
            int index = start;
            foreach (string token in tokens)
            {
                text.SelectionStart = index;
                text.SelectionLength = token.Length;
                text.SelectionColor = Color.Black;

                // Check for a comment.
                if (token == "//" || token.StartsWith("//"))
                {
                    // Find the start of the comment and then extract the whole comment.
                    int index2 = line.IndexOf("//");
                    string comment = line.Substring(index2, line.Length - index2);
                    text.SelectionStart = start+index2;
                    text.SelectionLength = comment.Length;
                    text.SelectionColor = Color.SeaGreen;
                    break;
                }

                // Check whether the token is a keyword. 
                String[] keywords = { "public", "void", "using", "static", "class" };
                for (int i = 0; i < keywords.Length; i++)
                {
                    if (keywords[i] == token)
                    {
                        // Apply alternative color and font to highlight keyword. 
                        text.SelectionColor = Color.Blue;
                        break;
                    }
                }
                index += token.Length;
            }
        }
        */

        internal void GotoFileLine(Entry e, bool selected)
        {
            current_entry = e;

            if (e.record.stack != null && e.record.stack.Count > 0)
            {
                int topWithValidFile = 0;
                while (topWithValidFile < e.record.stack.Count)
                {
                    if (e.record.stack[topWithValidFile].file != null &&
                        e.record.stack[topWithValidFile].line > 0)
                    {
                        break;
                    }
                    topWithValidFile++;
                }
                if (topWithValidFile >= e.record.stack.Count)
                    return;
                var tos = e.record.stack[topWithValidFile];
                if (tos.file != null && tos.line > 0)
                {
                    //if (topWithValidFile != 0)
                    //    show_file_line(tos.file, tos.line - 1, selected); // this is a temporary fix
                    //else
                    show_file_line(tos.file, tos.line, selected);
                }

                // update the stack view
                // TODO: grey out elements with no source code
                // Shutdown the painting of the ListBox as items are added
                var stack = this.listBox1;
                stack.Update();
                stack.Items.Clear();
                for (int i = 0; i < e.record.stack.Count; i++)
                {
                    stack.Items.Add(e.record.stack[i].proc);
                }
                // select the
                stack.SetSelected(topWithValidFile, true);
                // Allow the ListBox to repaint and display the new items.
                stack.EndUpdate();
            }
        }

        internal Entry current_entry;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb_click = (ListBox)sender;
            MoveLine(lb_click.SelectedIndex);
        }

        private void show_file_line(FileData file, int line, bool selected)
        {
            // Make sure we're looking at the correct file's lines
            if (file.Id != last_file)
            {
                richTextBox1.Lines = file.Lines;
                last_file = file.Id;
            }

            string[] split = file.Name.Split(new char[] { '\\' });
            groupBox1.Text = title + ": " + split[split.Length - 1] + ", line " + line;
            /*
            if (!selected)
            {
                richTextBox1.SelectionBackColor = Color.LightGray;
            }*/

            // BUG FIX: Item 7197 - It's possible that the 'line' in the stack trace doesn't actually exist anymore in the current state of the source file.
            if ((line - 1) < file.Lines.Length)
                richTextBox1.Select(file.GetLineOffset(line - 1, "\n"), file.Lines[line - 1].Length); // Assumes that richTextBox1 has the Lines specified in the file.
            else
                richTextBox1.Select(0, 0);

            /*if (!selected)
            {
                richTextBox1.SelectionBackColor = Color.LightBlue;
                richTextBox1.SelectionLength = 0;
            }
            else
            {
                richTextBox1.SelectionBackColor = Color.LightGray;
            }*/
        }

        private void MoveLine(int f)
        {
            if (current_entry != null)
            {
                lock (current_entry.record)
                {
                    if (current_entry.record.stack != null && current_entry.record.stack.Count > 0)
                    {
                        int topWithValidFile = 0;
                        while (topWithValidFile < current_entry.record.stack.Count)
                        {
                            if (current_entry.record.stack[topWithValidFile].file != null &&
                                current_entry.record.stack[topWithValidFile].line > 0)
                            {
                                break;
                            }
                            topWithValidFile++;
                        }

                        if (f + topWithValidFile >= current_entry.record.stack.Count)
                            return;

                        var frame = current_entry.record.stack[f + topWithValidFile];
                        if (frame.file != null && frame.line > 0)
                        {
                            //if (topWithValidFile != 0 && f == 0)
                            //    show_file_line(frame.file, frame.line-1, true); // this is a temporary fix
                            //else
                            show_file_line(frame.file, frame.line, true);
                        }
                    }
                }
            }
        }
    }
}
