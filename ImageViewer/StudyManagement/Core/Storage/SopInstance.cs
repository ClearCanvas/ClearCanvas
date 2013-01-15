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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    internal class SopInstance : ISopInstance
    {
		private readonly Series _parentSeries;
		private readonly InstanceXml _xml;
		private readonly DicomAttributeCollection _metaInfo;

		internal SopInstance(Series parentSeries, InstanceXml instanceXml)
		{
			_parentSeries = parentSeries;
			_xml = instanceXml;
			_metaInfo = new DicomAttributeCollection();

			if (instanceXml.TransferSyntax != null)
			{
				string transferSyntax = instanceXml.TransferSyntax.UidString;
				if (!String.IsNullOrEmpty(transferSyntax))
					_metaInfo[DicomTags.TransferSyntaxUid].SetString(0, transferSyntax);
			}

			if (instanceXml.SopClass != null)
			{
				string sopClass = instanceXml.SopClass.Uid;
				if (!String.IsNullOrEmpty(sopClass))
					_metaInfo[DicomTags.SopClassUid].SetString(0, sopClass);
			}
		}

		#region ISopInstance Members

		public ISeries GetParentSeries()
		{
			return _parentSeries;
		}

        public string FilePath
        {
            get { return _parentSeries.ParentStudy.StudyLocation.GetSopInstancePath(SeriesInstanceUid, SopInstanceUid); }
        }

		public string SpecificCharacterSet
		{
			get { return _xml[DicomTags.SpecificCharacterSet].ToString(); }
		}

		public string StudyInstanceUid
		{
			get { return _parentSeries.GetParentStudy().StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _parentSeries.SeriesInstanceUid; }
		}

		public string SopInstanceUid
		{
			get { return _xml.SopInstanceUid; }
		}

		public int InstanceNumber
		{
			get { return _xml[DicomTags.InstanceNumber].GetInt32(0, 0); }
		}

		public string SopClassUid
		{
			get
			{
				if (_xml.SopClass == null)
					return ""; //shouldn't happen.

				return _xml.SopClass.Uid;
			}
		}

		public string TransferSyntaxUid
		{
			get { return _xml.TransferSyntax.UidString; }
		}

        public int? BitsAllocated
        {
            get 
            { 
                int bitsAllocated;
                if (this[DicomTags.BitsAllocated].TryGetInt32(0, out bitsAllocated))
                    return bitsAllocated;

                return null;
            }
        }

        public int? BitsStored
        {
            get
            {
                int bitsStored;
                if (this[DicomTags.BitsStored].TryGetInt32(0, out bitsStored))
                    return bitsStored;

                return null;
            }
        }

        public string PhotometricInterpretation
        {
            get { return this[DicomTags.PhotometricInterpretation].GetString(0, null); }
        }

        public string SourceAETitle
        {
            get { return this[DicomTags.SourceApplicationEntityTitle].GetString(0, null); }
        }

        public bool IsStoredTag(uint tag)
		{
			DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);
			if (dicomTag == null)
				return false;

			return IsStoredTag(dicomTag);
		}

		public bool IsStoredTag(DicomTag tag)
		{
			Platform.CheckForNullReference(tag, "tag");

			if (_metaInfo.Contains(tag))
				return true;

			//if it's meta info, just defer to the file.
			if (tag.TagValue <= 0x0002FFFF)
				return false;

			if (_xml.IsTagExcluded(tag.TagValue))
				return false;

			if (tag.VR == DicomVr.SQvr)
			{
				var items = _xml[tag].Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem)item).HasExcludedTags(true))
								return false;
						}
					}
				}
			}

			bool isBinary = tag.VR == DicomVr.OBvr || tag.VR == DicomVr.OWvr || tag.VR == DicomVr.OFvr;
			//these tags are not stored in the xml.
			if (isBinary || tag.IsPrivate || tag.VR == DicomVr.UNvr)
				return false;

			return true;
		}

    	public DicomAttribute this[DicomTag tag]
    	{
			get
			{
				DicomAttribute attribute;
				if (_metaInfo.TryGetAttribute(tag, out attribute))
					return attribute;

				return _xml[tag];
			}
    	}

		public DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (_metaInfo.TryGetAttribute(tag, out attribute))
					return attribute;

				return _xml[tag];
			}
		}

		#endregion

        public ImageEntry ToStoreEntry()
        {
            var entry = new ImageEntry
                                   {
                                       Image = new ImageIdentifier(this)
                                                   {
                                                       InstanceAvailability = "ONLINE",
                                                       RetrieveAE = ServerDirectory.GetLocalServer(),
                                                       SpecificCharacterSet = SpecificCharacterSet
                                                   },
                                       Data = new ImageEntryData
                                                  {
                                                      SourceAETitle = SourceAETitle
                                                  }
                                   };
            return entry;
        }
    }
}
