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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// ProcedureEditorComponentBase class
	/// </summary>
	public abstract class ProcedureEditorComponentBase : ApplicationComponent
	{
		private readonly List<FacilitySummary> _facilityChoices;
		private readonly List<DepartmentSummary> _allDepartments;
		private List<DepartmentSummary> _departmentChoices;
		private readonly List<ModalitySummary> _allModalities;
		private List<ModalitySummary> _modalityChoices;
		private readonly DepartmentSummary _departmentNone = new DepartmentSummary(null, SR.DummyItemNone, null, null, null, true);
		private readonly ModalitySummary _modalityNone = new ModalitySummary(null, SR.DummyItemNone, null, null, null, null, true);
		private readonly List<EnumValueInfo> _lateralityChoices;
		private readonly List<EnumValueInfo> _schedulingCodeChoices;
		private readonly EnumValueInfo _schedulingCodeNone = new EnumValueInfo(null, SR.DummyItemNone);

		private DateTime? _scheduledDateTime;
		private int _scheduledDuration;
		private FacilitySummary _selectedFacility;
		private DepartmentSummary _selectedDepartment;
		private ModalitySummary _selectedModality;
		private EnumValueInfo _selectedLaterality;
		private EnumValueInfo _selectedSchedulingCode;
		private bool _portableModality;
		private bool _checkedIn;

		protected ProcedureEditorComponentBase(
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<ModalitySummary> modalityChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
		{
			Platform.CheckForNullReference(facilityChoices, "facilityChoices");
			Platform.CheckForNullReference(departmentChoices, "departmentChoices");
			Platform.CheckForNullReference(modalityChoices, "modalityChoices");
			Platform.CheckForNullReference(lateralityChoices, "lateralityChoices");
			Platform.CheckForNullReference(schedulingCodeChoices, "schedulingCodeChoices");

			_facilityChoices = facilityChoices;
			_allDepartments = departmentChoices;
			_lateralityChoices = lateralityChoices;
			_allModalities = modalityChoices;

			_schedulingCodeChoices = new List<EnumValueInfo> {_schedulingCodeNone};
			_schedulingCodeChoices.AddRange(schedulingCodeChoices);

		}

		public override void Start()
		{
			LoadFromRequisition();

			// update department choices based on selected facility
			UpdateDepartmentChoices();

			base.Start();
		}

		protected abstract void LoadFromRequisition();

		protected abstract void UpdateRequisition();

		#region Presentation Model

		public virtual bool IsScheduledDateTimeEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsScheduledDurationEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsPerformingFacilityEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsPerformingDepartmentEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsModalityEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsLateralityEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsSchedulingCodeEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsPortableEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsCheckedInEditable
		{
			get { return true; }
			set { }
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		public string FormatFacility(object facility)
		{
			return ((FacilitySummary) facility).Name;
		}

		[ValidateNotNull]
		public FacilitySummary SelectedFacility
		{
			get { return _selectedFacility; }
			set
			{
				if (Equals(value, _selectedFacility))
					return;

				_selectedFacility = value;
				NotifyPropertyChanged("SelectedFacility");

				UpdateDepartmentChoices();
				NotifyPropertyChanged("DepartmentChoicesChanged");

				UpdateModalityChoices();
				NotifyPropertyChanged("ModalityChoicesChanged");

				// clear selection
				this.SelectedDepartment = null;
			}
		}

		public IList DepartmentChoices
		{
			get { return _departmentChoices; }
		}

		public string FormatDepartment(object department)
		{
			return department == null || Equals(department, _departmentNone) ? "" : ((DepartmentSummary)department).Name;
		}

		public DepartmentSummary SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				if (Equals(value, _selectedDepartment))
					return;

				// It's important to convert _departemntNone to null here, in order for "not-null" custom validation rules
				// to behave as expected
				_selectedDepartment = Equals(value, _departmentNone) ? null : value;
				NotifyPropertyChanged("SelectedDepartment");
			}
		}

		public IList ModalityChoices
		{
			get { return _modalityChoices; }
		}

		public string FormatModality(object modality)
		{
			return modality == null || Equals(modality, _modalityNone) ? "" : ((ModalitySummary)modality).Name;
		}

		public ModalitySummary SelectedModality
		{
			get { return _selectedModality; }
			set
			{
				if (Equals(value, _selectedModality))
					return;

				// It's important to convert _modalityNone to null here, in order for "not-null" custom validation rules
				// to behave as expected
				_selectedModality = Equals(value, _modalityNone) ? null : value;
				NotifyPropertyChanged("SelectedModality");
			}
		}

		public IList LateralityChoices
		{
			get { return _lateralityChoices; }
		}

		public EnumValueInfo SelectedLaterality
		{
			get { return _selectedLaterality; }
			set
			{
				if (Equals(value, _selectedLaterality))
					return;

				_selectedLaterality = value;
				NotifyPropertyChanged("SelectedLaterality");
			}
		}

		public IList SchedulingCodeChoices
		{
			get { return _schedulingCodeChoices; }
		}

		public string FormatSchedulingCode(object schedulingCode)
		{
			return schedulingCode == null || Equals(schedulingCode, _schedulingCodeNone) ? "" : ((EnumValueInfo)schedulingCode).Value;
		}

		public EnumValueInfo SelectedSchedulingCode
		{
			get { return _selectedSchedulingCode; }
			set
			{
				if (Equals(value, _selectedSchedulingCode))
					return;

				_selectedSchedulingCode = Equals(value, _schedulingCodeNone) ? null : value;
				NotifyPropertyChanged("SelectedSchedulingCode");
			}
		}

		public DateTime? ScheduledDate
		{
			get { return _scheduledDateTime; }
			set
			{
				if (value == _scheduledDateTime)
					return;

				_scheduledDateTime = value;
				NotifyPropertyChanged("ScheduledDate");
				NotifyPropertyChanged("ScheduledTime");
			}
		}

		public DateTime? ScheduledTime
		{
			get { return this.ScheduledDate; }
			set { this.ScheduledDate = value; }
		}

		public int ScheduledDuration
		{
			get { return _scheduledDuration; }
			set
			{
				if (value == _scheduledDuration)
					return;

				_scheduledDuration = value;
				NotifyPropertyChanged("ScheduledDuration");
			}
		}

		public bool PortableModality
		{
			get { return _portableModality; }
			set
			{
				if (value == _portableModality)
					return;

				_portableModality = value;
				NotifyPropertyChanged("PortableModality");
			}
		}

		public bool CheckedIn
		{
			get { return _checkedIn; }
			set
			{
				if (value == _checkedIn)
					return;

				_checkedIn = value;
				NotifyPropertyChanged("CheckedIn");
			}
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			UpdateRequisition();

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		protected static bool IsModalityValidForFacility(ModalitySummary modality, FacilitySummary facility)
		{
			return modality == null
				   || facility == null
				   || modality.Facility == null
				   || modality.Facility.Code == facility.Code;
		}

		private void UpdateDepartmentChoices()
		{
			_departmentChoices = new List<DepartmentSummary>{_departmentNone};

			// limit department choices to those that are associated with the selected performing facility
			if (_selectedFacility == null)
				return;

			var departmentsForSelectedFacility = CollectionUtils.Select(_allDepartments, d => d.FacilityCode == _selectedFacility.Code);
			_departmentChoices.AddRange(departmentsForSelectedFacility);
		}

		private void UpdateModalityChoices()
		{
			_modalityChoices = new List<ModalitySummary>{_modalityNone};

			// limit department choices to those that are associated with the selected performing facility
			if (_selectedFacility == null)
				return;

			var modalitiesForSelectedFacility = CollectionUtils.Select(_allModalities, m => IsModalityValidForFacility(m, _selectedFacility));
			_modalityChoices.AddRange(modalitiesForSelectedFacility);
		}

	}
}
