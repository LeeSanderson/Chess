/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class Model : IEntityModel
    {

        class WatchedEntity
        {

            private FileSystemWatcher _watcher;
            private bool _isWaitingForFolderToBeCreated;
            private string _existingFolderPath;
            Stack<string> _foldersNeedingToBeCreated;

            public WatchedEntity(IHasTestContainerSourceFile entity)
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                if (String.IsNullOrEmpty(entity.SourceFilePath))
                    throw new ArgumentException("SourceFilePath is not specified for the entity.", "entity");

                HasSourceFileEntity = entity;
                Entity = (EntityBase)entity;

                string folderPath = Path.GetDirectoryName(HasSourceFileEntity.SourceFilePath);
                if (Directory.Exists(folderPath))
                {
                    CreateFSWForFile();
                }
                else
                {
                    _foldersNeedingToBeCreated = new Stack<string>();
                    string pathRoot = Path.GetPathRoot(folderPath);
                    do
                    {
                        _foldersNeedingToBeCreated.Push(Path.GetFileName(folderPath));
                        folderPath = Path.GetDirectoryName(folderPath);
                    } while (!folderPath.Equals(pathRoot, StringComparison.OrdinalIgnoreCase)
                        && !Directory.Exists(folderPath)
                        );
                    _existingFolderPath = folderPath;

                    _watcher = new FileSystemWatcher() {
                        Path = _existingFolderPath,
                        //Filter = _foldersNeedingToBeCreated.Peek(),
                        NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size,
                        //IncludeSubdirectories = true,
                    };
                    //_watcher.NotifyFilter |= NotifyFilters.CreationTime;

                    _isWaitingForFolderToBeCreated = true;

                    _watcher.Created += watcher_Folder;
                    _watcher.Changed += watcher_Folder;
                    _watcher.Renamed += watcher_Folder;
                    //_watcher.Deleted += watcher_Folder;
                    Debug.WriteLine("_existingFolderPath=" + _existingFolderPath, "watcher_Folder");
                }
            }

            private void CreateFSWForFile()
            {
                _watcher = new FileSystemWatcher() {
                    Path = Path.GetDirectoryName(HasSourceFileEntity.SourceFilePath),
                    Filter = Path.GetFileName(HasSourceFileEntity.SourceFilePath),
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                    IncludeSubdirectories = false,
                };

                _watcher.Changed += watcher_Changed;
            }

            public EntityBase Entity { get; private set; }
            public IHasTestContainerSourceFile HasSourceFileEntity { get; private set; }
            public string SourceFilePath { get { return HasSourceFileEntity.SourceFilePath; } }
            public bool Enabled { get; private set; }

            void watcher_Folder(object sender, FileSystemEventArgs e)
            {
                if (_isWaitingForFolderToBeCreated && Directory.Exists(_existingFolderPath))
                {
                    _watcher.EnableRaisingEvents = false;

                    // See if our folder/s got created
                    while (_foldersNeedingToBeCreated.Count != 0)
                    {
                        string nextExistingFolderPath = Path.Combine(_existingFolderPath, _foldersNeedingToBeCreated.Peek());
                        if (!Directory.Exists(nextExistingFolderPath))
                            break;

                        _existingFolderPath = nextExistingFolderPath;
                        _foldersNeedingToBeCreated.Pop();
                    }

                    if (_foldersNeedingToBeCreated.Count == 0)
                    {
                        Debug.Assert(Directory.Exists(Path.GetDirectoryName(HasSourceFileEntity.SourceFilePath)));
                        _isWaitingForFolderToBeCreated = false;

                        _watcher.Dispose();
                        _watcher = null;
                        CreateFSWForFile();

                        // Just in case we missed it
                        if (File.Exists(HasSourceFileEntity.SourceFilePath))
                            OnSourceFileChanged();
                    }
                    else
                    {
                        // Update to the latest folder that does exist
                        _watcher.Path = _existingFolderPath;
                        Debug.WriteLine("_existingFolderPath=" + _existingFolderPath, "watcher_Folder");
                    }

                    _watcher.EnableRaisingEvents = true;
                }
            }

            void watcher_Changed(object sender, FileSystemEventArgs e)
            {
                if ((e.ChangeType & (WatcherChangeTypes.Changed | WatcherChangeTypes.Created)) != 0)
                    OnSourceFileChanged();
            }

            public event EventHandler SourceFileChanged;
            protected void OnSourceFileChanged()
            {
                if (SourceFileChanged != null)
                    SourceFileChanged(this, EventArgs.Empty);
            }

            internal void StartWatching()
            {
                _watcher.EnableRaisingEvents = true;
                Enabled = true;
            }

            internal void StopWatching()
            {
                _watcher.EnableRaisingEvents = false;
                Enabled = false;
            }

        }

        internal Session session;
        internal XDocument xdocument;
        internal Selection selection;
        internal Runs runs;
        internal CommandController controller;
        internal AppTaskController tasksController;

        private List<WatchedEntity> _watchedEntitySourceFiles;

        // we raise this event on changes to the xml session tree

        /// <summary>Raised only once per application run.</summary>
        internal event ModelEventHandler SessionInitialized;

        /// <summary>The event raised when an entity is about to be changed.</summary>
        internal event ModelEntityEventHandler<EntityChangeEventArgs> EntityChanging;
        /// <summary>The event raised when an entity has been changed.</summary>
        internal event ModelEntityEventHandler<EntityChangeEventArgs> EntityChanged;

        // we raise these before/after making batch changes to the xml session tree
        internal event ModelEventHandler BeginUpdate;
        internal event ModelEventHandler EndUpdate;

        // we raise this if the user selects an entity
        internal event ModelEventHandler<Selection.State, Selection.State> SelectionUpdated;

        internal Microsoft.Concurrency.TestTools.Alpaca.Views.MainForm mainForm;

        internal Model()
        {
            this.EntityBuilder = new EntityBuilder(this);

            selection = new Selection(this);
            runs = new Runs(this);
            controller = new CommandController(this);

            _watchedEntitySourceFiles = new List<WatchedEntity>();
        }

        public EntityBuilderBase EntityBuilder { get; private set; }

        ISessionEntity IEntityModel.Session { get { return this.session.Entity; } }

        internal void StartSession(string sessionFilePath, string backupFilename, IEnumerable<Command> batchcommands)
        {
            Debug.Assert(session == null, "The session has already been started.");

            // Setup the session
            session = new Session(this, sessionFilePath, backupFilename);

            //// Try to load any test projects that haven't been given the chance
            //var testProjectsToLoad = session.Entity.Descendants<TestProjectEntity>()
            //    .Where(tp => !tp.IsProjectFileLoaded && !tp.HasProjectLoadError)
            //    .ToArray(); // Since we'll be changing the entity try when we register
            //foreach (var testProj in testProjectsToLoad)
            //    testProj.TryLoadProjectFile();

            // Register test assemblies
            foreach (var testAssy in session.Entity.Descendants<TestAssemblyEntity>())
                session.Entity.RegisterTestAssembly(testAssy);

            // Setup the task controller and add existing tasks (from runs) to it.
            tasksController = new AppTaskController(this, session.Entity.BaseTasksFolderPath) {
                MaxConcurrentTasks = session.Entity.RuntimeState.MaxConcurrentTasks,
            };
            tasksController.TaskCompleted += new AppTaskEventHandler(tasksController_TaskCompleted);
            // Register all tasks with the controller.
            foreach (var run in session.Entity.DescendantRuns())
                tasksController.AddTask(run.Task);

            // Add a delegate for keeping track of changes to session xml
            EventHandler<XObjectChangeEventArgs> setupDel = (x, args) => session.MarkSessionChanged();
            xdocument.Changed += setupDel;

            // Initialize the main form
            mainForm = new Views.MainForm();
            mainForm.Initialize(this);
            //mainForm.Show();

            RegisterEntitiesWithSourceFiles(session.Entity);
            ProcessIncludeElements(session.Entity);

            controller.Init(batchcommands);

            session.AutoSave();
            OnSessionInitialized();

            // Only setup the change notifications after the whole model has been setup
            xdocument.Changed -= setupDel;  // Remove the old one we had just for keeping track of changes
            xdocument.Changing += xdocument_Changing;
            xdocument.Changed += xdocument_Changed;
        }

        void tasksController_TaskCompleted(AppTask task, EventArgs e)
        {
            if (task.Status == AppTaskStatus.Error)
            {
                string errMsg;
                string stackTrace;
                if (task.ErrorEx != null)
                {
                    errMsg = task.ErrorEx.Message;
                    stackTrace = task.ErrorEx.StackTrace;
                }
                else// if(task.XError != null)  // One should be specified.
                {
                    errMsg = (string)task.XError.Element(XNames.ErrorMessage);
                    stackTrace = (string)task.XError.Element(XNames.ErrorStackTrace);
                }

                Debug.WriteLine("Task {0} failed for the following reason:\n{1}\nStack Trace: {2}", task.ID, errMsg, stackTrace);
                System.Windows.Forms.MessageBox.Show(
                       String.Format("Task {0} failed for the following reason:\n{1}", task.ID, errMsg)
                       , "Task Error"
                       , System.Windows.Forms.MessageBoxButtons.OK
                       , System.Windows.Forms.MessageBoxIcon.Error
                       );
            }
        }

        // session updates
        internal void OnSessionInitialized()
        {
            SessionInitialized();
        }

        #region Entity Change notification methods

        private XElement GetChangedElement(XObject xobj, XObjectChange xchange, out EntityChange chg)
        {
            chg = XChangeToEntityChange(xchange);

            XElement x = xobj as XElement;

            // If not an element, get the parent (assuming it's an element node)
            if (x == null)
            {
                // Handle the case where the parent is null because the node is being removed
                if (chg == EntityChange.Remove && xobj.Parent == null)
                {
                    Debug.Assert(xobj == _changingXObj);
                    xobj = _changingXObjOrigParent;
                }
                else // navigate normally
                    xobj = xobj.Parent;

                x = xobj as XElement;
                chg = EntityChange.Modified;
            }

            return x;
        }

        /// <summary>Finds the changed entity if possible.</summary>
        /// <param name="x">The changed element.</param>
        /// <param name="chg">The applicable change made to the entity.</param>
        private XElement FindChangedEntityElement(XElement x, ref EntityChange chg)
        {
            // Find the owning entity (and create if performing an Add)
            XElement xentity = x;
            while (xentity != null)
            {
                if (EntityBuilder.IsEntity(xentity.Name))
                    break;

                // Navigate up the xtree
                if (chg == EntityChange.Remove && xentity.Parent == null)
                {
                    Debug.Assert(xentity == _changingXObj, "If chg==Remove, than x must be the original node being changed because GetChangedElement changes the chg to Modified otherwise.");
                    xentity = _changingXObjOrigParent as XElement;
                }
                else
                    xentity = xentity.Parent;   // null if ObjectChange == Add

                // Since we're going up, then the entity change changes to Modified because
                // either it's child is being added, removed, modified
                chg = EntityChange.Modified;
            }

            return xentity;
        }

        XObject _changingXObj;
        XObject _changingXObjOrigParent;
        private void xdocument_Changing(object o, XObjectChangeEventArgs e)
        {
            XObject xobj = (XObject)o;
            Debug.Assert(_changingXObj == null, "Model._changingNode wasn't set back to null from the previous change to the xdocument. This may occur while modifying an entity's xml while it's being loaded.");
            _changingXObj = xobj;
            _changingXObjOrigParent = xobj.Parent;

            EntityChange chg;
            XElement x = GetChangedElement(xobj, e.ObjectChange, out chg);
            if (x == null)
            {
                //Debug.WriteLine(xobj, "xdocument_Changing - Non XElement node");
                return;
            }

            // Find the owning entity (and create if performing an Add)
            XElement xentity = FindChangedEntityElement(x, ref chg);
            if (xentity == null && e.ObjectChange == XObjectChange.Add)
                return; // There's no way to know what the entity is if the element itself is being added
            Debug.Assert(xentity != null, "This would only happen if the element isn't under the root element (session).");
            EntityBase entity = xentity.GetEntity();
            if (entity == null)
            {
                Debug.Assert(chg == EntityChange.Add, "The element doesn't have it's Entity bound. It should've been done when the element was added to the document tree.");

                // Then the entity needs to be created
                entity = EntityBuilder.CreateEntityAndBindToElement(xentity);
                // NOTE: When the element is being added, it's Parent property isn't set.
            }

            if (entity.RaisesEntityChangedEvents)
                OnEntityChanging(entity, new EntityChangeEventArgs(chg, x, e.ObjectChange));

            // backwards compatibility:
            //SessionChanging(x, e);
        }

        private void xdocument_Changed(object o, XObjectChangeEventArgs e)
        {
            // Tell the session that we've changed
            session.MarkSessionChanged();

            XObject xobj = (XObject)o;
            Debug.Assert(_changingXObj != null, "Model._changingNode didn't get set during the xdocument.Changing event.");
            Debug.Assert(xobj == _changingXObj, "The changed node isn't the same node that was changed during the xdocument.Changing event.");

            EntityChange chg;
            XElement x = GetChangedElement(xobj, e.ObjectChange, out chg);
            if (x == null)
            {
                Debug.WriteLine(xobj, "xdocument_Changed - Non XElement node");
                return;
            }

            // Find the owning entity (and create if performing an Add)
            XElement xentity = FindChangedEntityElement(x, ref chg);
            Debug.Assert(xentity != null, "This would only happen if the element isn't under the root element (session).");
            EntityBase entity = xentity.GetEntity();
            Debug.Assert(entity != null, "The element's entity instance should've been bound during the Changing event.");

            // Before doing anything that may run outside code, reset our temp state
            _changingXObj = null;
            _changingXObjOrigParent = null;

            if (chg == EntityChange.Add)
                EntityBuilder.SetParentEntity(entity);

            // And load the children entities too
            if (e.ObjectChange == XObjectChange.Add)
                entity.LoadChildren(true);

            if (entity.RaisesEntityChangedEvents)
                OnEntityChanged(entity, new EntityChangeEventArgs(chg, x, e.ObjectChange));
        }

        private void OnEntityChanging(EntityBase entity, EntityChangeEventArgs args)
        {
            Debug.Assert(args.EntityChange != EntityChange.Add || args.EntityChange != EntityChange.Remove || !(entity is SessionEntity), "Shouldn't be able to remove or add the Session element/entity.");

            if (EntityChanging != null)
                EntityChanging(entity, args);
        }

        private void OnEntityChanged(EntityBase entity, EntityChangeEventArgs args)
        {
            Debug.Assert(args.EntityChange != EntityChange.Add || args.EntityChange != EntityChange.Remove || !(entity is SessionEntity), "Shouldn't be able to remove or add the Session element/entity.");

            OnPreEntityChanged(entity, args);

            if (EntityChanged != null)
                EntityChanged(entity, args);
        }

        private EntityChange XChangeToEntityChange(XObjectChange xchange)
        {
            switch (xchange)
            {
                case XObjectChange.Add: return EntityChange.Add;
                case XObjectChange.Remove: return EntityChange.Remove;
                case XObjectChange.Value: return EntityChange.Modified;
                case XObjectChange.Name: throw new InvalidOperationException("Changing the name of an XObject is not supported in the model.");
                default: throw new NotImplementedException("Unhandled XObjectChage: " + xchange);
            }
        }

        #endregion

        private void OnPreEntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            // Do our own logic for when an entity is changed
            //Debug.WriteLine("Model_EntityChanged. ChangeType: {2,-8} Type: {0,-20} DisplayName: {1}", entity.GetType().Name, entity.DisplayName, e.EntityChange);

            if (e.EntityChange == EntityChange.Remove)
            {
                UnregisterEntitiesWithSourceFiles(entity);
            }
            else if (e.EntityChange == EntityChange.Add)
            {
                RegisterEntitiesWithSourceFiles(entity);

                foreach (var testAssy in entity.DescendantsAndSelf<TestAssemblyEntity>())
                    session.Entity.RegisterTestAssembly(testAssy);

                ProcessIncludeElements(entity);
            }
        }

        private void ProcessIncludeElements(EntityBase entity)
        {
            // Auto-import any import xml elements
            // Need to convert to an array so we're immune to changes to the xml tree while iterating
            var includesToLoad = entity.DescendantsAndSelf<IncludeEntity>().ToArray();
            foreach (var include in includesToLoad)
                controller.AddNewCommand(new LoadIncludeCommand(include, true));
        }

        #region Entity Source File Watching

        private void RegisterEntitiesWithSourceFiles(EntityBase source)
        {
            foreach (var hsf in source.DescendantsAndSelf().OfType<IHasTestContainerSourceFile>())
            {
                if (hsf.SupportsRefresh && !String.IsNullOrEmpty(hsf.SourceFilePath) && ((EntityBase)hsf).GetSessionProperty_DetectChanges())
                {
                    var we = new WatchedEntity(hsf);

                    we.SourceFileChanged += new EventHandler(watchedEntity_SourceFileChanged);

                    _watchedEntitySourceFiles.Add(we);
                    we.StartWatching();
                    Debug.WriteLine("Registered entity w/source file: " + we.SourceFilePath);
                }
            }
        }

        private void UnregisterEntitiesWithSourceFiles(EntityBase source)
        {
            foreach (var hsf in source.DescendantsAndSelf().OfType<IHasTestContainerSourceFile>())
            {
                var we = _watchedEntitySourceFiles.Find(item => item.Entity == hsf);
                if (we != null)
                {
                    we.StopWatching();
                    _watchedEntitySourceFiles.Remove(we);
                    Debug.WriteLine("Unregistered entity w/source file: " + we.SourceFilePath);
                }
            }
        }

        void watchedEntity_SourceFileChanged(object sender, EventArgs e)
        {
            var we = (WatchedEntity)sender;
            Debug.WriteLine("Source file changed: " + we.SourceFilePath);

            // Just in case it got disabled between a bunch of file system changes.
            if (!we.Enabled)
                return;

            // Make these a follow up command so any redundant change notifications may be minimized
            controller.AddFollowupCommand(new RefreshCommand(we.Entity, true, session.Entity.RuntimeState.ConfirmAutoRefresh));
        }

        #endregion

        private int _isUpdatingCnt;
        internal void OnBeginUpdate()
        {
            if (System.Threading.Interlocked.Increment(ref _isUpdatingCnt) == 1)
            {
                // We only fire the update event once per update block.
                if (BeginUpdate != null)
                    BeginUpdate();
            }
        }

        internal void DoEndUpdate()
        {
            if (System.Threading.Interlocked.Decrement(ref _isUpdatingCnt) == 0)
            {
                // We fire the update event after all BeginUpdate calls have been ended.
                if (EndUpdate != null)
                    EndUpdate();
            }
        }

        // selection
        internal void SelectionUpdateNotify(Selection.State previous, Selection.State current)
        {
            SelectionUpdated(previous, current);
        }

    }
}
