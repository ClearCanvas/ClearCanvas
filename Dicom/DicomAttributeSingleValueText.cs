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
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{

	#region DicomAttributeSingleValueText

	/// <summary>
	/// <see cref="DicomAttribute"/> derived class for storing single value text value representation attributes.
	/// </summary>
	public abstract class DicomAttributeSingleValueText : DicomAttribute
	{
		private String _value;

		#region Constructors

		internal DicomAttributeSingleValueText(uint tag)
			: base(tag) {}

		internal DicomAttributeSingleValueText(DicomTag tag)
			: base(tag) {}

		internal DicomAttributeSingleValueText(DicomTag tag, ByteBuffer item)
			: base(tag)
		{
			_value = item.GetString();

			// Saw some Osirix images that had padding on SH attributes with a null character, just
			// pull them out here.
			_value = _value.Trim(new char[] {tag.VR.PadChar, '\0'});

			Count = 1;
			StreamLength = (uint) _value.Length;
		}

		internal DicomAttributeSingleValueText(DicomAttributeSingleValueText attrib)
			: base(attrib)
		{
			var value = attrib.Values as string;
		    if (value != null)
		        _value = value;
		}

		#endregion

		#region Abstract Method Implementation

		public override void SetNullValue()
		{
			_value = "";
			base.StreamLength = 0;
			base.Count = 1;
		}

		public override void SetEmptyValue()
		{
			_value = null;
			base.StreamLength = 0;
			base.Count = 0;
		}

		/// <summary>
		/// The StreamLength of the attribute.
		/// </summary>
		public override uint StreamLength
		{
			get
			{
				if (IsNull || IsEmpty)
				{
					return 0;
				}

				if (ParentCollection != null && ParentCollection.SpecificCharacterSet != null)
				{
					return (uint) GetByteBuffer(TransferSyntax.ExplicitVrBigEndian, ParentCollection.SpecificCharacterSet).Length;
				}
				return base.StreamLength;
			}
		}

		public override bool TryGetString(int i, out String value)
		{
			if (i == 0)
			{
				value = _value;
				return true;
			}
			value = "";
			return false;
		}

		public override string ToString()
		{
			if (_value == null)
				return "";

			return _value;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;

			DicomAttribute a = (DicomAttribute) obj;
			return Equals(a.Values, _value);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public override Type GetValueType()
		{
			return typeof (string);
		}

		public override bool IsNull
		{
			get
			{
				if ((_value != null) && (_value.Length == 0))
					return true;
				return false;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				if ((Count == 0) && (_value == null))
					return true;
				return false;
			}
		}

		public override Object Values
		{
			get { return _value; }
			set
			{
				if (value is String)
				{
					SetStringValue((string) value);
				}
				else
				{
					throw new DicomException(SR.InvalidType);
				}
			}
		}

		public override void SetString(int index, string value)
		{
			if (index != 0)
				throw new DicomException(SR.InvalidType);
			SetStringValue(value);
		}

		public override void SetStringValue(String stringValue)
		{
			if (stringValue == null || stringValue.Length == 0)
			{
				Count = 1;
				StreamLength = 0;
				_value = "";
				return;
			}

			_value = stringValue;

			Count = 1;
			StreamLength = (uint) _value.Length;
		}

		public abstract override DicomAttribute Copy();
		internal abstract override DicomAttribute Copy(bool copyBinary);

		internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
		{
			ByteBuffer bb = new ByteBuffer(syntax.Endian);
			if (Tag.VR.SpecificCharacterSet)
				bb.SpecificCharacterSet = specificCharacterSet;

			//if (_value == null)
			//{
			//    return bb; // return empty buffer if the value is not set
			//}

			bb.SetString(_value, (byte) ' ');
			return bb;
		}

		#endregion
	}

	#endregion

	#region DicomAttributeLT

	/// <summary>
	/// <see cref="DicomAttributeSingleValueText"/> derived class for storing LT value representation attributes.
	/// </summary>
	public class DicomAttributeLT : DicomAttributeSingleValueText
	{
		#region Constructors

		public DicomAttributeLT(uint tag)
			: base(tag) {}

		public DicomAttributeLT(DicomTag tag)
			: base(tag)
		{
			if (!tag.VR.Equals(DicomVr.LTvr)
			    && !tag.MultiVR)
				throw new DicomException(SR.InvalidVR);
		}

		internal DicomAttributeLT(DicomTag tag, ByteBuffer item)
			: base(tag, item) {}

		internal DicomAttributeLT(DicomAttributeLT attrib)
			: base(attrib) {}

		#endregion

		public override DicomAttribute Copy()
		{
			return new DicomAttributeLT(this);
		}

		internal override DicomAttribute Copy(bool copyBinary)
		{
			return new DicomAttributeLT(this);
		}
	}

	#endregion

	#region DicomAttributeST

	/// <summary>
	/// <see cref="DicomAttributeSingleValueText"/> derived class for storing ST value representation attributes.
	/// </summary>
	public class DicomAttributeST : DicomAttributeSingleValueText
	{
		#region Constructors

		public DicomAttributeST(uint tag)
			: base(tag) {}

		public DicomAttributeST(DicomTag tag)
			: base(tag)
		{
			if (!tag.VR.Equals(DicomVr.STvr)
			    && !tag.MultiVR)
				throw new DicomException(SR.InvalidVR);
		}

		internal DicomAttributeST(DicomTag tag, ByteBuffer item)
			: base(tag, item) {}

		internal DicomAttributeST(DicomAttributeST attrib)
			: base(attrib) {}

		#endregion

		public override DicomAttribute Copy()
		{
			return new DicomAttributeST(this);
		}

		internal override DicomAttribute Copy(bool copyBinary)
		{
			return new DicomAttributeST(this);
		}
	}

	#endregion

	#region DicomAttributeUT

	/// <summary>
	/// <see cref="DicomAttributeSingleValueText"/> derived class for storing UT value representation attributes.
	/// </summary>
	public class DicomAttributeUT : DicomAttributeSingleValueText
	{
		#region Constructors

		public DicomAttributeUT(uint tag)
			: base(tag) {}

		public DicomAttributeUT(DicomTag tag)
			: base(tag)
		{
			if (!tag.VR.Equals(DicomVr.UTvr)
			    && !tag.MultiVR)
				throw new DicomException(SR.InvalidVR);
		}

		internal DicomAttributeUT(DicomTag tag, ByteBuffer item)
			: base(tag, item) {}

		internal DicomAttributeUT(DicomAttributeUT attrib)
			: base(attrib) {}

		#endregion

		public override DicomAttribute Copy()
		{
			return new DicomAttributeUT(this);
		}

		internal override DicomAttribute Copy(bool copyBinary)
		{
			return new DicomAttributeUT(this);
		}
	}

	#endregion
}