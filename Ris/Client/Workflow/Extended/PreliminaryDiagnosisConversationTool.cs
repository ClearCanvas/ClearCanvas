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

using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Extends <see cref="OrderNoteConversationToolBase{TSummaryItem,TToolContext}"/> to provide a base class for tools which open an 
	/// <see cref="OrderNoteConversationComponent"/> for the purpose of creating a preliminary diagnosis
	/// </summary>
	/// <typeparam name="TSummaryItem"></typeparam>
	/// <typeparam name="TToolContext"></typeparam>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Preliminary Diagnosis", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Preliminary Diagnosis", "Open")]
	[Tooltip("pd", "Create/view the preliminary diagnosis for the selected item")]
	[EnabledStateObserver("pd", "Enabled", "EnabledChanged")]
	[IconSet("pd", "Icons.PrelimDiagConvoToolSmall.png", "Icons.PrelimDiagConvoToolMedium.png", "Icons.PrelimDiagConvoToolLarge.png")]
	[ActionPermission("pd", ClearCanvas.Ris.Application.Extended.Common.AuthorityTokens.Workflow.PreliminaryDiagnosis.Create)]
	public abstract class PreliminaryDiagnosisConversationTool<TSummaryItem, TToolContext> : OrderNoteConversationToolBase<TSummaryItem, TToolContext>
		where TSummaryItem : WorklistItemSummaryBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		public override bool Enabled
		{
			get
			{
				return base.Enabled && this.SummaryItem.OrderRef != null;
			}
		}

		protected override string TitleContextDescription
		{
			get
			{
				return string.Format(SR.FormatTitleContextDescriptionOrderNoteConversation,
					PersonNameFormat.Format(this.SummaryItem.PatientName),
					MrnFormat.Format(this.SummaryItem.Mrn),
					AccessionFormat.Format(this.SummaryItem.AccessionNumber));
			}
		}

		protected override string[] OrderNoteCategories
		{
			get { return new [] { OrderNoteCategory.PreliminaryDiagnosis.Key }; }
		}

		protected override void OnDialogClosed(ApplicationComponentExitCode exitCode)
		{
			this.Context.InvalidateSelectedFolder();

			base.OnDialogClosed(exitCode);
		}
	}
}