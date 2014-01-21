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
using ClearCanvas.Dicom.Iod.Macros.HierarchicalSeriesInstanceReference;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// Hierarchical Series Instance Reference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.17.2.1 (Table C.17-3a)</remarks>
	public interface IHierarchicalSeriesInstanceReferenceMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		string SeriesInstanceUid { get; set; }

		/// <summary>
		/// Gets or sets the value of RetrieveAeTitle in the underlying collection. Type 3.
		/// </summary>
		string RetrieveAeTitle { get; set; }

		/// <summary>
		/// Gets or sets the value of RetrieveLocationUid in the underlying collection. Type 3.
		/// </summary>
		string RetrieveLocationUid { get; set; }

		/// <summary>
		/// Gets or sets the value of StorageMediaFileSetId in the underlying collection. Type 3.
		/// </summary>
		string StorageMediaFileSetId { get; set; }

		/// <summary>
		/// Gets or sets the value of StorageMediaFileSetUid in the underlying collection. Type 3.
		/// </summary>
		string StorageMediaFileSetUid { get; set; }

		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		IReferencedSopSequence[] ReferencedSopSequence { get; set; }

		/// <summary>
		/// Creates a single instance of a ReferencedSopSequence item. Does not modify the ReferencedSopSequence in the underlying collection.
		/// </summary>
		IReferencedSopSequence CreateReferencedSopSequence();
	}

	namespace HierarchicalSeriesInstanceReference
	{
		/// <summary>
		/// SOP Instance Reference Macro as used in the <see cref="IHierarchicalSeriesInstanceReferenceMacro"/>
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section 10.8 (Table 10-11)</remarks>
		public interface IReferencedSopSequence : ISopInstanceReferenceMacro
		{
			/// <summary>
			/// Gets or sets the value of PurposeOfReferenceCodeSequence in the underlying collection. Type 3.
			/// </summary>
			CodeSequenceMacro[] PurposeOfReferenceCodeSequence { get; set; }

			/// <summary>
			/// Gets or sets the value of ReferencedDigitalSignatureSequence in the underlying collection. Type 3.
			/// </summary>
			ReferencedDigitalSignatureSequence[] ReferencedDigitalSignatureSequence { get; set; }

			/// <summary>
			/// Gets or sets the value of ReferencedSopInstanceMacSequence in the underlying collection. Type 3.
			/// </summary>
			ReferencedSopInstanceMacSequence ReferencedSopInstanceMacSequence { get; set; }
		}
	}

	/// <summary>
	/// Hierarchical Series Instance Reference Macro Base Implementation
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.2.1 (Table C.17-3a)</remarks>
	internal class HierarchicalSeriesInstanceReferenceMacro : SequenceIodBase, IHierarchicalSeriesInstanceReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HierarchicalSeriesInstanceReferenceMacro"/> class.
		/// </summary>
		public HierarchicalSeriesInstanceReferenceMacro() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="HierarchicalSeriesInstanceReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public HierarchicalSeriesInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			SeriesInstanceUid = "1";
			RetrieveAeTitle = null;
			RetrieveLocationUid = null;
			StorageMediaFileSetId = null;
			StorageMediaFileSetUid = null;
		}

		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "SeriesInstanceUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RetrieveAeTitle in the underlying collection. Type 3.
		/// </summary>
		public string RetrieveAeTitle
		{
			get { return DicomAttributeProvider[DicomTags.RetrieveAeTitle].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.RetrieveAeTitle] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RetrieveAeTitle].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RetrieveLocationUid in the underlying collection. Type 3.
		/// </summary>
		public string RetrieveLocationUid
		{
			get { return DicomAttributeProvider[DicomTags.RetrieveLocationUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.RetrieveLocationUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RetrieveLocationUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of StorageMediaFileSetId in the underlying collection. Type 3.
		/// </summary>
		public string StorageMediaFileSetId
		{
			get { return DicomAttributeProvider[DicomTags.StorageMediaFileSetId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.StorageMediaFileSetId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.StorageMediaFileSetId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of StorageMediaFileSetUid in the underlying collection. Type 3.
		/// </summary>
		public string StorageMediaFileSetUid
		{
			get { return DicomAttributeProvider[DicomTags.StorageMediaFileSetUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.StorageMediaFileSetUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.StorageMediaFileSetUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		public IReferencedSopSequence[] ReferencedSopSequence
		{
			get
			{
				DicomAttribute dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedSopSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					return null;

				IReferencedSopSequence[] result = new IReferencedSopSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ReferencedSopSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "ReferencedSopSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ReferencedSopSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ReferencedSopSequence item. Does not modify the ReferencedSopSequence in the underlying collection.
		/// </summary>
		public IReferencedSopSequence CreateReferencedSopSequence()
		{
			IReferencedSopSequence iodBase = new ReferencedSopSequenceItem(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Referenced SOP Sequence Base Implementation
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.17.2.1 (Table C.17-3a)</remarks>
		internal class ReferencedSopSequenceItem : SopInstanceReferenceMacro, IReferencedSopSequence
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ReferencedSopSequence"/> class.
			/// </summary>
			public ReferencedSopSequenceItem() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="ReferencedSopSequence"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public ReferencedSopSequenceItem(DicomSequenceItem dicomSequenceItem)
				: base(dicomSequenceItem) {}

			/// <summary>
			/// Initializes the underlying collection to implement the module using default values.
			/// </summary>
			public override void InitializeAttributes()
			{
				base.InitializeAttributes();
				PurposeOfReferenceCodeSequence = null;
				ReferencedDigitalSignatureSequence = null;
				ReferencedSopInstanceMacSequence = null;
			}

			/// <summary>
			/// Gets or sets the value of PurposeOfReferenceCodeSequence in the underlying collection. Type 3.
			/// </summary>
			public CodeSequenceMacro[] PurposeOfReferenceCodeSequence
			{
				get
				{
					DicomAttribute dicomAttribute = DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					CodeSequenceMacro[] result = new CodeSequenceMacro[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new CodeSequenceMacro(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence].Values = result;
				}
			}

			/// <summary>
			/// Gets or sets the value of ReferencedDigitalSignatureSequence in the underlying collection. Type 3.
			/// </summary>
			public ReferencedDigitalSignatureSequence[] ReferencedDigitalSignatureSequence
			{
				get
				{
					DicomAttribute dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedDigitalSignatureSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					ReferencedDigitalSignatureSequence[] result = new ReferencedDigitalSignatureSequence[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new ReferencedDigitalSignatureSequence(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						DicomAttributeProvider[DicomTags.ReferencedDigitalSignatureSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					DicomAttributeProvider[DicomTags.ReferencedDigitalSignatureSequence].Values = result;
				}
			}

			/// <summary>
			/// Gets or sets the value of ReferencedSopInstanceMacSequence in the underlying collection. Type 3.
			/// </summary>
			public ReferencedSopInstanceMacSequence ReferencedSopInstanceMacSequence
			{
				get
				{
					DicomAttribute dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedSopInstanceMacSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}
					return new ReferencedSopInstanceMacSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
				}
				set
				{
					DicomAttribute dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedSopInstanceMacSequence];
					if (value == null)
					{
						DicomAttributeProvider[DicomTags.ReferencedSopInstanceMacSequence] = null;
						return;
					}
					dicomAttribute.Values = new[] {value.DicomSequenceItem};
				}
			}
		}
	}
}