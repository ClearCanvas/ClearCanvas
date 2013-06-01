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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Utilities;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// A class for deserializing a key image series into the constituent images and associated presentation states.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyImageDeserializer
	{
		private readonly KeyObjectSelectionDocumentIod _document;

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageDeserializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageDeserializer(Sop sourceSop)
		{
			_document = new KeyObjectSelectionDocumentIod(sourceSop.DataSource);
		}

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageDeserializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageDeserializer(KeyObjectSelectionDocumentIod iod)
		{
			_document = iod;
		}

		/// <summary>
		/// Deserializes the key object selection SOP instance into a list of constituent images and associated presentation states.
		/// </summary>
		public IList<IKeyObjectContentItem> Deserialize()
		{
			List<IKeyObjectContentItem> contentItems = new List<IKeyObjectContentItem>();

			SrDocumentContentModuleIod srDocument = _document.SrDocumentContent;
            if (srDocument.ContentSequence != null)
            {
                foreach (IContentSequence contentItem in srDocument.ContentSequence)
                {
                    if (contentItem.RelationshipType == RelationshipType.Contains)
                    {
                        if (contentItem.ValueType == ValueType.Image)
                        {
                            IImageReferenceMacro imageReference = contentItem;
                            if (imageReference.ReferencedSopSequence == null)
                            {
                                Platform.Log(LogLevel.Warn, "Invalid Key Object Selection document has no Referenced SOP Sequence.");
                                continue;
                            }

                            string referencedSopInstanceUid = imageReference.ReferencedSopSequence.ReferencedSopInstanceUid;
                            string presentationStateSopInstanceUid = null;

                            if (imageReference.ReferencedSopSequence.ReferencedSopSequence != null)
                            {
                                presentationStateSopInstanceUid = imageReference.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid;
                            }

                            string referencedFrameNumbers = imageReference.ReferencedSopSequence.ReferencedFrameNumber;
                            int[] frameNumbers;
                            if (!string.IsNullOrEmpty(referencedFrameNumbers)
                                && DicomStringHelper.TryGetIntArray(referencedFrameNumbers, out frameNumbers) && frameNumbers.Length > 0)
                            {
                                foreach (int frameNumber in frameNumbers)
                                {
                                    KeyImageContentItem item = new KeyImageContentItem(referencedSopInstanceUid, frameNumber, presentationStateSopInstanceUid, _document);
                                    contentItems.Add(item);
                                }
                            }
                            else
                            {
                                KeyImageContentItem item = new KeyImageContentItem(referencedSopInstanceUid, presentationStateSopInstanceUid, _document);
                                contentItems.Add(item);
                            }
                        }
                        else
                        {
                            Platform.Log(LogLevel.Warn, "Unsupported key object selection content item of value type {0}.", contentItem.ValueType);
                            continue;
                        }

                    }
                }
            }
            else
            {
                Platform.Log(LogLevel.Warn, "Invalid Key Object Selection document has no Content Sequence.");
            }

			return contentItems.AsReadOnly();
		}
	}
}
