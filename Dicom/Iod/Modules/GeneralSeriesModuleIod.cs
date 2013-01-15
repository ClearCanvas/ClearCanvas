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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.PerformedProcedureStepSummary;
using ClearCanvas.Dicom.Iod.Macros.PerformedProcedureStepSummary.PerformedProtocolCodeSequence;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralSeries Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.3.1 (Table C.7-5a)</remarks>
	public class GeneralSeriesModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralSeriesModuleIod"/> class.
		/// </summary>	
		public GeneralSeriesModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralSeriesModuleIod"/> class.
		/// </summary>
		public GeneralSeriesModuleIod(IDicomAttributeProvider provider) : base(provider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.BodyPartExamined = null;
			this.CommentsOnThePerformedProcedureStep = null;
			this.LargestPixelValueInSeries = null;
			this.Laterality = null;
			//this.Modality = Modality.None;
			this.OperatorIdentificationSequence =null;
			this.OperatorsName = null;
			this.PatientPosition = null;
			this.PerformedProcedureStepDescription = null;
			this.PerformedProcedureStepId = null;
			this.PerformedProcedureStepStartDateTime = null;
			this.PerformedProtocolCodeSequence = null;
			this.PerformingPhysicianIdentificationSequence = null;
			this.PerformingPhysiciansName = null;
			this.ProtocolName = null;
			this.ReferencedPerformedProcedureStepSequence = null;
			this.RelatedSeriesSequence = null;
			this.RequestAttributesSequence = null;
			this.SeriesDateTime = null;
			this.SeriesDescription = null;
			this.SeriesInstanceUid  = "1";
			this.SeriesNumber = null;
			this.SmallestPixelValueInSeries = null;
			this.AnatomicalOrientationType = AnatomicalOrientationType.None;
		}

		/// <summary>
		/// Gets or sets the value of Modality in the underlying collection. Type 1.
		/// </summary>
		public Modality Modality
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.Modality].GetString(0, string.Empty), Modality.None); }
			set
			{
				if (value == Modality.None)
					throw new ArgumentOutOfRangeException("value", "Modality is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.Modality], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SeriesInstanceUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesNumber in the underlying collection. Type 2.
		/// </summary>
		public int? SeriesNumber
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[DicomTags.SeriesNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.SeriesNumber].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.SeriesNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Laterality in the underlying collection. Type 2C.
		/// </summary>
		public string Laterality
		{
			get { return base.DicomAttributeProvider[DicomTags.Laterality].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.Laterality] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.Laterality].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesDate and SeriesTime in the underlying collection.  Type 3.
		/// </summary>
		public DateTime? SeriesDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.SeriesDate].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.SeriesTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.SeriesDate] = null;
					base.DicomAttributeProvider[DicomTags.SeriesTime] = null;
					return;
				}
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.SeriesDate];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.SeriesTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformingPhysiciansName in the underlying collection. Type 3.
		/// </summary>
		public string PerformingPhysiciansName
		{
			get { return base.DicomAttributeProvider[DicomTags.PerformingPhysiciansName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PerformingPhysiciansName] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PerformingPhysiciansName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformingPhysicianIdentificationSequence in the underlying collection. Type 3.
		/// </summary>
		public PersonIdentificationMacro[] PerformingPhysicianIdentificationSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PerformingPhysicianIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				PersonIdentificationMacro[] result = new PersonIdentificationMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PersonIdentificationMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.PerformingPhysicianIdentificationSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.PerformingPhysicianIdentificationSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of ProtocolName in the underlying collection. Type 3.
		/// </summary>
		public string ProtocolName
		{
			get { return base.DicomAttributeProvider[DicomTags.ProtocolName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ProtocolName] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ProtocolName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesDescription in the underlying collection. Type 3.
		/// </summary>
		public string SeriesDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.SeriesDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.SeriesDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.SeriesDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OperatorsName in the underlying collection. Type 3.
		/// </summary>
		public string OperatorsName
		{
			get { return base.DicomAttributeProvider[DicomTags.OperatorsName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.OperatorsName] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.OperatorsName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OperatorIdentificationSequence in the underlying collection. Type 3.
		/// </summary>
		public PersonIdentificationMacro[] OperatorIdentificationSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.OperatorIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				PersonIdentificationMacro[] result = new PersonIdentificationMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PersonIdentificationMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.OperatorIdentificationSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.OperatorIdentificationSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedPerformedProcedureStepSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedPerformedProcedureStepSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ReferencedPerformedProcedureStepSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro CreateReferencedPerformedProcedureStepSequence()
		{
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] {dicomSequenceItem};
				ISopInstanceReferenceMacro sequenceType = new SopInstanceReferenceMacro(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of RelatedSeriesSequence in the underlying collection. Type 3.
		/// </summary>
		public RelatedSeriesSequence[] RelatedSeriesSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.RelatedSeriesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				RelatedSeriesSequence[] result = new RelatedSeriesSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new RelatedSeriesSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.RelatedSeriesSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.RelatedSeriesSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of BodyPartExamined in the underlying collection. Type 3.
		/// </summary>
		public string BodyPartExamined
		{
			get { return base.DicomAttributeProvider[DicomTags.BodyPartExamined].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.BodyPartExamined] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.BodyPartExamined].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientPosition in the underlying collection. Type 2C.
		/// </summary>
		public string PatientPosition
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientPosition].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PatientPosition] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientPosition].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SmallestPixelValueInSeries in the underlying collection. Type 3.
		/// </summary>
		public int? SmallestPixelValueInSeries
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[DicomTags.SmallestPixelValueInSeries].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.SmallestPixelValueInSeries] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.SmallestPixelValueInSeries].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of LargestPixelValueInSeries in the underlying collection. Type 3.
		/// </summary>
		public int? LargestPixelValueInSeries
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[DicomTags.LargestPixelValueInSeries].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.LargestPixelValueInSeries] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.LargestPixelValueInSeries].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RequestAttributesSequence in the underlying collection. Type 3.
		/// </summary>
		public IRequestAttributesMacro[] RequestAttributesSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.RequestAttributesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				IRequestAttributesMacro[] result = new IRequestAttributesMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new RequestAttributesMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.RequestAttributesSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.RequestAttributesSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a RequestAttributesSequence item. Does not modify the RequestAttributesSequence in the underlying collection.
		/// </summary>
		public IRequestAttributesMacro CreateRequestAttributesSequence()
		{
			IRequestAttributesMacro iodBase = new RequestAttributesMacro(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of PerformedProcedureStepId in the underlying collection. Type 3.
		/// </summary>
		public string PerformedProcedureStepId
		{
			get { return base.DicomAttributeProvider[DicomTags.PerformedProcedureStepId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PerformedProcedureStepId] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PerformedProcedureStepId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformedProcedureStepStartDate and PerformedProcedureStepStartTime in the underlying collection.  Type 3.
		/// </summary>
		public DateTime? PerformedProcedureStepStartDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.PerformedProcedureStepStartDate].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.PerformedProcedureStepStartTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.PerformedProcedureStepStartDate] = null;
					base.DicomAttributeProvider[DicomTags.PerformedProcedureStepStartTime] = null;
					return;
				}
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.PerformedProcedureStepStartDate];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.PerformedProcedureStepStartTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformedProcedureStepDescription in the underlying collection. Type 3.
		/// </summary>
		public string PerformedProcedureStepDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.PerformedProcedureStepDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PerformedProcedureStepDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PerformedProcedureStepDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformedProtocolCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public IPerformedProtocolCodeSequence[] PerformedProtocolCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PerformedProtocolCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				IPerformedProtocolCodeSequence[] result = new IPerformedProtocolCodeSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PerformedProtocolCodeSequenceClass(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.PerformedProtocolCodeSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.PerformedProtocolCodeSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PerformedProtocolCodeSequence item. Does not modify the PerformedProtocolCodeSequence in the underlying collection.
		/// </summary>
		public IPerformedProtocolCodeSequence CreatePerformedProtocolCodeSequence()
		{
			IPerformedProtocolCodeSequence iodBase = new PerformedProtocolCodeSequenceClass(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of CommentsOnThePerformedProcedureStep in the underlying collection. Type 3.
		/// </summary>
		public string CommentsOnThePerformedProcedureStep
		{
			get { return base.DicomAttributeProvider[DicomTags.CommentsOnThePerformedProcedureStep].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.CommentsOnThePerformedProcedureStep] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.CommentsOnThePerformedProcedureStep].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the anatomical orientation type.  Type 1C
		/// </summary>
		public AnatomicalOrientationType AnatomicalOrientationType
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.AnatomicalOrientationType].GetString(0, String.Empty), AnatomicalOrientationType.None); }
			set
			{
				if (value == AnatomicalOrientationType.None)
				{
					base.DicomAttributeProvider[DicomTags.AnatomicalOrientationType].SetNullValue();
					return;
				}

				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.AnatomicalOrientationType], value);
			}
		}

		/// <summary>
		/// PerformedProtocol Code Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.13 (Table 10-16)</remarks>
		internal class PerformedProtocolCodeSequenceClass : CodeSequenceMacro, IPerformedProtocolCodeSequence
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PerformedProtocolCodeSequenceClass"/> class.
			/// </summary>
			public PerformedProtocolCodeSequenceClass() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="PerformedProtocolCodeSequenceClass"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public PerformedProtocolCodeSequenceClass(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Initializes the underlying collection to implement the module or sequence using default values.
			/// </summary>
			public virtual void InitializeAttributes() {}

			/// <summary>
			/// Gets or sets the value of ProtocolContextSequence in the underlying collection. Type 3.
			/// </summary>
			public IProtocolContextSequence[] ProtocolContextSequence
			{
				get
				{
					DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ProtocolContextSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					ProtocolContextSequenceClass[] result = new ProtocolContextSequenceClass[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new ProtocolContextSequenceClass(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						base.DicomAttributeProvider[DicomTags.ProtocolContextSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					base.DicomAttributeProvider[DicomTags.ProtocolContextSequence].Values = result;
				}
			}

			/// <summary>
			/// Creates a single instance of a ProtocolContextSequence item. Does not modify the tag in the underlying collection.
			/// </summary>
			public IProtocolContextSequence CreateProtocolContextSequence()
			{
				IProtocolContextSequence iodBase = new ProtocolContextSequenceClass(new DicomSequenceItem());
				iodBase.InitializeAttributes();
				return iodBase;
			}

			/// <summary>
			/// ProtocolContext Sequence
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.13 (Table 10-16)</remarks>
			internal class ProtocolContextSequenceClass : ContentItemMacro, IProtocolContextSequence
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="ProtocolContextSequenceClass"/> class.
				/// </summary>
				public ProtocolContextSequenceClass() : base() {}

				/// <summary>
				/// Initializes a new instance of the <see cref="ProtocolContextSequenceClass"/> class.
				/// </summary>
				/// <param name="dicomSequenceItem">The dicom sequence item.</param>
				public ProtocolContextSequenceClass(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

				/// <summary>
				/// Initializes the underlying collection to implement the module or sequence using default values.
				/// </summary>
				public virtual void InitializeAttributes() {}

				/// <summary>
				/// Gets or sets the value of ContentItemModifierSequence in the underlying collection. Type 3.
				/// </summary>
				public ContentItemMacro[] ContentItemModifierSequence
				{
					get
					{
						DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentItemModifierSequence];
						if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
						{
							return null;
						}

						ContentItemMacro[] result = new ContentItemMacro[dicomAttribute.Count];
						DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
						for (int n = 0; n < items.Length; n++)
							result[n] = new ContentItemMacro(items[n]);

						return result;
					}
					set
					{
						if (value == null || value.Length == 0)
						{
							base.DicomAttributeProvider[DicomTags.ContentItemModifierSequence] = null;
							return;
						}

						DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
						for (int n = 0; n < value.Length; n++)
							result[n] = value[n].DicomSequenceItem;

						base.DicomAttributeProvider[DicomTags.ContentItemModifierSequence].Values = result;
					}
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.BodyPartExamined;
				yield return DicomTags.CommentsOnThePerformedProcedureStep;
				yield return DicomTags.LargestPixelValueInSeries;
				yield return DicomTags.Laterality;
				yield return DicomTags.Modality;
				yield return DicomTags.OperatorIdentificationSequence;
				yield return DicomTags.OperatorsName;
				yield return DicomTags.PatientPosition;
				yield return DicomTags.PerformedProcedureStepDescription;
				yield return DicomTags.PerformedProcedureStepId;
				yield return DicomTags.PerformedProcedureStepStartDate;
				yield return DicomTags.PerformedProcedureStepStartTime;
				yield return DicomTags.PerformedProtocolCodeSequence;
				yield return DicomTags.PerformingPhysicianIdentificationSequence;
				yield return DicomTags.PerformingPhysiciansName;
				yield return DicomTags.ProtocolName;
				yield return DicomTags.ReferencedPerformedProcedureStepSequence;
				yield return DicomTags.RelatedSeriesSequence;
				yield return DicomTags.RequestAttributesSequence;
				yield return DicomTags.SeriesDate;
				yield return DicomTags.SeriesTime;
				yield return DicomTags.SeriesDescription;
				yield return DicomTags.SeriesInstanceUid;
				yield return DicomTags.SeriesNumber;
				yield return DicomTags.SmallestPixelValueInSeries;
				yield return DicomTags.AnatomicalOrientationType;
			}
		}
	}
}