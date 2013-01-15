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
using ClearCanvas.Dicom.Iod.Macros.PresentationStateRelationship;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// PresentationStateRelationship Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.11 (Table C.11.11-1)</remarks>
	public class PresentationStateRelationshipModuleIod : IodBase, IPresentationStateRelationshipMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateRelationshipModuleIod"/> class.
		/// </summary>	
		public PresentationStateRelationshipModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateRelationshipModuleIod"/> class.
		/// </summary>
		public PresentationStateRelationshipModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		DicomSequenceItem IIodMacro.DicomSequenceItem
		{
			get { return base.DicomAttributeProvider as DicomSequenceItem; }
			set { base.DicomAttributeProvider = value; }
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public virtual void InitializeAttributes() { }

		/// <summary>
		/// Gets or sets the value of ReferencedSeriesSequence in the underlying collection. Type 1.
		/// </summary>
		public IReferencedSeriesSequence[] ReferencedSeriesSequence {
			get {
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSeriesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					return null;

				IReferencedSeriesSequence[] result = new IReferencedSeriesSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[])dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PresentationStateRelationshipMacro.ReferencedSeriesSequenceItem(items[n]);

				return result;
			}
			set {
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
		public IReferencedSeriesSequence CreateReferencedSeriesSequence() {
			IReferencedSeriesSequence iodBase = new PresentationStateRelationshipMacro.ReferencedSeriesSequenceItem(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ReferencedSeriesSequence;
			}
		}
	}
}