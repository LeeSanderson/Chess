using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.Alpaca.Actions;
using System.Diagnostics;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    internal partial class AppRibbonStripControl : UserControl
    {

        public enum MenuTab { Home, Debug, Chess, Tools };

        /// <summary>The application model initialized with this control.</summary>
        private Model model;

        #region Properties

        private SelectionContext _selectionContext;
        [Browsable(false)]
        public SelectionContext SelectionContext
        {
            get { return _selectionContext; }
            set
            {
                if (value != _selectionContext)
                {
                    _selectionContext = value;
                    OnSelectionContextChanged();
                }
            }
        }

        private MenuTab _selectedMenuTab;
        [Category("Design")]
        [DefaultValue(MenuTab.Home)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MenuTab SelectedMenuTab
        {
            get { return _selectedMenuTab; }
            set
            {
                // Make sure we're selecting a visible tab
                if (!IsTabVisible(value))
                    value = MenuTab.Home;

                _selectedMenuTab = value;
                tabControl1.SelectedTab = MenuTabToTabPage(value);
            }
        }

        private Dictionary<MenuTab, bool> _menuTabsVisibility;
        public bool IsTabVisible(MenuTab menu)
        {
            return DesignMode || _menuTabsVisibility[menu];
        }

        #endregion

        #region Constructor

        public AppRibbonStripControl()
        {
            InitializeComponent();

            tbpgHome.Tag = MenuTab.Home;
            tbpgDebug.Tag = MenuTab.Debug;
            tbpgChess.Tag = MenuTab.Chess;
            tbpgTools.Tag = MenuTab.Tools;

            PopulateComboBox_Preemptions();
            PopulateComboBox_MaxSchedules();
            PopulateComboBox_MaxRunTime();
            PopulateComboBox_AppOpt_MaxConcurrentTasks();

            _menuTabsVisibility = new Dictionary<MenuTab, bool>();
            foreach (var tab in Enum.GetValues(typeof(MenuTab)))    // So when we add a new tab enum
                _menuTabsVisibility[(MenuTab)tab] = false;
            // These are the defaults (This may not be needed if they're done inside of BindToContext)
            _menuTabsVisibility[MenuTab.Home] = true;
            _menuTabsVisibility[MenuTab.Tools] = true;

            BindToContext();    // Binds to the null context
        }

        #endregion

        #region Initialization

        bool _isInitialized;
        internal void Init(Model model)
        {
            this.model = model;

            // Bind the application options
            chkAppOpt_ConfirmAutoRefresh.Checked = model.session.Entity.RuntimeState.ConfirmAutoRefresh;
            chkAppOpt_EnableRegressionTestingMode.Checked = model.session.Entity.RuntimeState.EnableRegressionTestingMode;
            chkAppOpt_UseGoldenObsFiles.Checked = model.session.Entity.RuntimeState.UseGoldenObservationFiles;
            cmboAppOpt_MaxConcurrentTasks.SelectedItem = model.session.Entity.RuntimeState.MaxConcurrentTasks;

            dlgOpenTestsContainer.InitialDirectory = model.session.Entity.FolderPath;

            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);

            _isInitialized = true;
        }

        private void PopulateComboBox_Preemptions()
        {
            var datasource = new List<KeyValuePair<string, int>>();
            datasource.Add(new KeyValuePair<string, int>("0", 0));
            datasource.Add(new KeyValuePair<string, int>("1", 1));
            datasource.Add(new KeyValuePair<string, int>("2", 2));
            datasource.Add(new KeyValuePair<string, int>("3", 3));
            datasource.Add(new KeyValuePair<string, int>("Unlimited", 100000));
            cmboPreemptions.DataSource = datasource;
        }

        private void PopulateComboBox_MaxSchedules()
        {
            var datasource = new List<KeyValuePair<string, int>>();
            datasource.Add(new KeyValuePair<string, int>("5", 5));
            datasource.Add(new KeyValuePair<string, int>("10", 10));
            datasource.Add(new KeyValuePair<string, int>("100", 100));
            datasource.Add(new KeyValuePair<string, int>("1000", 1000));
            datasource.Add(new KeyValuePair<string, int>("10000", 10000));
            datasource.Add(new KeyValuePair<string, int>("100000", 100000));
            datasource.Add(new KeyValuePair<string, int>("Unlimited", 0));
            cmboMaxSchedules.DataSource = datasource;
        }

        private void PopulateComboBox_MaxRunTime()
        {
            var datasource = new List<KeyValuePair<string, int>>();
            datasource.Add(new KeyValuePair<string, int>("5 sec", 5));
            datasource.Add(new KeyValuePair<string, int>("10 sec", 10));
            datasource.Add(new KeyValuePair<string, int>("30 sec", 30));
            datasource.Add(new KeyValuePair<string, int>("1 min", 60));
            datasource.Add(new KeyValuePair<string, int>("5 min", 300));
            datasource.Add(new KeyValuePair<string, int>("10 min", 600));
            datasource.Add(new KeyValuePair<string, int>("30 min", 1800));
            datasource.Add(new KeyValuePair<string, int>("1 hr", 3600));
            datasource.Add(new KeyValuePair<string, int>("2 hr", 7200));
            datasource.Add(new KeyValuePair<string, int>("3 hr", 10800));
            datasource.Add(new KeyValuePair<string, int>("Unlimited", 0));
            cmboMaxRunTime.DataSource = datasource;
        }

        private void PopulateComboBox_AppOpt_MaxConcurrentTasks()
        {
            cmboAppOpt_MaxConcurrentTasks.DataSource = new[] { 1, 2, 4, 8, 12, 16 };
        }

        #endregion

        #region Event Handlers for the application model

        private void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
        }

        #endregion

        private void OnSelectionContextChanged()
        {
            BindToContext();
        }

        private void BindToContext()
        {
            if (DesignMode)
            {
                cmboPreemptions.SelectedText = "Unlimited";
                cmboMaxSchedules.SelectedText = "Unlimited";
                cmboMaxRunTime.SelectedText = "Unlimited";

                SelectedMenuTab = MenuTab.Home;
            }

            System.Diagnostics.Debug.WriteLine(this.GetType().Name + ".BindToContext");

            if (SelectionContext == null)
            {
                //// Clear all context-sensitive items
                //grpbxTestsHome.Visible = false;
                //grpbxTestsContainerHome.Visible = false;
                //grpbxBounds.Visible = false;
                //_menuTabsVisibility[MenuTab.Debug] = false;
                //_menuTabsVisibility[MenuTab.Chess] = false;
                //grpbxAdvanced.Visible = false;
                //BindMenuTabVisibility();
                return;
            }

            // Reset parent controls to visible or else setting a child Visible=true won't do anything
            grpbxAdvanced.Visible = true;

            btnOpenDirectory.Visible = false;
            btnOpenCmdShell.Visible = false;


            grpbxTestsContainerHome.Visible = SelectionContext.Target is TestGroupingEntity;
            grpbxTestsHome.Visible = SelectionContext.Test != null;

            bool testUsesMChess = SelectionContext.Test != null && (SelectionContext.Test is MChessTestEntity || SelectionContext.Test is MChessBasedTestEntity);
            grpbxBounds.Visible = testUsesMChess;
            grpbxDiagnostics.Visible = testUsesMChess;
            _menuTabsVisibility[MenuTab.Chess] = testUsesMChess;

            BindToTestsContainer();

            // Test Runs
            var testRun = SelectionContext.TaskRun;
            if (testRun != null && !String.IsNullOrEmpty(testRun.TaskFolderPath))
            {
                btnOpenDirectory.Visible = true;
                btnOpenDirectory.Tag = testRun.TaskFolderPath;
                btnOpenCmdShell.Visible = true;
                btnOpenCmdShell.Tag = testRun.TaskFolderPath;
            }

            BindEntitiesWithSourceFile();

            btnDeleteSelected.Visible = SelectionContext.Target.IsDeleteable();
            btnDeleteSelected.Tag = SelectionContext.Target;

            //grpbxAdvanced.Visible = grpbxAdvanced.Controls[0].Controls.Cast<Control>().Any(c=>c.Visible);
            BindMenuTabVisibility();
        }

        private void BindMenuTabVisibility()
        {
            return;
            var prevSelTab = tabControl1.SelectedTab;
            tabControl1.SuspendLayout();
            // TODO: Make this smarter so Tools is the last menu and hidden tabs get inserted before Tools
            //foreach (var menu in Enum.GetValues(typeof(MenuTab)).Cast<MenuTab>())
            //{
            //}
            foreach (var kvp in _menuTabsVisibility)
            {
                var tbpg = MenuTabToTabPage(kvp.Key);
                if (kvp.Value)
                {
                    if (!tabControl1.TabPages.Contains(tbpg))
                        tabControl1.TabPages.Add(tbpg);
                }
                else
                {
                    if (tabControl1.TabPages.Contains(tbpg))
                        tabControl1.TabPages.Remove(tbpg);
                }
            }
            tabControl1.ResumeLayout();

            if (prevSelTab != null)
                SelectedMenuTab = (MenuTab)prevSelTab.Tag;
        }

        private void BindToTestsContainer()
        {
            btnClearTasksFromTestContainer.Visible = SelectionContext.Target.CanDeleteTasks();
            btnClearTasksFromTestContainer.Tag = SelectionContext.Target;

            btnDeleteTestsContainer.Visible = SelectionContext.Target.IsDeleteable();
            btnDeleteTestsContainer.Tag = SelectionContext.Target;
        }

        private void BindEntitiesWithSourceFile()
        {
            var hsf = SelectionContext.Target as IHasTestContainerSourceFile;
            if (SelectionContext.TestProject != null && SelectionContext.TestProject.TestAssembly != null)
                hsf = SelectionContext.TestProject.TestAssembly;

            // Refresh & open source file location
            btnRefreshTestContainer.Visible = false;
            btnOpenSourceFileLocation.Visible = false;
            string srcFilePath = hsf == null ? null : hsf.SourceFilePath;
            if (hsf != null && !String.IsNullOrEmpty(srcFilePath))
            {
                if (hsf.SupportsRefresh)
                {
                    btnRefreshTestContainer.Visible = true;
                    btnRefreshTestContainer.Tag = hsf;
                }

                btnOpenSourceFileLocation.Visible = true;
                btnOpenSourceFileLocation.Tag = hsf;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab != null)
                _selectedMenuTab = (MenuTab)tabControl1.SelectedTab.Tag;
        }

        private TabPage MenuTabToTabPage(MenuTab menu)
        {
            switch (menu)
            {
                case MenuTab.Home: return tbpgHome;
                case MenuTab.Debug: return tbpgDebug;
                case MenuTab.Chess: return tbpgChess;
                case MenuTab.Tools: return tbpgTools;
                default:
                    throw new NotImplementedException("MenuTab not implemented: " + menu);
            }
        }

        #region Event Handlers for MenuTab: Home - File

        private void btnOpenTestsContainer_Click(object sender, EventArgs e)
        {
            if (dlgOpenTestsContainer.ShowDialog() == DialogResult.OK)
            {
                model.session.Entity.AddTestContainer(dlgOpenTestsContainer.FileName);
            }
        }

        private void btnDeleteSession_Click(object sender, EventArgs e)
        {
            DeleteCommand command = new DeleteCommand(false, true, true);
            model.controller.AddNewCommand(command);
        }

        private void btnClearAllTasks_Click(object sender, EventArgs e)
        {
            model.controller.AddNewCommand(new DeleteCommand(true, true, true));
        }

        #endregion

        #region Event Handlers for MenuTab: Home - etc

        private void btnRunTest_Click(object sender, EventArgs e)
        {

        }

        private void btnRunAllTests_Click(object sender, EventArgs e)
        {

        }

        private void btnReproTestResult_Click(object sender, EventArgs e)
        {

        }

        private void btnReproLastSchedule_Click(object sender, EventArgs e)
        {

        }

        private void btnViewInCE_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Event Handlers for MenuTab: Debug

        private void btnDebugTest_Click(object sender, EventArgs e)
        {

        }

        private void btnDebugLastSchedule_Click(object sender, EventArgs e)
        {

        }

        private void btnDebugResult_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Event Handlers for MenuTab: Options

        private void chkAppOpt_ConfirmAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitialized) return;

            model.session.Entity.RuntimeState.ConfirmAutoRefresh = chkAppOpt_ConfirmAutoRefresh.Checked;
        }

        private void chkAppOpt_EnableRegressionTestingMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitialized) return;

            model.session.Entity.RuntimeState.EnableRegressionTestingMode = chkAppOpt_EnableRegressionTestingMode.Checked;
        }

        private void chkAppOpt_UseGoldenObsFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitialized) return;

            model.session.Entity.RuntimeState.UseGoldenObservationFiles = chkAppOpt_UseGoldenObsFiles.Checked;
        }

        private void cmboAppOpt_MaxConcurrentTasks_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!_isInitialized) return;

            int maxConcurrentTasks = (int)cmboAppOpt_MaxConcurrentTasks.SelectedValue;
            model.session.Entity.RuntimeState.MaxConcurrentTasks = maxConcurrentTasks;
            model.tasksController.MaxConcurrentTasks = maxConcurrentTasks;
        }

        #endregion

        #region Event Handlers for MenuTab: Tools -> Advanced

        private void btnPrintXmlToConsole_Click(object sender, EventArgs e)
        {
            Debug.Assert(SelectionContext != null && SelectionContext.Target != null);
            System.Console.WriteLine(SelectionContext.Target.DataElement.ToString());
        }

        private void btnOpenSourceFileLocation_Click(object sender, EventArgs e)
        {
            var hsf = ((Control)sender).Tag as IHasTestContainerSourceFile;
            Debug.Assert(hsf != null, "Should've been set to a valid IHasTestContainerSourceFile.");

            model.controller.AddNewCommand(BrowseCommand.OpenExplorer(hsf.SourceFilePath, true));
        }

        private void btnOpenCmdShell_Click(object sender, EventArgs e)
        {
            var workingDir = ((Control)sender).Tag as string;
            Debug.Assert(!String.IsNullOrEmpty(workingDir ), "btn.Tag should've been set to the working directory.");

            model.controller.AddNewCommand(BrowseCommand.OpenCmdShell(workingDir));
        }

        private void btnOpenDirectory_Click(object sender, EventArgs e)
        {
            var folderPath = ((Control)sender).Tag as string;
            Debug.Assert(!String.IsNullOrEmpty(folderPath), "btn.Tag should've been set to the folder path.");

            model.controller.AddNewCommand(BrowseCommand.OpenExplorer(folderPath, false));
        }

        #endregion

    }
}
