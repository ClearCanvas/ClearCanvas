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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml.Study
{
	/// <summary>
	/// Represents an <see cref="ISopInstance"/> whose main source of data is a <see cref="StudyXml"/> document.
	/// </summary>
	//JR: made this class public so that Specifications using Jscript.NET can operate on it (JScript.NET can't see members on internal classes)
	public class SopInstance : ISopInstance
	{
		private readonly InstanceXml _xml;
		private readonly Series _parentSeries;
		private readonly IDicomFileLoader _dicomFileLoader;
		private readonly Dictionary<uint, bool> _sequenceHasExcludedTags;
		private DicomAttributeCollection _metaInfo;
		private DicomFile _fullHeader;


		internal SopInstance(InstanceXml xml, Series parent, IDicomFileLoader dicomFileLoader)
		{
			_xml = xml;
			_parentSeries = parent;
			_dicomFileLoader = dicomFileLoader;
			_sequenceHasExcludedTags = new Dictionary<uint, bool>();
			_metaInfo = new DicomAttributeCollection();
			if (xml.TransferSyntax != null)
			{
				var transferSyntax = xml.TransferSyntax.UidString;
				if (!String.IsNullOrEmpty(transferSyntax))
					_metaInfo[DicomTags.TransferSyntaxUid].SetString(0, transferSyntax);
			}

			if (xml.SopClass != null)
			{
				var sopClass = xml.SopClass.Uid;
				if (!String.IsNullOrEmpty(sopClass))
					_metaInfo[DicomTags.SopClassUid].SetString(0, sopClass);
			}
		}

		#region ISopInstanceData Members

		public string StudyInstanceUid
		{
			get { return _parentSeries.StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _parentSeries.SeriesInstanceUid; }
		}

		public string SopInstanceUid
		{
			get { return _xml.SopInstanceUid; }
		}

		public string SopClassUid
		{
			get { return _xml.SopClass.Uid; }
		}

		public int InstanceNumber
		{
			get { return DataSet[DicomTags.InstanceNumber].GetInt32(0, 0); }
		}

		#endregion

		#region ISopInstance Members

		ISeries ISopInstance.ParentSeries
		{
			get { return _parentSeries; }
		}

		public SopClass SopClass
		{
			get { return SopClass.GetSopClass(_xml.SopClass.Uid); }
		}

		public StorageInfo StorageInfo
		{
			get { return new StorageInfo(_xml.SourceFileName, _xml.FileSize, _xml.TransferSyntax); }
		}

		public string SourceApplicationEntityTitle
		{
			get { return GetAttribute(DicomTags.SourceApplicationEntityTitle).ToString(); }
		}

		public IReadOnlyDicomAttribute GetAttribute(uint dicomTag)
		{
			if (!IsStoredTag(dicomTag))
				LoadFullHeader();

			DicomAttribute attribute;
			if (MetaInfo.TryGetAttribute(dicomTag, out attribute))
				return attribute;

			if (_fullHeader != null)
				return _fullHeader.DataSet[dicomTag];

			return _xml[dicomTag];
		}

		public IReadOnlyDicomAttribute GetAttribute(DicomTag dicomTag)
		{
			return GetAttribute(dicomTag.TagValue);
		}

		//public DicomFile GetHeader(bool forceComplete)
		//{
		//    if (forceComplete)
		//        LoadFullHeader(); //Make sure the full header's loaded.

		//    //Return a copy of whatever we've got.
		//    return new DicomFile(null, MetaInfo.Copy(), DataSet.Copy());
		//}

		//public DicomFile GetCompleteSop()
		//{
		//    //Just get the entire thing from the loader.
		//    var args = new LoadDicomFileArgs(StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, true, true);
		//    return DicomFileLoader.LoadDicomFile(args);
		//}

		//public IFramePixelData GetFramePixelData(int frameNumber)
		//{
		//    var args = new LoadFramePixelDataArgs(this.StudyInstanceUid, this.SeriesInstanceUid, this.SopInstanceUid, frameNumber);
		//    return DicomFileLoader.LoadFramePixelData(args);
		//}

		#endregion

		private bool IsStoredTag(uint tag)
		{
			var dicomTag = DicomTagDictionary.GetDicomTag(tag);
			return dicomTag != null && IsStoredTag(dicomTag);
		}

		private bool IsStoredTag(DicomTag tag)
		{
			Platform.CheckForNullReference(tag, "tag");

			// if the full header has already been retrieved, all tags are considered "stored"
			if (_fullHeader != null)
				return true;

			if (_metaInfo.Contains(tag))
				return true;

			//if it's meta info, just defer to the file.
			if (tag.TagValue <= 0x0002FFFF)
				return false;

			if (_xml.IsTagExcluded(tag.TagValue))
				return false;

			if (tag.VR == DicomVr.SQvr)
			{
				// cache the results for the recursive SQ item excluded tags check - it adds up if you've got a multiframe image and functional group sequences get accessed repeatedly
				bool sequenceHasExcludedTags;
				if (!_sequenceHasExcludedTags.TryGetValue(tag.TagValue, out sequenceHasExcludedTags))
				{
					var items = _xml[tag].Values as DicomSequenceItem[];
					_sequenceHasExcludedTags[tag.TagValue] = sequenceHasExcludedTags = (items != null && items.OfType<InstanceXmlDicomSequenceItem>().Any(item => item.HasExcludedTags(true)));
				}

				if (sequenceHasExcludedTags)
					return false;
			}

			var isBinary = tag.VR == DicomVr.OBvr || tag.VR == DicomVr.OWvr || tag.VR == DicomVr.OFvr;

			//these tags are not usually stored in the xml.
			if (isBinary || tag.IsPrivate || tag.VR == DicomVr.UNvr)
				return false;

			return true;
		}

		private void LoadFullHeader()
		{
			if (_fullHeader != null)
				return; //We've already got it without the pixel data.

			var args = new LoadDicomFileArgs(this.StudyInstanceUid, this.SeriesInstanceUid, this.SopInstanceUid, true, false);
			_fullHeader = _dicomFileLoader.LoadDicomFile(args);
			_metaInfo = null;
		}

		private DicomAttributeCollection MetaInfo
		{
			get { return _fullHeader != null ? _fullHeader.MetaInfo : _metaInfo; }
		}

		private DicomAttributeCollection DataSet
		{
			get { return _fullHeader != null ? _fullHeader.DataSet : _xml.Collection; }
		}
	}
}