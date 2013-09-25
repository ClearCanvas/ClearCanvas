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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	partial class ImageSop
	{
		private readonly IDictionary<uint, FunctionalGroupDescriptor> _functionalGroups;

		public override sealed DicomAttribute this[DicomTag tag]
		{
			get { return GetDicomAttribute(-1, tag); }
		}

		public override sealed DicomAttribute this[uint tag]
		{
			get { return GetDicomAttribute(-1, tag); }
		}

		public virtual DicomAttribute GetDicomAttribute(int frameNumber, DicomTag tag)
		{
			return GetMultiFrameDicomAttribute(frameNumber, tag) ?? base[tag];
		}

		public virtual DicomAttribute GetDicomAttribute(int frameNumber, uint tag)
		{
			return GetMultiFrameDicomAttribute(frameNumber, tag) ?? base[tag];
		}

		public virtual bool TryGetDicomAttribute(int frameNumber, DicomTag tag, out DicomAttribute dicomAttribute)
		{
			dicomAttribute = GetMultiFrameDicomAttribute(frameNumber, tag);
			return dicomAttribute != null || DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		public virtual bool TryGetDicomAttribute(int frameNumber, uint tag, out DicomAttribute dicomAttribute)
		{
			dicomAttribute = GetMultiFrameDicomAttribute(frameNumber, tag);
			return dicomAttribute != null || DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		private DicomAttribute GetMultiFrameDicomAttribute(int frameNumber, DicomTag tag)
		{
			FunctionalGroupDescriptor functionalGroupDescriptor;
			if (frameNumber > 0 && _functionalGroups != null && _functionalGroups.TryGetValue(tag.TagValue, out functionalGroupDescriptor))
			{
				DicomAttribute dicomAttribute;
				var functionalGroup = MultiFrameFunctionalGroupsModuleIod.GetFunctionalGroup(functionalGroupDescriptor, DataSource, frameNumber);
				var item = functionalGroup != null ? functionalGroup.SingleItem : null;
				if (item != null && item.TryGetAttribute(tag, out dicomAttribute)) return dicomAttribute;
			}
			return null;
		}

		private DicomAttribute GetMultiFrameDicomAttribute(int frameNumber, uint tag)
		{
			FunctionalGroupDescriptor functionalGroupDescriptor;
			if (frameNumber > 0 && _functionalGroups != null && _functionalGroups.TryGetValue(tag, out functionalGroupDescriptor))
			{
				DicomAttribute dicomAttribute;
				var functionalGroup = MultiFrameFunctionalGroupsModuleIod.GetFunctionalGroup(functionalGroupDescriptor, DataSource, frameNumber);
				var item = functionalGroup != null ? functionalGroup.SingleItem : null;
				if (item != null && item.TryGetAttribute(tag, out dicomAttribute)) return dicomAttribute;
			}
			return null;
		}

		private static IDictionary<uint, FunctionalGroupDescriptor> GetFunctionalGroupMap(ISopDataSource sopDataSource)
		{
			return new MultiFrameFunctionalGroupsModuleIod(sopDataSource).HasValues() ? FunctionalGroupDescriptor.GetFunctionalGroupMap(sopDataSource.SopClassUid) : null;
		}

		#region Frame VOI Data LUTs

		private readonly Dictionary<VoiDataLutsCacheKey, IList<VoiDataLut>> _frameVoiDataLuts = new Dictionary<VoiDataLutsCacheKey, IList<VoiDataLut>>();

		internal IList<VoiDataLut> GetFrameVoiDataLuts(int frameNumber)
		{
			Platform.CheckPositive(frameNumber, "frameNumber");

			lock (_syncLock)
			{
				var cacheKey = VoiDataLutsCacheKey.RootKey;
				var dataset = (IDicomAttributeProvider) DataSource;

				// attempt to find the VOI LUT Sequence in a functional group pertaining to the requested frame
				FunctionalGroupDescriptor functionalGroupDescriptor;
				if (_functionalGroups != null && _functionalGroups.TryGetValue(DicomTags.VoiLutSequence, out functionalGroupDescriptor))
				{
					bool perFrame;
					var functionalGroup = MultiFrameFunctionalGroupsModuleIod.GetFunctionalGroup(functionalGroupDescriptor, DataSource, frameNumber, out perFrame);
					var item = functionalGroup != null ? functionalGroup.SingleItem : null;

					DicomAttribute dicomAttribute;
					if (item != null && item.TryGetAttribute(DicomTags.VoiLutSequence, out dicomAttribute))
					{
						cacheKey = perFrame ? VoiDataLutsCacheKey.GetFrameKey(frameNumber) : VoiDataLutsCacheKey.SharedKey;
						dataset = item;
					}
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