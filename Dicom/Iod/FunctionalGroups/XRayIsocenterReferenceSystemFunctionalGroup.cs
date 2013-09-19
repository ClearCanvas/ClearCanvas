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
	/// X-Ray Isocenter Reference System Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.13 (Table C.8.19.6-13)</remarks>
	public class XRayIsocenterReferenceSystemFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayIsocenterReferenceSystemFunctionalGroup"/> class.
		/// </summary>
		public XRayIsocenterReferenceSystemFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayIsocenterReferenceSystemFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayIsocenterReferenceSystemFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.IsocenterReferenceSystemSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.PositionerIsocenterPrimaryAngle;
				yield return DicomTags.PositionerIsocenterSecondaryAngle;
				yield return DicomTags.PositionerIsocenterDetectorRotationAngle;
				yield return DicomTags.TableXPositionToIsocenter;
				yield return DicomTags.TableYPositionToIsocenter;
				yield return DicomTags.TableZPositionToIsocenter;
				yield return DicomTags.TableHorizontalRotationAngle;
				yield return DicomTags.TableHeadTiltAngle;
				yield return DicomTags.TableCradleTiltAngle;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of IsocenterReferenceSystemSequence in the underlying collection. Type 1.
		/// </summary>
		public IsocenterReferenceSystemSequenceItem IsocenterReferenceSystemSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IsocenterReferenceSystemSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new IsocenterReferenceSystemSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IsocenterReferenceSystemSequence];
				if (value == null)
				{
					const string msg = "IsocenterReferenceSystemSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the IsocenterReferenceSystemSequence in the underlying collection. Type 1.
		/// </summary>
		public IsocenterReferenceSystemSequenceItem CreateIsocenterReferenceSystemSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.IsocenterReferenceSystemSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new IsocenterReferenceSystemSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new IsocenterReferenceSystemSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Isocenter Reference System Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.13 (Table C.8.19.6-13)</remarks>
	public class IsocenterReferenceSystemSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IsocenterReferenceSystemSequenceItem"/> class.
		/// </summary>
		public IsocenterReferenceSystemSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IsocenterReferenceSystemSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IsocenterReferenceSystemSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}