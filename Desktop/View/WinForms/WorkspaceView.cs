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

using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// WinForms implementation of <see cref="IWorkspaceView"/>. 
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
	/// class must also be subclassed in order to instantiate the subclass from 
	/// its <see cref="DesktopWindowView.CreateWorkspaceView"/> method.
	/// </para>
	/// <para>
	/// Reasons for subclassing may include: overriding <see cref="SetTitle"/> to customize the display of the workspace title.
	/// </para>
	/// </remarks>
	public class WorkspaceView : DesktopObjectView, IWorkspaceView
	{
		private Crownwood.DotNetMagic.Controls.TabPage _tabPage;
		private Control _control;
		private WorkspaceDialogBoxViewManager _dialogBoxManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="workspace"></param>
		/// <param name="desktopView"></param>
		protected internal WorkspaceView(Workspace workspace, DesktopWindowView desktopView)
		{
			var componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(workspace.Component.GetType());
			componentView.SetComponent((IApplicationComponent)workspace.Component);

			_control = (Control)componentView.GuiElement;
			_tabPage = new Crownwood.DotNetMagic.Controls.TabPage { Control = _control, Tag = this };
			this.DesktopView = desktopView;

			_dialogBoxManager = new WorkspaceDialogBoxViewManager(this, _control);
		}


		protected internal DesktopWindowView DesktopView { get; private set; }

		/// <summary>
		/// Gets the tab page that hosts this workspace view.
		/// </summary>
		protected internal Crownwood.DotNetMagic.Controls.TabPage TabPage
		{
			get { return _tabPage; }
		}

		/// <summary>
		/// Creates a new view for the specified <see cref="WorkspaceDialogBox"/>.
		/// </summary>
		/// <remarks>
		/// Override this method if you want to return a custom implementation of <see cref="IWorkspaceDialogBoxView"/>.
		/// In practice, it is preferable to subclass <see cref="WorkspaceDialogBoxView"/> rather than implement <see cref="IWorkspaceDialogBoxView"/>
		/// directly.
		/// </remarks>
		/// <param name="dialogBox"></param>
		/// <returns></returns>
		public virtual IWorkspaceDialogBoxView CreateDialogBoxView(WorkspaceDialogBox dialogBox)
		{
			return new WorkspaceDialogBoxView(dialogBox, this);
		}

		#region DesktopObjectView overrides

		/// <summary>
		/// Sets the title of the workspace.
		/// </summary>
		/// <param name="title"></param>
		public override void SetTitle(string title)
		{
			_tabPage.Title = title;
			_tabPage.ToolTip = title;
		}

		/// <summary>
		/// Opens the workspace, adding the tab to the tab group.
		/// </summary>
		public override void Open()
		{
			DesktopView.AddWorkspaceView(this);
		}

		/// <summary>
		/// Activates the workspace, making the tab the selected tab.
		/// </summary>
		public override void Activate()
		{
			_tabPage.Selected = true;
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public override void Show()
		{
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public override void Hide()
		{
		}

		/// <summary>
		/// Disposes of this object.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_tabPage != null)
				{
					// Remove the tab
					this.DesktopView.RemoveWorkspaceView(this);

					_dialogBoxManager.Dispose();
					_dialogBoxManager = null;
					_control.Dispose();
					_control = null;
					_tabPage.Dispose();
					_tabPage = null;

				}
			}
			base.Dispose(disposing);
		}

		#endregion

		#region DialogBoxView management

		internal void AddDialogBoxView(WorkspaceDialogBoxView view)
		{
			_dialogBoxManager.AddDialogBoxView(view);
		}

		internal void RemoveDialogBoxView(WorkspaceDialogBoxView view)
		{
			_dialogBoxManager.RemoveDialogBoxView(view);
		}

		#endregion

	}
}
