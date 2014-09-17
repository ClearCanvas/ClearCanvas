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
using System.Linq;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;


namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointTable : Table<ExternalPractitionerContactPointDetail>
	{
		private event EventHandler _defaultContactPointChanged;

		public ExternalPractitionerContactPointTable()
		{
			this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColulmnPractitionerName, cp => cp.Name, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnDescription, cp => cp.Description, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, bool>(SR.ColumnDefault, cp => cp.IsDefaultContactPoint, 0.15f));
		}

		public event EventHandler DefaultContactPointChanged
		{
			add { _defaultContactPointChanged += value; }
			remove { _defaultContactPointChanged -= value; }
		}

		public void MakeDefaultContactPoint(ExternalPractitionerContactPointDetail cp)
		{
			foreach (var item in this.Items)
			{
				item.IsDefaultContactPoint = (item == cp);
				this.Items.NotifyItemUpdated(item);
			}

			EventsHelper.Fire(_defaultContactPointChanged, this, EventArgs.Empty);
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerContactPointSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ExternalPractitionerContactPointSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerContactPointSummaryComponent class
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerContactPointSummaryComponentViewExtensionPoint))]
	public class ExternalPractitionerContactPointSummaryComponent : SummaryComponentBase<ExternalPractitionerContactPointDetail, ExternalPractitionerContactPointTable>
	{
		private readonly EntityRef _practitionerRef;

		private readonly List<EnumValueInfo> _addressTypeChoices;
		private readonly List<EnumValueInfo> _phoneTypeChoices;
		private readonly List<EnumValueInfo> _resultCommunicationModeChoices;
		private readonly List<EnumValueInfo> _informationAuthorityChoices;
		private readonly string _practitionerName;

		/// <summary>
		/// Constructor for editing. Set the <see cref="Subject"/> property before starting.
		/// </summary>
		public ExternalPractitionerContactPointSummaryComponent(
			EntityRef practitionerRef,
			List<EnumValueInfo> addressTypeChoices,
			List<EnumValueInfo> phoneTypeChoices,
			List<EnumValueInfo> resultCommunicationModeChoices,
			List<EnumValueInfo> informationAuthorityChoices,
			string practitionerName)
			: base(false)
		{
			_practitionerRef = practitionerRef;
			_addressTypeChoices = addressTypeChoices;
			_phoneTypeChoices = phoneTypeChoices;
			_resultCommunicationModeChoices = resultCommunicationModeChoices;
			_informationAuthorityChoices = informationAuthorityChoices;
			_practitionerName = practitionerName;
		}

		/// <summary>
		/// Constructor for read-only selection. Set the <see cref="Subject"/> property before starting.
		/// </summary>
		public ExternalPractitionerContactPointSummaryComponent(EntityRef practitionerRef)
			: base(true)
		{
			_practitionerRef = practitionerRef;
			_addressTypeChoices = new List<EnumValueInfo>();
			_phoneTypeChoices = new List<EnumValueInfo>();
			_resultCommunicationModeChoices = new List<EnumValueInfo>();
			_informationAuthorityChoices = new List<EnumValueInfo>();
		}

		public override void Start()
		{
			var thisTable = (ExternalPractitionerContactPointTable)this.SummaryTable;
			thisTable.DefaultContactPointChanged += delegate
				{
					if (this.SetModifiedOnListChange)
						this.Modified = true;

					// Force validation to refresh if it is already visible.
					ShowValidation(this.ValidationVisible);
				};

			this.Validation.Add(new ValidationRule("SummarySelection", component =>
			{
				return CollectionUtils.Contains(this.Subject, contactPoint => contactPoint.IsDefaultContactPoint && !contactPoint.Deactivated)
					? new ValidationResult(true, "")
					: new ValidationResult(false, SR.MessageDefaultContactPointRequired);
			}));

			base.Start();
		}
		public IItemCollection<ExternalPractitionerContactPointDetail> Subject
		{
			get { return this.Table.Items; }
		}

		protected override bool SupportsPaging
		{
			get { return false; }
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<ExternalPractitionerContactPointDetail> ListItems(int firstItem, int maxItems)
		{
			// Return an empty list because the table items are set using the Subject property
			return new List<ExternalPractitionerContactPointDetail>();
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ExternalPractitionerContactPointDetail> addedItems)
		{
			addedItems = new List<ExternalPractitionerContactPointDetail>();

			var contactPoint = new ExternalPractitionerContactPointDetail
				{
					PreferredResultCommunicationMode = _resultCommunicationModeChoices.Count > 0 ? _resultCommunicationModeChoices[0] : null
				};

			// Keep looping until user enters an unique contact point name, or cancel the add operation
			ApplicationComponentExitCode exitCode;
			while (true)
			{
				var editor = new ExternalPractitionerContactPointEditorComponent(
					contactPoint,
					_addressTypeChoices,
					_phoneTypeChoices,
					_resultCommunicationModeChoices,
					_informationAuthorityChoices);

				exitCode = LaunchAsDialog(
					this.Host.DesktopWindow, editor, SR.TitleAddContactPoint + " - " + _practitionerName);

				var isUnique = IsContactPointNameUnique(null, contactPoint);
				if (exitCode == ApplicationComponentExitCode.Accepted && !isUnique)
					this.Host.DesktopWindow.ShowMessageBox(string.Format(SR.MessageExternalPractitionerContactPointNotUnique, contactPoint.Name), MessageBoxActions.Ok);
				else
					break;
			}

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(contactPoint);

				// if item was made default, then make sure no other items are also set as default
				if (contactPoint.IsDefaultContactPoint)
					this.Table.MakeDefaultContactPoint(contactPoint);

				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<ExternalPractitionerContactPointDetail> items, out IList<ExternalPractitionerContactPointDetail> editedItems)
		{
			editedItems = new List<ExternalPractitionerContactPointDetail>();
			var item = CollectionUtils.FirstElement(items);

			var contactPoint = (ExternalPractitionerContactPointDetail)item.Clone();

			// Keep looping until user enters an unique contact point name, or cancel the edit operation
			ApplicationComponentExitCode exitCode;
			while (true)
			{
				var editor = new ExternalPractitionerContactPointEditorComponent(
					contactPoint,
					_addressTypeChoices,
					_phoneTypeChoices,
					_resultCommunicationModeChoices,
					_informationAuthorityChoices);

				exitCode = LaunchAsDialog(
					this.Host.DesktopWindow,
					editor,
					string.Format(SR.TitleUpdateContactPoint + " - " + _practitionerName, contactPoint.Name));

				var isUnique = IsContactPointNameUnique(item, contactPoint);
				if (exitCode == ApplicationComponentExitCode.Accepted && !isUnique)
					this.Host.DesktopWindow.ShowMessageBox(string.Format(SR.MessageExternalPractitionerContactPointNotUnique, contactPoint.Name), MessageBoxActions.Ok);
				else
					break;
			}

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(contactPoint);

				// For new contact point, the updated contactPoint is a cloned of the original item.  So they are not referenced equal.
				// There is also no entityRef.  The only identifier is their name.
				// If name is changed, the IsSameItem won't identify the updated "contactPoint" and the original "item" as the same item
				// Therefore we must manually remove existing item and add updated contact point.
				var index = this.Table.Items.IndexOf(item);
				this.Table.Items.Remove(item);
				this.Table.Items.Insert(index, contactPoint);

				// if item was made default, then make sure no other items are also set as default
				if (contactPoint.IsDefaultContactPoint)
					this.Table.MakeDefaultContactPoint(contactPoint);

				return true;
			}
			return false;
		}

		protected override bool GetDeleteConfirmationMessage(IList<ExternalPractitionerContactPointDetail> itemsToBeDeleted, out string message)
		{
			// given that saved contact points cannot be deleted, it seems silly to confirm
			message = null;
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ExternalPractitionerContactPointDetail> items, out IList<ExternalPractitionerContactPointDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = items.Where(item => item.ContactPointRef == null).ToList();

			var nonDeletableItems = items.Where(item => item.ContactPointRef != null).ToList();
			if (nonDeletableItems.Count > 0)
			{
				this.Host.ShowMessageBox(SR.MessageSavedContactPointsCannotBeDeleted, MessageBoxActions.Ok);
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool UpdateItemsActivation(IList<ExternalPractitionerContactPointDetail> items, out IList<ExternalPractitionerContactPointDetail> editedItems)
		{
			if (CollectionUtils.Contains(items, item => item.Deactivated && item.IsMerged))
			{
				this.Host.ShowMessageBox(SR.MessageCannotActivateSelectedContactPoints, MessageBoxActions.Ok);
				editedItems = new List<ExternalPractitionerContactPointDetail>();
				return false;
			}

			editedItems = new List<ExternalPractitionerContactPointDetail>();
			foreach (var item in items)
			{
				item.Deactivated = !item.Deactivated;
				if(item.Deactivated)
					item.IsDefaultContactPoint = false;
				editedItems.Add(item);
			}

			// Force validation to refresh if it is already visible.
			ShowValidation(this.ValidationVisible);

			return true;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
		{
			if (ReferenceEquals(x, y))
				return true;

			// if only one is null, they are not the same
			if (x.ContactPointRef == null || y.ContactPointRef == null)
				return false;

			return x.ContactPointRef.Equals(y.ContactPointRef, true);
		}

		/// <summary>
		/// Check if the contact point name is unique within the existing table items.
		/// </summary>
		/// <param name="original">The original item for comparison.</param>
		/// <param name="edited">The cloned of the original that is edited by user.</param>
		/// <returns></returns>
		private bool IsContactPointNameUnique(ExternalPractitionerContactPointDetail original, ExternalPractitionerContactPointDetail edited)
		{
			var hasItemOfTheSameName = CollectionUtils.Contains(this.Table.Items, item =>
			{
				// Don't compare name with itself
				if (original != null && IsSameItem(item, original))
					return false;

				// Find an existing item with the same name.
				return Equals(item.Name, edited.Name);
			});

			return !hasItemOfTheSameName;
		}
	}
}
