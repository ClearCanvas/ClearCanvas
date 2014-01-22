﻿#region License

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
	/// CT Table Dynamics Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.4 (Table C.8-120)</remarks>
	public class CtTableDynamicsFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtTableDynamicsFunctionalGroup"/> class.
		/// </summary>
		public CtTableDynamicsFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtTableDynamicsFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtTableDynamicsFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtTableDynamicsSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.TableSpeed;
				yield return DicomTags.TableFeedPerRotation;
				yield return DicomTags.SpiralPitchFactor;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtTableDynamicsSequence in the underlying collection. Type 1.
		/// </summary>
		public CtTableDynamicsSequenceItem CtTableDynamicsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtTableDynamicsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtTableDynamicsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtTableDynamicsSequence];
				if (value == null)
				{
					const string msg = "CtTableDynamicsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtTableDynamicsSequence in the underlying collection. Type 1.
		/// </summary>
		public CtTableDynamicsSequenceItem CreateCtTableDynamicsSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtTableDynamicsSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtTableDynamicsSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtTableDynamicsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT Table Dynamics Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.4 (Table C.8-120)</remarks>
	public class CtTableDynamicsSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtTableDynamicsSequenceItem"/> class.
		/// </summary>
		public CtTableDynamicsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtTableDynamicsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtTableDynamicsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}