#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using System.Linq;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml
{
    /// <summary>
    /// Represents an <see cref="ISopInstance"/> whose main source of data is a <see cref="StudyXml"/> document.
    /// </summary>
    internal class SopInstance : ISopInstance
    {
        private DicomAttributeCollection _metaInfo;
        private readonly InstanceXml _xml;
        private DicomFile _fullHeader;

        public SopInstance(InstanceXml xml, Series parent)
        {
            _xml = xml;
            ParentSeries = parent;

            _metaInfo = new DicomAttributeCollection();

            if (xml.TransferSyntax != null)
            {
                string transferSyntax = xml.TransferSyntax.UidString;
                if (!String.IsNullOrEmpty(transferSyntax))
                    _metaInfo[DicomTags.TransferSyntaxUid].SetString(0, transferSyntax);
            }

            if (xml.SopClass != null)
            {
                string sopClass = xml.SopClass.Uid;
                if (!String.IsNullOrEmpty(sopClass))
                    _metaInfo[DicomTags.SopClassUid].SetString(0, sopClass);
            }
        }

        internal Series ParentSeries { get; private set; }

        ISeries ISopInstance.ParentSeries { get { return ParentSeries; } }

        private bool IsStoredTag(uint tag)
        {
            DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);
            if (dicomTag == null)
                return false;

            return IsStoredTag(dicomTag);
        }

        private bool IsStoredTag(DicomTag tag)
        {
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
                    if (items.OfType<InstanceXmlDicomSequenceItem>().Any(item => item.HasExcludedTags(true)))
                        return false;
                }
            }

            bool isBinary = tag.VR == DicomVr.OBvr || tag.VR == DicomVr.OWvr || tag.VR == DicomVr.OFvr;
            //these tags are not stored in the xml.
            if (isBinary || tag.IsPrivate || tag.VR == DicomVr.UNvr)
                return false;

            return true;
        }

        private void LoadFullHeader(bool includePixelData)
        {
            if (_fullHeader != null)
                return;

            var args = new LoadDicomFileArgs(this.StudyInstanceUid, this.SeriesInstanceUid, this.SopInstanceUid, true, includePixelData);
            _fullHeader = ParentSeries.ParentStudy.HeaderProvider.LoadDicomFile(args);
            _metaInfo = null;
        }

        private DicomAttributeCollection MetaInfo
        {
            get
            {
                return _fullHeader != null ? _fullHeader.MetaInfo : _metaInfo;
            }
        }
        
        private DicomAttributeCollection DataSet
        {
            get
            {
                return _fullHeader != null ? _fullHeader.DataSet : _xml.Collection;
            }
        }

        #region ISopInstanceData Members

        public string StudyInstanceUid
        {
            get { return ParentSeries.StudyInstanceUid; }
        }

        public string SeriesInstanceUid
        {
            get { return ParentSeries.SeriesInstanceUid; }
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

        public DicomAttribute GetAttribute(uint dicomTag)
        {
            if (!IsStoredTag(dicomTag))
                LoadFullHeader(false);

            DicomAttribute attribute;
            if (MetaInfo.TryGetAttribute(dicomTag, out attribute))
                return attribute;

            if (_fullHeader != null)
                return _fullHeader.DataSet[dicomTag];

            return _xml[dicomTag];
        }

        public DicomAttribute GetAttribute(DicomTag dicomTag)
        {
            if (!IsStoredTag(dicomTag))
                LoadFullHeader(false);

            DicomAttribute attribute;
            if (MetaInfo.TryGetAttribute(dicomTag, out attribute))
                return attribute;

            if (_fullHeader != null)
                return _fullHeader.DataSet[dicomTag];

            return _xml[dicomTag];
        }

        #endregion
        #region IHeaderProvider Members

        public bool CanLoadCompleteHeader
        {
            get { return ParentSeries.ParentStudy.HeaderProvider.CanLoadCompleteHeader; }
        }

        public bool CanLoadPixelData
        {
            get { return ParentSeries.ParentStudy.HeaderProvider.CanLoadPixelData; }
        }

        public DicomFile LoadDicomFile(LoadSopDicomFileArgs args)
        {
            if (args.ForceCompleteHeader)
                LoadFullHeader(args.IncludePixelData);

            return new DicomFile(null, MetaInfo.Copy(), DataSet.Copy());
        }

        #endregion
    }
}
