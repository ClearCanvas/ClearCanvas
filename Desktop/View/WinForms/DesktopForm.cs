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

#region Additional permission to link with DotNetMagic

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with DotNetMagic (or a modified version of that library), containing parts
// covered by the terms of the Crownwood Software DotNetMagic license, the
// licensors of this Program grant you additional permission to convey the
// resulting work.

#endregion



using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
	/// Form used by the <see cref="DesktopWindowView"/> class.
    /// </summary>
    /// <remarks>
    /// This class may be subclassed.
    /// </remarks>
    public partial class DesktopForm : DotNetMagicForm
    {
        private ActionModelNode _menuModel;
        private ActionModelNode _toolbarModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopForm()
        {
#if !MONO
			SplashScreenManager.DismissSplashScreen(this);
#endif
			InitializeComponent();

			//Set both to be initially invisible, since there's nothing on them.
            _toolbar.Visible = false;
            _mainMenu.Visible = false;

			// manually subscribe this event handler *after* the call to InitializeComponent()
			_toolbar.ParentChanged += OnToolbarParentChanged;
            _dockingManager = new DockingManager(_toolStripContainer.ContentPanel, VisualStyle.IDE2005);
            _dockingManager.ActiveColor = SystemColors.Control;
            _dockingManager.InnerControl = _tabbedGroups;
			_dockingManager.TabControlCreated += OnDockingManagerTabControlCreated;

			_tabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;
			_tabbedGroups.TabControlCreated += OnTabbedGroupsTabControlCreated;

			if (_tabbedGroups.ActiveLeaf != null)
			{
				InitializeTabControl(_tabbedGroups.ActiveLeaf.TabControl);
			}

			ToolStripSettings.Default.PropertyChanged += OnToolStripSettingsPropertyChanged;
			OnToolStripSettingsPropertyChanged(ToolStripSettings.Default, new PropertyChangedEventArgs("WrapLongToolstrips"));
			OnToolStripSettingsPropertyChanged(ToolStripSettings.Default, new PropertyChangedEventArgs("IconSize"));
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the menu model.
        /// </summary>
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;

                //Unsubscribe, so we don't update visibility as the model is being figured out.
                _mainMenu.LayoutCompleted -= OnMenuLayoutCompleted;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Menu, _mainMenu, _menuModel);

                _mainMenu.LayoutCompleted += OnMenuLayoutCompleted;
                //Subscribe so the visibility updates if all actions suddenly become invisible or unavailable.
                OnMenuLayoutCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the toolbar model.
        /// </summary>
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;

                //Unsubscribe, so we don't update visibility as the model is being figured out.
                _toolbar.LayoutCompleted -= OnToolbarLayoutCompleted;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Toolbar, _toolbar, _toolbarModel);

                //Subscribe so the visibility updates if all actions suddenly become invisible or unavailable.
                _toolbar.LayoutCompleted += OnToolbarLayoutCompleted;
                OnToolbarLayoutCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the <see cref="TabbedGroups"/> object that manages workspace tab groups.
        /// </summary>
        public TabbedGroups TabbedGroups
        {
            get { return _tabbedGroups; }
        }

        /// <summary>
        /// Gets the <see cref="DockingManager"/> object that manages shelf docking windows.
        /// </summary>
        public DockingManager DockingManager
        {
            get { return _dockingManager; }
        }

        #endregion

        #region Form event handlers

        private void OnTabbedGroupsTabControlCreated(TabbedGroups tabbedGroups, Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }

        private void OnDockingManagerTabControlCreated(Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }

    	private void OnToolStripSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    	{
    		ToolStripSettings settings = ToolStripSettings.Default;
    		if (e.PropertyName == "WrapLongToolstrips" || e.PropertyName == "ToolStripDock")
    		{
				// handle both wrapping and docking together because both affect flow direction
    			bool verticalOrientation = ReferenceEquals(_toolbar.Parent, _toolStripContainer.LeftToolStripPanel)
    			                           || ReferenceEquals(_toolbar.Parent, _toolStripContainer.RightToolStripPanel);

                _toolbar.SuspendLayout();
    			_toolbar.LayoutStyle = settings.WrapLongToolstrips ? ToolStripLayoutStyle.Flow : ToolStripLayoutStyle.StackWithOverflow;
    			if (settings.WrapLongToolstrips)
    				((FlowLayoutSettings) _toolbar.LayoutSettings).FlowDirection = verticalOrientation ? FlowDirection.TopDown : FlowDirection.LeftToRight;
    			_toolbar.ResumeLayout(true);

    			ToolStripPanel targetParent = ConvertToToolStripPanel(_toolStripContainer, settings.ToolStripDock);
    			if (targetParent != null && !ReferenceEquals(targetParent, _toolbar.Parent))
    			{
    				_toolStripContainer.SuspendLayout();
    				targetParent.Join(_toolbar);

    				_toolStripContainer.ResumeLayout(true);
    			}
    		}
			else if (e.PropertyName == "IconSize")
			{
				ToolStripBuilder.ChangeIconSize(_toolbar, settings.IconSize);
			}
    	}

    	private void OnToolbarParentChanged(object sender, EventArgs e)
    	{
    		ToolStripDock dock = ConvertToToolStripDock(_toolStripContainer, _toolbar);
    		if (dock != ToolStripDock.None)
    		{
    			ToolStripSettings settings = ToolStripSettings.Default;
    			settings.ToolStripDock = dock;
    			settings.Save();
    		}
    	}

        /// <summary>
        /// This will fire anytime the main menu layout changes, which includes items 
        /// changing their visibility/availability and the menu being rebuilt.
        /// </summary>
        private void OnToolbarLayoutCompleted(object sender, EventArgs e)
        {
            var anyVisible = _toolbar.Items.Cast<ToolStripItem>().Any(i => i.Available);
            if (_toolbar.Visible != anyVisible)
                _toolbar.Visible = anyVisible;
        }

        /// <summary>
        /// This will fire anytime the toolbar layout changes, which includes items 
        /// changing their visibility/availability and the toolbar being rebuilt.
        /// </summary>
        private void OnMenuLayoutCompleted(object sender, EventArgs e)
        {
            var anyVisible = _mainMenu.Items.Cast<ToolStripItem>().Any(i => i.Available);
            if (_mainMenu.Visible != anyVisible)
                _mainMenu.Visible = anyVisible;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Called to initialize a <see cref="Crownwood.DotNetMagic.Controls.TabControl"/>. Override
        /// this method to perform custom initialization.
        /// </summary>
        /// <param name="tabControl"></param>
        protected virtual void InitializeTabControl(Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			if (tabControl == null)
				return;

			tabControl.TextTips = true;
			tabControl.ToolTips = false;
			tabControl.MaximumHeaderWidth = 256;
        }

        /// <summary>
        /// Called to build menus and toolbars.  Override this method to customize menu and toolbar building.
        /// </summary>
        /// <remarks>
        /// The default implementation simply clears and re-creates the toolstrip using methods on the
        /// utility class <see cref="ToolStripBuilder"/>.
        /// </remarks>
        /// <param name="kind"></param>
        /// <param name="toolStrip"></param>
        /// <param name="actionModel"></param>
        protected virtual void BuildToolStrip(ToolStripBuilder.ToolStripKind kind, ToolStrip toolStrip, ActionModelNode actionModel)
        {
            // avoid flicker
            toolStrip.SuspendLayout();
            // very important to clean up the existing ones first
            ToolStripBuilder.Clear(toolStrip.Items);

            if (actionModel != null)
            {
				if (actionModel.ChildNodes.Count > 0)
				{
					// Toolstrip should only be visible if there are items on it
					if (kind == ToolStripBuilder.ToolStripKind.Toolbar)
						ToolStripBuilder.BuildToolStrip(kind, toolStrip.Items, actionModel.ChildNodes, ToolStripBuilder.ToolStripBuilderStyle.GetDefault(), ToolStripSettings.Default.IconSize);
					else
						ToolStripBuilder.BuildToolStrip(kind, toolStrip.Items, actionModel.ChildNodes);
                }
            }

            toolStrip.ResumeLayout();
        }

    	private static ToolStripPanel ConvertToToolStripPanel(ToolStripContainer toolStripContainer, ToolStripDock dock)
    	{
    		switch (dock)
    		{
    			case ToolStripDock.Left:
    				return toolStripContainer.LeftToolStripPanel;
    			case ToolStripDock.Top:
    				return toolStripContainer.TopToolStripPanel;
    			case ToolStripDock.Right:
    				return toolStripContainer.RightToolStripPanel;
    			case ToolStripDock.Bottom:
    				return toolStripContainer.BottomToolStripPanel;
    			case ToolStripDock.None:
    			default:
    				return null;
    		}
    	}

    	private static ToolStripDock ConvertToToolStripDock(ToolStripContainer toolStripContainer, ToolStrip toolStrip)
    	{
    		ToolStripPanel parent = toolStrip.Parent as ToolStripPanel;
    		if (ReferenceEquals(parent, toolStripContainer.TopToolStripPanel))
    			return ToolStripDock.Top;
    		else if (ReferenceEquals(parent, toolStripContainer.LeftToolStripPanel))
    			return ToolStripDock.Left;
    		else if (ReferenceEquals(parent, toolStripContainer.BottomToolStripPanel))
    			return ToolStripDock.Bottom;
    		else if (ReferenceEquals(parent, toolStripContainer.RightToolStripPanel))
    			return ToolStripDock.Right;
			else
				return ToolStripDock.None;
    	}

        #endregion
    }
}
