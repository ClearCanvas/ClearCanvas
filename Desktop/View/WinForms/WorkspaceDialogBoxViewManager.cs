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
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Manages the set of workspace dialog views for a <see cref="WorkspaceView"/>.
	/// </summary>
	internal class WorkspaceDialogBoxViewManager : IDisposable
	{
		private readonly WorkspaceView _owner;
		private readonly Control _workspaceContent;

		private readonly List<WorkspaceDialogBoxForm> _dialogBoxForms = new List<WorkspaceDialogBoxForm>();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="workspaceContent"></param>
		internal WorkspaceDialogBoxViewManager(WorkspaceView owner, Control workspaceContent)
		{
			_owner = owner;
			_workspaceContent = workspaceContent;

			// subscribe to some events so we can keep workspace dialogs appropriately sized/visible
			_workspaceContent.VisibleChanged += WorkspaceContentVisibleChanged;
			_workspaceContent.SizeChanged += WorkspaceContentSizeChanged;
			_owner.DesktopView.DesktopForm.LocationChanged += DesktopFormLocationChanged;
		}

		public void Dispose()
		{
			// unsubscribe from events so we don't have memory leaks
			_owner.DesktopView.DesktopForm.LocationChanged -= DesktopFormLocationChanged;
			_workspaceContent.VisibleChanged -= WorkspaceContentVisibleChanged;
			_workspaceContent.SizeChanged -= WorkspaceContentSizeChanged;
		}

		/// <summary>
		/// Adds the specified dialog box view.
		/// </summary>
		/// <param name="view"></param>
		public void AddDialogBoxView(WorkspaceDialogBoxView view)
		{
			// ensure content and any existing dialogs are disabled
			_workspaceContent.Enabled = false;
			CollectionUtils.ForEach(_dialogBoxForms, lb => lb.Enabled = false);

			// position and display the new dialog
			var dialogForm = view.DialogBoxForm;
			dialogForm.CentreInControl(_workspaceContent);

			// show, with the owner as the desktop form
			dialogForm.Show(_owner.DesktopView.DesktopForm);

			// add to list
			_dialogBoxForms.Add(dialogForm);
		}

		/// <summary>
		/// Removes the specified dialog box view.
		/// </summary>
		/// <param name="view"></param>
		public void RemoveDialogBoxView(WorkspaceDialogBoxView view)
		{
			// close the dialog and remove it
			var form = view.DialogBoxForm;
			form.Close();
			_dialogBoxForms.Remove(form);

			// re-enable the appropriate thing
			var previousDialogBox = CollectionUtils.LastElement(_dialogBoxForms);
			if (previousDialogBox != null)
			{
				// enable and activate previous dialog box in the list
				previousDialogBox.Enabled = true;
				previousDialogBox.Activate();
			}
			else
			{
				// activate the workspace content
				_workspaceContent.Enabled = true;
				_owner.Activate();
			}
		}

		#region Event Handlers

		private void DesktopFormLocationChanged(object sender, EventArgs e)
		{
			if (!_workspaceContent.Visible)
				return;

			UpdateDialogPositions();
		}

		private void WorkspaceContentSizeChanged(object sender, EventArgs e)
		{
			if (!_workspaceContent.Visible)
				return;

			UpdateDialogPositions();
		}

		private void WorkspaceContentVisibleChanged(object sender, EventArgs e)
		{
			// show or hide dialogs in response to workspace activation changing
			foreach (var form in _dialogBoxForms)
			{
				form.Visible = _workspaceContent.Visible;
			}

			// if the control has just become visible, it is possible that
			// the dialogs are not properly positioned, so we need to update them
			if (_workspaceContent.Visible)
			{
				UpdateDialogPositions();
			}
		}

		#endregion

		/// <summary>
		/// Resets the position of all dialog box forms.
		/// </summary>
		private void UpdateDialogPositions()
		{
			foreach (var form in _dialogBoxForms)
			{
				form.CentreInControl(_workspaceContent);
			}
		}
	}
}
