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
using System.Globalization;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.Dicom
{
    #region DicomAttributeMultiValueText
    /// <summary>
    /// <see cref="DicomAttribute"/> derived class for storing character based DICOM value representation attributes with multiple values.
    /// </summary>
    public abstract class DicomAttributeMultiValueText : DicomAttribute
    {
        #region Private Members

        protected string[] _values = new string[0];

        #endregion

        #region Constructors

        internal DicomAttributeMultiValueText(uint tag)
            : base(tag)
        {

        }

        internal DicomAttributeMultiValueText(DicomTag tag)
            : base(tag)
        {

        }

        internal DicomAttributeMultiValueText(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            string valueArray;

            valueArray = item.GetString();

            // store the length before removing pad chars
            StreamLength = (uint) valueArray.Length;

            // Saw some Osirix images that had padding on SH attributes with a null character, just
            // pull them out here.
			// Leading and trailing space characters are non-significant in all multi-valued VRs,
			// so pull them out here as well since some devices seem to pad UI attributes with spaces too.
            valueArray = valueArray.Trim(new [] {tag.VR.PadChar, '\0', ' '});

            if (valueArray.Length == 0)
            {
                _values = new string[0];
                Count = 1;
                StreamLength = 0;
            }
            else
            {
                _values = valueArray.Split(new char[] {'\\'});

                Count = (long) _values.Length;
                StreamLength = (uint) valueArray.Length;
            }
        }

        internal DicomAttributeMultiValueText(DicomAttributeMultiValueText attrib)
            : base(attrib)
        {
            var values = (string[])attrib.Values;
            _values = new string[values.Length];
            for (int i = 0; i < values.Length; ++i)
                _values[i] = values[i];
        }

        #endregion

        #region Operators

        public string this[int val]
        {
            get
            {
                return _values[val];
            }
        }



        #endregion


        #region Abstract Method Implementation

        /// <summary>
        /// Validate a string to be used as an attribute value
        /// Throw DicomDataException if string cannot be used as a value for the attribute.
        /// </summary>
        /// <param name="value"></param>
        internal virtual void ValidateString(string value)
        {
            if (value == null)
                return;

            if (ValidateVrLengths || DicomImplementation.UnitTest)
            {
                if (Tag.VR.MaximumLength != 0)
                {
                    string[] valueArray = value.Trim().Split('\\');
                    foreach (string subVal in valueArray)
                    {
                        string trimmedVal = subVal.Trim();
						if (trimmedVal.Length > Tag.VR.MaximumLength)
						{
							// Handle case of DICOM range matching queries
							if ((Tag.VR.Equals(DicomVr.DAvr)
								|| Tag.VR.Equals(DicomVr.TMvr)
								|| Tag.VR.Equals(DicomVr.DTvr))
								&& trimmedVal.Contains("-"))
								return;
								
							throw new DicomDataException(
								string.Format("Invalid value length ({0}) for tag {1} of VR {2} of max size {3}", subVal, Tag, Tag.VR, Tag.VR.MaximumLength));
						}
                    }
                }
            }
        }

        public sealed override void SetNullValue()
        {
            _values = new string[0];
            base.StreamLength = 0;
            base.Count = 1;
        }

		public override void SetEmptyValue()
		{
			_values = new string[0];
			base.StreamLength = 0;
			base.Count = 0;
		}

    	/// <summary>
    	/// The number of values assigned to the attribute.
    	/// </summary>
    	public sealed override long Count {
			get { return base.Count; }
			protected set { base.Count = value; }
		}

    	/// <summary>
    	/// The length in bytes if the attribute was placed in a DICOM stream.
    	/// </summary>
    	public sealed override uint StreamLength
        {
            get
            {
                if (IsNull || IsEmpty)
                {
                    return 0;
                }
                    

                if (ParentCollection!=null && !string.IsNullOrEmpty(ParentCollection.SpecificCharacterSet))
                {
                    return (uint)GetByteBuffer(TransferSyntax.ExplicitVrBigEndian, ParentCollection.SpecificCharacterSet).Length;
                }
                return base.StreamLength;
            }
			protected set { base.StreamLength = value; }
        }

	    public override string ToString()
	    {
			if (_values == null)
				return string.Empty;

			if (_values.Length == 0) return string.Empty;

			return string.Join("\\", _values);
		}

	    public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            DicomAttributeMultiValueText a = (DicomAttributeMultiValueText)obj;

            // check if both values are null
            if (IsNull && a.IsNull)
                return true;

			if (_values.Length != a._values.Length)
				return false;

			for (int i = 0; i < _values.Length; i++)
			{
				if ((_values[i] == null && a._values[i] == null))
					continue;
				if ((_values[i] == null || a._values[i] == null))
					return false;
				if (!_values[i].Equals(a._values[i]))
					return false;
			}

        	return true;
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public sealed override Type GetValueType()
        {
            return typeof(string);
        }

        public sealed override bool IsNull
        {
            get
            {
                if ((Count == 1) && (_values != null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }

        public sealed override bool IsEmpty
        {
            get
            {
                if ((Count == 0) && (_values == null || _values.Length == 0))
                    return true;
                return false;
            }
        }


        public override object Values
        {
            get { return _values; }
            set
            {
                if (value is string[])
                {
                    _values = (string[])value;
                    // If the values array length is 0, then it is technically an empty string and we want Count to be 1
                    Count = _values.Length == 0 ? 1 : _values.Length;
                    StreamLength = (uint)ToString().Length;
                }
                else if (value is string)
                {
                    SetStringValue((string)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public abstract override DicomAttribute Copy();
        internal abstract override DicomAttribute Copy(bool copyBinary);

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, string specificCharacterSet)
        {
            var bb = new ByteBuffer(syntax.Endian);

            if (Tag.VR.SpecificCharacterSet)
                bb.SpecificCharacterSet = specificCharacterSet;

            bb.SetString(ToString(), (byte)' ');

            return bb;
        }



        /// <summary>
        /// Retrieve a value in the attribute 
        /// </summary>
        /// <param name="i">zero-based index of the value to retrieve</param>
        /// <param name="value">reference to the value retrieved</param>
        /// <returns><i>true</i> if the value can be retrieved. <i>false</i> if the element is not present (</returns>
        public override bool TryGetString(int i, out string value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = string.Empty;
                return false;
            }

            value = _values[i];
            return true;
        }

        /// <summary>
        /// Set the tag value(s) from a string. Values are separated by "\". Existing values, if any, will be overwritten.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <exception cref="DicomDataException">If <i>stringValue</i> cannot values that cannot be convert into the attribute VR</exception>
        /// <example>
        ///     DicomAttributeDT attr = new DicomAttributeDT(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
        ///     attr.SetStringValue("20001012122213.123456\\20001012120013.123456");
        /// 
        /// </example>
        ///
        public override void SetStringValue(string stringValue)
        {            
            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 1;
                StreamLength = 0;
                _values = new string[0];
                return;
            }

            ValidateString(stringValue);

            _values = stringValue.Split(new [] { '\\' });

            Count = _values.Length;

            StreamLength = (uint)stringValue.Length;
        }

        /// <summary>
        /// Set a value from a string. Existing value, if any, will be overwritten.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <exception cref="DicomDataException">If <i>stringValue</i> cannot values that cannot be convert into the attribute VR</exception>
        /// <remarks>If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly the same as <seealso cref="AppendString"/></remarks>
        /// <example>
        ///     DicomAttributeDT attr = new DicomAttributeDT(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
        ///     attr.SetString(0, "20001012122213.123456");
        /// 
        ///     DicomAttributeUS attrib = new DicomAttributeUS(DicomTagDictionary.GetDicomTag(DicomTags.SelectorUsValue));
        ///     attrib.SetString(0, "-1000"); // will throw DicomDataException (US VR can only hold value in the range 0..2^16
        /// 
        /// </example>
        ///
        public override void SetString(int index, string value)
        {
            ValidateString(value);

            if (index == Count || IsNull)
                AppendString(value);
            else
            {
                _values[index] = value;
			}
        }

        /// <summary>
        /// Append a value from a string.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>stringValue</i> cannot values that cannot be convert into the attribute VR</exception>
        /// <remarks>If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly the same as <seealso cref="AppendString"/></remarks>
        /// <example>
        ///     DicomAttributeDT attr = new DicomAttributeDT(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
        ///     attr.AppendString("20001012122213.123456");
        /// 
        ///     DicomAttributeUS attrib = new DicomAttributeUS(DicomTagDictionary.GetDicomTag(DicomTags.SelectorUsValue));
        ///     attrib.AppendString("-1000"); // will throw DicomDataException (US VR can only hold value in the range 0..2^16
        /// 
        /// </example>      
        public override void AppendString(string stringValue)
        {
            ValidateString(stringValue);

            //TODO: optimize the following code... there are a lot of redudant computations

            int newArrayLength = 1;
            int oldArrayLength = 0;

            if (_values != null)
            {
                newArrayLength = _values.Length + 1;
                oldArrayLength = _values.Length;
            }

            string[] newArray = new string[newArrayLength];
            if (oldArrayLength > 0)
                _values.CopyTo(newArray, 0);
            newArray[newArrayLength - 1] = stringValue;
            _values = newArray;

            StreamLength = (uint) ToString().Length;

            Count = newArray.Length;
        }
        #endregion

    }
    #endregion

    #region DicomAttributeAE
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing AE value representation attributes.
    /// </summary>
    public class DicomAttributeAE : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeAE(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeAE(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.AEvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeAE(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeAE(DicomAttributeAE attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation

        public override DicomAttribute Copy()
        {
            return new DicomAttributeAE(this);
        }
        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeAE(this);
        }

        #endregion

     
    }
    #endregion

    #region DicomAttributeAS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing AS value representation attributes.
    /// </summary>
    public class DicomAttributeAS : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeAS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeAS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ASvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeAS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeAS(DicomAttributeAS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeAS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeAS(this);
        }

    }
    #endregion

    #region DicomAttributeCS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing CS value representation attributes.
    /// </summary>
    public class DicomAttributeCS : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeCS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeCS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.CSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeCS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributeCS(DicomAttributeCS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeCS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeCS(this);
        }

    }
    #endregion

    #region DicomAttributeDA
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing DA value representation attributes.
    /// </summary>
    public class DicomAttributeDA : DicomAttributeMultiValueText
    {

        #region Constructors

        public DicomAttributeDA(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeDA(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DAvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }



        internal DicomAttributeDA(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeDA(DicomAttributeDA attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeDA(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeDA(this);
        }

        internal override void ValidateString(string value)
        {
            base.ValidateString(value);

            if (ValidateVrValues || DicomImplementation.UnitTest)
                DAStringValidator.ValidateString(Tag, value);
        }

        /// <summary>
        /// Method to retrieve a datetime value from a DA attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetDateTime(int i, out DateTime value)
        {
			if (_values == null || _values.Length <= i || i<0)
            {
                value = new DateTime();
                return false;
            }

            if (!DateParser.Parse(_values[i], out value))
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Set a DA value based on a datetime object. Existing value will be overwritten.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendDateTime"/>
        /// </remarks>
        public override void SetDateTime(int index, DateTime value)
        {
            if (index == _values.Length)
            {
                AppendDateTime(value);
            }
            else
            {
                _values[index] = value.ToString(DateParser.DicomDateFormat);
                StreamLength = (uint) ToString().Length;

            }
        }

        /// <summary>
        /// Append a DA value based on a datetime object.
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void AppendDateTime(DateTime value)
        {
			string[] temp = new string[_values.Length + 1];

            if (_values != null && _values.Length > 0)
            {
                Array.Copy(_values, temp, _values.Length);
            }

            _values = temp;
			_values[_values.Length-1] = value.ToString(DateParser.DicomDateFormat);
        	Count = _values.Length;
            StreamLength = (uint) ToString().Length;

        }

        
       
    }
    #endregion

    #region DicomAttributeDS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing DS value representation attributes.
    /// </summary>
    /// 
    public class DicomAttributeDS : DicomAttributeMultiValueText
    {
        protected NumberStyles _numberStyle = NumberStyles.Any;
        
        #region Constructors

        public DicomAttributeDS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeDS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeDS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeDS(DicomAttributeDS attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Properties
        public NumberStyles NumberStyle
        {
            get { return _numberStyle; }
            set { _numberStyle = value; }
        }

        
        #endregion

        internal override void ValidateString(string value)
        {
            base.ValidateString(value);

            if (ValidateVrValues || DicomImplementation.UnitTest)
                DSStringValidator.ValidateString(Tag, value);
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeDS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeDS(this);
        }


        /// <summary>
        /// Set an integer as a DS value. Existing value will be overwritten
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendInt16"/>
        /// </remarks>
        public override void SetInt16(int index, Int16 value)
        {
            SetInt64(index, value);
        }
        /// <summary>
        /// Set an integer as a DS value. Existing value will be overwritten
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendInt32"/>
        /// </remarks>
        public override void SetInt32(int index, Int32 value)
        {
            SetInt64(index, value);
        }

        /// <summary>
        /// Set an integer as a DS value. Existing value will be overwritten
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendInt64"/>
        /// </remarks>
        public override void SetInt64(int index, Int64 value)
        {

			if (index == _values.Length)
            {
                AppendInt64(value);
            }
            else
            {
				_values[index] = value.ToString(CultureInfo.InvariantCulture);
                StreamLength = (uint)ToString().Length;
            }
        }

        /// <summary>
        /// Set an integer as a DS value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUInt16"/>
        /// </remarks>
        public override void SetUInt16(int index, UInt16 value)
        {
            SetUInt64(index, value);
        }
        /// <summary>
        /// Set DS value from an interger
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUInt32"/>
        /// </remarks>
        public override void SetUInt32(int index, UInt32 value)
        {
            SetUInt64(index, value);
        }

        /// <summary>
        /// Set DS value from an interger
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUInt64"/>
        /// </remarks>
        public override void SetUInt64(int index, UInt64 value)
        {
			if (index == _values.Length)
            {
                AppendUInt64(value);
            }
            else
            {
				_values[index] = value.ToString(CultureInfo.InvariantCulture);
                StreamLength = (uint)ToString().Length;
            }
        }

        /// <summary>
        /// Set DS value from a floating-point number
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendFloat32"/>
        /// </remarks>
        public override void SetFloat32(int index, float value)
        {
        	// DO NOT CALL SetFloat64. Precision loss will occur when converting float to double

        	if (index == _values.Length)
        	{
        		AppendFloat32(value);
        	}
        	else
        	{
				_values[index] = value.ToString("G12", CultureInfo.InvariantCulture);
        		StreamLength = (uint) ToString().Length;
        	}
        }

        /// <summary>
        /// Set DS value from a floating-point number
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendFloat64"/>
        /// </remarks>
        public override void SetFloat64(int index, double value)
        {
			if (index == _values.Length)
            {
                AppendFloat64(value);
            }
            else
            {
				_values[index] = value.ToString("G12", CultureInfo.InvariantCulture);
                StreamLength = (uint)ToString().Length;
            }
        }

        /// <summary>
        /// Append a floating-point number as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// Since DS is actualy a string, <i>value</i> will be converted into string. Precision loss may occur during
        /// this conversion. That is, the value retrieved using Get methods may not produce exactly the same value. 
        /// </remarks>
        public override void AppendFloat32(float value)
        {
            // DO NOT CALL AppendFloat64. Precision loss will occur when converting float to double

			string[] temp = new string[_values.Length + 1];
            if (_values != null && _values.Length >= 1)
                Array.Copy(_values, temp, _values.Length);


            _values = temp;

			// Originallly had R here, but ran into problems with the attribute value being too large
			// for a DS tag (16 bytes)
			_values[_values.Length - 1] = value.ToString("G12", CultureInfo.InvariantCulture); 
            StreamLength = (uint)ToString().Length;
			Count = _values.Length;
        }

        /// <summary>
        /// Append a floating-point number as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// Since DS is actualy a string, <i>value</i> will be converted into string. Precision loss may occur during
        /// this conversion. That is, the value retrieved using Get methods may not produce exactly the same value. 
        /// </remarks>
        public override void AppendFloat64(double value)
        {
			string[] temp = new string[_values.Length + 1];
            if (_values != null && _values.Length >= 1)
                Array.Copy(_values, temp, _values.Length);


            _values = temp;

			// Originallly had R here, but ran into problems with the attribute value being too large
			// for a DS tag (16 bytes)
			_values[_values.Length - 1] = value.ToString("G12", CultureInfo.InvariantCulture); 
            StreamLength = (uint)ToString().Length;
			Count = _values.Length;
        }

        /// <summary>
        /// Append an integer as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendInt16(short value)
        {
            AppendInt64(value);
        }

        /// <summary>
        /// Append an integer as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendInt32(Int32 value)
        {
            AppendInt64(value);
        }

        /// <summary>
        /// Append an integer as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendInt64(long value)
        {
			string[] temp = new string[_values.Length + 1];

            if (_values != null && _values.Length >= 1)
                Array.Copy(_values, temp, _values.Length);

            _values = temp;

			_values[_values.Length - 1] = value.ToString(CultureInfo.InvariantCulture);
            StreamLength = (uint)ToString().Length;
			Count = _values.Length;

        }

        /// <summary>
        /// Append an integer as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendUInt16(ushort value)
        {
            AppendUInt64(value);
        }

        /// <summary>
        /// Append an integer as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendUInt32(uint value)
        {
            AppendUInt64(value);
        }

        /// <summary>
        /// Append an integer as a DS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendUInt64(ulong value)
        {
			string[] temp = new string[_values.Length + 1];

            if (_values!=null && _values.Length>=1)
                Array.Copy(_values, temp, _values.Length);

            _values = temp;

			_values[_values.Length - 1] = value.ToString(CultureInfo.InvariantCulture);
            StreamLength = (uint)ToString().Length;
			Count = _values.Length;

        }

        /// <summary>
        /// Method to retrieve an Int16 value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetInt16(int i, out Int16 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }
            if (!Int16.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Method to retrieve an Int32 value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetInt32(int i, out Int32 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            } 
            if (!Int32.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Method to retrieve an Int64 value from a DS attribute.
         /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// </remarks>
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            } 
            if (!Int64.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to retrieve an UInt16 value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetUInt16(int i, out UInt16 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            } 
            if (!UInt16.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Method to retrieve an UInt32 value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetUInt32(int i, out UInt32 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            } 
            if (!UInt32.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            } 
            if (!UInt64.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to retrieve a float value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value is too big to fit into an float (eg, 1.0E+100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetFloat32(int i, out float value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            } 
            if (!float.TryParse(_values[i], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Method to retrieve a double value from a DS attribute.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetFloat64(int i, out double value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0.0d;
                return false;
            }
			if (!double.TryParse(_values[i], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                value = 0;
                return false;
            }
            return true;
        }

        public override object Values
        {
            get
            {
                return base.Values;
            }
            set
            {
                // look for specific float or double, if not then pass on to base
                if (value != null && (value is float || value is double || value is int))
                {
                    if (value is float)
                        SetFloat32(0, (float)value);
                    if (value is double)
                        SetFloat64(0, (double)value);
                    if (value is int)
                        SetInt32(0, (int)value);
                    return;
                }
                base.Values = value;
            }
        }
    }
    #endregion

    #region DicomAttributeDT
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing DT value representation attributes.
    /// </summary>
    /// 
    public class DicomAttributeDT : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeDT(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeDT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DTvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeDT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeDT(DicomAttributeDT attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Protected members
        protected bool _useTimeZone = false;

        #endregion

        #region Public properties
        /// <summary>
        /// Indicate whether to use TimeZone when encoding the DT value. If set to <i>true</i>, the datetime passed in <seealso cref="SetDateTime"/> 
        /// and <seealso cref="AppendDateTime"/> will be converted into Universal datetime (ie, with zone offset)
        /// </summary>
        public bool UseTimeZone
        {
            set { _useTimeZone = value; }
            get { return _useTimeZone; }
        }
        #endregion

        internal override void ValidateString(string value)
        {
            base.ValidateString(value);

            if (ValidateVrValues || DicomImplementation.UnitTest)
                DTStringValidator.ValidateString(Tag, value);
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeDT(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeDT(this);
        }

        /// <summary>
        /// Method to retrieve a datetime value from a DT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetDateTime(int i, out DateTime value)
        {
			if (_values == null || _values.Length <= i || i < 0)
			{
                value = new DateTime();
                return false;
            }

            if (!DateTimeParser.Parse(_values[i], out value))
                return false;
            
            return true;
        }
        /// <summary>
        /// Set DT value from a datetime object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendDateTime"/>
        /// </remarks>
        public override void SetDateTime(int index, DateTime value)
        {
			if (index == _values.Length)
                AppendDateTime(value);
            else
            {
                _values[index] = DateTimeParser.ToDicomString(value, UseTimeZone);
                StreamLength = (uint)ToString().Length;
            }
        }
        /// <summary>
        /// Append a datetime object as a DT value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        public override void AppendDateTime(DateTime value)
        {
			string[] temp = new string[_values.Length + 1];
            if (_values != null && _values.Length > 0)
            {
                Array.Copy(_values, temp, _values.Length);
            }

            _values = temp;
			_values[_values.Length - 1] = DateTimeParser.ToDicomString(value, UseTimeZone);
        	Count = _values.Length;
            StreamLength = (uint)ToString().Length;
        }
    }
    #endregion

    #region DicomAttributeIS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing IS value representation attributes.
    /// </summary>
    public class DicomAttributeIS : DicomAttributeMultiValueText
    {
        protected NumberStyles _numberStyle = NumberStyles.Any;

        #region Constructors

        public DicomAttributeIS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeIS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ISvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeIS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeIS(DicomAttributeIS attrib)
            : base(attrib)
        {
        }

        #endregion

        internal override void ValidateString(string value)
        {
            base.ValidateString(value);

            if (ValidateVrValues || DicomImplementation.UnitTest)
                ISStringValidator.ValidateString(Tag, value);
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeIS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeIS(this);
        }

        
        /// <summary>
        /// Method to retrieve an Int16 value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int i, out short value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            return short.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
        /// <summary>
        /// Method to retrieve an Int32 value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int i, out int value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

			return int.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
        /// <summary>
        /// Method to retrieve an Int64 value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

			return Int64.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Method to retrieve an UInt16 value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetUInt16(int i, out ushort value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

			return ushort.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
        /// <summary>
        /// Method to retrieve an UInt32 value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int i, out uint value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

			return uint.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

			return UInt64.TryParse(_values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Method to retrieve a float value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value is too big to fit into an float (eg, 1.0E+100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat32(int i, out float value)
        {
            if (i < 0 || _values == null || _values.Length <= i )
            {
                value = 0.0f;
                return false;
            }

            return float.TryParse(_values[i], NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
        /// <summary>
        /// Method to retrieve a double value from an IS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat64(int i, out double value)
        {
            if (i < 0 || _values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

			return double.TryParse(_values[i], NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Set an integer as an IS value. Existing value will be overwritten
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendInt16"/>
        /// </remarks>
        /// 
        public override void SetInt16(int index, short value)
        {
            SetInt32(index, value);
        }
        /// <summary>
        /// Set an integer as an IS value. Existing value will be overwritten
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendInt32"/>
        /// </remarks>
        /// 
        public override void SetInt32(int index, int value)
        {
            SetInt64(index, value);
        }
        /// <summary>
        /// Set an integer as an IS value. Existing value will be overwritten
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendInt64"/>
        /// </remarks>
        /// 
        public override void SetInt64(int index, long value)
        {
			if (index == _values.Length)
			{
				AppendInt64(value);
			}
			else {
				_values[index] = value.ToString(CultureInfo.InvariantCulture);
				StreamLength = (uint)ToString().Length;
			}
        }

        /// <summary>
        /// Set an integer as an IS value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUInt16"/>
        /// </remarks>
        /// 
        public override void SetUInt16(int index, ushort value)
        {
            SetUInt32(index, value);
        }
        /// <summary>
        /// Set IS value from an interger
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUInt32"/>
        /// </remarks>
        /// 
        public override void SetUInt32(int index, uint value)
        {
            SetUInt64(index, value);
        }
        /// <summary>
        /// Set IS value from an interger
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUInt64"/>
        /// </remarks>
        /// 
        public override void SetUInt64(int index, ulong value)
        {
			if (index == _values.Length)
			{
				AppendUInt64(value);
			}
            else
            {
				_values[index] = value.ToString(CultureInfo.InvariantCulture);
                StreamLength = (uint)ToString().Length;
            }
        }

        /// <summary>
        /// Append an integer as an IS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        /// 
        public override void AppendInt16(short value)
        {
            AppendInt32(value);
        }
        /// <summary>
        /// Append an integer as an IS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        /// 
        public override void AppendInt32(int value)
        {
            AppendInt64(value);
        }
        /// <summary>
        /// Append an integer as an IS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        /// 
        public override void AppendInt64(long intValue)
        {
			string[] newArray = new string[_values.Length + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(newArray, 0);
            _values = newArray;

			_values[_values.Length - 1] = intValue.ToString(CultureInfo.InvariantCulture);
            StreamLength = (uint)ToString().Length;
        	Count = _values.Length;
        }

        /// <summary>
        /// Append an integer as an IS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        /// 
        public override void AppendUInt16(ushort value)
        {
            AppendUInt32(value);
        }
        /// <summary>
        /// Append an integer as an IS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        /// 
        public override void AppendUInt32(uint value)
        {
            AppendUInt64(value);
        }
        /// <summary>
        /// Append an integer as an IS value
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// </remarks>
        /// 
        public override void AppendUInt64(ulong value)
        {

			string[] temp = new string[_values.Length + 1];
            if (_values != null && _values.Length > 0)
                Array.Copy(_values, temp, _values.Length);
            _values = temp;
			_values[_values.Length - 1] = value.ToString(CultureInfo.InvariantCulture);
            StreamLength = (uint)ToString().Length;
        	Count = _values.Length;
        }

        public override object Values
        {
            get { return _values; }
            set
            {
                if (value == null)
                {
                    _values = null;
                    StreamLength = 0;
                    return;
                }

                if (value is Int16 || value is Int32 || value is Int64)
                {
                    // reset in case had more than 1 value
                    //_values = null or new string[0];
                    //StreamLength = 0;
                    if (value is Int16)
                        SetInt16(0, (Int16)value);
                    else if (value is Int32)
                        SetInt32(0, (Int32)value);
                    else if (value is Int64)
                        SetInt64(0, (Int64)value);
                }
                else
                {
                    base.Values = value;
                }

            }
        }
    }
    #endregion

    #region DicomAttributeLO
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing LO value representation attributes.
    /// </summary>
    public class DicomAttributeLO : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeLO(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeLO(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.LOvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeLO(DicomAttributeLO attrib)
            : base(attrib)
        {
        }

        internal DicomAttributeLO(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeLO(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeLO(this);
        }

    }
    #endregion

    #region DicomAttributePN
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing PN value representation attributes.
    /// </summary>
    public class DicomAttributePN : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributePN(uint tag)
            : base(tag)
        {

        }

        public DicomAttributePN(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.PNvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributePN(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributePN(DicomAttributePN attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributePN(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributePN(this);
        }

    }
    #endregion

    #region DicomAttributeSH
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing SH value representation attributes.
    /// </summary>
    public class DicomAttributeSH : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeSH(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSH(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SHvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSH(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributeSH(DicomAttributeSH attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSH(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSH(this);
        }

    }
    #endregion

    #region DicomAttributeTM
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing TM value representation attributes.
    /// </summary>
    public class DicomAttributeTM : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeTM(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeTM(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.TMvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeTM(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeTM(DicomAttributeTM attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Propperties
        #endregion


        internal override void ValidateString(string value)
        {
            base.ValidateString(value);

            if (ValidateVrValues || DicomImplementation.UnitTest)
                TMStringValidator.ValidateString(Tag, value);
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeTM(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeTM(this);
        }


        /// <summary>
        /// Method to retrieve a datetime value from a TM attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        public override bool TryGetDateTime(int i, out DateTime value)
        {
			if (_values == null || _values.Length <= i || i < 0)
			{
                value = new DateTime();
                return false;
            }
            if (!TimeParser.Parse(_values[i], out value))
                return false;
            return true;

        }

        /// <summary>
        /// Set a TM value based on a datetime object. Existing value will be overwritten.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendDateTime"/>
        /// </remarks>
        public override void SetDateTime(int index, DateTime value)
        {
			if (index == _values.Length)
			{
				AppendDateTime(value);
			}
            else
            {
                _values[index] = value.ToString(TimeParser.DicomFullTimeFormat);
                StreamLength = (uint)ToString().Length;
            }
        }

        /// <summary>
        /// Append a TM value based on a datetime object.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// 
        public override void AppendDateTime(DateTime value)
        {
			string[] temp = new string[_values.Length + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(temp, 0);

            _values = temp;
			_values[_values.Length-1] = value.ToString(TimeParser.DicomFullTimeFormat);
        	Count = _values.Length;
            StreamLength = (uint)ToString().Length;
        }

    }
    #endregion

    #region DicomAttributeUI
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing UI value representation attributes.
    /// </summary>
    public class DicomAttributeUI : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeUI(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUI(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.UIvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeUI(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeUI(DicomAttributeUI attrib)
            : base(attrib)
        {
        }

        #endregion

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, string specificCharacterSet)
        {
            var bb = new ByteBuffer(syntax.Endian);

            bb.SetString(ToString(), 0x00);

            return bb;
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUI(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUI(this);
        }

        /// <summary>
        /// Method to retrieve a DicomUI value from a UI attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value">DicomUI object encapsulating the UI value</param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// Returned DicomUI object may be different from the DicomUI that's set using <seealso cref="SetUid"/> or <seealso cref="AppendUid"/>
        /// Only the UID property of the DicomUI will be the same.
        /// 
        /// </remarks>
        public override bool TryGetUid(int i, out DicomUid value)
        {

            if (i < 0 || i >= Count || _values.Length == 0)
            {
                value = null;
                return false;
            }

            if (string.IsNullOrEmpty(_values[i]))
            {
                value = null;
                return true; // this is special case
            }

            SopClass sop = SopClass.GetSopClass(_values[i]);
            if (sop != null)
            {
                value = sop.DicomUid;
                return true;
            }

            TransferSyntax ts = TransferSyntax.GetTransferSyntax(_values[i]);
            if (ts != null)
            {
                value = ts.DicomUid;
                return true;
            }

            value = new DicomUid(_values[i], _values[i], UidType.Unknown);
            return true;
        }

        /// <summary>
        /// Set a UI attribute value base on the content of the DicomUid object.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfBoundException">If <i>index</i> is less than 0 or greater than <seealso cref="Count"/></exception>
        /// <remarks>
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves exactly as <seealso cref="AppendUid"/>
        /// </remarks>
        public override void SetUid(int index, DicomUid value)
        {
			if (index == _values.Length)
			{
				AppendUid(value);
			}
            else
            {
                _values[index] = value.UID;
                StreamLength = (uint)ToString().Length;
            }
        }

        /// <summary>
        /// Append a UI value based on a DicomUid object.
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void AppendUid(DicomUid uid)
        {
			string[] temp = new string[_values.Length + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(temp, 0);

            _values = temp;
			_values[_values.Length - 1] = uid.UID;
            StreamLength = (uint)ToString().Length;
        	Count = _values.Length;
        }
    }
    #endregion
}
