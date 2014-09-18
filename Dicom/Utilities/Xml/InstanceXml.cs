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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities.Xml
{
    public class SourceImageInfo
    {
        public string SopInstanceUid { get; set; }
    }

	/// <summary>
	/// Class for representing a SOP Instance as XML.
	/// </summary>
	/// <remarks>
	/// This class may change in a future release.
	/// </remarks>
	public class InstanceXml
	{
		#region Private members

		private readonly String _sopInstanceUid;
		private readonly SopClass _sopClass;
		private readonly TransferSyntax _transferSyntax;
        private string _sourceAETitle;
        private string _sourceFileName;
		private long _fileSize = 0;

		private BaseInstanceXml _baseInstance;
		private XmlElement _cachedElement;

		private DicomAttributeCollection _collection;

		private IEnumerator<DicomAttribute> _baseCollectionEnumerator;
		private IEnumerator _instanceXmlEnumerator;

	    private IList<SourceImageInfo> _sourceImageInfoList;
	    private bool _sourceImageInfoListLoaded;

	    public IList<SourceImageInfo> SourceImageInfoList
	    {
	        get
	        {
                if (!_sourceImageInfoListLoaded)
                {
                    _sourceImageInfoList = LoadSourceImageInfo(Collection);
                    _sourceImageInfoListLoaded = true;
                }
	            return _sourceImageInfoList;
	        } 
            private set
            {
                _sourceImageInfoList = value;
            }
	    }

		#endregion

		#region Public Properties

		public String SopInstanceUid
		{
			get
			{
				return _sopInstanceUid ?? "";
			}
		}

		public SopClass SopClass
		{
			get { return _sopClass; }
		}

		public string SourceFileName
		{
			get { return _sourceFileName; }
			internal set
			{
				if (value != null && Path.IsPathRooted(value))
					value = Path.GetFullPath(value);

				_sourceFileName = value;
			}
		}

	    public string SourceAETitle
	    {
            get { return _sourceAETitle; }
            set { _sourceAETitle = value; }
	    }

	    public long FileSize
		{
			get { return _fileSize; }
			set { _fileSize = value; }
		}

		public TransferSyntax TransferSyntax
		{
			get { return _transferSyntax; }
		}

		public string XmlFragment { get; set; }

		/// <summary>
		/// Gets the underlying data as a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <remarks>
		/// When parsed from xml, the return type is <see cref="InstanceXmlDicomAttributeCollection"/>, otherwise
		/// it is the source <see cref="DicomAttributeCollection"/>.
		/// </remarks>
		public DicomAttributeCollection Collection
		{
			get
			{
				ParseTo(0xffffffff);
				return _collection;
			}
		}

		public DicomAttribute this[DicomTag tag]
		{
			get
			{
				ParseTo(tag.TagValue);
				return _collection[tag.TagValue];
			}
		}

		public DicomAttribute this[uint tag]
		{
			get
			{
				ParseTo(tag);
				return _collection[tag];
			}
		}

		public bool IsTagExcluded(uint tag)
		{
			if (_collection is IInstanceXmlDicomAttributeCollection)
			{
				ParseTo(tag);
				return ((IInstanceXmlDicomAttributeCollection) _collection).IsTagExcluded(tag);
			}
			return false;
		}

		#endregion

		#region Constructors

	    public InstanceXml(DicomAttributeCollection collection, SopClass sopClass, TransferSyntax syntax)
		{
			_sopInstanceUid = collection[DicomTags.SopInstanceUid];

			_collection = collection;
			_sopClass = sopClass;
			_transferSyntax = syntax;

			_collection.ValidateVrValues = false;
			_collection.ValidateVrLengths = false;
		}

        private static IList<SourceImageInfo> LoadSourceImageInfo(DicomAttributeCollection collection)
	    {
	        if (collection.Contains(DicomTags.SourceImageSequence))
	        {
	            DicomAttributeSQ sq = collection[DicomTags.SourceImageSequence] as DicomAttributeSQ;
	            IList<SourceImageInfo> list = new List<SourceImageInfo>();
	            foreach(DicomSequenceItem item in sq.Values as DicomSequenceItem[])
	            {
                    list.Add(new SourceImageInfo()
	                                            {SopInstanceUid = item[DicomTags.ReferencedSopInstanceUid].ToString()});

	            }
                return list;
	        }

            return null;
	    }

	    public InstanceXml(XmlNode instanceNode, DicomAttributeCollection baseCollection)
		{
			InstanceXmlDicomAttributeCollection thisCollection = new InstanceXmlDicomAttributeCollection();
			_collection = thisCollection;
			_collection.ValidateVrValues = false;
			_collection.ValidateVrLengths = false;

			if (baseCollection != null)
			{
				AddExcludedTagsFromBase(baseCollection);

				_baseCollectionEnumerator = baseCollection.GetEnumerator();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

			if (!instanceNode.HasChildNodes)
				return;

			_instanceXmlEnumerator = instanceNode.ChildNodes.GetEnumerator();
			if (!_instanceXmlEnumerator.MoveNext())
				_instanceXmlEnumerator = null;

			if (instanceNode.Attributes["UID"] != null)
			{
				_sopInstanceUid = instanceNode.Attributes["UID"].Value;
			}

            if (instanceNode.Attributes["SourceAETitle"] != null)
            {
                _sourceAETitle = XmlUnescapeString(instanceNode.Attributes["SourceAETitle"].Value);
            }

			if (instanceNode.Attributes["SopClassUID"] != null)
			{
				_sopClass = SopClass.GetSopClass(instanceNode.Attributes["SopClassUID"].Value);
			}

			_transferSyntax = instanceNode.Attributes["TransferSyntaxUID"] != null 
				? TransferSyntax.GetTransferSyntax(instanceNode.Attributes["TransferSyntaxUID"].Value) 
				: TransferSyntax.ExplicitVrLittleEndian;

			if (instanceNode.Attributes["SourceFileName"] != null)
			{
				_sourceFileName = instanceNode.Attributes["SourceFileName"].Value;
			}

			if (instanceNode.Attributes["FileSize"] != null)
			{
				long.TryParse(instanceNode.Attributes["FileSize"].Value, out _fileSize);
			}

			// This should never happen
			if (_sopClass == null)
			{
				_sopClass = SopClass.GetSopClass(Collection[DicomTags.SopClassUid].GetString(0, String.Empty));
			}

		}

		#endregion

		#region Internal Methods

		internal XmlElement GetMemento(XmlDocument theDocument, StudyXmlOutputSettings settings)
		{
			if (_cachedElement != null)
			{
				return _cachedElement;
			}

			if (_baseInstance != null)
			{
				_cachedElement = GetMementoForCollection(theDocument, _baseInstance.Collection, Collection, settings);

				// Only keep around the cached xml data, free the collection to reduce memory usage
				SwitchToCachedXml();

				return _cachedElement;
			}

			_cachedElement = GetMementoForCollection(theDocument, null, Collection, settings);
			return _cachedElement;
		}

		internal void SetBaseInstance(BaseInstanceXml baseInstance)
		{
			if (_baseInstance != baseInstance)
			{
				// force the element to be regenerated when GetMemento() is called
				_cachedElement = null;
				XmlFragment = null;
			}

			_baseInstance = baseInstance;
		}

		#endregion


		#region Private Methods

		private void AddExcludedTagsFromBase(DicomAttributeCollection baseCollection)
		{
			if (baseCollection is IPrivateInstanceXmlDicomAttributeCollection)
			{
				if (_collection is IPrivateInstanceXmlDicomAttributeCollection)
				{
					((IPrivateInstanceXmlDicomAttributeCollection)_collection).ExcludedTagsHelper.Add(
						((IPrivateInstanceXmlDicomAttributeCollection)baseCollection).ExcludedTags);
				}
			}
		}

		private static StudyXmlTagInclusion AttributeShouldBeIncluded(DicomAttribute attribute, StudyXmlOutputSettings settings)
		{
			if (settings == null)
				return StudyXmlTagInclusion.IncludeTagValue;

			if (attribute is DicomAttributeSQ)
			{
				if (attribute.Tag.IsPrivate)
					return settings.IncludePrivateValues;
				return StudyXmlTagInclusion.IncludeTagValue;
			}


			// private tag
			if (attribute.Tag.IsPrivate)
			{
				if (settings.IncludePrivateValues != StudyXmlTagInclusion.IncludeTagValue)
					return settings.IncludePrivateValues;
			}

			// check type
			if (attribute is DicomAttributeUN)
			{
				if (settings.IncludeUnknownTags != StudyXmlTagInclusion.IncludeTagValue)
					return settings.IncludeUnknownTags;
			}

			// This check isn't needed, but it bypasses the StreamLength calculation if its not needed
			if (settings.IncludeLargeTags == StudyXmlTagInclusion.IncludeTagValue)
				return settings.IncludeLargeTags;

			// check the size
			ulong length = attribute.StreamLength;
			if (length <= settings.MaxTagLength)
				return StudyXmlTagInclusion.IncludeTagValue;

			// Move here, such that we first check if the tag should be excluded
			if ((attribute is DicomAttributeOB)
			 || (attribute is DicomAttributeOW)
			 || (attribute is DicomAttributeOF)
			 || (attribute is DicomAttributeOD)
			 || (attribute is DicomFragmentSequence))
				return StudyXmlTagInclusion.IncludeTagExclusion;

			return settings.IncludeLargeTags;
		}

		private void ParseTo(uint tag)
		{
			while (_baseCollectionEnumerator != null && _baseCollectionEnumerator.Current.Tag.TagValue <= tag)
			{
				_collection[_baseCollectionEnumerator.Current.Tag] = _baseCollectionEnumerator.Current.Copy();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

			while (_instanceXmlEnumerator != null)
			{
				XmlNode node = (XmlNode)_instanceXmlEnumerator.Current;
				String tagString = node.Attributes["Tag"].Value;
				uint tagValue;
				if (tagString.StartsWith("$"))
				{
					DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tagString.Substring(1));
					if (dicomTag == null) throw new DicomDataException("Invalid tag name when parsing XML: " + tagString);
					tagValue = dicomTag.TagValue;
				}
				else
				{
					tagValue = uint.Parse(tagString, NumberStyles.HexNumber);
				}

				if (tagValue <= tag)
				{
					ParseAttribute(_collection, node);
					if (!_instanceXmlEnumerator.MoveNext())
						_instanceXmlEnumerator = null;
				}
				else break;
			}
		}

		private static void ParseCollection(DicomAttributeCollection theCollection, XmlNode theNode)
		{
			XmlNode attributeNode = theNode.FirstChild;
			while (attributeNode != null)
			{
				ParseAttribute(theCollection, attributeNode);
				attributeNode = attributeNode.NextSibling;
			}
		}

		private static void ParseAttribute(DicomAttributeCollection theCollection, XmlNode attributeNode)
		{
			if (attributeNode.Name.Equals("Attribute"))
			{
				DicomTag theTag = GetTagFromAttributeNode(attributeNode);

				DicomAttribute attribute = theCollection[theTag];
				if (attribute is DicomAttributeSQ)
				{
					DicomAttributeSQ attribSQ = (DicomAttributeSQ)attribute;
					//set the null value in case there are no child nodes.
					attribute.SetNullValue();

					if (attributeNode.HasChildNodes)
					{
						XmlNode itemNode = attributeNode.FirstChild;

						while (itemNode != null)
						{
							DicomSequenceItem theItem = new InstanceXmlDicomSequenceItem();

							ParseCollection(theItem, itemNode);

							attribSQ.AddSequenceItem(theItem);

							itemNode = itemNode.NextSibling;
						}
					}
				}
				else
				{
					// Cleanup the common XML character replacements
					string tempString = attributeNode.InnerText;
					tempString = XmlUnescapeString(tempString);
					try
					{
						attribute.SetStringValue(tempString);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e,
									 "Unexpected exception with tag {0} setting value '{1}'.  Ignoring tag value.",
									 attribute.Tag,
									 tempString);
					}

					((IPrivateInstanceXmlDicomAttributeCollection)theCollection).ExcludedTagsHelper.Remove(theTag);
				}
			}
			else if (attributeNode.Name.Equals("EmptyAttribute"))
			{
				//Means that the tag should not be in this collection, but is in the base.  So, we remove it.
				DicomTag theTag = GetTagFromAttributeNode(attributeNode);

				//NOTE: we want these attributes to be in the collection and empty so that
				//the base collection doesn't need to be modified in order to insert
				//excluded tags in the correct order.
				theCollection[theTag].SetEmptyValue();

				((IPrivateInstanceXmlDicomAttributeCollection) theCollection).ExcludedTagsHelper.Remove(theTag);
			}
			else if (attributeNode.Name.Equals("ExcludedAttribute"))
			{
				DicomTag theTag = GetTagFromAttributeNode(attributeNode);

				//NOTE: we want these attributes to be in the collection and empty so that
				//the base collection doesn't need to be modified in order to insert
				//excluded tags in the correct order.
				theCollection[theTag].SetEmptyValue();

				((IPrivateInstanceXmlDicomAttributeCollection)theCollection).ExcludedTagsHelper.Add(theTag);
			}
		}

		private void SwitchToCachedXml()
		{
			// Give to the garbage collector the memory associated with the collection
			_collection = new InstanceXmlDicomAttributeCollection();
			_collection.ValidateVrValues = false;
			_collection.ValidateVrLengths = false;

			if (_baseInstance != null)
			{
				AddExcludedTagsFromBase(_baseInstance.Collection);

				_baseCollectionEnumerator = _baseInstance.Collection.GetEnumerator();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

			if (!_cachedElement.HasChildNodes)
				return;

			_instanceXmlEnumerator = _cachedElement.ChildNodes.GetEnumerator();
			if (!_instanceXmlEnumerator.MoveNext())
				_instanceXmlEnumerator = null;
		}

		private XmlElement GetMementoForCollection(XmlDocument theDocument, DicomAttributeCollection baseCollection,
												   DicomAttributeCollection collection, StudyXmlOutputSettings settings)
		{
			XmlElement instance;

			if (collection is DicomSequenceItem)
			{
				instance = theDocument.CreateElement("Item");
			}
			else
			{
				instance = theDocument.CreateElement("Instance");

				XmlAttribute sopInstanceUid = theDocument.CreateAttribute("UID");
				sopInstanceUid.Value = _sopInstanceUid;
				instance.Attributes.Append(sopInstanceUid);

				if (_sopClass != null)
				{
					XmlAttribute sopClassAttribute = theDocument.CreateAttribute("SopClassUID");
					sopClassAttribute.Value = _sopClass.Uid;
					instance.Attributes.Append(sopClassAttribute);
				}

				if (_transferSyntax != null && !(this is BaseInstanceXml))
				{
					XmlAttribute transferSyntaxAttribute = theDocument.CreateAttribute("TransferSyntaxUID");
					transferSyntaxAttribute.Value = _transferSyntax.UidString;
					instance.Attributes.Append(transferSyntaxAttribute);
				}

                if (_sourceAETitle != null)
                {
                    XmlAttribute sourceAEAttribute = theDocument.CreateAttribute("SourceAETitle");
                    sourceAEAttribute.Value = XmlEscapeString(_sourceAETitle);
                    instance.Attributes.Append(sourceAEAttribute);
                }
                
                if (_sourceFileName != null && settings.IncludeSourceFileName)
				{
					XmlAttribute sourceFileNameAttribute = theDocument.CreateAttribute("SourceFileName");
					string fileName = SecurityElement.Escape(_sourceFileName);
					sourceFileNameAttribute.Value = fileName;
					instance.Attributes.Append(sourceFileNameAttribute);
				}

				if (_fileSize != 0)
				{
					XmlAttribute fileSize = theDocument.CreateAttribute("FileSize");
					fileSize.Value = _fileSize.ToString();
					instance.Attributes.Append(fileSize);
				}
			}

			IPrivateInstanceXmlDicomAttributeCollection thisCollection = null;
			if (collection is IPrivateInstanceXmlDicomAttributeCollection)
			{
				thisCollection = (IPrivateInstanceXmlDicomAttributeCollection)collection;
				foreach (DicomTag tag in thisCollection.ExcludedTags)
				{
					//Artificially seed the collection with empty attributes from this
					//instance and the base instance so we can add them in the right order.
					//Note in the case of the base instance, this will never alter
					//the collection because the empty attribute is already there (see ParseAttribute).
					DicomAttribute attribute = collection[tag];
				}
			}

			IEnumerator<DicomAttribute> baseIterator = null;
			IPrivateInstanceXmlDicomAttributeCollection privateBaseCollection = null;
			if (baseCollection != null)
			{
				privateBaseCollection = baseCollection as IPrivateInstanceXmlDicomAttributeCollection;
				if (privateBaseCollection != null)
				{
					foreach (DicomTag tag in privateBaseCollection.ExcludedTags)
					{
						//Artificially seed the collection with empty attributes from this
						//instance and the base instance so we can add them in the right order.
						//Note in the case of the base instance, this will never alter
						//the collection because the empty attribute is already there (see ParseAttribute).
						DicomAttribute attribute = collection[tag];
					}
				}

				baseIterator = baseCollection.GetEnumerator();
				if (!baseIterator.MoveNext())
					baseIterator = null;
			}

			List<DicomTag> newlyExcludedTags = new List<DicomTag>();

			// pre-sort the list of excluded tags, so that finding an entry in the list is as fast as a binary search
			var thisCollectionExcludedTags = thisCollection != null ? thisCollection.ExcludedTags.Select(u => u.TagValue).OrderBy(u => u).ToList() : null;
			var privateBaseCollectionExcludedTags = privateBaseCollection != null ? privateBaseCollection.ExcludedTags.Select(u => u.TagValue).OrderBy(u => u).ToList() : null;

			foreach (DicomAttribute attribute in collection)
			{
				bool isExcludedFromThisCollection = thisCollectionExcludedTags != null &&
					thisCollectionExcludedTags.BinarySearch(attribute.Tag.TagValue) >= 0;

				bool isExcludedFromBase = privateBaseCollectionExcludedTags != null &&
					privateBaseCollectionExcludedTags.BinarySearch(attribute.Tag.TagValue) >= 0;

				bool isInBase = isExcludedFromBase;
				bool isSameAsInBase = isExcludedFromThisCollection && isExcludedFromBase;

				if (baseIterator != null)
				{
					while (baseIterator != null && baseIterator.Current.Tag < attribute.Tag)
					{
						if (!baseIterator.Current.IsEmpty)
						{
							XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, baseIterator.Current, "EmptyAttribute");
							instance.AppendChild(emptyAttributeElement);
						}

						if (!baseIterator.MoveNext())
							baseIterator = null;
					}

					if (baseIterator != null)
					{
						if (baseIterator.Current.Tag == attribute.Tag)
						{
							isInBase = !baseIterator.Current.IsEmpty || isExcludedFromBase;

							bool isEmpty = attribute.IsEmpty && !isExcludedFromThisCollection;
							bool isEmptyInBase = baseIterator.Current.IsEmpty && !isExcludedFromBase;

							isSameAsInBase = (isExcludedFromThisCollection && isExcludedFromBase)
							                 || (isEmpty && isEmptyInBase);

							if (!baseIterator.Current.IsEmpty && !isExcludedFromBase)
							{
								if (!(attribute is DicomAttributeOB)
								    && !(attribute is DicomAttributeOW)
								    && !(attribute is DicomAttributeOF)
								    && !(attribute is DicomAttributeOD)
								    && !(attribute is DicomFragmentSequence))
								{
									if (attribute.Equals(baseIterator.Current))
										isSameAsInBase = true;
								}
							}

							// Move to the next attribute for the next time through the loop
							if (!baseIterator.MoveNext())
								baseIterator = null;
						}
					}
				}
				
				//by this point, equality has been covered for both attributes with values, empty attributes and excluded attributes.
				if (isSameAsInBase)
					continue;

				if (isExcludedFromThisCollection)
				{
					XmlElement excludedAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "ExcludedAttribute");
					instance.AppendChild(excludedAttributeElement);
					continue;
				}

				if (attribute.IsEmpty)
				{
					//Only store an empty attribute if it is in the base (either non-empty or excluded).
					if (isInBase)
					{
						XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "EmptyAttribute");
						instance.AppendChild(emptyAttributeElement);
					}
					continue;
				}

				StudyXmlTagInclusion inclusion = AttributeShouldBeIncluded(attribute, settings);
				if (inclusion == StudyXmlTagInclusion.IncludeTagExclusion)
				{
					newlyExcludedTags.Add(attribute.Tag);
					if (!isExcludedFromBase)
					{
						XmlElement excludedAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "ExcludedAttribute");
						instance.AppendChild(excludedAttributeElement);
					}
					continue;
				}
				if (inclusion == StudyXmlTagInclusion.IgnoreTag)
				{
					continue;
				}

				XmlElement instanceElement = CreateDicomAttributeElement(theDocument, attribute, "Attribute");
				if (attribute is DicomAttributeSQ)
				{
					DicomSequenceItem[] items = (DicomSequenceItem[])attribute.Values;
					foreach (DicomSequenceItem item in items)
					{
						XmlElement itemElement = GetMementoForCollection(theDocument, null, item, settings);

						instanceElement.AppendChild(itemElement);
					}
				}
				else if (attribute is DicomAttributeOW || attribute is DicomAttributeUN)
				{
					byte[] val = (byte[])attribute.Values;

					StringBuilder str = null;
					foreach (byte i in val)
					{
						if (str == null)
							str = new StringBuilder(i.ToString());
						else
							str.AppendFormat("\\{0}", i);
					}
					if (str != null)
						instanceElement.InnerText = str.ToString();
				}
				else if (attribute is DicomAttributeOD)
				{
					double[] val = (double[])attribute.Values;
					StringBuilder str = null;
					foreach (double i in val)
					{
						if (str == null)
							str = new StringBuilder(i.ToString());
						else
							str.AppendFormat("\\{0}", i);
					}
					if (str != null)
						instanceElement.InnerText = str.ToString();
				}
				else if (attribute is DicomAttributeOF)
				{
					float[] val = (float[])attribute.Values;
					StringBuilder str = null;
					foreach (float i in val)
					{
						if (str == null)
							str = new StringBuilder(i.ToString());
						else
							str.AppendFormat("\\{0}", i);
					}
					if (str != null)
						instanceElement.InnerText = str.ToString();
				}
				else
				{
					instanceElement.InnerText = XmlEscapeString(attribute);
				}

				instance.AppendChild(instanceElement);
			}

			//fill in empty attributes past the end of this collection
			while(baseIterator != null)
			{
				if (!baseIterator.Current.IsEmpty)
				{
					XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, baseIterator.Current, "EmptyAttribute");
					instance.AppendChild(emptyAttributeElement);
				}

				if (!baseIterator.MoveNext())
					baseIterator = null;
			}

			if (thisCollection != null)
			{
				//when this is the base collection, 'thisCollection' will never be null.
				//when this is not the base collection, we switch to 'cached xml' right after this, anyway,
				//so the fact that this won't occur is ok.
				foreach (DicomTag tag in newlyExcludedTags)
				{
					collection[tag].SetEmptyValue();
					thisCollection.ExcludedTagsHelper.Add(tag);
				}
			}

			return instance;
		}

		private static DicomTag GetTagFromAttributeNode(XmlNode attributeNode)
		{
			String tag = attributeNode.Attributes["Tag"].Value;

			DicomTag theTag;
			if (tag.StartsWith("$"))
			{
				theTag = DicomTagDictionary.GetDicomTag(tag.Substring(1));
			}
			else
			{
				uint tagValue = uint.Parse(tag, NumberStyles.HexNumber);
				theTag = DicomTagDictionary.GetDicomTag(tagValue);
				DicomVr xmlVr = DicomVr.GetVR(attributeNode.Attributes["VR"].Value);
				if (theTag == null)
					theTag = new DicomTag(tagValue, "Unknown tag", "UnknownTag", xmlVr, false, 1, uint.MaxValue, false);

				if (!theTag.VR.Equals(xmlVr))
				{
					theTag = new DicomTag(tagValue, theTag.Name, theTag.VariableName, xmlVr, theTag.MultiVR, theTag.VMLow,
										  theTag.VMHigh, theTag.Retired);
				}
			}
			return theTag;
		}

		private static XmlElement CreateDicomAttributeElement(XmlDocument document, DicomAttribute attribute, string name)
		{
			return CreateDicomAttributeElement(document, attribute.Tag, name);
		}

		private static XmlElement CreateDicomAttributeElement(XmlDocument document, DicomTag dicomTag, string name)
		{
			XmlElement dicomAttributeElement = document.CreateElement(name);

			XmlAttribute tag = document.CreateAttribute("Tag");
			tag.Value = dicomTag.HexString;

			XmlAttribute vr = document.CreateAttribute("VR");
			vr.Value = dicomTag.VR.ToString();

			dicomAttributeElement.Attributes.Append(tag);
			dicomAttributeElement.Attributes.Append(vr);

			return dicomAttributeElement;
		}

		private static string XmlEscapeString(string input)
		{
			string result = input ?? string.Empty;

			result = SecurityElement.Escape(result);

			// Do the regular expression to escape out other invalid XML characters in the string not caught by the above.
			// NOTE: the \x sequences you see below are C# escapes, not Regex escapes
			result = Regex.Replace(result, "[^\x9\xA\xD\x20-\xFFFD]", m => string.Format("&#x{0:X};", (int) m.Value[0]));

			return result;
		}

        private static readonly Regex _unescapeRegex1 = new Regex("&#[Xx]([0-9A-Fa-f]+);", RegexOptions.Compiled);
        private static readonly Regex _unescapeRegex2 = new Regex("&#([0-9]+);", RegexOptions.Compiled);
		
        private static string XmlUnescapeString(string input)
		{
			string result = input ?? string.Empty;

			// unescape any value-encoded XML entities
            result = _unescapeRegex1.Replace(result, m => ((char)int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString());
            result = _unescapeRegex2.Replace(result, m => ((char)int.Parse(m.Groups[1].Value)).ToString());
			
            // unescape any entities encoded by SecurityElement.Escape (only <>'"&)
			result = result.Replace("&lt;", "<").
				Replace("&gt;", ">").
				Replace("&quot;", "\"").
				Replace("&apos;", "'").
				Replace("&amp;", "&");

			return result;
		}

#if UNIT_TESTS
		internal static string TestXmlEscapeString(string input)
		{
			return XmlEscapeString(input);
		}

		internal static string TestXmlUnescapeString(string input)
		{
			return XmlUnescapeString(input);
		}
#endif

		#endregion
	}
}
