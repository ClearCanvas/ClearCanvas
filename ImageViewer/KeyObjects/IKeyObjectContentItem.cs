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

using ClearCanvas.Dicom.Iod.Iods;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// A single key object content item in a key object document.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public interface IKeyObjectContentItem
	{
		ValueType ValueType { get; }
		KeyObjectSelectionDocumentIod Source { get; }
	}

	/// <summary>
	/// A single key image content item in a key object document.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyImageContentItem : IKeyObjectContentItem
	{
		public readonly string ReferencedImageSopInstanceUid;
		public readonly string PresentationStateSopInstanceUid;

		public readonly int? FrameNumber;

		private readonly KeyObjectSelectionDocumentIod _source;

		public KeyImageContentItem(string imageSopInstanceUid, string presentationSopInstanceUid, KeyObjectSelectionDocumentIod source)
		{
			this.ReferencedImageSopInstanceUid = imageSopInstanceUid;
			this.PresentationStateSopInstanceUid = presentationSopInstanceUid;
			this.FrameNumber = null;
			this._source = source;
		}

		public KeyImageContentItem(string imageSopInstanceUid, int frameNumber, string presentationSopInstanceUid, KeyObjectSelectionDocumentIod source)
			: this(imageSopInstanceUid, presentationSopInstanceUid, source)
		{
			this.FrameNumber = frameNumber;
		}

		ValueType IKeyObjectContentItem.ValueType
		{
			get { return ValueType.Image; }
		}

		public KeyObjectSelectionDocumentIod Source
		{
			get { return _source; }
		}
	}
}