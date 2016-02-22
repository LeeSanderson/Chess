namespace Microsoft.Concurrency.TestTools.TaskoMeter
{
    internal partial class Stats
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Task = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Start = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.End = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Display = new Microsoft.Concurrency.TestTools.TaskoMeter.Stats.DataGridViewCustomColumn();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.Go = new System.Windows.Forms.Button();
            this.measurex = new System.Windows.Forms.Label();
            this.measurelabel = new System.Windows.Forms.Label();
            this.measures = new System.Windows.Forms.TextBox();
            this.warmups = new System.Windows.Forms.TextBox();
            this.warmuplabel = new System.Windows.Forms.Label();
            this.warmupx = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeight = 30;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Task,
            this.Count,
            this.Duration,
            this.Start,
            this.End,
            this.Display});
            this.dataGridView1.Location = new System.Drawing.Point(1, 56);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1233, 308);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView1_UserDeletingRow);
            this.dataGridView1.SizeChanged += new System.EventHandler(this.dataGridView1_SizeChanged);
            // 
            // Task
            // 
            this.Task.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Task.HeaderText = "Name";
            this.Task.Name = "Task";
            this.Task.ReadOnly = true;
            this.Task.Width = 60;
            // 
            // Count
            // 
            this.Count.HeaderText = "Count";
            this.Count.Name = "Count";
            this.Count.ReadOnly = true;
            this.Count.Width = 40;
            // 
            // Duration
            // 
            this.Duration.HeaderText = "Duration";
            this.Duration.Name = "Duration";
            this.Duration.ReadOnly = true;
            this.Duration.Width = 50;
            // 
            // Start
            // 
            this.Start.HeaderText = "Start";
            this.Start.Name = "Start";
            this.Start.ReadOnly = true;
            this.Start.Width = 50;
            // 
            // End
            // 
            this.End.HeaderText = "End";
            this.End.Name = "End";
            this.End.ReadOnly = true;
            this.End.Width = 50;
            // 
            // Display
            // 
            this.Display.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Display.HeaderText = "";
            this.Display.Name = "Display";
            this.Display.ReadOnly = true;
            // 
            // textBox2
            // 
            this.textBox2.AcceptsTab = true;
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(1173, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(47, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "100";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged_1);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1073, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Display Width [ms]";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar1.LargeChange = 20000;
            this.hScrollBar1.Location = new System.Drawing.Point(322, 33);
            this.hScrollBar1.Maximum = 100000;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(912, 20);
            this.hScrollBar1.SmallChange = 5000;
            this.hScrollBar1.TabIndex = 6;
            this.hScrollBar1.Value = 80000;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.LargeChange = 10000;
            this.trackBar1.Location = new System.Drawing.Point(322, 5);
            this.trackBar1.Maximum = 100000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(746, 45);
            this.trackBar1.SmallChange = 1000;
            this.trackBar1.TabIndex = 6;
            this.trackBar1.TabStop = false;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 100000;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(678, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // Go
            // 
            this.Go.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go.Location = new System.Drawing.Point(270, 7);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(44, 44);
            this.Go.TabIndex = 8;
            this.Go.Text = "Go";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.button1_Click);
            // 
            // measurex
            // 
            this.measurex.AutoSize = true;
            this.measurex.Location = new System.Drawing.Point(114, 38);
            this.measurex.Name = "measurex";
            this.measurex.Size = new System.Drawing.Size(12, 13);
            this.measurex.TabIndex = 11;
            this.measurex.Text = "x";
            // 
            // measurelabel
            // 
            this.measurelabel.AutoSize = true;
            this.measurelabel.Location = new System.Drawing.Point(12, 38);
            this.measurelabel.Name = "measurelabel";
            this.measurelabel.Size = new System.Drawing.Size(48, 13);
            this.measurelabel.TabIndex = 12;
            this.measurelabel.Text = "Measure";
            // 
            // measures
            // 
            this.measures.Location = new System.Drawing.Point(66, 35);
            this.measures.Name = "measures";
            this.measures.Size = new System.Drawing.Size(48, 20);
            this.measures.TabIndex = 13;
            this.measures.TextChanged += new System.EventHandler(this.measures_TextChanged);
            // 
            // warmups
            // 
            this.warmups.Location = new System.Drawing.Point(203, 35);
            this.warmups.Name = "warmups";
            this.warmups.Size = new System.Drawing.Size(48, 20);
            this.warmups.TabIndex = 16;
            this.warmups.TextChanged += new System.EventHandler(this.warmups_TextChanged);
            // 
            // warmuplabel
            // 
            this.warmuplabel.AutoSize = true;
            this.warmuplabel.Location = new System.Drawing.Point(149, 38);
            this.warmuplabel.Name = "warmuplabel";
            this.warmuplabel.Size = new System.Drawing.Size(47, 13);
            this.warmuplabel.TabIndex = 15;
            this.warmuplabel.Text = "Warmup";
            // 
            // warmupx
            // 
            this.warmupx.AutoSize = true;
            this.warmupx.Location = new System.Drawing.Point(251, 38);
            this.warmupx.Name = "warmupx";
            this.warmupx.Size = new System.Drawing.Size(12, 13);
            this.warmupx.TabIndex = 14;
            this.warmupx.Text = "x";
            // 
            // Stats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1232, 365);
            this.Controls.Add(this.warmups);
            this.Controls.Add(this.warmuplabel);
            this.Controls.Add(this.warmupx);
            this.Controls.Add(this.measures);
            this.Controls.Add(this.measurelabel);
            this.Controls.Add(this.measurex);
            this.Controls.Add(this.Go);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Name = "Stats";
            this.Text = "TaskoMeter";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.DataGridViewTextBoxColumn Task;
        private System.Windows.Forms.DataGridViewTextBoxColumn Count;
        private System.Windows.Forms.DataGridViewTextBoxColumn Duration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Start;
        private System.Windows.Forms.DataGridViewTextBoxColumn End;
        private Stats.DataGridViewCustomColumn Display;
        private System.Windows.Forms.Label measurex;
        private System.Windows.Forms.Label measurelabel;
        private System.Windows.Forms.TextBox measures;
        private System.Windows.Forms.TextBox warmups;
        private System.Windows.Forms.Label warmuplabel;
        private System.Windows.Forms.Label warmupx;

    }
}