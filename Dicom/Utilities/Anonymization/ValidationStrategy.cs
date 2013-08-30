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
using System.Collections.ObjectModel;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	/// <summary>
	/// An enumeration of flags to control the behaviour of the <see cref="DicomAnonymizer.ValidationStrategy"/>.
	/// </summary>
	[FlagsAttribute]
	public enum ValidationOptions : uint
	{

		#region Individual Flags

		/// <summary>
		/// Indicates that the <see cref="DicomAnonymizer.ValidationStrategy"/> should not enforce a non-empty patient ID in the anonymized data set.
		/// </summary>
		AllowEmptyPatientId = 0x01,

		/// <summary>
		/// Indicates that the <see cref="DicomAnonymizer.ValidationStrategy"/> should not enforce a non-empty patient name in the anonymized data set.
		/// </summary>
		AllowEmptyPatientName = 0x02,

		/// <summary>
		/// Indicates that the <see cref="DicomAnonymizer.ValidationStrategy"/> should not enforce a different patient's birthdate in the anonymized data set.
		/// </summary>
		AllowEqualBirthDate = 0x04,

		/// <summary>
		/// Indicates that the <see cref="DicomAnonymizer.ValidationStrategy"/> should allow unchanged values in the anonymized data set.
		/// </summary>
		AllowUnchangedValues = 0x08,

		#endregion

		#region Group Flags

		/// <summary>
		/// Indicates that the <see cref="DicomAnonymizer.ValidationStrategy"/> should relax all optional attribute value checks in the anonymized data set.
		/// </summary>
		RelaxAllChecks = AllowEmptyPatientId | AllowEmptyPatientName | AllowEqualBirthDate,

		/// <summary>
		/// Indicates that the <see cref="DicomAnonymizer.ValidationStrategy"/>  should use its default behaviour, which is to enforce non-empty and different values in all checked attributes.
		/// </summary>
		Default = 0x0

		#endregion
	}

	/// <summary>
	/// Reason for the validation failure.
	/// </summary>
	public enum ValidationFailureReason
	{
		EmptyValue,
		ConflictingValue
	}

	/// <summary>
	/// Describes the validation failure.
	/// </summary>
	/// <remarks>
	/// For those properties, in <see cref="SeriesData"/> and <see cref="StudyData"/>, that have a 'Raw' counterpart
	/// (e.g. <see cref="StudyData.PatientsNameRaw"/>), the <see cref="PropertyName"/> will always correspond to the non-raw
	/// property (e.g. <see cref="StudyData.PatientsName"/>).
	/// </remarks>
	public class ValidationFailureDescription
	{
		internal ValidationFailureDescription(string propertyName, ValidationFailureReason reason, string description)
		{
			PropertyName = propertyName;
			Reason = reason;
			Description = description;
		}

		public readonly string PropertyName;
		public readonly ValidationFailureReason Reason;
		public readonly string Description;
	}

	public partial class DicomAnonymizer
	{
		/// <summary>
		/// The strategy used by the <see cref="DicomAnonymizer"/> to validate anonymized data.
		/// </summary>
		/// <remarks>
		/// You can use this class on its own to pre-determine if your anonymized <see cref="StudyData"/> or
		/// <see cref="SeriesData"/> will be accepted by the <see cref="DicomAnonymizer"/>.
		/// </remarks>
		public class ValidationStrategy
		{
			private ValidationOptions _options = ValidationOptions.Default;

			private List<ValidationFailureDescription> _failures;


			/// <summary>
			/// Default constructor.
			/// </summary>
			public ValidationStrategy()
			{
			}

			/// <summary>
			/// Gets or sets the validation options.
			/// </summary>
			public ValidationOptions Options
			{
				get { return _options; }
				set { _options = value; }
			}

			public ReadOnlyCollection<ValidationFailureDescription> GetValidationFailures(StudyData originalData, StudyData anonymizedData)
			{
				_failures = new List<ValidationFailureDescription>();

				if (!IsOptionSet(_options, ValidationOptions.AllowUnchangedValues))
				{
					ValidatePatientNamesNotEqual(originalData.PatientsName, anonymizedData.PatientsName);
					ValidateNotEqual(originalData.PatientId, anonymizedData.PatientId, "PatientId");
					ValidateNotEqual(originalData.AccessionNumber, anonymizedData.AccessionNumber, "AccessionNumber");
					ValidateNotEqual(originalData.StudyId, anonymizedData.StudyId, "StudyId");
				}

				if (!IsOptionSet(_options, ValidationOptions.AllowEqualBirthDate))
					ValidateNotEqual(originalData.PatientsBirthDateRaw, anonymizedData.PatientsBirthDateRaw, "PatientsBirthDate");
				if (!IsOptionSet(_options, ValidationOptions.AllowEmptyPatientId))
					ValidateNotEmpty(anonymizedData.PatientId, "PatientId");
				if (!IsOptionSet(_options, ValidationOptions.AllowEmptyPatientName))
					ValidateNotEmpty(anonymizedData.PatientsNameRaw, "PatientsName");

				ReadOnlyCollection<ValidationFailureDescription> failures = _failures.AsReadOnly();
				_failures = null;
				return failures;
			}

			/// <summary>
			/// Gets a list of <see cref="ValidationFailureDescription"/>s describing all validation failures.
			/// </summary>
			/// <remarks>
			/// When an empty list is returned, it means there were no validation failures.
			/// </remarks>
			internal ReadOnlyCollection<ValidationFailureDescription> GetValidationFailures(SeriesData originalData, SeriesData anonymizedData)
			{
				_failures = new List<ValidationFailureDescription>();
				ReadOnlyCollection<ValidationFailureDescription> failures = _failures.AsReadOnly();

				//nothing to validate for now.

				_failures = null;
				return failures;
			}

			/// <summary>
			/// Gets a list of <see cref="ValidationFailureDescription"/>s describing all validation failures.
			/// </summary>
			/// <remarks>
			/// When an empty list is returned, it means there were no validation failures.
			/// </remarks>
			private void ValidatePatientNamesNotEqual(PersonName original, PersonName anonymized)
			{
				ValidatePatientNamesNotEqual(original.SingleByte, anonymized.SingleByte, "SingleByte");
				ValidatePatientNamesNotEqual(original.Ideographic, anonymized.Ideographic, "Ideographic");
				ValidatePatientNamesNotEqual(original.Phonetic, anonymized.Phonetic, "Phonetic");
			}

			private void ValidatePatientNamesNotEqual(ComponentGroup original, ComponentGroup anonymized, string componentGroup)
			{
				string format = "The anonymized name component ({0}:{1}) cannot be the same as the original.";

				//may not have the same family, given, or middle name
				ValidateNotEqual(original.FamilyName, anonymized.FamilyName, "PatientsName", String.Format(format, componentGroup, "Family"));
				ValidateNotEqual(original.GivenName, anonymized.GivenName, "PatientsName", String.Format(format, componentGroup, "Given"));
				ValidateNotEqual(original.MiddleName, anonymized.MiddleName, "PatientsName", String.Format(format, componentGroup, "Middle"));
			}

			private void ValidateNotEmpty(string value, string property)
			{
				ValidateNotEmpty(value, property, null);
			}

			private void ValidateNotEmpty(string value, string property, string description)
			{
				if (String.IsNullOrEmpty(value))
					_failures.Add(new ValidationFailureDescription(property, ValidationFailureReason.EmptyValue, description ?? "The value cannot be empty."));
			}

			private void ValidateNotEqual(string original, string anonymized, string property)
			{
				ValidateNotEqual(original, anonymized, property, null);
			}

			private void ValidateNotEqual(string original, string anonymized, string property, string description)
			{
				if (String.IsNullOrEmpty(original) && String.IsNullOrEmpty(anonymized))
					return;

				if (String.Compare(original ?? "", anonymized ?? "", true) == 0)
					_failures.Add(new ValidationFailureDescription(property, ValidationFailureReason.ConflictingValue, description ?? "The anonymized value cannot be unchanged from the original."));
			}

			private static bool IsOptionSet(ValidationOptions options, ValidationOptions flag)
			{
				return (options & flag) == flag;
			}
		}
	}
}
