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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// OverlayPlane Module and MultiFrameOverlay Module
	/// </summary>
	/// <seealso cref="OverlayPlaneModuleIod.this"/>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Sections C.9.2 (Table C.9-2) and C.9.3 (Table C.9-3)</remarks>
	public class OverlayPlaneModuleIod : IodBase, IEnumerable<OverlayPlane>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlaneModuleIod"/> class.
		/// </summary>	
		public OverlayPlaneModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlaneModuleIod"/> class.
		/// </summary>
		public OverlayPlaneModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets the Overlays in the underlying collection. The index must be between 0 and 15 inclusive.
		/// </summary>
		/// <remarks>
		/// The implementation of the Overlay Plane module involving repeating groups is a holdover
		/// from previous versions of the DICOM Standard. For each of the 16 allowed overlays, there
		/// exists a separate set of tags bearing the same element numbers but with a group number
		/// of the form 60xx, where xx is an even number from 00 to 1E inclusive. In order to make
		/// these IOD classes easier to use, each of these 16 sets of tags are represented as
		/// separate items of a collection, and may be addressed by an index between 0 and 15
		/// inclusive (mapping to the even groups from 6000 to 601E).
		/// </remarks>
		public OverlayPlane this[int index]
		{
			get
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				return new OverlayPlane(index, this.DicomAttributeProvider);
			}
		}

		/// <summary>
		/// Removes all the tags associated with a particular group from the underlying data source.
		/// </summary>
		/// <param name="index">The index of the group to remove.</param>
		public void Delete(int index)
		{
			Platform.CheckArgumentRange(index, 0, 15, "index");
			foreach (uint tag in this[index].DefinedTags)
				base.DicomAttributeProvider[tag] = null;
		}

		public bool HasOverlayPlane(int index)
		{
			if (index < 0 || index >= 16)
				return false;
			DicomAttribute attrib;
			if (!base.DicomAttributeProvider.TryGetAttribute(ComputeTagOffset(index) + DicomTags.OverlayBitPosition, out attrib))
				return false;
			else if (attrib != null)
				return !attrib.IsEmpty;
			else
				return false;
		}

		// TODO (CR May 2011): Doesn't feel 100% like this method belongs here,
		// but it doesn't quite fit into DicomPixelData either, which is probably where it should go.

		/// <summary>
		/// Extracts all overlays embedded in pixel data and populates the <see cref="DicomTags.OverlayData"/> attribute.
		/// </summary>
		/// <returns>True if any overlays were actually extracted, otherwise false.</returns>
		/// <remarks>Certain restrictions apply to the use of this method:
		/// <list type="bullet">
		/// <item><see cref="IodBase.DicomAttributeProvider"/> must be a <see cref="DicomAttributeCollection"/>.</item>
		/// <item>The <see cref="DicomTags.PixelData"/> attribute must not contain encapsulated pixel data.
		/// Check the <see cref="TransferSyntax"/> of the source message before calling this method.</item>
		/// </list>
		/// </remarks>
		public bool ExtractEmbeddedOverlays()
		{
			var collection = base.DicomAttributeProvider as DicomAttributeCollection;
			if (collection == null)
				throw new ArgumentException("Module must wrap a DicomAttributeCollection in order to extract overlays.");

			DicomAttribute attribute;
			if (!DicomAttributeProvider.TryGetAttribute(DicomTags.PixelData, out attribute))
			{
				Platform.Log(LogLevel.Debug, "Sop does not appear to have any pixel data from which to extract embedded overlays.");
				return false;
			}

			if (attribute is DicomFragmentSequence)
			{
				Platform.Log(LogLevel.Debug, "Sop pixel data must be uncompressed to extract overlays.");
				return false;
			}

			if (attribute.IsNull)
			{
				Platform.Log(LogLevel.Debug, "Sop pixel data has no valid value and cannot have embedded overlays extracted.");
				return false;
			}

			var pixelData = new DicomUncompressedPixelData(collection);
			bool anyExtracted = false;
			// Check if Overlay is embedded in pixels
			foreach (var overlay in this)
			{
				if (overlay.HasOverlayData)
					continue;

				Platform.Log(LogLevel.Debug, "SOP Instance has embedded overlay in pixel data, extracting");
				overlay.ExtractEmbeddedOverlay(pixelData);
				anyExtracted = true;
			}

			return anyExtracted;
		}

		internal static uint ComputeTagOffset(int index)
		{
			return (uint) index*2*0x10000;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				for (int n = 0; n < 16; n++)
				{
					uint tagOffset = ComputeTagOffset(n);
					yield return tagOffset + DicomTags.OverlayBitPosition;
					yield return tagOffset + DicomTags.OverlayBitsAllocated;
					yield return tagOffset + DicomTags.OverlayColumns;
					yield return tagOffset + DicomTags.OverlayData;
					yield return tagOffset + DicomTags.OverlayDescription;
					yield return tagOffset + DicomTags.OverlayLabel;
					yield return tagOffset + DicomTags.OverlayOrigin;
					yield return tagOffset + DicomTags.OverlayRows;
					yield return tagOffset + DicomTags.OverlaySubtype;
					yield return tagOffset + DicomTags.OverlayType;
					yield return tagOffset + DicomTags.RoiArea;
					yield return tagOffset + DicomTags.RoiMean;
					yield return tagOffset + DicomTags.RoiStandardDeviation;
					yield return tagOffset + DicomTags.NumberOfFramesInOverlay;
					yield return tagOffset + DicomTags.ImageFrameOrigin;
				}
			}
		}

		#region IEnumerable<OverlayPlane> Members

		public IEnumerator<OverlayPlane> GetEnumerator()
		{
			for (int n = 0; n < 16; n++)
			{
				if (this.HasOverlayPlane(n))
					yield return this[n];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// Enumerated values for the <see cref="DicomTags.OverlayType"/> attribute indicating whether this overlay represents a region of interest or other graphics.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.9.2 (Table C.9-2)</remarks>
	public enum OverlayType
	{
		/// <summary>
		/// Graphics
		/// </summary>
		G,

		/// <summary>
		/// ROI
		/// </summary>
		R,

		/// <summary>
		/// Represents the null value, which is equivalent to the unknown status.
		/// </summary>
		None
	}

	/// <summary>
	/// Defined terms for the <see cref="DicomTags.OverlaySubtype"/> attribute identifying the intended purpose of the <see cref="OverlayType"/>.
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2008, Part 3, Section C.9.2.1.3</para>
	/// <para>
	/// Additional or alternative Defined Terms may be specified in modality specific Modules,
	/// such as in the Ultrasound Image Module, C.8.5.6.1.11.
	/// </para>
	/// </remarks>
	public class OverlaySubtype
	{
		/// <summary>
		/// User created graphic annotation (e.g. operator).
		/// </summary>
		public static readonly OverlaySubtype User = new OverlaySubtype("USER");

		/// <summary>
		/// Machine or algorithm generated graphic annotation, such as output of a Computer Assisted Diagnosis algorithm.
		/// </summary>
		public static readonly OverlaySubtype Automated = new OverlaySubtype("AUTOMATED");

		/// <summary>
		/// Gets the <see cref="OverlaySubtype"/> matching the given defined term.
		/// </summary>
		/// <param name="definedTerm">The defined term.</param>
		/// <returns>The defined term.</returns>
		public static OverlaySubtype FromDefinedTerm(string definedTerm)
		{
			foreach (OverlaySubtype term in DefinedTerms)
			{
				if (term.DefinedTerm == definedTerm)
					return term;
			}
			return null;
		}

		/// <summary>
		/// Enumerates the defined terms.
		/// </summary>
		public static IEnumerable<OverlaySubtype> DefinedTerms
		{
			get
			{
				yield return User;
				yield return Automated;
			}
		}

		/// <summary>
		/// Gets the defined term this object represents.
		/// </summary>
		public readonly string DefinedTerm;

		/// <summary>
		/// Constructs a new object with the given defined term.
		/// </summary>
		/// <param name="definedTerm">The defined term.</param>
		protected OverlaySubtype(string definedTerm)
		{
			if (string.IsNullOrEmpty(definedTerm))
				throw new ArgumentNullException("definedTerm");
			this.DefinedTerm = definedTerm;
		}

		public override sealed int GetHashCode()
		{
			return this.DefinedTerm.GetHashCode() ^ -0x2D417CC3;
		}

		public override sealed bool Equals(object obj)
		{
			return (obj is OverlaySubtype && ((OverlaySubtype) obj).DefinedTerm.Equals(this.DefinedTerm));
		}

		public override sealed string ToString()
		{
			return this.DefinedTerm;
		}
	}

	/// <summary>
	/// Overlay Plane Group
	/// </summary>
	/// <seealso cref="OverlayPlaneModuleIod.this"/>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Sections C.9.2 (Table C.9-2) and C.9.3 (Table C.9-3)</remarks>
	public class OverlayPlane : IodBase
	{
		private readonly int _index;
		private readonly uint _tagOffset;

		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlane"/> class.
		/// </summary>
		/// <param name="index">The zero-based index of this overlay.</param>
		/// <param name="dicomAttributeProvider">The underlying collection.</param>
		internal OverlayPlane(int index, IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
		{
			_index = index;
			_tagOffset = OverlayPlaneModuleIod.ComputeTagOffset(_index);
		}

		/// <summary>
		/// Gets the zero-based index of the overlay to which this group refers (0-15).
		/// </summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>
		/// Gets the DICOM tag group number.
		/// </summary>
		public ushort Group
		{
			get { return (ushort) (_index*2 + 0x6000); }
		}

		/// <summary>
		/// Gets the DICOM tag value offset from the defined base tags (such as <see cref="DicomTags.OverlayActivationLayer"/>).
		/// </summary>
		public uint TagOffset
		{
			get { return _tagOffset; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the <see cref="OverlayData"/> is stored in 16-bit big-endian words.
		/// </summary>
		public bool IsBigEndianOW
		{
			get { return ByteBuffer.LocalMachineEndian == Endian.Big && base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData] is DicomAttributeOW; }
		}

		/// <summary>
		/// Gets a value indicating if the overlay data for this plane is embedded in the unused bits of the <see cref="DicomTags.PixelData"/>.
		/// </summary>
		/// <remarks>
		/// A previous implementation of this property checked for consistency of the attributes in the Overlay Plane Module
		/// with the attributes in the Image Pixel Module. This has since been deprecated as there are legitimate
		/// use cases where the Image Pixel Module is not necessarily in the same dataset as the Overlay Plane Module.
		/// This property now simply returns the inverse of <see cref="HasOverlayData"/>.
		/// </remarks>
		[Obsolete("This property has been deprecated in favour of checking the inverse of HasOverlayData.")]
		public bool IsEmbedded
		{
			get { return !HasOverlayData; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="DicomTags.OverlayData"/> attribute exists for this plane in the underlying collection.
		/// </summary>
		/// <remarks>
		/// In previous versions of the DICOM Standard, overlay data bits could be stored in unused bits of an image SOP's
		/// <see cref="DicomTags.PixelData"/>, provided the image had 1 sample per pixel and the overlay pixels had a
		/// 1-to-1 relationship with the image pixels. If the overlay plane was stored in such a manner, the <see cref="DicomTags.OverlayData"/>
		/// attribute would not be included at all. This property may be used to determine whether or not the <see cref="DicomTags.OverlayData"/>
		/// exists in the collection in order to support legacy SOP instances.
		/// </remarks>
		public bool HasOverlayData
		{
			get
			{
				DicomAttribute attribute = base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData];
				return !attribute.IsEmpty && !attribute.IsNull;
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayRows in the underlying collection. Type 1.
		/// </summary>
		public int OverlayRows
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayRows].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayRows].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayColumns in the underlying collection. Type 1.
		/// </summary>
		public int OverlayColumns
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayColumns].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayColumns].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayType in the underlying collection. Type 1.
		/// </summary>
		public OverlayType OverlayType
		{
			get { return ParseEnum(base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayType].GetString(0, string.Empty), OverlayType.None); }
			set
			{
				if (value == OverlayType.None)
					throw new ArgumentOutOfRangeException("value", "OverlayType is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayType], value, true);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayOrigin in the underlying collection. Type 1.
		/// </summary>
		public Point? OverlayOrigin
		{
			get
			{
				DicomAttribute attribute = base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayOrigin];
				int[] result = new int[2];
				if (attribute.TryGetInt32(0, out result[0]))
					if (attribute.TryGetInt32(1, out result[1]))
						return new Point(result[0], result[1]);
				return null;
			}
			set
			{
				if (!value.HasValue)
					throw new ArgumentNullException("value", "OverlayOrigin is Type 1 Required.");
				DicomAttribute attribute = base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayOrigin];
				attribute.SetInt32(0, value.Value.X);
				attribute.SetInt32(1, value.Value.Y);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayBitsAllocated in the underlying collection. Type 1.
		/// </summary>
		public int OverlayBitsAllocated
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitsAllocated].GetInt32(0, 0); }
			set
			{
				if (value != 1)
					throw new ArgumentOutOfRangeException("value", "OverlayBitsAllocated must be 1. Encoding overlay data in the unused bits of PixelData is not supported.");
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitsAllocated].SetInt32(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayBitPosition in the underlying collection. Type 1.
		/// </summary>
		public int OverlayBitPosition
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitPosition].GetInt32(0, 0); }
			set
			{
				if (value != 0)
					throw new ArgumentOutOfRangeException("value", "OverlayBitPosition must be 0. Encoding overlay data in the unused bits of PixelData is not supported.");
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitPosition].SetInt32(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayData in the underlying collection. Type 1.
		/// </summary>
		public byte[] OverlayData
		{
			get { return (byte[]) base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "OverlayData is Type 1 Required.");
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayDescription in the underlying collection. Type 3.
		/// </summary>
		public string OverlayDescription
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayDescription] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlaySubtype in the underlying collection. Type 3.
		/// </summary>
		public OverlaySubtype OverlaySubtype
		{
			get { return OverlaySubtype.FromDefinedTerm(base.DicomAttributeProvider[_tagOffset + DicomTags.OverlaySubtype].GetString(0, string.Empty)); }
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.OverlaySubtype] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlaySubtype].SetString(0, value.DefinedTerm);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayLabel in the underlying collection. Type 3.
		/// </summary>
		public string OverlayLabel
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayLabel] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayLabel].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RoiArea in the underlying collection. Type 3.
		/// </summary>
		public int? RoiArea
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.RoiArea].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.RoiArea] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.RoiArea].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RoiMean in the underlying collection. Type 3.
		/// </summary>
		public double? RoiMean
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.RoiMean].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.RoiMean] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.RoiMean].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RoiStandardDeviation in the underlying collection. Type 3.
		/// </summary>
		public double? RoiStandardDeviation
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.RoiStandardDeviation].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.RoiStandardDeviation] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.RoiStandardDeviation].SetFloat64(0, value.Value);
			}
		}

		#region Support for Multi-Frame Overlays

		/// <summary>
		/// Gets or sets the value of NumberOfFramesInOverlay in the underlying collection. Type 1C - Required if the overlay has multiple frames.
		/// </summary>
		public int? NumberOfFramesInOverlay
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.NumberOfFramesInOverlay].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.NumberOfFramesInOverlay] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.NumberOfFramesInOverlay].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ImageFrameOrigin in the underlying collection. Type 3C - Optional if the overlay has multiple frames.
		/// </summary>
		public int? ImageFrameOrigin
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.ImageFrameOrigin].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.ImageFrameOrigin] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.ImageFrameOrigin].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets a value indicating if this overlay plane uses the Multi-Frame Overlay Module (DICOM Standard 2008, Part 3, Section C.9.3 (Table C.9-3))
		/// </summary>
		public bool IsMultiFrame
		{
			get { return NumberOfFramesInOverlay.HasValue; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this multi-frame overlay plane is valid given the total number of frames in the image.
		/// </summary>
		/// <remarks>
		/// DICOM 2009 PS 3.3 Section C.9.3.1.1 states that &quot;The Number of Frames in Overlay (60xx,0015) plus the Image Frame Origin (60xx,0051) minus 1
		/// shall be less than or equal to the total number of frames in the Multi-frame Image.&quot;
		/// </remarks>
		/// <param name="totalImageFrames">The total number of frames in the image.</param>
		/// <returns>True if this multi-frame overlay plane is valid or the overlay plane is not a multi-frame; False otherwise.</returns>
		public bool IsValidMultiFrameOverlay(int totalImageFrames)
		{
			if (totalImageFrames < 1)
				throw new ArgumentOutOfRangeException("totalImageFrames", "Total number of frames in the image must be 1 or greater.");
			return NumberOfFramesInOverlay.GetValueOrDefault(1) + ImageFrameOrigin.GetValueOrDefault(1) - 1 <= totalImageFrames;
		}

		/// <summary>
		/// Enumerates the overlay frames that are applicable to a given image frame, if any.
		/// </summary>
		/// <remarks>
		/// This method has since been deprecated in favour of <see cref="GetRelevantOverlayFrame"/> or
		/// <see cref="TryGetRelevantOverlayFrame"/> as the interpretation of the overlay frame matching attributes
		/// does not allow for more than one overlay frame to be matched to any particular image frame.
		/// For a description of the matching algorithm, please see <see cref="TryGetRelevantOverlayFrame"/>.
		/// </remarks>
		/// <param name="imageFrameNumber">The one-based image frame number.</param>
		/// <param name="totalImageFrames">The total number of frames in the image.</param>
		/// <returns>An enumeration of one-based overlay frame number that are applicable to the given image frame.</returns>
		[Obsolete("Use GetRelevantOverlayFrame or TryGetRelevantOverlayFrame instead.")]
		public IEnumerable<int> GetRelevantOverlayFrames(int imageFrameNumber, int totalImageFrames)
		{
			int overlayFrameNumber;
			if (TryGetRelevantOverlayFrame(imageFrameNumber, totalImageFrames, out overlayFrameNumber))
				yield return overlayFrameNumber;
		}

		/// <summary>
		/// Gets the number of the overlay frame that is applicable to a given image frame, if any.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The DICOM Standard 2008, Part 3, Section C.9.3.1.1 states:
		/// "The frames within a Multi-frame Overlay shall be conveyed as a logical sequence. If Multi-frame
		/// Overlays are related to a Multi-frame Image, the order of the Overlay Frames are one to one with
		/// the order of the Image frames. Otherwise, no attribute is used to indicate the sequencing of the
		/// Overlay Frames. If Image Frame Origin (60xx,0051) is present, the Overlay frames are applied
		/// one to one to the Image frames, beginning at the indicated frame number. Otherwise, no attribute
		/// is used to indicated the sequencing of the Overlay Frames."
		/// </para>
		/// <para>
		/// This is taken to mean that each image frame may have at most ONE overlay frame from any given
		/// overlay plane group. The algorithm used here evaluates a number of conditions and computes an
		/// appropriate overlay frame number, if any. The cases are, in order of precedence:
		/// </para>
		/// <list>
		/// <item>If the overlay is embedded in the pixel data, this function always returns the same frame number
		/// as this was required under the 2004 version of the standard (the last version to allow this usage).</item>
		/// <item>If the overlay is not multi-frame, all image frames will match with the sole overlay frame.</item>
		/// <item>If the image frame number is less than <see cref="ImageFrameOrigin"/> or greater than
		/// <see cref="ImageFrameOrigin"/>+<see cref="NumberOfFramesInOverlay"/>-1, it will have no overlay frame.</item>
		/// <item>Otherwise, this function returns the matching overlay frame number as determined by the
		/// <see cref="ImageFrameOrigin"/>.</item>
		/// </list>
		/// <para>
		/// N.B.: This function does NOT access attributes outside this module (i.e. Number of Frames (0028,0008))
		/// so as to prevent unintended side effects in the dataset. Therefore, this information must be provided
		/// by the calling code via <paramref cref="totalImageFrames"/>.
		/// </para>
		/// </remarks>
		/// <param name="imageFrameNumber">The one-based image frame number.</param>
		/// <param name="totalImageFrames">The total number of frames in the image.</param>
		/// <returns>The one-based overlay frame number if an overlay frame exists for the given image frame; NULL otherwise.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="imageFrameNumber"/> does not specify a valid frame number according to <paramref name="totalImageFrames"/>.</exception>
		public int? GetRelevantOverlayFrame(int imageFrameNumber, int totalImageFrames)
		{
			// if image frame identification is invalid, fail
			if (imageFrameNumber < 1 || imageFrameNumber > totalImageFrames)
				throw new ArgumentOutOfRangeException("imageFrameNumber", "imageFrameNumber must be a positive, non-zero value and less than or equal to totalImageFrames.");

			// if the overlay plane is embedded in the pixel data, the overlay frames are required to be 1-to-1 with each image frame
			if (!HasOverlayData)
				return imageFrameNumber;

			// if the overlay is not a multi-frame, then the only overlay frame is applicable to all image frames
			if (!IsMultiFrame)
				return 1;

			var origin = ImageFrameOrigin.GetValueOrDefault(1);
			var count = NumberOfFramesInOverlay.GetValueOrDefault(1);

			// otherwise the origin and count specify the range of image frames for which a 1-to-1 relation exists to the overlay frames
			if (imageFrameNumber >= origin && imageFrameNumber < origin + count)
				return imageFrameNumber - (origin - 1);

			return null;
		}

		/// <summary>
		/// Gets the number of the overlay frame that is applicable to a given image frame, if any.
		/// </summary>
		/// <remarks>
		/// For a description of the matching algorithm, please see <see cref="TryGetRelevantOverlayFrame"/>.
		/// </remarks>
		/// <param name="imageFrameNumber">The one-based image frame number.</param>
		/// <param name="totalImageFrames">The total number of frames in the image.</param>
		/// <param name="overlayFrameNumber">The one-based overlay frame number. This value is invalid if the function returns false.</param>
		/// <returns>True if there an overlay frame exists for the given image frame; False otherwise.</returns>
		public bool TryGetRelevantOverlayFrame(int imageFrameNumber, int totalImageFrames, out int overlayFrameNumber)
		{
			// if image frame identification is invalid, fail
			if (imageFrameNumber < 1 || imageFrameNumber > totalImageFrames)
			{
				overlayFrameNumber = -1;
				return false;
			}

			var result = GetRelevantOverlayFrame(imageFrameNumber, totalImageFrames);
			overlayFrameNumber = result.GetValueOrDefault(-1);
			return result.HasValue;
		}

		/// <summary>
		/// Computes the offset in the <see cref="OverlayData"/> bit stream from which a specific overlay frame can be read.
		/// </summary>
		/// <param name="overlayFrameNumber">The one-based frame number for which the offset in the <see cref="OverlayData"/> bit stream is to be computed.</param>
		/// <returns>The offset from the beginning of the <see cref="OverlayData"/> in bits.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if this overlay plane does not contain a frame with the specified number.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the overlay plane is embedded in the pixel data.</exception>
		public int ComputeOverlayDataBitOffset(int overlayFrameNumber)
		{
			// if the overlay is embedded in the pixel data, fail
			if (!HasOverlayData)
				throw new InvalidOperationException("This operation is invalid when the overlay plane is embedded in the pixel data.");

			int result;
			if (!TryComputeOverlayDataBitOffset(overlayFrameNumber, out result))
				throw new ArgumentOutOfRangeException("overlayFrameNumber", string.Format("The overlay plane does not contain a frame with the number {0}.", overlayFrameNumber));
			return result;
		}

		/// <summary>
		/// Computes the offset in the <see cref="OverlayData"/> bit stream from which a specific overlay frame can be read.
		/// </summary>
		/// <param name="overlayFrameNumber">The one-based frame number for which the offset in the <see cref="OverlayData"/> bit stream is to be computed.</param>
		/// <param name="bitOffset">The offset from the beginning of the <see cref="OverlayData"/> in bits.</param>
		/// <returns>True if a valid bit offset was computed; False otherwise.</returns>
		public bool TryComputeOverlayDataBitOffset(int overlayFrameNumber, out int bitOffset)
		{
			// if the overlay is embedded in the pixel data, fail
			if (!HasOverlayData)
			{
				bitOffset = -1;
				return false;
			}

			// if the overlay is out of range, fail
			var count = NumberOfFramesInOverlay ?? 1;
			if (overlayFrameNumber < 1 || overlayFrameNumber > count)
			{
				bitOffset = -1;
				return false;
			}

			bitOffset = OverlayRows*OverlayColumns*(overlayFrameNumber - 1);
			return true;
		}

		/// <summary>
		/// Computes the length of each overlay frame in bits.
		/// </summary>
		/// <returns>The length of each overlay frame in bits.</returns>
		public int GetOverlayFrameLength()
		{
			return OverlayRows*OverlayColumns;
		}

		#endregion

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this group.
		/// </summary>
		public IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return _tagOffset + DicomTags.OverlayBitPosition;
				yield return _tagOffset + DicomTags.OverlayBitsAllocated;
				yield return _tagOffset + DicomTags.OverlayColumns;
				yield return _tagOffset + DicomTags.OverlayData;
				yield return _tagOffset + DicomTags.OverlayDescription;
				yield return _tagOffset + DicomTags.OverlayLabel;
				yield return _tagOffset + DicomTags.OverlayOrigin;
				yield return _tagOffset + DicomTags.OverlayRows;
				yield return _tagOffset + DicomTags.OverlaySubtype;
				yield return _tagOffset + DicomTags.OverlayType;
				yield return _tagOffset + DicomTags.RoiArea;
				yield return _tagOffset + DicomTags.RoiMean;
				yield return _tagOffset + DicomTags.RoiStandardDeviation;
				yield return _tagOffset + DicomTags.NumberOfFramesInOverlay;
				yield return _tagOffset + DicomTags.ImageFrameOrigin;
			}
		}

		[Obsolete("Just calls ExtractEmbeddedOverlay.")]
		public bool ConvertEmbeddedOverlay(DicomUncompressedPixelData pd)
		{
			return ExtractEmbeddedOverlay(pd);
		}

		/// <summary>
		/// Fills the <see cref="OverlayData"/> property with the overlay(s) that had been encoded
		/// in the <see cref="DicomTags.PixelData"/> of the SOP Instance.  If the image is a
		/// multi-frame, overlay data is extracted from all the frames.
		/// </summary>
		/// <param name="pd">The pixel data that contains the encoded overlay(s).</param>
		/// <exception cref="DicomException">Thrown if <paramref name="pd"/> is not a valid source of embedded overlay data.</exception>
		/// <returns>True if the <see cref="OverlayData"/> was populated with data encoded in the pixel data; False if <see cref="OverlayData"/> is not empty.</returns>
		public unsafe bool ExtractEmbeddedOverlay(DicomUncompressedPixelData pd)
		{
			byte[] overlayData = this.OverlayData;
			if (overlayData != null && overlayData.Length > 0)
				return false;

			// General sanity checks
			if (pd.SamplesPerPixel > 1)
				throw new DicomException("Unable to convert embedded overlays when Samples Per Pixel > 1");
			if (pd.BitsStored == 8 && pd.BitsAllocated == 8)
				throw new DicomException("Unable to remove overlay with 8 Bits Stored and 8 Bits Allocated");
			if (pd.BitsStored == 16 && pd.BitsAllocated == 16)
				throw new DicomException("Unable to remove overlay with 16 Bits Stored and 16 Bits Allocated");

			if (OverlayBitPosition <= pd.HighBit && OverlayBitPosition >= pd.LowBit)
				throw new DicomException(String.Format("Invalid overlay bit position ({0}); overlay would be in the middle of the pixel data.", OverlayBitPosition));

			int frameSize = pd.UncompressedFrameSize;
			int overlayDataLength = (int)Math.Ceiling((frameSize * pd.NumberOfFrames)/(pd.BitsAllocated*1d));
            int frameLength = frameSize/pd.BytesAllocated;

            // Ensure even length overlay
            if (overlayDataLength % 2 == 1) overlayDataLength++;

			overlayData = new byte[overlayDataLength];
			int overlayOffset = 0;
			byte overlayMask = 0x01;

			if (pd.BitsAllocated <= 8)
			{
				var embeddedOverlayMask = ((byte)(0x1 << OverlayBitPosition));
				// Embededded overlays must exist for all frames, they can't be for a subset
				for (int i = 0; i < pd.NumberOfFrames; i++)
				{
					byte[] frameData = pd.GetFrame(i);
					ExtractEmbeddedOverlay(frameData, frameLength, embeddedOverlayMask, overlayData, ref overlayOffset, ref overlayMask);
					pd.SetFrame(i, frameData);
				}
			}
			else
			{
				var embeddedOverlayMask = ((ushort)(0x1 << OverlayBitPosition));
				// Embededded overlays must exist for all frames, they can't be for a subset
				for (int i = 0; i < pd.NumberOfFrames; i++)
				{
					byte[] frameData = pd.GetFrame(i);
					ExtractEmbeddedOverlay(frameData, frameLength, embeddedOverlayMask, overlayData, ref overlayOffset, ref overlayMask);
					pd.SetFrame(i, frameData);
				}
			}

			pd.UpdatePixelDataAttribute();

			// Assign the new overlay tags
			this.OverlayBitPosition = 0;
			this.OverlayBitsAllocated = 1;
			if (this.IsBigEndianOW)
			{
				// Just do a bulk swap, performance isn't much of an issue.
				ByteBuffer buffer = new ByteBuffer(overlayData, Endian.Little);
				buffer.Swap2();
				this.OverlayData = buffer.ToBytes();
			}
			else
				this.OverlayData = overlayData;

			// Cleanup Rows/Columns if necessary
			if (this.OverlayColumns == 0)
				this.OverlayColumns = pd.ImageWidth;
			if (this.OverlayRows == 0)
				this.OverlayRows = pd.ImageHeight;

			return true;
		}

		private static unsafe void ExtractEmbeddedOverlay(byte[] frameData, int frameLength, byte embeddedOverlayMask, byte[] overlayData, ref int overlayOffset, ref byte overlayMask)
		{
			var pixelMask = (byte)~embeddedOverlayMask;
			fixed (byte* pFrameData = frameData)
			{
				var pixelData = pFrameData;
				for (int p = 0; p < frameLength; p++, pixelData++)
				{
					if ((*pixelData & embeddedOverlayMask) != 0)
					{
						overlayData[overlayOffset] |= overlayMask;
						*pixelData &= pixelMask;
					}

					if (overlayMask == 0x80)
					{
						overlayMask = 0x01;
						overlayOffset++;
					}
					else
					{
						overlayMask <<= 1;
					}
				}
			}
		}

		private static unsafe void ExtractEmbeddedOverlay(byte[] frameData, int frameLength, ushort embeddedOverlayMask, byte[] overlayData, ref int overlayOffset, ref byte overlayMask)
		{
			var pixelMask = (ushort)~embeddedOverlayMask;
			fixed (byte* pFrameData = frameData)
			{
				var pixelData = (ushort*)pFrameData;
				for (int p = 0; p < frameLength; p++, pixelData++)
				{
					if ((*pixelData & embeddedOverlayMask) != 0)
					{
						overlayData[overlayOffset] |= overlayMask;
						*pixelData &= pixelMask;
					}

					if (overlayMask == 0x80)
					{
						overlayMask = 0x01;
						overlayOffset++;
					}
					else
					{
						overlayMask <<= 1;
					}
				}
			}
		}
	}
}