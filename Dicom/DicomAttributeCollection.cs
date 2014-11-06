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
using System.Linq;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// The DicomAttributeCollection class models an a collection of DICOM attributes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class represents a collection of <see cref="DicomAttribute"/> classes.  It is used by the <see cref="DicomMessageBase"/> class to 
	/// represent the meta info and data set of <see cref="DicomFile"/> and <see cref="DicomMessage"/> objects.
	/// </para>
	/// <para>
	/// 
	/// </para>
	/// </remarks>
	public partial class DicomAttributeCollection : IEnumerable<DicomAttribute>, IDicomAttributeProvider
	{
		#region Member Variables

		private readonly SortedDictionary<uint, DicomAttribute> _attributeList = new SortedDictionary<uint, DicomAttribute>();
		private String _specificCharacterSet = String.Empty;
		private readonly uint _startTag = 0x00000000;
		private readonly uint _endTag = 0xFFFFFFFF;

		private static readonly bool ValidateVrLengthsDefault = DicomSettings.Default.ValidateVrLengths;
		private static readonly bool ValidateVrValuesDefault = DicomSettings.Default.ValidateVrValues;
		private static readonly bool IgnoreOutOfRangeTagsDefault = DicomSettings.Default.IgnoreOutOfRangeTags;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constuctor.
		/// </summary>
		public DicomAttributeCollection()
		{
			ValidateVrLengths = ValidateVrLengthsDefault;
			ValidateVrValues = ValidateVrValuesDefault;
			IgnoreOutOfRangeTags = IgnoreOutOfRangeTagsDefault;
		}

		/// <summary>
		/// Contructor that sets the range of tags in use for the collection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This constructor is used to set a range of valid tags for the collection.
		/// All tags must be greater than or equal to <paramref name="startTag"/>
		/// ad less than or equal to <paramref name="endTag"/>.  
		/// </para>
		/// <para>
		/// The <see cref="DicomMessage"/> and <see cref="DicomFile"/> classes use 
		/// this form of the constructor when creating the DataSet and MetaInfo 
		/// <see cref="DicomAttributeCollection"/> instances.</para>
		/// </remarks>
		/// <param name="startTag">The valid start tag for attributes in the collection.</param>
		/// <param name="endTag">The value stop tag for attributes in the collection.</param>
		public DicomAttributeCollection(uint startTag, uint endTag)
		{
			_startTag = startTag;
			_endTag = endTag;
			ValidateVrLengths = ValidateVrLengthsDefault;
			ValidateVrValues = ValidateVrValuesDefault;
			IgnoreOutOfRangeTags = IgnoreOutOfRangeTagsDefault;
		}

		/// <summary>
		/// Internal constructor used when creating a copy of an DicomAttributeCollection.
		/// </summary>
		/// <param name="source">The source collection to copy attributes from.</param>
		/// <param name="copyBinary"></param>
		/// <param name="copyPrivate"></param>
		/// <param name="copyUnknown"></param>
		internal DicomAttributeCollection(DicomAttributeCollection source, bool copyBinary, bool copyPrivate, bool copyUnknown)
			: this(source, copyBinary, copyPrivate, copyUnknown, 0xFFFFFFFF) {}

		internal DicomAttributeCollection(DicomAttributeCollection source, bool copyBinary, bool copyPrivate, bool copyUnknown, uint stopTag)
		{
			ValidateVrLengths = ValidateVrLengthsDefault;
			ValidateVrValues = ValidateVrValuesDefault;
			IgnoreOutOfRangeTags = IgnoreOutOfRangeTagsDefault;

			_startTag = source.StartTagValue;
			_endTag = source.EndTagValue;
			_specificCharacterSet = source.SpecificCharacterSet;

			foreach (DicomAttribute attrib in source)
			{
				if (attrib.Tag.TagValue >= stopTag)
					break;

				if (!copyPrivate && attrib.Tag.IsPrivate)
					continue;

				if (!copyUnknown && attrib is DicomAttributeUN)
					continue;

				if (copyBinary ||
				    (!(attrib is DicomAttributeOB)
				     && !(attrib is DicomAttributeOW)
				     && !(attrib is DicomAttributeOF)
				     && !(attrib is DicomAttributeOD)
				     && !(attrib is DicomFragmentSequence)))
				{
					this[attrib.Tag] = attrib.Copy(copyBinary);
				}
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The specific character set string associated with the collection.
		/// </summary>
		/// <remarks>An empty string is returned if the specific character set
		/// tag is not set for the collection.</remarks>
		public String SpecificCharacterSet
		{
			get { return _specificCharacterSet; }
			set
			{
				_specificCharacterSet = value;

				// This line forces the value to be placed in sequences when we don't want it to be, because of how the parser is set
				//this[DicomTags.SpecificCharacterSet].SetStringValue(_specificCharacterSet);
			}
		}

		/// <summary>
		/// The number of attributes in the collection.
		/// </summary>
		public int Count
		{
			get { return _attributeList.Count; }
		}

		/// <summary>
		/// The first valid tag for attributes in the collection.
		/// </summary>
		public uint StartTagValue
		{
			get { return _startTag; }
		}

		/// <summary>
		/// The last valid tag for attributes in the collection.
		/// </summary>
		public uint EndTagValue
		{
			get { return _endTag; }
		}

		/// <summary>
		/// Gets the dump string (useful for seeing the dump output in the debugger's local variables window).
		/// </summary>
		/// <value>The dump string.</value>
		public string DumpString
		{
			get { return Dump(String.Empty, DicomDumpOptions.None); }
		}

		public bool ValidateVrLengths { get; set; }

		public bool ValidateVrValues { get; set; }

		public bool IgnoreOutOfRangeTags { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines if an attribute collection is empty.
		/// </summary>
		/// <returns>true if empty (no tags have a value), false otherwise.</returns>
		public bool IsEmpty()
		{
			foreach (DicomAttribute attr in this)
			{
				if (attr.Count > 0)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Check if a tag is contained in an DicomAttributeCollection and has a value.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool Contains(uint tag)
		{
			DicomAttribute attrib;
			return TryGetAttribute(tag, out attrib);
		}

		/// <summary>
		/// Check if a tag is contained in an DicomAttributeCollection and has a value.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool Contains(DicomTag tag)
		{
			DicomAttribute attrib;
			return TryGetAttribute(tag, out attrib);
		}

		/// <summary>
		/// Combines the functionality of the <see cref="Contains(uint)"/> method with the indexer.  Returns the attribute if it exists within the collection.
		/// </summary>
		/// <param name="tag">The tag to get.</param>
		/// <param name="attrib">The output attribute.  Null if the attribute doesn't exist in the collection.  Will be set if the attribute exists, but is empty.</param>
		/// <returns>true if the attribute exists and is not empty.</returns>
		public bool TryGetAttribute(uint tag, out DicomAttribute attrib)
		{
			if (!_attributeList.TryGetValue(tag, out attrib))
			{
				return false;
			}

			return !attrib.IsEmpty;
		}

		/// <summary>
		/// Combines the functionality of the <see cref="Contains(uint)"/> method with the indexer.  Returns the attribute if it exists within the collection.
		/// </summary>
		/// <param name="tag">The tag to get.</param>
		/// <param name="attrib">The output attribute.  Null if the attribute doesn't exist in the collection.  Will be set if the attribute exists, but is empty.</param>
		/// <returns>true if the attribute exists and is not empty.</returns>
		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attrib)
		{
			if (!_attributeList.TryGetValue(tag.TagValue, out attrib))
			{
				return false;
			}

			return !attrib.IsEmpty;
		}

		/// <summary>
		/// Get a <see cref="DicomAttribute"/> for a specific DICOM tag.
		/// </summary>
		/// <remarks>
		/// <para>If <paramref name="tag"/> does not exist in the collection, a new <see cref="DicomAttribute"/>
		/// instance is created, however, it is not added to the collection.</para>
		/// <para>A check is done to be sure that <paramref name="tag"/> is a valid DICOM tag in the 
		/// <see cref="DicomTagDictionary"/>.  If it is not a valid tag, a <see cref="DicomException"/>
		/// is thrown.</para>
		/// </remarks>
		/// <param name="tag">The DICOM tag.</param>
		/// <returns>A <see cref="DicomAttribute"/> instance.</returns>
		public DicomAttribute GetAttribute(uint tag)
		{
			DicomAttribute attr;
			if (!_attributeList.TryGetValue(tag, out attr))
			{
				if (((tag < _startTag) || (tag > _endTag)) && !IgnoreOutOfRangeTags)
					throw new DicomException("Tag is out of range for collection: " + tag.ToString("X8"));

				DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);

				if (dicomTag == null)
				{
					throw new DicomException("Invalid tag: " + tag.ToString("X8"));
				}

				attr = dicomTag.CreateDicomAttribute();
			}

			return attr;
		}

		/// <summary>
		/// Get a <see cref="DicomAttribute"/> for a specific DICOM tag.
		/// </summary>
		/// <remarks>
		/// <para>If <paramref name="tag"/> does not exist in the collection, a new <see cref="DicomAtribute"/>
		/// instance is created, however, it is not added to the collection.</para>
		/// <para>A check is done to be sure that <paramref name="tag"/> is a valid DICOM tag in the 
		/// <see cref="DicomTagDictionary"/>.  If it is not a valid tag, a <see cref="DicomException"/>
		/// is thrown.</para>
		/// </remarks>
		/// <param name="tag">The DICOM tag.</param>
		/// <returns>A <see cref="DicomAttribute"/> instance.</returns>
		public DicomAttribute GetAttribute(DicomTag tag)
		{
			DicomAttribute attr;

			if (tag == null)
				throw new NullReferenceException("Null DicomTag parameter");

			if (!_attributeList.TryGetValue(tag.TagValue, out attr))
			{
				if (((tag.TagValue < _startTag) || (tag.TagValue > _endTag)) && !IgnoreOutOfRangeTags)
					throw new DicomException("Tag is out of range for collection: " + tag);

				attr = tag.CreateDicomAttribute();

				if (attr == null)
					throw new DicomException("Invalid tag: " + tag.HexString);
			}

			return attr;
		}

		/// <summary>
		/// Removes the specified attribute from the collection.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns><b>true</b> if the tag is successfully removed. The method also return true if the specified tag does not exist in the collection.</returns>
		public bool RemoveAttribute(uint tag)
		{
			DicomAttribute attr;
			if (!_attributeList.TryGetValue(tag, out attr))
			{
				// doesn't exist. Ignore it
				return true;
			}
			return _attributeList.Remove(tag);
		}

		/// <summary>
		/// Removes the specified tag from the collection.
		/// </summary>
		/// <param name="tag">The DICOM tag to be removed.</param>
		/// <returns><b>true</b> if the tag is successfully removed. The method also return true if the specified tag does not exist in the collection.</returns>
		public bool RemoveAttribute(DicomTag tag)
		{
			return RemoveAttribute(tag.TagValue);
		}

		/// <summary>
		/// Indexer to return a specific tag in the attribute collection.
		/// </summary>
		/// <remarks>
		/// <para>When setting, if the value is null, the tag will be removed from the collection.</para>
		/// <para>If the tag does not exist within the collection, a new <see cref="DicomAttribute"/>
		/// derived instance will be created and returned by this indexer.</para>
		/// </remarks>
		/// <param name="tag">The tag to look for.</param>
		/// <returns></returns>
		public DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attr;

				if (!_attributeList.TryGetValue(tag, out attr))
				{
					DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);

					if (dicomTag == null)
					{
						dicomTag = DicomTag.GetTag(tag);
					}
					attr = dicomTag.CreateDicomAttribute();
					if ((tag < _startTag) || (tag > _endTag))
					{
						if (!IgnoreOutOfRangeTags)
							throw new DicomException("Tag is out of range for collection: " + tag);
					}
					else
					{
						attr.ParentCollection = this;
						_attributeList[tag] = attr;
					}
				}

				return attr;
			}
			set
			{
				if (value == null)
				{
					DicomAttribute attr;
					if (_attributeList.TryGetValue(tag, out attr))
					{
						attr.ParentCollection = null;
						_attributeList.Remove(tag);
					}
				}
				else
				{
					if (value.Tag.TagValue != tag)
						throw new DicomException("Tag being set does not match tag in DicomAttribute");

					if ((tag < _startTag) || (tag > _endTag))
					{
						if (!IgnoreOutOfRangeTags)
							throw new DicomException("Tag is out of range for collection: " + tag);
					}
					else
					{
						_attributeList[tag] = value;
						value.ParentCollection = this;
					}
				}
			}
		}

		/// <summary>
		/// Indexer when retrieving a specific tag in the collection.
		/// </summary>
		/// <remarks>
		/// <para>When setting, if the value is null, the tag will be removed from the collection.</para>
		/// <para>If the tag does not exist within the collection, a new <see cref="DicomAttribute"/>
		/// derived instance will be created and returned by this indexer.</para>
		/// </remarks>
		/// <param name="tag"></param>
		/// <returns></returns>
		public DicomAttribute this[DicomTag tag]
		{
			get
			{
				DicomAttribute attr;

				if (!_attributeList.TryGetValue(tag.TagValue, out attr))
				{
					attr = tag.CreateDicomAttribute();

					if (attr == null)
					{
						throw new DicomException("Invalid tag: " + tag.HexString);
					}
					if ((tag.TagValue < _startTag) || (tag.TagValue > _endTag))
					{
						if (!IgnoreOutOfRangeTags)
							throw new DicomException("Tag is out of range for collection: " + tag);
					}
					else
					{
						attr.ParentCollection = this;
						_attributeList[tag.TagValue] = attr;
					}
				}

				return attr;
			}
			set
			{
				if (value == null)
				{
					DicomAttribute attr;
					if (_attributeList.TryGetValue(tag.TagValue, out attr))
					{
						attr.ParentCollection = null;
						_attributeList.Remove(tag.TagValue);
					}
				}
				else
				{
					uint tagValue = tag.TagValue;
					if (value.Tag.TagValue != tagValue)
						throw new DicomException("Tag being set does not match tag in DicomAttribute");

					if ((tagValue < _startTag) || (tagValue > _endTag))
					{
						if (!IgnoreOutOfRangeTags)
							throw new DicomException("Tag is out of range for collection: " + tag);
					}
					else
					{
						_attributeList[tagValue] = value;
						value.ParentCollection = this;
					}
				}
			}
		}

		/// <summary>
		/// Create a duplicate copy of the DicomAttributeCollection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method creates a copy of all of the attributes within the DicomAttributeCollection and returns 
		/// a new copy.  Note that binary attributes with a VR of OB, OW, OF, OD, and UN are copied.
		/// </para>
		/// </remarks>
		/// <returns>A new DicomAttributeCollection.</returns>
		public virtual DicomAttributeCollection Copy()
		{
			return Copy(true, true, true);
		}

		/// <summary>
		/// Create a duplicate copy of the DicomAttributeCollection.
		/// </summary>
		/// <remarks>This method will not copy <see cref="DicomAttributeOB"/>,
		/// <see cref="DicomAttributeOW"/> and <see cref="DicomAttributeOF"/>
		/// instances if the <paramref name="copyBinary"/> parameter is set
		/// to false.</remarks>
		/// <param name="copyBinary">Flag to set if binary VR (OB, OW, OF, OD) attributes will be copied.</param>
		/// <param name="copyPrivate">Flag to set if Private attributes will be copied</param>
		/// <param name="copyUnknown">Flag to set if UN VR attributes will be copied</param>
		/// <returns>a new DicomAttributeCollection.</returns>
		public virtual DicomAttributeCollection Copy(bool copyBinary, bool copyPrivate, bool copyUnknown)
		{
			return new DicomAttributeCollection(this, copyBinary, copyPrivate, copyUnknown);
		}

		/// <summary>
		/// Create a duplicate copy of the DicomAttributeCollection.
		/// </summary>
		/// <remarks>This method will not copy <see cref="DicomAttributeOB"/>,
		/// <see cref="DicomAttributeOW"/> and <see cref="DicomAttributeOF"/>
		/// instances if the <paramref name="copyBinary"/> parameter is set
		/// to false.</remarks>
		/// <param name="copyBinary">Flag to set if binary VR (OB, OW, OF, OD) attributes will be copied.</param>
		/// <param name="copyPrivate">Flag to set if Private attributes will be copied.</param>
		/// <param name="stopTag">Indicates a tag at which to stop copying.</param>
		/// <param name="copyUnknown">Flag to set if UN VR attributes will be copied.</param>
		/// <returns>a new DicomAttributeCollection.</returns>
		public virtual DicomAttributeCollection Copy(bool copyBinary, bool copyPrivate, bool copyUnknown, uint stopTag)
		{
			return new DicomAttributeCollection(this, copyBinary, copyPrivate, copyUnknown, stopTag);
		}

		/// <summary>
		/// Check if the contents of the DicomAttributeCollection is identical to another DicomAttributeCollection instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method compares the contents of two attribute collections to see if they are equal.  The method
		/// will step through each of the tags within the collection, and compare them to see if they are equal.  The
		/// method will also recurse into sequence attributes to be sure they are equal.</para>
		/// </remarks>
		/// <param name="obj">The objec to compare to.</param>
		/// <returns>true if the collections are equal.</returns>
		public override bool Equals(object obj)
		{
			List<DicomAttributeComparisonResult> failureReason = new List<DicomAttributeComparisonResult>();
			return Equals(obj, ref failureReason);
		}

		/// <summary>
		/// Check if the contents of the DicomAttributeCollection is identical to another DicomAttributeCollection instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method compares the contents of two attribute collections to see if they are equal.  The method
		/// will step through each of the tags within the collection, and compare them to see if they are equal.  The
		/// method will also recurse into sequence attributes to be sure they are equal.</para>
		/// </remarks>
		/// <param name="obj">The objec to compare to.</param>
		/// <param name="comparisonFailure">An output string describing why the objects are not equal.</param>
		/// <param name="failures">List of tags that failed comparison.</param>
		/// <returns>true if the collections are equal.</returns>
		public bool Equals(object obj, ref List<DicomAttributeComparisonResult> failures)
		{
			DicomAttributeCollection a = obj as DicomAttributeCollection;
			if (a == null)
			{
				DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
				result.ResultType = ComparisonResultType.InvalidType;
				result.Details = String.Format("Comparison object is invalid type: {0}", obj.GetType());
				failures.Add(result);
				return false;
			}

			IEnumerator<DicomAttribute> thisEnumerator = GetEnumerator();
			IEnumerator<DicomAttribute> compareEnumerator = a.GetEnumerator();

			for (;;)
			{
				bool thisValidNext = thisEnumerator.MoveNext();
				bool compareValidNext = compareEnumerator.MoveNext();

				// Skip empty attributes
				while (thisValidNext && thisEnumerator.Current.IsEmpty)
					thisValidNext = thisEnumerator.MoveNext();
				while (compareValidNext && compareEnumerator.Current.IsEmpty)
					compareValidNext = compareEnumerator.MoveNext();

				if (!thisValidNext && !compareValidNext)
					break; // break & exit with true

				if (!thisValidNext || !compareValidNext)
				{
					DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
					result.ResultType = ComparisonResultType.DifferentAttributeSet;
					result.Details = String.Format("Invalid last tag in attribute collection");
					failures.Add(result);
					return false;
				}
				DicomAttribute thisAttrib = thisEnumerator.Current;
				DicomAttribute compareAttrib = compareEnumerator.Current;

				if (thisAttrib.Tag.Element == 0x0000)
				{
					thisValidNext = thisEnumerator.MoveNext();

					if (!thisValidNext)
					{
						DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
						result.ResultType = ComparisonResultType.DifferentAttributeSet;
						result.Details = String.Format("Invalid last tag in attribute collection");
						failures.Add(result);
						return false;
					}
					thisAttrib = thisEnumerator.Current;
				}

				if (compareAttrib.Tag.Element == 0x0000)
				{
					compareValidNext = compareEnumerator.MoveNext();

					if (!compareValidNext)
					{
						DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
						result.ResultType = ComparisonResultType.DifferentAttributeSet;
						result.Details = String.Format("Invalid last tag in attribute collection");
						failures.Add(result);
						return false;
					}
					compareAttrib = compareEnumerator.Current;
				}

				if (!thisAttrib.Tag.Equals(compareAttrib.Tag))
				{
					DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
					result.ResultType = ComparisonResultType.DifferentValues;
					result.TagName = thisAttrib.Tag.Name;
					result.Details = String.Format(
						"Source tag {0} and comparison message tag {1} not the same, possible missing tag.",
						thisAttrib.Tag, compareAttrib.Tag);

					failures.Add(result);
					return false;
				}
				if (!thisAttrib.Equals(compareAttrib))
				{
					if (thisAttrib.StreamLength < 64 && compareAttrib.StreamLength < 64)
					{
						DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
						result.ResultType = ComparisonResultType.DifferentValues;
						result.TagName = thisAttrib.Tag.Name;
						result.Details = String.Format("Tag {0} values not equal, Base value: '{1}', Comparison value: '{2}'",
						                               thisAttrib.Tag,
						                               thisAttrib, compareAttrib);
						failures.Add(result);
					}
					else
					{
						DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
						result.ResultType = ComparisonResultType.DifferentValues;
						result.TagName = thisAttrib.Tag.Name;
						result.Details = String.Format("Tag {0} values not equal in message", thisAttrib.Tag);
						failures.Add(result);
					}
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Override to get a hash code to represent the object.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode(); // TODO
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Used to calculate group lengths.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="syntax"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		internal uint CalculateGroupWriteLength(ushort group, TransferSyntax syntax, DicomWriteOptions options)
		{
			uint length = 0;
			foreach (DicomAttribute item in this)
			{
				if (item.Tag.Group < group || item.IsEmpty || item.Tag.Element == 0x0000) // skip Group Length elements and empty elements
					continue;
				if (item.Tag.Group > group)
					return length;
				length += item.CalculateWriteLength(syntax, options);
			}
			return length;
		}

		/// <summary>
		/// Used to calculate the write length of the collection.
		/// </summary>
		/// <param name="syntax"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		internal uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
		{
			return CalculateWriteLength(uint.MinValue, uint.MaxValue, syntax, options);
		}

		/// <summary>
		/// Used to calculate the write length of the collection.
		/// </summary>
		/// <param name="startTag"></param>
		/// <param name="stopTag"></param>
		/// <param name="syntax"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		internal uint CalculateWriteLength(uint startTag, uint stopTag, TransferSyntax syntax, DicomWriteOptions options)
		{
			uint length = 0;
			ushort group = 0xffff;

			foreach (DicomAttribute item in this)
			{
				if (item.Tag.TagValue < startTag || item.IsEmpty || item.Tag.Element == 0x0000) // skip Group Length elements and empty elements
					continue;
				if (item.Tag.TagValue > stopTag)
					return length;

				// once for each group, add on the length of the Group Length element if the option was specified
				if (item.Tag.Group != group)
				{
					group = item.Tag.Group;
					if (Flags.IsSet(options, DicomWriteOptions.CalculateGroupLengths))
					{
						// Group Length (gggg,0000) is VR=UL, VM=1
						// under explicit VR encoding, the length of the element is 4 (tag) + 2 (VR) + 2 (length) + 4 (value) = 12
						// under implicit VR encoding, the length of the element is 4 (tag) + 4 (length) + 4 (value) = 12
						length += 12;
					}
				}

				// add on the actual length of the element
				length += item.CalculateWriteLength(syntax, options);
			}
			return length;
		}

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		/// Method for implementing the <see cref="IEnumerable"/> interface.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<DicomAttribute> GetEnumerator()
		{
			return _attributeList.Values.GetEnumerator();
		}

		/// <summary>
		/// Method for implementing the <see cref="IEnumerable"/> interface.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Binding

		/// <summary>
		/// Internal method for getting a default value for an attribute.
		/// </summary>
		/// <param name="vtype"></param>
		/// <param name="deflt"></param>
		/// <returns></returns>
		private static object GetDefaultValue(Type vtype, DicomFieldDefault deflt)
		{
			try
			{
				if (deflt == DicomFieldDefault.Null || deflt == DicomFieldDefault.None)
					return null;
				if (deflt == DicomFieldDefault.DBNull)
					return DBNull.Value;
				if (deflt == DicomFieldDefault.Default && vtype != typeof (string))
					return Activator.CreateInstance(vtype);
				if (vtype == typeof (string))
				{
					if (deflt == DicomFieldDefault.StringEmpty || deflt == DicomFieldDefault.Default)
						return String.Empty;
				}
				else if (vtype == typeof (DateTime))
				{
					if (deflt == DicomFieldDefault.DateTimeNow)
						return DateTime.Now;
					if (deflt == DicomFieldDefault.MinValue)
						return DateTime.MinValue;
					if (deflt == DicomFieldDefault.MaxValue)
						return DateTime.MaxValue;
				}
				else if (vtype.IsSubclassOf(typeof (ValueType)))
				{
					if (deflt == DicomFieldDefault.MinValue)
					{
						PropertyInfo pi = vtype.GetProperty("MinValue", BindingFlags.Static);
						if (pi != null) return pi.GetValue(null, null);
					}
					if (deflt == DicomFieldDefault.MaxValue)
					{
						PropertyInfo pi = vtype.GetProperty("MaxValue", BindingFlags.Static);
						if (pi != null) return pi.GetValue(null, null);
					}
					return Activator.CreateInstance(vtype);
				}
				return null;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error in default value type! - {0}", vtype.ToString());
				return null;
			}
		}

		private static object LoadDicomFieldValue(DicomAttribute elem, Type vtype, DicomFieldDefault deflt, bool udzl)
		{
			if (vtype.IsSubclassOf(typeof (DicomAttribute)))
			{
				if (elem != null && vtype != elem.GetType())
					throw new DicomDataException("Invalid binding type for Element VR!");
				return elem;
			}
			if (vtype.IsArray)
			{
				if (elem != null)
				{
					if (vtype.GetElementType() == typeof (float) && (elem.Tag.VR == DicomVr.DSvr))
					{
						float[] array = new float[elem.Count];
						for (int i = 0; i < array.Length; i++)
						{
							elem.TryGetFloat32(i, out array[i]);
						}
						return array;
					}
					if (vtype.GetElementType() == typeof (double) && (elem.Tag.VR == DicomVr.DSvr))
					{
						double[] array = new double[elem.Count];
						for (int i = 0; i < array.Length; i++)
							elem.TryGetFloat64(i, out array[i]);

						return array;
					}

					if (vtype.GetElementType() != elem.GetValueType())
						throw new DicomDataException("Invalid binding type for Element VR!");
					//if (elem.GetValueType() == typeof(DateTime))
					//    return (elem as AbstractAttribute).GetDateTimes();
					return elem.Values;
				}
				if (deflt == DicomFieldDefault.EmptyArray)
					return Array.CreateInstance(vtype, 0);
				return null;
			}
			if (elem != null)
			{
				if ((elem.IsNull || elem.IsEmpty) && udzl)
				{
					return GetDefaultValue(vtype, deflt);
				}
				if (vtype == typeof (string))
				{
					return elem.ToString();
				}

				Type nullableType;
				if (null != (nullableType = Nullable.GetUnderlyingType(vtype)) || vtype.IsValueType)
				{
					bool isNullable = nullableType != null;
					Type valueType = nullableType ?? vtype;

					if (valueType == typeof (ushort))
					{
						ushort value;
						if (!elem.TryGetUInt16(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (short))
					{
						short value;
						if (!elem.TryGetInt16(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (uint))
					{
						uint value;
						if (!elem.TryGetUInt32(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (int))
					{
						int value;
						if (!elem.TryGetInt32(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (UInt64))
					{
						UInt64 value;
						if (!elem.TryGetUInt64(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (Int64))
					{
						Int64 value;
						if (!elem.TryGetInt64(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (float))
					{
						float value;
						if (!elem.TryGetFloat32(0, out value) && isNullable)
							return null;

						return value;
					}
					if (valueType == typeof (double))
					{
						double value;
						if (!elem.TryGetFloat64(0, out value) && isNullable)
							return null;
						return value;
					}
					if (valueType == typeof (DateTime))
					{
						DateTime value;
						if (!elem.TryGetDateTime(0, out value) && isNullable)
							return null;
						return value;
					}
				}

				if (vtype != elem.GetValueType())
				{
					if (vtype == typeof (DicomUid) && elem.Tag.VR == DicomVr.UIvr)
					{
						DicomUid uid;
						elem.TryGetUid(0, out uid);
						return uid;
					}
					if (vtype == typeof (TransferSyntax) && elem.Tag.VR == DicomVr.UIvr)
					{
						return TransferSyntax.GetTransferSyntax(elem.ToString());
					}
					//else if (vtype == typeof(DcmDateRange) && elem.GetType().IsSubclassOf(typeof(AttributeMultiValueText)))
					//{
					//    return (elem as AbstractAttribute).GetDateTimeRange();
					// }
					if (vtype == typeof (object))
					{
						return elem.Values;
					}
					throw new DicomDataException("Invalid binding type for Element VR!");
				}
				return elem.Values;
			}
			return GetDefaultValue(vtype, deflt);
		}

		/// <summary>
		/// Load the contents of attributes in the collection into a structure or class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method will use reflection to look at the contents of the object specified by
		/// <paramref name="obj"/> and copy the values of attributes within this collection to
		/// fields in the object with the <see cref="DicomFieldAttribute"/> attribute set for
		/// them.
		/// </para>
		/// </remarks>
		/// <param name="obj"></param>
		/// <seealso cref="DicomFieldAttribute"/>
		public bool LoadDicomFields(object obj)
		{
			// TODO: perhaps we should throw exception if there's problem binding any field so the caller knows the fields in the object may be incomplete.
			// Decided to return a boolean instead so existing code still works.
			var failureCount = 0;

			FieldData[] fields;
			PropertyData[] properties;
			GetPropertiesAndFields(obj.GetType(), out fields, out properties);
			foreach (var f in fields)
			{
				var field = f.FieldInfo;
				var dfa = f.DicomFieldAttribute;
				try
				{
					if ((dfa.Tag.TagValue >= StartTagValue) && (dfa.Tag.TagValue <= EndTagValue))
					{
						DicomAttribute elem;
						if (TryGetAttribute(dfa.Tag, out elem))
						{
							if (dfa.DefaultValue == DicomFieldDefault.None
							    && dfa.UseDefaultForZeroLength
							    && (elem.IsNull || elem.IsEmpty))
							{
								// do nothing
							}
							else
							{
								field.SetValue(obj,
								               LoadDicomFieldValue(elem, field.FieldType, dfa.DefaultValue,
								                                   dfa.UseDefaultForZeroLength));
							}
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unable to bind field {0}", field.Name);
					failureCount++;
				}
			}

			foreach (var p in properties)
			{
				var property = p.PropertyInfo;
				var dfa = p.DicomFieldAttribute;
				try
				{
					DicomAttribute elem;
					if (TryGetAttribute(dfa.Tag, out elem))
					{
						if (dfa.DefaultValue == DicomFieldDefault.None
						    && dfa.UseDefaultForZeroLength
						    && (elem.IsNull || elem.IsEmpty))
						{
							// do nothing
						}
						else
						{
							property.SetValue(obj, LoadDicomFieldValue(elem, property.PropertyType, dfa.DefaultValue, dfa.UseDefaultForZeroLength), null);
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unable to bind property {0}", property.Name);
					failureCount++;
				}
			}

			return failureCount == 0;
		}

		private void SaveDicomFieldValue(DicomTag tag, object value, bool createEmpty, bool setNullIfEmpty)
		{
			if (value != null && value != DBNull.Value)
			{
				Type vtype = value.GetType();
				DicomSequenceItem sq;
				if (vtype == this[tag].GetType())
				{
					this[tag] = (DicomAttribute) value;
				}
				else if (null != (sq = value as DicomSequenceItem))
				{
					DicomAttribute elem = this[tag];
					elem.AddSequenceItem(sq);
				}
				else
				{
					DicomAttribute elem = this[tag];
					if (vtype.IsArray)
					{
						if (vtype.GetElementType() != elem.GetValueType())
							throw new DicomDataException("Invalid binding type for Element VR!");
//                        if (elem.GetValueType() == typeof(DateTime))
						//                          (elem as AbstractAttribute).SetDateTimes((DateTime[])value);
						//                    else
						elem.Values = value;
					}
					else
					{
						if (elem.Tag.VR == DicomVr.UIvr && vtype == typeof (DicomUid))
						{
							DicomUid ui = (DicomUid) value;
							elem.SetStringValue(ui.UID);
						}
						else if (elem.Tag.VR == DicomVr.UIvr && vtype == typeof (TransferSyntax))
						{
							TransferSyntax ts = (TransferSyntax) value;
							elem.SetStringValue(ts.DicomUid.UID);
						}
							//  else if (vtype == typeof(DcmDateRange) && elem.GetType().IsSubclassOf(typeof(AbstractAttribute)))
							//  {
							//      DcmDateRange dr = (DcmDateRange)value;
							//      (elem as AbstractAttribute).SetDateTimeRange(dr);
							//  }
						else if (vtype != elem.GetValueType())
						{
							if (vtype == typeof (string))
							{
								elem.SetStringValue((string) value);
							}
							else
							{
								Type nullableType;
								if (null != (nullableType = Nullable.GetUnderlyingType(vtype)) || vtype.IsValueType)
								{
									Type valueType = nullableType ?? vtype;

									if (valueType == typeof (UInt16))
										elem.SetUInt16(0, (UInt16) value);
									else if (valueType == typeof (Int16))
										elem.SetInt16(0, (Int16) value);
									else if (valueType == typeof (UInt32))
										elem.SetUInt32(0, (UInt32) value);
									else if (valueType == typeof (Int32))
										elem.SetInt32(0, (Int32) value);
									else if (valueType == typeof (Int64))
										elem.SetInt64(0, (Int64) value);
									else if (valueType == typeof (UInt64))
										elem.SetUInt64(0, (UInt64) value);
									else if (valueType == typeof (float))
										elem.SetFloat32(0, (float) value);
									else if (valueType == typeof (double))
										elem.SetFloat64(0, (double) value);
									else if (valueType == typeof (DateTime))
										elem.SetDateTime(0, (DateTime) value);
									else
										throw new DicomDataException("Invalid binding type for Element VR!");
								}
								else
									throw new DicomDataException("Invalid binding type for Element VR!");
							}
						}
						else
						{
							elem.Values = value;
						}
					}
				}
			}
			else
			{
				DicomAttribute attr;
				if (createEmpty)
				{
					// force the element creation
					attr = this[tag];
					if (setNullIfEmpty)
						attr.SetNullValue();
				}
				else if (TryGetAttribute(tag, out attr))
				{
					attr.Values = null;
				}
			}
		}

		/// <summary>
		/// This method will copy attributes from the input object into the collection.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="obj">The object to copy values out of into the collection.</param>
		/// <seealso cref="DicomFieldAttribute"/>
		public void SaveDicomFields(object obj)
		{
			FieldData[] fields;
			PropertyData[] properties;
			GetPropertiesAndFields(obj.GetType(), out fields, out properties);
			foreach (var field in fields)
			{
				var dfa = field.DicomFieldAttribute;
				object value = field.FieldInfo.GetValue(obj);
				SaveDicomFieldValue(dfa.Tag, value, dfa.CreateEmptyElement, dfa.SetNullValueIfEmpty);
			}

			foreach (var property in properties)
			{
				var dfa = property.DicomFieldAttribute;
				object value = property.PropertyInfo.GetValue(obj, null);
				SaveDicomFieldValue(dfa.Tag, value, dfa.CreateEmptyElement, dfa.SetNullValueIfEmpty);
			}
		}

		#endregion

		#region Dump

		/// <summary>
		/// Method to dump the contents of the collection to a <see>StringBuilder</see> instance.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="prefix"></param>
		/// <param name="options"></param>
		public void Dump(StringBuilder sb, String prefix, DicomDumpOptions options)
		{
			if (sb == null) throw new ArgumentNullException("sb");
			foreach (DicomAttribute item in this)
			{
				// skip entries that were inserted into collection but are not "present"
				if (item.IsEmpty)
					continue;

				item.Dump(sb, prefix, options);
				sb.AppendLine();
			}
		}

		/// <summary>
		/// Method to dump the contents of a collection to a string.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public string Dump(string prefix, DicomDumpOptions options)
		{
			StringBuilder sb = new StringBuilder();
			Dump(sb, prefix, options);
			return sb.ToString();
		}

		/// <summary>
		/// Method to dump the contents of a collection to a string.
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public string Dump(string prefix)
		{
			return Dump(prefix, DicomDumpOptions.Default);
		}

		/// <summary>
		/// Method to dump the contents of a collection to a string.
		/// </summary>
		/// <returns></returns>
		public string Dump()
		{
			return Dump(String.Empty, DicomDumpOptions.Default);
		}

		#endregion
	}

	#region Type caching for Load/Save Dicom Fields

	partial class DicomAttributeCollection
	{
		private class FieldData
		{
			public FieldData(FieldInfo fieldInfo, DicomFieldAttribute dicomFieldAttribute)
			{
				FieldInfo = fieldInfo;
				DicomFieldAttribute = dicomFieldAttribute;
			}

			public readonly FieldInfo FieldInfo;
			public readonly DicomFieldAttribute DicomFieldAttribute;
		}

		private class PropertyData
		{
			public PropertyData(PropertyInfo propertyInfo, DicomFieldAttribute dicomFieldAttribute)
			{
				PropertyInfo = propertyInfo;
				DicomFieldAttribute = dicomFieldAttribute;
			}

			public readonly PropertyInfo PropertyInfo;
			public readonly DicomFieldAttribute DicomFieldAttribute;
		}

		private class TypeData
		{
			public TypeData(Type type, FieldData[] fields, PropertyData[] properties)
			{
				Type = type;
				Fields = fields;
				Properties = properties;
			}

			public readonly Type Type;
			public readonly FieldData[] Fields;
			public readonly PropertyData[] Properties;
		}

		private static TypeData _lastTypeData;
		private static readonly object _typeDataLock = new object();
		private static readonly Dictionary<Type, TypeData> _typeData = new Dictionary<Type, TypeData>();

		private void GetPropertiesAndFields(Type type, out FieldData[] fields, out PropertyData[] properties)
		{
			var last = _lastTypeData;
			//Faster than looking up in a dictionary, and it'll often be the same.
			if (last != null && last.Type == type)
			{
				fields = last.Fields;
				properties = last.Properties;
				return;
			}

			TypeData typeData;
			lock (_typeDataLock)
				_typeData.TryGetValue(type, out typeData);

			if (typeData == null)
			{
				typeData = new TypeData(type,
				                        (from field in type.GetFields()
				                         let attributes = field.GetCustomAttributes(typeof (DicomFieldAttribute), true)
				                         where attributes.Length > 0
				                         select new FieldData(field, (DicomFieldAttribute) attributes[0])).ToArray(),
				                        (from property in type.GetProperties()
				                         let attributes = property.GetCustomAttributes(typeof (DicomFieldAttribute), true)
				                         where attributes.Length > 0
				                         select new PropertyData(property, (DicomFieldAttribute) attributes[0])).ToArray());

				lock (_typeDataLock)
					_typeData[type] = typeData;
			}

			fields = typeData.Fields;
			properties = typeData.Properties;

			_lastTypeData = typeData;
		}
	}

	#endregion
}