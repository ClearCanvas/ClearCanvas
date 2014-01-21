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
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Frame Display Shutter Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.16 (Table C.7.6.16-17)</remarks>
	public class FrameDisplayShutterFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDisplayShutterFunctionalGroup"/> class.
		/// </summary>
		public FrameDisplayShutterFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDisplayShutterFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameDisplayShutterFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FrameDisplayShutterSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { return DisplayShutterMacroIod.DefinedTags; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FrameDisplayShutterSequence in the underlying collection. Type 1.
		/// </summary>
		public FrameDisplayShutterSequenceItem FrameDisplayShutterSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameDisplayShutterSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new FrameDisplayShutterSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameDisplayShutterSequence];
				if (value == null)
				{
					const string msg = "FrameDisplayShutterSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FrameDisplayShutterSequence in the underlying collection. Type 1.
		/// </summary>
		public FrameDisplayShutterSequenceItem CreateFrameDisplayShutterSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FrameDisplayShutterSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FrameDisplayShutterSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FrameDisplayShutterSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Frame Display Shutter Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.16 (Table C.7.6.16-17)</remarks>
	public class FrameDisplayShutterSequenceItem : DisplayShutterMacroIod
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDisplayShutterSequenceItem"/> class.
		/// </summary>
		public FrameDisplayShutterSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameDisplayShutterSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameDisplayShutterSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets the dicom attribute collection as a dicom sequence item.
		/// </summary>
		/// <value>The dicom sequence item.</value>
		public DicomSequenceItem DicomSequenceItem
		{
			get { return DicomAttributeProvider as DicomSequenceItem; }
			set { DicomAttributeProvider = value; }
		}
	}
}