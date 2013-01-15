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
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop.Tools;
using Gtk;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	public class ToolViewManager
	{
		//private ToolManager _toolManager;
		private ToolSet _toolManager;
		private bool _active;
		private Dictionary<ToolViewProxy, ToolViewHostDialog> _viewHosts;
		private Window _parentWindow;
		
		//public ToolViewManager(ToolManager toolManager, Window parentWindow)
		public ToolViewManager(ToolSet toolManager, Window parentWindow)
		{
			_toolManager = toolManager;
			_active = false;
			_viewHosts = new Dictionary<ToolViewProxy, ToolViewHostDialog>();
			_parentWindow = parentWindow;
			
			foreach(ToolViewProxy view in _toolManager.ToolViews)
			{
				view.ActivationChanged += OnToolViewActivationChanged;
			}
			
		}
		
		public void Activate(bool active)
		{
			if(active == _active)
				return;
			
			_active = active;
			if(_active)
			{
				foreach(ToolViewProxy view in _toolManager.ToolViews)
				{
					UpdateViewHost(view);
				}
			}
			else
			{
				// hide all views
				foreach(ToolViewHostDialog host in _viewHosts.Values)
				{
					host.Hide();
				}
			}
		}
		
		private void OnToolViewActivationChanged(object sender, EventArgs e)
		{
			if(_active)
			{
				UpdateViewHost((ToolViewProxy)sender);
			}
		}
		
		private void UpdateViewHost(ToolViewProxy view)
		{
			if(view.Active)
			{
				ToolViewHostDialog host;
				if(_viewHosts.ContainsKey(view))
				{
					host = _viewHosts[view];
				}
				else
				{
					host = new ToolViewHostDialog(view, _parentWindow);
					_viewHosts.Add(view, host);
				}
				host.ShowAll();
			}
			else
			{
				if(_viewHosts.ContainsKey(view))
				{
					_viewHosts[view].Hide();
				}
			}
		}
	}
}
