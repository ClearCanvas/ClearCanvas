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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	public abstract class LocalExplorerStudyFilterToolProxy<T> : StudyFilterTool
		where T : ToolBase, new()
	{
		private T _baseTool;

		protected LocalExplorerStudyFilterToolProxy()
		{
			_baseTool = new T();
		}

		protected T BaseTool
		{
			get { return _baseTool; }
		}

		protected IActionSet BaseActions
		{
			get { return _baseTool.Actions; }
		}

		public override void Initialize()
		{
			base.Initialize();
			_baseTool.SetContext(new ToolContextProxy(this));
			_baseTool.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _baseTool != null)
			{
				_baseTool.Dispose();
				_baseTool = null;
			}
			base.Dispose(disposing);
		}

		private class ToolContextProxy : ILocalImageExplorerToolContext
		{
			private readonly LocalExplorerStudyFilterToolProxy<T> _owner;
			private ClickHandlerDelegate _defaultActionHandler;

			public ToolContextProxy(LocalExplorerStudyFilterToolProxy<T> owner)
			{
				_owner = owner;
			}

			public event EventHandler SelectedPathsChanged
			{
				add { _owner.Context.SelectedItems.SelectionChanged += value; }
				remove { _owner.Context.SelectedItems.SelectionChanged -= value; }
			}

			public IPathSelection SelectedPaths
			{
				get
				{
					var selection = new List<string>();
					foreach (IStudyItem item in _owner.SelectedItems)
						selection.Add(item.Filename);
					return new PathSelection(selection);
				}
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.Context.DesktopWindow; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _defaultActionHandler; }
				set { _defaultActionHandler = value; }
			}
		}
	}
}