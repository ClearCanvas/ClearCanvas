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
using System.IO;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities.Xml
{
	/// <summary>
	/// Class that can represent a study as XML data.
	/// </summary>
	public class StudyXml : IEnumerable<SeriesXml>
	{
		#region Private members

		private readonly Dictionary<string, SeriesXml> _seriesList = new Dictionary<string, SeriesXml>();
		private String _studyInstanceUid = null;
		private XmlDocument _doc = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// Study Instance UID associated with this stream file.
		/// </summary>
		public String StudyInstanceUid
		{
			get
			{
				if (_studyInstanceUid == null)
					return string.Empty;
				return _studyInstanceUid;
			}
		}

		public String PatientsName
		{
			get
			{
				foreach (SeriesXml series in _seriesList.Values)
					foreach (InstanceXml instance in series)
					{
						DicomAttribute attrib;
						if (instance.Collection.TryGetAttribute(DicomTags.PatientsName, out attrib))
							return attrib.GetString(0, string.Empty);
					}
				return string.Empty;
			}
		}

		public String PatientId
		{
			get
			{
				foreach (SeriesXml series in _seriesList.Values)
					foreach (InstanceXml instance in series)
					{
						DicomAttribute attrib;
						if (instance.Collection.TryGetAttribute(DicomTags.PatientId, out attrib))
							return attrib.GetString(0, string.Empty);
					}
				return string.Empty;
			}
		}

		public int NumberOfStudyRelatedSeries
		{
			get { return _seriesList.Count; }
		}

		public int NumberOfStudyRelatedInstances
		{
			get
			{
				int total = 0;
				foreach (SeriesXml series in _seriesList.Values)
					total += series.NumberOfSeriesRelatedInstances;

				return total;
			}
		}

		#endregion

		#region Constructors

		public StudyXml(String studyInstanceUid)
		{
			_studyInstanceUid = studyInstanceUid;
		}

		public StudyXml() {}

		#endregion

		#region Public Methods

		/// <summary>
		/// Indexer to retrieve specific <see cref="SeriesXml"/> objects from the <see cref="StudyXml"/>.
		/// </summary>
		/// <param name="seriesInstanceUid"></param>
		/// <returns></returns>
		public SeriesXml this[String seriesInstanceUid]
		{
			get
			{
				SeriesXml series;
				if (!_seriesList.TryGetValue(seriesInstanceUid, out series))
					return null;

				return series;
			}
			set
			{
				if (value == null)
					_seriesList.Remove(seriesInstanceUid);
				else
				{
					_seriesList[seriesInstanceUid] = value;
				}
			}
		}

		/// <summary>
		/// Remove a specific file from the object.
		/// </summary>
		/// <param name="theFile"></param>
		/// <returns></returns>
		public bool RemoveFile(DicomFile theFile)
		{
			// Create a copy of the collection without pixel data
			DicomAttributeCollection data = theFile.DataSet;

			String studyInstanceUid = data[DicomTags.StudyInstanceUid];

			if (!_studyInstanceUid.Equals(studyInstanceUid))
				return false;

			String seriesInstanceUid = data[DicomTags.SeriesInstanceUid];
			String sopInstanceUid = data[DicomTags.SopInstanceUid];

			return RemoveInstance(seriesInstanceUid, sopInstanceUid);
		}

		/// <summary>
		/// Removes a series from the StudyXml.
		/// </summary>
		/// <param name="seriesInstanceUid">The Series Instance UID of the series to be removed.</param>
		/// <returns>true if the series is removed or does not exist.</returns>
		public bool RemoveSeries(string seriesInstanceUid)
		{
			if (Contains(seriesInstanceUid))
				return _seriesList.Remove(seriesInstanceUid);
			else
				return true; // treated as ok.
		}

		/// <summary>
		/// Remove a specific SOP instance from the StudyXml.
		/// </summary>
		/// <param name="seriesInstanceUid">The Series Instance Uid of the instance to be removed</param>
		/// <param name="sopInstanceUid">The SOP Instance Uid of the instance to be removed</param>
		/// <returns>true on SOP instance exists and is removed.</returns>
		public bool RemoveInstance(String seriesInstanceUid, String sopInstanceUid)
		{
			SeriesXml series = this[seriesInstanceUid];

			if (series == null)
				return false;

			InstanceXml instance = series[sopInstanceUid];
			if (instance == null)
				return false;

			// Setting the indexer to null removes the sop instance from the stream
			series[sopInstanceUid] = null;

			return true;
		}

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <returns>true on success.</returns>
		public bool AddFile(DicomFile theFile)
		{
			return AddFile(theFile, 0);
		}

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <param name="fileSize">The size in bytes of the file being added.</param>
		/// <returns>true on scuccess.</returns>
		public bool AddFile(DicomFile theFile, long fileSize)
		{
			return AddFile(theFile, fileSize, new StudyXmlOutputSettings());
		}

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <param name="fileName">The name of the file being added.</param>
		/// <returns>true on scuccess.</returns>
		public bool AddFile(DicomFile theFile, string fileName)
		{
			return AddFile(theFile, fileName, 0, new StudyXmlOutputSettings());
		}

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <param name="fileName">The name of the file being added.</param>
		/// <param name="fileSize">The size in bytes of the file being added.</param>
		/// <returns>true on scuccess.</returns>
		public bool AddFile(DicomFile theFile, string fileName, long fileSize)
		{
			return AddFile(theFile, fileName, fileSize, new StudyXmlOutputSettings());
		}

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <param name="fileSize">The size in bytes of the file being added.</param>
		/// <param name="settings">The settings used when writing out the file.</param>
		/// <returns>true on scuccess.</returns>
		public bool AddFile(DicomFile theFile, long fileSize, StudyXmlOutputSettings settings)
		{
			return AddFile(theFile, null, fileSize, settings);
		}

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <param name="fileName">The name of the file being added.</param>
		/// <param name="fileSize">The size in bytes of the file being added.</param>
		/// <param name="settings">The settings used when writing out the file.</param>
		/// <returns>true on scuccess.</returns>
		public bool AddFile(DicomFile theFile, string fileName, long fileSize, StudyXmlOutputSettings settings)
		{
			Platform.CheckForNullReference(settings, "settings");

			// Create a copy of the collection without pixel data
			DicomAttributeCollection data = new InstanceXmlDicomAttributeCollection(theFile.DataSet, true,
			                                                                        settings.IncludePrivateValues !=
			                                                                        StudyXmlTagInclusion.IgnoreTag,
			                                                                        settings.IncludeUnknownTags !=
			                                                                        StudyXmlTagInclusion.IgnoreTag,
			                                                                        DicomTags.PixelData);

			String studyInstanceUid = data[DicomTags.StudyInstanceUid];

			if (String.IsNullOrEmpty(_studyInstanceUid))
				_studyInstanceUid = studyInstanceUid;
			else if (!_studyInstanceUid.Equals(studyInstanceUid))
			{
				Platform.Log(LogLevel.Error,
				             "Attempting to add an instance to the stream where the study instance UIDs don't match for SOP: {0}",
				             theFile.MediaStorageSopInstanceUid);
				return false;
			}
			String seriesInstanceUid = data[DicomTags.SeriesInstanceUid];

			SeriesXml series = this[seriesInstanceUid];

			if (series == null)
			{
				series = new SeriesXml(seriesInstanceUid);
				this[seriesInstanceUid] = series;
			}

			String sopInstanceUid = data[DicomTags.SopInstanceUid];

			InstanceXml instance = series[sopInstanceUid];
			if (instance != null)
			{
				// Decided to remove this log as part of the Marmot development milestone.  Didn't seem like much value.
				//Platform.Log(LogLevel.Warn,
				//             "Attempting to add a duplicate SOP instance to the stream.  Replacing value: {0}",
				//             theFile.MediaStorageSopInstanceUid);
			}

			instance = new InstanceXml(data, theFile.SopClass, theFile.TransferSyntax);
			instance.SourceAETitle = theFile.SourceApplicationEntityTitle;
			instance.SourceFileName = !string.IsNullOrEmpty(fileName) ? fileName : theFile.Filename;
			instance.FileSize = fileSize;
			series[sopInstanceUid] = instance;

			return true;
		}

		/// <summary>
		/// Gets the total size of all instances in the study.
		/// </summary>
		/// <returns>
		/// Size of the study, in bytes.
		/// </returns>
		public long GetStudySize()
		{
			long size = 0;
			foreach (SeriesXml series in this)
			{
				foreach (InstanceXml instance in series)
					size += instance.FileSize;
			}

			return size;
		}

		/// <summary>
		/// Get an XML document representing the <see cref="StudyXml"/>.
		/// </summary>
		/// <remarks>
		/// This method can be called multiple times as DICOM SOP Instances are added
		/// to the <see cref="StudyXml"/>.  Note that caching is done of the 
		/// XmlDocument to improve performance.  If the collections in the InstanceStreams 
		/// are modified, the caching mechanism may cause the updates not to be contained
		/// in the generated XmlDocument.
		/// </remarks>
		/// <returns></returns>
		public StudyXmlMemento GetMemento(StudyXmlOutputSettings settings)
		{
			var memento = new StudyXmlMemento();
			if (_doc == null)
				_doc = new XmlDocument();
			else
			{
				_doc.RemoveAll();
			}
			memento.Document = _doc;

			if (settings.OptimizedMemento)
			{
				memento.RootNode = new StudyXmlMemento.StudyXmlNode
					{
						ElementName = "ClearCanvasStudyXml",
					};

				var studyElement = new StudyXmlMemento.StudyXmlNode
					{
						ElementName = "Study",
					};
				studyElement.AddAttribute("UID", _studyInstanceUid);

				memento.RootNode.AddChild(studyElement);

				foreach (SeriesXml series in this)
				{
					var seriesElement = series.GetMemento(memento, settings);

					studyElement.AddChild(seriesElement);
				}
			}
			else
			{
				XmlElement clearCanvas = _doc.CreateElement("ClearCanvasStudyXml");

				XmlElement study = _doc.CreateElement("Study");

				XmlAttribute studyInstanceUid = _doc.CreateAttribute("UID");
				studyInstanceUid.Value = _studyInstanceUid;
				study.Attributes.Append(studyInstanceUid);

				foreach (SeriesXml series in this)
				{
					XmlElement seriesElement = series.GetMemento(_doc, settings);

					study.AppendChild(seriesElement);
				}

				clearCanvas.AppendChild(study);
				_doc.AppendChild(clearCanvas);
			}

			return memento;
		}

		/// <summary>
		/// Populate this <see cref="StudyXml"/> object based on the supplied XML document.
		/// </summary>
		/// <param name="theMemento"></param>
		public void SetMemento(StudyXmlMemento theMemento)
		{
			// This should only happen in unit tests, but force the stream into a 
			// memory stream, and then load in the xml document from there.
			if (theMemento.RootNode != null)
			{
				using (var ms = new MemoryStream())
				{
					StudyXmlIo.Write(theMemento, ms);
					using (var ms2 = new MemoryStream(ms.ToArray()))
						theMemento.Document.Load(ms2);
				}
			}

			XmlDocument theDocument = theMemento.Document;

			if (!theDocument.HasChildNodes)
				return;

			// There should be one root node.
			XmlNode rootNode = theDocument.FirstChild;
			while (rootNode != null && !rootNode.Name.Equals("ClearCanvasStudyXml"))
				rootNode = rootNode.NextSibling;

			if (rootNode == null)
				return;

			XmlNode studyNode = rootNode.FirstChild;

			while (studyNode != null)
			{
				// Just search for the first study node, parse it, then break
				if (studyNode.Name.Equals("Study"))
				{
					_studyInstanceUid = studyNode.Attributes["UID"].Value;

					if (studyNode.HasChildNodes)
					{
						XmlNode seriesNode = studyNode.FirstChild;

						while (seriesNode != null)
						{
							String seriesInstanceUid = seriesNode.Attributes["UID"].Value;

							SeriesXml seriesStream = new SeriesXml(seriesInstanceUid);

							_seriesList.Add(seriesInstanceUid, seriesStream);

							seriesStream.SetMemento(seriesNode);

							// Go to next node in doc
							seriesNode = seriesNode.NextSibling;
						}
					}
				}
				studyNode = studyNode.NextSibling;
			}
		}

		#endregion

		#region IEnumerator Implementation

		public IEnumerator<SeriesXml> GetEnumerator()
		{
			return _seriesList.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public InstanceXml FindInstanceXml(string seriesUid, string instanceUid)
		{
			SeriesXml seriesXml = this[seriesUid];
			if (seriesXml == null)
				return null;

			return seriesXml[instanceUid];
		}

		/// <summary>
		/// Returns a boolean indicating whether the specified series exists in the study XML.
		/// </summary>
		/// <param name="seriesUid">The Series Instance UID of the series to check</param>
		/// <returns>True if the series exists in the study XML</returns>
		public bool Contains(string seriesUid)
		{
			return _seriesList.ContainsKey(seriesUid);
		}

		/// <summary>
		/// Returns a boolean indicating whether the specified SOP instance exists in the study XML.
		/// </summary>
		/// <param name="seriesUid">The Series Instance UID of the SOP instance to check</param>
		/// <param name="instanceUid">The SOP Instance UID of the SOP instance to check</param>
		/// <returns>True if the SOP instance exists in the study XML</returns>
		public bool Contains(string seriesUid, string instanceUid)
		{
			SeriesXml series = this[seriesUid];
			if (series == null)
			{
				return false;
			}

			return series[instanceUid] != null;
		}
	}
}