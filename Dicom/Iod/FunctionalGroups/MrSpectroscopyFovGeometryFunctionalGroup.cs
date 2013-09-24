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
	/// MR Spectroscopy FOV/Geometry Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.14.3.2 (Table C.8-105)</remarks>
	public class MrSpectroscopyFovGeometryFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFovGeometryFunctionalGroup"/> class.
		/// </summary>
		public MrSpectroscopyFovGeometryFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFovGeometryFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrSpectroscopyFovGeometryFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrSpectroscopyFovGeometrySequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.SpectroscopyAcquisitionDataColumns;
				yield return DicomTags.SpectroscopyAcquisitionPhaseRows;
				yield return DicomTags.SpectroscopyAcquisitionPhaseColumns;
				yield return DicomTags.SpectroscopyAcquisitionOutOfPlanePhaseSteps;
				yield return DicomTags.PercentSampling;
				yield return DicomTags.PercentPhaseFieldOfView;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrSpectroscopyFovGeometrySequence in the underlying collection. Type 1.
		/// </summary>
		public MrSpectroscopyFovGeometrySequenceItem MrSpectroscopyFovGeometrySequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpectroscopyFovGeometrySequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrSpectroscopyFovGeometrySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpectroscopyFovGeometrySequence];
				if (value == null)
				{
					const string msg = "MrSpectroscopyFovGeometrySequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrSpectroscopyFovGeometrySequence in the underlying collection. Type 1.
		/// </summary>
		public MrSpectroscopyFovGeometrySequenceItem CreateMrSpectroscopyFovGeometrySequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpectroscopyFovGeometrySequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrSpectroscopyFovGeometrySequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrSpectroscopyFovGeometrySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Spectroscopy FOV/Geometry Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.14.3.2 (Table C.8-105)</remarks>
	public class MrSpectroscopyFovGeometrySequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFovGeometrySequenceItem"/> class.
		/// </summary>
		public MrSpectroscopyFovGeometrySequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFovGeometrySequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrSpectroscopyFovGeometrySequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}