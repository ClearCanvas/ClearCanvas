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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Multi-Frame Dimension Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.17 (Table C.7.6.17-1)</remarks>
	public class MultiFrameDimensionModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiFrameDimensionModuleIod"/> class.
		/// </summary>	
		public MultiFrameDimensionModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiFrameDimensionModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public MultiFrameDimensionModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.DimensionOrganizationSequence;
				yield return DicomTags.DimensionOrganizationType;
				yield return DicomTags.DimensionIndexSequence;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes() {}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(DimensionOrganizationSequence)
			         && IsNullOrEmpty(DimensionOrganizationType)
			         && IsNullOrEmpty(DimensionIndexSequence));
		}

		/// <summary>
		/// Gets or sets the value of DimensionOrganizationSequence in the underlying collection. Type 1.
		/// </summary>
		public DimensionOrganizationSequenceItem[] DimensionOrganizationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DimensionOrganizationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new DimensionOrganizationSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new DimensionOrganizationSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "DimensionOrganizationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.DimensionOrganizationSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a DimensionOrganizationSequence item. Does not modify the DimensionOrganizationSequence in the underlying collection.
		/// </summary>
		public DimensionOrganizationSequenceItem CreateDimensionOrganizationSequenceItem()
		{
			var iodBase = new DimensionOrganizationSequenceItem(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of DimensionOrganizationType in the underlying collection. Type 3.
		/// </summary>
		public string DimensionOrganizationType
		{
			get { return DicomAttributeProvider[DicomTags.DimensionOrganizationType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DimensionOrganizationType] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DimensionOrganizationType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DimensionIndexSequence in the underlying collection. Type 1.
		/// </summary>
		public DimensionIndexSequenceItem[] DimensionIndexSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DimensionIndexSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new DimensionIndexSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new DimensionIndexSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "DimensionIndexSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.DimensionIndexSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a DimensionIndexSequence item. Does not modify the DimensionIndexSequence in the underlying collection.
		/// </summary>
		public DimensionIndexSequenceItem CreateDimensionIndexSequenceItem()
		{
			var iodBase = new DimensionIndexSequenceItem(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Finds the Dimension Index Sequence Item that references the specified DICOM tag, returning the index of the dimension or -1 if the dimension was not found.
		/// </summary>
		/// <param name="dimensionIndexPointer">The DICOM tag that describes the dimension (i.e. the value of Dimension Index Pointer).</param>
		/// <param name="dimensionIndexPrivateCreator">The private creator code of the <paramref name="dimensionIndexPointer"/>, if it is a private tag.</param>
		/// <returns>The index of the dimension if it was found; -1 otherwise.</returns>
		public int FindDimensionIndexSequenceItemByTag(uint dimensionIndexPointer, string dimensionIndexPrivateCreator = null)
		{
			DimensionIndexSequenceItem sequenceItem;
			return FindDimensionIndexSequenceItemByTag(dimensionIndexPointer, out sequenceItem, dimensionIndexPrivateCreator);
		}

		/// <summary>
		/// Finds the Dimension Index Sequence Item that references the specified DICOM tag, returning the index of the dimension or -1 if the dimension was not found.
		/// </summary>
		/// <param name="dimensionIndexPointer">The DICOM tag that describes the dimension (i.e. the value of Dimension Index Pointer).</param>
		/// <param name="sequenceItem">The <see cref="DimensionIndexSequenceItem"/> of the dimension if it was found; NULL otherwise.</param>
		/// <param name="dimensionIndexPrivateCreator">The private creator code of the <paramref name="dimensionIndexPointer"/>, if it is a private tag.</param>
		/// <returns>The index of the dimension if it was found; -1 otherwise.</returns>
		public int FindDimensionIndexSequenceItemByTag(uint dimensionIndexPointer, out DimensionIndexSequenceItem sequenceItem, string dimensionIndexPrivateCreator = null)
		{
			return FindDimensionIndexSequenceItemByTag(dimensionIndexPointer, 0, out sequenceItem, dimensionIndexPrivateCreator);
		}

		/// <summary>
		/// Finds the Dimension Index Sequence Item that references the specified DICOM tag, returning the index of the dimension or -1 if the dimension was not found.
		/// </summary>
		/// <param name="dimensionIndexPointer">The DICOM tag that describes the dimension (i.e. the value of Dimension Index Pointer).</param>
		/// <param name="functionalGroupPointer">The DICOM tag of the functional group sequence in which the <paramref name="dimensionIndexPointer"/> tag is used.</param>
		/// <param name="dimensionIndexPrivateCreator">The private creator code of the <paramref name="dimensionIndexPointer"/>, if it is a private tag.</param>
		/// <param name="functionalGroupPrivateCreator">The private creator code of the <paramref name="functionalGroupPointer"/>, if it is a private tag.</param>
		/// <returns>The index of the dimension if it was found; -1 otherwise.</returns>
		public int FindDimensionIndexSequenceItemByTag(uint dimensionIndexPointer, uint functionalGroupPointer, string dimensionIndexPrivateCreator = null, string functionalGroupPrivateCreator = null)
		{
			DimensionIndexSequenceItem sequenceItem;
			return FindDimensionIndexSequenceItemByTag(dimensionIndexPointer, functionalGroupPointer, out sequenceItem, dimensionIndexPrivateCreator, functionalGroupPrivateCreator);
		}

		/// <summary>
		/// Finds the Dimension Index Sequence Item that references the specified DICOM tag, returning the index of the dimension or -1 if the dimension was not found.
		/// </summary>
		/// <param name="dimensionIndexPointer">The DICOM tag that describes the dimension (i.e. the value of Dimension Index Pointer).</param>
		/// <param name="functionalGroupPointer">The DICOM tag of the functional group sequence in which the <paramref name="dimensionIndexPointer"/> tag is used.</param>
		/// <param name="sequenceItem">The <see cref="DimensionIndexSequenceItem"/> of the dimension if it was found; NULL otherwise.</param>
		/// <param name="dimensionIndexPrivateCreator">The private creator code of the <paramref name="dimensionIndexPointer"/>, if it is a private tag.</param>
		/// <param name="functionalGroupPrivateCreator">The private creator code of the <paramref name="functionalGroupPointer"/>, if it is a private tag.</param>
		/// <returns>The index of the dimension if it was found; -1 otherwise.</returns>
		public int FindDimensionIndexSequenceItemByTag(uint dimensionIndexPointer, uint functionalGroupPointer, out DimensionIndexSequenceItem sequenceItem, string dimensionIndexPrivateCreator = null, string functionalGroupPrivateCreator = null)
		{
			var indexSequence = DimensionIndexSequence;
			if (!IsNullOrEmpty(indexSequence))
			{
				// find the dimension that references the specified tags
				var index = Array.FindIndex(indexSequence, s => s.DimensionIndexPointer == dimensionIndexPointer
				                                                && (functionalGroupPointer == 0 || s.FunctionalGroupPointer == functionalGroupPointer)
				                                                && (string.IsNullOrEmpty(dimensionIndexPrivateCreator) || s.DimensionIndexPrivateCreator == dimensionIndexPrivateCreator)
				                                                && (string.IsNullOrEmpty(functionalGroupPrivateCreator) || s.FunctionalGroupPrivateCreator == functionalGroupPrivateCreator));
				sequenceItem = index >= 0 ? indexSequence[index] : null;
				return index;
			}
			sequenceItem = null;
			return -1;
		}
	}

	/// <summary>
	/// Dimension Organization Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.17 (Table C.7.6.17-1)</remarks>
	public class DimensionOrganizationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionOrganizationSequenceItem"/> class.
		/// </summary>
		public DimensionOrganizationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionOrganizationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public DimensionOrganizationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of DimensionOrganizationUid in the underlying collection. Type 1.
		/// </summary>
		public string DimensionOrganizationUid
		{
			get { return DicomAttributeProvider[DicomTags.DimensionOrganizationUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "DimensionOrganizationUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.DimensionOrganizationUid].SetString(0, value);
			}
		}
	}

	/// <summary>
	/// Dimension Index Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.17 (Table C.7.6.17-1)</remarks>
	public class DimensionIndexSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionIndexSequenceItem"/> class.
		/// </summary>
		public DimensionIndexSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionIndexSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public DimensionIndexSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of DimensionIndexPointer in the underlying collection. Type 1.
		/// </summary>
		public uint DimensionIndexPointer
		{
			get { return DicomAttributeProvider[DicomTags.DimensionIndexPointer].GetUInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.DimensionIndexPointer].SetUInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of DimensionIndexPrivateCreator in the underlying collection. Type 1C.
		/// </summary>
		public string DimensionIndexPrivateCreator
		{
			get { return DicomAttributeProvider[DicomTags.DimensionIndexPrivateCreator].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DimensionIndexPrivateCreator] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DimensionIndexPrivateCreator].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FunctionalGroupPointer in the underlying collection. Type 1C.
		/// </summary>
		public uint? FunctionalGroupPointer
		{
			get
			{
				uint result;
				if (DicomAttributeProvider[DicomTags.FunctionalGroupPointer].TryGetUInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FunctionalGroupPointer] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FunctionalGroupPointer].SetUInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FunctionalGroupPrivateCreator in the underlying collection. Type 1C.
		/// </summary>
		public string FunctionalGroupPrivateCreator
		{
			get { return DicomAttributeProvider[DicomTags.FunctionalGroupPrivateCreator].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.FunctionalGroupPrivateCreator] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FunctionalGroupPrivateCreator].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DimensionOrganizationUid in the underlying collection. Type 1C.
		/// </summary>
		public string DimensionOrganizationUid
		{
			get { return DicomAttributeProvider[DicomTags.DimensionOrganizationUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DimensionOrganizationUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DimensionOrganizationUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DimensionDescriptionLabel in the underlying collection. Type 3.
		/// </summary>
		public string DimensionDescriptionLabel
		{
			get { return DicomAttributeProvider[DicomTags.DimensionDescriptionLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DimensionDescriptionLabel] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DimensionDescriptionLabel].SetString(0, value);
			}
		}
	}
}