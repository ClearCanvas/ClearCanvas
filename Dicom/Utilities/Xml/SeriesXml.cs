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
using System.Xml;

namespace ClearCanvas.Dicom.Utilities.Xml
{
    /// <summary>
    /// Class that can represent a series as XML.
    /// </summary>
    public class SeriesXml : IEnumerable<InstanceXml>
    {
        #region Private members

        private Dictionary<string, InstanceXml> _sourceImageList;

        private readonly Dictionary<string, InstanceXml> _instanceList = new Dictionary<string, InstanceXml>();
        private String _seriesInstanceUid = null;
        private BaseInstanceXml _seriesTagsStream = null;
        private bool _dirty = true; 

        #endregion

        #region Public Properties

        public String SeriesInstanceUid
        {
            get
            {
                if (_seriesInstanceUid == null)
                    return "";
                return _seriesInstanceUid;
            }
        }

    	public int NumberOfSeriesRelatedInstances
    	{
			get { return _instanceList.Count; }
    	}

        #endregion

        #region Constructors

        public SeriesXml(String seriesInstanceUid)
        {
            _seriesInstanceUid = seriesInstanceUid;
        }

        #endregion

        #region Public Methods

        public InstanceXml this[String sopInstanceUid]
        {
            get
            {
            	InstanceXml instance;
				if (!_instanceList.TryGetValue(sopInstanceUid, out instance))
					return null;

            	return instance;
            }
            set
            {
                if (value == null)
                    _instanceList.Remove(sopInstanceUid);
                else
                {
                    _instanceList[sopInstanceUid] = value;
                }

                _dirty = true;
            }
        }

        #endregion

        #region Internal Methods

        internal void CalculateBaseCollectionForSeries()
        {
            if (_instanceList.Count < 2)
                return;

            // Optimization:  a base collection has already been created, just return.
            if (_seriesTagsStream != null)
                return;

            IEnumerator<InstanceXml> iterator = GetEnumerator();

            if (false == iterator.MoveNext())
                return;

            DicomAttributeCollection collect1 = iterator.Current.Collection;

            if (false == iterator.MoveNext())
                return;

            DicomAttributeCollection collect2 = iterator.Current.Collection;

            _seriesTagsStream = new BaseInstanceXml(collect1, collect2);
        }

        internal XmlElement GetMemento(XmlDocument theDocument, StudyXmlOutputSettings settings)
        {
            _dirty = false;
            // Calc the base attributes
            CalculateBaseCollectionForSeries();

            XmlElement series = theDocument.CreateElement("Series");

            XmlAttribute seriesInstanceUid = theDocument.CreateAttribute("UID");
            seriesInstanceUid.Value = _seriesInstanceUid;
            series.Attributes.Append(seriesInstanceUid);

            XmlElement baseElement = theDocument.CreateElement("BaseInstance");

            // If there's only 1 total image in the series, leave an empty base instance
            // and just have the entire image be stored.
            if (_instanceList.Count > 1)
            {
                XmlElement baseInstance = _seriesTagsStream.GetMemento(theDocument, settings);
                baseElement.AppendChild(baseInstance);
            }
            else
            {
                _seriesTagsStream = null;
            }

            series.AppendChild(baseElement);

            foreach (InstanceXml instance in _instanceList.Values)
            {
                instance.SetBaseInstance(_seriesTagsStream);
                XmlElement instanceElement = instance.GetMemento(theDocument, settings);

                series.AppendChild(instanceElement);
            }

            return series;
        }

		internal StudyXmlMemento.StudyXmlNode GetMemento(StudyXmlMemento theDocument, StudyXmlOutputSettings settings)
		{
			_dirty = false;
			// Calc the base attributes
			CalculateBaseCollectionForSeries();

			var seriesElement = new StudyXmlMemento.StudyXmlNode {ElementName = "Series"};
			seriesElement.AddAttribute("UID",_seriesInstanceUid);


			// If there's only 1 total image in the series, leave an empty base instance
			// and just have the entire image be stored.
			if (_instanceList.Count > 1)
			{
				if (!string.IsNullOrEmpty(_seriesTagsStream.XmlFragment))
				{
					seriesElement.AddChild(new StudyXmlMemento.StudyXmlNode(_seriesTagsStream.XmlFragment));
				}
				else
				{
					XmlElement baseElement = theDocument.Document.CreateElement("BaseInstance");
					XmlElement baseInstance = _seriesTagsStream.GetMemento(theDocument.Document, settings);
					baseElement.AppendChild(baseInstance);

					var baseNode = new StudyXmlMemento.StudyXmlNode(baseElement);
					_seriesTagsStream.XmlFragment = baseNode.XmlElementFragment;
					seriesElement.AddChild(baseNode);
				}
			}
			else
			{
				_seriesTagsStream = null;
				seriesElement.AddChild(new StudyXmlMemento.StudyXmlNode("<BaseInstance/>"));
			}

			foreach (InstanceXml instance in _instanceList.Values)
			{
				instance.SetBaseInstance(_seriesTagsStream);

				if (!string.IsNullOrEmpty(instance.XmlFragment))
				{
					seriesElement.AddChild(new StudyXmlMemento.StudyXmlNode(instance.XmlFragment));
				}
				else
				{
					XmlElement instanceElement = instance.GetMemento(theDocument.Document, settings);

					var node = new StudyXmlMemento.StudyXmlNode(instanceElement);
					instance.XmlFragment = node.XmlElementFragment;
					seriesElement.AddChild(node);
				}
			}

			return seriesElement;
		}

        internal void SetMemento(XmlNode theSeriesNode)
        {
            _dirty = true;
            _seriesInstanceUid = theSeriesNode.Attributes["UID"].Value;

            if (!theSeriesNode.HasChildNodes)
                return;

            XmlNode childNode = theSeriesNode.FirstChild;

            while (childNode != null)
            {
                // Just search for the first study node, parse it, then break
                if (childNode.Name.Equals("BaseInstance"))
                {
                    if (childNode.HasChildNodes)
                    {
                        XmlNode instanceNode = childNode.FirstChild;
                        if (instanceNode.Name.Equals("Instance"))
                        {
                            _seriesTagsStream = new BaseInstanceXml(instanceNode);
                        }
                    }
                }
                else if (childNode.Name.Equals("Instance"))
                {
                    // This assumes the BaseInstance is in the xml ahead of the actual instances, note, however,
                    // that if there is only 1 instance in the series, there will be no base instance value

                    InstanceXml instanceStream;

                    if (_seriesTagsStream == null)
                        instanceStream = new InstanceXml(childNode, null);
                    else
						instanceStream = new InstanceXml(childNode, _seriesTagsStream.Collection);

                    _instanceList.Add(instanceStream.SopInstanceUid, instanceStream);
                }

                childNode = childNode.NextSibling;
            }
        }

        #endregion

        #region IEnumerator Implementation

        public IEnumerator<InstanceXml> GetEnumerator()
        {
            return _instanceList.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public InstanceXml FindSourceImageInstanceXml(string instanceUid)
        {
            BuildSourceImageMap();
            InstanceXml instanceXml;
            if (_sourceImageList.TryGetValue(instanceUid, out instanceXml))
            {
                return instanceXml;
            }
            else
            {
                return null;
            }
        }

        private void BuildSourceImageMap()
        {
            if (_dirty || _sourceImageList==null)
            {
                _sourceImageList = new Dictionary<string, InstanceXml>();
                foreach(InstanceXml instanceXml in _instanceList.Values)
                {
                    if (instanceXml.SourceImageInfoList!=null)
                    {
                       foreach(SourceImageInfo sourceInfo in instanceXml.SourceImageInfoList)
                       {
                           _sourceImageList.Add(sourceInfo.SopInstanceUid, instanceXml);
                       }
                    }
                }
            }
        }
    }
}