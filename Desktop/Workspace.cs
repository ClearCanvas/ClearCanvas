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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a workspace within a desktop window.
	/// </summary>
	public class Workspace : DesktopObject, IWorkspace
	{
		#region Host Implementation

		// implements the host interface, which is exposed to the hosted application component
		private class Host : ApplicationComponentHost, IWorkspaceHost
		{
			private readonly Workspace _workspace;

			internal Host(Workspace workspace, IApplicationComponent component)
				: base(component)
			{
				Platform.CheckForNullReference(workspace, "workspace");
				_workspace = workspace;
			}

			public override void Exit()
			{
				_workspace._exitRequestedByComponent = true;

				// close the workspace
				_workspace.Close(UserInteraction.Allowed, CloseReason.Program);
			}

			public override CommandHistory CommandHistory
			{
				get { return _workspace._commandHistory; }
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _workspace._desktopWindow; }
			}

			public override string Title
			{
				get { return _workspace.Title; }
				set { _workspace.Title = value; }
			}

			public bool IsWorkspaceActive
			{
				get { return _workspace.Active; }
			}

			public event EventHandler IsWorkspaceActiveChanged
			{
				add { _workspace.ActiveChanged += value; }
				remove { _workspace.ActiveChanged -= value; }
			}
		}

		#endregion

		private Host _host;
		private DesktopWindow _desktopWindow;
		private CommandHistory _commandHistory;
		private bool _exitRequestedByComponent;
		private readonly bool _userClosable;
		private WorkspaceDialogBoxCollection _dialogBoxes;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args">Arguments for creation of the <see cref="Workspace"/>.</param>
		/// <param name="desktopWindow">The <see cref="DesktopWindow"/> that owns the <see cref="Workspace"/>.</param>
		protected internal Workspace(WorkspaceCreationArgs args, DesktopWindow desktopWindow)
			: base(args)
		{
			_commandHistory = new CommandHistory(100);
			_desktopWindow = desktopWindow;
			_userClosable = args.UserClosable;
			_dialogBoxes = new WorkspaceDialogBoxCollection(this);
			_host = new Host(this, args.Component);
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
		/// Gets the desktop window that owns this workspace.
		/// </summary>
		public DesktopWindow DesktopWindow
		{
			get { return _desktopWindow; }
		}

		/// <summary>
		/// Gets the command history associated with this workspace.
		/// </summary>
		public CommandHistory CommandHistory
		{
			get { return _commandHistory; }
		}

		/// <summary>
		/// Gets a value indicating whether this workspace can be closed directly by the user.
		/// </summary>
		public bool UserClosable
		{
			get { return _userClosable; }
		}

		#endregion

		#region Protected overrides

		/// <summary>
		/// Asks the object whether it is in a closable state without user intervention.
		/// </summary>
		/// <returns>True if the object can be closed, otherwise false.</returns>
		protected internal override bool CanClose()
		{
			// are there any dialogs that cannot close?
			var waitingDialogBoxes = CollectionUtils.Contains(_dialogBoxes, lb => !lb.CanClose());
			if (waitingDialogBoxes)
				return false;

			return _exitRequestedByComponent || _host.Component.CanExit();
		}

		/// <summary>
		/// Gives the hosted component a chance to prepare for a forced exit.
		/// </summary>
		protected override bool PrepareClose(CloseReason reason)
		{
			if (!base.PrepareClose(reason))
				return false;

			// attempt to close any open dialogs first 
			// process in reverse order, so that the last opened is closed first
			var dialogBoxes = new List<WorkspaceDialogBox>(_dialogBoxes);
			dialogBoxes.Reverse();
			foreach (var dialogBox in dialogBoxes)
			{
				// if the dialogBox is still open, try to close it
				// (the check is necessary because there is no guarantee that the dialogBox is still open)
				if (dialogBox.State == DesktopObjectState.Open &&
				    !dialogBox.Close(UserInteraction.Allowed, reason | CloseReason.ParentClosing))
					return false;
			}

			// now it is up to the hosted component
			return _exitRequestedByComponent || _host.Component.PrepareExit();
		}

		/// <summary>
		/// Overridden to prevent closing the <see cref="Workspace"/> if <see cref="UserClosable"/> is false.
		/// </summary>
		protected override void OnClosing(ClosingEventArgs args)
		{
			var parentClosing = (args.Reason & CloseReason.ParentClosing) == CloseReason.ParentClosing;
			var userClosing = (args.Reason & CloseReason.UserInterface) == CloseReason.UserInterface;

			if (userClosing && !parentClosing && !_userClosable)
			{
				// the user is attempting to close this workspace (not the parent window), but it is not user-closable
				// cancel the close, and do not call the base class (do not fire the event publicly)
				args.Cancel = true;

				return;
			}

			base.OnClosing(args);
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

				if (_dialogBoxes != null)
					(_dialogBoxes as IDisposable).Dispose();

				_commandHistory = null;
				_dialogBoxes = null;
				_desktopWindow = null;
			}
		}

		/// <summary>
		/// Creates a view for this workspace.
		/// </summary>
		protected override sealed IDesktopObjectView CreateView()
		{
			return _desktopWindow.CreateWorkspaceView(this);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the set of actions that are exported from the hosted component.
		/// </summary>
		protected internal IActionSet Actions
		{
			get { return _host.Component.ExportedActions; }
		}

		/// <summary>
		/// Gets the view for this object as an <see cref="IWorkspaceView"/>.
		/// </summary>
		protected IWorkspaceView WorkspaceView
		{
			get { return (IWorkspaceView) View; }
		}

		/// <summary>
		/// Creates a dialog box view for the specified dialog box.
		/// </summary>
		internal IWorkspaceDialogBoxView CreateWorkspaceDialogBoxView(WorkspaceDialogBox dialogBox)
		{
			return WorkspaceView.CreateDialogBoxView(dialogBox);
		}

		#endregion

		#region IWorkspace Members

		/// <summary>
		/// Shows a dialog box in front of this workspace.
		/// </summary>
		/// <param name="args">Arguments used to create the dialog box.</param>
		/// <returns>The newly created dialog box object.</returns>
		public WorkspaceDialogBox ShowDialogBox(DialogBoxCreationArgs args)
		{
			AssertState(new[] {DesktopObjectState.Open, DesktopObjectState.Closing});

			return _dialogBoxes.AddNew(args);
		}

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> that owns this <see cref="Workspace"/>.
		/// </summary>
		IDesktopWindow IWorkspace.DesktopWindow
		{
			get { return _desktopWindow; }
		}

		#endregion
	}
}