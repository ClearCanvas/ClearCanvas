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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class ExternalPractitionerFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class ExternalPractitionerItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ExternalPractitionerFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IExternalPractitionerItemToolContext : IWorkflowItemToolContext<ExternalPractitionerSummary>
	{
	}

	public interface IExternalPractitionerFolderToolContext : IWorkflowFolderToolContext
	{
	}

	public class ExternalPractitionerSearchParams : SearchParams
	{
		public ExternalPractitionerSearchParams(string textSearch)
			: base(textSearch)
		{
		}
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Extended.Common.AuthorityTokens.FolderSystems.ExternalPractitioner)]
	public class ExternalPractitionerFolderSystem : WorkflowFolderSystem<
		ExternalPractitionerSummary,
		ExternalPractitionerFolderToolExtensionPoint,
		ExternalPractitionerItemToolExtensionPoint,
		ExternalPractitionerSearchParams>
	{
		class ExternalPractitionerItemToolContext : WorkflowItemToolContext, IExternalPractitionerItemToolContext
		{
			public ExternalPractitionerItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		class ExternalPractitionerFolderToolContext : WorkflowFolderToolContext, IExternalPractitionerFolderToolContext
		{
			public ExternalPractitionerFolderToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		public ExternalPractitionerFolderSystem()
			: base(SR.TitleExternalPractitionerFolderSystem)
		{
			this.Folders.Add(new UnverifiedFolder());
			this.Folders.Add(new VerifiedTodayFolder());
		}

		#region Search Related

		public override string SearchMessage
		{
			get { return SR.MessageSearchMessageExternalPractitioner; }
		}

		public override bool SearchEnabled
		{
			get { return true; }
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new ExternalPractitionerSearchFolder();
		}

		public override SearchParams CreateSearchParams(string searchText)
		{
			return new ExternalPractitionerSearchParams(searchText);
		}

		public override bool AdvancedSearchEnabled
		{
			// advance searching not currently supported
			get { return false; }
		}

		public override void LaunchSearchComponent()
		{
			// advance searching not currently supported
			return;
		}

		public override Type SearchComponentType
		{
			// advance searching not currently supported
			get { return null; }
		}

		#endregion

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ExternalPractitionerSummary> items)
		{
			return WebResourcesSettings.Default.ExternalPractitionerFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ExternalPractitionerSummary> items)
		{
			return new PreviewOperationAuditData[0];	// audit not required
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new ExternalPractitionerItemToolContext(this);
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new ExternalPractitionerFolderToolContext(this);
		}
	}
}
