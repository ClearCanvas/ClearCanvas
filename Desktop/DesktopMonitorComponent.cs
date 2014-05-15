#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

#pragma warning disable 1591
// ReSharper disable LocalizableElement

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{

#if DEBUG   // only include this tool in debug builds
    
    [MenuAction("launch", "global-menus/MenuTools/MenuUtilities/Desktop Monitor", "Launch")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class DesktopMonitorTool : Tool<IDesktopToolContext>
    {
        private Workspace _workspace;
        void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    DesktopMonitorComponent component = new DesktopMonitorComponent();
                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        "Desktop Monitor");
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

#endif

    /// <summary>
    /// Extension point for views onto <see cref="DesktopMonitorComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DesktopMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// A test component not intended for production use.
    /// </summary>
    [AssociateView(typeof(DesktopMonitorComponentViewExtensionPoint))]
    public class DesktopMonitorComponent : ApplicationComponent
    {
        private class EventLogItem
        {
            private string _objectName;
            private string _objectTitle;
            private string _message;
            private int _index;
            private static int _indexCounter = 0;

            public EventLogItem(DesktopObject source, string message)
            {
                // note that we don't cache a reference to the DesktopObject, as this might
                // affect results of GC tests
                _objectName = source.Name;
                _objectTitle = source.Title;

                _message = message;
                _index = ++_indexCounter;
            }

            public int Index
            {
                get { return _index; }
            }

            public string Name
            {
                get { return _objectName; }
            }

            public string Title
            {
                get { return _objectTitle; }
            }

            public string Message
            {
                get { return _message; }
            }
        }

        private static int _uniqueNameCounter = 0;

        private Table<DesktopWindow> _windows;
        private Table<Workspace> _workspaces;
        private Table<Shelf> _shelves;
        private Table<EventLogItem> _events;

        private DesktopWindow _selectedWindow;
        private Workspace _selectedWorkspace;
        private Shelf _selectedShelf;


        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopMonitorComponent()
        {
            _windows = new Table<DesktopWindow>();
            _windows.Columns.Add(new TableColumn<DesktopWindow, string>("Name", delegate(DesktopWindow item) { return item.Name; }));
            _windows.Columns.Add(new TableColumn<DesktopWindow, string>("Title", delegate(DesktopWindow item) { return item.Title; }));
            _windows.Columns.Add(new TableColumn<DesktopWindow, string>("Visible", delegate(DesktopWindow item) { return item.Visible ? "Visible" : ""; }));
            _windows.Columns.Add(new TableColumn<DesktopWindow, string>("Active", delegate(DesktopWindow item) { return item.Active ? "Active" : ""; }));

            _workspaces = new Table<Workspace>();
            _workspaces.Columns.Add(new TableColumn<Workspace, string>("Name", delegate(Workspace item) { return item.Name; }));
            _workspaces.Columns.Add(new TableColumn<Workspace, string>("Title", delegate(Workspace item) { return item.Title; }));
            _workspaces.Columns.Add(new TableColumn<Workspace, string>("Visible", delegate(Workspace item) { return item.Visible ? "Visible" : ""; }));
            _workspaces.Columns.Add(new TableColumn<Workspace, string>("Active", delegate(Workspace item) { return item.Active ? "Active" : ""; }));

            _shelves = new Table<Shelf>();
            _shelves.Columns.Add(new TableColumn<Shelf, string>("Name", delegate(Shelf item) { return item.Name; }));
            _shelves.Columns.Add(new TableColumn<Shelf, string>("Title", delegate(Shelf item) { return item.Title; }));
            _shelves.Columns.Add(new TableColumn<Shelf, string>("Visible", delegate(Shelf item) { return item.Visible ? "Visible" : ""; }));
            _shelves.Columns.Add(new TableColumn<Shelf, string>("Active", delegate(Shelf item) { return item.Active ? "Active" : ""; }));

            _events = new Table<EventLogItem>();
            _events.Columns.Add(new TableColumn<EventLogItem, int>("#", delegate(EventLogItem item) { return item.Index; }, 0.2F));
            _events.Columns.Add(new TableColumn<EventLogItem, string>("Name", delegate(EventLogItem item) { return item.Name; }));
            _events.Columns.Add(new TableColumn<EventLogItem, string>("Title", delegate(EventLogItem item) { return item.Title; }));
            _events.Columns.Add(new TableColumn<EventLogItem, string>("Message", delegate(EventLogItem item) { return item.Message; }));
            
            // add newer events to the top of the list
            _events.Sort(new TableSortParams(_events.Columns[0], false));
        }

        public override void Start()
        {
            base.Start();

            Application.DesktopWindows.ItemOpened += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemOpened);
            Application.DesktopWindows.ItemOpening += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemOpening);
            Application.DesktopWindows.ItemClosing += new EventHandler<ClosingItemEventArgs<DesktopWindow>>(Windows_ItemClosing);
            Application.DesktopWindows.ItemClosed += new EventHandler<ClosedItemEventArgs<DesktopWindow>>(Windows_ItemClosed);
            Application.DesktopWindows.ItemActivationChanged += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemActivationChanged);
            Application.DesktopWindows.ItemVisibilityChanged += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemVisibilityChanged);

            foreach (DesktopWindow window in Application.DesktopWindows)
            {
                SubscribeWindow(window);
            }

            _windows.Items.AddRange(Application.DesktopWindows);
        }

        public override void Stop()
        {
            // important to unsubscribe from events, or this component will not get cleaned up
            // (and will continue logging events)
            foreach (DesktopWindow window in Application.DesktopWindows)
            {
                UnsubscribeWindow(window);
            }

            Application.DesktopWindows.ItemOpened -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemOpened);
            Application.DesktopWindows.ItemOpening -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemOpening);
            Application.DesktopWindows.ItemClosing -= new EventHandler<ClosingItemEventArgs<DesktopWindow>>(Windows_ItemClosing);
            Application.DesktopWindows.ItemClosed -= new EventHandler<ClosedItemEventArgs<DesktopWindow>>(Windows_ItemClosed);
            Application.DesktopWindows.ItemActivationChanged -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemActivationChanged);
            Application.DesktopWindows.ItemVisibilityChanged -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow>>(Windows_ItemVisibilityChanged);

            base.Stop();
        }

        private void SubscribeWindow(DesktopWindow window)
        {
            window.Workspaces.ItemOpening += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemOpening);
            window.Workspaces.ItemOpened += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemOpened);
            window.Workspaces.ItemClosing += new EventHandler<ClosingItemEventArgs<Workspace>>(Workspaces_ItemClosing);
            window.Workspaces.ItemClosed += new EventHandler<ClosedItemEventArgs<Workspace>>(Workspaces_ItemClosed);
            window.Workspaces.ItemActivationChanged += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemActivationChanged);
            window.Workspaces.ItemVisibilityChanged += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemVisibilityChanged);
            window.Shelves.ItemOpening += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemOpening);
            window.Shelves.ItemOpened += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemOpened);
            window.Shelves.ItemClosing += new EventHandler<ClosingItemEventArgs<Shelf>>(Shelves_ItemClosing);
            window.Shelves.ItemClosed += new EventHandler<ClosedItemEventArgs<Shelf>>(Shelves_ItemClosed);
            window.Shelves.ItemActivationChanged += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemActivationChanged);
            window.Shelves.ItemVisibilityChanged += new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemVisibilityChanged);
        }

        private void UnsubscribeWindow(DesktopWindow window)
        {
            window.Workspaces.ItemOpening -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemOpening);
            window.Workspaces.ItemOpened -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemOpened);
            window.Workspaces.ItemClosing -= new EventHandler<ClosingItemEventArgs<Workspace>>(Workspaces_ItemClosing);
            window.Workspaces.ItemClosed -= new EventHandler<ClosedItemEventArgs<Workspace>>(Workspaces_ItemClosed);
            window.Workspaces.ItemActivationChanged -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemActivationChanged);
            window.Workspaces.ItemVisibilityChanged -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Workspace>>(Workspaces_ItemVisibilityChanged);
            window.Shelves.ItemOpening -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemOpening);
            window.Shelves.ItemOpened -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemOpened);
            window.Shelves.ItemClosing -= new EventHandler<ClosingItemEventArgs<Shelf>>(Shelves_ItemClosing);
            window.Shelves.ItemClosed -= new EventHandler<ClosedItemEventArgs<Shelf>>(Shelves_ItemClosed);
            window.Shelves.ItemActivationChanged -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemActivationChanged);
            window.Shelves.ItemVisibilityChanged -= new EventHandler<ClearCanvas.Common.Utilities.ItemEventArgs<Shelf>>(Shelves_ItemVisibilityChanged);
        }


        void Windows_ItemClosing(object sender, ClosingItemEventArgs<DesktopWindow> e)
        {
            LogState(e.Item);
        }

        void Windows_ItemOpening(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow> e)
        {
            SubscribeWindow(e.Item);
            _windows.Items.Add(e.Item);
            LogState(e.Item);
        }

        void Windows_ItemVisibilityChanged(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow> e)
        {
            if (_windows.Items.Contains(e.Item)) _windows.Items.NotifyItemUpdated(e.Item);
            LogVisiblility(e.Item);
        }

        void Windows_ItemActivationChanged(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow> e)
        {
            if (_windows.Items.Contains(e.Item)) _windows.Items.NotifyItemUpdated(e.Item);
            LogActivation(e.Item, Application.DesktopWindows.ActiveWindow);
        }

        void Windows_ItemClosed(object sender, ClosedItemEventArgs<DesktopWindow> e)
        {
            _windows.Items.Remove(e.Item);

            UnsubscribeWindow(e.Item);
            
            LogState(e.Item);
        }

        void Windows_ItemOpened(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<DesktopWindow> e)
        {
            LogState(e.Item);
        }

        private string CreateName(string baseName)
        {
            return string.Format("{0} {1}", baseName, ++_uniqueNameCounter);
        }


        public ITable Windows
        {
            get { return _windows; }
        }

        public ISelection SelectedWindow
        {
            get { return new Selection(_selectedWindow); }
            set
            {
                if (_selectedWindow != (DesktopWindow)value.Item)
                {
                    SelectedWindowChanged((DesktopWindow)value.Item);
                }
            }
        }

        public ITable Workspaces
        {
            get { return _workspaces; }
        }

        public ISelection SelectedWorkspace
        {
            get { return new Selection(_selectedWorkspace); }
            set
            {
                if (_selectedWorkspace != (Workspace)value.Item)
                {
                    _selectedWorkspace = (Workspace)value.Item;
                }
            }
        }

        public ITable Shelves
        {
            get { return _shelves; }
        }

        public ISelection SelectedShelf
        {
            get { return new Selection(_selectedShelf); }
            set
            {
                if (_selectedShelf != (Shelf)value.Item)
                {
                    _selectedShelf = (Shelf)value.Item;
                }
            }
        }

        public ITable EventLog
        {
            get { return _events; }
        }

        private void SelectedWindowChanged(DesktopWindow newWindow)
        {
            _workspaces.Items.Clear();
            _shelves.Items.Clear();

            _selectedWindow = newWindow;

            if (_selectedWindow != null)
            {
                _workspaces.Items.AddRange(_selectedWindow.Workspaces);
                _shelves.Items.AddRange(_selectedWindow.Shelves);
            }
        }

        void Shelves_ItemClosing(object sender, ClosingItemEventArgs<Shelf> e)
        {
            LogState(e.Item);
        }

        void Shelves_ItemOpening(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Shelf> e)
        {
            LogState(e.Item);
        }

        void Workspaces_ItemClosing(object sender, ClosingItemEventArgs<Workspace> e)
        {
            LogState(e.Item);
        }

        void Workspaces_ItemOpening(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Workspace> e)
        {
            LogState(e.Item);
        }

        void Shelves_ItemVisibilityChanged(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Shelf> e)
        {
            if (_shelves.Items.Contains(e.Item)) _shelves.Items.NotifyItemUpdated(e.Item);
            LogVisiblility(e.Item);
        }

        void Workspaces_ItemVisibilityChanged(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Workspace> e)
        {
            if (_workspaces.Items.Contains(e.Item)) _workspaces.Items.NotifyItemUpdated(e.Item);
            LogVisiblility(e.Item);
        }

        void Shelves_ItemActivationChanged(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Shelf> e)
        {
            if (_shelves.Items.Contains(e.Item)) _shelves.Items.NotifyItemUpdated(e.Item);
            LogActivation(e.Item, null);
        }

        void Workspaces_ItemActivationChanged(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Workspace> e)
        {
            if (_workspaces.Items.Contains(e.Item)) _workspaces.Items.NotifyItemUpdated(e.Item);
            LogActivation(e.Item, e.Item.DesktopWindow.Workspaces.ActiveWorkspace);
        }

        void Shelves_ItemClosed(object sender, ClosedItemEventArgs<Shelf> e)
        {
            _shelves.Items.Remove(e.Item);
            LogState(e.Item);
        }

        void Workspaces_ItemClosed(object sender, ClosedItemEventArgs<Workspace> e)
        {
            _workspaces.Items.Remove(e.Item);
            LogState(e.Item);
        }

        void Shelves_ItemOpened(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Shelf> e)
        {
            _shelves.Items.Add(e.Item);
            LogState(e.Item);
        }

        void Workspaces_ItemOpened(object sender, ClearCanvas.Common.Utilities.ItemEventArgs<Workspace> e)
        {
            _workspaces.Items.Add(e.Item);
            LogState(e.Item);
        }



        public void OpenWindow()
        {
            string name = CreateName("Window");
            Application.DesktopWindows.AddNew(new DesktopWindowCreationArgs(name, name));
        }

        public void ActivateSelectedWindow()
        {
            if (_selectedWindow != null)
            {
                _selectedWindow.Activate();
            }
        }

        public void CloseSelectedWindow()
        {
            if (_selectedWindow != null)
            {
                _selectedWindow.Close();
            }
        }

        public void OpenWorkspace()
        {
            if (_selectedWindow != null)
            {
                string name = CreateName("Workspace");
                _selectedWindow.Workspaces.AddNew(new WorkspaceCreationArgs(new TestComponent(name), name, name));
            }
        }

        public void ActivateSelectedWorkspace()
        {
            if (_selectedWorkspace != null)
            {
                _selectedWorkspace.Activate();
            }
        }

        public void CloseSelectedWorkspace()
        {
            if (_selectedWorkspace != null)
            {
                _selectedWorkspace.Close();
            }
        }

        public void OpenShelf()
        {
            if (_selectedWindow != null)
            {
                string name = CreateName("Shelf");
                _selectedWindow.Shelves.AddNew(new ShelfCreationArgs(new TestComponent(name), name, name,
                    ShelfDisplayHint.DockRight|ShelfDisplayHint.DockAutoHide));
            }
        }

        public void ActivateSelectedShelf()
        {
            if (_selectedShelf != null)
            {
                _selectedShelf.Activate();
            }
        }

        public void ShowSelectedShelf()
        {
            if (_selectedShelf != null)
            {
                _selectedShelf.Show();
            }
        }

        public void HideSelectedShelf()
        {
            if (_selectedShelf != null)
            {
                _selectedShelf.Hide();
            }
        }

        public void CloseSelectedShelf()
        {
            if (_selectedShelf != null)
            {
                _selectedShelf.Close();
            }
        }

        private void LogVisiblility(DesktopObject source)
        {
            LogEvent(new EventLogItem(source, source.Visible ? "Visible" : "Hidden"));
        }

        private void LogActivation(DesktopObject changeObj, DesktopObject nowActive)
        {
            string message = changeObj.Active ?
                "Activated" : string.Format("Deactivated, Active object: {0}", nowActive == null ? "none" : nowActive.Name);

            LogEvent(new EventLogItem(changeObj, message));
        }

        private void LogState(DesktopObject changeObj)
        {
            LogEvent(new EventLogItem(changeObj, "State: " + changeObj.State.ToString()));
        }

        private void LogEvent(EventLogItem item)
        {
            // if above threshold, remove the oldest event
            if (_events.Items.Count == 100)
            {
                // find the oldest item
                EventLogItem oldestEvent = CollectionUtils.Reduce<EventLogItem, EventLogItem>(_events.Items, null,
                    delegate(EventLogItem x, EventLogItem y)
                    {
                        return y == null ? x : (x.Index < y.Index ? x : y);
                    });

                _events.Items.Remove(oldestEvent);
            }

            _events.Items.Add(item);
            _events.Sort();
        }
    }
}

// ReSharper restore LocalizableElement