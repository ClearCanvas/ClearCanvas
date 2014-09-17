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

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuOpenDocumentation", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuOpenDocumentation", "Apply")]
	[IconSet("apply", "PerformingOpenDocumentationSmall.png", "PerformingOpenDocumentationMedium.png", "PerformingOpenDocumentationLarge.png")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Create)]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class PerformingDocumentationTool : Tool<IPerformingWorkflowItemToolContext>
	{
		public override void Initialize()
		{ 
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		public bool Enabled
		{
			get
			{
				return this.Context.SelectedItems.Count == 1
					&& CollectionUtils.FirstElement(this.Context.SelectedItems).OrderRef != null;
			}
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		public void Apply()
		{
			try
			{
				var item = CollectionUtils.FirstElement(this.Context.SelectedItems);

				if (item != null && item.OrderRef != null)
				{
					var document = DocumentManager.Get<PerformingDocumentationDocument>(item.OrderRef);
					if (document == null)
					{
						document = new PerformingDocumentationDocument(item, this.Context.DesktopWindow);
						document.Open();

						var selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
						document.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
					}
					else
					{
						document.Open();
					}
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
