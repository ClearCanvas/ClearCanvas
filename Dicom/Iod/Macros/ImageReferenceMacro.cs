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
using ClearCanvas.Dicom.Iod.Macros.ImageReference;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// ImageReference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.4 (Table C.18.4-1)</remarks>
	public interface IImageReferenceMacro : ICompositeObjectReferenceMacro
	{
		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		new IReferencedSopSequence ReferencedSopSequence { get; set; }

		/// <summary>
		/// Creates the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		new IReferencedSopSequence CreateReferencedSopSequence();
	}

	namespace ImageReference
	{
		/// <summary>
		/// ReferencedSop Sequence of the ImageReference Macro
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.4 (Table C.18.4-1)</remarks>
		public interface IReferencedSopSequence : ISopInstanceReferenceMacro
		{
			/// <summary>
			/// Gets or sets the value of ReferencedFrameNumber in the underlying collection. Type 1C.
			/// </summary>
			string ReferencedFrameNumber { get; set; }

			/// <summary>
			/// Gets or sets the value of ReferencedSegmentNumber in the underlying collection. Type 1C.
			/// </summary>
			string ReferencedSegmentNumber { get; set; }

			/// <summary>
			/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 3.
			/// </summary>
			ISopInstanceReferenceMacro ReferencedSopSequence { get; set; }

			/// <summary>
			/// Creates the value of ReferencedSopSequence in the underlying collection. Type 3.
			/// </summary>
			ISopInstanceReferenceMacro CreateReferencedSopSequence();

			/// <summary>
			/// Gets or sets the value of ReferencedRealWorldValueMappingInstanceSequence in the underlying collection. Type 3.
			/// </summary>
			ISopInstanceReferenceMacro ReferencedRealWorldValueMappingInstanceSequence { get; set; }

			/// <summary>
			/// Creates the value of ReferencedRealWorldValueMappingInstanceSequence in the underlying collection. Type 3.
			/// </summary>
			ISopInstanceReferenceMacro CreateReferencedRealWorldValueMappingInstanceSequence();

			/// <summary>
			/// Gets or sets the value of ImageIconSequence in the underlying collection. Type 3.
			/// </summary>
			object ImageIconSequence { get; set; }
		}
	}

	/// <summary>
	/// ImageReference Macro Base Implementation
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.4 (Table C.18.4-1)</remarks>
	internal class ImageReferenceMacro : SequenceIodBase, IImageReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageReferenceMacro"/> class.
		/// </summary>
		public ImageReferenceMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ImageReferenceMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

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
		ISopInstanceReferenceMacro ICompositeObjectReferenceMacro.ReferencedSopSequence
		{
			get { return this.ReferencedSopSequence; }
			set { this.ReferencedSopSequence = (IReferencedSopSequence) value; }
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		public IReferencedSopSequence ReferencedSopSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSopSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new ReferencedSopSequenceType(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
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
		ISopInstanceReferenceMacro ICompositeObjectReferenceMacro.CreateReferencedSopSequence()
		{
			return this.CreateReferencedSopSequence();
		}

		/// <summary>
		/// Creates the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		public IReferencedSopSequence CreateReferencedSopSequence()
		{
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSopSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] {dicomSequenceItem};
				ReferencedSopSequenceType iodBase = new ReferencedSopSequenceType(dicomSequenceItem);
				iodBase.InitializeAttributes();
				return iodBase;
			}
			return new ReferencedSopSequenceType(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// ReferencedSop Sequence of the ImageReference Macro Base Implementation
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.4 (Table C.18.4-1)</remarks>
		internal class ReferencedSopSequenceType : SopInstanceReferenceMacro, IReferencedSopSequence
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ReferencedSopSequenceType"/> class.
			/// </summary>
			public ReferencedSopSequenceType() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="ReferencedSopSequenceType"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public ReferencedSopSequenceType(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Initializes the underlying collection to implement the module using default values.
			/// </summary>
			public override void InitializeAttributes()
			{
				base.InitializeAttributes();
				this.ReferencedFrameNumber = null;
				this.ReferencedSegmentNumber = null;
				this.ReferencedSopSequence = null;
				this.ReferencedRealWorldValueMappingInstanceSequence = null;
				this.ImageIconSequence = null;
			}

			/// <summary>
			/// Gets or sets the value of ReferencedFrameNumber in the underlying collection. Type 1C.
			/// </summary>
			public string ReferencedFrameNumber
			{
				get { return base.DicomAttributeProvider[DicomTags.ReferencedFrameNumber].ToString(); }
				set
				{
					if (string.IsNullOrEmpty(value))
						base.DicomAttributeProvider[DicomTags.ReferencedFrameNumber] = null;
					base.DicomAttributeProvider[DicomTags.ReferencedFrameNumber].SetStringValue(value);
				}
			}

			/// <summary>
			/// Gets or sets the value of ReferencedSegmentNumber in the underlying collection. Type 1C.
			/// </summary>
			public string ReferencedSegmentNumber
			{
				get { return base.DicomAttributeProvider[DicomTags.ReferencedSegmentNumber].ToString(); }
				set
				{
					if (string.IsNullOrEmpty(value))
						base.DicomAttributeProvider[DicomTags.ReferencedSegmentNumber] = null;
					base.DicomAttributeProvider[DicomTags.ReferencedSegmentNumber].SetStringValue(value);
				}
			}

			/// <summary>
			/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 3.
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
					{
						base.DicomAttributeProvider[DicomTags.ReferencedSopSequence] = null;
						return;
					}
					base.DicomAttributeProvider[DicomTags.ReferencedSopSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
				}
			}

			/// <summary>
			/// Creates the value of ReferencedSopSequence in the underlying collection. Type 3.
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

			/// <summary>
			/// Gets or sets the value of ReferencedRealWorldValueMappingInstanceSequence in the underlying collection. Type 3.
			/// </summary>
			public ISopInstanceReferenceMacro ReferencedRealWorldValueMappingInstanceSequence
			{
				get
				{
					DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedRealWorldValueMappingInstanceSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}
					return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
				}
				set
				{
					if (value == null)
					{
						base.DicomAttributeProvider[DicomTags.ReferencedRealWorldValueMappingInstanceSequence] = null;
						return;
					}
					base.DicomAttributeProvider[DicomTags.ReferencedRealWorldValueMappingInstanceSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
				}
			}

			/// <summary>
			/// Creates the value of ReferencedRealWorldValueMappingInstanceSequence in the underlying collection. Type 3.
			/// </summary>
			public ISopInstanceReferenceMacro CreateReferencedRealWorldValueMappingInstanceSequence()
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedRealWorldValueMappingInstanceSequence];
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

			/// <summary>
			/// NOT IMPLEMENTED. Gets or sets the value of ImageIconSequence in the underlying collection. Type 3.
			/// </summary>
			public object ImageIconSequence
			{
				// TODO - Implement this.
				get { throw new NotImplementedException(); }
				set { 
					//throw new NotImplementedException(); 
				}
			}
		}
	}
}