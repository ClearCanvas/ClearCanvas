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
	[MenuAction("apply", "attached-document-items/MenuAdd", "Apply")]
	[ButtonAction("apply", "attached-document-items/MenuAdd", "Apply")]
	[IconSet("apply", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
	[VisibleStateObserver("apply", "Visible")]
	[ExtensionOf(typeof(AttachedDocumentToolExtensionPoint))]
	public class DocumentAttachTool : Tool<IAttachedDocumentToolContext>
	{

		public bool Visible
		{
			get { return !this.Context.IsReadonly; }
		}

		public void Apply()
		{
			var component = new AttachDocumentComponent(this.Context.Site);
			var exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleAttachFile);
			if(exitCode == ApplicationComponentExitCode.Accepted)
			{
				this.Context.AddAttachment(component.Document, component.Category);
			}
		}
	}
}

