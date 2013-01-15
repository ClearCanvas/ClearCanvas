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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// Container Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.8 (Table C.18.8-1)</remarks>
	public interface IContainerMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of ContinuityOfContent in the underlying collection. Type 1.
		/// </summary>
		ContinuityOfContent ContinuityOfContent { get; set; }

		/// <summary>
		/// Gets or sets the value of ContentTemplateSequence in the underlying collection. Type 1C.
		/// </summary>
		ContentTemplateSequence ContentTemplateSequence { get; set; }

		/// <summary>
		/// Creates the value of ContentTemplateSequence in the underlying collection. Type 1C.
		/// </summary>
		ContentTemplateSequence CreateContentTemplateSequence();
	}

	/// <summary>
	/// Container Macro Base Implementation
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.8 (Table C.18.8-1)</remarks>
	internal class ContainerMacro : SequenceIodBase, IContainerMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerMacro"/> class.
		/// </summary>
		public ContainerMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ContainerMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.ContinuityOfContent = ContinuityOfContent.Separate;
			this.ContentTemplateSequence = null;
		}

		/// <summary>
		/// Gets or sets the value of ContinuityOfContent in the underlying collection. Type 1.
		/// </summary>
		public ContinuityOfContent ContinuityOfContent
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.ContinuityOfContent].GetString(0, string.Empty), ContinuityOfContent.Unknown); }
			set
			{
				if (value == ContinuityOfContent.Unknown)
					throw new ArgumentNullException("value", "Continuity of Content is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ContinuityOfContent], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentTemplateSequence in the underlying collection. Type 1C.
		/// </summary>
		public ContentTemplateSequence ContentTemplateSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentTemplateSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new ContentTemplateSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ContentTemplateSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ContentTemplateSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the value of ContentTemplateSequence in the underlying collection. Type 1C.
		/// </summary>
		public ContentTemplateSequence CreateContentTemplateSequence()
		{
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentTemplateSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] {dicomSequenceItem};
				ContentTemplateSequence iodBase = new ContentTemplateSequence(dicomSequenceItem);
				iodBase.InitializeAttributes();
				return iodBase;
			}
			return new ContentTemplateSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}
}