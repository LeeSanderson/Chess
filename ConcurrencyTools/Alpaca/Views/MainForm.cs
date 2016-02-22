using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    internal partial class MainForm : Form
    {

        private Model model;

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


        public MainForm()
        {
            InitializeComponent();

            splitForMenuRibbon.IsSplitterFixed = true;
            splitForMenuRibbon.SplitterWidth = 1;
        }

        private void appControllerTimer_Tick(object sender, EventArgs e)
        {
            Debug.Assert(_isInitialized);

            model.controller.TimerTick();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // first time around, do not actually close, but give controller chance to run
            if (!model.controller.closing)
            {
                model.controller.closing = true;
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        bool _isInitialized;
        internal void Initialize(Model model)
        {
            this.model = model;

            appRibbonStrip.Init(model);
            explorer.Init(model);
            bucketView.Init(model);
            taskList.Init(model);
            bottomTabs.Init(model);
            PopupMenu.Init(model);

            model.SessionInitialized += new ModelEventHandler(model_SessionInitialized);
            model.SelectionUpdated += new ModelEventHandler<Selection.State, Selection.State>(model_SelectionUpdated);
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);

            _isInitialized = true;

            appControllerTimer.Enabled = true;
        }

        private void model_SessionInitialized()
        {
            SelectionContext = null;
        }

        private void model_SelectionUpdated(Selection.State previous, Selection.State current)
        {
            if (SelectionContext == null || !Object.ReferenceEquals(current.SelectedEntity, SelectionContext.Target))
            {
                SelectionContext = current.SelectedEntity == null ? null : new SelectionContext(current.SelectedEntity);
            }
        }

        private void OnSelectionContextChanged()
        {
            appRibbonStrip.SelectionContext = SelectionContext;
        }

        private void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
        }

    }
}
