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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	public interface ILocalImageExplorerToolContext : IToolContext
	{
		event EventHandler SelectedPathsChanged;
		IPathSelection SelectedPaths { get; }
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	[ExtensionPoint()]
	public sealed class LocalImageExplorerToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public sealed class LocalImageExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(LocalImageExplorerComponentViewExtensionPoint))]
	public class LocalImageExplorerComponent : ApplicationComponent
	{
		protected class LocalImageExplorerToolContext : ToolContext, ILocalImageExplorerToolContext
		{
			private LocalImageExplorerComponent _component;

			public LocalImageExplorerToolContext(LocalImageExplorerComponent component)
			{
				_component = component;
			}

			#region LocalImageExplorerToolContext Members

			public event EventHandler SelectedPathsChanged
			{
				add { _component.SelectionChanged += value; }
				remove { _component.SelectionChanged -= value; }
			}

			public IPathSelection SelectedPaths
			{
				get { return _component.Selection; }
			}

			public IDesktopWindow DesktopWindow
			{
				get
				{
					return _component.Host.DesktopWindow;
				}
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			#endregion
		}

		/// LocalImageExplorerComponent members

		private event EventHandler _selectionChanged;
		private IPathSelection _selection;

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;

		public LocalImageExplorerComponent()
		{
		}

		protected ToolSet ToolSet
		{
			get { return _toolSet; }
			set { _toolSet = value; }
		}

		public ClickHandlerDelegate DefaultActionHandler
		{
			get { return _defaultActionHandler; }
			set { _defaultActionHandler = value; }
		}

		public IPathSelection Selection
		{
			get { return _selection ?? new PathSelection(); }
			set
			{
				if (_selection != value)
				{
					_selection = value;
					OnSelectionChanged();
				}
			}
		}

		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		protected void OnSelectionChanged()
		{
			EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
		}

		public void DefaultAction()
		{
			if (this.DefaultActionHandler != null)
				this.DefaultActionHandler();
		}

		public IDesktopWindow DesktopWindow
		{
			get { return base.Host.DesktopWindow; }
		}

		public ActionModelNode ContextMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "explorerlocal-contextmenu", ToolSet.Actions);
			}
		}

		public override void Start()
		{
			base.Start();
			ToolSet = new ToolSet(new LocalImageExplorerToolExtensionPoint(), new LocalImageExplorerToolContext(this));
		}

		public override void Stop()
		{
			base.Stop();
			ToolSet.Dispose();
			ToolSet = null;
		}
	}
}
