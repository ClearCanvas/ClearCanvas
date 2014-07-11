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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="MultipleProceduresEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class MultipleProceduresEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MultipleProceduresEditorComponent class.
	/// </summary>
	[AssociateView(typeof(MultipleProceduresEditorComponentViewExtensionPoint))]
	public class MultipleProceduresEditorComponent : ProcedureEditorComponentBase
	{
		private readonly List<ProcedureRequisition> _requisitions;

		private bool _isScheduledTimeEditable;
		private bool _isScheduledDurationEditable;
		private bool _isPerformingFacilityEditable;
		private bool _isPerformingDepartmentEditable;
		private bool _isLateralityEditable;
		private bool _isSchedulingCodeEditable;
		private bool _isPortableEditable;
		private bool _isCheckedInEditable;
		private bool _isModalityEditable;

		public MultipleProceduresEditorComponent(List<ProcedureRequisition> requisitions,
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<ModalitySummary> modalityChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
			: base(facilityChoices, departmentChoices, modalityChoices, lateralityChoices, schedulingCodeChoices)
		{
			Platform.CheckForNullReference(requisitions, "requisitions");

			_requisitions = requisitions;
		}

		public override void Start()
		{
			// This validation shows the icon beside the scheduled time if it's being edited
			this.Validation.Add(new ValidationRule("ScheduledTime",
				delegate
				{
					return this.IsScheduledDateTimeEditable ? ValidateCheckInTime() : new ValidationResult(true, "");
				}));

			// This validation shows the icon beside the checkedIn if it's being edited
			this.Validation.Add(new ValidationRule("CheckedIn",
				delegate
				{
					return this.IsCheckedInEditable ? ValidateCheckInTime() : new ValidationResult(true, "");
				}));

			// This validation shows the icon beside the modality if it's being edited
			this.Validation.Add(new ValidationRule("SelectedModality",
				delegate
				{
					if (!this.IsModalityEditable)
						return new ValidationResult(true, "");

					if (this.SelectedModality == null)
						return new ValidationResult(false, SR.MessageValueRequired);

					return ValidateModalityAndFacility("Modality is not valid for performing facility.");
				}));

			// This validation shows the icon beside the facility if it's being edited
			this.Validation.Add(new ValidationRule("SelectedFacility",
				delegate
				{
					return this.IsPerformingFacilityEditable ? ValidateModalityAndFacility("Performing facility is not valid for all procedures' modalities.") : new ValidationResult(true, "");
				}));

			base.Start();
		}

		protected override void LoadFromRequisition()
		{
			this.ScheduledTime = GetCommonValue(_requisitions, r => r.ScheduledTime);
			this.ScheduledDuration = GetCommonValue(_requisitions, r => r.ScheduledDuration);
			this.SelectedFacility = GetCommonValue(_requisitions, r => r.PerformingFacility);
			this.SelectedDepartment = GetCommonValue(_requisitions, r => r.PerformingDepartment);
			this.SelectedModality = GetCommonValue(_requisitions, r => r.Modality);
			this.SelectedLaterality = GetCommonValue(_requisitions, r => r.Laterality);
			this.SelectedSchedulingCode = GetCommonValue(_requisitions, r => r.SchedulingCode);
			this.PortableModality = GetCommonValue(_requisitions, r => r.PortableModality);
			this.CheckedIn = GetCommonValue(_requisitions, r => r.CheckedIn);
		}

		protected override void UpdateRequisition()
		{
			foreach (var requisition in _requisitions)
			{
				if (_isScheduledTimeEditable)
					requisition.ScheduledTime = this.ScheduledTime;

				if (_isScheduledDurationEditable)
					requisition.ScheduledDuration = this.ScheduledDuration;

				if (_isPerformingFacilityEditable)
					requisition.PerformingFacility = this.SelectedFacility;

				if (_isPerformingDepartmentEditable)
					requisition.PerformingDepartment = this.SelectedDepartment;

				if (_isModalityEditable)
					requisition.Modality = this.SelectedModality;

				if (_isLateralityEditable)
					requisition.Laterality = this.SelectedLaterality;

				if (_isSchedulingCodeEditable)
					requisition.SchedulingCode = this.SelectedSchedulingCode;

				if (_isPortableEditable)
					requisition.PortableModality = this.PortableModality;

				if (_isCheckedInEditable)
					requisition.CheckedIn = this.CheckedIn;
			}
		}

		#region Presentation Models

		public override bool IsScheduledDateTimeEditable
		{
			get { return _isScheduledTimeEditable; }
			set { _isScheduledTimeEditable = value; }
		}

		public override bool IsScheduledDurationEditable
		{
			get { return _isScheduledDurationEditable; }
			set { _isScheduledDurationEditable = value; }
		}

		public override bool IsPerformingFacilityEditable
		{
			get { return _isPerformingFacilityEditable; }
			set { _isPerformingFacilityEditable = value; }
		}

		public override bool IsPerformingDepartmentEditable
		{
			get { return _isPerformingDepartmentEditable; }
			set { _isPerformingDepartmentEditable = value; }
		}

		public override bool IsModalityEditable
		{
			get { return _isModalityEditable; }
			set { _isModalityEditable = value; }
		}

		public override bool IsLateralityEditable
		{
			get { return _isLateralityEditable; }
			set { _isLateralityEditable = value; }
		}

		public override bool IsSchedulingCodeEditable
		{
			get { return _isSchedulingCodeEditable; }
			set { _isSchedulingCodeEditable = value; }
		}

		public override bool IsPortableEditable
		{
			get { return _isPortableEditable; }
			set { _isPortableEditable = value; }
		}

		public override bool IsCheckedInEditable
		{
			get { return _isCheckedInEditable; }
			set { _isCheckedInEditable = value; }
		}

		#endregion

		/// <summary>
		/// Find a common property value for a list of requisitions.
		/// </summary>
		private static TValue GetCommonValue<TValue>(
			List<ProcedureRequisition> requisitions,
			Converter<ProcedureRequisition, TValue> propertyGetter)
		{
			var mappedValues = CollectionUtils.Map(requisitions, propertyGetter);
			var uniqueValues = CollectionUtils.Unique(mappedValues);
			return uniqueValues.Count != 1 ? default(TValue) : uniqueValues[0];
		}

		private ValidationResult ValidateCheckInTime()
		{
			var checkInTime = Platform.Time;
			foreach (var r in _requisitions)
			{
				// Use the edited property if the property is being edited
				var checkedIn = this.IsCheckedInEditable ? this.CheckedIn : r.CheckedIn;
				var scheduledTime = this.IsScheduledDateTimeEditable ? this.ScheduledTime : r.ScheduledTime;

				if (!checkedIn)
					continue;

				string alertMessage;
				if (CheckInSettings.ValidateResult.Success ==
					CheckInSettings.Validate(scheduledTime, checkInTime, out alertMessage))
					continue;

				// Validation failed.
				if (this.IsCheckedInEditable && this.IsScheduledDateTimeEditable)
				{
					// If user is modifying both checkIn and scheduledDateTime, give them a more detail alert message.
					return new ValidationResult(false, alertMessage);
				}

				// Otherwise, they must edit each procedure individually.
				return new ValidationResult(false, SR.MessageAlertMultipleProceduresCheckInValidation);
			}

			return new ValidationResult(true, string.Empty);
		}

		private ValidationResult ValidateModalityAndFacility(string message)
		{
			// Here, we exploit the knowledge that all procedures must have the same performing facility
			// to simplify the validation logic.  Either the Modality is editable, in which case all
			// procedures will have that modality and we just test it against selected facility, or it is not editable,
			// in which case we need to test the modality of each procedure.
			var valid = IsModalityEditable ? 
				IsModalityValidForFacility(this.SelectedModality, this.SelectedFacility)
				: _requisitions.All(r => IsModalityValidForFacility(r.Modality, this.SelectedFacility));
			return new ValidationResult(valid, message);
		}

	}
}
