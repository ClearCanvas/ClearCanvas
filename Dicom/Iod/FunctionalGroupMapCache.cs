#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents a cached mapping of tags to functional groups in a multi-frame data set.
	/// </summary>
	public sealed class FunctionalGroupMapCache
	{
		private readonly IDicomAttributeProvider _dataSet;
		private readonly IDictionary<uint, FunctionalGroupDescriptor> _tagMap;
		private readonly IDictionary<FrameFunctionalGroupKey, FrameFunctionalGroupValue> _cache;

		/// <summary>
		/// Initializes a new instance of <see cref="FunctionalGroupMapCache"/>.
		/// </summary>
		/// <param name="dataSet">The data set.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/> is NULL.</exception>
		public FunctionalGroupMapCache(IDicomAttributeProvider dataSet)
			: this(dataSet, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="FunctionalGroupMapCache"/>.
		/// </summary>
		/// <param name="dataSet">The data set.</param>
		/// <param name="sopClassUid">Overrides the detected SOP class UID of the data set.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/> is NULL.</exception>
		public FunctionalGroupMapCache(IDicomAttributeProvider dataSet, string sopClassUid)
		{
			Platform.CheckForNullReference(dataSet, "dataSet");

			if (string.IsNullOrEmpty(sopClassUid))
				sopClassUid = dataSet[DicomTags.SopClassUid].ToString();

			_dataSet = dataSet;
			_cache = new Dictionary<FrameFunctionalGroupKey, FrameFunctionalGroupValue>();
			_tagMap = new MultiFrameFunctionalGroupsModuleIod(dataSet).HasValues() ? FunctionalGroupDescriptor.GetFunctionalGroupMap(sopClassUid) : null;
		}

		/// <summary>
		/// Gets the data set.
		/// </summary>
		public IDicomAttributeProvider DataSet
		{
			get { return _dataSet; }
		}

		/// <summary>
		/// Clears the functional group resolver cache.
		/// </summary>
		public void Clear()
		{
			_cache.Clear();
		}

		/// <summary>
		/// Gets a nested frame attribute from a functional group in the data set.
		/// </summary>
		/// <param name="frameNumber">DICOM frame number (first frame is 1).</param>
		/// <param name="tag">DICOM tag of the frame attribute.</param>
		public DicomAttribute this[int frameNumber, DicomTag tag]
		{
			get
			{
				bool isFrameSpecific;
				return GetFrameAttribute(frameNumber, tag, out isFrameSpecific);
			}
		}

		/// <summary>
		/// Gets a nested frame attribute from a functional group in the data set.
		/// </summary>
		/// <param name="frameNumber">DICOM frame number (first frame is 1).</param>
		/// <param name="tag">DICOM tag of the frame attribute.</param>
		public DicomAttribute this[int frameNumber, uint tag]
		{
			get
			{
				bool isFrameSpecific;
				return GetFrameAttribute(frameNumber, tag, out isFrameSpecific);
			}
		}

		/// <summary>
		/// Gets the functional group sequence item for a particular tag in the data set.
		/// </summary>
		/// <param name="frameNumber">DICOM frame number (first frame is 1).</param>
		/// <param name="tag">DICOM tag of the attribute.</param>
		/// <param name="isFrameSpecific">Indicates whether or not the returned functional group is specific to the requested frame.</param>
		/// <returns>The requested functional group sequence item.</returns>
		public DicomSequenceItem GetFrameFunctionalGroupItem(int frameNumber, uint tag, out bool isFrameSpecific)
		{
			FunctionalGroupDescriptor functionalGroupDescriptor;
			if (frameNumber > 0 && _tagMap != null && _tagMap.TryGetValue(tag, out functionalGroupDescriptor))
			{
				FrameFunctionalGroupValue functionalGroupResult;
				var key = new FrameFunctionalGroupKey(functionalGroupDescriptor, frameNumber);
				if (!_cache.TryGetValue(key, out functionalGroupResult))
					_cache[key] = functionalGroupResult = new FrameFunctionalGroupValue(MultiFrameFunctionalGroupsModuleIod.GetFunctionalGroup(functionalGroupDescriptor, _dataSet, frameNumber, out isFrameSpecific), isFrameSpecific);
				else
					isFrameSpecific = functionalGroupResult.IsFrameSpecific;

				var functionalGroup = functionalGroupResult.FunctionalGroupMacro;
				return functionalGroup != null ? functionalGroup.SingleItem : null;
			}
			isFrameSpecific = false;
			return null;
		}

		/// <summary>
		/// Gets the functional group sequence item for a particular tag in the data set.
		/// </summary>
		/// <param name="frameNumber">DICOM frame number (first frame is 1).</param>
		/// <param name="tag">DICOM tag of the attribute.</param>
		/// <param name="isFrameSpecific">Indicates whether or not the returned functional group is specific to the requested frame.</param>
		/// <returns>The requested functional group sequence item.</returns>
		public DicomSequenceItem GetFrameFunctionalGroupItem(int frameNumber, DicomTag tag, out bool isFrameSpecific)
		{
			return GetFrameFunctionalGroupItem(frameNumber, tag.TagValue, out isFrameSpecific);
		}

		/// <summary>
		/// Gets a nested frame attribute from a functional group in the data set.
		/// </summary>
		/// <param name="frameNumber">DICOM frame number (first frame is 1).</param>
		/// <param name="tag">DICOM tag of the frame attribute.</param>
		/// <param name="isFrameSpecific">Indicates whether or not the returned frame attribute is specific to the requested frame.</param>
		/// <returns>The requested frame attribute.</returns>
		public DicomAttribute GetFrameAttribute(int frameNumber, uint tag, out bool isFrameSpecific)
		{
			DicomAttribute dicomAttribute;
			var item = GetFrameFunctionalGroupItem(frameNumber, tag, out isFrameSpecific);
			if (item != null && item.TryGetAttribute(tag, out dicomAttribute)) return dicomAttribute;
			return null;
		}

		/// <summary>
		/// Gets a nested frame attribute from a functional group in the data set.
		/// </summary>
		/// <param name="frameNumber">DICOM frame number (first frame is 1).</param>
		/// <param name="tag">DICOM tag of the frame attribute.</param>
		/// <param name="isFrameSpecific">Indicates whether or not the returned frame attribute is specific to the requested frame.</param>
		/// <returns>The requested frame attribute.</returns>
		public DicomAttribute GetFrameAttribute(int frameNumber, DicomTag tag, out bool isFrameSpecific)
		{
			DicomAttribute dicomAttribute;
			var item = GetFrameFunctionalGroupItem(frameNumber, tag.TagValue, out isFrameSpecific);
			if (item != null && item.TryGetAttribute(tag, out dicomAttribute)) return dicomAttribute;
			return null;
		}

		private class FrameFunctionalGroupValue
		{
			public readonly FunctionalGroupMacro FunctionalGroupMacro;
			public readonly bool IsFrameSpecific;

			public FrameFunctionalGroupValue(FunctionalGroupMacro functionalGroupMacro, bool isFrameSpecific)
			{
				FunctionalGroupMacro = functionalGroupMacro;
				IsFrameSpecific = isFrameSpecific;
			}
		}

		private class FrameFunctionalGroupKey
		{
			private readonly FunctionalGroupDescriptor _functionalGroupDescriptor;
			private readonly int _frameNumber;

			public FrameFunctionalGroupKey(FunctionalGroupDescriptor functionalGroupDescriptor, int frameNumber)
			{
				_functionalGroupDescriptor = functionalGroupDescriptor;
				_frameNumber = frameNumber;
			}

			public override int GetHashCode()
			{
				return _functionalGroupDescriptor.GetHashCode() ^ _frameNumber.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				var other = obj as FrameFunctionalGroupKey;
				return other != null && (_frameNumber == other._frameNumber && _functionalGroupDescriptor.Equals(other._functionalGroupDescriptor));
			}

			public override string ToString()
			{
				return string.Format("Frame #{0} - {1}", _frameNumber, _functionalGroupDescriptor.Name);
			}
		}
	}
}