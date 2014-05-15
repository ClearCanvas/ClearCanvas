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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerSelectDisabledContactPointsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerSelectDisabledContactPointsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerSelectDisabledContactPointsComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerSelectDisabledContactPointsComponentViewExtensionPoint))]
	public class ExternalPractitionerSelectDisabledContactPointsComponent : ApplicationComponent
	{
		private class ExternalPractitionerContactPointTable : Table<ExternalPractitionerContactPointDetail>
		{
			private const int NumRows = 5;
			private const int PhoneRow = 1;
			private const int FaxRow = 2;
			private const int AddressRow = 3;
			private const int EmailRow = 4;

			private event EventHandler _tableModified;

			public ExternalPractitionerContactPointTable()
				: base(NumRows)
			{
				var activeColumn = new TableColumn<ExternalPractitionerContactPointDetail, bool>(SR.ColumnActive,
					cp => !cp.Deactivated, SetDeactivatedStatus, 0.1f);

				var defaultColumn = new TableColumn<ExternalPractitionerContactPointDetail, bool>(SR.ColumnDefault,
					cp => cp.IsDefaultContactPoint, MakeDefaultContactPoint, 0.1f);

				var nameColumn = new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnContactPointName,
					cp => cp.Name, 0.3f);

				var descriptionColumn = new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnDescription,
					cp => cp.Description, 0.5f);

				var phoneColumn = new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnPhone,
					cp => string.Format(SR.FormatPhone, cp.CurrentPhoneNumber == null ? "" : TelephoneFormat.Format(cp.CurrentPhoneNumber)),
					1.0f, PhoneRow);

				var faxColumn = new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnFax,
					cp => string.Format(SR.FormatFax, cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentFaxNumber)),
					1.0f, FaxRow);

				var addressColumn = new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnAddress,
					cp => string.Format(SR.FormatAddress, cp.CurrentAddress == null ? "" : AddressFormat.Format(cp.CurrentAddress)),
					1.0f, AddressRow);

				var emailColumn = new TableColumn<ExternalPractitionerContactPointDetail, string>(SR.ColumnEmail,
					cp => string.Format(SR.FormatEmail, cp.CurrentEmailAddress == null ? "" : cp.CurrentEmailAddress.Address),
					1.0f, EmailRow);

				this.Columns.Add(activeColumn);
				this.Columns.Add(defaultColumn);
				this.Columns.Add(nameColumn);
				this.Columns.Add(descriptionColumn);
				this.Columns.Add(phoneColumn);
				this.Columns.Add(faxColumn);
				this.Columns.Add(addressColumn);
				this.Columns.Add(emailColumn);
			}

			public event EventHandler TableModified
			{
				add { _tableModified += value; }
				remove { _tableModified -= value; }
			}

			public void MakeDefaultContactPoint(ExternalPractitionerContactPointDetail cp, bool value)
			{
				if (cp == null)
					return;

				// Make sure only one contact point is checked as the default.
				foreach (var item in this.Items)
				{
					item.IsDefaultContactPoint = item == cp ? value : false;
					this.Items.NotifyItemUpdated(item);
				}

				EventsHelper.Fire(_tableModified, this, EventArgs.Empty);
			}

			private void SetDeactivatedStatus(ExternalPractitionerContactPointDetail cp, bool value)
			{
				cp.Deactivated = !value;
				EventsHelper.Fire(_tableModified, this, EventArgs.Empty);
			}
		}

		private readonly ExternalPractitionerContactPointTable _table;
		private readonly List<ExternalPractitionerContactPointDetail> _deactivatedContactPointNotShown;
		private ExternalPractitionerContactPointDetail _selectedItem;

		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerDetail _duplicatePractitioner;

		public ExternalPractitionerSelectDisabledContactPointsComponent()
		{
			_table = new ExternalPractitionerContactPointTable();
			_deactivatedContactPointNotShown = new List<ExternalPractitionerContactPointDetail>();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(this.ActiveContactPoints.Count > 0, SR.MessageValidationAtLeastOneActiveContactPoint)));

			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(this.DefaultContactPoint != null, SR.MessageValidationMustHaveOneDefaultContactPoint)));

			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(this.DefaultContactPoint == null ? true : this.DefaultContactPoint.Deactivated == false, SR.MessageValidationDefaultContactPointMustBeActive)));

			_table.TableModified += ((sender, e) => this.ShowValidation(true));

			base.Start();
		}

		public event EventHandler ContactPointSelectionChanged
		{
			add { _table.TableModified += value; }
			remove { _table.TableModified -= value; }
		}

		public ExternalPractitionerDetail OriginalPractitioner
		{
			get { return _originalPractitioner; }
			set
			{
				if (_originalPractitioner != null && _originalPractitioner.Equals(value))
					return;

				_originalPractitioner = value;
				UpdateContactPointsTable();
			}
		}

		public ExternalPractitionerDetail DuplicatePractitioner
		{
			get { return _duplicatePractitioner; }
			set
			{
				if (_duplicatePractitioner != null && _duplicatePractitioner.Equals(value))
					return;

				_duplicatePractitioner = value;
				UpdateContactPointsTable();
			}
		}

		public ExternalPractitionerContactPointDetail DefaultContactPoint
		{
			get { return CollectionUtils.SelectFirst(_table.Items, cp => cp.IsDefaultContactPoint); }
		}

		public List<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return CollectionUtils.Select(_table.Items, cp => cp.Deactivated == false); }
		}

		public List<ExternalPractitionerContactPointDetail> DeactivatedContactPoints
		{
			get { return CollectionUtils.Select(_table.Items, cp => cp.Deactivated); }
		}

		public void Save(ExternalPractitionerDetail practitioner)
		{
			practitioner.ContactPoints.Clear();

			// Add the originally deactivated contact points
			practitioner.ContactPoints.AddRange(_deactivatedContactPointNotShown);

			// Add the items in the table.
			practitioner.ContactPoints.AddRange(_table.Items);
		}

		#region Presentation Models

		public string Instruction
		{
			get { return SR.MessageInstructionSelectContactPoints; }
		}

		public ITable ContactPointTable
		{
			get { return _table; }
		}

		public ISelection SummarySelection
		{
			get { return new Selection(_selectedItem); }
			set { _selectedItem = (ExternalPractitionerContactPointDetail)value.Item; }
		}

		#endregion

		private void UpdateContactPointsTable()
		{
			if (_originalPractitioner == null || _duplicatePractitioner == null)
				return;

			var combinedContactPoints = new List<ExternalPractitionerContactPointDetail>();

			// Clone each contact points.  Do not alter the original copy.
			foreach (var cp in _originalPractitioner.ContactPoints)
				combinedContactPoints.Add((ExternalPractitionerContactPointDetail)cp.Clone());

			var originalDefaultContactPoint = CollectionUtils.SelectFirst(combinedContactPoints, cp => cp.IsDefaultContactPoint);

			// Same for the duplicate practitioner
			foreach (var cp in _duplicatePractitioner.ContactPoints)
				combinedContactPoints.Add((ExternalPractitionerContactPointDetail)cp.Clone());

			// Store the deactivated contact point
			_deactivatedContactPointNotShown.Clear();
			_deactivatedContactPointNotShown.AddRange(CollectionUtils.Select(combinedContactPoints, cp => cp.Deactivated));

			// and only show the originally active contact points to user
			_table.Items.Clear();
			_table.Items.AddRange(CollectionUtils.Select(combinedContactPoints, cp => !cp.Deactivated));

			// There may be two default contact point, one from origial, one from duplicate
			// Make sure only one is checked as default
			_table.MakeDefaultContactPoint(originalDefaultContactPoint, true);
		}
	}
}
