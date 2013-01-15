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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// C.9.3 Multi-frame Overlay Module, PS 3.3 - 2008
	/// </summary>
	public class MultiframeOverlayModule : IodBase
	{
		#region Constructors
        /// <summary>
		/// Initializes a new instance of the <see cref="MultiframeOverlayModule"/> class.
        /// </summary>
        public MultiframeOverlayModule()
        {
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="MultiframeOverlayModule"/> class.
        /// </summary>
		public MultiframeOverlayModule(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider)
        {
        }
        #endregion

		/// <summary>
		/// Number of Frames in Overlay. Required if Overlay data contains multiple frames.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A Multi-frame Overlay is defined as an Overlay whose overlay data consists of a sequential set of
		/// individual Overlay frames. A Multi-frame Overlay is transmitted as a single contiguous stream of
		/// overlay data. Frame delimiters are not contained within the data stream.
		/// </para>
		/// <para>
		///Each individual frame shall be defined (and thus can be identified) by the Attributes in the Overlay
		///Plane Module (see C.9.2).
		/// </para>
		/// <para>
		///The total number of frames contained within a Multi-frame Overlay is conveyed in the Number of
		///Frames in Overlay (60xx,0015).
		/// </para>
		/// <para>
		///The frames within a Multi-frame Overlay shall be conveyed as a logical sequence. If Multi-frame
		///Overlays are related to a Multi-frame Image, the order of the Overlay Frames are one to one with
		///the order of the Image frames. Otherwise, no attribute is used to indicate the sequencing of the
		///Overlay Frames. If Image Frame Origin (60xx,0051) is present, the Overlay frames are applied
		///one to one to the Image frames, beginning at the indicated frame number. Otherwise, no attribute
		///is used to indicated the sequencing of the Overlay Frames.
		/// </para>
		/// <para>
		///The Number of Frames in Overlay (60xx,0015) plus the Image Frame Origin (60xx,0051) minus 1
		///shall be less than or equal to the total number of frames in the Multi-frame Image.
		/// </para>
		/// <para>
		///If the Overlay data are embedded in the pixel data, then the Image Frame Origin (60xx,0051)
		///must be 1 and the Number of Frames in Overlay (60xx,0015) must equal the number of frames in
		///the Multi-frame Image.
		/// </para>
		/// </remarks>
		public ushort NumberOfFramesInOverlay
		{
			get { return DicomAttributeProvider[DicomTags.NumberOfFramesInOverlay].GetUInt16(0, 0); }
			set { DicomAttributeProvider[DicomTags.NumberOfFramesInOverlay].SetUInt16(0, value); }
		}

		/// <summary>
		/// Frame number of Multi-frame Image to which this overlay applies; frames are numbered from 1.
		/// </summary>
		public ushort ImageFrameOrigin
		{
			get { return DicomAttributeProvider[DicomTags.ImageFrameOrigin].GetUInt16(0, 0); }
			set { DicomAttributeProvider[DicomTags.ImageFrameOrigin].SetUInt16(0, value); }
		}
	}
}
