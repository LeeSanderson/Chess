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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    public partial class StreamBox : UserControl
    {
        public StreamBox()
        {
            InitializeComponent();
        }

        /// <summary>Gets or sets the title of this box.</summary>
        public string Title
        {
            get { return groupBox1.Text; }
            set { groupBox1.Text = value; }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    value = null;
                if (value == FilePath)
                    return;

                _filePath = value;
                OnFilePathChanged();
            }
        }

        private void OnFilePathChanged()
        {
            textBox1.Text = null;
        }

        internal void RefreshFile()
        {
            if (!String.IsNullOrEmpty(_filePath))
            {
                textBox1.Text = "(reading file)";
                textBox1.Refresh(); // do this right away to make user notice what's happening

                if (File.Exists(_filePath))
                {
                    textBox1.ForeColor = Color.Black;
                    try
                    {
                        Stream filestream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        StreamReader filereader = new StreamReader(filestream);
                        string text = filereader.ReadToEnd();
                        filestream.Close();
                        if (text.Length > 32767)
                        {
                            int trunclen = text.Length - 32767;
                            text = text.Substring(trunclen);
                            text = "(file too large - removing the first " + trunclen + " characters)" + Environment.NewLine
                                + text;
                        }
                        textBox1.Text = text;
                    }
                    catch
                    {
                        textBox1.Text = "(could not read file)";
                    }
                }
                else
                {
                    textBox1.Text = "(file not found)";
                }
            }
            else
            {
                textBox1.Text = "";
            }

            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            RefreshFile();
        }
    }
}
