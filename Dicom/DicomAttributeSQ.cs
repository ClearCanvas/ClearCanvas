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
    /// <see cref="DicomAttribute"/> derived class for storing DICOM SQ value representation attributes.
    /// </summary>
    public class DicomAttributeSQ : DicomAttribute
    {
        #region Private Variables
        private DicomSequenceItem[] _values;
        #endregion

        #region Constructors

        public DicomAttributeSQ(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSQ(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SQvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

		public DicomAttributeSQ(DicomTag tag, ByteBuffer bb)
			: base(tag)
		{
			if (!tag.VR.Equals(DicomVr.SQvr)
			 && !tag.MultiVR)
				throw new DicomException(SR.InvalidVR);

/* This doesn't work yet.  Need to implement a different way.
			DicomStreamReader reader = new DicomStreamReader(bb.Stream);
			reader.Dataset = ds;
			reader.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
			DicomReadStatus stat = reader.Read(new DicomTag(0xffffffff, "Test", "Test", DicomVr.UNvr, false, 1, 1, true), DicomReadOptions.Default);
			if (stat != DicomReadStatus.Success)
			{
				Platform.Log(LogLevel.Error, "Unexpected parsing error ({0}) when reading sequence attribute.", stat);
				throw new DicomDataException("Unexpected error parsing SQ attribute");
			}
*/
		}

        internal DicomAttributeSQ(DicomAttributeSQ attrib, bool copyBinary, bool copyPrivate, bool copyUnknown)
            : base(attrib)
        {
            DicomSequenceItem[] items = (DicomSequenceItem[])attrib.Values;

			if (items != null)
			{
				_values = new DicomSequenceItem[items.Length];
				for (int i = 0; i < items.Length; i++)
					_values[i] = (DicomSequenceItem) items[i].Copy(copyBinary, copyPrivate, copyUnknown);
			}
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="ClearCanvas.Dicom.DicomSequenceItem"/> at the specified index if it exists, otherwise returns null.
        /// </summary>
        /// <value></value>
        public DicomSequenceItem this[int index]
        {
            get
            {
                if (_values != null)
                {
                    if (_values.Length > index)
                        return _values[index];
                }
                return null;
            }
        }
        #endregion

        #region Public Methods

        public void ClearSequenceItems()
        {
            _values = null;
			base.StreamLength = 0;
        	base.Count = 0;
        }

        /// <summary>
        /// Method for adding a <see cref="DicomSequenceItem"/> to an attributes value.
        /// </summary>
        /// <param name="item">The <see cref="DicomSequenceItem"/> to add to the attribute.</param>
        /// <remarks>
        /// This method is value for <see cref="DicomAttributeSQ"/> attributes only.
        /// </remarks>
        public override void AddSequenceItem(DicomSequenceItem item)
        {
            if (_values == null)
            {
                _values = new DicomSequenceItem[1];
                _values[0] = item;
            }
            else
            {
                DicomSequenceItem[] oldValues = _values;

                _values = new DicomSequenceItem[oldValues.Length + 1];
                oldValues.CopyTo(_values, 0);
                _values[oldValues.Length] = item;
            }

            if (item.SpecificCharacterSet == null)
                item.SpecificCharacterSet = ParentCollection.SpecificCharacterSet;
            base.Count = _values.Length;
            base.StreamLength = (uint)base.Count;
        }

	    public override DicomSequenceItem GetSequenceItem(int i)
	    {
		    return this[i];
	    }

	    public override bool TryGetSequenceItem(int i, out DicomSequenceItem item)
	    {
		    item = this[i];
		    return item != null;
	    }

	    #endregion

        #region Abstract Method Implementation

        public override void SetNullValue()
        {
            _values = new DicomSequenceItem[0];
            base.StreamLength = 0;
            base.Count = 1;
        }

		public override void SetEmptyValue()
		{
			_values = null;
			base.StreamLength = 0;
			base.Count = 0;
		}

        public override string ToString()
        {
            return Tag;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            DicomAttributeSQ a = (DicomAttributeSQ)obj;
            DicomSequenceItem[] array = (DicomSequenceItem[])a.Values;

            if (Count != a.Count)
                return false;
            if (Count == 0 && a.Count == 0)
                return true;
			if (IsNull && a.IsNull)
				return true;

			if (_values.Length != array.Length)
				return false;

			for (int i = 0; i < _values.Length; i++)
                if (!array[i].Equals(_values[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (_values == null)
                return 0; // TODO

            return _values.GetHashCode();
        }

        public override Type GetValueType()
        {
            return typeof(DicomSequenceItem);
        }

        public override bool IsNull
        {
            get
            {
                if ((Count == 1) && (_values != null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if ((Count == 0) && (_values == null))
                    return true;
                return false;
            }
        }


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is DicomSequenceItem)
                {
                    _values = new DicomSequenceItem[1];
                    _values[0] = value as DicomSequenceItem;
                    base.Count = 1;
                }
                else if (value is DicomSequenceItem[])
                {
                    _values = (DicomSequenceItem[])value;
                    base.Count = _values.Length;
                    base.StreamLength = (uint)base.Count;
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSQ(this, true, true, true);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
        	return new DicomAttributeSQ(this, copyBinary, true, true);
        }

        public override void SetStringValue(String stringValue)
        {
            throw new DicomException("Function all incompatible with SQ VR type");
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            throw new DicomException("Unexpected call to GetByteBuffer() for a SQ attribute");
        }
        internal override uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 0;
            length += 4; // element tag
            if (syntax.ExplicitVr)
            {
                length += 2; // vr
                length += 6; // length
            }
            else
            {
                length += 4; // length
            }

            if (_values != null)
            {
                foreach (DicomSequenceItem item in _values)
                {
                    length += 4 + 4; // Sequence Item Tag
                    length += item.CalculateWriteLength(syntax, options & ~DicomWriteOptions.CalculateGroupLengths);
                    if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
                        length += 4 + 4; // Sequence Item Delimitation Item
                }
                if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
                    length += 4 + 4; // Sequence Delimitation Item
            }
            return length;
        }
        #endregion

        #region Dump
        public override void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            sb.Append(prefix);
        	sb.AppendFormat("({0:x4},{1:x4}) {2} {3}", Tag.Group, Tag.Element, Tag.VR.Name, Tag.Name);
            if (_values == null)
            {
                sb.AppendLine();
            }
            else
            {
                foreach (DicomSequenceItem item in _values)
                {
                    sb.AppendLine().Append(prefix).Append(" Item:").AppendLine();
                    item.Dump(sb, prefix + "  > ", options);
                    sb.Length = sb.Length - 1;
                }
            }
        }
        #endregion
    }
}
