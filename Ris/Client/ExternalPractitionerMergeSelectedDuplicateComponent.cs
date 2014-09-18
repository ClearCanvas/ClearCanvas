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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectedDuplicateComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectedDuplicateComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectedDuplicateComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectedDuplicateComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectedDuplicateComponent : ApplicationComponent
	{
		private class ExternalPractitionerTable : Table<ExternalPractitionerSummary>
		{
			private readonly ExternalPractitionerMergeSelectedDuplicateComponent _owner;

			public ExternalPractitionerTable(ExternalPractitionerMergeSelectedDuplicateComponent owner)
			{
				_owner = owner;

				var nameColumn = new TableColumn<ExternalPractitionerSummary, string>(SR.ColulmnPractitionerName,
					prac => PersonNameFormat.Format(prac.Name), 0.5f)
					{ ClickLinkDelegate = _owner.LaunchSelectedPractitionerPreview };

				var licenseColumn = new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
					prac => prac.LicenseNumber, 0.25f);

				var billingColumn = new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
					prac => prac.BillingNumber, 0.25f);

				this.Columns.Add(nameColumn);
				this.Columns.Add(licenseColumn);
				this.Columns.Add(billingColumn);
			}
		}

		private readonly ExternalPractitionerTable _table;
		private readonly EntityRef _specifiedDuplicatePractitionerRef;
		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerSummary _selectedItem;
		private event EventHandler _selectedItemChanged;

		public ExternalPractitionerMergeSelectedDuplicateComponent()
			: this(null)
		{
		}

		public ExternalPractitionerMergeSelectedDuplicateComponent(EntityRef specifiedDuplicatePractitionerRef)
		{
			_specifiedDuplicatePractitionerRef = specifiedDuplicatePractitionerRef;
			_table = new ExternalPractitionerTable(this);
		}

		public override void Start()
		{
			// Must select at least one practitioner
			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(_selectedItem != null, SR.MessageValidationSelectDuplicate)));

			base.Start();
		}

		public ExternalPractitionerDetail OriginalPractitioner
		{
			get { return _originalPractitioner; }
			set
			{
				if (_originalPractitioner != null && _originalPractitioner.Equals(value))
					return;

				_originalPractitioner = value;

				_table.Items.Clear();
				if (_originalPractitioner == null)
					return;

				if (_specifiedDuplicatePractitionerRef == null)
				{
					var duplicates = LoadDuplicates(_originalPractitioner.PractitionerRef);
					_table.Items.AddRange(duplicates);
				}
				else
				{
					var duplicatePractitioner = LoadPractitionerDetail(_specifiedDuplicatePractitionerRef);
					_table.Items.AddRange(new List<ExternalPractitionerSummary> { duplicatePractitioner.CreateSummary() });
					
				}
			}
		}

		public ExternalPractitionerSummary SelectedPractitioner
		{
			get { return _selectedItem; }
		}

		public event EventHandler SelectedPractitionerChanged
		{
			add { _selectedItemChanged += value; }
			remove { _selectedItemChanged -= value; }
		}

		#region Presentation Models

		public string Name
		{
			get { return _originalPractitioner == null ? null : PersonNameFormat.Format(_originalPractitioner.Name); }
		}

		public string LicenseNumber
		{
			get { return _originalPractitioner == null ? null : _originalPractitioner.LicenseNumber; }
		}

		public string BillingNumber
		{
			get { return _originalPractitioner == null ? null : _originalPractitioner.BillingNumber; }
		}

		public string Instruction
		{
			get { return SR.MessageInstructionSelectDuplicate; }
		}

		public ITable PractitionerTable
		{
			get { return _table; }
		}

		public ISelection SummarySelection
		{
			get { return new Selection(_selectedItem); }
			set
			{
				var previousSelection = new Selection(_selectedItem);
				if (previousSelection.Equals(value)) 
					return;

				_selectedItem = (ExternalPractitionerSummary) value.Item;

				NotifyPropertyChanged("SummarySelection");
				EventsHelper.Fire(_selectedItemChanged, this, EventArgs.Empty);
			}
		}

		#endregion

		private static List<ExternalPractitionerSummary> LoadDuplicates(EntityRef practitionerRef)
		{
			var duplicates = new List<ExternalPractitionerSummary>();

			if (practitionerRef != null)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadMergeExternalPractitionerFormDataRequest { PractitionerRef = practitionerRef };
						var response = service.LoadMergeExternalPractitionerFormData(request);

						duplicates = response.Duplicates;
					});
			}

			return duplicates;
		}

		private static ExternalPractitionerDetail LoadPractitionerDetail(EntityRef practitionerRef)
		{
			ExternalPractitionerDetail detail = null;

			if (practitionerRef != null)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadExternalPractitionerForEditRequest(practitionerRef);
						var response = service.LoadExternalPractitionerForEdit(request);
						detail = response.PractitionerDetail;
					});
			}

			return detail;
		}

		private void LaunchSelectedPractitionerPreview(object practitioner)
		{
			var component = new ExternalPractitionerOverviewComponent { PractitionerSummary = (ExternalPractitionerSummary) practitioner };
			LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitlePractitioner);
		}
	}
}
