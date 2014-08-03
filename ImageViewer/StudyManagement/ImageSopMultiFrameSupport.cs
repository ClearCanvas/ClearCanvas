﻿#region License

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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	partial class ImageSop
	{
		private readonly FunctionalGroupMapCache _functionalGroups;

		public override sealed DicomAttribute this[DicomTag tag]
		{
			get { return GetFrameAttribute(-1, tag); }
		}

		public override sealed DicomAttribute this[uint tag]
		{
			get { return GetFrameAttribute(-1, tag); }
		}

		public override sealed bool TryGetAttribute(DicomTag tag, out DicomAttribute dicomAttribute)
		{
			return TryGetFrameAttribute(-1, tag, out dicomAttribute);
		}

		public override sealed bool TryGetAttribute(uint tag, out DicomAttribute dicomAttribute)
		{
			return TryGetFrameAttribute(-1, tag, out dicomAttribute);
		}

		/// <summary>
		/// Gets a specific DICOM attribute for the specified frame.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned by this method should be considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="frameNumber">The number of the frame for which the attribute is to be retrieved (1-based index).</param>
		/// <param name="tag">The DICOM tag to retrieve.</param>
		/// <returns>Returns the requested <see cref="DicomAttribute"/>.</returns>
		public virtual DicomAttribute GetFrameAttribute(int frameNumber, DicomTag tag)
		{
			if (frameNumber <= 0) return DataSource[tag];
			return _functionalGroups[frameNumber, tag] ?? DataSource[tag];
		}

		/// <summary>
		/// Gets a specific DICOM attribute for the specified frame.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned by this method should be considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="frameNumber">The number of the frame for which the attribute is to be retrieved (1-based index).</param>
		/// <param name="tag">The DICOM tag to retrieve.</param>
		/// <returns>Returns the requested <see cref="DicomAttribute"/>.</returns>
		public virtual DicomAttribute GetFrameAttribute(int frameNumber, uint tag)
		{
			if (frameNumber <= 0) return DataSource[tag];
			return _functionalGroups[frameNumber, tag] ?? DataSource[tag];
		}

		/// <summary>
		/// Gets a specific DICOM attribute for the specified frame.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned by this method should be considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="frameNumber">The number of the frame for which the attribute is to be retrieved (1-based index).</param>
		/// <param name="tag">The DICOM tag to retrieve.</param>
		/// <param name="dicomAttribute">Returns the requested <see cref="DicomAttribute"/>, or NULL if it was not found.</param>
		/// <returns>Returns TRUE if the requested attribute was found; FALSE otherwise.</returns>
		public virtual bool TryGetFrameAttribute(int frameNumber, DicomTag tag, out DicomAttribute dicomAttribute)
		{
			if (frameNumber <= 0) return DataSource.TryGetAttribute(tag, out dicomAttribute);

			dicomAttribute = _functionalGroups[frameNumber, tag];
			return dicomAttribute != null || DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		/// <summary>
		/// Gets a specific DICOM attribute for the specified frame.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned by this method should be considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="frameNumber">The number of the frame for which the attribute is to be retrieved (1-based index).</param>
		/// <param name="tag">The DICOM tag to retrieve.</param>
		/// <param name="dicomAttribute">Returns the requested <see cref="DicomAttribute"/>, or NULL if it was not found.</param>
		/// <returns>Returns TRUE if the requested attribute was found; FALSE otherwise.</returns>
		public virtual bool TryGetFrameAttribute(int frameNumber, uint tag, out DicomAttribute dicomAttribute)
		{
			if (frameNumber <= 0) return DataSource.TryGetAttribute(tag, out dicomAttribute);

			dicomAttribute = _functionalGroups[frameNumber, tag];
			return dicomAttribute != null || DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		/// <summary>
		/// Gets the Dimension Index Value of a specific dimension for the specified frame.
		/// </summary>
		/// <remarks>
		/// The Dimension Index Value is a number associated with a specific frame in a multi-frame image
		/// that identifies its sequential order relative to the other frames in a particular dimension.
		/// The dimension is identified by the DICOM tag which has the value of the frame in this dimension,
		/// and the DICOM tag of the parent functional group sequence.
		/// </remarks>
		/// <param name="frameNumber">The number of the frame for which the dimension is to be retrieved (1-based index).</param>
		/// <param name="dimensionIndexTag">The DICOM tag of the dimension to retrieve (i.e. the Dimension Index Pointer).</param>
		/// <param name="functionalGroupTag">The DICOM tag of the parent functional group sequence (i.e. the Functional Group Pointer).</param>
		/// <returns>The index value of the frame in the specified dimension, or NULL if the image is not a multi-frame or does not contain the specified dimension.</returns>
		public int? GetFrameDimensionIndexValue(int frameNumber, uint dimensionIndexTag, uint functionalGroupTag)
		{
			if (frameNumber > 0 && IsMultiframe)
			{
				DimensionIndexSequenceItem dimension;
				var dimensionIndex = new MultiFrameDimensionModuleIod(DataSource).FindDimensionIndexSequenceItemByTag(dimensionIndexTag, functionalGroupTag, out dimension);
				if (dimensionIndex >= 0)
				{
					int dimensionIndexValue;
					var dicomAttribute = _functionalGroups[frameNumber, DicomTags.DimensionIndexValues];
					if (dicomAttribute != null && dicomAttribute.TryGetInt32(dimensionIndex, out dimensionIndexValue))
						return dimensionIndexValue;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the Dimension Organization UID of a specific dimension for the specified frame.
		/// </summary>
		/// <remarks>
		/// The Dimension Organization UID identifies when the same dimension index values are valid
		/// across multiple SOP instances.
		/// </remarks>
		/// <param name="frameNumber">The number of the frame for which the dimension is to be retrieved (1-based index).</param>
		/// <param name="dimensionIndexTag">The DICOM tag of the dimension to retrieve (i.e. the Dimension Index Pointer).</param>
		/// <param name="functionalGroupTag">The DICOM tag of the parent functional group sequence (i.e. the Functional Group Pointer).</param>
		/// <returns>The Dimension Organization UID of the specified dimension, or empty if the image is not a multi-frame or does not contain the specified dimension.</returns>
		public string GetFrameDimensionOrganizationUid(int frameNumber, uint dimensionIndexTag, uint functionalGroupTag)
		{
			if (frameNumber > 0 && IsMultiframe)
			{
				DimensionIndexSequenceItem dimension;
				var dimensionIndex = new MultiFrameDimensionModuleIod(DataSource).FindDimensionIndexSequenceItemByTag(dimensionIndexTag, functionalGroupTag, out dimension);
				if (dimensionIndex >= 0) return dimension.DimensionOrganizationUid;
			}
			return string.Empty;
		}

		#region Frame VOI Data LUTs

		private readonly Dictionary<VoiDataLutsCacheKey, IList<VoiDataLut>> _frameVoiDataLuts = new Dictionary<VoiDataLutsCacheKey, IList<VoiDataLut>>();

		/// <summary>
		/// Gets the VOI data LUTs for specified frame.
		/// </summary>
		/// <param name="frameNumber"></param>
		/// <returns></returns>
		internal IList<VoiDataLut> GetFrameVoiDataLuts(int frameNumber)
		{
			Platform.CheckPositive(frameNumber, "frameNumber");

			lock (_syncLock)
			{
				var cacheKey = VoiDataLutsCacheKey.RootKey;
				var dataset = (IDicomAttributeProvider) DataSource;

				// attempt to find the VOI LUT Sequence in a functional group pertaining to the requested frame
				bool isFrameSpecific;
				DicomAttribute dicomAttribute;
				var item = _functionalGroups.GetFrameFunctionalGroupItem(frameNumber, DicomTags.VoiLutSequence, out isFrameSpecific);
				if (item != null && item.TryGetAttribute(DicomTags.VoiLutSequence, out dicomAttribute))
				{
					cacheKey = isFrameSpecific ? VoiDataLutsCacheKey.GetFrameKey(frameNumber) : VoiDataLutsCacheKey.SharedKey;
					dataset = item;
				}

				IList<VoiDataLut> dataLuts;
				if (!_frameVoiDataLuts.TryGetValue(cacheKey, out dataLuts))
					_frameVoiDataLuts.Add(cacheKey, dataLuts = CreateVoiDataLuts(dataset));
				return dataLuts;
			}
		}

		private static IList<VoiDataLut> CreateVoiDataLuts(IDicomAttributeProvider dataset)
		{
			try
			{
				return VoiDataLut.Create(dataset).AsReadOnly();
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Creation of VOI Data LUTs failed.");
				return new List<VoiDataLut>(0).AsReadOnly();
			}
		}

		private class VoiDataLutsCacheKey
		{
			/// <summary>
			/// Gets the key representing the VOI LUT Sequence attribute at the root of the SOP instance data set.
			/// </summary>
			public static readonly VoiDataLutsCacheKey RootKey = new VoiDataLutsCacheKey(-1);

			/// <summary>
			/// Gets the key representing the VOI LUT Sequence attribute in the Shared Functional Groups Sequence.
			/// </summary>
			public static readonly VoiDataLutsCacheKey SharedKey = new VoiDataLutsCacheKey(0);

			/// <summary>
			/// Gets the key representing a VOI LUT Sequence attribute in the Per-Frame Functional Groups Sequence.
			/// </summary>
			public static VoiDataLutsCacheKey GetFrameKey(int frameNumber)
			{
				return new VoiDataLutsCacheKey(frameNumber);
			}

			private readonly int _frameNumber;

			private VoiDataLutsCacheKey(int frameNumber)
			{
				_frameNumber = frameNumber;
			}

			public override int GetHashCode()
			{
				return _frameNumber.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return obj is VoiDataLutsCacheKey && _frameNumber == ((VoiDataLutsCacheKey) obj)._frameNumber;
			}

			public override string ToString()
			{
				return _frameNumber < 0 ? "ROOT" : (_frameNumber == 0 ? "SHARED" : string.Format("FRAME {0}", _frameNumber));
			}
		}

		#endregion
	}
}