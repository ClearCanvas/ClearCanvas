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

using System;
using System.Collections.Generic;
using System.Text;

using Gtk;
using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
//using ClearCanvas.ImageViewer.Actions;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public class MainWindow : Window
    {
        private MenuBar _mainMenu;
        private Toolbar _toolBar;
	private Tooltips _tooltips;
        private HBox _toolBarBox;
        private Notebook _notebook;
	private VBox _outerBox;
		
	private ToolViewManager _workbenchToolViewManager;
	private Dictionary<IWorkspace, ToolViewManager> _workspaceToolViewManagers;
	private IWorkspace _lastActiveWorkspace;
		
	private int openedStudyCount = 0;

	public MainWindow()
		: base("ClearCanvas")
        {
             this.SetDefaultSize(Screen.Width, Screen.Height);
			
	    _workspaceToolViewManagers = new Dictionary<IWorkspace, ToolViewManager>();
			
            _mainMenu = new MenuBar();
            _toolBar = new Toolbar();

            _notebook = new Notebook();
			_notebook.TabPos = PositionType.Top;
			_notebook.SwitchPage += OnNotebookSwitchPage;
			
            // this box holds the main menu
            HBox menuBox = new HBox(false, 0);
            menuBox.PackStart(_mainMenu, true, true, 0);

            // this box holds the toolbar
            _toolBarBox = new HBox(false, 0);
            _toolBarBox.PackStart(_toolBar, true, true, 0);

            // this box holds the overall layout
            _outerBox = new VBox(false, 0);
            _outerBox.PackStart(menuBox, false, false, 0);
            _outerBox.PackStart(_toolBarBox, false, false, 0);
            _outerBox.PackStart(_notebook, true, true, 0);

 			//WorkstationModel.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
            //WorkstationModel.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
            //WorkstationModel.WorkspaceManager.WorkspaceActivated += new EventHandler<WorkspaceEventArgs>(OnWorkspaceActivated);
			
            DesktopApplication.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
            DesktopApplication.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
            DesktopApplication.WorkspaceManager.WorkspaceActivated += new EventHandler<WorkspaceEventArgs>(OnWorkspaceActivated);

			this.Add(_outerBox);
            this.ShowAll();

            BuildMenusAndToolBars(null);
			UpdateToolViews(null);
        }

        private void BuildMenusAndToolBars(IWorkspace workspace)
        {
            BuildMenus(workspace);
            BuildToolbars(workspace);
        }
		

        private void BuildToolbars(IWorkspace workspace)
        {
            _toolBarBox.Remove(_toolBar);
			_toolBar.Destroy();		// make sure the old one is cleaned up!
            _toolBar = new Toolbar();
			_toolBar.ToolbarStyle = ToolbarStyle.Icons;
            _toolBarBox.PackStart(_toolBar, true, true, 0);
			_tooltips = new Tooltips();
			
			ActionModelRoot model = new ActionModelRoot("");
			//model.Merge(WorkstationModel.ToolManager.ToolbarModel);
			model.Merge(DesktopApplication.ToolSet.ToolbarModel);
			if(workspace != null) {
				//model.Merge(workspace.ToolManager.ToolbarModel);
				model.Merge(workspace.ToolSet.ToolbarModel);
			}
            GtkToolbarBuilder.BuildToolbar(_toolBar, _tooltips, model);
			
            _toolBar.ShowAll();
        }

        private void BuildMenus(IWorkspace workspace)
        {
            foreach(Widget w in _mainMenu) {
                _mainMenu.Remove(w);
				w.Destroy();
            }
			
			ActionModelRoot model = new ActionModelRoot("");
			//model.Merge(WorkstationModel.ToolManager.MenuModel);
			model.Merge(DesktopApplication.ToolSet.MenuModel);
			if(workspace != null) {
				//model.Merge(workspace.ToolManager.MenuModel);
				model.Merge(workspace.ToolSet.MenuModel);
			}

            GtkMenuBuilder.BuildMenu(_mainMenu, model);
            _mainMenu.ShowAll();
        }
		
		private void UpdateToolViews(IWorkspace workspace)
		{
			if(_workbenchToolViewManager == null)
			{
				//_workbenchToolViewManager = new ToolViewManager(WorkstationModel.ToolManager, this);
				_workbenchToolViewManager = new ToolViewManager(DesktopApplication.ToolSet, this);
				_workbenchToolViewManager.Activate(true);	// always active
			}
			
			if(_lastActiveWorkspace != null && _workspaceToolViewManagers.ContainsKey(_lastActiveWorkspace))
			{
				_workspaceToolViewManagers[_lastActiveWorkspace].Activate(false);
			}
			
			if(workspace != null)
			{
				if(!_workspaceToolViewManagers.ContainsKey(workspace))
				{
					//_workspaceToolViewManagers.Add(workspace, new ToolViewManager(workspace.ToolManager, this));
					_workspaceToolViewManagers.Add(workspace, new ToolViewManager((ToolSet)workspace.ToolSet, this));
				}
				
				_workspaceToolViewManagers[workspace].Activate(true);
		
			
			_lastActiveWorkspace = workspace;
			}
		}	

		private void OnNotebookSwitchPage(object sender, SwitchPageArgs args)
		{
			IWorkspace workspace = GetWorkspaceForWidget(_notebook.GetNthPage((int)args.PageNum));
			if(workspace != null)
			{
				workspace.IsActivated = true;
			}
		}
		
 		private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
        {
            try
            {
				AddWorkspace(e.Workspace);
            }
            catch (Exception ex)
            {
                Platform.Log(ex);
            }
        }
		
        // This is the event handler for when a workspace is removed from the
        // WorkspaceManager.  Not to be confused with OnWorkspaceClosed.
        private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
        {
            try
            {
				RemoveWorkspace(e.Workspace);
            }
            catch (Exception ex)
            {
                Platform.Log(ex);
            }
        }
		
		private void OnWorkspaceActivated(object sender, WorkspaceEventArgs e)
		{
            try
            {
 	            BuildMenusAndToolBars(e.Workspace);
				UpdateToolViews(e.Workspace);
			}
            catch (Exception ex)
            {
                Platform.Log(ex);
            }
		}
		
		public IWorkspace ActiveWorkspace
		{
			get
			{
				return GetWorkspaceForWidget(_notebook.CurrentPageWidget);
			}
		}

	private IWorkspace GetWorkspaceForWidget(Widget widget)
	{
		foreach(IWorkspace workspace in DesktopApplication.WorkspaceManager.Workspaces)
		{
			if(workspace.View.GuiElement == widget)
				return workspace;
		}
		return null;
	}

        public void AddWorkspace(IWorkspace workspace)
        {
			Widget wda = (Widget)workspace.View.GuiElement;
			Label label = new Label(string.Format("Study {0}", ++openedStudyCount));
			_notebook.AppendPage(wda, label);
			wda.Show();
 
			_notebook.CurrentPage = _notebook.NPages-1;
		}

        public void RemoveWorkspace(IWorkspace workspace)
        {
           // Find the form that owns the workspace to be removed and close it
            for (int i = 0; i < _notebook.NPages; i++)
            {
                Widget wda = (Widget)_notebook.GetNthPage(i);
                if (workspace.View.GuiElement == wda)
                {
                    _notebook.RemovePage(i);
                    wda.Destroy();
                    break;
                }
            }
			
			if(_workspaceToolViewManagers.ContainsKey(workspace))
			{
				_workspaceToolViewManagers[workspace].Activate(false);
				_workspaceToolViewManagers.Remove(workspace);
			}
 			
            if (_notebook.NPages == 0)
            {
                BuildMenusAndToolBars(null);
				UpdateToolViews(null);
            }
		}
     }
}
