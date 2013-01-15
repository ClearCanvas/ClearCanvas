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

using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a workspace-modal dialog box.
	/// </summary>
	public class WorkspaceDialogBox : DesktopObject, IWorkspaceDialogBox
	{
		#region Host Implementation

		// implements the host interface, which is exposed to the hosted application component
		private class Host : ApplicationComponentHost, IWorkspaceDialogBoxHost
		{
			private readonly WorkspaceDialogBox _owner;

			internal Host(WorkspaceDialogBox owner, IApplicationComponent component)
				: base(component)
			{
				Platform.CheckForNullReference(owner, "owner");
				_owner = owner;
			}

			public override void Exit()
			{
				_owner._exitRequestedByComponent = true;

				// close the dialog
				_owner.Close(UserInteraction.Allowed, CloseReason.Program);
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _owner.Workspace.DesktopWindow; }
			}

			public override string Title
			{
				get { return _owner.Title; }
				set { _owner.Title = value; }
			}
		}

		#endregion

		private Host _host;
		private bool _exitRequestedByComponent;
		private readonly DialogSizeHint _sizeHint;
		private readonly Size _size;
		private Workspace _workspace;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="workspace"></param>
		protected internal WorkspaceDialogBox(DialogBoxCreationArgs args, Workspace workspace)
			: base(args)
		{
			_workspace = workspace;
			_host = new Host(this, args.Component);
			_size = args.Size;
			_sizeHint = args.SizeHint;
		}

		#region Public properties

		/// <summary>
		/// Gets the hosted component.
		/// </summary>
		public object Component
		{
			get { return _host.Component; }
		}

		/// <summary>
		/// Gets the workspace that owns this dialog box.
		/// </summary>
		public Workspace Workspace
		{
			get { return _workspace; }
		}

		/// <summary>
		/// Gets the size hint.
		/// </summary>
		public DialogSizeHint SizeHint
		{
			get { return _sizeHint; }
		}

		/// <summary>
		/// Gets the explicit size of the dialog, if specified.
		/// </summary>
		public Size Size
		{
			get { return _size; }
		}

		#endregion

		#region Protected overrides

		/// <summary>
		/// Asks the object whether it is in a closable state without user intervention.
		/// </summary>
		/// <returns>True if the object can be closed, otherwise false.</returns>
		protected internal override bool CanClose()
		{
			return _exitRequestedByComponent || _host.Component.CanExit();
		}

		/// <summary>
		/// Gives the hosted component a chance to prepare for a forced exit.
		/// </summary>
		protected override bool PrepareClose(CloseReason reason)
		{
			base.PrepareClose(reason);

			return _exitRequestedByComponent || _host.Component.PrepareExit();
		}

		/// <summary>
		/// Starts the hosted component.
		/// </summary>
		protected override void Initialize()
		{
			_host.StartComponent();

			base.Initialize();
		}

		/// <summary>
		/// Stops the hosted component.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && _host != null)
			{
				_host.StopComponent();
				_host = null;
				_workspace = null;
			}
		}

		/// <summary>
		/// Creates a view for this dialog box.
		/// </summary>
		protected sealed override IDesktopObjectView CreateView()
		{
			return _workspace.CreateWorkspaceDialogBoxView(this);
		}

		#endregion

		#region Helpers


		/// <summary>
		/// Gets the view for this object as an <see cref="IWorkspaceDialogBoxView"/>.
		/// </summary>
		protected IWorkspaceDialogBoxView WorkspaceDialogBoxView
		{
			get { return (IWorkspaceDialogBoxView)this.View; }
		}

		#endregion

		#region IWorkspace Members

		/// <summary>
		/// Gets the workspace that owns this dialog box.
		/// </summary>
		IWorkspace IWorkspaceDialogBox.Workspace
		{
			get { return _workspace; }
		}

		#endregion

	}
}
