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
using JetBrains.Annotations;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Abstract class representing a DICOM attribute within an attribute collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The DicomAttribute class is a base class that represents a DICOM attribute.  A number of abstract methods are 
    /// defined.  Derived classes exist for each of the VR types.  In addition, the <see cref="DicomAttributeBinary"/>,
    /// <see cref="AttributeMultiValueText"/>, and <see cref="AttributeSingelValueText"/> classes contain generic
    /// implementations for binary VRs, text values that contain multiple values, and text VRs that contain a single
    /// value respectively.
    /// </para>
    /// </remarks>
    public abstract class DicomAttribute : IReadOnlyDicomAttribute
    {
        #region Private Members

    	private long _valueCount = 0;
        private uint _length = 0;
        private DicomAttributeCollection _parentCollection = null;
        #endregion

        #region Abstract and Virtual Methods
        /// <summary>
        /// Method to return a string representation of the attribute.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
        /// <summary>
        /// Method to check if two attributes are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the attributes are equal.</returns>
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();

    	public abstract bool IsNull { get; }
        public abstract bool IsEmpty { get; }
        public abstract Object Values { get; set; }
        public abstract DicomAttribute Copy();
        
        public abstract Type GetValueType();
        public abstract void SetNullValue();
		public abstract void SetEmptyValue();

        internal abstract ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet);
        internal abstract DicomAttribute Copy(bool copyBinary);

        /// <summary>
        /// Calculate the length to write the attribute.
        /// </summary>
        /// <param name="syntax">The transfer syntax to calculate the length for.</param>
        /// <param name="options">The write options to calculate the length for.</param>
        /// <returns></returns>
        internal virtual uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 4; // element tag
            if (syntax.ExplicitVr)
            {
                length += 2; // vr
                if (Tag.VR.Is16BitLengthField)
                    length += 2;
                else
                    length += 6;
            }
            else
            {
                length += 4; // length tag				
            }
            length += StreamLength;
            if ((length & 0x00000001) != 0)
                length++;

            return length;
        }

        /// <summary>
        /// Get the string representation of the value
        /// when the attribute is saved using the specific character set and transfer syntax 
        /// and read again using one of the GetString methods
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="specificCharacterSet"></param>
        /// <returns></returns>
        public virtual string GetEncodedString(TransferSyntax syntax, String specificCharacterSet)
        {
            var buffer = GetByteBuffer(syntax, specificCharacterSet);
            if (buffer == null)
                return null;

            return buffer.GetString();
        }

        public virtual void SetStringValue(String stringValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetString(int index, string value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetInt16(int index, Int16 value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetInt32(int index, Int32 value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetInt64(int index, Int64 value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetUInt16(int index, UInt16 value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetUInt32(int index, UInt32 value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetUInt64(int index, UInt64 value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetFloat32(int index, float value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetFloat64(int index, double value)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual void SetDateTime(int index, DateTime value)
        {
            throw new DicomException(SR.InvalidType);
        }

        /// <summary>
        /// Sets the date time with support for nullable datetime, calls the virtual <see cref="SetDateTime"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public virtual void SetDateTime(int index, DateTime? value)
        {
            if (value.HasValue)
                SetDateTime(index, value.Value);
            else
                SetNullValue();
        }

        public virtual void SetUid(int index, DicomUid value)
        {
            throw new DicomException(SR.InvalidType);
        }

        public virtual bool TryGetUInt16(int i, out ushort value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetInt16(int i, out Int16 value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetUInt32(int i, out UInt32 value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetInt32(int i, out Int32 value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetUInt64(int i, out UInt64 value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetInt64(int i, out Int64 value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetFloat32(int i, out float value)
        {
            value = 0.0f;
            return false;
        }
        public virtual bool TryGetFloat64(int i, out double value)
        {
            value = 0.0f;
            return false;
        }
        public virtual bool TryGetString(int i, out String value)
        {
            value = "";
            return false;
        }
        /// <summary>
        /// Method to retrieve a <see cref="DateTime"/> attribute for the tag.
        /// </summary>
        /// <param name="i">A zero index value to retrieve.</param>
        /// <param name="value"></param>
        /// <returns>true on success, false on failure.</returns>
        public virtual bool TryGetDateTime(int i, out DateTime value)
        {
            value = new DateTime();
            return false;
        }
        /// <summary>
        /// Retrieves a <see cref="DicomUid"/> instance for a value.
        /// </summary>
        /// <remarks>This function only works for <see cref="DicomAttributeUI"/> attributes.</remarks>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns>True on success, false on failure.</returns>
        public virtual bool TryGetUid(int i, out DicomUid value)
        {
            value = null;
            return false;
        }
        /// <summary>
        /// Append an Int16 value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendInt16(Int16 intValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an Int32 value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendInt32(Int32 intValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an Int64 value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendInt64(Int64 intValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an UInt16 value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendUInt16(UInt16 intValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an UInt32 value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendUInt32(UInt32 intValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an UInt64 value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendUInt64(UInt64 intValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an float value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendFloat32(float value)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an double value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendFloat64(double value)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an DateTime value to the tag.
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendDateTime(DateTime value)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an string value to the tag.
        /// <remarks>If the value cannot be converted into the underlying VR (eg, append("ABC") to an US tag), DicomDataException will be thrown</remarks>
        /// <remarks>If the value does not fit into or not compatible with the tag VR (eg, append(-1) to an US tag), DicomDataException will be thrown</remarks>
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendString(string stringValue)
        {
            throw new DicomException(SR.InvalidType);
        }
        /// <summary>
        /// Append an uid to the tag.
        /// </summary>
        /// <param name="intValue"></param>
        public virtual void AppendUid(DicomUid uid)
        {
            throw new DicomException(SR.InvalidType);
        }
        
        #region Value retrieval methods
        /// <summary>
        /// Retrieve a value as a UInt16.
        /// If the value doesn't exist or cannot be converted into UInt16, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual UInt16 GetUInt16(int i, UInt16 defaultVal)
        {
        	try
            {
            	UInt16 value;
            	bool ok = TryGetUInt16(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal; 
            }
        }
        /// <summary>
        /// Retrieve a value as a UInt32.
        /// If the value doesn't exist or cannot be converted into UInt32, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual UInt32 GetUInt32(int i, UInt32 defaultVal)
        {
        	try
            {
            	UInt32 value;
            	bool ok = TryGetUInt32(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        /// <summary>
        /// Retrieve a value as a UInt64.
        /// If the value doesn't exist or cannot be converted into UInt64, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual UInt64 GetUInt64(int i, UInt64 defaultVal)
        {
        	try
            {
            	UInt64 value;
            	bool ok = TryGetUInt64(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        /// <summary>
        /// Retrieve a value as a Int16.
        /// If the value doesn't exist or cannot be converted into Int16, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>        
        public virtual Int16 GetInt16(int i, Int16 defaultVal)
        {
            Int16 value;
            try
            {
                bool ok = TryGetInt16(i, out value);
                if (!ok)
                    return defaultVal;
            }
            catch (Exception)
            {
                return defaultVal;
            }
            return value;
        }
        /// <summary>
        /// Retrieve a value as a Int32.
        /// If the value doesn't exist or cannot be converted into Int32, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual Int32 GetInt32(int i, Int32 defaultVal)
        {
        	try
            {
            	Int32 value;
            	bool ok = TryGetInt32(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        /// <summary>
        /// Retrieve a value as an Int64.
        /// If the value doesn't exist or cannot be converted into Int64, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual Int64 GetInt64(int i, Int64 defaultVal)
        {
        	try
            {
            	Int64 value;
            	bool ok = TryGetInt64(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        /// <summary>
        /// Retrieve a value as a float.
        /// If the value doesn't exist or cannot be converted into float, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual float GetFloat32(int i, float defaultVal)
        {
        	try
            {
            	float value;
            	bool ok = TryGetFloat32(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        /// <summary>
        /// Retrieve a value as a double.
        /// If the value doesn't exist or cannot be converted into double, the <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual double GetFloat64(int i, double defaultVal)
        {
            double value;
            bool ok = TryGetFloat64(i, out value);
            if (!ok)
                return defaultVal;
            return value;
        }
        /// <summary>
        /// Retrieve a value as a string.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual String GetString(int i, String defaultVal)
        {
        	try
            {
            	String value;
            	bool ok = TryGetString(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        /// <summary>
        /// Retrieve a datetime value.
        /// If the value cannot be converted into a <see cref="DateTime"/> object, <i>defaultVal</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(int i, DateTime defaultVal)
        {
        	try
            {
            	DateTime value;
            	bool ok = TryGetDateTime(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// Retrieve a datetime value.
        /// If the value cannot be converted into a <see cref="DateTime"/> object, <i>null</i> will be returned.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public DateTime? GetDateTime(int i)
        {
            DateTime dateTime = GetDateTime(i, DateTime.MinValue);
            if (dateTime == DateTime.MinValue)
                return null;
            else
                return dateTime;
        }

        /// <summary>
        /// Retrieve an UID value. 
        /// 
        /// <see cref="DicomUid"/> 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public virtual DicomUid GetUid(int i, DicomUid defaultVal)
        {
        	try
            {
            	DicomUid value;
            	bool ok = TryGetUid(i, out value);
                if (!ok)
                    return defaultVal;
                return value;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }

	    #endregion

        /// <summary>
        /// Method for adding a <see cref="DicomSequenceItem"/> to an attributes value.
        /// </summary>
        /// <remarks>
        /// This method is value for <see cref="DicomAttributeSQ"/> attributes only.
        /// </remarks>
        /// <param name="item">The <see cref="DicomSequenceItem"/> to add to the attribute.</param>
        public virtual void AddSequenceItem(DicomSequenceItem item)
        {
            throw new DicomException(SR.InvalidType);
        }

		/// <summary>
		/// Gets the specified sequence item from the attribute (applicable only to SQ attributes).
		/// </summary>
		/// <param name="i">The index of the item to retrieve.</param>
		/// <returns>The sequence item, or null if there is no item at the specified index.</returns>
		/// <exception cref="DicomException">The attribute is not a sequence.</exception>
		[CanBeNull]
		public virtual DicomSequenceItem GetSequenceItem(int i)
	    {
			throw new DicomException(SR.InvalidType);
		}

		/// <summary>
		/// Gets the specified sequence item from the attribute, if it exists.
		/// </summary>
		/// <remarks>
		/// For non-sequence attributes, this method always returns false.
		/// </remarks>
		public virtual bool TryGetSequenceItem(int i, out DicomSequenceItem item)
		{
			item = null;
			return false;
		}

        
        #endregion

        #region Internal Properties
        /// <summary>
        /// The <see cref="DicomAttributeCollection"/> that the attribute is contained in.
        /// </summary>
        /// <remarks>
        /// This field is used to determine the Specific Character Set of a string attribute.
        /// </remarks>
        internal DicomAttributeCollection ParentCollection
        {
            get { return _parentCollection; }
            set { _parentCollection = value; }
        }

    	internal bool ValidateVrLengths
    	{
    		get
    		{
				if (ParentCollection == null)
					return DicomSettings.Default.ValidateVrLengths;

    			return ParentCollection.ValidateVrLengths;
    		}
    	}

		internal bool ValidateVrValues
		{
			get
			{
				if (ParentCollection == null)
					return DicomSettings.Default.ValidateVrValues;

				return ParentCollection.ValidateVrValues;
			}
		}
		
		#endregion


        #region Constructors
        /// <summary>
        /// Internal constructor when a <see cref="DicomTag"/> is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal DicomAttribute(DicomTag tag)
        {
			Tag = tag;
        }

        /// <summary>
        /// Internal constructor when a uint representation of the tag is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal DicomAttribute(uint tag)
        {
			Tag = DicomTagDictionary.GetDicomTag(tag);
        }

        /// <summary>
        /// Internal constructor used when copying an attribute from a pre-existing attribute instance.
        /// </summary>
        /// <param name="attrib">The attribute that is being copied.</param>
        internal DicomAttribute(DicomAttribute attrib)
        {
            Tag = attrib.Tag;
            _valueCount = attrib.Count;
            _length = attrib._length;
        }

        #endregion


		#region IReadOnlyDicomAttribute implementation

		DicomTag IReadOnlyDicomAttribute.Tag
		{
			get { return this.Tag; }
		}

		string IReadOnlyDicomAttribute.GetStringValue()
		{
			return ToString();
		}

	    IReadOnlyDicomSequenceItem IReadOnlyDicomAttribute.GetSequenceItem(int i)
	    {
		    return GetSequenceItem(i);
	    }

	    bool IReadOnlyDicomAttribute.TryGetSequenceItem(int i, out IReadOnlyDicomSequenceItem value)
	    {
		    DicomSequenceItem item;
		    if (TryGetSequenceItem(i, out item))
		    {
			    value = item;
			    return true;
		    }
		    value = null;
		    return false;
	    }

	    #endregion

		#region Public Properties

		/// <summary>
    	/// Retrieve <see cref="Tag"/> instance for the attribute.
    	/// </summary>
    	/// <remarks>
    	/// This was explicitly changed to a readonly member variable
    	/// from a property as a performance improvement.  This value
		/// is referenced frequently and the change results in a small
		/// performance improvement.  
		/// </remarks>
    	public readonly DicomTag Tag;

        public string DicomTagDescription
        {
            get
            {
                DicomTag dicomTag = DicomTagDictionary.GetDicomTag(Tag);
                if (dicomTag != null)
                    return dicomTag.Name;
                else
                    return String.Empty;
            }
        }
        /// <summary>
        /// The length in bytes if the attribute was placed in a DICOM stream.
        /// </summary>
        public virtual uint StreamLength
        {
            get
            {
                if ((_length & 0x00000001) == 1)
                    return _length + 1;

                return _length;
            }
            protected set { _length = value; }
        }

        /// <summary>
        /// The number of values assigned to the attribute.
        /// </summary>
        public virtual long Count
        {
            get { return _valueCount; }
            protected set { _valueCount = value; }
        }

        #endregion

        #region Operators
        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(DicomAttribute attr)
        {
            // Uses the actual ToString implementation of the derived class.
            return attr.ToString();
        }
        #endregion

        #region Dump
        /// <summary>
        /// Method for dumping the contents of the attribute to a string.
        /// </summary>
        /// <param name="sb">The StringBuilder to write the attribute to.</param>
        /// <param name="prefix">A prefix to place before the value.</param>
        /// <param name="options">The <see cref="DicomDumpOptions"/> to use for the output string.</param>
        public virtual void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            int ValueWidth = 40 - prefix.Length;
            int SbLength = sb.Length;

            sb.Append(prefix);
            sb.AppendFormat("({0:x4},{1:x4}) {2} ", Tag.Group, Tag.Element, Tag.VR.Name);
            if (Count == 0)
            {
                String value = "(no value available)";
                sb.Append(value.PadRight(ValueWidth, ' '));
            }
            else
            {
                if (Tag.VR.IsTextVR)
                {
                    String value;
                    if (Tag.VR == DicomVr.UIvr)
                    {
                        DicomAttributeUI ui = this as DicomAttributeUI;

                        DicomUid uid;
                        bool ok = ui.TryGetUid(0, out uid);

                        if (ok && uid.Type != UidType.Unknown)
                        {
                            value = "=" + uid.Description;
                            if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                            {
                                if (value.Length > ValueWidth)
                                {
                                    value = value.Substring(0, ValueWidth - 3);
                                }
                            }
                        }
                        else
                        {
                            value = "[" + ToString() + "]";
                            if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                            {
                                if (value.Length > ValueWidth)
                                {
                                    value = value.Substring(0, ValueWidth - 4) + "...]";
                                }
                            }
                        }
                    }
                    else
                    {
                        value = "[" + ToString() + "]";
                        if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                        {
                            if (value.Length > ValueWidth)
                            {
                                value = value.Substring(0, ValueWidth - 4) + "...]";
                            }
                        }
                    }
                    sb.Append(value.PadRight(ValueWidth, ' '));
                }
                else
                {
                    String value = ToString();
                    if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                    {
                        if (value.Length > ValueWidth)
                        {
                            value = value.Substring(0, ValueWidth - 3) + "...";
                        }
                    }
                    sb.Append(value.PadRight(ValueWidth, ' '));
                }
            }
            sb.AppendFormat(" # {0,4} {2} {1}", StreamLength, Tag.VM, Tag.Name.Replace("&apos;", "'"));

            if (Flags.IsSet(options, DicomDumpOptions.Restrict80CharactersPerLine))
            {
                if (sb.Length > (SbLength + 79))
                {
                    sb.Length = SbLength + 79;
                    //sb.Append(">");
                }
            }
        }
        #endregion
    }
}
