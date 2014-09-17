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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Multi-Frame Functional Groups Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16 (Table C.7.16-1)</remarks>
	public class MultiFrameFunctionalGroupsModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiFrameFunctionalGroupsModuleIod"/> class.
		/// </summary>	
		public MultiFrameFunctionalGroupsModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiFrameFunctionalGroupsModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public MultiFrameFunctionalGroupsModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.SharedFunctionalGroupsSequence;
				yield return DicomTags.PerFrameFunctionalGroupsSequence;
				yield return DicomTags.InstanceNumber;
				yield return DicomTags.ContentDate;
				yield return DicomTags.ContentTime;
				yield return DicomTags.NumberOfFrames;
				yield return DicomTags.ConcatenationFrameOffsetNumber;
				yield return DicomTags.RepresentativeFrameNumber;
				yield return DicomTags.ConcatenationUid;
				yield return DicomTags.SopInstanceUidOfConcatenationSource;
				yield return DicomTags.InConcatenationNumber;
				yield return DicomTags.InConcatenationTotalNumber;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			SharedFunctionalGroupsSequence = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(SharedFunctionalGroupsSequence)
			         && IsNullOrEmpty(PerFrameFunctionalGroupsSequence));
		}

		/// <summary>
		/// Gets or sets the value of SharedFunctionalGroupsSequence in the underlying collection. Type 2.
		/// </summary>
		public FunctionalGroupsSequenceItem SharedFunctionalGroupsSequence
		{
			get
			{
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.SharedFunctionalGroupsSequence, out dicomAttribute) || dicomAttribute.IsNull)
                    return null;
                
                return new FunctionalGroupsSequenceItem(((DicomSequenceItem[])dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SharedFunctionalGroupsSequence];
				if (value == null)
				{
					dicomAttribute.SetNullValue();
					return;
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the SharedFunctionalGroupsSequence in the underlying collection. Type 2.
		/// </summary>
		public FunctionalGroupsSequenceItem CreateSharedFunctionalGroupsSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.SharedFunctionalGroupsSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FunctionalGroupsSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FunctionalGroupsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of PerFrameFunctionalGroupsSequence in the underlying collection. Type 1.
		/// </summary>
		public FunctionalGroupsSequenceItem[] PerFrameFunctionalGroupsSequence
		{
			get
			{
			    DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.PerFrameFunctionalGroupsSequence, out dicomAttribute) || dicomAttribute.IsNull)
					return null;

				var result = new FunctionalGroupsSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new FunctionalGroupsSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "PerFrameFunctionalGroupsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.PerFrameFunctionalGroupsSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PerFrameFunctionalGroupsSequence item. Does not modify the PerFrameFunctionalGroupsSequence in the underlying collection.
		/// </summary>
		public FunctionalGroupsSequenceItem CreatePerFrameFunctionalGroupsSequence()
		{
			var iodBase = new FunctionalGroupsSequenceItem(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 1.
		/// </summary>
		public int InstanceNumber
		{
			get { return DicomAttributeProvider[DicomTags.InstanceNumber].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ContentDate and ContentTime in the underlying collection.  Type 1.
		/// </summary>
		public DateTime? ContentDateTime
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.ContentDate].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.ContentTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					const string msg = "Content is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				var date = DicomAttributeProvider[DicomTags.ContentDate];
				var time = DicomAttributeProvider[DicomTags.ContentTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of NumberOfFrames in the underlying collection. Type 1.
		/// </summary>
		public int NumberOfFrames
		{
			get { return DicomAttributeProvider[DicomTags.NumberOfFrames].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.NumberOfFrames].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ConcatenationFrameOffsetNumber in the underlying collection. Type 1C.
		/// </summary>
		public int? ConcatenationFrameOffsetNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.ConcatenationFrameOffsetNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ConcatenationFrameOffsetNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ConcatenationFrameOffsetNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RepresentativeFrameNumber in the underlying collection. Type 3.
		/// </summary>
		public int? RepresentativeFrameNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.RepresentativeFrameNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RepresentativeFrameNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RepresentativeFrameNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ConcatenationUid in the underlying collection. Type 1C.
		/// </summary>
		public string ConcatenationUid
		{
			get { return DicomAttributeProvider[DicomTags.ConcatenationUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ConcatenationUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ConcatenationUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SopInstanceUidOfConcatenationSource in the underlying collection. Type 1C.
		/// </summary>
		public string SopInstanceUidOfConcatenationSource
		{
			get { return DicomAttributeProvider[DicomTags.SopInstanceUidOfConcatenationSource].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SopInstanceUidOfConcatenationSource] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SopInstanceUidOfConcatenationSource].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InConcatenationNumber in the underlying collection. Type 1C.
		/// </summary>
		public int? InConcatenationNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.InConcatenationNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.InConcatenationNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InConcatenationNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InConcatenationTotalNumber in the underlying collection. Type 3.
		/// </summary>
		public int? InConcatenationTotalNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.InConcatenationTotalNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.InConcatenationTotalNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InConcatenationTotalNumber].SetInt32(0, value.Value);
			}
		}

		#region Static Functional Group Helpers

		/// <summary>
		/// Gets the specified functional group for a specific frame in the data set.
		/// </summary>
		/// <remarks>
		/// This method automatically handles getting the correct functional group IOD class for the specified frame, regardless
		/// whether the functional group exists in the Per-Frame Functional Groups Sequence, or the Shared Functional Groups Sequence.
		/// </remarks>
		/// <typeparam name="T">The functional group type (derived class of <see cref="FunctionalGroupMacro"/>).</typeparam>
		/// <param name="dataSet">The DICOM data set of the composite image SOP instance.</param>
		/// <param name="frameNumber">The DICOM frame number to be retrieved (1-based index).</param>
		/// <returns>A new instance of <typeparamref name="T"/> wrapping the sequence item pertaining to the specified frame.</returns>
		public static T GetFunctionalGroup<T>(IDicomAttributeProvider dataSet, int frameNumber)
			where T : FunctionalGroupMacro, new()
		{
			Platform.CheckForNullReference(dataSet, "dataSet");
			Platform.CheckPositive(frameNumber, "frameNumber");

			DicomAttribute sqAttribute;
			if (dataSet.TryGetAttribute(DicomTags.PerFrameFunctionalGroupsSequence, out sqAttribute) && sqAttribute.Count >= frameNumber)
			{
				var sequenceItem = ((DicomAttributeSQ) sqAttribute)[frameNumber - 1];
				var functionalGroup = new T {DicomSequenceItem = sequenceItem};
				if (functionalGroup.HasValues())
					return functionalGroup;
			}

			if (dataSet.TryGetAttribute(DicomTags.SharedFunctionalGroupsSequence, out sqAttribute) && sqAttribute.Count > 0)
			{
				var sequenceItem = ((DicomAttributeSQ) sqAttribute)[0];
				var functionalGroup = new T {DicomSequenceItem = sequenceItem};
				if (functionalGroup.HasValues())
					return functionalGroup;
			}

			return null;
		}

		/// <summary>
		/// Gets the specified functional group for a specific frame in the data set.
		/// </summary>
		/// <remarks>
		/// This method automatically handles getting the correct functional group IOD class for the specified frame, regardless
		/// whether the functional group exists in the Per-Frame Functional Groups Sequence, or the Shared Functional Groups Sequence.
		/// </remarks>
		/// <param name="functionalGroupDescriptor">The functional group type (derived class of <see cref="FunctionalGroupMacro"/>).</param>
		/// <param name="dataSet">The DICOM data set of the composite image SOP instance.</param>
		/// <param name="frameNumber">The DICOM frame number to be retrieved (1-based index).</param>
		/// <returns>A new instance of <paramref name="functionalGroupDescriptor"/> wrapping the sequence item pertaining to the specified frame.</returns>
		public static FunctionalGroupMacro GetFunctionalGroup(FunctionalGroupDescriptor functionalGroupDescriptor, IDicomAttributeProvider dataSet, int frameNumber)
		{
			bool dummy;
			return GetFunctionalGroup(functionalGroupDescriptor, dataSet, frameNumber, out dummy);
		}

		/// <summary>
		/// Gets the specified functional group for a specific frame in the data set.
		/// </summary>
		/// <remarks>
		/// This method automatically handles getting the correct functional group IOD class for the specified frame, regardless
		/// whether the functional group exists in the Per-Frame Functional Groups Sequence, or the Shared Functional Groups Sequence.
		/// </remarks>
		/// <param name="functionalGroupDescriptor">The functional group type (derived class of <see cref="FunctionalGroupMacro"/>).</param>
		/// <param name="dataSet">The DICOM data set of the composite image SOP instance.</param>
		/// <param name="frameNumber">The DICOM frame number to be retrieved (1-based index).</param>
		/// <param name="isFrameSpecific">Returns True if the functional group was invoked in the Per-Frame Functional Groups Sequence (5200,9230); returns False otherwise.</param>
		/// <returns>A new instance of <paramref name="functionalGroupDescriptor"/> wrapping the sequence item pertaining to the specified frame,.</returns>
		public static FunctionalGroupMacro GetFunctionalGroup(FunctionalGroupDescriptor functionalGroupDescriptor, IDicomAttributeProvider dataSet, int frameNumber, out bool isFrameSpecific)
		{
			Platform.CheckForNullReference(functionalGroupDescriptor, "functionalGroupType");
			Platform.CheckForNullReference(dataSet, "dataSet");
			Platform.CheckPositive(frameNumber, "frameNumber");
			isFrameSpecific = false;

			DicomAttribute sqAttribute;
			if (dataSet.TryGetAttribute(DicomTags.PerFrameFunctionalGroupsSequence, out sqAttribute) && sqAttribute.Count >= frameNumber)
			{
				var sequenceItem = ((DicomAttributeSQ) sqAttribute)[frameNumber - 1];
				var functionalGroup = functionalGroupDescriptor.Create(sequenceItem);
				if (functionalGroup.HasValues())
				{
					isFrameSpecific = true;
					return functionalGroup;
				}
			}

			if (dataSet.TryGetAttribute(DicomTags.SharedFunctionalGroupsSequence, out sqAttribute) && sqAttribute.Count > 0)
			{
				var sequenceItem = ((DicomAttributeSQ) sqAttribute)[0];
				var functionalGroup = functionalGroupDescriptor.Create(sequenceItem);
				functionalGroup.DicomSequenceItem = sequenceItem;
				if (functionalGroup.HasValues())
					return functionalGroup;
			}

			return null;
		}

		public static bool TryGetMultiFrameAttribute(IDicomAttributeProvider dataSet, int frameNumber, uint dicomTag, out DicomAttribute dicomAttribute)
		{
			Platform.CheckForNullReference(dataSet, "dataSet");
			Platform.CheckPositive(frameNumber, "frameNumber");

			DicomAttribute attribute;
			var sopClassUid = dataSet.TryGetAttribute(DicomTags.SopClassUid, out attribute) ? attribute.ToString() : string.Empty;

			var functionalGroupType = FunctionalGroupDescriptor.GetFunctionalGroupByTag(sopClassUid, dicomTag);
			if (functionalGroupType != null)
			{
				var functionalGroup = GetFunctionalGroup(functionalGroupType, dataSet, frameNumber);
				var item = functionalGroup != null ? functionalGroup.SingleItem : null;
				if (item != null) return item.TryGetAttribute(dicomTag, out dicomAttribute);
			}

			dicomAttribute = null;
			return false;
		}

		public static bool TryGetMultiFrameAttribute(IDicomAttributeProvider dataSet, int frameNumber, DicomTag dicomTag, out DicomAttribute dicomAttribute)
		{
			Platform.CheckForNullReference(dataSet, "dataSet");
			Platform.CheckPositive(frameNumber, "frameNumber");
			Platform.CheckForNullReference(dicomTag, "dicomTag");

			DicomAttribute attribute;
			var sopClassUid = dataSet.TryGetAttribute(DicomTags.SopClassUid, out attribute) ? attribute.ToString() : string.Empty;

			var functionalGroupType = FunctionalGroupDescriptor.GetFunctionalGroupByTag(sopClassUid, dicomTag.TagValue);
			if (functionalGroupType != null)
			{
				var functionalGroup = GetFunctionalGroup(functionalGroupType, dataSet, frameNumber);
				var item = functionalGroup != null ? functionalGroup.SingleItem : null;
				if (item != null) return item.TryGetAttribute(dicomTag, out dicomAttribute);
			}

			dicomAttribute = null;
			return false;
		}

		#endregion
	}
}