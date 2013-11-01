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
using System.Text;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// The DicomTag class contains all DICOM information for a specific tag.
	/// </summary>
	/// <remarks>
	/// <para>The DicomTag class is used as described in the Flyweight pattern.  A single instance should only be allocated
	/// for each DICOM tag, and that instance will be shared in any <see cref="DicomAttributeCollection"/> 
	/// that references the specific tag.</para>
	/// <para>Note, however, that non standard DICOM tags (or tags not in stored in the <see cref="DicomTagDictionary"/>
	/// will have a specific instance allocated to store their information when they are encountered by the assembly.</para>
	/// </remarks>
	public class DicomTag : IComparable
	{
		#region Static Members

		/// <summary>
		/// Return a uint with a tags value based on the input group and element.
		/// </summary>
		/// <param name="group">The Group for the tag.</param>
		/// <param name="element">The Element for the tag.</param>
		/// <returns></returns>
		public static uint GetTagValue(ushort group, ushort element)
		{
			return (uint) group << 16 | element;
		}

		/// <summary>
		/// Checks if a Group is private (odd).
		/// </summary>
		/// <param name="group">The Group to check.</param>
		/// <returns>true if the Group is private, false otherwise.</returns>
		public static bool IsPrivateGroup(ushort group)
		{
			return (group & 1) == 1;
		}

		/// <summary>
		/// Gets a <see cref="DicomTag"/> instance for the specified tag, which may not necessary be a tag defined in the data dictionary.
		/// </summary>
		/// <remarks>
		/// Whereas <see cref="DicomTagDictionary.GetDicomTag(uint)"/> returns only tags defined in the data dictionary,
		/// this method returns a tag instance even for undefined tags (e.g. private tags, group lengths or other unknown tags).
		/// </remarks>
		/// <param name="tag">The tag value.</param>
		public static DicomTag GetTag(uint tag)
		{
			return GetTag((ushort) ((tag >> 16) & 0x0000ffff), (ushort) (tag & 0x0000ffff));
		}

		/// <summary>
		/// Gets a <see cref="DicomTag"/> instance for the specified tag, which may not necessary be a tag defined in the data dictionary.
		/// </summary>
		/// <remarks>
		/// Whereas <see cref="DicomTagDictionary.GetDicomTag(ushort,ushort)"/> returns only tags defined in the data dictionary,
		/// this method returns a tag instance even for undefined tags (e.g. private tags, group lengths or other unknown tags).
		/// </remarks>
		/// <param name="group">The Group for the tag.</param>
		/// <param name="element">The Element for the tag.</param>
		public static DicomTag GetTag(ushort group, ushort element)
		{
			DicomTag dicomTag;
			var tagValue = GetTagValue(group, element);
			if (DicomTagDictionary.TagDictionary.TryGetValue(tagValue, out dicomTag)) return dicomTag;

			if (element == 0)
				return CreateGroupLengthTag(group);
			else if ((group & 1) == 1)
				return (element & 0xff00) == 0 ? CreatePrivateCreatorTag(@group, (byte) element) : CreatePrivateTag(@group, element);
			return new DicomTag(tagValue, "Unknown Tag", "UnknownTag", DicomVr.UNvr, false, 1, 1, false);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a private creator code tag.
		/// </summary>
		/// <param name="group">The Group of the private tag.</param>
		/// <param name="element">The Element of any private tag in the block.</param>
		[Obsolete("Consider using CreatePrivateCreatorTag(UInt16, Byte) instead.")]
		public static DicomTag GetPrivateCreatorTag(ushort group, ushort element)
		{
			return CreatePrivateCreatorTag(group, (byte) (element >> 8));
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a private creator code tag.
		/// </summary>
		/// <param name="group">The Group of the private tag.</param>
		/// <param name="block">The Private Block of the private tag.</param>
		public static DicomTag CreatePrivateCreatorTag(ushort group, byte block)
		{
			return new DicomTag(((uint) group << 16) | block, "Private Creator", "PrivateCreator", DicomVr.LOvr, false, 1, 1, false);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a private creator code tag.
		/// </summary>
		/// <param name="group">The Group of the private tag.</param>
		/// <param name="element">The Private Element of the private tag.</param>
		/// <param name="block">The Private Block of the private tag.</param>
		public static DicomTag CreatePrivateTag(ushort group, byte element, byte block = 0)
		{
			return CreatePrivateTag(group, element, DicomVr.UNvr, block);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a private creator code tag.
		/// </summary>
		/// <param name="group">The Group of the private tag.</param>
		/// <param name="element">The Private Element of the private tag.</param>
		/// <param name="vr">The VR of the private tag.</param>
		/// <param name="block">The Private Block of the private tag.</param>
		public static DicomTag CreatePrivateTag(ushort group, byte element, DicomVr vr, byte block = 0)
		{
			return CreatePrivateTag(group, (ushort) ((block << 8) | element), vr);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a private creator code tag.
		/// </summary>
		/// <param name="group">The Group of the private tag.</param>
		/// <param name="element">The Element of the private tag.</param>
		public static DicomTag CreatePrivateTag(ushort group, ushort element)
		{
			return CreatePrivateTag(group, element, DicomVr.UNvr);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a private creator code tag.
		/// </summary>
		/// <param name="group">The Group of the private tag.</param>
		/// <param name="element">The Element of the private tag.</param>
		/// <param name="vr">The VR of the private tag.</param>
		public static DicomTag CreatePrivateTag(ushort group, ushort element, DicomVr vr)
		{
			if (vr == null) vr = DicomVr.UNvr;
			return new DicomTag(((uint) group << 16) | element, "Private Tag", "PrivateTag", vr, false, 1, vr.IsMultiValue ? uint.MaxValue : 1, false);
		}

		/// <summary>
		/// Creates an instance of <see cref="DicomTag"/> representing a group length tag.
		/// </summary>
		/// <param name="group">The Group of the tag.</param>
		public static DicomTag CreateGroupLengthTag(ushort group)
		{
			// group length is retired for use in the data set (i.e. group >= 8)
			return new DicomTag(((uint) group << 16), "Group Length", "GroupLength", DicomVr.ULvr, false, 1, 1, group >= 8);
		}

		/// <summary>(fffe,e0dd) VR= Sequence Delimitation Item</summary>
		internal static DicomTag SequenceDelimitationItem = new DicomTag(0xfffee0dd, "Sequence Delimitation Item");

		/// <summary>(fffe,e000) VR= Item</summary>
		internal static DicomTag Item = new DicomTag(0xfffee000, "Item");

		/// <summary>(fffe,e00d) VR= Item Delimitation Item</summary>
		internal static DicomTag ItemDelimitationItem = new DicomTag(0xfffee00d, "Item Delimitation Item");

		#endregion

		#region Private Members

		private readonly string _name;
		private readonly string _varName;
		private readonly DicomVr _vr;
		private readonly uint _vmLow;
		private readonly uint _vmHigh;
		private readonly bool _isRetired;
		private readonly bool _multiVrTag;

		#endregion

		#region Constructors

		/// <summary>
		/// *** For XML serialization ***
		/// </summary>
		public DicomTag() {}

		/// <summary>
		/// Primary constructor for dictionary tags
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="name"></param>
		/// <param name="varName"></param>
		/// <param name="vr"></param>
		/// <param name="isMultiVrTag"></param>
		/// <param name="vmLow"></param>
		/// <param name="vmHigh"></param>
		/// <param name="isRetired"></param>
		public DicomTag(uint tag, String name, String varName, DicomVr vr, bool isMultiVrTag, uint vmLow, uint vmHigh, bool isRetired)
		{
			TagValue = tag;
			_name = name;
			_varName = varName;
			_vr = vr;
			_multiVrTag = isMultiVrTag;
			_vmLow = vmLow;
			_vmHigh = vmHigh;
			_isRetired = isRetired;
		}

		private DicomTag(uint tag, String name)
		{
			TagValue = tag;
			_name = name;
			_vr = DicomVr.UNvr;
			_multiVrTag = false;
			_vmLow = 1;
			_vmHigh = 1;
			_isRetired = false;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Group Number of the tag as a 16-bit unsigned integer.
		/// </summary>
		public ushort Group
		{
			get { return ((ushort) ((TagValue & 0xffff0000) >> 16)); }
		}

		/// <summary>
		/// Gets the Element Number of the tag as a 16-bit unsigned integer.
		/// </summary>
		public ushort Element
		{
			get { return ((ushort) (TagValue & 0x0000ffff)); }
		}

		/// <summary>
		/// Gets a text description of the tag.
		/// </summary>
		public String Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets a text description of the tag with spaces removed and proper .NET casing.
		/// </summary>
		public String VariableName
		{
			get { return _varName; }
		}

		/// <summary>
		/// Gets a boolean telling if the tag is retired.
		/// </summary>
		public bool Retired
		{
			get { return _isRetired; }
		}

		/// <summary>
		/// Gets a boolean telling if the tag supports multiple VRs.
		/// </summary>
		public bool MultiVR
		{
			get { return _multiVrTag; }
		}

		/// <summary>
		/// Returns a <see cref="DicomVr"/> object representing the Value Representation (VR) of the tag.
		/// </summary>
		public DicomVr VR
		{
			get { return _vr; }
		}

		/// <summary>
		/// Gets a uint representing the low Value of Multiplicity defined by DICOM for the tag. 
		/// </summary>
		public uint VMLow
		{
			get { return _vmLow; }
		}

		/// <summary>
		/// Gets a uint representing the high Value of Multiplicity defined by DICOM for the tag.
		/// </summary>
		public uint VMHigh
		{
			get { return _vmHigh; }
		}

		/// <summary>
		/// Gets a string representing the value of multiplicity defined by DICOM for the tag.
		/// </summary>
		public string VM
		{
			get
			{
				if (_vmLow == _vmHigh)
					return _vmLow.ToString();
				if (_vmHigh == uint.MaxValue)
					return _vmLow + "-N";
				return _vmLow + "-" + _vmHigh;
			}
		}

		/// <summary>
		/// Returns a uint DICOM Tag value for the object.
		/// </summary>
		/// <remarks>
		/// This was explicitly changed to a readonly member variable
		/// from a property as a performance improvement.  This value
		/// is referenced frequently and the change results in a small
		/// performance improvement.  
		/// </remarks>
		public readonly uint TagValue;

		/// <summary>
		/// Returns a string with the tag value in Hex
		/// </summary>
		public String HexString
		{
			get { return TagValue.ToString("X8"); }
		}

		/// <summary>
		/// Returns a bool as true if the tag is private
		/// </summary>
		public bool IsPrivate
		{
			get { return (Group & 1) == 1; }
		}

		#endregion

		#region System.Object Overrides

		/// <summary>
		/// Provides a hash code that's more natural by using the
		/// Group and Element number of the tag.
		/// </summary>
		/// <returns>The Group and Element number as a 32-bit integer.</returns>
		public override int GetHashCode()
		{
			return ((int) TagValue);
		}

		/// <summary>
		/// Provides a human-readable representation of the tag.
		/// </summary>
		/// <returns>The string representation of the Group and Element.</returns>
		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.AppendFormat("({0:x4},{1:x4}) ", Group, Element);
			buffer.Append(_name);

			return buffer.ToString();
		}

		/// <summary>
		/// This override allows the comparison of two DicomTag objects
		/// for semantic equivalence. 
		/// </summary>
		/// <param name="obj">The other DicomTag object to compare this object to.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			DicomTag otherTag = obj as DicomTag;
			if (null == otherTag)
				return false;

			return (otherTag.GetHashCode() == GetHashCode());
		}

		#endregion

		#region Operators

		/// <summary>
		/// Implicit cast to a String object, for ease of use.
		/// </summary>
		public static implicit operator String(DicomTag myTag)
		{
			return myTag.ToString();
		}

		/// <summary>
		/// Equality operator.
		/// </summary>
		public static bool operator ==(DicomTag t1, DicomTag t2)
		{
			if ((object) t1 == null && (object) t2 == null)
				return true;
			if ((object) t1 == null || (object) t2 == null)
				return false;
			return t1.TagValue == t2.TagValue;
		}

		/// <summary>
		/// Not equal operator.
		/// </summary>
		public static bool operator !=(DicomTag t1, DicomTag t2)
		{
			return !(t1 == t2);
		}

		/// <summary>
		/// Less than operator.
		/// </summary>
		public static bool operator <(DicomTag t1, DicomTag t2)
		{
			if ((object) t1 == null || (object) t2 == null)
				return false;
			if (t1.Group == t2.Group && t1.Element < t2.Element)
				return true;
			if (t1.Group < t2.Group)
				return true;
			return false;
		}

		/// <summary>
		/// Greater than operator.
		/// </summary>
		public static bool operator >(DicomTag t1, DicomTag t2)
		{
			return !(t1 < t2);
		}

		/// <summary>
		/// Less than or equal to operator.
		/// </summary>
		public static bool operator <=(DicomTag t1, DicomTag t2)
		{
			if ((object) t1 == null || (object) t2 == null)
				return false;
			if (t1.Group == t2.Group && t1.Element <= t2.Element)
				return true;
			if (t1.Group < t2.Group)
				return true;
			return false;
		}

		/// <summary>
		/// Greater than or equal to operator.
		/// </summary>
		public static bool operator >=(DicomTag t1, DicomTag t2)
		{
			if ((object) t1 == null || (object) t2 == null)
				return false;
			if (t1.Group == t2.Group && t1.Element >= t2.Element)
				return true;
			if (t1.Group > t2.Group)
				return true;
			return false;
		}

		#endregion

		#region Public Methds

		/// <summary>
		/// This method creates a <see cref="DicomAttribute"/> derived class for the tag.
		/// </summary>
		/// <returns></returns>
		public DicomAttribute CreateDicomAttribute()
		{
			return _vr.CreateDicomAttribute(this);
		}

		/// <summary>
		/// This method creates a <see cref="DicomAttribute"/> derived class for the tag, and 
		/// sets the intial value of the tag to the value contains in <paramref name="bb"/>.
		/// </summary>
		/// <param name="bb">A ByteBuffer containing an intial raw value for the tag.</param>
		/// <returns></returns>
		public DicomAttribute CreateDicomAttribute(ByteBuffer bb)
		{
			return _vr.CreateDicomAttribute(this, bb);
		}

		#endregion

		/// <summary>
		/// IComparable.CompareTo implementation, which compares <see cref="TagValue"/>.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>See <see cref="IComparable"/>.</returns>
		public int CompareTo(object obj)
		{
			DicomTag val = obj as DicomTag;
			if (val == null) return -1;

			return TagValue.CompareTo(val.TagValue);
		}
	}
}