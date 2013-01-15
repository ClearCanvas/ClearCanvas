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
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Data
{
	public class ImageSetField : IEquatable<ImageSetField>
	{
		private DicomTag _tag;
		private string _value;

		public DicomTag DicomTag
		{
			get { return _tag; }
			set
			{
				Debug.Assert(value != null);
				_tag = value;
			}
		}

		public string Tag
		{
			get { return _tag.HexString; }
			set
			{
				Debug.Assert(!String.IsNullOrEmpty(value));
				_tag = DicomTagDictionary.GetDicomTag(uint.Parse(value, NumberStyles.HexNumber));
			}
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ImageSetField() { }

		public ImageSetField(DicomAttribute attr)
		{
			_tag = attr.Tag;
			if (attr.IsEmpty || attr.IsNull)
				_value = String.Empty;
			else
			{
				_value = RemoveEscapeChars(attr.ToString());
			}
		}

		#region IEquatable<ImageSetField> Members

		public bool Equals(ImageSetField other)
		{
			return DicomTag.Equals(other.DicomTag) && _value.Equals(other.Value);
		}

		#endregion

		#region Private Methods
		private static string RemoveEscapeChars(string text)
		{
			if (String.IsNullOrEmpty(text))
				return text;

			// Remove escape characters
			string escape = String.Format("{0}", (char)0x1B);			
			text = text.Replace(escape, String.Empty);
			return text;
		}

		#endregion
	}

	/// <summary>
	/// Represents a serializable descriptor of an image set.
	/// </summary>
	[Serializable]
	[XmlRoot("ImageSetDescriptor")]
	public class ImageSetDescriptor : IEquatable<ImageSetDescriptor>, IXmlSerializable
	{
		#region Private Method
		private Dictionary<DicomTag, ImageSetField> _fields = new Dictionary<DicomTag, ImageSetField>();
		#endregion

		#region Constructors
		public ImageSetDescriptor()
		{
		}

		public ImageSetDescriptor(IDicomAttributeProvider attributeProvider)
		{
			PopulateField(DicomTags.PatientId, attributeProvider);
			PopulateField(DicomTags.IssuerOfPatientId, attributeProvider);
			PopulateField(DicomTags.PatientsName, attributeProvider);
			PopulateField(DicomTags.PatientsBirthDate, attributeProvider);
			PopulateField(DicomTags.PatientsSex, attributeProvider);
			PopulateField(DicomTags.AccessionNumber, attributeProvider);
			PopulateField(DicomTags.StudyDate, attributeProvider);
		}
		#endregion

		#region Private Methods
		private void PopulateField(uint tag, IDicomAttributeProvider attributeProvider)
		{
			DicomAttribute attr;
			if (attributeProvider.TryGetAttribute(tag, out attr))
			{
				AddField(new ImageSetField(attr));
			}
			else
			{
				// add default value
				AddField(new ImageSetField(DicomTagDictionary.GetDicomTag(tag).CreateDicomAttribute()));    
			}

		}

		#endregion

		#region Public Properties
		public ImageSetField[] Fields
		{
			get
			{
				ImageSetField[] array = new ImageSetField[_fields.Count];
				_fields.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				_fields = new Dictionary<DicomTag, ImageSetField>();
				foreach (ImageSetField field in value)
				{
					_fields.Add(field.DicomTag, field);
				}
			}
		}
		#endregion

		#region Indexers
		public ImageSetField this[DicomTag tag]
		{
			get
			{
				if (_fields.ContainsKey(tag))
					return _fields[tag];
				return null;
			}
		}

		public ImageSetField this[uint tag]
		{
			get
			{
                DicomTag theTag = DicomTagDictionary.GetDicomTag(tag);
                return this[theTag];
			}
		}
		#endregion

		#region Protected Methods
		protected void AddField(ImageSetField field)
		{
			_fields.Add(field.DicomTag, field);
		}
		#endregion

        #region Public Methods
        /// <summary>
        /// Retrieves the value for a Dicom tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(uint tag, out string value)
        {
            ImageSetField field = this[tag];

            if (field == null) // doesn't exist
            {
                value = null;
                return false;
            }
        	value = field.Value;
        	return true;
        } 
        #endregion

        #region Public Static Methods
        static public ImageSetDescriptor Parse(XmlElement element)
        {
            return XmlUtils.Deserialize<ImageSetDescriptor>(element);
        } 
        #endregion

		#region IEquatable<ImageSetDescriptor> Members

		public bool Equals(ImageSetDescriptor other)
		{
			if (this == other)
				return true;

			if (Fields.Length == 0 && other.Fields.Length > 0)
				return false;

			foreach (ImageSetField field in Fields)
			{
				if (other[field.DicomTag] == null || !field.Equals(other[field.DicomTag]))
					return false;
			}

			return true;
		}

		#endregion

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ReadXml(XmlReader reader)
		{
			// skip <ImageSetDescriptor>
			reader.Read();
            
			while (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Field")
			{
				ImageSetField field = new ImageSetField
				                      	{
				                      		Tag = reader["Tag"],
				                      		Value = String.IsNullOrEmpty(reader["Value"]) ? String.Empty : reader["Value"]
				                      	};
				AddField(field);
				reader.Read();
			}
        

			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Details")
			{
				//Details = XmlUtils.Deserialize<ImageSetDetails>(reader);
			}

			reader.Read();
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (ImageSetField field in _fields.Values)
			{
				writer.WriteStartElement("Field");
				writer.WriteAttributeString("Tag", field.Tag);
				writer.WriteAttributeString("Value", field.Value);
				writer.WriteEndElement();
			}

			//XmlUtils.Serialize(Details, writer);
		}

		#endregion



	}
}