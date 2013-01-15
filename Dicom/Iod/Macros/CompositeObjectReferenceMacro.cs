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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// CompositeObjectReference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.3 (Table C.18.3-1)</remarks>
	public interface ICompositeObjectReferenceMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		ISopInstanceReferenceMacro ReferencedSopSequence { get; set; }

		/// <summary>
		/// Creates the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		ISopInstanceReferenceMacro CreateReferencedSopSequence();
	}

	/// <summary>
	/// CompositeObjectReference Macro Base Implementation
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.3 (Table C.18.3-1)</remarks>
	internal class CompositeObjectReferenceMacro : SequenceIodBase, ICompositeObjectReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeObjectReferenceMacro"/> class.
		/// </summary>
		public CompositeObjectReferenceMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeObjectReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public CompositeObjectReferenceMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.CreateReferencedSopSequence();
			this.ReferencedSopSequence.InitializeAttributes();
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedSopSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSopSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "ReferencedSopSequence is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.ReferencedSopSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		public ISopInstanceReferenceMacro CreateReferencedSopSequence()
		{
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSopSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] {dicomSequenceItem};
				SopInstanceReferenceMacro iodBase = new SopInstanceReferenceMacro(dicomSequenceItem);
				iodBase.InitializeAttributes();
				return iodBase;
			}
			return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}
}