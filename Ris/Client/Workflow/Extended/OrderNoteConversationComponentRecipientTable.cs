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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;
using System.Collections;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// PreliminaryDiagnosisConversationComponent class.
	/// </summary>
	public partial class OrderNoteConversationComponent
	{
		#region RecipientTableItem class

		private class RecipientTableItem
		{
			public RecipientTableItem()
			{
					
			}

			public RecipientTableItem(object staffOrGroupSummary, bool mandatory)
			{
				this.Recipient = staffOrGroupSummary;
				this.IsMandatory = mandatory;
			}

			/// <summary>
			/// Gets or sets the recipient, which must be an instance of a <see cref="StaffGroupSummary"/> or <see cref="StaffSummary"/>.
			/// </summary>
			public object Recipient
			{
				get { return IsStaffRecipient ? (object) this.StaffSummary : this.StaffGroupSummary; }
				set
				{
					// clear both, in case value is changing from one type to the other
					this.StaffSummary = null;
					this.StaffGroupSummary = null;

					if (value is StaffGroupSummary)
						this.StaffGroupSummary = (StaffGroupSummary) value;
					else if (value is StaffSummary)
						this.StaffSummary = (StaffSummary) value;
				}
			}

			public bool IsStaffRecipient
			{
				get { return StaffSummary != null; }
			}

			public StaffSummary StaffSummary { get; private set; }

			public bool IsGroupRecipient
			{
				get { return StaffGroupSummary != null; }
			}

			public StaffGroupSummary StaffGroupSummary { get; private set; }

			public bool IsMandatory { get; private set; }

			public static string Format(object staffOrGroup)
			{
				return (staffOrGroup is StaffSummary)
					? StaffNameAndRoleFormat.Format(((StaffSummary)staffOrGroup))
					: (staffOrGroup is StaffGroupSummary) ? ((StaffGroupSummary)staffOrGroup).Name : string.Empty;
			}

		}

		#endregion

		private class RecipientTable : Table<Checkable<RecipientTableItem>>
		{
			private readonly OrderNoteConversationComponent _owner;

			public RecipientTable(OrderNoteConversationComponent owner)
			{
				_owner = owner;
				var checkColumn = new TableColumn<Checkable<RecipientTableItem>, bool>(
					"Select",
					item => item.IsChecked,
					(item, value) =>
					{
						item.IsChecked = value;
						// bug: #2594: forces validation to refresh otherwise the screen doesn't update 
						// only when validation is visible
						if (_owner.ValidationVisible)
							_owner.ShowValidation(true);
					},
					0.4f) { EditableHandler = (x => !x.Item.IsMandatory) };

				this.Columns.Add(checkColumn);

				var nameColumn = new TableColumn<Checkable<RecipientTableItem>, object>(
					"Name",
					item => item.Item.Recipient,
					(x, value) => x.Item.Recipient = value,
					2.0f)
                 	{
                 		ValueFormatter = RecipientTableItem.Format,
                 		CellEditor = new LookupHandlerCellEditor(new StaffAndGroupLookupHandler(owner.Host.DesktopWindow)),
						EditableHandler = (x => !x.Item.IsMandatory)
                 	};
				this.Columns.Add(nameColumn);
			}

			public List<StaffSummary> CheckStaff
			{
				get
				{
					return CollectionUtils.Map(
						CollectionUtils.Select(
							this.Items,
							item => item.IsChecked && item.Item.IsStaffRecipient),
						(Checkable<RecipientTableItem> item) => item.Item.StaffSummary);
				}
			}

			public List<StaffGroupSummary> CheckedStaffGroups
			{
				get
				{
					return CollectionUtils.Map(
						CollectionUtils.Select(
							this.Items,
							item => item.IsChecked && item.Item.IsGroupRecipient),
						(Checkable<RecipientTableItem> item) => item.Item.StaffGroupSummary);
				}
			}

			public void AddRange(IEnumerable staffOrGroups, bool mandatory, bool @checked)
			{
				foreach (var item in staffOrGroups)
				{
					Add(item, mandatory, @checked);
				}
			}

			public Checkable<RecipientTableItem> AddNew(bool @checked)
			{
				var recip = new Checkable<RecipientTableItem>(new RecipientTableItem(), @checked);
				this.Items.Add(recip);
				return recip;
			}

			public void Add(object staffOrGroup, bool mandatory, bool @checked)
			{
				var exists = CollectionUtils.Contains(this.Items,
											item => Equals(item.Item.Recipient, staffOrGroup));

				if (!exists)
				{
					this.Items.Add(new Checkable<RecipientTableItem>(new RecipientTableItem(staffOrGroup, mandatory), mandatory || @checked));
				}
			}

		}
	}
}
