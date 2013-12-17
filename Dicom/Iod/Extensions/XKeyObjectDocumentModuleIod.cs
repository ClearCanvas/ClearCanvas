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

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Extensions
{
	/// <summary>
	/// Key Object Document Module with ClearCanvas private extensions.
	/// </summary>
	/// <remarks>Based on the Key Object Document Module defined in the DICOM Standard 2011, Part 3, Section C.17.6.2 (Table C.17.6-2)</remarks>
	public class XKeyObjectDocumentModuleIod : KeyObjectDocumentModuleIod
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XKeyObjectDocumentModuleIod"/> class.
		/// </summary>	
		public XKeyObjectDocumentModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XKeyObjectDocumentModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public XKeyObjectDocumentModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XKeyObjectDocumentModuleIod"/> class.
		/// </summary>
		/// <param name="keyObjectDocumentModule">A <see cref="KeyObjectDocumentModuleIod"/>.</param>
		public XKeyObjectDocumentModuleIod(KeyObjectDocumentModuleIod keyObjectDocumentModule)
			: base(keyObjectDocumentModule != null ? keyObjectDocumentModule.DicomAttributeProvider : new DicomAttributeCollection()) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public new static IEnumerable<uint> DefinedTags
		{
			get { return KeyObjectDocumentModuleIod.DefinedTags.Concat(new[] {DicomTags.PredecessorDocumentsSequence}); }
		}

		/// <summary>
		/// Gets or sets the value of PredecessorDocumentsSequence in the underlying collection. Type 1C.
		/// </summary>
		/// <remarks>
		/// The helper class <see cref="HierarchicalSopInstanceReferenceDictionary"/> can be used to assist in creating
		/// an identical documents sequence with minimal repetition.
		/// </remarks>
		public IHierarchicalSopInstanceReferenceMacro[] PredecessorDocumentsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PredecessorDocumentsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new IHierarchicalSopInstanceReferenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new HierarchicalSopInstanceReferenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.PredecessorDocumentsSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.PredecessorDocumentsSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PredecessorDocumentsSequence item. Does not modify the PredecessorDocumentsSequence in the underlying collection.
		/// </summary>
		public IHierarchicalSopInstanceReferenceMacro CreatePredecessorDocumentsSequenceItem()
		{
			var iodBase = new HierarchicalSopInstanceReferenceMacro(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}
	}
}