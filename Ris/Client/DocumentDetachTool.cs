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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("apply", "attached-document-items/MenuRemove", "Apply")]
	[ButtonAction("apply", "attached-document-items/MenuRemove", "Apply")]
	[IconSet("apply", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible")]
	[ExtensionOf(typeof(AttachedDocumentToolExtensionPoint))]
	public class DocumentDetachTool : Tool<IAttachedDocumentToolContext>
	{
		private bool _enabled;

		public override void Initialize()
		{
			base.Initialize();

			this.Context.SelectedAttachmentChanged += delegate { this.Enabled = this.Context.SelectedAttachment != null; };
		}

		public bool Visible
		{
			get { return !this.Context.IsReadonly; }
		}

		public bool Enabled
		{
			get { return !this.Context.IsReadonly && _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged;


		public void Apply()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.ConfirmDetachDocument, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
				this.Context.RemoveSelectedAttachment();
		}
	}
}

