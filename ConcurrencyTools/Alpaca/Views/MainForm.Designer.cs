namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.StatusStrip statusStrip1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.appControllerTimer = new System.Windows.Forms.Timer(this.components);
            this.splitForMenuRibbon = new System.Windows.Forms.SplitContainer();
            this.appRibbonStrip = new Microsoft.Concurrency.TestTools.Alpaca.Views.AppRibbonStripControl();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.explorer = new Microsoft.Concurrency.TestTools.Alpaca.Explorer();
            this.taskList = new Microsoft.Concurrency.TestTools.Alpaca.TaskList();
            this.bucketView = new Microsoft.Concurrency.TestTools.Alpaca.BucketView();
            this.bottomTabs = new Microsoft.Concurrency.TestTools.Alpaca.BottomTabs();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.splitForMenuRibbon)).BeginInit();
            this.splitForMenuRibbon.Panel1.SuspendLayout();
            this.splitForMenuRibbon.Panel2.SuspendLayout();
            this.splitForMenuRibbon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new System.Drawing.Point(0, 540);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(784, 22);
            statusStrip1.TabIndex = 0;
            // 
            // appControllerTimer
            // 
            this.appControllerTimer.Interval = 2000;
            this.appControllerTimer.Tick += new System.EventHandler(this.appControllerTimer_Tick);
            // 
            // splitForMenuRibbon
            // 
            this.splitForMenuRibbon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitForMenuRibbon.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitForMenuRibbon.Location = new System.Drawing.Point(0, 0);
            this.splitForMenuRibbon.Margin = new System.Windows.Forms.Padding(0);
            this.splitForMenuRibbon.Name = "splitForMenuRibbon";
            this.splitForMenuRibbon.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitForMenuRibbon.Panel1
            // 
            this.splitForMenuRibbon.Panel1.Controls.Add(this.appRibbonStrip);
            this.splitForMenuRibbon.Panel1MinSize = 92;
            // 
            // splitForMenuRibbon.Panel2
            // 
            this.splitForMenuRibbon.Panel2.Controls.Add(this.mainSplitContainer);
            this.splitForMenuRibbon.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitForMenuRibbon.Size = new System.Drawing.Size(784, 540);
            this.splitForMenuRibbon.SplitterDistance = 100;
            this.splitForMenuRibbon.TabIndex = 4;
            // 
            // appRibbonStrip
            // 
            this.appRibbonStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appRibbonStrip.Location = new System.Drawing.Point(0, 0);
            this.appRibbonStrip.MinimumSize = new System.Drawing.Size(615, 92);
            this.appRibbonStrip.Name = "appRibbonStrip";
            this.appRibbonStrip.Size = new System.Drawing.Size(784, 100);
            this.appRibbonStrip.TabIndex = 4;
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.splitContainer2);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.bottomTabs);
            this.mainSplitContainer.Size = new System.Drawing.Size(778, 430);
            this.mainSplitContainer.SplitterDistance = 205;
            this.mainSplitContainer.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.bucketView);
            this.splitContainer2.Size = new System.Drawing.Size(778, 205);
            this.splitContainer2.SplitterDistance = 527;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.explorer);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.taskList);
            this.splitContainer3.Size = new System.Drawing.Size(527, 205);
            this.splitContainer3.SplitterDistance = 191;
            this.splitContainer3.TabIndex = 0;
            // 
            // explorer
            // 
            this.explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorer.Location = new System.Drawing.Point(0, 0);
            this.explorer.Name = "explorer";
            this.explorer.Size = new System.Drawing.Size(191, 205);
            this.explorer.TabIndex = 0;
            // 
            // taskList
            // 
            this.taskList.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.taskList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskList.Location = new System.Drawing.Point(0, 0);
            this.taskList.Name = "taskList";
            this.taskList.Size = new System.Drawing.Size(332, 205);
            this.taskList.TabIndex = 0;
            // 
            // bucketView
            // 
            this.bucketView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bucketView.Location = new System.Drawing.Point(0, 0);
            this.bucketView.Name = "bucketView";
            this.bucketView.Size = new System.Drawing.Size(247, 205);
            this.bucketView.TabIndex = 0;
            // 
            // bottomTabs
            // 
            this.bottomTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomTabs.Location = new System.Drawing.Point(0, 0);
            this.bottomTabs.Name = "bottomTabs";
            this.bottomTabs.Size = new System.Drawing.Size(778, 221);
            this.bottomTabs.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitForMenuRibbon);
            this.Controls.Add(statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(630, 600);
            this.Name = "MainForm";
            this.Text = "Alpaca";
            this.splitForMenuRibbon.Panel1.ResumeLayout(false);
            this.splitForMenuRibbon.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitForMenuRibbon)).EndInit();
            this.splitForMenuRibbon.ResumeLayout(false);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer appControllerTimer;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private Alpaca.Explorer explorer;
        private Alpaca.TaskList taskList;
        private Alpaca.BucketView bucketView;
        private Alpaca.BottomTabs bottomTabs;
        private System.Windows.Forms.SplitContainer splitForMenuRibbon;
        private AppRibbonStripControl appRibbonStrip;

    }
}