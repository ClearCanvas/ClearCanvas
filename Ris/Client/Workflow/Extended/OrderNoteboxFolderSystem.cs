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
using System.Linq;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class OrderNoteboxFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class OrderNoteboxItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class OrderNoteboxFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IOrderNoteboxItemToolContext : IWorkflowItemToolContext<OrderNoteboxItemSummary>
	{
	}

	public interface IOrderNoteboxFolderToolContext : IWorkflowFolderToolContext
	{
		void RebuildGroupFolders();
	}



	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Extended.Common.AuthorityTokens.FolderSystems.OrderNotes)]
	public class OrderNoteboxFolderSystem : WorkflowFolderSystem<
		OrderNoteboxItemSummary,
		OrderNoteboxFolderToolExtensionPoint,
		OrderNoteboxItemToolExtensionPoint,
		SearchParams>
	{
		class OrderNoteboxItemToolContext : WorkflowItemToolContext, IOrderNoteboxItemToolContext
		{
			public OrderNoteboxItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		class OrderNoteboxFolderToolContext : WorkflowFolderToolContext, IOrderNoteboxFolderToolContext
		{
			private readonly OrderNoteboxFolderSystem _owner;

			public OrderNoteboxFolderToolContext(OrderNoteboxFolderSystem owner)
				: base(owner)
			{
				_owner = owner;
			}

			public void RebuildGroupFolders()
			{
				_owner.RebuildGroupFolders();
			}
		}

		private readonly IconSet _unacknowledgedNotesIconSet;
		private readonly string _baseTitle;
		private readonly PersonalInboxFolder _inboxFolder;

		public OrderNoteboxFolderSystem()
			: base(SR.TitleOrderNoteboxFolderSystem)
		{
			_unacknowledgedNotesIconSet = new IconSet("NoteUnread.png");
			_baseTitle = SR.TitleOrderNoteboxFolderSystem;

			_inboxFolder = new PersonalInboxFolder(this);
			_inboxFolder.TotalItemCountChanged += FolderItemCountChangedEventHandler;
			this.Folders.Add(_inboxFolder);
			this.Folders.Add(new SentItemsFolder(this));
		}

		public override bool SearchEnabled
		{
			// searching not currently supported
			get { return false; }
		}

		public override bool AdvancedSearchEnabled
		{
			// searching not currently supported
			get { return false; }
		}

		// We can't lazy initialize this folder system, because the title bar needs to show the status.
		public override bool LazyInitialize
		{
			get { return false; }
		}

		public override string SearchMessage
		{
			get { return SR.MessageSearchNotSupported; }
		}

		public override void Initialize()
		{
			base.Initialize();

			RebuildGroupFolders();
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<OrderNoteboxItemSummary> items)
		{
			return WebResourcesSettings.Default.OrderNoteboxFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<OrderNoteboxItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Order Notes", item.Mrn, item.PatientName, item.AccessionNumber)).ToArray();
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new OrderNoteboxFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new OrderNoteboxItemToolContext(this);
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			// searching not currently supported
			return null;
		}

		public override SearchParams CreateSearchParams(string searchText)
		{
			// searching not currently supported
			return null;
		}

		public override void LaunchSearchComponent()
		{
			// searching not currently supported
			return;
		}

		public override Type SearchComponentType
		{
			get { return null; }
		}

		protected override IDictionary<string, bool> QueryOperationEnablement(ISelection selection)
		{
			return new Dictionary<string, bool>();
		}

		protected void FolderItemCountChangedEventHandler(object sender, EventArgs e)
		{
			int count = CountTotalInboxItems();
			this.Title = string.Format(SR.FormatOrderNoteboxFolderSystemTitle, _baseTitle, count);
			this.TitleIcon = count > 0 ? _unacknowledgedNotesIconSet : null;
		}

		private int CountTotalInboxItems()
		{
			return _inboxFolder.TotalItemCount +
				CollectionUtils.Reduce<IFolder, int>(this.Folders, 0,
					delegate(IFolder f, int sum)
					{
						return sum + ((f is GroupInboxFolder && f.Visible) ? f.TotalItemCount : 0);
					});
		}

		private void RebuildGroupFolders()
		{
			List<StaffGroupSummary> groupsToShow = null;
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					//List<string> visibleGroups = OrderNoteboxFolderSystemSettings.Default.GroupFolders.StaffGroupNames;
					groupsToShow = service.ListStaffGroups(new ListStaffGroupsRequest()).StaffGroups;

					// select those groups that are marked as visible
					//groupsToShow = CollectionUtils.Select(groupsToShow,
					//    delegate(StaffGroupSummary g)
					//    {
					//        return CollectionUtils.Contains(visibleGroups,
					//            delegate(string groupName) { return groupName == g.Name; });
					//    });
				});

			// sort groups alphabetically
			groupsToShow.Sort(delegate(StaffGroupSummary x, StaffGroupSummary y) { return x.Name.CompareTo(y.Name); });

			// temporarily disable events while we manipulate the folders collection
			this.Folders.EnableEvents = false;

			// remove existing group folders
			CollectionUtils.Remove(this.Folders,
				delegate(IFolder f) { return f is GroupInboxFolder; });

			// add new group folders again
			foreach (StaffGroupSummary group in groupsToShow)
			{
				GroupInboxFolder groupFolder = new GroupInboxFolder(this, group);
				groupFolder.TotalItemCountChanged += FolderItemCountChangedEventHandler;

				this.Folders.Add(groupFolder);
			}

			// re-enable events
			this.Folders.EnableEvents = true;

			// notify that the entire folders collection has changed so that the tree is reconstructed
			this.NotifyFoldersChanged();
		}
	}
}
