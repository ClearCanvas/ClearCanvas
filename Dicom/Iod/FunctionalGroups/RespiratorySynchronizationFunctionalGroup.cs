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
	/// Respiratory Synchronization Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.17 (Table C.7.6.16-18)</remarks>
	public class RespiratorySynchronizationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RespiratorySynchronizationFunctionalGroup"/> class.
		/// </summary>
		public RespiratorySynchronizationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RespiratorySynchronizationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public RespiratorySynchronizationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.RespiratorySynchronizationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.RespiratoryIntervalTime;
				yield return DicomTags.NominalPercentageOfRespiratoryPhase;
				yield return DicomTags.NominalRespiratoryTriggerDelayTime;
				yield return DicomTags.ActualRespiratoryTriggerDelayTime;
				yield return DicomTags.StartingRespiratoryAmplitude;
				yield return DicomTags.StartingRespiratoryPhase;
				yield return DicomTags.EndingRespiratoryAmplitude;
				yield return DicomTags.EndingRespiratoryPhase;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of RespiratorySynchronizationSequence in the underlying collection. Type 1.
		/// </summary>
		public RespiratorySynchronizationSequenceItem RespiratorySynchronizationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.RespiratorySynchronizationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new RespiratorySynchronizationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.RespiratorySynchronizationSequence];
				if (value == null)
				{
					const string msg = "RespiratorySynchronizationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the RespiratorySynchronizationSequence in the underlying collection. Type 1.
		/// </summary>
		public RespiratorySynchronizationSequenceItem CreateRespiratorySynchronizationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.RespiratorySynchronizationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new RespiratorySynchronizationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new RespiratorySynchronizationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Respiratory Synchronization Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.17 (Table C.7.6.16-18)</remarks>
	public class RespiratorySynchronizationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RespiratorySynchronizationSequenceItem"/> class.
		/// </summary>
		public RespiratorySynchronizationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RespiratorySynchronizationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public RespiratorySynchronizationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of RespiratoryIntervalTime in the underlying collection. Type 1C.
		/// </summary>
		public double? RespiratoryIntervalTime
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.RespiratoryIntervalTime].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RespiratoryIntervalTime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RespiratoryIntervalTime].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of NominalPercentageOfRespiratoryPhase in the underlying collection. Type 1C.
		/// </summary>
		public double? NominalPercentageOfRespiratoryPhase
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.NominalPercentageOfRespiratoryPhase].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.NominalPercentageOfRespiratoryPhase] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.NominalPercentageOfRespiratoryPhase].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of NominalRespiratoryTriggerDelayTime in the underlying collection. Type 1.
		/// </summary>
		public double NominalRespiratoryTriggerDelayTime
		{
			get { return DicomAttributeProvider[DicomTags.NominalRespiratoryTriggerDelayTime].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.NominalRespiratoryTriggerDelayTime].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ActualRespiratoryTriggerDelayTime in the underlying collection. Type 1C.
		/// </summary>
		public double? ActualRespiratoryTriggerDelayTime
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.ActualRespiratoryTriggerDelayTime].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ActualRespiratoryTriggerDelayTime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ActualRespiratoryTriggerDelayTime].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of StartingRespiratoryAmplitude in the underlying collection. Type 1C.
		/// </summary>
		public double? StartingRespiratoryAmplitude
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.StartingRespiratoryAmplitude].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.StartingRespiratoryAmplitude] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.StartingRespiratoryAmplitude].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of StartingRespiratoryPhase in the underlying collection. Type 1C.
		/// </summary>
		public string StartingRespiratoryPhase
		{
			get { return DicomAttributeProvider[DicomTags.StartingRespiratoryPhase].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.StartingRespiratoryPhase] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.StartingRespiratoryPhase].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of EndingRespiratoryAmplitude in the underlying collection. Type 1C.
		/// </summary>
		public double? EndingRespiratoryAmplitude
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.EndingRespiratoryAmplitude].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.EndingRespiratoryAmplitude] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.EndingRespiratoryAmplitude].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of EndingRespiratoryPhase in the underlying collection. Type 1C.
		/// </summary>
		public string EndingRespiratoryPhase
		{
			get { return DicomAttributeProvider[DicomTags.EndingRespiratoryPhase].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.EndingRespiratoryPhase] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.EndingRespiratoryPhase].SetString(0, value);
			}
		}
	}
}