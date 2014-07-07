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
using System.Data;
using System.IO;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    /// <summary>
    /// A <see cref="DicomMessageSopDataSource"/> whose underlying data resides partially in
    /// <see cref="InstanceXml"/>, and partially elsewhere, such as a local file or WADO server.
    /// </summary>
    public abstract class XmlSopDataSource : DicomMessageSopDataSource
    {
        private readonly Dictionary<uint, bool> _sequenceHasExcludedTags = new Dictionary<uint, bool>();
        internal volatile bool _fullHeaderLoaded;

		protected XmlSopDataSource(InstanceXml instanceXml)
			: base(new DicomFile("", new DicomAttributeCollection{ValidateVrLengths = false, ValidateVrValues = false}, instanceXml.Collection))
        {
            //These don't get set properly for instance xml.
            var sourceFile = (DicomFile)SourceMessage;
            sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
            sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;
            sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0, instanceXml.SopClass == null
                                                                    ? instanceXml[DicomTags.SopClassUid].ToString()
                                                                    : instanceXml.SopClass.Uid);
        }

        /// <summary>
        /// Gets whether or not the full header has been loaded, as opposed to the initial
        /// <see cref="InstanceXml">xml</see> which may be partial.
        /// </summary>
        /// <remarks>
        /// The full header is loaded on-demand when <see cref="DicomAttribute"/>s are requested via
        /// the indexers or TryGetAttribute methods.
        /// </remarks>
        protected bool FullHeaderLoaded { get { return _fullHeaderLoaded; } }

        public override DicomAttribute this[DicomTag tag]
        {
            get
            {
                lock (SyncLock)
                {
                    DicomAttribute attribute;
                    if (TryGetXmlAttribute(tag.TagValue, out attribute))
                        return attribute;

                    return base[tag];
                }
            }
        }

        public override DicomAttribute this[uint tag]
        {
            get
            {
                lock (SyncLock)
                {
                    DicomAttribute attribute;
                    if (TryGetXmlAttribute(tag, out attribute))
                        return attribute;

                    return base[tag];
                }
            }
        }

        public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                return TryGetXmlAttribute(tag.TagValue, out attribute) || base.TryGetAttribute(tag, out attribute);
            }
        }

        public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                return TryGetXmlAttribute(tag, out attribute) || base.TryGetAttribute(tag, out attribute);
            }
        }

        private bool TryGetXmlAttribute(uint tag, out DicomAttribute attribute)
        {
            if (_fullHeaderLoaded)
            {
                attribute = null;
                return false;
            }

            var needFullHeader = false;
            var dataset = SourceMessage.DataSet;
            if (dataset.TryGetAttribute(tag, out attribute))
            {
                //If it's a sequence, it can have excluded tags in it.
                if (IsSequenceWithExcludedTags(attribute))
                    needFullHeader = true;
                else
                    return true; //We have all we need.
            }

            if (!needFullHeader)
            {
                // if it's a private tag and not already in the collection (which has been checked already), we MUST retrieve full header
                // early releases of the study XML functionality excluded private tags but also did not report their exclusion
                if (DicomTag.IsPrivateTag(tag) || ((InstanceXmlDicomAttributeCollection)dataset).IsTagExcluded(tag))
                    needFullHeader = true;
            }

            if (needFullHeader)
                LoadFullHeader();

            return false;
        }

        private bool IsSequenceWithExcludedTags(DicomAttribute attribute)
        {
            if (!(attribute is DicomAttributeSQ)) 
                return false;

            var tag = attribute.Tag.TagValue;
            // cache the results for the recursive SQ item excluded tags check - it adds up if you've got a multiframe image and functional group sequences get accessed repeatedly
            bool sequenceHasExcludedTags;
            if (!_sequenceHasExcludedTags.TryGetValue(tag, out sequenceHasExcludedTags))
            {
                var items = attribute.Values as DicomSequenceItem[];
                _sequenceHasExcludedTags[tag] = sequenceHasExcludedTags = (items != null && items.OfType<InstanceXmlDicomSequenceItem>().Any(item => item.HasExcludedTags(true)));
            }

            return sequenceHasExcludedTags;
        }

        protected void LoadFullHeader()
        {
            if (_fullHeaderLoaded) return;

            var fullHeader = GetFullHeader();
            if (fullHeader == null)
                throw new InvalidOperationException("GetFullHeader must return a valid DicomFile");

            SourceMessage = fullHeader;
            _fullHeaderLoaded = true;
        }

        protected abstract DicomFile GetFullHeader();
    }
}