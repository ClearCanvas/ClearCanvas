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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.FunctionalGroups;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents a cached mapping of tags to functional groups in a multi-frame data set.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class caches a mapping of certain tags used in functional group to the specific functional group
	/// pertaining to that tag and frame. Mapped tags include both the root sequence (SQ) tag used each
	/// the functional group as well as the nested tags in each singleton functional group (i.e. if and only if
	/// the functional group allows has a maximum of one sequence item in the root sequence tag).
	/// </para>
	/// <para>
	/// For example, the tags <see cref="DicomTags.PatientOrientationInFrameSequence"/> and
	/// <see cref="DicomTags.PatientOrientation"/> will map to the <see cref="PatientOrientationInFrameFunctionalGroup"/>
	/// functional group because <see cref="DicomTags.PatientOrientationInFrameSequence"/> is the root
	/// sequence tag used in the functional group, and <see cref="DicomTags.PatientOrientation"/> is a nested
	/// tag used in the sole sequence item allowed by the root sequence attribute. In contrast, the tag
	/// <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> will map to the <see cref="RadiopharmaceuticalUsageFunctionalGroup"/>
	/// functional group because <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> is the root sequence
	/// tag used in the functional group, but <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> will not
	/// be mapped to anything because the root sequence attribute can have multiple items.
	/// </para>
	/// </remarks>
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
		/// <remarks>
		/// It is not possible to directly access a nested tag of a functional group if the root sequence tag allows multiple sequence items
		/// (e.g. <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> as a nested tag of <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/>).
		/// To do so, access the <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> and handle the sequence items individually.
		/// </remarks>
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
		/// <remarks>
		/// It is not possible to directly access a nested tag of a functional group if the root sequence tag allows multiple sequence items
		/// (e.g. <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> as a nested tag of <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/>).
		/// To do so, access the <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> and handle the sequence items individually.
		/// </remarks>
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
		/// Gets the functional group sequence item for a particular tag in the data set (i.e. the parent sequence item containing the requested tag).
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the requested tag is the root sequence tag of a functional group (e.g. <see cref="DicomTags.PatientOrientationInFrameSequence"/>),
		/// the returned sequence item will be the functional group sequence item for the frame in which the requested attribute can be found.
		/// </para>
		/// <para>
		/// If the requested tag is a nested tag of a functional group and the root sequence tag allows only one sequence item
		/// (e.g. <see cref="DicomTags.PatientOrientation"/> as a nested tag of <see cref="DicomTags.PatientOrientationInFrameSequence"/>),
		/// the returned sequence item will be the single sequence item in the root sequence attribute.
		/// </para>
		/// <para>
		/// It is not possible to directly access a nested tag of a functional group if the root sequence tag allows multiple sequence items
		/// (e.g. <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> as a nested tag of <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/>).
		/// To do so, access the <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> and handle the sequence items individually.
		/// </para>
		/// </remarks>
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
				if (functionalGroup == null) return null;

				// if the requested tag is the root sequence tag, return the entire per-frame/shared functional group sequence item as the result
				// otherwise, return the singleton sequence item that contains the requested nested tag (or null if the containing sequence item is not a singleton)
				return functionalGroup.DefinedTags.Contains(tag) ? functionalGroup.DicomSequenceItem : functionalGroup.SingleItem;
			}
			isFrameSpecific = false;
			return null;
		}

		/// <summary>
		/// Gets the functional group sequence item for a particular tag in the data set (i.e. the parent sequence item containing the requested tag).
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the requested tag is the root sequence tag of a functional group (e.g. <see cref="DicomTags.PatientOrientationInFrameSequence"/>),
		/// the returned sequence item will be the functional group sequence item for the frame in which the requested attribute can be found.
		/// </para>
		/// <para>
		/// If the requested tag is a nested tag of a functional group and the root sequence tag allows only one sequence item
		/// (e.g. <see cref="DicomTags.PatientOrientation"/> as a nested tag of <see cref="DicomTags.PatientOrientationInFrameSequence"/>),
		/// the returned sequence item will be the single sequence item in the root sequence attribute.
		/// </para>
		/// <para>
		/// It is not possible to directly access a nested tag of a functional group if the root sequence tag allows multiple sequence items
		/// (e.g. <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> as a nested tag of <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/>).
		/// To do so, access the <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> and handle the sequence items individually.
		/// </para>
		/// </remarks>
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
		/// <remarks>
		/// It is not possible to directly access a nested tag of a functional group if the root sequence tag allows multiple sequence items
		/// (e.g. <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> as a nested tag of <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/>).
		/// To do so, access the <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> and handle the sequence items individually.
		/// </remarks>
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
		/// <remarks>
		/// It is not possible to directly access a nested tag of a functional group if the root sequence tag allows multiple sequence items
		/// (e.g. <see cref="DicomTags.RadiopharmaceuticalAgentNumber"/> as a nested tag of <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/>).
		/// To do so, access the <see cref="DicomTags.RadiopharmaceuticalUsageSequence"/> and handle the sequence items individually.
		/// </remarks>
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