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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class PerformingDocumentationDocument : Document
	{
		private readonly ModalityWorklistItemSummary _item;
		private PerformingDocumentationComponent _component;

		public PerformingDocumentationDocument(ModalityWorklistItemSummary item, IDesktopWindow desktopWindow)
			: base(item.OrderRef, desktopWindow)
		{
			if(item == null)
			{
				throw new ArgumentNullException("item");
			}

			_item = item;
		}

		public override string GetTitle()
		{
			return string.Format("Performing - {0} - {1}", PersonNameFormat.Format(_item.PatientName), MrnFormat.Format(_item.Mrn));
		}

		public override bool SaveAndClose()
		{
			_component.SaveDocumentation();
			return base.Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new PerformingDocumentationComponent(_item);
			return _component;
		}

		public override OpenWorkspaceOperationAuditData GetAuditData()
		{
			return new OpenWorkspaceOperationAuditData("Performing", _item);
		}
	}
}