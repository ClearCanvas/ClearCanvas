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
	/// HierarchicalSopInstanceReference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.2.1 (Table C.17-3)</remarks>
	public interface IHierarchicalSopInstanceReferenceMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of StudyInstanceUid in the underlying collection. Type 1.
		/// </summary>
		string StudyInstanceUid { get; set; }

		/// <summary>
		/// Gets or sets the value of ReferencedSeriesSequence in the underlying collection. Type 1.
		/// </summary>
		IHierarchicalSeriesInstanceReferenceMacro[] ReferencedSeriesSequence { get; set; }

		/// <summary>
		/// Creates a single instance of a ReferencedSeriesSequence item. Does not modify the ReferencedSeriesSequence in the underlying collection.
		/// </summary>
		IHierarchicalSeriesInstanceReferenceMacro CreateReferencedSeriesSequence();
	}

	/// <summary>
	/// HierarchicalSopInstanceReference Macro Base Implementation
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.2.1 (Table C.17-3)</remarks>
	internal class HierarchicalSopInstanceReferenceMacro : SequenceIodBase, IHierarchicalSopInstanceReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HierarchicalSopInstanceReferenceMacro"/> class.
		/// </summary>
		public HierarchicalSopInstanceReferenceMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="HierarchicalSopInstanceReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public HierarchicalSopInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.StudyInstanceUid = "1";
		}

		/// <summary>
		/// Gets or sets the value of StudyInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "StudyInstanceUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSeriesSequence in the underlying collection. Type 1.
		/// </summary>
		public IHierarchicalSeriesInstanceReferenceMacro[] ReferencedSeriesSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSeriesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					return null;

				IHierarchicalSeriesInstanceReferenceMacro[] result = new IHierarchicalSeriesInstanceReferenceMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new HierarchicalSeriesInstanceReferenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "ReferencedSeriesSequence is Type 1 Required.");

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.ReferencedSeriesSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ReferencedSeriesSequence item. Does not modify the ReferencedSeriesSequence in the underlying collection.
		/// </summary>
		public IHierarchicalSeriesInstanceReferenceMacro CreateReferencedSeriesSequence()
		{
			IHierarchicalSeriesInstanceReferenceMacro iodBase = new HierarchicalSeriesInstanceReferenceMacro(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}
	}
}