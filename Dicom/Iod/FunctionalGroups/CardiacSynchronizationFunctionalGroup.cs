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

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Cardiac Synchronization Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.7 (Table C.7.6.16-8)</remarks>
	public class CardiacSynchronizationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CardiacSynchronizationFunctionalGroup"/> class.
		/// </summary>
		public CardiacSynchronizationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CardiacSynchronizationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CardiacSynchronizationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CardiacSynchronizationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.NominalPercentageOfCardiacPhase;
				yield return DicomTags.NominalCardiacTriggerDelayTime;
				yield return DicomTags.ActualCardiacTriggerDelayTime;
				yield return DicomTags.NominalCardiacTriggerTimePriorToRPeak;
				yield return DicomTags.ActualCardiacTriggerTimePriorToRPeak;
				yield return DicomTags.IntervalsAcquired;
				yield return DicomTags.IntervalsRejected;
				yield return DicomTags.HeartRate;
				yield return DicomTags.RRIntervalTimeNominal;
				yield return DicomTags.LowRRValue;
				yield return DicomTags.HighRRValue;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CardiacSynchronizationSequence in the underlying collection. Type 1.
		/// </summary>
		public CardiacSynchronizationSequenceItem CardiacSynchronizationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CardiacSynchronizationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CardiacSynchronizationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CardiacSynchronizationSequence];
				if (value == null)
				{
					const string msg = "CardiacSynchronizationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CardiacSynchronizationSequence in the underlying collection. Type 1.
		/// </summary>
		public CardiacSynchronizationSequenceItem CreateCardiacSynchronizationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CardiacSynchronizationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CardiacSynchronizationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CardiacSynchronizationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Cardiac Synchronization Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.7 (Table C.7.6.16-8)</remarks>
	public class CardiacSynchronizationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CardiacSynchronizationSequenceItem"/> class.
		/// </summary>
		public CardiacSynchronizationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CardiacSynchronizationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CardiacSynchronizationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of NominalPercentageOfCardiacPhase in the underlying collection. Type 1C.
		/// </summary>
		public double? NominalPercentageOfCardiacPhase
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.NominalPercentageOfCardiacPhase].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.NominalPercentageOfCardiacPhase] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.NominalPercentageOfCardiacPhase].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of NominalCardiacTriggerDelayTime in the underlying collection. Type 1.
		/// </summary>
		public double NominalCardiacTriggerDelayTime
		{
			get { return DicomAttributeProvider[DicomTags.NominalCardiacTriggerDelayTime].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.NominalCardiacTriggerDelayTime].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ActualCardiacTriggerDelayTime in the underlying collection. Type 1C.
		/// </summary>
		public double? ActualCardiacTriggerDelayTime
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.ActualCardiacTriggerDelayTime].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ActualCardiacTriggerDelayTime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ActualCardiacTriggerDelayTime].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of NominalCardiacTriggerTimePriorToRPeak in the underlying collection. Type 3.
		/// </summary>
		public double? NominalCardiacTriggerTimePriorToRPeak
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.NominalCardiacTriggerTimePriorToRPeak].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.NominalCardiacTriggerTimePriorToRPeak] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.NominalCardiacTriggerTimePriorToRPeak].SetFloat64(0, value.Value);
			}
		}
	}
}